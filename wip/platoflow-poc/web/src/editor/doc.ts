// The editor's app document: the contract GraphDoc plus purely-editorial state
// (selection, palette position, last eval statuses). Every graph mutation goes
// through `applyIntent` — gestures, the palette and external dispatch alike.
import type { Vec } from "gratify";
import type { GraphDoc, GraphNode, Intent, NodeKindInfo, NodeStatus, ParamSchema } from "../contracts";
import { applyIntent, emptyDoc, freshNodeId } from "../reducer";
import { edgeKey } from "./geom";

export type Selection =
  | { kind: "node"; id: string }
  /** Multi-selection (T13). INVARIANT: `ids.length >= 2` — constructors go
   *  through `selFromIds`, which collapses 1 → `node` and 0 → null, so every
   *  pre-T13 single-selection code path keeps its discriminated shape. */
  | { kind: "nodes"; ids: string[] }
  | { kind: "edge"; key: string }
  | null;

/** Normalize an id set into a Selection (0 → null, 1 → node, 2+ → nodes). */
export const selFromIds = (ids: string[]): Selection =>
  ids.length === 0 ? null :
  ids.length === 1 ? { kind: "node", id: ids[0] } :
  { kind: "nodes", ids };

/** All selected node ids ([] for edge/null selection). */
export const selectedNodeIds = (doc: EditorDoc): string[] =>
  doc.sel?.kind === "node" ? [doc.sel.id] :
  doc.sel?.kind === "nodes" ? doc.sel.ids : [];

// ── press-modifier channel (T13) ─────────────────────────────────────────────
// The card's `.press` (select) has no access to pointer modifiers, so the move
// gesture's `begin` (which sees Query.mods and runs on every card-body press)
// records shift here; the "select" step consumes it one-shot to turn a plain
// replace into a membership toggle. Widget rows (scrub/toggle/chips) claim
// their own presses, so shift-click toggles membership on headers/plain rows
// only — documented limitation, shift already means "fine scrub" there.
let pressShift = false;
export const setPressShift = (b: boolean): void => { pressShift = b; };

export interface EditorDoc {
  graph: GraphDoc;
  sel: Selection;
  /** World position the palette was opened at (null = closed). */
  palette: Vec | null;
  status: Record<string, NodeStatus>;
  /** Per-node help expando ("?" under the title). Editorial state, not graph state. */
  helpOpen: Record<string, boolean>;
  /** Bumped on every graph mutation; external DOM (param popover) watches it. */
  rev: number;
  /** Undo history: snapshot stacks of the (structurally shared) graph. `load` clears. */
  past: GraphDoc[];
  future: GraphDoc[];
  /** Coalescing key of the entry on top of `past` (e.g. "move:n3"); consecutive
   *  graph intents with the same key collapse into one undo step. */
  histKey: string | null;
}

export type EditorIntent =
  | { k: "graph"; intent: Intent }
  /** Like "graph", but with a caller-supplied coalescing key: consecutive
   *  same-key dispatches form ONE undo step. For flooding sources (MCP agents
   *  replaying many intents) that want a whole burst to revert in one Ctrl-Z. */
  | { k: "graphBatchKey"; intent: Intent; key: string }
  | { k: "addKind"; kind: string; at: Vec }
  | { k: "select"; sel: Selection }
  /** Shift-click membership toggle (T13). */
  | { k: "toggleSelect"; id: string }
  /** Gesture placeholder that changes nothing (alt-drag duplicate suppresses
   *  the live move this way — the copy is created on release). */
  | { k: "noop" }
  | { k: "palette"; at: Vec | null }
  | { k: "status"; status: Record<string, NodeStatus> }
  | { k: "run"; node: string }
  | { k: "toggleHelp"; node: string }
  | { k: "helpAll"; open: boolean }
  | { k: "deleteNode"; node: string }
  | { k: "deleteSel" }
  | { k: "undo" }
  | { k: "redo" };

export const initialDoc = (): EditorDoc =>
  ({ graph: emptyDoc("editor"), sel: null, palette: null, status: {}, helpOpen: {}, rev: 0,
     past: [], future: [], histKey: null });

/** Schema defaults, so a fresh node starts with usable params. */
export function defaultParams(info: NodeKindInfo): Record<string, unknown> {
  const out: Record<string, unknown> = {};
  for (const [name, schema] of Object.entries(info.params)) out[name] = defaultOf(schema);
  return out;
}

export function defaultOf(s: ParamSchema): unknown {
  switch (s.kind) {
    case "string": return s.default ?? "";
    case "text": return s.default ?? "";
    case "number": return s.default ?? 0;
    case "boolean": return s.default ?? false;
    case "enum": return s.default ?? s.options[0] ?? "";
    case "dynamicEnum": return s.default ?? "";
    case "modelPick": return "";
  }
}

export interface UpdateHooks {
  onDocChange?(doc: GraphDoc): void;
  onRunEffect?(nodeId: string): void;
  kindInfo(kind: string): NodeKindInfo | undefined;
}

