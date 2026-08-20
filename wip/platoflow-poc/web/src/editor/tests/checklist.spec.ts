// Wave-11 Track E specs: the select.checklist body organ (editor/checklist.ts).
// Pure halves — excluded-list algebra (inverted storage), row layout/clip/
// "…N more" math, the shared hover/click hit-test — plus a headless mount that
// drives the REAL gesture + reducer + undo history.
//
// The cards.ts hookup (adorn push + gesture declaration) is applied by the
// supervisor at the join, so the mount rig here hosts checklistGesture on a
// card-shaped stub part with the REAL nodeMoveGesture declared after it — the
// exact chain position the hookup will use (after gridClick, before move) —
// and mounts the real checklistBody elements beside it. Decorative parts are
// transparent to topInteractiveHit, so presses on the drawn rows reach the
// stub's gesture chain exactly as they will reach the card's.
import { describe, expect, it } from "vitest";
import {
  Free, Runtime, at, part, rect, v, walk,
  type Instance, type Query, type Rect, type Vec,
} from "gratify";
import type { GraphDoc, NodeStatus } from "../../contracts";
import { kindInfo } from "../../kinds";
import { initialDoc, makeUpdate, type EditorDoc, type EditorIntent } from "../doc";
import { bodyRect, nodeLayout } from "../geom";
import { nodeMoveGesture } from "../wires";
import type { NodeCardProps } from "../cards";
import {
  CHECK_ROW_H, checkRowAt, checklistBody, checklistGesture, checklistLayout,
  excludedList, isTicked, toggleExcluded,
  type ChecklistDrag, type ChecklistEntry,
} from "../checklist";

// ── fixtures ─────────────────────────────────────────────────────────────────

const INFO = kindInfo("select.checklist")!;

/** 10 live values: more than a 120px body fits (7 lines → 6 rows + "…4 more").
 *  `on` flags mirror the fixture's excluded param ("IfcSlab"). */
const ENTRIES: ChecklistEntry[] = [
  { value: "IfcWall", on: true, count: 42 },
  { value: "IfcDoor", on: true, count: 12 },
  { value: "IfcSlab", on: false, count: 9 },
  { value: "IfcWindow", on: true, count: 18 },
  { value: "IfcBeam", on: true, count: 7 },
  { value: "IfcColumn", on: true, count: 5 },
  { value: "IfcStair", on: true, count: 2 },
  { value: "IfcRailing", on: true, count: 4 },
  { value: "IfcRoof", on: true },
  { value: "IfcCovering", on: true, count: 11 },
];

const DOC: GraphDoc = {
  name: "checklist-fixture",
  nodes: [{ id: "cl1", kind: "select.checklist",
    params: { source: "types", excluded: "IfcSlab" }, x: 60, y: 80 }],
  edges: [],
  display: null,
};

const STATUS: Record<string, NodeStatus> = {
  cl1: { state: "ok", summary: "119 entities", checklist: ENTRIES },
};

const layoutOf = (params: Record<string, unknown>, status?: NodeStatus) =>
  nodeLayout(INFO, { params, wiredInputs: new Set() }, { helpOpen: false, zoom: 1 }, status);

// ── excluded algebra (pure; inverted comma storage) ──────────────────────────

describe("excluded-list algebra", () => {
  it("parses the comma list whitespace-tolerantly; empty means all ticked", () => {
    expect(excludedList("")).toEqual([]);
    expect(excludedList(undefined)).toEqual([]);
    expect(excludedList("IfcWall")).toEqual(["IfcWall"]);
    expect(excludedList(" IfcWall , IfcDoor ,,")).toEqual(["IfcWall", "IfcDoor"]);
  });

  it("isTicked inverts membership (ticked = ABSENT from the list)", () => {
    expect(isTicked("", "IfcWall")).toBe(true);
    expect(isTicked("IfcWall,IfcDoor", "IfcWall")).toBe(false);
    expect(isTicked("IfcWall,IfcDoor", "IfcSlab")).toBe(true);
  });

  it("toggle adds an absent value (untick) and removes a present one (re-tick)", () => {
    expect(toggleExcluded("", "IfcWall")).toBe("IfcWall");             // untick
    expect(toggleExcluded("IfcSlab", "IfcWall")).toBe("IfcSlab,IfcWall");
    expect(toggleExcluded("IfcSlab,IfcWall", "IfcSlab")).toBe("IfcWall"); // re-tick
    expect(toggleExcluded("IfcSlab", "IfcSlab")).toBe("");
    // round-trip: toggling twice restores the set
    expect(toggleExcluded(toggleExcluded("IfcSlab", "IfcDoor"), "IfcDoor")).toBe("IfcSlab");
  });
});

