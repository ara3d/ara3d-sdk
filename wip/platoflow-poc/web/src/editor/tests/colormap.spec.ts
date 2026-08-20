// T5 specs: the colormap gradient body (editor/colormap.ts).
// Pure halves — ramp sampling, label formatting, body geometry — plus a
// headless drive half that mounts the REAL body parts on a headless Gratify
// runtime with the REAL update (reducer + undo history) and clicks them.
import { describe, expect, it } from "vitest";
import { Free, Runtime, rect, v, walk, type Instance, type Rect } from "gratify";
import type { GraphDoc } from "../../contracts";
import { kindInfo } from "../../kinds";
import { initialDoc, makeUpdate, type EditorDoc, type EditorIntent } from "../doc";
import { nodeLayout, wiredInputsOf } from "../geom";
import {
  RAMP_CYCLE, cmapBodyLayout, cmapSliderDomain, cmapSnapStep, cmapState, colormapBody,
  fmtDomain, nextRamp, rampStops, sliderDragMax, sliderDragMin, sliderFrac,
  sliderShiftWindow, sliderValue, sliderZone,
} from "../colormap";
import { createDrive } from "./drive";

const close = (a: number, b: number) => Math.abs(a - b) < 1e-9;

// ── ramp sampling ────────────────────────────────────────────────────────────

describe("colormap ramp sampling", () => {
  it("cycle comes from the kinds.ts enum schema", () => {
    const s = kindInfo("viz.colormap")!.params.ramp;
    expect(s.kind).toBe("enum");
    expect(RAMP_CYCLE).toEqual((s as { options: string[] }).options);
    expect(RAMP_CYCLE).toEqual(["viridis", "heat", "greenred"]);
  });

  it("nextRamp cycles viridis → heat → greenred → viridis; unknown restarts", () => {
    expect(nextRamp("viridis")).toBe("heat");
    expect(nextRamp("heat")).toBe("greenred");
    expect(nextRamp("greenred")).toBe("viridis");
    expect(nextRamp("nope")).toBe("viridis");    // indexOf -1 + 1 = first entry
  });

  it("viridis stops span the evaluator's table endpoints", () => {
    const stops = rampStops("viridis", 8);
    expect(stops).toHaveLength(8);
    // first/last rows of RAMPS.viridis in flow/nodes.ts
    expect(stops[0].every((c, i) => close(c, [0.267, 0.005, 0.329][i]))).toBe(true);
    expect(stops[7].every((c, i) => close(c, [0.992, 0.906, 0.145][i]))).toBe(true);
  });

  it("heat runs black → white; greenred midpoint is the yellow stop", () => {
    const heat = rampStops("heat", 4);
    expect(heat[0]).toEqual([0, 0, 0]);
    expect(heat[3]).toEqual([1, 1, 1]);
    const gr = rampStops("greenred", 3);
    expect(gr[1].every((c, i) => close(c, [1, 0.9, 0][i]))).toBe(true);
  });

  it("unknown ramp names sample as viridis (the evaluator's fallback)", () => {
    expect(rampStops("bogus", 6)).toEqual(rampStops("viridis", 6));
  });

  it("degenerate n clamps to at least 2 samples", () => {
    expect(rampStops("heat", 1)).toHaveLength(2);
  });
});

// ── label formatting ─────────────────────────────────────────────────────────

describe("colormap domain labels", () => {
  it("fmtDomain: integers bare, fractions to two trimmed places", () => {
    expect(fmtDomain(100)).toBe("100");
    expect(fmtDomain(0.456)).toBe("0.46");
    expect(fmtDomain(-3.5)).toBe("-3.5");
    expect(fmtDomain(2.0)).toBe("2");
  });

  it("manual mode shows the params, auto off", () => {
    expect(cmapState({ ramp: "heat", auto: false, min: 5, max: 12.5 }))
      .toEqual({ rampName: "heat", minText: "5", maxText: "12.5", auto: false, min: 5, max: 12.5 });
  });

  it("auto mode prefers a resolved lo–hi from the status summary (numerics stay the params)", () => {
    const st = cmapState({ ramp: "viridis", auto: true, min: 0, max: 100 },
      { state: "ok", summary: "221–412 viridis" });
    expect(st).toEqual({ rampName: "viridis", minText: "221", maxText: "412", auto: true, min: 0, max: 100 });
  });

  it("auto mode without a status falls back to the params, tag stays on", () => {
    expect(cmapState({ ramp: "viridis", auto: true, min: 0, max: 100 }))
      .toEqual({ rampName: "viridis", minText: "0", maxText: "100", auto: true, min: 0, max: 100 });
  });

  it("unset params read as the kinds.ts schema defaults", () => {
    expect(cmapState({}))
      .toEqual({ rampName: "viridis", minText: "0", maxText: "100", auto: true, min: 0, max: 100 });
  });

  it("a hyphen-minus separator is NOT mistaken for a domain readout", () => {
    const st = cmapState({ auto: true, min: 1, max: 2 }, { state: "ok", summary: "3-5 viridis" });
    expect(st.minText).toBe("1");                // en dash only — "-" is ambiguous with minus
  });
});