/** The app reducer, closed over the host's callbacks. */
export function makeUpdate(hooks: UpdateHooks) {
  return (doc: EditorDoc, i: EditorIntent): EditorDoc => {
    const stepped = step(doc, i, hooks);
    const next = stepped.graph !== doc.graph ? normalizeSel(stepped) : stepped;
    if (next.graph !== doc.graph && hooks.onDocChange) {
      // after the frame, never inside it — the host may dispatch back at us
      queueMicrotask(() => hooks.onDocChange!(next.graph));
    }
    return next;
  };
}

const HISTORY_CAP = 200;

/** Coalescing key: consecutive intents with the same key form ONE undo step
 *  (a 60-event scrub or drag reverts in one Ctrl-Z). */
function coalesceKey(intent: Intent): string | null {
  switch (intent.t) {
    case "move": return `move:${intent.node}`;
    case "resize": return `resize:${intent.node}`;
    case "setParam": return `setParam:${intent.node}:${intent.name}`;
    default: return null;
  }
}

function withGraph(doc: EditorDoc, intent: Intent): EditorDoc {
  const graph = applyIntent(doc.graph, intent);
  if (graph === doc.graph) return doc;
  if (intent.t === "load")                      // loading a document resets history
    return { ...doc, graph, rev: doc.rev + 1, past: [], future: [], histKey: null };
  const key = coalesceKey(intent);
  const coalesce = key !== null && key === doc.histKey;
  const past = coalesce ? doc.past : [...doc.past.slice(-(HISTORY_CAP - 1)), doc.graph];
  return { ...doc, graph, rev: doc.rev + 1, past, future: [], histKey: key };
}

/** `withGraph` with a caller-supplied coalescing key (see "graphBatchKey").
 *  NON-frozen companion to the frozen `withGraph`: same push/coalesce/cap
 *  policy, but the key comes from the caller instead of `coalesceKey`. `load`
 *  keeps its clears-history semantics by delegating. */
function withGraphKeyed(doc: EditorDoc, intent: Intent, key: string): EditorDoc {
  if (intent.t === "load") return withGraph(doc, intent);
  const graph = applyIntent(doc.graph, intent);
  if (graph === doc.graph) return doc;
  const past = key === doc.histKey ? doc.past : [...doc.past.slice(-(HISTORY_CAP - 1)), doc.graph];
  return { ...doc, graph, rev: doc.rev + 1, past, future: [], histKey: key };
}

/** Undo/redo (or any external mutation) can remove the node or edge the
 *  selection points at; clear a dangling selection so DOM chrome and the
 *  host's onSelect never track a ghost. Identity-preserving when valid. */
function normalizeSel(doc: EditorDoc): EditorDoc {
  const sel = doc.sel;
  if (!sel) return doc;
  if (sel.kind === "nodes") {
    const ids = sel.ids.filter((id) => doc.graph.nodes.some((n) => n.id === id));
    return ids.length === sel.ids.length ? doc : { ...doc, sel: selFromIds(ids) };
  }
  const gone = sel.kind === "node"
    ? !doc.graph.nodes.some((n) => n.id === sel.id)
    : !doc.graph.edges.some((e) => edgeKey(e) === sel.key);
  return gone ? { ...doc, sel: null } : doc;
}

function step(doc: EditorDoc, i: EditorIntent, hooks: UpdateHooks): EditorDoc {
  switch (i.k) {
    case "graph":
      return withGraph(doc, i.intent);

    case "graphBatchKey":
      return withGraphKeyed(doc, i.intent, i.key);

    case "addKind": {
      const info = hooks.kindInfo(i.kind);
      if (!info) return doc;
      const id = freshNodeId(doc.graph);
      const added = withGraph(doc, {
        t: "addNode", id, kind: i.kind, x: i.at.x, y: i.at.y, params: defaultParams(info),
      });
      return { ...added, palette: null, sel: { kind: "node", id } };
    }

    case "select": {
      // Shift-click on a node card toggles membership instead of replacing
      // (the modifier arrives via setPressShift — see the channel note above).
      if (i.sel?.kind === "node" && pressShift) {
        pressShift = false;
        return step(doc, { k: "toggleSelect", id: i.sel.id }, hooks);
      }
      pressShift = false;
      const sel = i.sel?.kind === "nodes" ? selFromIds(i.sel.ids) : i.sel;
      return { ...doc, sel, palette: null };
    }

    case "toggleSelect": {
      const ids = selectedNodeIds(doc);
      const next = ids.includes(i.id) ? ids.filter((x) => x !== i.id) : [...ids, i.id];
      return { ...doc, sel: selFromIds(next), palette: null };
    }

    case "noop":
      return doc;

    case "palette":
      return { ...doc, palette: i.at };

    case "status":
      return { ...doc, status: i.status };

    case "run":
      hooks.onRunEffect?.(i.node);
      return doc;

    case "toggleHelp":
      return { ...doc, helpOpen: { ...doc.helpOpen, [i.node]: !doc.helpOpen[i.node] } };

    case "helpAll": {
      const helpOpen: Record<string, boolean> = {};
      for (const n of doc.graph.nodes) helpOpen[n.id] = i.open;
      return { ...doc, helpOpen };
    }

    case "deleteNode": {
      const next = withGraph(doc, { t: "removeNode", id: i.node });
      return doc.sel?.kind === "node" && doc.sel.id === i.node ? { ...next, sel: null } : next;
    }

    case "undo": {
      if (!doc.past.length) return doc;
      const graph = doc.past[doc.past.length - 1];
      return { ...doc, graph, rev: doc.rev + 1, past: doc.past.slice(0, -1),
               future: [doc.graph, ...doc.future], histKey: null };
    }

    case "redo": {
      if (!doc.future.length) return doc;
      const [graph, ...future] = doc.future;
      return { ...doc, graph, rev: doc.rev + 1, past: [...doc.past, doc.graph],
               future, histKey: null };
    }

    case "deleteSel": {
      const sel = doc.sel;
      if (!sel) return doc;
      if (sel.kind === "node") return { ...withGraph(doc, { t: "removeNode", id: sel.id }), sel: null };
      if (sel.kind === "nodes") {
        // whole set as ONE batch → one undo entry (batch never coalesces)
        const intents: Intent[] = sel.ids.map((id) => ({ t: "removeNode", id }));
        return { ...withGraph(doc, { t: "batch", intents }), sel: null };
      }
      const edge = doc.graph.edges.find((e) => edgeKey(e) === sel.key);
      if (!edge) return doc;
      return { ...withGraph(doc, { t: "disconnect", to: edge.to }), sel: null };
    }
  }
}

