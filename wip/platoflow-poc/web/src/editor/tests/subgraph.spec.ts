// T16 specs: subgraphs with promoted ports.
// Pure halves — subInfo (dynamic ports), key decode, collapse/expand builders,
// enter/commit-on-exit — then reducer-level round trips (collapse→expand
// identity, one-Ctrl-Z undo) through the REAL editor update via the headless
// drive, an anchor-level wiring check, and the evaluation round trip
// (collapsed === uncollapsed) against the flow harness's mock ctx.
import { describe, expect, it } from "vitest";
import { rect, v } from "gratify";
import type { GraphDoc, SubgraphSpec } from "../../contracts";
import { kindInfo } from "../../kinds";
import { applyIntent } from "../../reducer";
import { evaluateGraph } from "../../flow/evaluate";
import { sceneAt, stateOf, stubCtx, viewAt, msgOf } from "../../flow/tests/harness";
import { nodeLayout, socketPos, subInfo } from "../geom";
import {
  collapseSelection, commitExit, enterDoc, expandNode, subgraphKeyAction,
} from "../subgraph";
import { createDrive } from "./drive";

// ── fixture: the beat-1-shaped chain (load → select → csv → attach → cmap → colorBy)
const chainDoc = (): GraphDoc => ({
  name: "t", display: null,
  nodes: [
    { id: "n1", kind: "load.model", params: { model: "mock" }, x: 0, y: 0 },
    { id: "n2", kind: "select.byType", params: { type: "IfcWall" }, x: 200, y: 0 },
    { id: "n3", kind: "data.csv", params: { url: "x" }, x: 200, y: 160 },
    { id: "n4", kind: "attach.column", params: { keyColumn: "GlobalId", valueColumn: "embodied_carbon" }, x: 400, y: 40 },
    { id: "n5", kind: "viz.colormap", params: { ramp: "viridis", auto: false, min: 0, max: 100 }, x: 400, y: 200 },
    { id: "n6", kind: "viz.colorBy", params: { channel: "embodied_carbon", ghostOthers: true }, x: 600, y: 80 },
  ],
  edges: [
    { from: { node: "n1", slot: "out" }, to: { node: "n2", slot: "in" } },
    { from: { node: "n2", slot: "out" }, to: { node: "n4", slot: "scene" } },
    { from: { node: "n3", slot: "out" }, to: { node: "n4", slot: "table" } },
    { from: { node: "n4", slot: "out" }, to: { node: "n6", slot: "scene" } },
    { from: { node: "n5", slot: "out" }, to: { node: "n6", slot: "colormap" } },
  ],
});

const collapseMid = (doc: GraphDoc) =>
  collapseSelection(doc, ["n2", "n4"], kindInfo)!;   // the mid-section: 3 crossings in, 1 out

// ── subInfo: the ONE dynamic-arity kind ──────────────────────────────────────

describe("subInfo (geom dynamic ports)", () => {
  const sub: SubgraphSpec = {
    nodes: [], edges: [],
    inputs: [{ name: "a.in", type: "scene", inner: { node: "a", slot: "in" } }],
    outputs: [
      { name: "a.out", type: "table", inner: { node: "a", slot: "out" } },
      { name: "b.out", type: "view", inner: { node: "b", slot: "out" } },
    ],
  };

  it("returns info unchanged without a sub (every other kind)", () => {
    const info = kindInfo("select.byType")!;
    expect(subInfo(info, undefined)).toBe(info);
  });

  it("merges the SubgraphSpec's ports onto the static declaration", () => {
    const eff = subInfo(kindInfo("graph.sub")!, sub);
    expect(eff.inputs.map((p) => [p.name, p.type])).toEqual([["a.in", "scene"]]);
    expect(eff.outputs.map((p) => [p.name, p.type])).toEqual([["a.out", "table"], ["b.out", "view"]]);
  });

  it("the layout allocates socket rows for the dynamic ports (geom pin)", () => {
    const stat = nodeLayout(kindInfo("graph.sub")!, { params: {}, wiredInputs: new Set() },
      { helpOpen: false, zoom: 1 });
    const dyn = nodeLayout(subInfo(kindInfo("graph.sub")!, sub), { params: {}, wiredInputs: new Set() },
      { helpOpen: false, zoom: 1 });
    expect(dyn.h).toBeGreaterThan(stat.h);           // 2 socket rows vs the 1-row minimum
    // socket anchors land on distinct rows on the out side
    const r = rect(0, 0, dyn.w, dyn.h);
    const y0 = socketPos(dyn, r, "out", 0).y, y1 = socketPos(dyn, r, "out", 1).y;
    expect(y1).toBeGreaterThan(y0);
  });
});

