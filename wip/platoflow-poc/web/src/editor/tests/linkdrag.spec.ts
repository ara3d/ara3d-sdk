// T3 headless drive specs: link-drag-search (wire → empty canvas → filtered
// palette → one-batch pick), palette cascade, and drop-on-wire splice.
// All coordinates via nodeLayout helpers in drive.ts — no magic numbers.
import { beforeEach, describe, expect, it } from "vitest";
import { v, type Vec } from "gratify";
import type { GraphDoc } from "../../contracts";
import { HEADER_H, NODE_W, ROW_H } from "../geom";
import { resetCascade } from "../palette";
import { spliceTarget } from "../wires";
import { createDrive, type Drive } from "./drive";

// Wire-type fixtures: scene (load.model out), table (data.csv out),
// colormap (viz.colormap out), view (viz.colorBy out).
const DOC: GraphDoc = {
  name: "linkdrag-fixture",
  nodes: [
    { id: "n1", kind: "load.model", params: { model: "duplex" }, x: 60, y: 80 },
    { id: "n2", kind: "select.byType", params: { type: "IfcWall" }, x: 560, y: 80 },
    { id: "n3", kind: "data.csv", params: { url: "" }, x: 60, y: 420 },
    { id: "n4", kind: "viz.colormap", params: {}, x: 560, y: 420 },
    { id: "n5", kind: "viz.colorBy", params: {}, x: 900, y: 420 },
  ],
  edges: [],
  display: null,
};

/** An empty spot: far from every card and socket (no snap, no card under). */
const EMPTY = v(320, 700);

const fresh = (): Drive => {
  resetCascade();
  const d = createDrive();
  d.load(DOC);
  d.settle();
  return d;
};

const dragToEmpty = (d: Drive, node: string, dir: "in" | "out", slot: string, to: Vec = EMPTY) => {
  d.drag(d.socketCenter({ node, dir, slot }), to);
  d.settle();                                     // palette rows need a frame to mount
};

const palAt = (d: Drive) => d.doc().palette as (Vec & { link?: unknown }) | null;

describe("link-drag-search: drop on empty canvas", () => {
  it("scene wire opens the palette AT the drop point, filtered to scene inputs", () => {
    const d = fresh();
    dragToEmpty(d, "n1", "out", "out");
    const p = palAt(d);
    expect(p).not.toBeNull();
    expect(v(p!.x, p!.y)).toEqual(EMPTY);
    expect(p!.link).toEqual({ node: "n1", slot: "out", dir: "out", type: "scene" });
    expect(d.rectOfKey("select.byType")).not.toBeNull();     // scene input → listed
    expect(d.rectOfKey("view.scene")).not.toBeNull();
    expect(d.rectOfKey("table.filter")).toBeNull();          // table input → filtered out
    expect(d.rectOfKey("viz.colormap")).toBeNull();          // no inputs → filtered out
    expect(d.rectOfKey("data.csv")).toBeNull();
  });

  it("table wire filters to table-input kinds", () => {
    const d = fresh();
    dragToEmpty(d, "n3", "out", "out");
    expect(d.rectOfKey("table.filter")).not.toBeNull();
    expect(d.rectOfKey("chart.bar")).not.toBeNull();
    expect(d.rectOfKey("select.byType")).toBeNull();
    expect(d.rectOfKey("sink.writePset")).toBeNull();        // scene input, not table
  });

  it("colormap wire filters to viz.colorBy (the only colormap input)", () => {
    const d = fresh();
    dragToEmpty(d, "n4", "out", "out");
    expect(d.rectOfKey("viz.colorBy")).not.toBeNull();
    expect(d.rectOfKey("select.byType")).toBeNull();
    expect(d.rectOfKey("table.filter")).toBeNull();
  });

  it("view wire filters to the view modifiers (W11: boxes/explode accept view)", () => {
    const d = fresh();
    dragToEmpty(d, "n5", "out", "out");
    expect(palAt(d)).not.toBeNull();
    expect(d.rectOfKey("pal-empty")).toBeNull();             // no longer empty
    expect(d.rectOfKey("viz.boxes")).not.toBeNull();
    expect(d.rectOfKey("viz.explode")).not.toBeNull();
    expect(d.rectOfKey("viz.colorBy")).toBeNull();           // colorBy still has no view input
    expect(d.graph().edges).toEqual([]);
  });

  it("a short unsnapped drag (a click on the socket) does NOT open the palette", () => {
    const d = fresh();
    const a = d.socketCenter({ node: "n1", dir: "out", slot: "out" });
    d.drag(a, v(a.x + 10, a.y + 8));
    expect(palAt(d)).toBeNull();
  });

  it("dropping the wire on another CARD (not a socket) cancels", () => {
    const d = fresh();
    const { l, origin } = d.layoutOf("n4");                  // incompatible card body
    d.drag(d.socketCenter({ node: "n1", dir: "out", slot: "out" }),
      v(origin.x + l.w / 2, origin.y + l.h - 6));
    expect(palAt(d)).toBeNull();
    expect(d.graph().edges).toEqual([]);
  });
});