// ── row layout + shared hit-test (pure) ──────────────────────────────────────

describe("checklistLayout / checkRowAt", () => {
  const l = layoutOf(DOC.nodes[0].params, STATUS.cl1);
  const area = bodyRect(l, rect(60, 80, l.w, l.h));

  it("the kind reserves the contract body (width 240, bodyHeight 120)", () => {
    expect(INFO.width).toBe(240);
    expect(INFO.bodyHeight).toBe(120);
    expect(l.bodyH).toBe(120);
  });

  it("all entries fit → one rect per entry at CHECK_ROW_H pitch, no more-line", () => {
    const g = checklistLayout(area, 5);
    expect(g.rows.length).toBe(5);
    expect(g.overflow).toBe(0);
    expect(g.more).toBeNull();
    g.rows.forEach((r, i) => {
      expect(r.y).toBe(area.y + i * CHECK_ROW_H);
      expect(r.h).toBe(CHECK_ROW_H);
      expect(r.bottom).toBeLessThanOrEqual(area.bottom + 1e-9);
    });
  });

  it("overflow → last line is the '…N more' rect (the picker precedent)", () => {
    const fit = Math.floor(area.h / CHECK_ROW_H);          // 116px → 7 lines
    expect(fit).toBe(7);
    const g = checklistLayout(area, ENTRIES.length);       // 10 entries
    expect(g.visible).toBe(fit - 1);                       // 6 data rows
    expect(g.overflow).toBe(ENTRIES.length - (fit - 1));   // "…4 more"
    expect(g.more).not.toBeNull();
    expect(g.more!.y).toBe(area.y + g.visible * CHECK_ROW_H);
    expect(g.more!.bottom).toBeLessThanOrEqual(area.bottom + 1e-9);
  });

  it("degenerate areas and empty lists clip to nothing", () => {
    const tiny = rect(area.x, area.y, area.w, CHECK_ROW_H - 1);
    expect(checklistLayout(tiny, 10).rows.length).toBe(0);
    expect(checklistLayout(area, 0).rows.length).toBe(0);
    expect(checklistLayout(area, 0).more).toBeNull();
  });

  it("a taller per-instance body (bh) reveals more rows", () => {
    const stretched = nodeLayout(INFO, { params: {}, wiredInputs: new Set(), bh: 240 },
      { helpOpen: false, zoom: 1 });
    const bigArea = bodyRect(stretched, rect(0, 0, stretched.w, stretched.h));
    const g = checklistLayout(bigArea, ENTRIES.length);
    expect(g.visible).toBe(ENTRIES.length);                // 240−4 = 236 → 15 lines
    expect(g.more).toBeNull();
  });

  it("row centers round-trip through checkRowAt (hover tint = click target)", () => {
    const g = checklistLayout(area, ENTRIES.length);
    g.rows.forEach((r, i) => {
      expect(checkRowAt(area, ENTRIES.length, r.center)).toBe(i);
      expect(checkRowAt(area, ENTRIES.length, v(r.x + 1, r.center.y))).toBe(i);
      expect(checkRowAt(area, ENTRIES.length, v(r.right - 1, r.center.y))).toBe(i);
    });
  });

  it("the '…N more' line, outside-x, above and below all miss", () => {
    const g = checklistLayout(area, ENTRIES.length);
    expect(checkRowAt(area, ENTRIES.length, g.more!.center)).toBeNull();
    expect(checkRowAt(area, ENTRIES.length, v(area.x - 2, g.rows[0].center.y))).toBeNull();
    expect(checkRowAt(area, ENTRIES.length, v(area.right + 2, g.rows[0].center.y))).toBeNull();
    expect(checkRowAt(area, ENTRIES.length, v(area.center.x, area.y - 3))).toBeNull();
    expect(checkRowAt(area, ENTRIES.length, v(area.center.x, area.bottom + 3))).toBeNull();
    expect(checkRowAt(area, 0, area.center)).toBeNull();   // placeholder: no rows
  });
});

