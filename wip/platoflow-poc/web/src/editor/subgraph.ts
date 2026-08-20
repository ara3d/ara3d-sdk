// T16 — subgraphs with promoted ports. Pure builders (collapse / expand /
// enter / commit-on-exit) plus the breadcrumb DOM organ and the key decoder.
// The interaction spec is ADOPTED from the Kea editor (labs/kea input.ts /
// document.ts, per kea-layering-guide §5): G groups the selection, U ungroups,
// double-click a subgraph node ENTERS it, Esc / double-click empty canvas
// exits a level, breadcrumb reads "root ▸ <group> …".
//
// Position convention: inner node x/y are stored RELATIVE to the graph.sub
// node's own position (which starts at the selection centroid), so collapse →
// expand restores absolute positions exactly while the group is unmoved, and
// moves rigidly with it otherwise. (Kea keeps a separate `anchor`; the
// contract's SubgraphSpec has none, so relative storage carries the same
// information with zero extra fields.)
//
// Undo policy (argued, and pinned by subgraph.spec): collapse and expand are
// each ONE `batch` intent → exactly one Ctrl-Z. Entering/exiting a subgraph
// dispatches `load`, which CLEARS history — history is per-level, exactly
// Kea's rule ("history is per-doc; cross-boundary undo is out of scope").
// The alternative — undoable enter — would leave history entries that rewrite
// a graph the user is no longer looking at, breaking "undo affects what you
// see"; load-clears keeps the reducer pure and every history entry visible.
import type { GraphDoc, GraphEdge, GraphNode, Intent, NodeKindInfo, SlotRef, SubgraphSpec, WireType } from "../contracts";
import { freshNodeId } from "../reducer";
import { subInfo } from "./geom";

type KindLookup = (kind: string) => NodeKindInfo | undefined;

/** Effective info for any node (graph.sub gets its sub ports). */
const infoOf = (n: GraphNode, kindInfo: KindLookup): NodeKindInfo | undefined => {
  const info = kindInfo(n.kind);
  return info ? subInfo(info, n.sub) : undefined;
};

/** Wire type of a slot on a node (input or output side), sub-aware. */
function slotType(
  doc: GraphDoc, ref: SlotRef, dir: "in" | "out", kindInfo: KindLookup,
): WireType | undefined {
  const n = doc.nodes.find((x) => x.id === ref.node);
  if (!n) return undefined;
  const info = infoOf(n, kindInfo);
  const list = dir === "in" ? info?.inputs : info?.outputs;
  return list?.find((p) => p.name === ref.slot)?.type;
}

// ── collapse ─────────────────────────────────────────────────────────────────

/** ONE `batch` intent that collapses `ids` (≥ 2 nodes) into a graph.sub node
 *  at the selection centroid: removeNode×N, addNode(graph.sub with the
 *  SubgraphSpec payload), reconnect every boundary-crossing edge to a promoted
 *  port. Port name = `<innerNode>.<slot>`; multiple external consumers of one
 *  inner output dedupe onto one port. A display flag inside the selection is
 *  cleared for free (the reducer's removeNode clears `display`). Returns null
 *  when the selection is not collapsible (< 2 known nodes). */
