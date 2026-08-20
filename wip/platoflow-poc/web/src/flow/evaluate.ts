// Whole-graph re-evaluation. No memoization: PoC models are small and correctness beats speed.
// Failures poison downstream naming the ROOT cause (wave 9): an upstream error yields
// `upstream error in <rootId>: <rootMessage>`; an upstream needs-setup yields
// `waiting on <rootId>` (state "needs-setup") — three hops down a chain the user
// still sees the originating node, not the immediate parent.
import type {
  EvalCtx, EvalResult, EvaluateGraph, GraphDoc, GraphEdge, GraphNode, NodeKindInfo, NodeStatus, Value,
} from "../contracts";
import { kindInfo } from "../kinds";
import { NODES } from "./nodes";
import { isOptionalInput } from "./lib";
import { summarize } from "./summaries";
import { NeedsSetup, type NodeInputs } from "./types";

/** Incoming wires per node, keyed by target slot (reducer already enforces one wire per input). */
function incomingBySlot(doc: GraphDoc): Map<string, Map<string, GraphEdge>> {
  const ids = new Set(doc.nodes.map(n => n.id));
  const map = new Map<string, Map<string, GraphEdge>>();
  for (const n of doc.nodes) map.set(n.id, new Map());
  for (const e of doc.edges) {
    if (!ids.has(e.from.node) || !ids.has(e.to.node)) continue;
    map.get(e.to.node)!.set(e.to.slot, e);
  }
  return map;
}

/** Kahn topological sort; returns the ordered ids plus whatever could not be ordered (cycles). */
function topoSort(doc: GraphDoc, incoming: Map<string, Map<string, GraphEdge>>) {
  const deps = new Map<string, Set<string>>();
  const dependents = new Map<string, Set<string>>();
  for (const n of doc.nodes) { deps.set(n.id, new Set()); dependents.set(n.id, new Set()); }
  for (const [id, slots] of incoming) {
    for (const e of slots.values()) {
      deps.get(id)!.add(e.from.node);                // self-loops included: they are cycles
      dependents.get(e.from.node)!.add(id);
    }
  }
  const pending = new Map([...deps].map(([id, s]) => [id, new Set(s)]));
  const ready = doc.nodes.filter(n => pending.get(n.id)!.size === 0).map(n => n.id);
  const order: string[] = [];
  while (ready.length > 0) {
    const id = ready.shift()!;
    order.push(id);
    for (const d of dependents.get(id)!) {
      const p = pending.get(d)!;
      p.delete(id);
      if (p.size === 0) ready.push(d);
    }
  }
  const ordered = new Set(order);
  const stuck = doc.nodes.map(n => n.id).filter(id => !ordered.has(id));
  return { order, stuck, deps };
}

/** Within the stuck subgraph, can `start` reach itself? Then it sits on a cycle. */
function onCycle(start: string, stuck: Set<string>, deps: Map<string, Set<string>>): boolean {
  const seen = new Set<string>();
  const queue = [...(deps.get(start) ?? [])].filter(d => stuck.has(d));
  while (queue.length > 0) {
    const id = queue.shift()!;
    if (id === start) return true;
    if (seen.has(id)) continue;
    seen.add(id);
    for (const d of deps.get(id) ?? []) if (stuck.has(d)) queue.push(d);
  }
  return false;
}

const errText = (e: unknown): string =>
  e instanceof Error ? e.message : String(e);

/** The node with ParamSchema defaults filled in for params the user never set (undefined only). */
function withDefaults(node: GraphNode, info: NodeKindInfo): GraphNode {
  let params = node.params;
  for (const [name, schema] of Object.entries(info.params)) {
    if (params[name] === undefined && schema.default !== undefined) {
      if (params === node.params) params = { ...params };
      params[name] = schema.default;
    }
  }
  return params === node.params ? node : { ...node, params };
}

// ── T16 subgraphs ────────────────────────────────────────────────────────────

/** A graph.sub node's ports come from its own SubgraphSpec, not from kinds.ts
 *  (the one kind with dynamic arity). Everything else keeps the declared ports. */
const effectiveInputs = (node: GraphNode, info: NodeKindInfo): { name: string }[] =>
  node.sub ? node.sub.inputs : info.inputs;

/** Seed for nested evaluation: `"<node>|<slot>" → Value` pre-binds an inner
 *  input slot to a value the OUTER graph supplied through a promoted port. */
export type InputSeed = Map<string, Value>;

/** EvalResult plus per-slot output values (multi-output nodes — graph.sub). */
export interface EvalRun extends EvalResult {
  slots: Map<string, Record<string, Value>>;
}

/** Value an edge delivers: a multi-output source resolves per slot, everything
 *  else falls back to the node's single output value. */
export const valueFrom = (run: EvalRun, ref: { node: string; slot: string }): Value | undefined =>
  run.slots.get(ref.node)?.[ref.slot] ?? run.values.get(ref.node);

export const evaluateGraph: EvaluateGraph = async (doc, ctx): Promise<EvalResult> =>
  evaluateGraphSeeded(doc, ctx);

