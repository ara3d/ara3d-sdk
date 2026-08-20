// Wave-11 Track E: the select.checklist node body — live check-boxes over the
// scene's actual values (design: "the value is the interface", the colormap
// precedent). THIRD in-node body organ, after colormap.ts (adorn-layer body)
// and the mini-grid (widgets.ts, drag-delegation gesture) — this file blends
// both patterns:
//
// - RENDER is an adorn-layer part (the colormapBody shape): one opaque panel
//   inside the frozen geom bodyRect drawing a row per `status.checklist` entry
//   — checkbox glyph (ticked = accent fill + check mark, unticked = empty
//   box), truncated value label, dim right-aligned entity count. More entries
//   than fit → the last line reads "…N more" (the picker-footer precedent).
//   The part is DECORATIVE (no press/gesture): gratify's adorn hit-test skips
//   it, so presses fall through to the card and the gesture chain below.
// - The TOGGLE gesture is a card-level GestureSpec (the W10-A gridClickGesture
//   delegation pattern, declared in cards.ts AFTER gridClickGesture and BEFORE
//   nodeMoveGesture): begin claims a press on a visible row only; a clean
//   click (≤ CLICK_SLOP) toggles that row's value; any real drag routes every
//   hook through nodeMoveGesture, so 120px of body never eats a card move.
// - Hover row tint via GNode.pointer (the W10-A freebie): the decorative part
//   never gets its own ch.hover (transparent to hit-testing), but n.pointer is
//   populated whenever the pointer is on the canvas — the same checkRowAt the
//   gesture hit-tests with picks the tinted row, so the tinted row IS the row
//   a click would toggle.
//
// Param storage is INVERTED (kinds.ts `excluded`): ticked values are ABSENT
// from the comma list, so new values arriving from upstream default to ticked.
// A toggle dispatches setParam `excluded` wrapped in a `batch` intent — batch
// never coalesces (doc.ts), so consecutive toggles are each their own undo
// entry (the T5 ramp-click precedent; NOTES wave-6 coalescing quirk).
//
// No checklist status yet (fresh node, evaluator mid-flight) → the body shows
// a dim "run to populate" placeholder and the gesture claims nothing.
//
// Known limit (comma is the storage delimiter): a value CONTAINING a comma
// cannot round-trip through `excluded`; IFC type/level strings don't carry
// commas in practice, so the PoC accepts it.
import {
  at, calpha, part, rect, rgb, v,
  type Element, type GestureSpec, type Rect, type Vec,
} from "gratify";
import type { NodeStatus } from "../contracts";
import type { EditorIntent } from "./doc";
import { bodyRect, nodeLayout, type NodeLayout } from "./geom";
// Runtime import is one-way (cards → checklist → wires); wires imports cards
// TYPE-only, so no cycle (the widgets.ts rule).
import { nodeMoveGesture } from "./wires";
// Type-only (erased at runtime — same rule widgets.ts uses).
import type { NodeCardProps } from "./cards";

export type ChecklistEntry = { value: string; on: boolean; count?: number };

// ── excluded-list algebra (pure; inverted storage) ───────────────────────────

/** Parse the `excluded` param (comma list, whitespace-tolerant) into values. */
export const excludedList = (excluded: unknown): string[] =>
  typeof excluded === "string" && excluded !== ""
    ? excluded.split(",").map((s) => s.trim()).filter((s) => s !== "")
    : [];

/** Is a value ticked? Inverted storage: ticked = NOT in the excluded list. */
export const isTicked = (excluded: unknown, value: string): boolean =>
  !excludedList(excluded).includes(value.trim());

/** The excluded list after toggling `value`: present → removed (re-tick),
 *  absent → appended (untick). Returns the new comma-joined param value. */
export function toggleExcluded(excluded: unknown, value: string): string {
  const list = excludedList(excluded);
  const val = value.trim();
  return (list.includes(val) ? list.filter((x) => x !== val) : [...list, val]).join(",");
}

// ── row geometry (pure; render and hit-test share it, per the widget rule) ───

/** Row pitch. bodyHeight 120 → bodyRect h 116 → 7 rows; a per-instance `bh`
 *  stretch reveals more (the mini-grid clamp rule). */
export const CHECK_ROW_H = 15;

export interface ChecklistLayout {
  /** One rect per VISIBLE data row (clipped to the area, like gridLayout). */
  rows: Rect[];
  visible: number;
  /** Entries that did not fit (the "…N more" count; 0 = all visible). */
  overflow: number;
  /** The "…N more" line's rect, when overflow > 0. */
  more: Rect | null;
}

/** Split the body area into CHECK_ROW_H rows: all `count` entries when they
 *  fit, else fit−1 data rows with the last line reserved for "…N more" (the
 *  picker-footer precedent). */
