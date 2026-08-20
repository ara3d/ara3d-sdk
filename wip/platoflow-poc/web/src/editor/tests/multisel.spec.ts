// T13 multi-select / copy / paste / duplicate / align — headless drive specs.
// The marquee is SHIFT-drag on empty canvas (a plain drag is the Pan()
// binding); shift-click toggles membership; dragging any member moves the
// whole set (one undo entry per drag); Ctrl-C/V/D and alt-drag go through the
// NodeClip builders in doc.ts; snap-to-align is pure math in surface.ts.
//
// Modifier note: the runtime's mods object is sticky (Object.assign per
// event), so every shift/alt interaction here ends with an explicit reset
// move — otherwise the NEXT plain click would still see the modifier.
import { describe, expect, it } from "vitest";
import { v, type Vec } from "gratify";
import type { GraphDoc } from "../../contracts";
import { copySelection, pasteIntent, selectedNode, selectedNodeIds, selFromIds } from "../doc";
import { clipboardKeyAction, PASTE_OFFSET } from "../index";
import { snapMove } from "../surface";
import { createDrive, type Drive } from "./drive";

const DOC: GraphDoc = {
  name: "multisel-fixture",
  nodes: [
    { id: "n1", kind: "load.model", params: { model: "duplex" }, x: 60, y: 80 },
    { id: "n2", kind: "select.byType", params: { type: "IfcWall" }, x: 460, y: 80 },
    { id: "n3", kind: "view.scene", params: {}, x: 460, y: 340 },
  ],
  edges: [
    { from: { node: "n1", slot: "out" }, to: { node: "n2", slot: "in" } },
    { from: { node: "n2", slot: "out" }, to: { node: "n3", slot: "in" } },
  ],
  display: null,
};

const fresh = () => {
  const d = createDrive();
  d.load(DOC);
  d.settle();
  return d;
};

const resetMods = (d: Drive, p: Vec) => {
  d.rt.pointerMove(p, { shift: false, alt: false });
  d.step(1);
};

const shiftClick = (d: Drive, p: Vec) => {
  d.rt.pointerDown(p, { shift: true });
  d.rt.pointerUp(p);
  d.step(2);
  resetMods(d, p);
};

const modDrag = (d: Drive, a: Vec, b: Vec, mods: { shift?: boolean; alt?: boolean }, steps = 4) => {
  d.rt.pointerDown(a, mods);
  for (let i = 1; i <= steps; i++) {
    d.rt.pointerMove(v(a.x + ((b.x - a.x) * i) / steps, a.y + ((b.y - a.y) * i) / steps), mods);
    d.step(1);
  }
  d.rt.pointerUp(b);
  d.step(2);
  resetMods(d, b);
};

const pos = (d: Drive, id: string) => {
  const n = d.graph().nodes.find((x) => x.id === id)!;
  return v(n.x, n.y);
};

describe("selection set (shift-click + marquee)", () => {
  it("shift-click extends, toggles, and collapses back to single", () => {
    const d = fresh();
    d.click(d.headerCenter("n1"));
    expect(d.doc().sel).toEqual({ kind: "node", id: "n1" });
    shiftClick(d, d.headerCenter("n2"));
    expect(d.doc().sel).toEqual({ kind: "nodes", ids: ["n1", "n2"] });
    expect(selectedNode(d.doc())).toBeUndefined();          // multi has no "the" node
    expect(selectedNodeIds(d.doc())).toEqual(["n1", "n2"]);
    shiftClick(d, d.headerCenter("n2"));                    // toggle out → single again
    expect(d.doc().sel).toEqual({ kind: "node", id: "n1" });
  });

  it("plain click replaces a multi-selection", () => {
    const d = fresh();
    d.click(d.headerCenter("n1"));
    shiftClick(d, d.headerCenter("n2"));
    d.click(d.headerCenter("n3"));
    expect(d.doc().sel).toEqual({ kind: "node", id: "n3" });
  });

  it("shift-drag marquee selects intersecting nodes (plain drag stays pan)", () => {
    const d = fresh();
    // start points dodge the on-screen HUD (+ button at 12,12–42,42)
    modDrag(d, v(55, 20), v(700, 300), { shift: true });    // covers n1 + n2, not n3
    expect(d.doc().sel).toEqual({ kind: "nodes", ids: ["n1", "n2"] });
    modDrag(d, v(55, 60), v(150, 160), { shift: true });    // just n1 → collapses
    expect(d.doc().sel).toEqual({ kind: "node", id: "n1" });
    modDrag(d, v(900, 500), v(950, 560), { shift: true });  // empty area → null
    expect(d.doc().sel).toBeNull();
    // plain drag on empty canvas pans the viewport instead of selecting
    const pan0 = { ...d.rt.viewport.pan };
    modDrag(d, v(300, 300), v(400, 370), {});               // empty canvas, off-HUD
    expect(d.rt.viewport.pan).not.toEqual(pan0);
    d.rt.viewport.pan = v(0, 0);                            // restore for sanity
  });

  it("deleting a member normalizes the set (nodes → node → null)", () => {
    const d = fresh();
    d.click(d.headerCenter("n1"));
    shiftClick(d, d.headerCenter("n2"));
    d.dispatch({ k: "deleteNode", node: "n2" });
    expect(d.doc().sel).toEqual({ kind: "node", id: "n1" });
  });
});