// ── body geometry ────────────────────────────────────────────────────────────

describe("colormap body geometry", () => {
  const area = rect(100, 200, 172, 36);          // a NODE_W card's bodyRect at bodyHeight 40

  it("bar on top, labels below, everything inside the area", () => {
    const g = cmapBodyLayout(area);
    for (const r of [g.bar, g.min, g.max]) {
      expect(r.x).toBeGreaterThanOrEqual(area.x);
      expect(r.right).toBeLessThanOrEqual(area.right);
      expect(r.y).toBeGreaterThanOrEqual(area.y);
      expect(r.bottom).toBeLessThanOrEqual(area.bottom);
    }
    expect(g.min.y).toBeGreaterThanOrEqual(g.bar.bottom);
    expect(g.max.y).toBeGreaterThanOrEqual(g.bar.bottom);
  });

  it("min sits left, max sits right, no overlap, tag between them", () => {
    const g = cmapBodyLayout(area);
    expect(g.min.x).toBeLessThan(g.max.x);
    expect(g.min.right).toBeLessThanOrEqual(g.max.x);
    expect(g.tag.x).toBeGreaterThan(g.min.right);
    expect(g.tag.x).toBeLessThan(g.max.x);
  });
});

// ── slider thumb math (T18; replica of gratify examples/shared/range-math.ts) ─

describe("colormap slider math", () => {
  it("domain pads half the span on each side", () => {
    expect(cmapSliderDomain(0, 100)).toEqual({ lo: -50, hi: 150 });
    expect(cmapSliderDomain(-10, 10)).toEqual({ lo: -20, hi: 20 });
  });

  it("degenerate span pads by magnitude (min 1) so thumbs always have room", () => {
    expect(cmapSliderDomain(0, 0)).toEqual({ lo: -1, hi: 1 });
    expect(cmapSliderDomain(40, 40)).toEqual({ lo: 0, hi: 80 });
  });

  it("snap step is ~1/100 of the span, a power of ten", () => {
    expect(cmapSnapStep({ lo: -50, hi: 150 })).toBe(1);       // span 200
    expect(cmapSnapStep({ lo: 0, hi: 1 })).toBe(0.01);        // span 1
    expect(cmapSnapStep({ lo: 0, hi: 50 })).toBe(0.1);        // span 50
  });

  it("px ↔ value round-trips through the bar (snapped)", () => {
    const d = { lo: -50, hi: 150 };                            // step 1
    const bar = { x: 100, w: 160 };
    for (const value of [-50, 0, 37, 100, 150]) {
      const px = bar.x + sliderFrac(value, d) * bar.w;
      expect(sliderValue(px, bar.x, bar.w, d)).toBe(value);
    }
  });

  it("px outside the bar clamps to the domain ends; zero-width bar is safe", () => {
    const d = { lo: 0, hi: 10 };
    expect(sliderValue(-999, 50, 100, d)).toBe(0);
    expect(sliderValue(999, 50, 100, d)).toBe(10);
    expect(sliderValue(123, 50, 0, d)).toBe(0);
  });

  it("zone: outside → nearer thumb, inner edges → thumb, middle → window", () => {
    expect(sliderZone(0.1, 0.3, 0.7, 0.05)).toBe("min");
    expect(sliderZone(0.9, 0.3, 0.7, 0.05)).toBe("max");
    expect(sliderZone(0.33, 0.3, 0.7, 0.05)).toBe("min");
    expect(sliderZone(0.67, 0.3, 0.7, 0.05)).toBe("max");
    expect(sliderZone(0.5, 0.3, 0.7, 0.05)).toBe("window");
  });

  it("a tight span never yields window", () => {
    expect(sliderZone(0.505, 0.5, 0.52, 0.05)).toBe("min");
    expect(sliderZone(0.517, 0.5, 0.52, 0.05)).toBe("max");
  });

  it("thumb drags can't cross", () => {
    expect(sliderDragMin(80, 60)).toEqual({ min: 60, max: 60 });
    expect(sliderDragMin(10, 60)).toEqual({ min: 10, max: 60 });
    expect(sliderDragMax(40, 10)).toEqual({ min: 40, max: 40 });
    expect(sliderDragMax(40, 90)).toEqual({ min: 40, max: 90 });
  });

  it("window shift preserves width exactly and clamps at the domain edges", () => {
    const d = { lo: -50, hi: 150 };
    expect(sliderShiftWindow(0, 100, 30, d)).toEqual({ min: 30, max: 130 });
    expect(sliderShiftWindow(0, 100, 500, d)).toEqual({ min: 50, max: 150 });
    expect(sliderShiftWindow(0, 100, -500, d)).toEqual({ min: -50, max: 50 });
  });

  it("window shift snaps min to the step grid, width still exact", () => {
    const out = sliderShiftWindow(0, 33, 7.4, { lo: -50, hi: 150 });   // step 1
    expect(out.min).toBe(7);
    expect(out.max - out.min).toBe(33);
  });
});

