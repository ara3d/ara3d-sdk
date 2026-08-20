// On-canvas node palette (panel + rows). Split from parts.ts in W0 (plan
// §2.5). Fence: T3 (link-drag-search + palette-at-cursor) owns this file.
//
// T3 additions: the palette-open position may carry a PendingLink (see
// wires.PaletteAt) — the palette then filters to kinds with a compatible port
// and a pick emits ONE batch intent (addNode + connect = one undo step).
// Plain picks drop the node AT the palette position, with a +24,+24 cascade
// when several nodes are added at the same spot (the stacking wart).
import { at, calpha, extendPart, Label, part, rgb, Stack, v, type Element, type GestureSpec, type Vec } from "gratify";
import type { Intent, NodeKindInfo, WireType } from "../contracts";
import { emptyDoc, freshNodeId } from "../reducer";
import { defaultParams, type EditorIntent } from "./doc";
import { CATEGORY_ORDER, HEADER_H, NODE_W, ROW_H } from "./geom";
import { currentGraph, type PaletteAt, type PendingLink } from "./wires";

const PALETTE_W = 178;

// ── cascade (palette-at-cursor wart fix) ─────────────────────────────────────
// Repeated adds at the SAME palette position used to stack exactly on top of
// each other. Consecutive picks from the same spot now walk +24,+24.

const CASCADE = 24;
let lastCascade: { base: Vec; n: number } | null = null;

/** Drop position for a plain palette pick: the palette's own world position,
 *  offset +24,+24 per consecutive add at the same spot. */
export function cascade(base: Vec): Vec {
  if (lastCascade && Math.abs(lastCascade.base.x - base.x) < 0.5
      && Math.abs(lastCascade.base.y - base.y) < 0.5) {
    lastCascade.n++;
    return v(base.x + CASCADE * lastCascade.n, base.y + CASCADE * lastCascade.n);
  }
  lastCascade = { base: v(base.x, base.y), n: 0 };
  return v(base.x, base.y);
}

/** Test seam: forget the cascade anchor (module state survives across mounts). */
export const resetCascade = (): void => { lastCascade = null; };

// ── link filtering + placement ───────────────────────────────────────────────

/** Kinds offering a port that can accept the dragged wire. Same type-equality
 *  rule as the socket snap (wires.compatible): WIRE types must match exactly. */
export const linkCompatible = (k: NodeKindInfo, link: PendingLink): boolean =>
  link.dir === "out"
    ? k.inputs.some((s) => s.type === link.type)
    : k.outputs.some((s) => s.type === link.type);

const firstSlot = (k: NodeKindInfo, dir: "in" | "out", type: WireType): { name: string; index: number } => {
  const list = dir === "in" ? k.inputs : k.outputs;
  const i = list.findIndex((s) => s.type === type);
  return { name: list[i].name, index: i };
};

/** Where the new node goes so its connecting socket lands at the drop point:
 *  dragging from an output places the node's left edge (input side) at the
 *  drop; dragging from an input places its right edge (output side) there.
 *  Vertical offset centres the chosen socket row on the drop point. */
export function linkPlacement(info: NodeKindInfo, link: PendingLink, drop: Vec): { pos: Vec; slot: string } {
  const side = link.dir === "out" ? "in" : "out";              // new node's connecting side
  const { name, index } = firstSlot(info, side, link.type);
  const y = drop.y - (HEADER_H + ROW_H / 2 + index * ROW_H);
  const x = side === "in" ? drop.x : drop.x - (info.width ?? NODE_W);
  return { pos: v(x, y), slot: name };
}

/** The ONE batch a link pick emits (addNode with schema defaults + connect),
 *  plus the follow-up select (which also closes the palette). Exported so
 *  specs can assert the exact shape. */
export function linkPickIntents(info: NodeKindInfo, link: PendingLink, drop: Vec): EditorIntent[] {
  const graph = currentGraph() ?? emptyDoc();
  const id = freshNodeId(graph);
  const { pos, slot } = linkPlacement(info, link, drop);
  const add: Intent = { t: "addNode", id, kind: info.kind, x: pos.x, y: pos.y, params: defaultParams(info) };
  const connect: Intent = link.dir === "out"
    ? { t: "connect", from: { node: link.node, slot: link.slot }, to: { node: id, slot } }
    : { t: "connect", from: { node: id, slot }, to: { node: link.node, slot: link.slot } };
  return [
    { k: "graph", intent: { t: "batch", intents: [add, connect] } },
    { k: "select", sel: { kind: "node", id } },                // also closes the palette
  ];
}

// ── parts ────────────────────────────────────────────────────────────────────

const PalettePanel = extendPart("pf-palette", Stack)
  .style((t) => ({
    fill: t.mix(t.bg, t.surface, 0.9),
    edge: calpha(t.accent, 0.5),
    shadow: calpha(rgb(0, 0, 0), 0.65),
  }))
  .render((n, p, s) => {
    p.push();
    p.alpha(0.3 + 0.7 * n.ch.enter);
    p.glow(s.shadow, 20, () => p.box(n.rect, 9, s.fill, s.edge, 1));
    p.pop();
  });

interface PaletteRowProps {
  kind: string;
  info: NodeKindInfo;
  label: string;
  at: Vec;
  link?: PendingLink;
  accent: number;
}

/** Click = pick. A gesture (not .press) because a link pick must dispatch TWO
 *  editor intents (the graph batch + the select that closes the palette) and
 *  only gesture `up` may return an array. */
const pickGesture: GestureSpec<PaletteRowProps, Record<string, never>> = {
  begin: () => ({}),
  up(_st, n, p): EditorIntent | EditorIntent[] | void {
    const r = n.rect;
    if (p.x < r.x || p.x > r.right || p.y < r.y || p.y > r.bottom) return;   // released off the row
    const { kind, info, at: base, link } = n.props;
    if (!link) return { k: "addKind", kind, at: cascade(base) };
    return linkPickIntents(info, link, base);
  },
};

const PaletteRow = part("pf-palette-row")
  .props<PaletteRowProps>()
  .intrinsic(PALETTE_W - 12, 22)
  .style((t, ch) => ({
    fill: calpha(t.accent, 0.22 * ch.hover + 0.14 * ch.press),
    text: t.mix(t.text, t.textBright, ch.hover),
  }))
  .render((n, p, s) => {
    p.box(n.rect, 5, s.fill);
    p.label(n.props.label, v(n.rect.x + 10, n.rect.center.y), s.text, { align: "left", size: 12 });
  })
  .gesture(pickGesture);

export function palette(kinds: NodeKindInfo[], atPos: Vec): Element {
  const link = (atPos as PaletteAt).link;
  const usable = link ? kinds.filter((k) => linkCompatible(k, link)) : kinds;
  const rows: Element[] = [];
  if (link) {
    rows.push(Label("link-note", {
      text: link.dir === "out" ? `${link.type} → …` : `… → ${link.type}`,
      dim: true, size: 10,
    }));
    if (!usable.length) rows.push(Label("pal-empty", { text: "no compatible nodes", dim: true, size: 11 }));
  }
  for (const cat of CATEGORY_ORDER) {
    const inCat = usable.filter((k) => k.category === cat);
    if (!inCat.length) continue;
    rows.push(Label(`cat-${cat}`, { text: cat.toUpperCase(), dim: true, size: 10 }));
    for (const k of inCat) {
      rows.push(PaletteRow(k.kind, { kind: k.kind, info: k, label: k.label, at: atPos, link, accent: 0 }));
    }
  }
  return at(PalettePanel("palette", { gap: 2, pad: 6 }, rows), atPos);
}