// ── headless mount: real gesture chain + real reducer + undo history ─────────

/** Card-shaped stub hosting the REAL gesture chain slice the cards.ts hookup
 *  declares: checklistGesture (after gridClick's slot) then nodeMoveGesture,
 *  plus the card's `.press` (select) — proves claim/decline/fall-through and
 *  press composition at the exact chain position. */
const CardStub = part("pf-cl-host")
  .props<NodeCardProps>()
  .size((p) => {
    const l = nodeLayout(p.info, { params: p.params, wiredInputs: p.wiredInputs, w: p.w, bh: p.bh },
      { helpOpen: p.helpOpen, zoom: 1 }, p.status);
    return v(l.w, l.h);
  })
  .render(() => {})
  .gesture<ChecklistDrag>(checklistGesture)
  .gesture<{ off: Vec }>(nodeMoveGesture)
  .press((n): EditorIntent => ({ k: "select", sel: { kind: "node", id: n.props.id } }));

function mount(doc: GraphDoc = DOC, status: Record<string, NodeStatus> = STATUS) {
  const rt = new Runtime<EditorDoc, EditorIntent>(null, {
    init: initialDoc(),
    update: makeUpdate({ kindInfo }),
    view: (d: EditorDoc) => {
      const els = d.graph.nodes.flatMap((n) => {
        const info = kindInfo(n.kind)!;
        const st = d.status[n.id];
        const l = nodeLayout(info, { params: n.params, wiredInputs: new Set(), w: n.w, bh: n.bh },
          { helpOpen: false, zoom: 1 }, st);
        const props: NodeCardProps = {
          id: n.id, info, pos: v(n.x, n.y), params: n.params, wiredInputs: new Set(),
          display: false, helpOpen: false, status: st, w: n.w, bh: n.bh,
        };
        // Body first, stub second: in the render tree, renderHit returns the
        // TOPMOST instance and interactiveHit only walks ANCESTORS, so a
        // decorative sibling on top would eat presses. In the real editor the
        // body rides the card's ADORN layer, whose interactiveHit skips
        // decorative parts and falls through to the card — stub-on-top
        // reproduces that routing.
        return [
          ...checklistBody({ id: n.id, params: n.params }, l, rect(n.x, n.y, l.w, l.h), st),
          at(CardStub(n.id, props), v(n.x, n.y)),
        ];
      });
      return Free("root", {}, els);
    },
  }, { headless: true, width: 900, height: 700 });
  rt.dispatch({ k: "graph", intent: { t: "load", doc } });
  rt.step(2);
  rt.dispatch({ k: "status", status });
  rt.step(30);

  const node = (id = "cl1") => rt.doc.graph.nodes.find((n) => n.id === id)!;
  const rectOfKey = (key: string): Rect | null => {
    let found: Rect | null = null;
    walk(rt.root, (i: Instance) => { if (found === null && i.key === key) found = i.rect; });
    return found;
  };
  /** Center of visible data row `row`, from the LIVE doc through the same
   *  bodyRect + checklistLayout the render and gesture use (§4.4: no magic
   *  coordinates). Viewport is identity here — world coords = screen coords. */
  const rowCenter = (row: number, id = "cl1"): Vec => {
    const n = node(id);
    const st = rt.doc.status[id];
    const l = nodeLayout(kindInfo(n.kind)!, { params: n.params, wiredInputs: new Set(), w: n.w, bh: n.bh },
      { helpOpen: false, zoom: 1 }, st);
    const area = bodyRect(l, rect(n.x, n.y, l.w, l.h));
    const g = checklistLayout(area, st?.checklist?.length ?? 0);
    if (row >= g.rows.length) throw new Error(`row ${row} not visible (${g.rows.length})`);
    return g.rows[row].center;
  };
  const moreCenter = (id = "cl1"): Vec => {
    const n = node(id);
    const st = rt.doc.status[id];
    const l = nodeLayout(kindInfo(n.kind)!, { params: n.params, wiredInputs: new Set() },
      { helpOpen: false, zoom: 1 }, st);
    const area = bodyRect(l, rect(n.x, n.y, l.w, l.h));
    return checklistLayout(area, st?.checklist?.length ?? 0).more!.center;
  };
  const click = (p: Vec) => { rt.pointerDown(p); rt.pointerUp(p); rt.step(2); };
  const drag = (a: Vec, b: Vec, steps = 6) => {
    rt.pointerDown(a);
    for (let i = 1; i <= steps; i++) {
      rt.pointerMove(v(a.x + ((b.x - a.x) * i) / steps, a.y + ((b.y - a.y) * i) / steps));
      rt.step(1);
    }
    rt.pointerUp(b);
    rt.step(2);
  };
  return { rt, node, rectOfKey, rowCenter, moreCenter, click, drag };
}

