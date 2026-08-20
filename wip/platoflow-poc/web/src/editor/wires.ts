// Wire parts (WirePart, RubberWire) AND the two card gestures that make/route
// wires: the socket-drag gesture (with the T3 link-drag-search branch — drop a
// rubber wire on empty canvas to get a type-filtered palette) and the node-move
// gesture (with the T3 drop-on-wire splice branch). Split from parts.ts in W0
// (plan §2.5); the gestures moved here from cards.ts in W1/T3 so the wire
// behavior has one home. Fence: T3 (link-drag-search), T11 (wire badges +
// ghost highlight) own this file.
import {
  calpha, part, rect, rgb, v, vdist, wireDist,
  type Anchor, type Element, type GestureSpec, type Query, type Vec,
} from "gratify";
import type { GraphDoc, NodeKindInfo, NodeStatus, SlotRef, WireType } from "../contracts";
import type { EditorIntent } from "./doc";
import type { NodeCardProps } from "./cards";
import { anchorId, wireColor } from "./geom";

const SOCKET_GRAB = 15;
const SNAP_RADIUS = 30;
/** A release closer than this to the grabbed socket is a click, not a drop. */
const LINK_MIN_DRAG = 24;
/** Pointer-to-wire distance that arms the drop-on-wire splice. */
const SPLICE_DIST = 10;

// ── scene registry ───────────────────────────────────────────────────────────
// The gestures need doc-level context a part's props don't carry: the current
// graph (fresh ids for the palette pick), the wire list (splice hit-testing)
// and node rects (a wire dropped ON a card cancels instead of opening the
// palette). surface.makeView registers them every view build, so headless
// drive specs get them for free through the real view function.

export interface SpliceEdge {
  ekey: string;
  fromAnchor: string;
  toAnchor: string;
  from: SlotRef;
  to: SlotRef;
  type: WireType;
}

export interface NodeBox { id: string; x: number; y: number; w: number; h: number }

let sceneGraph: GraphDoc | null = null;
let sceneEdges: SpliceEdge[] = [];
let sceneNodes: NodeBox[] = [];
let sceneStatus: Record<string, NodeStatus> = {};

export function registerScene(
  graph: GraphDoc, edges: SpliceEdge[], nodes: NodeBox[],
  status: Record<string, NodeStatus> = {},
): void {
  sceneGraph = graph;
  sceneEdges = edges;
  sceneNodes = nodes;
  sceneStatus = status;
  // A hovered wire that no longer exists (deleted/undone) must release the
  // ghost highlight — its part will never render again to clear it.
  if (hoverWire && !edges.some((e) => e.ekey === hoverWire!.ekey)) setWireHover(null);
}

export const currentGraph = (): GraphDoc | null => sceneGraph;

const nodeAt = (p: Vec): NodeBox | undefined =>
  sceneNodes.find((b) => p.x >= b.x && p.x <= b.x + b.w && p.y >= b.y && p.y <= b.y + b.h);

// ── link-drag pending state (carried INSIDE the palette-open position) ───────
// doc.palette is `Vec | null`; a Vec is a plain `{x, y}`, so the wire drop
// opens the palette with a structurally-compatible object that also carries
// the dragged wire's endpoint. Esc / click-away / select all clear doc.palette
// and the pending link dies with it — no extra editor state.

export interface PendingLink { node: string; slot: string; dir: "in" | "out"; type: WireType }

export interface PaletteAt extends Vec { link?: PendingLink }

// ── splice candidate (render affordance) ─────────────────────────────────────

let spliceKey: string | null = null;
const setSplice = (k: string | null) => { spliceKey = k; };
/** Edge key of the wire the dragged node would splice into, or null. */
export const spliceTarget = (): string | null => spliceKey;

// ── T11 wire badges (visual only — no hit, no gesture) ───────────────────────
// The mid-point badge shows the value crossing the wire: the SOURCE node's
// status.summary is already the per-type text (scene "216 entities", table
// "14 rows × 3 cols", colormap/view domain). Statuses arrive via
// registerScene; the source node id is the first segment of the from-anchor.

/** Badges render at/above this view zoom. LOCAL const: T10's global semantic-
 *  zoom threshold in geom.ts had not landed when T11 shipped — import and
 *  delete this when it does. */