describe("set move + align snap", () => {
  it("dragging any member moves the whole set; one undo entry per drag", () => {
    const d = fresh();
    d.click(d.headerCenter("n1"));
    shiftClick(d, d.headerCenter("n2"));
    const past0 = d.doc().past.length;
    const a = d.headerCenter("n1");
    modDrag(d, a, v(a.x + 60, a.y + 40), {});               // drag member n1
    d.settle();
    expect(pos(d, "n1")).toEqual(v(120, 120));
    expect(pos(d, "n2")).toEqual(v(520, 120));              // moved together
    expect(pos(d, "n3")).toEqual(v(460, 340));              // non-member untouched
    expect(d.doc().past.length).toBe(past0 + 1);            // whole drag = one entry
    const b = d.headerCenter("n2");
    modDrag(d, b, v(b.x + 30, b.y), {});                    // second drag, other member
    d.settle();
    expect(pos(d, "n1")).toEqual(v(150, 120));
    expect(d.doc().past.length).toBe(past0 + 2);            // per-drag, not per-set
    d.dispatch({ k: "undo" });
    expect(pos(d, "n1")).toEqual(v(120, 120));
    d.dispatch({ k: "undo" });
    expect(pos(d, "n1")).toEqual(v(60, 80));
    expect(pos(d, "n2")).toEqual(v(460, 80));
  });

  it("snapMove: snaps each axis to the nearest edge within range", () => {
    const others = [{ x: 100, y: 200 }, { x: 300, y: 400 }];
    expect(snapMove(others, 104, 250)).toEqual({ x: 100, y: 250, gx: 100, gy: null });
    expect(snapMove(others, 250, 396)).toEqual({ x: 250, y: 400, gx: null, gy: 400 });
    expect(snapMove(others, 296, 204)).toEqual({ x: 300, y: 200, gx: 300, gy: 200 });
    expect(snapMove(others, 150, 300)).toEqual({ x: 150, y: 300, gx: null, gy: null });
    expect(snapMove([], 5, 5)).toEqual({ x: 5, y: 5, gx: null, gy: null });
  });

  it("a live drag snaps to another node's left edge (guide math end-to-end)", () => {
    const d = fresh();
    d.click(d.headerCenter("n1"));
    const a = d.headerCenter("n1");
    // n1 x: 60 → 456, within SNAP_DIST of n2/n3's x=460 → snaps to 460
    modDrag(d, a, v(a.x + 396, a.y + 130), {}, 8);
    d.settle();
    expect(pos(d, "n1").x).toBe(460);
  });
});