// ── key decode (Kea bindings: G group, U ungroup, Esc exit) ──────────────────

describe("subgraphKeyAction", () => {
  const ev = (key: string, mods: Partial<{ ctrlKey: boolean; metaKey: boolean; altKey: boolean }> = {}) =>
    ({ key, ctrlKey: false, metaKey: false, altKey: false, ...mods });
  it("decodes g/G, u/U, Escape; modifiers stay native", () => {
    expect(subgraphKeyAction(ev("g"))).toBe("collapse");
    expect(subgraphKeyAction(ev("G"))).toBe("collapse");
    expect(subgraphKeyAction(ev("u"))).toBe("expand");
    expect(subgraphKeyAction(ev("Escape"))).toBe("exit");
    expect(subgraphKeyAction(ev("g", { ctrlKey: true }))).toBeNull();
    expect(subgraphKeyAction(ev("u", { metaKey: true }))).toBeNull();
    expect(subgraphKeyAction(ev("x"))).toBeNull();
  });
});

// ── collapse ─────────────────────────────────────────────────────────────────

describe("collapseSelection", () => {
  it("declines fewer than two known nodes", () => {
    const doc = chainDoc();
    expect(collapseSelection(doc, ["n2"], kindInfo)).toBeNull();
    expect(collapseSelection(doc, ["nope", "alsono"], kindInfo)).toBeNull();
  });

  it("one batch: removes the set, adds graph.sub at the centroid, reconnects crossings", () => {
    const doc = chainDoc();
    const { intent, id } = collapseMid(doc);
    expect(intent.t).toBe("batch");
    const next = applyIntent(doc, intent);

    const g = next.nodes.find((n) => n.id === id)!;
    expect(g.kind).toBe("graph.sub");
    expect(g.x).toBe(300);                           // centroid of n2 (200,0) and n4 (400,40)
    expect(g.y).toBe(20);
    expect(next.nodes.map((n) => n.id).sort()).toEqual(["n1", "n3", "n5", "n6", id].sort());

    // promoted ports: 2 ins (n2.in from n1, n4.table from n3) + 1 out (n4.out → n6)
    expect(g.sub!.inputs.map((p) => [p.name, p.type])).toEqual(
      [["n2.in", "scene"], ["n4.table", "table"]]);
    expect(g.sub!.outputs.map((p) => [p.name, p.type])).toEqual([["n4.out", "scene"]]);

    // boundary edges rewired onto the promoted ports; internal edge moved inside
    expect(next.edges).toContainEqual({ from: { node: "n1", slot: "out" }, to: { node: id, slot: "n2.in" } });
    expect(next.edges).toContainEqual({ from: { node: "n3", slot: "out" }, to: { node: id, slot: "n4.table" } });
    expect(next.edges).toContainEqual({ from: { node: id, slot: "n4.out" }, to: { node: "n6", slot: "scene" } });
    expect(g.sub!.edges).toEqual([{ from: { node: "n2", slot: "out" }, to: { node: "n4", slot: "scene" } }]);

    // inner positions are group-relative
    const inner = new Map(g.sub!.nodes.map((n) => [n.id, n]));
    expect([inner.get("n2")!.x, inner.get("n2")!.y]).toEqual([-100, -20]);
    expect([inner.get("n4")!.x, inner.get("n4")!.y]).toEqual([100, 20]);
  });

  it("multiple external consumers of ONE inner output dedupe onto one port", () => {
    const doc = chainDoc();
    // second consumer of n4.out
    doc.nodes.push({ id: "n7", kind: "view.scene", params: {}, x: 620, y: 240 });
    doc.edges.push({ from: { node: "n4", slot: "out" }, to: { node: "n7", slot: "in" } });
    const { intent, id } = collapseMid(doc);
    const next = applyIntent(doc, intent);
    const g = next.nodes.find((n) => n.id === id)!;
    expect(g.sub!.outputs).toHaveLength(1);          // one port …
    const fanout = next.edges.filter((e) => e.from.node === id && e.from.slot === "n4.out");
    expect(fanout).toHaveLength(2);                  // … two outer wires from it
  });

  it("a display flag inside the selection is cleared (reducer removeNode semantics)", () => {
    const doc = { ...chainDoc(), display: "n4" };
    const { intent } = collapseMid(doc);
    expect(applyIntent(doc, intent).display).toBeNull();
  });

  it("a display flag OUTSIDE the selection survives", () => {
    const doc = { ...chainDoc(), display: "n6" };
    const { intent } = collapseMid(doc);
    expect(applyIntent(doc, intent).display).toBe("n6");
  });

  it("a nested graph.sub inside the selection is just a node", () => {
    const doc = chainDoc();
    const first = collapseMid(doc);
    const step1 = applyIntent(doc, first.intent);
    const second = collapseSelection(step1, [first.id, "n6"], kindInfo)!;
    const step2 = applyIntent(step1, second.intent);
    const g2 = step2.nodes.find((n) => n.id === second.id)!;
    expect(g2.sub!.nodes.some((n) => n.id === first.id && n.sub)).toBe(true);
    // n5 → n6.colormap crossed the boundary; n1/n3 feeds crossed into the inner sub node
    expect(g2.sub!.inputs.map((p) => p.name).sort()).toEqual(
      [`${first.id}.n2.in`, `${first.id}.n4.table`, "n6.colormap"].sort());
  });
});

