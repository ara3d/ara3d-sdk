// Standalone harness for editor.html: the editor + a fake evaluator, so the
// graph UI can be exercised with no host, no viewer and no real model.
// `window.editorApp` is the EditorApp handle for console poking.
import "../theme"; // register + snap the cream gratify palette before mount
import type { EvalResult, GraphDoc, GraphNode, NodeStatus, Value } from "../contracts";
import { mockModel } from "../fixtures/mockModel";
import { KINDS, kindInfo } from "../kinds";
import { createEditor, type EditorApp } from "./index";

const canvas = document.getElementById("graph-canvas") as HTMLCanvasElement;
const model = mockModel();
const uniq = (xs: (string | null)[]) => [...new Set(xs.filter((x): x is string => !!x))];

const ENUMS: Record<string, string[]> = {
  types: uniq(model.types),
  levels: uniq(model.levels),
  parameters: model.paramNames().map((p) => p.name),
  channels: ["embodied_carbon", "Area"],
  columns: ["GlobalId", "embodied_carbon", "category", "Area", "Level"],
  models: ["duplex", "rac_basic", "mock"],
};

// ── fake evaluator ───────────────────────────────────────────────────────────
// Plausible statuses only: unconnected required inputs error, errors poison
// downstream, everything else reports a made-up but stable summary.

const hash = (s: string) => [...s].reduce((h, c) => (h * 31 + c.charCodeAt(0)) >>> 0, 7);

function summaryFor(node: GraphNode): string {
  const info = kindInfo(node.kind);
  const n = model.entityCount;
  const pick = (lo: number, hi: number) => lo + (hash(node.id + node.kind) % Math.max(1, hi - lo));
  switch (info?.outputs[0]?.type) {
    case "scene": return `${node.kind === "load.model" ? n : pick(3, n)} entities`;
    case "table": return `${pick(4, 40)} rows`;
    case "colormap": return `${node.params.ramp ?? "viridis"} ${node.params.min ?? 0}–${node.params.max ?? 100}`;
    case "view": return `${pick(3, n)} entities colored`;
    default: return info?.effect === "write" ? "ready to write" : "grid ready";
  }
}

function fakeEval(doc: GraphDoc): Map<string, NodeStatus> {
  const status = new Map<string, NodeStatus>();
  const wired = (id: string, slot: string) => doc.edges.some((e) => e.to.node === id && e.to.slot === slot);

  for (const node of doc.nodes) {
    const info = kindInfo(node.kind);
    if (!info) { status.set(node.id, { state: "error", message: `unknown kind ${node.kind}` }); continue; }
    const missing = info.inputs.find((i) => !wired(node.id, i.name));
    if (missing) {
      status.set(node.id, { state: "error", message: `input "${missing.name}" is not connected` });
      continue;
    }
    const st: NodeStatus = { state: "ok", summary: summaryFor(node) };
    // exercise the new in-node surfaces: chart body + help detail line
    if (node.kind === "chart.bar") {
      st.summary = "6 bars";
      st.chart = {
        labels: ["Walls", "Doors", "Windows", "Slabs", "Beams", "Roofs"],
        values: [412, 128, 96, 260, 44, 180],
        title: "carbon by category",
      };
    }
    if (node.kind === "table.ask") {
      st.summary = "5 rows";
      st.detail = "SELECT Category, COUNT(*) AS n FROM EntityText GROUP BY Category ORDER BY n DESC";
    }
    status.set(node.id, st);
  }
  // errors poison downstream (repeat until stable — PoC graphs are tiny)
  for (let pass = 0; pass < doc.nodes.length; pass++) {
    for (const e of doc.edges) {
      if (status.get(e.from.node)?.state === "error" && status.get(e.to.node)?.state === "ok") {
        status.set(e.to.node, { state: "error", message: `upstream error in ${e.from.node}` });
      }
    }
  }
  return status;
}

let timer: number | undefined;
function scheduleEval(doc: GraphDoc): void {
  clearTimeout(timer);
  const pending = new Map<string, NodeStatus>(doc.nodes.map((n) => [n.id, { state: "pending" } as NodeStatus]));
  app.setResults({ values: new Map<string, Value>(), status: pending });
  timer = window.setTimeout(() => {
    app.setResults({ values: new Map<string, Value>(), status: fakeEval(app.getDoc()) } satisfies EvalResult);
  }, 320);
}

// ── mount ────────────────────────────────────────────────────────────────────

// Fake example entries: the standalone harness has no host, so picking one just
// logs (and renames the doc) — enough to prove the menu wiring.
const EXAMPLES = ["carbon-walls", "carbon-by-level", "sql-explore", "write-pset",
  "whatif-expr", "inspect-flow", "filter-sort", "quality-audit", "cost-estimate", "disciplines"];

const app: EditorApp = createEditor(canvas, KINDS, {
  onDocChange: (doc) => scheduleEval(doc),
  onSelect: (id) => console.log("[harness] select", id),
  chrome: {
    title: "PlatoFlow",
    examples: EXAMPLES,
    onExample: (name) => {
      console.log("[harness] example", name);
      (window as unknown as { lastExample: string }).lastExample = name;
      app.setStatus(`example: ${name}`);
    },
  },
  onRunEffect: (nodeId) => {
    console.log("[harness] run effect on", nodeId);
    const status = fakeEval(app.getDoc());
    status.set(nodeId, { state: "ok", summary: "wrote 24 entities" });
    app.setResults({ values: new Map<string, Value>(), status });
  },
  getEnumOptions: (_node, _param, source) => ENUMS[source] ?? [],
});

const STARTER: GraphDoc = {
  name: "carbon by wall type",
  nodes: [
    { id: "load1", kind: "load.model", params: { model: "duplex" }, x: 60, y: 90 },
    { id: "type1", kind: "select.byType", params: { type: "IfcWall" }, x: 320, y: 90 },
    { id: "cmap1", kind: "viz.colormap", params: { ramp: "viridis", min: 0, max: 100 }, x: 320, y: 260 },
    { id: "color1", kind: "viz.colorBy", params: { channel: "embodied_carbon", ghostOthers: true }, x: 600, y: 160 },
    { id: "ask1", kind: "table.ask", params: { question: "Carbon by category?" }, x: 60, y: 330 },
    { id: "chart1", kind: "chart.bar", params: {}, x: 600, y: 360 },
  ],
  edges: [
    { from: { node: "load1", slot: "out" }, to: { node: "type1", slot: "in" } },
    { from: { node: "type1", slot: "out" }, to: { node: "color1", slot: "scene" } },
    { from: { node: "cmap1", slot: "out" }, to: { node: "color1", slot: "colormap" } },
    { from: { node: "load1", slot: "out" }, to: { node: "ask1", slot: "in" } },
    { from: { node: "ask1", slot: "out" }, to: { node: "chart1", slot: "in" } },
  ],
  display: "color1",
};

app.dispatch({ t: "load", doc: STARTER });
app.setStatus("harness — no host, fake evaluator");

(window as unknown as { editorApp: EditorApp }).editorApp = app;