describe("checklist body, driven headless through the real reducer", () => {
  it("mounts the body panel inside the node's body area", () => {
    const d = mount();
    const l = layoutOf(d.node().params, STATUS.cl1);
    const body = d.rectOfKey("checklist")!;
    expect(body).not.toBeNull();
    expect(body.y).toBeGreaterThanOrEqual(80 + l.bodyTop);
    expect(body.bottom).toBeLessThanOrEqual(80 + l.bodyTop + l.bodyH);
    expect(body.w).toBeGreaterThan(0);
  });

  it("clicking a ticked row unticks it: excluded gains the value, one undo entry, node doesn't move", () => {
    const d = mount();
    expect(d.rt.doc.past.length).toBe(0);          // load + status left history clean
    d.click(d.rowCenter(0));                       // IfcWall (ticked)
    expect(d.node().params.excluded).toBe("IfcSlab,IfcWall");
    expect(d.rt.doc.past.length).toBe(1);
    expect(d.node().x).toBe(60);                   // click ≠ move
    expect(d.node().y).toBe(80);
    expect(d.rt.doc.sel).toEqual({ kind: "node", id: "cl1" });   // press composes
  });

  it("clicking an unticked row re-ticks it: excluded loses the value", () => {
    const d = mount();
    d.click(d.rowCenter(2));                       // IfcSlab (excluded in the fixture)
    expect(d.node().params.excluded).toBe("");
    expect(d.rt.doc.past.length).toBe(1);
  });

  it("consecutive toggles are EACH their own undo entry (batch defeats coalescing)", () => {
    const d = mount();
    d.click(d.rowCenter(0));                       // + IfcWall
    d.click(d.rowCenter(1));                       // + IfcDoor
    d.click(d.rowCenter(0));                       // − IfcWall (same param, same node)
    expect(d.node().params.excluded).toBe("IfcSlab,IfcDoor");
    expect(d.rt.doc.past.length).toBe(3);          // no setParam coalescing across clicks
    d.rt.dispatch({ k: "undo" }); d.rt.step(1);
    expect(d.node().params.excluded).toBe("IfcSlab,IfcWall,IfcDoor");
    d.rt.dispatch({ k: "undo" }); d.rt.step(1);
    expect(d.node().params.excluded).toBe("IfcSlab,IfcWall");
    d.rt.dispatch({ k: "undo" }); d.rt.step(1);
    expect(d.node().params.excluded).toBe("IfcSlab");
  });

  it("a drag from a row MOVES the card (delegation) and never toggles", () => {
    const d = mount();
    const a = d.rowCenter(1);
    d.drag(a, v(a.x + 60, a.y + 40));
    expect(d.node().params.excluded).toBe("IfcSlab");            // untouched
    expect(d.node().x).toBeCloseTo(60 + 60, 0);                  // move delegated
    expect(d.node().y).toBeCloseTo(80 + 40, 0);
    expect(d.rt.doc.past.length).toBe(1);                        // move:<id> coalesced
  });

  it("the '…N more' line declines: click falls through to plain select", () => {
    const d = mount();
    d.click(d.moreCenter());
    expect(d.node().params.excluded).toBe("IfcSlab");
    expect(d.node().x).toBe(60);
    expect(d.rt.doc.sel).toEqual({ kind: "node", id: "cl1" });   // card press only
    expect(d.rt.doc.past.length).toBe(0);
  });

  it("placeholder (no checklist status): body renders, claims nothing, drags still move", () => {
    const d = mount(DOC, { cl1: { state: "needs-setup", message: "run to populate" } });
    expect(d.rectOfKey("checklist")).not.toBeNull();             // panel + dim text
    // a click where row 0 WOULD be toggles nothing
    const n0 = d.node();
    const l = layoutOf(n0.params, undefined);
    const area = bodyRect(l, rect(n0.x, n0.y, l.w, l.h));
    const p = v(area.center.x, area.y + CHECK_ROW_H / 2);
    d.click(p);
    expect(d.node().params.excluded).toBe("IfcSlab");
    expect(d.rt.doc.past.length).toBe(0);
    d.drag(p, v(p.x + 50, p.y));
    expect(d.node().x).toBeCloseTo(60 + 50, 0);                  // body never eats drags
  });
});

