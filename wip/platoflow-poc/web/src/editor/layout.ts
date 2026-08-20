// Pure auto-layout for the node canvas (W13-A). No DOM, no geometry imports —
// card sizes are injected through `sizeOf`, so this module never needs to know
// about nodeLayout, zoom, or help state, and specs can run without jsdom.
//
// tidyLayout: layered left-to-right by dataflow depth (longest path from the
// sources), columns packed left-to-right, rows ordered to keep wires roughly
// horizontal, disconnected components stacked vertically.
// placeFree: nearest deterministic free spot for a dropped card.
import type { GraphDoc, GraphNode } from "../contracts";

export interface Size { w: number; h: number }
export interface Placed { id: string; x: number; y: number }

/** Horizontal gap between a column's widest card and the next column. */
export const COL_GAP = 64;
/** Vertical gap between stacked cards within a column. */
export const ROW_GAP = 36;
/** Vertical gap between disconnected components. */
export const COMPONENT_GAP = 80;
/** Clearance placeFree keeps around a dropped card. */
export const DROP_MARGIN = 16;
/** placeFree cascade / sweep step. */
export const DROP_STEP = 28;

// ── depth assignment (cycle-safe) ────────────────────────────────────────────

/** Longest-path depth per node, plus the components. Back edges discovered
 *  during DFS are dropped (that's the cycle break: depth relaxation then runs
 *  over a guaranteed DAG, so it can never loop). All iteration follows
 *  graph.nodes / graph.edges order → deterministic. */
const analyze = (nodes: GraphNode[], edges: { from: string; to: string }[]) => {
  const out = new Map<string, string[]>(nodes.map((n) => [n.id, []]));
  const inn = new Map<string, string[]>(nodes.map((n) => [n.id, []]));
  for (const e of edges) { out.get(e.from)!.push(e.to); inn.get(e.to)!.push(e.from); }

  // Iterative DFS: classify each (from,to) pair; a target still on the stack
  // marks a back edge. Iterative (not recursive) so a 1000-node chain can't
  // blow the call stack.
  const state = new Map<string, 1 | 2>();          // absent = unvisited, 1 = on stack, 2 = done
  const dropped = new Set<string>();               // "from|to" pairs (parallel edges share fate)
  const finished: string[] = [];                   // post-order; reversed = topological
  for (const root of nodes) {
    if (state.has(root.id)) continue;
    const stack: { id: string; i: number }[] = [{ id: root.id, i: 0 }];
    state.set(root.id, 1);
    while (stack.length) {
      const f = stack[stack.length - 1];
      const succs = out.get(f.id)!;
      if (f.i < succs.length) {
        const v = succs[f.i++];
        const s = state.get(v);
        if (s === 1) dropped.add(`${f.id}|${v}`);  // back edge — the cycle break
        else if (s === undefined) { state.set(v, 1); stack.push({ id: v, i: 0 }); }
        // s === 2: forward/cross edge — harmless for longest path
      } else { state.set(f.id, 2); finished.push(f.id); stack.pop(); }
    }
  }

  // Longest path over the kept (acyclic) edges, relaxed in topological order.
  // Sources start at 0; every kept edge pushes its target one column right.
  const depth = new Map<string, number>(nodes.map((n) => [n.id, 0]));
  for (let i = finished.length - 1; i >= 0; i--) {
    const u = finished[i];
    for (const v of out.get(u)!)
      if (!dropped.has(`${u}|${v}`))
        depth.set(v, Math.max(depth.get(v)!, depth.get(u)! + 1));
  }

  // Undirected components (edge direction is irrelevant to "disconnected").
  const comp = new Map<string, number>();
  let nComps = 0;
  for (const root of nodes) {
    if (comp.has(root.id)) continue;
    const c = nComps++;
    const queue = [root.id];
    comp.set(root.id, c);
    while (queue.length) {
      const u = queue.shift()!;
      for (const v of [...out.get(u)!, ...inn.get(u)!])
        if (!comp.has(v)) { comp.set(v, c); queue.push(v); }
    }
  }
  return { depth, comp, nComps, inn };
};

// ── tidy layout ──────────────────────────────────────────────────────────────

/** Layered left-to-right layout of the graph by dataflow depth. Deterministic;
 *  cycle-safe; the result never overlaps two cards. */