/** The single selected node — undefined for edge, null AND multi selection
 *  (a multi-selection has no "the" node: the popover closes and the host's
 *  onSelect reports null, by design — see index.ts syncSel). */
export const selectedNode = (doc: EditorDoc) => {
  const sel = doc.sel;
  return sel?.kind === "node" ? doc.graph.nodes.find((n) => n.id === sel.id) : undefined;
};

// ── clipboard (T13) — pure builders, used by index.ts (Ctrl-C/V/D) and the
// alt-drag duplicate in surface.ts. The clipboard itself is a plain in-memory
// value (index.ts holds it); nothing here touches the OS clipboard.

/** Position-preserving snapshot of a node subset + the edges AMONG them
 *  (edges to outside nodes are dropped). Params deep-copied; edges stored as
 *  indexes into `nodes` so paste can remap onto fresh ids. */
export interface NodeClip {
  nodes: {
    kind: string; x: number; y: number; params: Record<string, unknown>;
    sub?: GraphNode["sub"]; w?: number; bh?: number; // subgraph payload + resize survive copy
  }[];
  edges: { from: number; fromSlot: string; to: number; toSlot: string }[];
}

export function copySelection(graph: GraphDoc, ids: string[]): NodeClip | null {
  const picked = graph.nodes.filter((n) => ids.includes(n.id));
  if (!picked.length) return null;
  const index = new Map(picked.map((n, i) => [n.id, i]));
  const edges: NodeClip["edges"] = [];
  for (const e of graph.edges) {
    const from = index.get(e.from.node), to = index.get(e.to.node);
    if (from !== undefined && to !== undefined)
      edges.push({ from, fromSlot: e.from.slot, to, toSlot: e.to.slot });
  }
  return {
    nodes: picked.map((n) => ({
      kind: n.kind, x: n.x, y: n.y, params: structuredClone(n.params),
      ...(n.sub ? { sub: structuredClone(n.sub) } : {}),
      ...(n.w !== undefined ? { w: n.w } : {}),
      ...(n.bh !== undefined ? { bh: n.bh } : {}),
    })),
    edges,
  };
}

/** ONE `batch` intent that adds every clip node under a fresh id at
 *  (+dx, +dy) from its original position and rebuilds the internal edges.
 *  Batch intents never coalesce, so a paste is exactly one undo step. */
export function pasteIntent(
  graph: GraphDoc, clip: NodeClip, dx = 24, dy = 24,
): { intent: Intent; ids: string[] } {
  const ids = clip.nodes.map(() => freshNodeId(graph));   // module counter → distinct
  const intents: Intent[] = clip.nodes.map((n, i) => ({
    t: "addNode", id: ids[i], kind: n.kind, x: n.x + dx, y: n.y + dy,
    params: structuredClone(n.params),
    ...(n.sub ? { sub: structuredClone(n.sub) } : {}),
  }));
  for (const [i, n] of clip.nodes.entries()) {
    if (n.w !== undefined || n.bh !== undefined) intents.push({ t: "resize", node: ids[i],
      ...(n.w !== undefined ? { w: n.w } : {}),
      ...(n.bh !== undefined ? { bh: n.bh } : {}) });
  }
  for (const e of clip.edges) {
    intents.push({
      t: "connect",
      from: { node: ids[e.from], slot: e.fromSlot },
      to: { node: ids[e.to], slot: e.toSlot },
    });
  }
  return { intent: { t: "batch", intents }, ids };
}

export const anyPending = (doc: EditorDoc): boolean =>
  Object.values(doc.status).some((s) => s.state === "pending");