// ── headless drive: clicks through the real reducer + undo history ───────────

const DOC: GraphDoc = {
  name: "cmap-fixture",
  nodes: [{ id: "c1", kind: "viz.colormap",
    params: { ramp: "viridis", auto: true, min: 0, max: 100 }, x: 60, y: 80 }],
  edges: [],
  display: null,
};

/** Mounts ONLY the colormap body parts (the cards.ts hookup line is applied by
 *  the supervisor at the join), against the real editor doc/update. `only`
 *  limits which node's body mounts (keys aren't node-namespaced here, so a
 *  multi-node fixture must pick one). W10-B: passes kind + wired the way the
 *  cards.ts hookup will — wired = an edge lands on the node's colormap INPUT
 *  (a port, so geom.wiredInputsOf never reports it — surface.ts owns edges). */
function mountBody(doc: GraphDoc = DOC, only?: string) {
  const rt = new Runtime<EditorDoc, EditorIntent>(null, {
    init: initialDoc(),
    update: makeUpdate({ kindInfo }),
    view: (d: EditorDoc) => {
      const els = d.graph.nodes.flatMap((n) => {
        if (only && n.id !== only) return [];
        const info = kindInfo(n.kind);
        if (!info) return [];
        const l = nodeLayout(info,
          { params: n.params, wiredInputs: wiredInputsOf(d.graph.edges, n.id, info) },
          { helpOpen: false, zoom: 1 }, d.status[n.id]);
        const wired = n.kind === "viz.colorBy" &&
          d.graph.edges.some((e) => e.to.node === n.id && e.to.slot === "colormap");
        return colormapBody({ id: n.id, params: n.params, kind: n.kind, wired },
          l, rect(n.x, n.y, l.w, l.h), d.status[n.id]);
      });
      return Free("root", {}, els);
    },
  }, { headless: true, width: 800, height: 600 });
  rt.dispatch({ k: "graph", intent: { t: "load", doc } });
  rt.step(30);                                   // settle enter/position springs
  const rectOfKey = (key: string): Rect | null => {
    let found: Rect | null = null;
    walk(rt.root, (i: Instance) => { if (found === null && i.key === key) found = i.rect; });
    return found;
  };
  const clickKey = (key: string) => {
    const r = rectOfKey(key);
    if (!r) throw new Error(`no instance ${key}`);
    rt.pointerDown(r.center);
    rt.pointerUp(r.center);
    rt.step(2);
  };
  const dragPts = (a: { x: number; y: number }, b: { x: number; y: number }, steps = 5) => {
    rt.pointerDown(v(a.x, a.y));
    for (let i = 1; i <= steps; i++) {
      rt.pointerMove(v(a.x + ((b.x - a.x) * i) / steps, a.y + ((b.y - a.y) * i) / steps));
      rt.step(1);
    }
    rt.pointerUp(v(b.x, b.y));
    rt.step(2);
  };
  const node = (id = "c1") => rt.doc.graph.nodes.find((n) => n.id === id)!;
  return { rt, rectOfKey, clickKey, dragPts, node };
}