describe("clipboard (copy / paste / duplicate)", () => {
  it("copySelection keeps internal edges only and deep-copies params", () => {
    const clip = copySelection(DOC, ["n1", "n2"])!;
    expect(clip.nodes.map((n) => n.kind)).toEqual(["load.model", "select.byType"]);
    expect(clip.edges).toEqual([{ from: 0, fromSlot: "out", to: 1, toSlot: "in" }]);  // n2→n3 dropped
    expect(clip.nodes[0].params).toEqual(DOC.nodes[0].params);
    expect(clip.nodes[0].params).not.toBe(DOC.nodes[0].params);
    expect(copySelection(DOC, [])).toBeNull();
  });

  it("paste = ONE batch: fresh ids, +24/+24, remapped edges, single undo", () => {
    const d = fresh();
    const clip = copySelection(d.graph(), ["n1", "n2"])!;
    const past0 = d.doc().past.length;
    const { intent, ids } = pasteIntent(d.graph(), clip, PASTE_OFFSET, PASTE_OFFSET);
    d.dispatch({ k: "graph", intent });
    expect(ids).toHaveLength(2);
    expect(d.graph().nodes.map((n) => n.id)).toContain(ids[0]);
    expect(new Set(ids).size).toBe(2);
    for (const id of ids) expect(["n1", "n2", "n3"]).not.toContain(id);  // fresh
    const c1 = d.graph().nodes.find((n) => n.id === ids[0])!;
    const c2 = d.graph().nodes.find((n) => n.id === ids[1])!;
    expect(v(c1.x, c1.y)).toEqual(v(60 + 24, 80 + 24));     // offset from ORIGINALS
    expect(v(c2.x, c2.y)).toEqual(v(460 + 24, 80 + 24));
    expect(c1.params).toEqual({ model: "duplex" });
    // internal edge remapped onto the fresh ids; no edge leaks to old nodes
    expect(d.graph().edges).toContainEqual(
      { from: { node: ids[0], slot: "out" }, to: { node: ids[1], slot: "in" } });
    expect(d.graph().edges.filter((e) => e.to.node === "n3")).toHaveLength(1);
    expect(d.doc().past.length).toBe(past0 + 1);            // batch = one undo step
    d.dispatch({ k: "undo" });
    expect(d.graph().nodes.map((n) => n.id)).toEqual(["n1", "n2", "n3"]);
    expect(d.graph().edges).toHaveLength(2);
  });

  it("clipboardKeyAction decodes Ctrl-C/V/D and nothing else", () => {
    const ev = (key: string, ctrl = true, shift = false) =>
      ({ key, ctrlKey: ctrl, metaKey: false, shiftKey: shift });
    expect(clipboardKeyAction(ev("c"))).toBe("copy");
    expect(clipboardKeyAction(ev("V"))).toBe("paste");
    expect(clipboardKeyAction(ev("d"))).toBe("duplicate");
    expect(clipboardKeyAction(ev("c", false))).toBeNull();  // plain typing
    expect(clipboardKeyAction(ev("d", true, true))).toBeNull();
    expect(clipboardKeyAction({ key: "v", ctrlKey: false, metaKey: true, shiftKey: false }))
      .toBe("paste");                                       // ⌘ works too
  });

  it("alt-drag duplicates the set at the drag offset; originals stay put", () => {
    const d = fresh();
    d.click(d.headerCenter("n1"));
    shiftClick(d, d.headerCenter("n2"));
    const past0 = d.doc().past.length;
    const a = d.headerCenter("n1");
    modDrag(d, a, v(a.x + 100, a.y + 60), { alt: true });
    d.settle();
    expect(pos(d, "n1")).toEqual(v(60, 80));                // originals unmoved
    expect(pos(d, "n2")).toEqual(v(460, 80));
    const copies = d.graph().nodes.filter((n) => !["n1", "n2", "n3"].includes(n.id));
    expect(copies.map((n) => n.kind).sort()).toEqual(["load.model", "select.byType"]);
    const c1 = copies.find((n) => n.kind === "load.model")!;
    expect(v(c1.x, c1.y)).toEqual(v(160, 140));             // +100,+60 from original
    // internal edge duplicated between the copies; the copies are the selection
    const ids = copies.map((n) => n.id);
    expect(d.graph().edges).toContainEqual(
      { from: { node: c1.id, slot: "out" }, to: { node: ids.find((i) => i !== c1.id)!, slot: "in" } });
    expect(selectedNodeIds(d.doc()).sort()).toEqual([...ids].sort());
    expect(d.doc().past.length).toBe(past0 + 1);            // one batch, one undo
    d.dispatch({ k: "undo" });
    expect(d.graph().nodes).toHaveLength(3);
  });
});

describe("delete-set + single-selection paths", () => {
  it("Delete removes the whole set as ONE batch (one undo restores all)", () => {
    const d = fresh();
    d.click(d.headerCenter("n1"));
    shiftClick(d, d.headerCenter("n2"));
    const past0 = d.doc().past.length;
    d.key("Delete");
    expect(d.graph().nodes.map((n) => n.id)).toEqual(["n3"]);
    expect(d.graph().edges).toEqual([]);                    // cascaded
    expect(d.doc().sel).toBeNull();
    expect(d.doc().past.length).toBe(past0 + 1);
    d.dispatch({ k: "undo" });
    expect(d.graph().nodes.map((n) => n.id)).toEqual(["n1", "n2", "n3"]);
    expect(d.graph().edges).toHaveLength(2);
  });

  it("single-selection paths unchanged: param row, single move", () => {
    // W13-B: the eye-chip leg of this test is gone with the chip itself
    // (calm.spec.ts proves no card renders one).
    const d = fresh();
    expect(d.clickParam("n2", "type")).toBe("type");        // row hit + select
    expect(d.doc().sel).toEqual({ kind: "node", id: "n2" });
    d.moveNode("n1", 80, 40);                               // plain single move
    expect(pos(d, "n1")).toEqual(v(140, 120));
    // a drag is not a click — moving an unselected node leaves selection alone
    expect(selectedNodeIds(d.doc())).toEqual(["n2"]);
  });

  it("selFromIds normalizes 0/1/n", () => {
    expect(selFromIds([])).toBeNull();
    expect(selFromIds(["a"])).toEqual({ kind: "node", id: "a" });
    expect(selFromIds(["a", "b"])).toEqual({ kind: "nodes", ids: ["a", "b"] });
  });
});