export function collapseSelection(
  doc: GraphDoc, ids: string[], kindInfo: KindLookup,
): { intent: Intent; id: string } | null {
  const picked = doc.nodes.filter((n) => ids.includes(n.id));
  if (picked.length < 2) return null;
  const inside = new Set(picked.map((n) => n.id));

  const cx = picked.reduce((a, n) => a + n.x, 0) / picked.length;
  const cy = picked.reduce((a, n) => a + n.y, 0) / picked.length;

  // Inner nodes: cloned, positions RELATIVE to the group (centroid). A nested
  // graph.sub inside the selection is fine — it is just a node with a `sub`.
  const nodes: GraphNode[] = picked.map((n) => structuredClone({ ...n, x: n.x - cx, y: n.y - cy }));

  const innerEdges: GraphEdge[] = [];
  const inputs: SubgraphSpec["inputs"] = [];
  const outputs: SubgraphSpec["outputs"] = [];
  const portFor = new Map<string, string>();       // "<dir>:<node>.<slot>" → port name
  const reconnect: Intent[] = [];
  const id = freshNodeId(doc);

  for (const e of doc.edges) {
    const fIn = inside.has(e.from.node), tIn = inside.has(e.to.node);
    if (fIn && tIn) { innerEdges.push(structuredClone(e)); continue; }
    if (!fIn && !tIn) continue;                    // untouched outer edge
    if (tIn) {                                     // outer → inner: promoted INPUT
      const name = `${e.to.node}.${e.to.slot}`;
      if (!portFor.has(`in:${name}`)) {            // one wire per input ⇒ unique anyway
        portFor.set(`in:${name}`, name);
        inputs.push({ name, type: slotType(doc, e.to, "in", kindInfo) ?? "scene", inner: { ...e.to } });
      }
      reconnect.push({ t: "connect", from: { ...e.from }, to: { node: id, slot: name } });
    } else {                                       // inner → outer: promoted OUTPUT (dedupe)
      const name = `${e.from.node}.${e.from.slot}`;
      if (!portFor.has(`out:${name}`)) {
        portFor.set(`out:${name}`, name);
        outputs.push({ name, type: slotType(doc, e.from, "out", kindInfo) ?? "scene", inner: { ...e.from } });
      }
      reconnect.push({ t: "connect", from: { node: id, slot: name }, to: { ...e.to } });
    }
  }

  const sub: SubgraphSpec = { nodes, edges: innerEdges, inputs, outputs };
  const intent: Intent = {
    t: "batch",
    intents: [
      ...picked.map((n): Intent => ({ t: "removeNode", id: n.id })),
      { t: "addNode", id, kind: "graph.sub", x: cx, y: cy,
        params: { label: `group (${picked.length})` }, sub },
      ...reconnect,
    ],
  };
  return { intent, id };
}

// ── expand ───────────────────────────────────────────────────────────────────

/** Inverse batch: removeNode(group), addNode for every inner node at
 *  group-relative positions, restore inner edges, reconnect boundary wires to
 *  the inner slots the ports forwarded to. Per-instance widths survive via
 *  trailing resize intents (addNode does not carry `w`). Returns the inner
 *  node ids so the caller can select them (Kea selects the ungrouped set). */
export function expandNode(
  doc: GraphDoc, id: string,
): { intent: Intent; ids: string[] } | null {
  const g = doc.nodes.find((n) => n.id === id);
  if (!g?.sub) return null;
  const sub = g.sub;
  const intents: Intent[] = [{ t: "removeNode", id }];
  for (const m of sub.nodes) {
    intents.push({ t: "addNode", id: m.id, kind: m.kind, x: g.x + m.x, y: g.y + m.y,
                   params: structuredClone(m.params), ...(m.sub ? { sub: structuredClone(m.sub) } : {}) });
    if (m.w !== undefined) intents.push({ t: "resize", node: m.id, w: m.w });
  }
  for (const e of sub.edges) intents.push({ t: "connect", from: { ...e.from }, to: { ...e.to } });
  for (const e of doc.edges) {
    if (e.to.node === id) {
      const p = sub.inputs.find((x) => x.name === e.to.slot);
      if (p) intents.push({ t: "connect", from: { ...e.from }, to: { ...p.inner } });
    } else if (e.from.node === id) {
      const p = sub.outputs.find((x) => x.name === e.from.slot);
      if (p) intents.push({ t: "connect", from: { ...p.inner }, to: { ...e.to } });
    }
  }
  return { intent: { t: "batch", intents }, ids: sub.nodes.map((n) => n.id) };
}

// ── enter / commit-on-exit (scratch-doc mechanism) ───────────────────────────

/** The inner graph as a standalone editable doc (absolute positions). Editing
 *  happens on THIS doc through the ordinary reducer; `commitExit` folds the
 *  result back. Promoted ports are not editable inside (no portal nodes in
 *  the PoC) — boundary rewiring is expand-then-recollapse. */