describe("colormap body, driven headless", () => {
  it("mounts bar and both labels inside the node's body area", () => {
    const d = mountBody();
    const info = kindInfo("viz.colormap")!;
    const l = nodeLayout(info, { params: d.node().params, wiredInputs: new Set() },
      { helpOpen: false, zoom: 1 });
    expect(l.bodyH).toBe(info.bodyHeight);       // kinds.ts reserved the height
    const bar = d.rectOfKey("cmap-bar")!;
    expect(bar).not.toBeNull();
    expect(bar.y).toBeGreaterThanOrEqual(80 + l.bodyTop);
    expect(bar.bottom).toBeLessThanOrEqual(80 + l.bodyTop + l.bodyH);
    expect(d.rectOfKey("cmap-min")).not.toBeNull();
    expect(d.rectOfKey("cmap-max")).not.toBeNull();
    expect(d.rectOfKey("cmap-ramp")).not.toBeNull();   // T18: the cycle affordance
  });

  it("each ramp-label click cycles the ramp param and is its own undo entry", () => {
    const d = mountBody();
    expect(d.rt.doc.past.length).toBe(0);        // load cleared history
    d.clickKey("cmap-ramp");
    expect(d.node().params.ramp).toBe("heat");
    expect(d.rt.doc.past.length).toBe(1);
    d.clickKey("cmap-ramp");
    expect(d.node().params.ramp).toBe("greenred");
    expect(d.rt.doc.past.length).toBe(2);        // batch defeats setParam coalescing
    d.clickKey("cmap-ramp");
    expect(d.node().params.ramp).toBe("viridis");
    expect(d.rt.doc.past.length).toBe(3);
    d.rt.dispatch({ k: "undo" });
    d.rt.step(1);
    expect(d.node().params.ramp).toBe("greenred");
  });

  it("clicking a domain label dispatches the param-edit selection intent", () => {
    const d = mountBody();
    expect(d.rt.doc.sel).toBeNull();
    d.clickKey("cmap-min");
    expect(d.rt.doc.sel).toEqual({ kind: "node", id: "c1" });
  });
});

// ── T18: the bar as a range control, driven through the FULL editor ──────────
// createDrive mounts the real makeView, so the colormap body rides the card's
// adorn layer (keys namespaced `<host>::<key>`) — this is the layer the drag
// must reach: adorn parts host their own gestures (chips already take clicks;
// wave-7 upstream 9e794ae pinned adorn/gesture roots to their targets), so the
// bar's gesture claims the press before the card's move gesture ever sees it —
// the same decline-by-null claim pattern as widgets.ts's widgetGesture, just on
// the adorn tree instead of a declaration-ordered list.

const MANUAL_DOC: GraphDoc = {
  name: "cmap-range-fixture",
  nodes: [{ id: "c1", kind: "viz.colormap",
    params: { ramp: "viridis", auto: false, min: 0, max: 100 }, x: 60, y: 80 }],
  edges: [],
  display: null,
};

/** Thumb centre in screen px, from the LIVE adorn bar rect + the pure math. */
function thumbX(bar: Rect, min: number, max: number, which: "min" | "max"): number {
  const dom = cmapSliderDomain(min, max);
  return bar.x + sliderFrac(which === "min" ? min : max, dom) * bar.w;
}