// ── expand: the inverse batch ────────────────────────────────────────────────

describe("expandNode", () => {
  it("collapse then expand is identity on nodes, edges and positions", () => {
    const doc = chainDoc();
    const { intent, id } = collapseMid(doc);
    const collapsed = applyIntent(doc, intent);
    const x = expandNode(collapsed, id)!;
    expect(x.ids.sort()).toEqual(["n2", "n4"]);
    const restored = applyIntent(collapsed, x.intent);

    const norm = (d: GraphDoc) => ({
      nodes: [...d.nodes].sort((a, b) => a.id.localeCompare(b.id))
        .map((n) => ({ id: n.id, kind: n.kind, params: n.params, x: n.x, y: n.y })),
      edges: [...d.edges].map((e) => JSON.stringify(e)).sort(),
    });
    expect(norm(restored)).toEqual(norm(doc));       // exact positions — group unmoved
  });

  it("moving the group before expanding moves the set rigidly", () => {
    const doc = chainDoc();
    const { intent, id } = collapseMid(doc);
    let collapsed = applyIntent(doc, intent);
    collapsed = applyIntent(collapsed, { t: "move", node: id, x: 300 + 50, y: 20 + 10 });
    const restored = applyIntent(collapsed, expandNode(collapsed, id)!.intent);
    const n2 = restored.nodes.find((n) => n.id === "n2")!;
    expect([n2.x, n2.y]).toEqual([250, 10]);         // original +50,+10
  });

  it("per-instance widths survive via trailing resize intents", () => {
    const doc = chainDoc();
    doc.nodes.find((n) => n.id === "n4")!.w = 300;
    const { intent, id } = collapseMid(doc);
    const collapsed = applyIntent(doc, intent);
    const restored = applyIntent(collapsed, expandNode(collapsed, id)!.intent);
    expect(restored.nodes.find((n) => n.id === "n4")!.w).toBe(300);
  });

  it("declines a node with no sub", () => {
    expect(expandNode(chainDoc(), "n2")).toBeNull();
  });
});

// ── enter / commit-on-exit (scratch-doc mechanism) ───────────────────────────

describe("enterDoc / commitExit", () => {
  it("enter shows the inner graph at absolute positions; commit folds edits back", () => {
    const doc = chainDoc();
    const { intent, id } = collapseMid(doc);
    const collapsed = applyIntent(doc, intent);
    const e = enterDoc(collapsed, id)!;
    expect(e.title).toBe("group (2)");
    expect(e.doc.nodes.find((n) => n.id === "n2")!.x).toBe(200);   // absolute again

    // edit inside: move n2, change a param — through the ordinary reducer
    let edited = applyIntent(e.doc, { t: "move", node: "n2", x: 210, y: 5 });
    edited = applyIntent(edited, { t: "setParam", node: "n2", name: "type", value: "IfcDoor" });
    const parent = commitExit(collapsed, id, edited);

    const g = parent.nodes.find((n) => n.id === id)!;
    const n2 = g.sub!.nodes.find((n) => n.id === "n2")!;
    expect([n2.x, n2.y]).toEqual([210 - g.x, 5 - g.y]);            // re-relativized
    expect(n2.params.type).toBe("IfcDoor");
    expect(parent.edges).toEqual(collapsed.edges);                 // boundary untouched
  });

  it("deleting an inner port target prunes the port AND its outer wires", () => {
    const doc = chainDoc();
    const { intent, id } = collapseMid(doc);
    const collapsed = applyIntent(doc, intent);
    const e = enterDoc(collapsed, id)!;
    const edited = applyIntent(e.doc, { t: "removeNode", id: "n4" });
    const parent = commitExit(collapsed, id, edited);
    const g = parent.nodes.find((n) => n.id === id)!;
    expect(g.sub!.inputs.map((p) => p.name)).toEqual(["n2.in"]);   // n4.table pruned
    expect(g.sub!.outputs).toEqual([]);                            // n4.out pruned
    expect(parent.edges.some((e2) => e2.to.node === id && e2.to.slot === "n4.table")).toBe(false);
    expect(parent.edges.some((e2) => e2.from.node === id)).toBe(false);
  });

  it("enterDoc declines a non-subgraph node", () => {
    expect(enterDoc(chainDoc(), "n2")).toBeNull();
  });
});