export const BADGE_MIN_ZOOM = 0.6;
export const BADGE_MAX_CHARS = 18;
/** Wires shorter than this (endpoint to endpoint) hide their badge — on a
 *  short wire the midpoint pill collides with the sockets it sits between
 *  (the T17 "known wart"; W9-C). ~90px clears a full badge + both dots. */
export const BADGE_MIN_WIRE_LEN = 90;

export const badgeVisible = (zoom: number): boolean => zoom >= BADGE_MIN_ZOOM;

/** Endpoint-to-endpoint room check — render and specs share the threshold. */
export const badgeFits = (a: Vec, b: Vec): boolean => vdist(a, b) >= BADGE_MIN_WIRE_LEN;

/** Hard-truncated badge text, or null when the source has no summary yet. */
export function badgeText(status: NodeStatus | undefined): string | null {
  const s = status?.summary;
  if (!s) return null;
  return s.length > BADGE_MAX_CHARS ? s.slice(0, BADGE_MAX_CHARS - 1) + "…" : s;
}

export type BadgeTone = "ok" | "dim" | "error";

/** pending (or no status yet — same thing) → dim, error → red tint, ok → full. */
export const badgeTone = (status: NodeStatus | undefined): BadgeTone =>
  status?.state === "error" ? "error"
  : !status || status.state === "pending" ? "dim"
  : "ok";

/** W13-B calm: badges are on-demand. Hidden at rest, faded in by the wire's
 *  hover channel — except error badges, which always show at full strength
 *  (a failure crossing a wire is not optional information). */
export const badgeAlpha = (tone: BadgeTone, hover: number): number =>
  tone === "error" ? 1 : Math.max(0, Math.min(1, hover));

/** Below this the badge is skipped entirely (no draw during the fade tail). */
export const BADGE_FADE_MIN = 0.02;

/** `node|dir|slot` anchor id → node id (see geom.anchorId). */
export const sourceOfAnchor = (fromAnchor: string): string => fromAnchor.split("|")[0];

const BADGE_H = 13;
// Cream theme (2026-08-19): badges are light pills now, not dark ones.
const BADGE_BG = rgb(255, 255, 255);
const BADGE_BG_ERR = rgb(248, 228, 225);
const BADGE_TEXT_ERR = rgb(180, 50, 40);

// ── T11 hover → 3D ghost highlight seam ──────────────────────────────────────
// Module-state hover key (same pattern as spliceTarget above): the WirePart
// render watches its own hover channel and reports scene-wire hover
// transitions here. The integration layer (main.ts, E fence) subscribes and
// maps source node id → SceneValue.entities → ViewerBridge.highlight.

export type WireHoverListener = (sourceNode: string | null) => void;

let hoverWire: { ekey: string; source: string } | null = null;
const hoverListeners: WireHoverListener[] = [];
/** Hover channel level that counts as hovered (approach-rate 16 crosses it
 *  ~3 frames after hover-in/out — instant enough for a ghost highlight). */
const HOVER_ON = 0.5;

/** Subscribe to scene-wire hover; fires with the hovered wire's SOURCE node
 *  id, and null on hover-out. Returns an unsubscribe. */
export function onWireHover(cb: WireHoverListener): () => void {
  hoverListeners.push(cb);
  return () => {
    const i = hoverListeners.indexOf(cb);
    if (i >= 0) hoverListeners.splice(i, 1);
  };
}

/** Source node id of the currently hovered scene wire, or null. */
export const hoveredWireSource = (): string | null => hoverWire?.source ?? null;

function setWireHover(next: { ekey: string; source: string } | null): void {
  if (hoverWire?.ekey === next?.ekey) return;
  hoverWire = next;
  for (const cb of hoverListeners) cb(next?.source ?? null);
}

// ── W9-C wire highlight (viewport pick → editor reverse link, design §4.4) ───
// The other direction of the hover seam above: the integration layer (main.ts,
// E fence) calls setWireHighlight when a 3D viewport pick lands, and matching
// wires draw brighter with a soft halo. Module state, same pattern as
// spliceTarget/onWireHover — WirePart's render reads it every frame.

let highlightKeys: ReadonlySet<string> | null = null;

/** CONTRACTS.md wire key `${from.node}.${from.slot}->${to.node}.${to.slot}` →
 *  internal ekey (geom.edgeKey form `node|slot>node|slot`). A key with no
 *  `->` (e.g. already-internal) passes through unchanged; a key that matches
 *  no registered wire simply highlights nothing. */