describe("colormap range control (full editor, adorn layer)", () => {
  it("dragging the min thumb edits params.min — one undo entry, node doesn't move", () => {
    const d = createDrive();
    d.load(MANUAL_DOC);
    d.settle();
    const bar = d.rectOfKey("c1::cmap-bar")!;
    expect(bar).not.toBeNull();
    const y = bar.y + bar.h / 2;
    const x0 = thumbX(bar, 0, 100, "min");                  // frac 0.25 of the bar
    expect(d.doc().past.length).toBe(0);
    d.drag(v(x0, y), v(x0 + bar.w * 0.25, y), 6);           // +0.25 frac = +50 units
    expect(d.graph().nodes[0].params.min).toBe(50);
    expect(d.graph().nodes[0].params.max).toBe(100);
    expect(d.doc().past.length).toBe(1);                    // setParam:<id>:min coalesced
    expect(d.graph().nodes[0].x).toBe(60);                  // the card-move never began
    expect(d.graph().nodes[0].y).toBe(80);
    d.dispatch({ k: "undo" });
    expect(d.graph().nodes[0].params.min).toBe(0);          // whole drag = one Ctrl-Z
  });

  it("dragging the max thumb edits params.max and cannot cross min", () => {
    const d = createDrive();
    d.load(MANUAL_DOC);
    d.settle();
    const bar = d.rectOfKey("c1::cmap-bar")!;
    const y = bar.y + bar.h / 2;
    const x0 = thumbX(bar, 0, 100, "max");
    d.drag(v(x0, y), v(x0 - bar.w, y), 8);                  // way past the min thumb
    expect(d.graph().nodes[0].params.max).toBe(0);          // clamped at min
    expect(d.graph().nodes[0].params.min).toBe(0);
    expect(d.doc().past.length).toBe(1);
  });

  it("dragging the gradient between the thumbs shifts the window — one undo entry", () => {
    const d = createDrive();
    d.load(MANUAL_DOC);
    d.settle();
    const bar = d.rectOfKey("c1::cmap-bar")!;
    const y = bar.y + bar.h / 2;
    const mid = bar.x + bar.w * 0.5;                        // centre of the 0..100 span
    d.drag(v(mid, y), v(mid + bar.w * 0.1, y), 5);          // +0.1 frac = +20 units
    const n = d.graph().nodes[0];
    expect(n.params.min).toBe(20);
    expect(n.params.max).toBe(120);                         // width preserved
    expect(d.doc().past.length).toBe(1);                    // graphBatchKey per-drag seq
    expect(n.x).toBe(60);
    d.dispatch({ k: "undo" });
    expect(d.graph().nodes[0].params.min).toBe(0);
    expect(d.graph().nodes[0].params.max).toBe(100);
  });

  it("auto ON: the bar is display-only — a drag never touches min/max", () => {
    const d = createDrive();
    d.load({ ...MANUAL_DOC,
      nodes: [{ ...MANUAL_DOC.nodes[0], params: { ...MANUAL_DOC.nodes[0].params, auto: true } }] });
    d.settle();
    const bar = d.rectOfKey("c1::cmap-bar")!;
    const y = bar.y + bar.h / 2;
    d.drag(v(bar.x + bar.w * 0.3, y), v(bar.x + bar.w * 0.6, y), 5);
    const n = d.graph().nodes[0];
    expect(n.params.min).toBe(0);
    expect(n.params.max).toBe(100);
  });
});

// ── W10-B: the SAME organ on viz.colorBy, from its embedded params ───────────
// Headless mount only (mountBody): the cards.ts hookup for colorBy is Track A's
// at the join, so createDrive's full view can't produce a colorBy body yet.

const COLORBY_DOC: GraphDoc = {
  name: "colorby-fixture",
  nodes: [{ id: "c1", kind: "viz.colorBy",
    params: { ramp: "heat", auto: false, min: 10, max: 60 }, x: 60, y: 80 }],
  edges: [],
  display: null,
};

/** colorBy + an upstream colormap wired into its colormap INPUT (override). */
const WIRED_DOC: GraphDoc = {
  name: "colorby-wired-fixture",
  nodes: [
    { id: "m1", kind: "viz.colormap", params: { ramp: "greenred", auto: true }, x: 400, y: 80 },
    { id: "c1", kind: "viz.colorBy",
      params: { ramp: "heat", auto: false, min: 10, max: 60 }, x: 60, y: 80 },
  ],
  edges: [{ from: { node: "m1", slot: "out" }, to: { node: "c1", slot: "colormap" } }],
  display: null,
};