/** `evaluateGraph` with pre-bound input slots (graph.sub evaluates its inner
 *  graph through this; the seed carries the outer wire values). */
export async function evaluateGraphSeeded(
  doc: GraphDoc, ctx: EvalCtx, seed?: InputSeed,
): Promise<EvalRun> {
  const values = new Map<string, Value>();
  const slots = new Map<string, Record<string, Value>>();
  const status = new Map<string, NodeStatus>();
  const byId = new Map(doc.nodes.map(n => [n.id, n]));
  const incoming = incomingBySlot(doc);
  const { order, stuck, deps } = topoSort(doc, incoming);

  // Root-cause map: a node that fails directly is its own root; a poisoned node
  // copies its upstream's root, so N hops down the message still names the origin.
  const roots = new Map<string, { id: string; message: string }>();
  const rootOf = (id: string): { id: string; message: string } =>
    roots.get(id) ?? { id, message: status.get(id)?.message ?? "error" };

  const stuckSet = new Set(stuck);
  const cycleIds = new Set(stuck.filter(id => onCycle(id, stuckSet, deps)));
  for (const id of cycleIds) {
    status.set(id, { state: "error", message: "cycle detected" });
    roots.set(id, { id, message: "cycle detected" });                 // cycle nodes are their own root
  }
  for (const id of stuck) {
    if (cycleIds.has(id)) continue;
    // Hangs off a cycle: walk stuck deps to the cycle node and name it as the root.
    const seen = new Set<string>();
    const queue = [...(deps.get(id) ?? [])].filter(d => stuckSet.has(d));
    let culprit: string | undefined;
    while (queue.length > 0) {
      const d = queue.shift()!;
      if (cycleIds.has(d)) { culprit = d; break; }
      if (seen.has(d)) continue;
      seen.add(d);
      for (const dd of deps.get(d) ?? []) if (stuckSet.has(dd)) queue.push(dd);
    }
    const root = { id: culprit ?? "?", message: "cycle detected" };
    status.set(id, { state: "error", message: `upstream error in ${root.id}: ${root.message}` });
    roots.set(id, root);
  }

  for (const id of order) {
    const node = byId.get(id)!;
    const info = kindInfo(node.kind);
    const fn = NODES.get(node.kind);
    if (!info || !fn) {
      status.set(id, { state: "error", message: `unknown node kind "${node.kind}"` });
      continue;
    }

    const incomingSlots = incoming.get(id)!;

    const ports = effectiveInputs(node, info);   // graph.sub ports come from node.sub

    // Poison: a failed upstream takes the node down before it ever runs, carrying the
    // ROOT cause. An error beats a needs-setup when both feed the same node.
    const upstreamIds = ports
      .map(p => incomingSlots.get(p.name)?.from.node)
      .filter((src): src is string => src !== undefined);
    const badError = upstreamIds.find(src => status.get(src)?.state === "error");
    if (badError !== undefined) {
      const root = rootOf(badError);
      status.set(id, { state: "error", message: `upstream error in ${root.id}: ${root.message}` });
      roots.set(id, root);
      continue;
    }
    const badSetup = upstreamIds.find(src => status.get(src)?.state === "needs-setup");
    if (badSetup !== undefined) {
      const root = rootOf(badSetup);
      status.set(id, { state: "needs-setup", message: `waiting on ${root.id}` });
      roots.set(id, root);
      continue;
    }

    // Every declared input is required in the PoC vocabulary. A seeded slot
    // (nested evaluation binding a promoted port) wins over any edge.
    const inputs: NodeInputs = {};
    let missing: string | undefined;
    for (const port of ports) {
      const seeded = seed?.get(`${id}|${port.name}`);
      if (seeded !== undefined) { inputs[port.name] = seeded; continue; }
      const edge = incomingSlots.get(port.name);
      const v = edge ? slots.get(edge.from.node)?.[edge.from.slot] ?? values.get(edge.from.node) : undefined;
      if (v === undefined) {
        if (isOptionalInput(node.kind, port.name)) continue;   // wave 10: declared-optional slot
        missing = port.name; break;
      }
      inputs[port.name] = v;
    }
    if (missing !== undefined) {
      // Unwired required input: the node is unconfigured, not broken.
      const message = `missing input "${missing}"`;
      status.set(id, { state: "needs-setup", message });
      roots.set(id, { id, message });
      continue;
    }

    try {
      const out = await fn(withDefaults(node, info), inputs, ctx);
      values.set(id, out.value);
      if (out.outputs) slots.set(id, out.outputs);   // multi-output (graph.sub)
      const st: NodeStatus = { state: "ok", summary: out.summary ?? summarize(out.value) };
      if (out.chart) st.chart = out.chart;
      if (out.detail !== undefined) st.detail = out.detail;
      if (out.checklist) st.checklist = out.checklist;
      if (out.warning !== undefined) st.warning = out.warning;
      status.set(id, st);
    } catch (e) {
      const message = errText(e);
      status.set(id, { state: e instanceof NeedsSetup ? "needs-setup" : "error", message });
      roots.set(id, { id, message });
    }
  }

  return { values, status, slots };
}