export function enterDoc(doc: GraphDoc, id: string): { doc: GraphDoc; title: string } | null {
  const g = doc.nodes.find((n) => n.id === id);
  if (!g?.sub) return null;
  const title = typeof g.params.label === "string" && g.params.label ? g.params.label : "group";
  return {
    doc: {
      name: title,
      nodes: g.sub.nodes.map((m) => structuredClone({ ...m, x: g.x + m.x, y: g.y + m.y })),
      edges: structuredClone(g.sub.edges),
      display: null,
    },
    title,
  };
}

/** Fold an edited inner doc back into the parent's graph.sub node: positions
 *  re-relativized around the (possibly unmoved) group node, ports whose inner
 *  endpoint no longer exists are pruned, and outer wires into pruned ports are
 *  dropped with them. Pure — the caller dispatches `load` with the result. */
export function commitExit(parent: GraphDoc, id: string, edited: GraphDoc): GraphDoc {
  const g = parent.nodes.find((n) => n.id === id);
  if (!g?.sub) return parent;
  const alive = new Set(edited.nodes.map((n) => n.id));
  const keep = (ref: SlotRef) => alive.has(ref.node);
  const sub: SubgraphSpec = {
    nodes: edited.nodes.map((m) => structuredClone({ ...m, x: m.x - g.x, y: m.y - g.y })),
    edges: edited.edges.filter((e) => keep(e.from) && keep(e.to)).map((e) => structuredClone(e)),
    inputs: g.sub.inputs.filter((p) => keep(p.inner)),
    outputs: g.sub.outputs.filter((p) => keep(p.inner)),
  };
  const inNames = new Set(sub.inputs.map((p) => p.name));
  const outNames = new Set(sub.outputs.map((p) => p.name));
  return {
    ...parent,
    nodes: parent.nodes.map((n) => (n.id === id ? { ...n, sub } : n)),
    edges: parent.edges.filter((e) =>
      (e.to.node !== id || inNames.has(e.to.slot)) &&
      (e.from.node !== id || outNames.has(e.from.slot))),
  };
}

// ── keyboard decode (pure, spec-able — mirrors Kea's bindings) ───────────────

/** `G` = collapse selection, `U` = expand selected subgraph, `Escape` = exit a
 *  level. Plain keys only (no modifiers) so Ctrl-G etc. stay native. */
export const subgraphKeyAction = (
  ev: { key: string; ctrlKey: boolean; metaKey: boolean; altKey: boolean },
): "collapse" | "expand" | "exit" | null => {
  if (ev.ctrlKey || ev.metaKey || ev.altKey) return null;
  const k = ev.key.toLowerCase();
  if (k === "g") return "collapse";
  if (k === "u") return "expand";
  if (ev.key === "Escape") return "exit";
  return null;
};

// ── breadcrumb (DOM organ — canvas HUD stays Gratify's; text belongs in DOM) ─

export interface Breadcrumb {
  /** `null` hides it; a path renders "root ▸ a ▸ b — editing subgraph…". */
  set(path: string[] | null): void;
  destroy(): void;
}

export function createBreadcrumb(host: HTMLElement, topInset = 0): Breadcrumb {
  const el = document.createElement("div");
  el.className = "pf-breadcrumb";
  el.style.cssText = [
    "position:absolute", `top:${topInset + 10}px`, "left:50%", "transform:translateX(-50%)",
    "padding:4px 12px", "border-radius:8px", "background:rgba(255,255,255,.94)",
    "border:1px solid var(--pf-border-strong)", "color:var(--pf-dim)",
    "font:11px var(--pf-font)", "pointer-events:none", "display:none", "z-index:30",
    "white-space:nowrap",
  ].join(";");
  host.appendChild(el);
  return {
    set(path) {
      if (!path || !path.length) { el.style.display = "none"; return; }
      el.textContent = "";
      const strong = (t: string) => { const b = document.createElement("b"); b.textContent = t; return b; };
      el.append("root");
      for (const p of path) { el.append(" ▸ "); el.append(strong(p)); }
      el.append("  —  editing subgraph · Esc exits");
      el.style.display = "block";
    },
    destroy() { el.remove(); },
  };
}