describe("colorBy embedded gradient body", () => {
  it("colorBy's ramp options match RAMP_CYCLE (one cycle serves both kinds)", () => {
    const s = kindInfo("viz.colorBy")!.params.ramp;
    expect(s.kind).toBe("enum");
    expect((s as { options: string[] }).options).toEqual(RAMP_CYCLE);
  });

  it("cmapState reads schema defaults from the colorBy kind", () => {
    expect(cmapState({}, undefined, "viz.colorBy"))
      .toEqual({ rampName: "viridis", minText: "0", maxText: "100", auto: true, min: 0, max: 100 });
    expect(cmapState({ ramp: "heat", auto: false, min: 10, max: 60 }, undefined, "viz.colorBy"))
      .toEqual({ rampName: "heat", minText: "10", maxText: "60", auto: false, min: 10, max: 60 });
  });

  it("renders bar + labels + ramp label inside the colorBy body area", () => {
    const d = mountBody(COLORBY_DOC);
    const info = kindInfo("viz.colorBy")!;
    const l = nodeLayout(info, { params: d.node().params, wiredInputs: new Set() },
      { helpOpen: false, zoom: 1 });
    expect(l.bodyH).toBe(info.bodyHeight);       // kinds.ts reserved the height
    const bar = d.rectOfKey("cmap-bar")!;
    expect(bar).not.toBeNull();
    expect(bar.y).toBeGreaterThanOrEqual(80 + l.bodyTop);
    expect(bar.bottom).toBeLessThanOrEqual(80 + l.bodyTop + l.bodyH);
    expect(d.rectOfKey("cmap-min")).not.toBeNull();
    expect(d.rectOfKey("cmap-max")).not.toBeNull();
    expect(d.rectOfKey("cmap-ramp")).not.toBeNull();
    expect(d.rectOfKey("cmap-wired")).toBeNull();     // not wired → normal organ
  });

  it("ramp-label clicks cycle colorBy's ramp param, one undo entry each", () => {
    const d = mountBody(COLORBY_DOC);
    d.clickKey("cmap-ramp");
    expect(d.node().params.ramp).toBe("greenred");    // heat → greenred
    expect(d.rt.doc.past.length).toBe(1);
    d.clickKey("cmap-ramp");
    expect(d.node().params.ramp).toBe("viridis");
    expect(d.rt.doc.past.length).toBe(2);
  });

  it("dragging the min thumb edits colorBy's params.min — one undo per drag", () => {
    const d = mountBody(COLORBY_DOC);
    const bar = d.rectOfKey("cmap-bar")!;
    const y = bar.y + bar.h / 2;
    const x0 = thumbX(bar, 10, 60, "min");            // domain 10..60 pads to -15..85
    expect(d.rt.doc.past.length).toBe(0);
    d.dragPts(v(x0, y), v(x0 + bar.w * 0.25, y), 6);  // +0.25 frac = +25 units
    expect(d.node().params.min).toBe(35);
    expect(d.node().params.max).toBe(60);
    expect(d.rt.doc.past.length).toBe(1);             // setParam:<id>:min coalesced
    d.rt.dispatch({ k: "undo" });
    d.rt.step(1);
    expect(d.node().params.min).toBe(10);             // whole drag = one Ctrl-Z
  });

  it("dragging the gradient window shifts colorBy's min+max — one undo entry", () => {
    const d = mountBody(COLORBY_DOC);
    const bar = d.rectOfKey("cmap-bar")!;
    const y = bar.y + bar.h / 2;
    const mid = bar.x + bar.w * 0.5;                  // centre of the 10..60 span
    d.dragPts(v(mid, y), v(mid + bar.w * 0.1, y), 5); // +0.1 frac = +10 units
    expect(d.node().params.min).toBe(20);
    expect(d.node().params.max).toBe(70);             // width preserved
    expect(d.rt.doc.past.length).toBe(1);             // graphBatchKey per-drag seq
    d.rt.dispatch({ k: "undo" });
    d.rt.step(1);
    expect(d.node().params.min).toBe(10);
    expect(d.node().params.max).toBe(60);
  });

  it("wired colormap input: tag shows, edit affordances gone", () => {
    const d = mountBody(WIRED_DOC, "c1");
    expect(d.rectOfKey("cmap-wired")).not.toBeNull(); // the override tag
    expect(d.rectOfKey("cmap-bar")).not.toBeNull();   // dim bar still drawn
    expect(d.rectOfKey("cmap-min")).toBeNull();       // inert settings → no labels
    expect(d.rectOfKey("cmap-max")).toBeNull();
    expect(d.rectOfKey("cmap-ramp")).toBeNull();      // no cycle affordance
  });

  it("wired: a drag on the bar declines — params never move, no undo entry", () => {
    const d = mountBody(WIRED_DOC, "c1");
    const bar = d.rectOfKey("cmap-bar")!;
    const y = bar.y + bar.h / 2;
    d.dragPts(v(bar.x + bar.w * 0.2, y), v(bar.x + bar.w * 0.8, y), 6);
    expect(d.node().params.min).toBe(10);
    expect(d.node().params.max).toBe(60);
    expect(d.rt.doc.past.length).toBe(0);
  });

  it("viz.colormap body is byte-identical: no wired tag ever, organ intact", () => {
    const d = mountBody();                            // the original DOC fixture
    expect(d.rectOfKey("cmap-wired")).toBeNull();
    expect(d.rectOfKey("cmap-bar")).not.toBeNull();
    expect(d.rectOfKey("cmap-min")).not.toBeNull();
    expect(d.rectOfKey("cmap-max")).not.toBeNull();
    expect(d.rectOfKey("cmap-ramp")).not.toBeNull();
  });
});