// ── through the real editor update: one-Ctrl-Z undo + dynamic anchors ────────

describe("subgraph through the headless drive", () => {
  it("undo after collapse restores everything in ONE Ctrl-Z (batch never coalesces)", () => {
    const d = createDrive();
    const doc = chainDoc();
    d.load(doc);
    const before = JSON.stringify(d.graph());
    const { intent, id } = collapseMid(d.graph());
    d.dispatch({ k: "graph", intent });
    expect(d.graph().nodes.some((n) => n.id === id)).toBe(true);
    expect(d.doc().past.length).toBe(1);             // one history entry for the whole batch
    d.dispatch({ k: "undo" });
    expect(JSON.stringify(d.graph())).toBe(before);
  });

  it("the card mounts with promoted sockets a real wire drag can land on", () => {
    const d = createDrive();
    const doc = chainDoc();
    d.load(doc);
    const { intent, id } = collapseMid(d.graph());
    d.dispatch({ k: "graph", intent });
    d.settle();

    // disconnect n3 → sub, then re-wire it by dragging between real sockets
    d.dispatch({ k: "graph", intent: { t: "disconnect", to: { node: id, slot: "n4.table" } } });
    const g = d.graph().nodes.find((n) => n.id === id)!;
    const eff = subInfo(kindInfo("graph.sub")!, g.sub);
    const l = nodeLayout(eff, { params: g.params, wiredInputs: new Set() },
      { helpOpen: false, zoom: 1 }, d.doc().status[id]);
    const gr = d.rectOfKey(id)!;
    const idx = eff.inputs.findIndex((p) => p.name === "n4.table");
    const target = socketPos(l, rect(gr.x, gr.y, l.w, l.h), "in", idx);
    d.drag(d.socketCenter({ node: "n3", dir: "out", slot: "out" }), v(target.x, target.y));
    expect(d.graph().edges).toContainEqual(
      { from: { node: "n3", slot: "out" }, to: { node: id, slot: "n4.table" } });
  });
});

// ── evaluation round trip: collapsed === uncollapsed ─────────────────────────

describe("collapse → evaluate round trip", () => {
  it("the collapsed graph computes the same view as the flat one", async () => {
    const flat = chainDoc();
    const rFlat = await evaluateGraph(flat, stubCtx());
    expect(stateOf(rFlat, "n6"), msgOf(rFlat, "n6")).toBe("ok");

    const { intent, id } = collapseMid(flat);
    const collapsed = applyIntent(flat, intent);
    const rCol = await evaluateGraph(collapsed, stubCtx());
    expect(stateOf(rCol, id), msgOf(rCol, id)).toBe("ok");
    expect(stateOf(rCol, "n6"), msgOf(rCol, "n6")).toBe("ok");

    const a = viewAt(rFlat, "n6"), b = viewAt(rCol, "n6");
    expect([...b.entities]).toEqual([...a.entities]);
    expect([...(b.colors ?? [])]).toEqual([...(a.colors ?? [])]);
    expect(sceneAt(rCol, "n1").entities.length).toBe(sceneAt(rFlat, "n1").entities.length);
  });

  it("collapse → expand → evaluate also matches (identity round trip)", async () => {
    const flat = chainDoc();
    const { intent, id } = collapseMid(flat);
    const collapsed = applyIntent(flat, intent);
    const restored = applyIntent(collapsed, expandNode(collapsed, id)!.intent);
    const rA = await evaluateGraph(flat, stubCtx());
    const rB = await evaluateGraph(restored, stubCtx());
    const a = viewAt(rA, "n6"), b = viewAt(rB, "n6");
    expect([...b.entities]).toEqual([...a.entities]);
    expect([...(b.colors ?? [])]).toEqual([...(a.colors ?? [])]);
  });
});
