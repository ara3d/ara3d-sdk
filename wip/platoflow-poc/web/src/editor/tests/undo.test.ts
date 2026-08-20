// W0 undo machinery: pure specs over doc.ts — intent sequence in, past/future shape out.
import { describe, expect, it } from "vitest";
import type { Intent, NodeKindInfo } from "../../contracts";
import { initialDoc, makeUpdate, type EditorDoc, type EditorIntent } from "../doc";

const INFO: NodeKindInfo = {
  kind: "test.node", label: "Test", category: "data", description: "test",
  inputs: [], outputs: [], params: { p: { kind: "number", default: 0 } },
};

const update = makeUpdate({ kindInfo: () => INFO });
const g = (intent: Intent): EditorIntent => ({ k: "graph", intent });
const run = (doc: EditorDoc, intents: EditorIntent[]) => intents.reduce(update, doc);

const addN = (id: string): EditorIntent =>
  g({ t: "addNode", id, kind: "test.node", x: 0, y: 0, params: { p: 0 } });

describe("undo history", () => {
  it("each distinct graph intent is one undo step", () => {
    const doc = run(initialDoc(), [addN("n1"), addN("n2")]);
    expect(doc.past.length).toBe(2);
    expect(doc.graph.nodes.length).toBe(2);
  });

  it("consecutive setParam on the same param coalesce to one step (scrub)", () => {
    const doc = run(initialDoc(), [
      addN("n1"),
      g({ t: "setParam", node: "n1", name: "p", value: 1 }),
      g({ t: "setParam", node: "n1", name: "p", value: 2 }),
      g({ t: "setParam", node: "n1", name: "p", value: 3 }),
    ]);
    expect(doc.past.length).toBe(2); // add + one coalesced setParam run
    const undone = update(doc, { k: "undo" });
    expect(undone.graph.nodes[0].params.p).toBe(0); // back to pre-scrub value
  });

  it("different params do not coalesce; a non-coalescable intent breaks the run", () => {
    const doc = run(initialDoc(), [
      addN("n1"),
      g({ t: "setParam", node: "n1", name: "p", value: 1 }),
      g({ t: "setDisplay", node: "n1" }),
      g({ t: "setParam", node: "n1", name: "p", value: 2 }),
    ]);
    expect(doc.past.length).toBe(4);
  });

  it("consecutive move on the same node coalesces (drag)", () => {
    const doc = run(initialDoc(), [
      addN("n1"),
      g({ t: "move", node: "n1", x: 10, y: 0 }),
      g({ t: "move", node: "n1", x: 20, y: 0 }),
    ]);
    expect(doc.past.length).toBe(2);
  });

  it("undo/redo round-trips and preserves structure", () => {
    const doc = run(initialDoc(), [addN("n1"), addN("n2")]);
    const u1 = update(doc, { k: "undo" });
    expect(u1.graph.nodes.map(n => n.id)).toEqual(["n1"]);
    expect(u1.future.length).toBe(1);
    const r1 = update(u1, { k: "redo" });
    expect(r1.graph.nodes.map(n => n.id)).toEqual(["n1", "n2"]);
    expect(r1.future.length).toBe(0);
  });

  it("a new edit clears the redo stack", () => {
    const doc = run(initialDoc(), [addN("n1"), addN("n2")]);
    const u = update(doc, { k: "undo" });
    const edited = update(u, addN("n3"));
    expect(edited.future.length).toBe(0);
  });

  it("undo on empty history is a no-op; redo likewise", () => {
    const doc = initialDoc();
    expect(update(doc, { k: "undo" })).toBe(doc);
    expect(update(doc, { k: "redo" })).toBe(doc);
  });

  it("load clears history", () => {
    const doc = run(initialDoc(), [addN("n1")]);
    const loaded = update(doc, g({ t: "load", doc: { name: "d", nodes: [], edges: [], display: null } }));
    expect(loaded.past.length).toBe(0);
    expect(loaded.future.length).toBe(0);
    expect(loaded.histKey).toBe(null);
  });

  it("batch is one undo entry", () => {
    const doc = run(initialDoc(), [
      g({ t: "batch", intents: [
        { t: "addNode", id: "a", kind: "test.node", x: 0, y: 0 },
        { t: "addNode", id: "b", kind: "test.node", x: 0, y: 0 },
        { t: "connect", from: { node: "a", slot: "out" }, to: { node: "b", slot: "in" } },
      ]}),
    ]);
    expect(doc.past.length).toBe(1);
    const u = update(doc, { k: "undo" });
    expect(u.graph.nodes.length).toBe(0);
  });

  it("history is capped at 200 entries", () => {
    let doc = initialDoc();
    for (let i = 0; i < 205; i++) doc = update(doc, addN(`n${i}`));
    expect(doc.past.length).toBe(200);
  });

  it("no-op intents leave history untouched", () => {
    const doc = run(initialDoc(), [addN("n1")]);
    const same = update(doc, g({ t: "removeNode", id: "missing" }));
    expect(same.past.length).toBe(1);
  });
});