describe("link-drag-search: pick", () => {
  it("pick = ONE batch (addNode + connect) = ONE undo entry; placed at the drop", () => {
    const d = fresh();
    dragToEmpty(d, "n1", "out", "out");
    const past = d.doc().past.length;
    d.clickKey("view.scene");
    const added = d.graph().nodes.find((n) => n.kind === "view.scene")!;
    expect(added).toBeDefined();
    expect(d.graph().edges).toEqual([
      { from: { node: "n1", slot: "out" }, to: { node: added.id, slot: "in" } },
    ]);
    // connecting (first scene) input socket centres on the drop point
    expect(v(added.x, added.y)).toEqual(v(EMPTY.x, EMPTY.y - (HEADER_H + ROW_H / 2)));
    expect(palAt(d)).toBeNull();                             // select closed the palette
    expect(d.doc().sel).toEqual({ kind: "node", id: added.id });
    expect(d.doc().past.length).toBe(past + 1);              // ONE undo step…
    d.dispatch({ k: "undo" });
    expect(d.graph().nodes.find((n) => n.kind === "view.scene")).toBeUndefined();
    expect(d.graph().edges).toEqual([]);                     // …removes node AND edge
  });

  it("reverse direction: drag from an INPUT, pick a kind with a compatible output", () => {
    const d = fresh();
    dragToEmpty(d, "n2", "in", "in");
    const p = palAt(d);
    expect(p!.link).toEqual({ node: "n2", slot: "in", dir: "in", type: "scene" });
    expect(d.rectOfKey("load.model")).not.toBeNull();        // scene output → listed
    expect(d.rectOfKey("data.csv")).toBeNull();              // table output → filtered out
    const past = d.doc().past.length;
    d.clickKey("load.model");
    const added = d.graph().nodes.find((n) => n.kind === "load.model" && n.id !== "n1")!;
    expect(d.graph().edges).toEqual([                        // connect REVERSED
      { from: { node: added.id, slot: "out" }, to: { node: "n2", slot: "in" } },
    ]);
    // output side: right edge at the drop point
    expect(v(added.x, added.y)).toEqual(v(EMPTY.x - NODE_W, EMPTY.y - (HEADER_H + ROW_H / 2)));
    expect(d.doc().past.length).toBe(past + 1);
    d.dispatch({ k: "undo" });
    expect(d.graph().edges).toEqual([]);
    expect(d.graph().nodes).toHaveLength(DOC.nodes.length);
  });

  it("Esc cancels: palette closes, doc untouched, no undo entry", () => {
    const d = fresh();
    const g = d.graph();
    const past = d.doc().past.length;
    dragToEmpty(d, "n1", "out", "out");
    expect(palAt(d)).not.toBeNull();
    d.key("Escape");                                         // pointer still over empty surface
    expect(palAt(d)).toBeNull();
    expect(d.graph()).toBe(g);                               // same reference — untouched
    expect(d.doc().past.length).toBe(past);
  });

  it("click-away cancels too and leaves the graph untouched", () => {
    const d = fresh();
    const g = d.graph();
    dragToEmpty(d, "n1", "out", "out");
    d.click(v(EMPTY.x + 260, EMPTY.y));                      // empty surface, off the panel
    expect(palAt(d)).toBeNull();
    expect(d.graph()).toBe(g);
  });
});