export function normalizeWireKey(key: string): string {
  const parts = key.split("->");
  if (parts.length !== 2) return key;
  const side = (s: string) => {
    const i = s.indexOf(".");
    return i < 0 ? s : `${s.slice(0, i)}|${s.slice(i + 1)}`;
  };
  return `${side(parts[0])}>${side(parts[1])}`;
}

/** Highlight the given wires (contract keys or internal ekeys); null clears.
 *  Unknown keys are ignored. Safe to call at any rate — no-ops when the set
 *  is unchanged. */
export function setWireHighlight(keys: ReadonlySet<string> | string[] | null): void {
  const next = keys === null ? null : new Set([...keys].map(normalizeWireKey));
  const same = next === null
    ? highlightKeys === null
    : highlightKeys !== null && next.size === highlightKeys.size &&
      [...next].every((k) => highlightKeys!.has(k));
  if (same) return;
  highlightKeys = next;
  // Prompt repaint: hover repaints because canvas pointer events call the
  // runtime's wake() — but a viewport pick never touches the editor canvas,
  // and the runtime sleeps 0.4s after input stops. attach() installs
  // window.gratifyResume = wake-this-runtime; poke it (no-op headless).
  (globalThis as { gratifyResume?: () => void }).gratifyResume?.();
}

/** Is this wire (internal ekey) in the current highlight set? Render + specs
 *  read through here. */
export const wireHighlighted = (ekey: string): boolean => highlightKeys?.has(ekey) ?? false;

// ── wire parts ───────────────────────────────────────────────────────────────

export interface WireProps {
  ekey: string;
  from: string;
  to: string;
  type: WireType;
  states?: Record<string, boolean>;
}

const SPLICE_GREEN = rgb(59, 165, 93);   // muted — matches --pf-green

export const WirePart = part("pf-wire")
  .props<WireProps>()
  .hit((n, pointer) => {
    const a = n.anchor?.(n.props.from), b = n.anchor?.(n.props.to);
    return !!a && !!b && wireDist(a, b, pointer) < 8;
  })
  .style((t, ch, p) => ({
    color: t.mix(wireColor(p.type), t.textBright, 0.5 * (ch.sel || 0)),
    /** W9-C viewport-pick highlight core — the hover/selection language
     *  (brighten toward textBright), one notch louder. */
    hi: t.mix(wireColor(p.type), t.textBright, 0.45),
    sel: ch.sel || 0,
  }))
  .render((n, p, s) => {
    const a = n.anchor?.(n.props.from), b = n.anchor?.(n.props.to);
    if (!a || !b) return;                                     // endpoint mid-exit
    // W9-C: a wire in the viewport-pick highlight set gets a soft halo under
    // the casing plus a brighter, thicker core stroke.
    const hl = wireHighlighted(n.props.ekey) ? 1 : 0;
    // W13-B calm: thinner base stroke, quieter under-casing and halo — the
    // resting wire field was the loudest layer of the canvas. Hover/selection/
    // highlight still thicken and brighten (feedback keeps its voice).
    if (hl) p.wire(a, b, calpha(s.hi, 0.22), 8);              // subtle glow halo
    p.wire(a, b, calpha(rgb(0, 0, 0), 0.05), 4);
    p.wire(a, b, calpha(hl ? s.hi : s.color, 0.92),
      1.8 + 1.6 * s.sel + 0.8 * n.ch.hover + 1.4 * hl);
    if (spliceKey === n.props.ekey) {                         // splice affordance
      p.wire(a, b, calpha(SPLICE_GREEN, 0.9), 4.2);
    }

    // T11 hover seam: scene wires report hover transitions (render is the one
    // per-frame place that sees the hover channel — same module-state pattern
    // as the splice affordance).
    const src = sourceOfAnchor(n.props.from);
    if (n.props.type === "scene") {
      const h = n.ch.hover || 0;
      // A deleted wire's instance keeps rendering while it animates out —
      // only a wire still in the registered edge list may (re)arm the hover.
      if (h > HOVER_ON && sceneEdges.some((e) => e.ekey === n.props.ekey)) {
        setWireHover({ ekey: n.props.ekey, source: src });
      } else if (h <= HOVER_ON && hoverWire?.ekey === n.props.ekey) {
        setWireHover(null);
      }
    }

    // T11 mid-point badge (visual only — hit() above is untouched, so wire
    // selection and T3's wireDist splice corridor are unaffected). The drawn
    // cubic has horizontal tangents with symmetric control offsets, so its
    // t=0.5 point IS the segment midpoint.
    const zoom = n.view?.zoom ?? 1;
    if (!badgeVisible(zoom)) return;
    if (!badgeFits(a, b)) return;   // W9-C: short wire — badge would sit on the sockets
    const text = badgeText(sceneStatus[src]);
    if (!text) return;
    const tone = badgeTone(sceneStatus[src]);
    // W13-B calm: non-error badges ride the wire's hover channel — invisible
    // at rest, fading in when the pointer approaches the wire. Error badges
    // stay pinned at full alpha.
    const ba = badgeAlpha(tone, n.ch.hover || 0);
    if (ba < BADGE_FADE_MIN) return;
    const mid = v((a.x + b.x) / 2, (a.y + b.y) / 2);
    const w = text.length * 5.4 + 10;
    const r = rect(mid.x - w / 2, mid.y - BADGE_H / 2, w, BADGE_H);
    const dim = (tone === "dim" ? 0.5 : 1) * ba;
    p.box(r, 6,
      calpha(tone === "error" ? BADGE_BG_ERR : BADGE_BG, 0.88 * dim),
      calpha(tone === "error" ? BADGE_TEXT_ERR : s.color, 0.55 * dim), 1);
    p.label(text, v(mid.x, mid.y + 0.5),
      calpha(tone === "error" ? BADGE_TEXT_ERR : s.color, 0.95 * dim),
      { size: 9, mono: true });
  })
  .press((n): EditorIntent => ({ k: "select", sel: { kind: "edge", key: n.props.ekey } }));