// ── gesture-level declines (fake-node probes, gridclick.spec pattern) ────────

describe("checklistGesture declines", () => {
  const q = { mods: {} } as unknown as Query;
  const mk = (over: Partial<NodeCardProps> & { zoom?: number } = {}) => {
    const { zoom = 1, ...rest } = over;
    const props: NodeCardProps = {
      id: "n1", info: INFO, pos: v(0, 0), params: { source: "types", excluded: "" },
      wiredInputs: new Set(), display: false, helpOpen: false,
      status: { state: "ok", checklist: ENTRIES }, ...rest,
    };
    const l = nodeLayout(props.info, { params: props.params, wiredInputs: new Set() },
      { helpOpen: false, zoom: 1 }, props.status);
    const n = { props, rect: rect(0, 0, l.w, l.h), view: { zoom } } as unknown as
      Parameters<typeof checklistGesture.begin>[0];
    // Probe point from the CHECKLIST kind's area (constant): variant layouts
    // (other kinds) may reserve no body at all, but the press lands where a
    // checklist row would be.
    const base = nodeLayout(INFO, { params: {}, wiredInputs: new Set() },
      { helpOpen: false, zoom: 1 });
    const baseArea = bodyRect(base, rect(0, 0, base.w, base.h));
    return { n, p: checklistLayout(baseArea, ENTRIES.length).rows[0].center };
  };

  it("claims a row press at zoom 1; declines the SAME point in chip mode", () => {
    const ok = mk();
    expect(checklistGesture.begin(ok.n, ok.p, q)).not.toBeNull();
    const chip = mk({ zoom: 0.4 });
    expect(checklistGesture.begin(chip.n, chip.p, q)).toBeNull();
  });

  it("declines other kinds and empty/missing checklists", () => {
    const wrongKind = mk({ info: kindInfo("select.byType")! });
    expect(checklistGesture.begin(wrongKind.n, wrongKind.p, q)).toBeNull();
    const noStatus = mk({ status: undefined });
    expect(checklistGesture.begin(noStatus.n, noStatus.p, q)).toBeNull();
    const emptyList = mk({ status: { state: "ok", checklist: [] } });
    expect(checklistGesture.begin(emptyList.n, emptyList.p, q)).toBeNull();
  });
});
