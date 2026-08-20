// The editor surface (grid, pan, key bindings), the HUD, and makeView — the
// app's view function assembling wires, cards and palette. Split from parts.ts
// in W0 (plan §2.5). Fence: T3 (drop-on-wire splice) and T13 (marquee) own it.
//
// T13 additions: the shift-drag marquee gesture, the pure snap-to-align math
// (snapMove), and the move extension (set-move, align guides, alt-drag
// duplicate) that wires.ts's nodeMoveGesture delegates to via registerMoveExt.
import {
  at, calpha, Free, Gesture, Label, Pan, part, rect, Stack, v,
  type Element, type GestureSpec, type Vec,
} from "gratify";
import type { Intent, NodeKindInfo, SubgraphSpec } from "../contracts";
import {
  copySelection, pasteIntent, selectedNodeIds, selFromIds, setPressShift,
  type EditorDoc, type EditorIntent,
} from "./doc";
import { anchorId, edgeKey, nodeLayout, subInfo, wiredInputsOf } from "./geom";
import { NodeCard } from "./cards";
import { palette } from "./palette";
import { registerMoveExt, registerScene, WirePart, type NodeBox, type SpliceEdge } from "./wires";

export interface SurfaceProps { hasSel: boolean }

const GRID = 28;

// ── T13 view-context registry ────────────────────────────────────────────────
// Same module-state pattern as wires.registerScene: makeView records the doc
// and node boxes every build, so the marquee and the move extension see the
// current selection/geometry — headless drive specs get it through the real
// view function for free.

let curDoc: EditorDoc | null = null;
let curBoxes: NodeBox[] = [];

// ── snap-to-align (pure — exported for specs) ────────────────────────────────

export const SNAP_DIST = 6;

/** Snap a prospective node origin against other nodes' left (x) and top (y)
 *  edges: nearest candidate within `dist` wins per axis. `gx`/`gy` report the
 *  snapped guide coordinate (null = no snap on that axis). */
export function snapMove(
  others: { x: number; y: number }[], x: number, y: number, dist = SNAP_DIST,
): { x: number; y: number; gx: number | null; gy: number | null } {
  let gx: number | null = null, gy: number | null = null;
  let bx = dist, by = dist;
  for (const o of others) {
    const dx = Math.abs(o.x - x), dy = Math.abs(o.y - y);
    if (dx <= bx) { bx = dx; gx = o.x; }
    if (dy <= by) { by = dy; gy = o.y; }
  }
  return { x: gx ?? x, y: gy ?? y, gx, gy };
}

// ── T13 move extension: set-move + align guides + alt-drag duplicate ─────────

interface MoveDrag {
  grab: string;
  /** Selection members with their begin-time offsets from the grabbed node's
   *  origin (constant for the whole drag — no per-frame doc staleness). */
  members: { id: string; dx: number; dy: number }[];
  alt: boolean;
  start: Vec;
  /** alt-dup: accumulated drag offset (copies are created on release). */
  off: Vec | null;
}

let drag: MoveDrag | null = null;
let guides: { x: number | null; y: number | null } | null = null;
/** Per-drag sequence for the set-move coalescing key: bumped on every begin
 *  so two drags of the same set are two undo entries (the single-node move
 *  key's cross-drag coalescing is frozen history policy — left untouched). */
let dragSeq = 0;

const GuideLine = part("pf-guide")
  .props<{ axis: "x" | "y"; at: number }>()
  .style((t) => ({ c: calpha(t.accent, 0.65) }))
  .render((n, p, s) => {
    const { axis, at: g } = n.props;
    if (axis === "x") p.line(v(g, -1e5), v(g, 1e5), s.c, 1);
    else p.line(v(-1e5, g), v(1e5, g), s.c, 1);
  });

const GhostBox = part("pf-ghost")
  .props<{ r: { x: number; y: number; w: number; h: number } }>()
  .style((t) => ({ fill: calpha(t.accent, 0.08), edge: calpha(t.accent, 0.55) }))
  .render((n, p, s) => {
    const { r } = n.props;
    p.box(rect(r.x, r.y, r.w, r.h), 10, s.fill, s.edge, 1);
  });