describe("palette-at-cursor cascade", () => {
  it("repeated adds at the same palette position cascade +24,+24", () => {
    const d = fresh();
    const at = v(400, 600);
    const posOf = (i: number) => {
      const n = d.graph().nodes.filter((x) => x.kind === "view.scene")[i]!;
      return v(n.x, n.y);
    };
    d.dispatch({ k: "palette", at });
    d.settle();
    d.clickKey("view.scene");
    expect(posOf(0)).toEqual(at);                            // first: at the palette position
    d.dispatch({ k: "palette", at });
    d.settle();
    d.clickKey("view.scene");
    expect(posOf(1)).toEqual(v(at.x + 24, at.y + 24));       // second: one diagonal step
    d.dispatch({ k: "palette", at });
    d.settle();
    d.clickKey("view.scene");
    expect(posOf(2)).toEqual(v(at.x + 48, at.y + 48));
  });

  it("a different palette position resets the cascade", () => {
    const d = fresh();
    d.dispatch({ k: "palette", at: v(400, 600) });
    d.settle();
    d.clickKey("view.scene");
    d.dispatch({ k: "palette", at: v(800, 640) });
    d.settle();
    d.clickKey("view.table");
    const n = d.graph().nodes.find((x) => x.kind === "view.table")!;
    expect(v(n.x, n.y)).toEqual(v(800, 640));
  });
});

describe("drop-on-wire splice", () => {
  const wired = (): Drive => {
    const d = fresh();
    d.dragSocket({ node: "n1", dir: "out", slot: "out" }, { node: "n2", dir: "in", slot: "in" });
    d.settle();
    return d;
  };

  const wireMid = (d: Drive): Vec => {
    const a = d.socketCenter({ node: "n1", dir: "out", slot: "out" });
    const b = d.socketCenter({ node: "n2", dir: "in", slot: "in" });
    return v((a.x + b.x) / 2, (a.y + b.y) / 2);
  };

  it("dropping a compatible node on a wire splices it in as ONE batch", () => {
    const d = wired();
    d.dispatch({ k: "addKind", kind: "view.scene", at: v(300, 600) });
    d.settle();
    const vs = d.graph().nodes.find((n) => n.kind === "view.scene")!;
    d.drag(d.headerCenter(vs.id), wireMid(d));
    expect(d.graph().edges).toEqual([
      { from: { node: "n1", slot: "out" }, to: { node: vs.id, slot: "in" } },
      { from: { node: vs.id, slot: "out" }, to: { node: "n2", slot: "in" } },
    ]);
    // splice was one batch AFTER the coalesced move: one undo restores the wire
    d.dispatch({ k: "undo" });
    expect(d.graph().edges).toEqual([
      { from: { node: "n1", slot: "out" }, to: { node: "n2", slot: "in" } },
    ]);
  });

  it("highlights the spliceable wire while hovering it mid-drag", () => {
    const d = wired();
    d.dispatch({ k: "addKind", kind: "view.scene", at: v(300, 600) });
    d.settle();
    const vs = d.graph().nodes.find((n) => n.kind === "view.scene")!;
    const a = d.headerCenter(vs.id), m = wireMid(d);
    d.rt.pointerDown(a);
    for (let i = 1; i <= 4; i++) {
      d.rt.pointerMove(v(a.x + ((m.x - a.x) * i) / 4, a.y + ((m.y - a.y) * i) / 4));
      d.step(1);
    }
    expect(spliceTarget()).toBe("n1|out>n2|in");             // affordance armed
    d.rt.pointerUp(m);
    d.step(2);
    expect(spliceTarget()).toBeNull();                       // cleared on release
  });

  it("an incompatible node (no matching in+out pair) does not splice", () => {
    const d = wired();
    d.dispatch({ k: "addKind", kind: "chart.bar", at: v(300, 600) });  // table in, no out
    d.settle();
    const cb = d.graph().nodes.find((n) => n.kind === "chart.bar")!;
    d.drag(d.headerCenter(cb.id), wireMid(d));
    expect(d.graph().edges).toEqual([
      { from: { node: "n1", slot: "out" }, to: { node: "n2", slot: "in" } },
    ]);
  });
});