export function checklistLayout(area: Rect, count: number): ChecklistLayout {
  const fit = Math.floor(area.h / CHECK_ROW_H);
  if (fit < 1 || count < 1) return { rows: [], visible: 0, overflow: Math.max(0, count), more: null };
  const visible = count <= fit ? count : fit - 1;
  const rows = Array.from({ length: visible }, (_, i) =>
    rect(area.x, area.y + i * CHECK_ROW_H, area.w, CHECK_ROW_H));
  const overflow = count - visible;
  const more = overflow > 0
    ? rect(area.x, area.y + visible * CHECK_ROW_H, area.w, CHECK_ROW_H)
    : null;
  return { rows, visible, overflow, more };
}

/** Visible DATA row index under a world point, or null — the "…N more" line,
 *  space below the rows and outside-x all miss (they fall through to card
 *  move, like the mini-grid's header strip). Pure: gesture, hover tint and
 *  specs all share checklistLayout's geometry. */
export function checkRowAt(area: Rect, count: number, p: Vec): number | null {
  const g = checklistLayout(area, count);
  if (!g.visible) return null;
  if (p.x < area.x || p.x > area.right || p.y < area.y) return null;
  const i = Math.floor((p.y - area.y) / CHECK_ROW_H);
  return i < g.visible ? i : null;
}

// ── the toggle gesture (cards.ts declares it after gridClick, before move) ───

/** Same threshold the runtime uses to distinguish a click from a drag. */
const CLICK_SLOP = 4;

export interface ChecklistDrag {
  /** Checklist entry index claimed at press. */
  row: number;
  p0: Vec;
  moved: boolean;
  /** Delegated card-move state — a drag from a row still moves the card
   *  (splice + multi-select included), the gridClickGesture pattern. */
  mv: { off: Vec };
}

const cardLayout = (p: NodeCardProps, zoom: number): NodeLayout =>
  nodeLayout(p.info, { params: p.params, wiredInputs: p.wiredInputs, w: p.w, bh: p.bh },
    { helpOpen: p.helpOpen, zoom }, p.status);

/** Claims presses on visible checklist rows of a select.checklist card whose
 *  status carries live entries; declines chip mode, the "…N more" line, empty
 *  body space and the placeholder state (all fall through to card-move — the
 *  resize bands sit outside bodyRect and are declared earlier anyway). A clean
 *  click toggles the row's value: setParam `excluded` (inverted storage) inside
 *  a `batch`, so every toggle is its own undo entry. Any real drag routes
 *  every hook through nodeMoveGesture — card-move from the body is
 *  byte-identical to before this gesture existed. */
export const checklistGesture: GestureSpec<NodeCardProps, ChecklistDrag> = {
  begin(n, p, q) {
    if (n.props.info.kind !== "select.checklist") return null;
    const l = cardLayout(n.props, n.view?.zoom ?? 1);
    if (l.chip || l.bodyH <= 0) return null;
    const list = n.props.status?.checklist;
    if (!list?.length) return null;                     // placeholder claims nothing
    const area = bodyRect(l, rect(n.rect.x, n.rect.y, l.w, l.h));
    const row = checkRowAt(area, list.length, p);
    if (row === null) return null;                      // "…N more" / empty → move
    const mv = nodeMoveGesture.begin(n, p, q);
    if (!mv) return null;
    return { row, p0: p, moved: false, mv };
  },
  move(s, n, p, q) {
    const moved = s.moved || Math.hypot(p.x - s.p0.x, p.y - s.p0.y) > CLICK_SLOP;
    return { ...s, moved, mv: nodeMoveGesture.move?.(s.mv, n, p, q) ?? s.mv };
  },
  during(s, n, p, q): EditorIntent | void {
    if (!s.moved) return;                               // still a clean click
    return nodeMoveGesture.during?.(s.mv, n, p, q) as EditorIntent | void;
  },
  up(s, n, p, q): EditorIntent | EditorIntent[] | void {
    // Delegate FIRST (moveExt.up must always pair with its begin), then append
    // the toggle on a clean click. The card's `.press` (select) also fires on
    // a clean click, so a toggle click selects the node too — like any row.
    const out = nodeMoveGesture.up?.(s.mv, n, p, q) as EditorIntent | EditorIntent[] | void;
    const prev = out === undefined ? [] : Array.isArray(out) ? out : [out];
    if (s.moved) return prev.length ? prev : undefined;
    const entry = n.props.status?.checklist?.[s.row];
    if (!entry) return prev.length ? prev : undefined;  // status changed mid-press
    return [...prev, {
      // batch on purpose: defeats setParam coalescing → one undo entry per
      // toggle (the T5 ramp-click precedent)
      k: "graph",
      intent: {
        t: "batch",
        intents: [{
          t: "setParam", node: n.props.id, name: "excluded",
          value: toggleExcluded(n.props.params.excluded, entry.value),
        }],
      },
    }];
  },
  view: (s, q) => (s.moved ? nodeMoveGesture.view?.(s.mv, q) ?? [] : []),
};