registerMoveExt({
  begin(id, q) {
    // .press has no modifier access — record shift for the "select" step's
    // membership toggle (doc.ts consumes it one-shot).
    setPressShift(q.mods.shift);
    guides = null;
    drag = null;
    dragSeq++;
    const g = curDoc?.graph.nodes.find((n) => n.id === id);
    if (!g || !curDoc) return;
    const selIds = selectedNodeIds(curDoc);
    const ids = selIds.length > 1 && selIds.includes(id) ? selIds : [id];
    const members = ids.flatMap((mid) => {
      const m = curDoc!.graph.nodes.find((n) => n.id === mid);
      return m ? [{ id: mid, dx: m.x - g.x, dy: m.y - g.y }] : [];
    });
    drag = { grab: id, members, alt: q.mods.alt, start: v(g.x, g.y), off: null };
  },

  during(id, target) {
    setPressShift(false);                    // a drag is not a click — no toggle
    if (!drag || drag.grab !== id) return null;
    if (drag.alt) {
      // duplicate: originals stay put; ghosts track the pointer (view below)
      drag.off = v(target.x - drag.start.x, target.y - drag.start.y);
      return { intent: { k: "noop" }, exclusive: true };
    }
    const moving = new Set(drag.members.map((m) => m.id));
    const s = snapMove(curBoxes.filter((b) => !moving.has(b.id)), target.x, target.y);
    guides = { x: s.gx, y: s.gy };
    if (drag.members.length <= 1) {
      // single node: default shape (splice + frozen coalescing stay intact)
      return {
        intent: { k: "graph", intent: { t: "move", node: id, x: s.x, y: s.y } },
        exclusive: false,
      };
    }
    // whole set as ONE batch per event, coalesced per-drag by the seq key so
    // every drag is exactly one undo step (batch alone never coalesces).
    const intents: Intent[] = drag.members.map((m) =>
      ({ t: "move", node: m.id, x: s.x + m.dx, y: s.y + m.dy }));
    return {
      intent: { k: "graphBatchKey", intent: { t: "batch", intents }, key: `moveSet:${id}:${dragSeq}` },
      exclusive: true,
    };
  },

  up(id) {
    guides = null;
    const d = drag;
    drag = null;
    if (!d || d.grab !== id || !curDoc) return null;
    // a set release never splices (which single node would it splice?)
    if (!d.alt) return d.members.length > 1 ? { intents: [], exclusive: true } : null;
    // alt-drag duplicate: paste the set at the drag offset, ONE batch + select
    if (d.off && Math.abs(d.off.x) + Math.abs(d.off.y) >= 4) {
      const clip = copySelection(curDoc.graph, d.members.map((m) => m.id));
      if (clip) {
        const { intent, ids } = pasteIntent(curDoc.graph, clip, d.off.x, d.off.y);
        return {
          intents: [{ k: "graph", intent }, { k: "select", sel: selFromIds(ids) }],
          exclusive: true,
        };
      }
    }
    return { intents: [], exclusive: true }; // alt press without a real drag
  },

  view() {
    const out: Element[] = [];
    if (guides?.x != null) out.push(GuideLine("guide-x", { axis: "x", at: guides.x }));
    if (guides?.y != null) out.push(GuideLine("guide-y", { axis: "y", at: guides.y }));
    if (drag?.alt && drag.off) {
      for (const m of drag.members) {
        const b = curBoxes.find((bb) => bb.id === m.id);
        if (b) out.push(GhostBox(`ghost-${m.id}`, {
          r: { x: b.x + drag.off.x, y: b.y + drag.off.y, w: b.w, h: b.h },
        }));
      }
    }
    return out;
  },
});

// ── T13 marquee (shift-drag on empty canvas) ─────────────────────────────────
// Binding choice: a PLAIN drag on empty canvas is the Pan() interactor — the
// canvas's only pan binding — so the marquee claims the drag only while Shift
// is held (declining lets the runtime fall through to pan). A sub-4px
// shift-"drag" is treated as a click and selects nothing new.

const marqueeRect = (a: Vec, b: Vec) =>
  rect(Math.min(a.x, b.x), Math.min(a.y, b.y), Math.abs(b.x - a.x), Math.abs(b.y - a.y));

const MarqueePart = part("pf-marquee")
  .props<{ a: Vec; b: Vec }>()
  .style((t) => ({ fill: calpha(t.accent, 0.08), edge: calpha(t.accent, 0.6) }))
  .render((n, p, s) => {
    p.box(marqueeRect(n.props.a, n.props.b), 2, s.fill, s.edge, 1);
  });

const marqueeGesture: GestureSpec<SurfaceProps, { a: Vec; b: Vec }> = {
  begin: (_n, p, q) => (q.mods.shift ? { a: p, b: p } : null),
  move: (st, _n, p) => ({ ...st, b: p }),
  view: (st) => [MarqueePart("marquee", { a: st.a, b: st.b })],
  up(st): EditorIntent | void {
    const r = marqueeRect(st.a, st.b);
    if (r.w < 4 && r.h < 4) return;                 // a click, not a marquee
    const ids = curBoxes
      .filter((b) => b.x < r.x + r.w && b.x + b.w > r.x && b.y < r.y + r.h && b.y + b.h > r.y)
      .map((b) => b.id);
    return { k: "select", sel: selFromIds(ids) };
  },
};

