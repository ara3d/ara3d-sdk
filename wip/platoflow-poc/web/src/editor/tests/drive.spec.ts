// W0 proof specs for the headless interaction harness (plan §2.6): the REAL
// input pipeline — pointer events → Gratify gestures → EditorIntents → the
// real reducer + undo history — with zero Chrome and zero DOM. Every W1+ track
// builds its inner loop on these helpers.
import { describe, expect, it } from "vitest";
import { v } from "gratify";
import type { GraphDoc } from "../../contracts";
import { createDrive } from "./drive";

const DOC: GraphDoc = {
  name: "drive-fixture",
  nodes: [
    { id: "n1", kind: "load.model", params: { model: "duplex" }, x: 60, y: 80 },
    { id: "n2", kind: "select.byType", params: { type: "IfcWall" }, x: 460, y: 80 },
    { id: "n3", kind: "chart.bar", params: {}, x: 460, y: 340 },
  ],
  edges: [],
  display: null,
};

const fresh = () => {
  const d = createDrive();
  d.load(DOC);
  d.settle();
  return d;
};

describe("headless drive harness", () => {
  it("adds a node through the palette (open → row press → addKind)", () => {
    const d = createDrive();
    d.dispatch({ k: "palette", at: v(500, 260) });
    d.settle();
    expect(d.doc().palette).toEqual(v(500, 260));
    d.clickKey("view.scene");                     // the palette row part, by kind
    expect(d.graph().nodes.map((n) => n.kind)).toContain("view.scene");
    expect(d.doc().palette).toBeNull();           // addKind closes the palette
    const added = d.graph().nodes.find((n) => n.kind === "view.scene")!;
    expect(d.doc().sel).toEqual({ kind: "node", id: added.id });
    expect(v(added.x, added.y)).toEqual(v(500, 260));
  });

  it("wires two sockets through the real drag gesture (snap + connect)", () => {
    const d = fresh();
    d.dragSocket({ node: "n1", dir: "out", slot: "out" }, { node: "n2", dir: "in", slot: "in" });
    expect(d.graph().edges).toEqual([
      { from: { node: "n1", slot: "out" }, to: { node: "n2", slot: "in" } },
    ]);
  });

  it("snap is type-checked: scene out does not connect to a table in", () => {
    const d = fresh();
    d.dragSocket({ node: "n1", dir: "out", slot: "out" }, { node: "n3", dir: "in", slot: "in" });
    expect(d.graph().edges).toEqual([]);          // no snap, no edge
  });

  it("wires backwards too (input socket → output socket)", () => {
    const d = fresh();
    d.dragSocket({ node: "n2", dir: "in", slot: "in" }, { node: "n1", dir: "out", slot: "out" });
    expect(d.graph().edges).toEqual([
      { from: { node: "n1", slot: "out" }, to: { node: "n2", slot: "in" } },
    ]);
  });

  it("moves a node by its header through the real move gesture", () => {
    const d = fresh();
    d.moveNode("n1", 80, 40);
    const n1 = d.graph().nodes.find((n) => n.id === "n1")!;
    expect(v(n1.x, n1.y)).toEqual(v(140, 120));
  });

  it("clickParam: layout-computed click hits the row and selects the node", () => {
    const d = fresh();
    const hit = d.clickParam("n2", "type");
    expect(hit).toBe("type");                     // hit-test round-trip via shared layout
    expect(d.doc().sel).toEqual({ kind: "node", id: "n2" });
  });

  it("scrub stub drags across a param row (no scrub widget yet — see T1)", () => {
    const d = fresh();
    expect(() => d.scrub("n2", "type", 40)).not.toThrow();
  });

  it("click card + Delete key removes the node and cascades its edges", () => {
    const d = fresh();
    d.dragSocket({ node: "n1", dir: "out", slot: "out" }, { node: "n2", dir: "in", slot: "in" });
    d.click(d.headerCenter("n2"));                // select via the card's press
    expect(d.doc().sel).toEqual({ kind: "node", id: "n2" });
    d.key("Delete");
    expect(d.graph().nodes.map((n) => n.id)).toEqual(["n1", "n3"]);
    expect(d.graph().edges).toEqual([]);
  });

  it("undo/redo through the real dispatch reverts gesture-made changes", () => {
    const d = fresh();
    d.dragSocket({ node: "n1", dir: "out", slot: "out" }, { node: "n2", dir: "in", slot: "in" });
    d.click(d.headerCenter("n2"));
    d.key("Delete");
    d.dispatch({ k: "undo" });                    // undo the delete
    expect(d.graph().nodes.map((n) => n.id)).toEqual(["n1", "n2", "n3"]);
    expect(d.graph().edges.length).toBe(1);
    d.dispatch({ k: "undo" });                    // undo the wire
    expect(d.graph().edges).toEqual([]);
    d.dispatch({ k: "redo" });
    expect(d.graph().edges.length).toBe(1);
  });

  it("a drag coalesces to ONE undo step (move springs back in one Ctrl-Z)", () => {
    const d = fresh();
    const before = d.doc().past.length;
    d.moveNode("n1", 120, 0);                     // several move intents in one gesture
    expect(d.doc().past.length).toBe(before + 1);
    d.dispatch({ k: "undo" });
    const n1 = d.graph().nodes.find((n) => n.id === "n1")!;
    expect(v(n1.x, n1.y)).toEqual(v(60, 80));
  });
});
