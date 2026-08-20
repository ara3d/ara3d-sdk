// T7 replacement for the retired edgate/intgate chip + help-expando checks:
// "?" toggles helpOpen and shifts socket anchors, ✕ deletes, Help ▸ Show/Hide
// all. Driven through the REAL gesture pipeline on the headless runtime
// (drive.ts); chip click points come from the live adornment instance rects
// (rectOfKey), never hardcoded offsets (§4.4).
//
// W13-B: the eye chip is REMOVED (selection drives the 3D view), and chips
// only render while the card is engaged (hovered/selected) — so every chip
// click here hovers the card first. calm.spec.ts owns the at-rest assertions.
// Single-node docs keep the shared chip keys ("help"/"del") unambiguous.
import { describe, expect, it } from "vitest";
import type { GraphDoc } from "../../contracts";
import { createDrive, type Drive } from "./drive";

const one = (kind: string, id: string, params: Record<string, unknown> = {}): GraphDoc =>
  ({ name: "chip-fixture", nodes: [{ id, kind, params, x: 80, y: 100 }], edges: [], display: null });

const fresh = (doc: GraphDoc) => {
  const d = createDrive();
  d.load(doc);
  d.settle();
  return d;
};

/** Park the pointer on the card's header so the engagement fade-in completes
 *  (hover channel approaches 1) and the chips become clickable. */
const engage = (d: Drive, id: string) => {
  d.rt.pointerMove(d.headerCenter(id));
  d.step(40);
};

describe("card chips through real clicks (retired browser checks)", () => {
  it("? chip toggles helpOpen and shifts the socket anchors (shared geometry)", () => {
    const d = fresh(one("load.model", "load1", { model: "duplex" }));
    engage(d, "load1");
    const before = d.socketCenter({ node: "load1", dir: "out", slot: "out" });
    d.clickKey("load1::help");
    expect(d.doc().helpOpen.load1).toBe(true);
    d.settle();                                    // card grows; springs glide
    const after = d.socketCenter({ node: "load1", dir: "out", slot: "out" });
    expect(after.y).toBeGreaterThan(before.y + 20);
    d.clickKey("load1::help");                            // toggles back off
    expect(d.doc().helpOpen.load1).toBe(false);
  });

  it("param row hit-test still round-trips with the help expando open", () => {
    const d = fresh(one("load.model", "load1", { model: "duplex" }));
    engage(d, "load1");
    d.clickKey("load1::help");
    d.settle();
    expect(d.clickParam("load1", "model")).toBe("model");
  });

  it("✕ chip deletes the node (and an undo brings it back)", () => {
    const d = fresh(one("select.byType", "t1", { type: "IfcWall" }));
    engage(d, "t1");
    d.clickKey("t1::del");
    expect(d.graph().nodes).toEqual([]);
    d.dispatch({ k: "undo" });
    expect(d.graph().nodes.map((n) => n.id)).toEqual(["t1"]);
  });

  it("helpAll opens and closes every node's expando", () => {
    const d = createDrive();
    d.load({
      name: "chip-fixture",
      nodes: [
        { id: "a", kind: "load.model", params: {}, x: 60, y: 80 },
        { id: "b", kind: "select.byType", params: {}, x: 400, y: 80 },
      ],
      edges: [], display: null,
    });
    d.settle();
    d.dispatch({ k: "helpAll", open: true });
    expect(d.doc().helpOpen).toEqual({ a: true, b: true });
    d.dispatch({ k: "helpAll", open: false });
    expect(Object.values(d.doc().helpOpen).every((x) => !x)).toBe(true);
  });
});