export const Surface = part("pf-surface")
  .props<SurfaceProps>()
  .fill()
  .hit(() => true)
  // W13-B calm: grid dots halved (0.4 → 0.2) — the resting canvas should read
  // as paper, not as a pattern competing with the cards.
  .style((t) => ({ dot: calpha(t.muted, 0.2) }))
  .render((n, p, s) => {
    const vp = n.view!;
    const x0 = Math.floor(-vp.pan.x / vp.zoom / GRID) * GRID;
    const y0 = Math.floor(-vp.pan.y / vp.zoom / GRID) * GRID;
    const x1 = (vp.w - vp.pan.x) / vp.zoom, y1 = (vp.h - vp.pan.y) / vp.zoom;
    for (let x = x0; x <= x1; x += GRID) for (let y = y0; y <= y1; y += GRID) p.dot(v(x, y), 1, s.dot);
  })
  .on(Gesture(marqueeGesture))                      // shift-drag = marquee …
  .on(Pan())                                        // … plain drag = pan
  .press((): EditorIntent => ({ k: "select", sel: null }))
  .keys({
    Delete: (): EditorIntent => ({ k: "deleteSel" }),
    Backspace: (): EditorIntent => ({ k: "deleteSel" }),
    Escape: (): EditorIntent => ({ k: "palette", at: null }),
  });

// ── view ─────────────────────────────────────────────────────────────────────
// (The old canvas HUD — "+" button + shortcut hint label — was removed 2026-08-19;
// the palette panel is open by default and Help ▸ Show instructions carries the text.)

/** `topInset` pushes the on-canvas HUD below the HTML menu bar when chrome is
 *  mounted — the wave-1 floating demo buttons covered the "+" button. */
export function makeView(
  kinds: NodeKindInfo[],
  info: (k: string) => NodeKindInfo | undefined,
  topInset = 0,
) {
  return (doc: EditorDoc): Element => {
    const world: Element[] = [];
    const spliceEdges: SpliceEdge[] = [];
    const nodeBoxes: NodeBox[] = [];
    // covers both single and multi selection — cards get the same `sel` state
    const selIds = new Set(selectedNodeIds(doc));

    // T16: a graph.sub node's ports come from its own SubgraphSpec — the view
    // hands every consumer (cards, anchors, wire gestures) the EFFECTIVE info.
    const effInfo = (n: { kind: string; sub?: SubgraphSpec }) => {
      const k = info(n.kind);
      return k ? subInfo(k, n.sub) : undefined;
    };

    for (const e of doc.graph.edges) {
      const src = doc.graph.nodes.find((n) => n.id === e.from.node);
      const type = src ? effInfo(src)?.outputs.find((o) => o.name === e.from.slot)?.type : undefined;
      const key = edgeKey(e);
      const fromAnchor = anchorId(e.from.node, "out", e.from.slot);
      const toAnchor = anchorId(e.to.node, "in", e.to.slot);
      spliceEdges.push({ ekey: key, fromAnchor, toAnchor, from: e.from, to: e.to, type: type ?? "scene" });
      world.push(WirePart(key, {
        ekey: key,
        from: fromAnchor,
        to: toAnchor,
        type: type ?? "scene",
        states: { sel: doc.sel?.kind === "edge" && doc.sel.key === key },
      }));
    }

    for (const n of doc.graph.nodes) {
      const kind = effInfo(n);
      if (!kind) continue;                              // unknown kind: nothing to draw
      const wiredInputs = wiredInputsOf(doc.graph.edges, n.id, kind);
      const l = nodeLayout(kind, { params: n.params, wiredInputs, w: n.w, bh: n.bh },
        { helpOpen: !!doc.helpOpen[n.id], zoom: 1 }, doc.status[n.id]);
      nodeBoxes.push({ id: n.id, x: n.x, y: n.y, w: l.w, h: l.h });
      world.push(NodeCard(n.id, {
        id: n.id,
        info: kind,
        pos: v(n.x, n.y),
        params: n.params,
        wiredInputs,
        w: n.w,
        bh: n.bh,
        // W13-B: no `display` prop — the eye chip and its glow are gone.
        // doc.graph.display still exists (MCP pinned fallback, main.ts).
        helpOpen: !!doc.helpOpen[n.id],
        status: doc.status[n.id],
        cmapWired: kind.kind === "viz.colorBy" &&
          doc.graph.edges.some((e) => e.to.node === n.id && e.to.slot === "colormap"),
        hideChips: !!doc.palette,
        states: { sel: selIds.has(n.id) },
      }));
    }

    // T13 view context (marquee hit-test, move extension's selection/boxes)
    curDoc = doc;
    curBoxes = nodeBoxes;

    // Wire/node context for the gestures in wires.ts (splice hit-test, the
    // dropped-on-a-card cancel, fresh ids for the link pick).
    registerScene(doc.graph, spliceEdges, nodeBoxes, doc.status);  // status → T11 wire badges

    if (doc.palette) world.push(palette(kinds, doc.palette));

    return Surface("root", { hasSel: !!doc.sel }, [
      Free("graph", {}, world),
    ]);
  };
}