// ── the body part (decorative — presses fall through to the card) ────────────

const truncate = (s: string, max: number) =>
  (s.length > max ? s.slice(0, Math.max(1, max - 1)) + "…" : s);

/** Estimated px per glyph at the row font sizes (the cards.ts footer rule). */
const PX_PER_CHAR = 5.2;
const BOX = 9;                                          // checkbox glyph size
const BOX_GAP = 5;                                      // box → label gap
const PAD_X = 3;

/** Opaque row panel: covers the card's default "no data" body text (the
 *  CmapBacking rule) and draws the rows. Decorative on purpose — gratify's
 *  adorn hit-test skips parts without interactors, so the host card keeps
 *  hover and the gesture chain gets every press. Hover tint reads n.pointer
 *  directly (world coords; undefined when the pointer leaves the canvas). */
const ChecklistRows = part("pf-checklist")
  .props<{ node: string; entries: ChecklistEntry[]; w: number; h: number }>()
  .size((p) => v(p.w, p.h))
  .style((t) => ({
    bg: t.mix(t.surface, rgb(0, 0, 0), 0.05),
    dim: t.textDim,
    bright: t.textBright,
    accent: t.accent,
  }))
  .render((n, p, s) => {
    const area = n.rect;
    p.box(area, 5, s.bg);
    const entries = n.props.entries;
    if (!entries.length) {
      // Track N hasn't populated the status yet (or the node never ran).
      p.label("run to populate", area.center, calpha(s.dim, 0.7), { size: 10 });
      return;
    }
    const g = checklistLayout(area, entries.length);
    const hover = n.pointer ? checkRowAt(area, entries.length, n.pointer) : null;
    g.rows.forEach((r, i) => {
      const e = entries[i];
      if (i === hover) p.box(r, 2, calpha(s.accent, 0.12));
      const bx = rect(r.x + PAD_X, r.center.y - BOX / 2, BOX, BOX);
      if (e.on) {
        p.box(bx, 2, calpha(s.accent, 0.9));
        // Cream theme: light check mark — the box is filled with the (mid-dark) accent.
        p.line(v(bx.x + 2, bx.center.y + 0.5), v(bx.x + 3.6, bx.bottom - 2.2), calpha(rgb(255, 255, 255), 0.95), 1.3);
        p.line(v(bx.x + 3.6, bx.bottom - 2.2), v(bx.right - 1.6, bx.y + 1.6), calpha(rgb(255, 255, 255), 0.95), 1.3);
      } else {
        p.box(bx, 2, calpha(rgb(0, 0, 0), 0.05), calpha(s.dim, 0.8), 1);
      }
      const countText = e.count !== undefined ? String(e.count) : "";
      const countW = countText ? countText.length * PX_PER_CHAR + 6 : 0;
      const chars = Math.max(2, Math.floor((r.w - PAD_X - BOX - BOX_GAP - countW) / PX_PER_CHAR));
      p.label(truncate(e.value, chars), v(bx.right + BOX_GAP, r.center.y),
        e.on ? s.bright : calpha(s.dim, 0.85), { align: "left", size: 9 });
      if (countText) {
        p.label(countText, v(r.right - PAD_X - 1, r.center.y),
          calpha(s.dim, 0.8), { align: "right", size: 8 });
      }
    });
    if (g.more) {
      p.label(`…${g.overflow} more`, v(g.more.x + PAD_X + 2, g.more.center.y),
        calpha(s.dim, 0.7), { align: "left", size: 9 });
    }
  });

// ── assembly (the one-line cards.ts hookup calls this) ───────────────────────

/** The checklist body: one decorative panel inside the frozen geom bodyRect of
 *  `cardRect`, drawing the live rows (or the placeholder). Narrow seam by
 *  design, mirroring colormapBody: node identity + params, the shared layout,
 *  the card's world rect, and the eval status. Returns [] when the kind
 *  reserves no body height. */
export function checklistBody(
  node: { id: string; params: Record<string, unknown> },
  l: NodeLayout,
  cardRect: Rect,
  status?: NodeStatus,
): Element[] {
  if (l.bodyH <= 0) return [];
  const area = bodyRect(l, cardRect);
  return [at(
    ChecklistRows("checklist", {
      node: node.id, entries: status?.checklist ?? [], w: area.w, h: area.h,
    }),
    v(area.x, area.y),
  )];
}