export const RubberWire = part("pf-rubber")
  .props<{ a: Vec; b: Vec; type: WireType; snapped: boolean }>()
  .style(() => ({}))
  .render((n, p) => {
    const base = wireColor(n.props.type);
    const c = n.props.snapped ? SPLICE_GREEN : calpha(base, 0.85);
    p.wire(n.props.a, n.props.b, c, n.props.snapped ? 2.8 : 2);
    p.dot(n.props.b, 4, c);
  });

// ── socket-drag gesture (cards attach it) ────────────────────────────────────

export interface SocketMetaLike { node: string; dir: "in" | "out"; slot: string; type: WireType }

const meta = (a: Anchor) => a.meta as SocketMetaLike;

/** A wire may join an output to an input of the SAME wire type on another node. */
const compatible = (from: SocketMetaLike, to: Anchor): boolean => {
  const m = meta(to);
  return !!m && m.dir !== from.dir && m.node !== from.node && m.type === from.type;
};

export interface WireDrag { from: Anchor; cursor: Vec; snap?: Anchor }

/** Drag from a socket to wire it up. Declines unless the press landed on a
 *  socket, so the move gesture gets the rest of the card. Release with no
 *  snap over EMPTY canvas = link-drag-search: open the palette at the drop
 *  point, filtered to kinds with a port compatible with the dragged wire. */
export const wireDragGesture: GestureSpec<NodeCardProps, WireDrag> = {
  begin(n, pointer, q) {
    const { id, info } = n.props;
    const ids = [
      ...info.inputs.map((s) => anchorId(id, "in", s.name)),
      ...info.outputs.map((s) => anchorId(id, "out", s.name)),
    ];
    for (const aid of ids) {
      const a = q.anchor(aid);
      if (a && vdist(a.pos, pointer) < SOCKET_GRAB) return { from: a, cursor: pointer };
    }
    return null;
  },
  move: (st, _n, pointer, q) => ({
    ...st,
    cursor: pointer,
    snap: q.nearestAnchor(pointer, SNAP_RADIUS, (c) => compatible(meta(st.from), c)),
  }),
  view: (st) => [RubberWire("rubber", {
    a: st.from.pos,
    b: st.snap?.pos ?? st.cursor,
    type: meta(st.from).type,
    snapped: !!st.snap,
  })],
  up(st, _n, p): EditorIntent | void {
    if (st.snap) {
      const a = meta(st.from), b = meta(st.snap);
      const [o, i] = a.dir === "out" ? [a, b] : [b, a];
      return {
        k: "graph",
        intent: { t: "connect", from: { node: o.node, slot: o.slot }, to: { node: i.node, slot: i.slot } },
      };
    }
    // T3 link-drag-search: unsnapped drop on empty canvas opens the palette
    // at the drop point, carrying the dragged endpoint for filtering + connect.
    const from = meta(st.from);
    if (vdist(st.from.pos, p) < LINK_MIN_DRAG) return;        // a click, not a drop
    if (nodeAt(p)) return;                                    // dropped on a card
    const at: PaletteAt = { x: p.x, y: p.y, link: { ...from } };
    return { k: "palette", at };
  },
};