export function tidyLayout(graph: GraphDoc, sizeOf: (n: GraphNode) => Size): Placed[] {
  const byId = new Map(graph.nodes.map((n) => [n.id, n]));
  // Stale edges (nodes deleted, doc hand-edited) must not poison the depth
  // walk — layout only trusts edges whose both ends exist.
  const edges = graph.edges
    .filter((e) => byId.has(e.from.node) && byId.has(e.to.node))
    .map((e) => ({ from: e.from.node, to: e.to.node }));
  const { depth, comp, nComps, inn } = analyze(graph.nodes, edges);
  const size = new Map(graph.nodes.map((n) => [n.id, sizeOf(n)]));

  const placed = new Map<string, Placed>();
  let compTop = 0;                                 // running y offset for component stacking
  for (let c = 0; c < nComps; c++) {
    // Columns = depth buckets; membership is fixed before any ordering, so
    // column widths/heights (and thus x positions and the midline) are
    // knowable up front.
    const cols: string[][] = [];
    for (const n of graph.nodes)
      if (comp.get(n.id) === c) (cols[depth.get(n.id)!] ??= []).push(n.id);
    const colH = cols.map((col) =>
      col.reduce((h, id) => h + size.get(id)!.h, 0) + (col.length - 1) * ROW_GAP);
    const maxH = Math.max(...colH);

    let x = 0;
    for (let k = 0; k < cols.length; k++) {
      // Order rows by the average y-centre of already-placed upstream cards —
      // this is what keeps wires roughly horizontal. Sources (no placed
      // upstream) fall back to their current y so unconnected cards keep
      // their relative order; then current y, then id, so ties never flap.
      const key = (id: string): number => {
        const ups = inn.get(id)!.filter((p) => placed.has(p));
        return ups.length
          ? ups.reduce((s, p) => s + placed.get(p)!.y + size.get(p)!.h / 2, 0) / ups.length
          : byId.get(id)!.y;
      };
      const ordered = cols[k]
        .map((id) => ({ id, k1: key(id), k2: byId.get(id)!.y }))
        .sort((a, b) => a.k1 - b.k1 || a.k2 - b.k2 || (a.id < b.id ? -1 : 1))
        .map((r) => r.id);
      // Stack top-down, but start at the offset that centres this column on
      // the component's midline — short columns sit mid-height, not top-pinned.
      let y = compTop + (maxH - colH[k]) / 2;
      for (const id of ordered) {
        placed.set(id, { id, x, y });
        y += size.get(id)!.h + ROW_GAP;
      }
      x += Math.max(...cols[k].map((id) => size.get(id)!.w)) + COL_GAP;
    }
    compTop += maxH + COMPONENT_GAP;
  }
  // Emit in graph.nodes order so callers can zip against the doc.
  return graph.nodes.map((n) => placed.get(n.id)!);
}

// ── drop placement ───────────────────────────────────────────────────────────

interface Box { x: number; y: number; w: number; h: number }

/** True when `r` (inflated by DROP_MARGIN) clears every box. Strict
 *  inequalities so exactly-touching edges still count as clear. */
const clear = (x: number, y: number, s: Size, boxes: Box[]): boolean =>
  boxes.every((b) =>
    x - DROP_MARGIN >= b.x + b.w || x + s.w + DROP_MARGIN <= b.x ||
    y - DROP_MARGIN >= b.y + b.h || y + s.h + DROP_MARGIN <= b.y);

/** Collision-avoiding drop placement: nearest free spot to `desired`, scanned
 *  in a deterministic outward pattern. Always terminates. */
export function placeFree(
  desired: { x: number; y: number }, size: Size, boxes: Box[],
): { x: number; y: number } {
  if (clear(desired.x, desired.y, size, boxes)) return { x: desired.x, y: desired.y };
  // First a short down-right cascade — the familiar "new window" stagger, so
  // repeated drops on the same spot read as a stack.
  for (let i = 1; i <= 12; i++) {
    const x = desired.x + i * DROP_STEP, y = desired.y + i * DROP_STEP;
    if (clear(x, y, size, boxes)) return { x, y };
  }
  // Then widening square rings around `desired`. Any x beyond every box's
  // right edge is clear regardless of y, so a ring of radius `cap` (which
  // reaches that x) must contain a free spot — the loop provably terminates.
  const right = Math.max(...boxes.map((b) => b.x + b.w)) + DROP_MARGIN;
  const cap = Math.max(DROP_STEP, Math.ceil((right - desired.x) / DROP_STEP + 1) * DROP_STEP);
  for (let r = DROP_STEP; r <= cap; r += DROP_STEP) {
    for (let dx = -r; dx <= r; dx += DROP_STEP) {
      const dys = Math.abs(dx) === r
        ? Array.from({ length: 2 * r / DROP_STEP + 1 }, (_, i) => -r + i * DROP_STEP)
        : [-r, r];                                 // interior columns: ring edge only
      for (const dy of dys)
        if (clear(desired.x + dx, desired.y + dy, size, boxes))
          return { x: desired.x + dx, y: desired.y + dy };
    }
  }
  // Unreachable by the cap argument above, but belt-and-braces: just right of
  // everything is always free.
  return { x: right, y: desired.y };
}
