// T16: graph.sub evaluation — inner graph runs with promoted inputs bound to
// the outer wire values (seed via sub.inputs[].inner), promoted outputs
// surface per slot, inner errors take the node down naming the root culprit.
import { describe, expect, it } from "vitest";
import type { GraphDoc, SubgraphSpec, TableValue } from "../../contracts";
import { evaluateGraph } from "../evaluate";
import { mkDoc, msgOf, sceneAt, stateOf, stubCtx, summaryOf, tableAt, viewAt } from "./harness";

const gsub = (
  id: string, sub: SubgraphSpec, x = 0, y = 0,
): GraphDoc["nodes"][number] =>
  ({ id, kind: "graph.sub", params: { label: "g" }, x, y, sub });

describe("graph.sub evaluation", () => {
  it("binds promoted inputs to outer wires and surfaces the promoted output", async () => {
    // outer: load → [sub: select walls] → view.scene
    const sub: SubgraphSpec = {
      nodes: [{ id: "s1", kind: "select.byType", params: { type: "IfcWall" }, x: 0, y: 0 }],
      edges: [],
      inputs: [{ name: "s1.in", type: "scene", inner: { node: "s1", slot: "in" } }],
      outputs: [{ name: "s1.out", type: "scene", inner: { node: "s1", slot: "out" } }],
    };
    const doc: GraphDoc = {
      name: "t", display: null,
      nodes: [
        { id: "n1", kind: "load.model", params: { model: "mock" }, x: 0, y: 0 },
        gsub("g1", sub),
        { id: "n3", kind: "view.scene", params: {}, x: 0, y: 0 },
      ],
      edges: [
        { from: { node: "n1", slot: "out" }, to: { node: "g1", slot: "s1.in" } },
        { from: { node: "g1", slot: "s1.out" }, to: { node: "n3", slot: "in" } },
      ],
    };
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "g1"), msgOf(r, "g1")).toBe("ok");
    expect(stateOf(r, "n3"), msgOf(r, "n3")).toBe("ok");
    expect(sceneAt(r, "n3").entities.length).toBe(8);          // the walls got through
    expect(summaryOf(r, "g1")).toBe("1 nodes · 1 in / 1 out");
  });

  it("a multi-node inner chain evaluates in topological order", async () => {
    const sub: SubgraphSpec = {
      nodes: [
        { id: "s1", kind: "select.byType", params: { type: "IfcWall" }, x: 0, y: 0 },
        { id: "s2", kind: "table.fromScene", params: {}, x: 0, y: 0 },
      ],
      edges: [{ from: { node: "s1", slot: "out" }, to: { node: "s2", slot: "in" } }],
      inputs: [{ name: "s1.in", type: "scene", inner: { node: "s1", slot: "in" } }],
      outputs: [{ name: "s2.out", type: "table", inner: { node: "s2", slot: "out" } }],
    };
    const doc: GraphDoc = {
      name: "t", display: null,
      nodes: [
        { id: "n1", kind: "load.model", params: { model: "mock" }, x: 0, y: 0 },
        gsub("g1", sub),
        { id: "n3", kind: "sink.table", params: {}, x: 0, y: 0 },
      ],
      edges: [
        { from: { node: "n1", slot: "out" }, to: { node: "g1", slot: "s1.in" } },
        { from: { node: "g1", slot: "s2.out" }, to: { node: "n3", slot: "in" } },
      ],
    };
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "n3"), msgOf(r, "n3")).toBe("ok");
    const t = tableAt(r, "n3");
    expect(t.rows.length).toBe(8);
    expect(summaryOf(r, "g1")).toBe("2 nodes · 1 in / 1 out");
  });

  it("multiple promoted outputs resolve per slot, not to one shared value", async () => {
    const sub: SubgraphSpec = {
      nodes: [
        { id: "s1", kind: "select.byType", params: { type: "IfcWall" }, x: 0, y: 0 },
        { id: "s2", kind: "table.fromScene", params: {}, x: 0, y: 0 },
      ],
      edges: [{ from: { node: "s1", slot: "out" }, to: { node: "s2", slot: "in" } }],
      inputs: [{ name: "s1.in", type: "scene", inner: { node: "s1", slot: "in" } }],
      outputs: [
        { name: "s1.out", type: "scene", inner: { node: "s1", slot: "out" } },
        { name: "s2.out", type: "table", inner: { node: "s2", slot: "out" } },
      ],
    };
    const doc: GraphDoc = {
      name: "t", display: null,
      nodes: [
        { id: "n1", kind: "load.model", params: { model: "mock" }, x: 0, y: 0 },
        gsub("g1", sub),
        { id: "n3", kind: "view.scene", params: {}, x: 0, y: 0 },
        { id: "n4", kind: "sink.table", params: {}, x: 0, y: 0 },
      ],
      edges: [
        { from: { node: "n1", slot: "out" }, to: { node: "g1", slot: "s1.in" } },
        { from: { node: "g1", slot: "s1.out" }, to: { node: "n3", slot: "in" } },
        { from: { node: "g1", slot: "s2.out" }, to: { node: "n4", slot: "in" } },
      ],
    };
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "n3"), msgOf(r, "n3")).toBe("ok");
    expect(stateOf(r, "n4"), msgOf(r, "n4")).toBe("ok");
    expect(sceneAt(r, "n3").entities.length).toBe(8);          // scene slot
    expect(tableAt(r, "n4").rows.length).toBe(8);              // table slot — distinct value
  });

  it("an unwired promoted input is a missing input on the sub node", async () => {
    const sub: SubgraphSpec = {
      nodes: [{ id: "s1", kind: "select.byType", params: { type: "IfcWall" }, x: 0, y: 0 }],
      edges: [],
      inputs: [{ name: "s1.in", type: "scene", inner: { node: "s1", slot: "in" } }],
      outputs: [{ name: "s1.out", type: "scene", inner: { node: "s1", slot: "out" } }],
    };
    const doc: GraphDoc = { name: "t", display: null, nodes: [gsub("g1", sub)], edges: [] };
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "g1")).toBe("needs-setup");
    expect(msgOf(r, "g1")).toBe('missing input "s1.in"');
  });

  it("an unconfigured inner node makes the sub needs-setup, naming the ROOT culprit", async () => {
    // s1 (no type param → needs-setup) → s2; the sub should blame s1, not s2.
    const sub: SubgraphSpec = {
      nodes: [
        { id: "s1", kind: "select.byType", params: {}, x: 0, y: 0 },
        { id: "s2", kind: "view.scene", params: {}, x: 0, y: 0 },
      ],
      edges: [{ from: { node: "s1", slot: "out" }, to: { node: "s2", slot: "in" } }],
      inputs: [{ name: "s1.in", type: "scene", inner: { node: "s1", slot: "in" } }],
      outputs: [{ name: "s2.out", type: "scene", inner: { node: "s2", slot: "out" } }],
    };
    const doc: GraphDoc = {
      name: "t", display: null,
      nodes: [
        { id: "n1", kind: "load.model", params: { model: "mock" }, x: 0, y: 0 },
        gsub("g1", sub),
        { id: "n3", kind: "view.scene", params: {}, x: 0, y: 0 },
      ],
      edges: [
        { from: { node: "n1", slot: "out" }, to: { node: "g1", slot: "s1.in" } },
        { from: { node: "g1", slot: "s2.out" }, to: { node: "n3", slot: "in" } },
      ],
    };
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "g1")).toBe("needs-setup");
    expect(msgOf(r, "g1")).toBe("in subgraph: s1: choose a type");
    expect(stateOf(r, "n3")).toBe("needs-setup");              // waiting flows on
    expect(msgOf(r, "n3")).toBe("waiting on g1");
  });

  it("an inner DATA error poisons the node as an error, naming the ROOT culprit", async () => {
    // s1 (unknown parameter name → real error) → s2; the sub blames s1, not s2.
    const sub: SubgraphSpec = {
      nodes: [
        { id: "s1", kind: "select.byParameter", params: { parameter: "Nope", op: ">", value: "1" }, x: 0, y: 0 },
        { id: "s2", kind: "view.scene", params: {}, x: 0, y: 0 },
      ],
      edges: [{ from: { node: "s1", slot: "out" }, to: { node: "s2", slot: "in" } }],
      inputs: [{ name: "s1.in", type: "scene", inner: { node: "s1", slot: "in" } }],
      outputs: [{ name: "s2.out", type: "scene", inner: { node: "s2", slot: "out" } }],
    };
    const doc: GraphDoc = {
      name: "t", display: null,
      nodes: [
        { id: "n1", kind: "load.model", params: { model: "mock" }, x: 0, y: 0 },
        gsub("g1", sub),
        { id: "n3", kind: "view.scene", params: {}, x: 0, y: 0 },
      ],
      edges: [
        { from: { node: "n1", slot: "out" }, to: { node: "g1", slot: "s1.in" } },
        { from: { node: "g1", slot: "s2.out" }, to: { node: "n3", slot: "in" } },
      ],
    };
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "g1")).toBe("error");
    expect(msgOf(r, "g1")).toBe('in subgraph: s1: no parameter or channel named "Nope"');
    expect(stateOf(r, "n3")).toBe("error");                    // poison flows on
    expect(msgOf(r, "n3")).toBe('upstream error in g1: in subgraph: s1: no parameter or channel named "Nope"');
  });

  it("an empty subgraph errors; a sink-only subgraph is ok with a stand-in value", async () => {
    const empty: GraphDoc = {
      name: "t", display: null,
      nodes: [gsub("g1", { nodes: [], edges: [], inputs: [], outputs: [] })], edges: [],
    };
    const r1 = await evaluateGraph(empty, stubCtx());
    expect(stateOf(r1, "g1")).toBe("error");
    expect(msgOf(r1, "g1")).toBe("empty subgraph");

    const sinkOnly: SubgraphSpec = {
      nodes: [{ id: "s1", kind: "sink.table", params: {}, x: 0, y: 0 }],
      edges: [],
      inputs: [{ name: "s1.in", type: "table", inner: { node: "s1", slot: "in" } }],
      outputs: [],
    };
    const doc = mkDoc(
      [{ id: "n1", kind: "load.model", params: { model: "mock" } },
       { id: "n2", kind: "table.fromScene", params: {} }],
      [["n1", "n2", "in"]]);
    doc.nodes.push(gsub("g1", sinkOnly));
    doc.edges.push({ from: { node: "n2", slot: "out" }, to: { node: "g1", slot: "s1.in" } });
    const r2 = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r2, "g1"), msgOf(r2, "g1")).toBe("ok");
    expect(summaryOf(r2, "g1")).toBe("1 nodes · 1 in / 0 out");
    expect((r2.values.get("g1") as TableValue).columns).toEqual([]);   // stand-in
  });

  it("nested subgraph inside a subgraph evaluates (it is just a node)", async () => {
    const innermost: SubgraphSpec = {
      nodes: [{ id: "t1", kind: "select.byType", params: { type: "IfcWall" }, x: 0, y: 0 }],
      edges: [],
      inputs: [{ name: "t1.in", type: "scene", inner: { node: "t1", slot: "in" } }],
      outputs: [{ name: "t1.out", type: "scene", inner: { node: "t1", slot: "out" } }],
    };
    const outer: SubgraphSpec = {
      nodes: [gsub("h1", innermost)],
      edges: [],
      inputs: [{ name: "h1.t1.in", type: "scene", inner: { node: "h1", slot: "t1.in" } }],
      outputs: [{ name: "h1.t1.out", type: "scene", inner: { node: "h1", slot: "t1.out" } }],
    };
    const doc: GraphDoc = {
      name: "t", display: null,
      nodes: [
        { id: "n1", kind: "load.model", params: { model: "mock" }, x: 0, y: 0 },
        gsub("g1", outer),
        { id: "n3", kind: "view.scene", params: {}, x: 0, y: 0 },
      ],
      edges: [
        { from: { node: "n1", slot: "out" }, to: { node: "g1", slot: "h1.t1.in" } },
        { from: { node: "g1", slot: "h1.t1.out" }, to: { node: "n3", slot: "in" } },
      ],
    };
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "g1"), msgOf(r, "g1")).toBe("ok");
    expect(sceneAt(r, "n3").entities.length).toBe(8);
  });

  it("colormap wires cross the boundary too (view value survives)", async () => {
    const sub: SubgraphSpec = {
      nodes: [{ id: "s1", kind: "viz.colorBy", params: { channel: "embodied_carbon", ghostOthers: true }, x: 0, y: 0 }],
      edges: [],
      inputs: [
        { name: "s1.scene", type: "scene", inner: { node: "s1", slot: "scene" } },
        { name: "s1.colormap", type: "colormap", inner: { node: "s1", slot: "colormap" } },
      ],
      outputs: [{ name: "s1.out", type: "view", inner: { node: "s1", slot: "out" } }],
    };
    const doc: GraphDoc = {
      name: "t", display: null,
      nodes: [
        { id: "n1", kind: "load.model", params: { model: "mock" }, x: 0, y: 0 },
        { id: "n2", kind: "data.csv", params: { url: "x" }, x: 0, y: 0 },
        { id: "n3", kind: "attach.column", params: { keyColumn: "GlobalId", valueColumn: "embodied_carbon" }, x: 0, y: 0 },
        { id: "n4", kind: "viz.colormap", params: { ramp: "viridis", auto: false, min: 0, max: 100 }, x: 0, y: 0 },
        gsub("g1", sub),
      ],
      edges: [
        { from: { node: "n1", slot: "out" }, to: { node: "n3", slot: "scene" } },
        { from: { node: "n2", slot: "out" }, to: { node: "n3", slot: "table" } },
        { from: { node: "n3", slot: "out" }, to: { node: "g1", slot: "s1.scene" } },
        { from: { node: "n4", slot: "out" }, to: { node: "g1", slot: "s1.colormap" } },
      ],
    };
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "g1"), msgOf(r, "g1")).toBe("ok");
    const view = r.values.get("g1");
    expect(viewAt(r, "g1").colors).toBeDefined();
    expect(view).toBeDefined();
  });
});