// ── node-move gesture (cards attach it) ──────────────────────────────────────

const findSplice = (
  id: string, info: NodeKindInfo, p: Vec, q: Query,
): { edge: SpliceEdge; inSlot: string; outSlot: string } | null => {
  for (const e of sceneEdges) {
    if (e.from.node === id || e.to.node === id) continue;     // already an endpoint
    const inSlot = info.inputs.find((s) => s.type === e.type)?.name;
    const outSlot = info.outputs.find((s) => s.type === e.type)?.name;
    if (!inSlot || !outSlot) continue;                        // node can't pass this type through
    const a = q.anchor(e.fromAnchor), b = q.anchor(e.toAnchor);
    if (!a || !b) continue;
    if (wireDist(a.pos, b.pos, p) < SPLICE_DIST) return { edge: e, inSlot, outSlot };
  }
  return null;
};

// ── T13 move-extension seam ──────────────────────────────────────────────────
// Multi-select set-move, snap-to-align and alt-drag duplicate all live in
// surface.ts (T13's fence); the move gesture only delegates through this
// registry (same module-state pattern as registerScene above). A null/absent
// extension leaves the pre-T13 behavior byte-identical.

export interface MoveExt {
  /** Every move-gesture press (sees modifiers — shift-select, alt-dup). */
  begin(id: string, q: Query): void;
  /** Replacement intent for one live move step (target = grabbed node's new
   *  origin), or null for the default single-node move. `exclusive` suppresses
   *  the splice affordance (set-move / alt-dup never splice). */
  during(id: string, target: Vec): { intent: EditorIntent; exclusive: boolean } | null;
  /** Extra intents on release; `exclusive` suppresses the splice drop. */
  up(id: string): { intents: EditorIntent[]; exclusive: boolean } | null;
  /** Overlay elements while the gesture runs (align guides, dup ghosts). */
  view(): Element[];
}

let moveExt: MoveExt | null = null;
export const registerMoveExt = (m: MoveExt | null): void => { moveExt = m; };

/** Move the card; `during` dispatches live (the position spring makes it
 *  glide). Releasing the drag with the pointer over a type-compatible wire
 *  splices the node in — ONE batch: disconnect + two connects. */
export const nodeMoveGesture: GestureSpec<NodeCardProps, { off: Vec }> = {
  begin(n, pointer, q) {
    setSplice(null);
    moveExt?.begin(n.props.id, q);
    return { off: v(pointer.x - n.props.pos.x, pointer.y - n.props.pos.y) };
  },
  during(st, n, pointer, q): EditorIntent {
    const target = v(pointer.x - st.off.x, pointer.y - st.off.y);
    const ext = moveExt?.during(n.props.id, target) ?? null;
    setSplice(ext?.exclusive ? null
      : findSplice(n.props.id, n.props.info, pointer, q)?.edge.ekey ?? null);
    return ext?.intent
      ?? { k: "graph", intent: { t: "move", node: n.props.id, x: target.x, y: target.y } };
  },
  up(_st, n, p, q): EditorIntent | EditorIntent[] | void {
    const ext = moveExt?.up(n.props.id) ?? null;
    const hit = ext?.exclusive ? null : findSplice(n.props.id, n.props.info, p, q);
    setSplice(null);
    const out: EditorIntent[] = [...(ext?.intents ?? [])];
    if (hit) {
      const { edge, inSlot, outSlot } = hit;
      out.push({
        k: "graph",
        intent: {
          t: "batch",
          intents: [
            { t: "disconnect", to: edge.to },
            { t: "connect", from: edge.from, to: { node: n.props.id, slot: inSlot } },
            { t: "connect", from: { node: n.props.id, slot: outSlot }, to: edge.to },
          ],
        },
      });
    }
    if (out.length) return out;
  },
  view: () => moveExt?.view() ?? [],
};
