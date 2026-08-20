// In-node param widget parts (T1, plan §3 W1; re-keyed as a REGISTRY in T15).
// Scrub / toggle / chips render INSIDE the 16px param row a node card already
// lays out — no new geometry, every draw and hit-test stays within the
// paramRowRect its layout row defines.
//
// Widget choice: geom.ts's `widgetFor` (FROZEN) is the sole classifier —
// schema → WidgetKind string. This file maps that string to an IMPLEMENTATION
// via `WIDGETS` (the §11.2 registry): one entry per WidgetKind with a row
// `draw` and optional gesture hooks. Adding a widget = add a WidgetKind in
// geom.ts + one registry entry here; cards.ts's render loop and the shared
// `widgetGesture` never grow another switch. Unknown/unregistered kinds fall
// back to the plain "row" entry (value text; click opens the popover).
//
//     number            → "scrub"    horizontal drag edits, step-quantized
//     boolean           → "toggle"   click flips, no popover
//     enum, ≤5 options  → "chips"    segmented chips, click one sets it
//     everything else   → "row"      value text; click opens the popover
//
// Gesture claiming (NOTES wave-5 finding 2): a drag across a param row used to
// begin the card-MOVE gesture. Gratify tries a part's gestures in declaration
// order and `begin` may decline with null — so `widgetGesture` is declared in
// cards.ts BETWEEN the wire gesture and the move gesture: it claims presses on
// scrub/toggle/chips rows (scrubbing no longer drags the node) and declines on
// "row" params, headers and everything else, which fall through to move.
// `resizeGesture` (T15) uses the same decline pattern for the card's right
// edge, declared BEFORE widgetGesture so the extreme edge wins the ~1px
// overlap with a param row's right end.
//
// T5: the `// gradient` region at the bottom of this file is reserved for the
// colormap body.
import {
  calpha, rect, rgb, v,
  type Color, type GestureSpec, type GNode, type Painter, type Query, type Rect, type Vec,
} from "gratify";
import type { NodeKindInfo, NodeStatus, ParamSchema, TableValue } from "../contracts";
import type { EditorIntent } from "./doc";
import { defaultOf } from "./doc";
import {
  bodyRect, clampBodyH, clampNodeW, NODE_W, nodeLayout, paramRowAt, paramRowRect,
  resizeHandleAt, widgetFor, type NodeLayout, type WidgetKind,
} from "./geom";
// Runtime import is one-way (cards → widgets → wires); wires imports cards
// TYPE-only, so no cycle. The grid-click gesture delegates drags to the real
// card-move gesture instead of eating them.
import { nodeMoveGesture } from "./wires";
// Type-only (erased at runtime — same rule wires.ts uses for NodeCardProps).
import type { NodeCardProps } from "./cards";

type NumberSchema = Extract<ParamSchema, { kind: "number" }>;
type EnumSchema = Extract<ParamSchema, { kind: "enum" }>;

/** Segmented chips stay readable up to this many options; beyond it the row
 *  keeps the popover (a future searchable picker is T8's job). */
export const CHIPS_MAX = 5;

/** Single source of truth is geom.ts's `widgetFor` (the mapping was folded in
 *  at the W1 join); this alias keeps T1-era call sites and specs stable. */
export const widgetForSchema = (schema: ParamSchema): WidgetKind => widgetFor(schema, false);

// ── scrub math (pure — specs hit this directly) ──────────────────────────────

/** Pixels of horizontal drag per quantum step. */
export const SCRUB_PX_PER_STEP = 2;

const decimalsOf = (x: number): number => {
  const s = String(x);
  const i = s.indexOf(".");
  return i < 0 ? 0 : s.length - i - 1;
};

/** Value after dragging `dx` px from `start`: step-quantized (schema.step ?? 1,
 *  ×0.1 when `fine`/Shift), then clamped to schema.min/max. */
export function scrubValue(schema: NumberSchema, start: number, dx: number, fine: boolean): number {
  const quantum = (schema.step ?? 1) * (fine ? 0.1 : 1);
  const raw = start + (dx / SCRUB_PX_PER_STEP) * quantum;
  let val = Number((Math.round(raw / quantum) * quantum).toFixed(decimalsOf(quantum)));
  if (schema.min !== undefined) val = Math.max(schema.min, val);
  if (schema.max !== undefined) val = Math.min(schema.max, val);
  return val;
}

// ── in-row geometry (render + hit-test share these; no private geometry) ─────

/** Fraction of the row the `name:` label keeps; chips fill the rest. Fixed
 *  (not measured) so gesture hit-tests need no painter. */
const CHIPS_LABEL_FRAC = 0.34;
const CHIP_GAP = 2;

export const chipsArea = (row: Rect): Rect => {
  const x = row.x + row.w * CHIPS_LABEL_FRAC;
  return rect(x, row.y + 1, row.right - 2 - x, row.h - 2);
};

/** One rect per option, equal-width segments across the chips area. */
export function chipRects(row: Rect, n: number): Rect[] {
  const a = chipsArea(row);
  const w = (a.w - CHIP_GAP * (n - 1)) / n;
  return Array.from({ length: n }, (_, i) => rect(a.x + i * (w + CHIP_GAP), a.y, w, a.h));
}

/** Option index under a point, or null (label zone / gaps miss). */
export function chipAt(row: Rect, n: number, p: Vec): number | null {
  const chips = chipRects(row, n);
  for (let i = 0; i < n; i++) {
    const c = chips[i];
    if (p.x >= c.x && p.x <= c.right && p.y >= c.y && p.y <= c.bottom) return i;
  }
  return null;
}

/** The toggle glyph (a small switch) sits at the row's right edge; the CLICK
 *  target is the whole row. */
export const toggleRect = (row: Rect): Rect =>
  rect(row.right - 26, row.center.y - 5, 20, 10);

// ── the widget gesture (cards.ts declares it between wire and move) ──────────

/** What the gesture needs from the card's props — a structural subset of
 *  cards.ts's NodeCardProps, kept import-free to avoid a runtime cycle. */
export interface WidgetHostProps {
  id: string;
  info: NodeKindInfo;
  params: Record<string, unknown>;
  wiredInputs: Set<string>;
  helpOpen: boolean;
  status?: NodeStatus;
  /** Per-instance width override (GraphNode.w, T15). */
  w?: number;
  /** Per-instance body-height override (GraphNode.bh). */
  bh?: number;
}

const hostLayout = (p: WidgetHostProps, zoom: number): NodeLayout =>
  nodeLayout(p.info, { params: p.params, wiredInputs: p.wiredInputs, w: p.w, bh: p.bh },
    { helpOpen: p.helpOpen, zoom }, p.status);

const currentOf = (p: WidgetHostProps, name: string): unknown => {
  const val = p.params[name];
  return val === undefined || val === null || val === ""
    ? defaultOf(p.info.params[name]) : val;
};

const setParam = (node: string, name: string, value: unknown): EditorIntent =>
  ({ k: "graph", intent: { t: "setParam", node, name, value } });

/** Same threshold the runtime uses to distinguish a click from a drag. */
const CLICK_SLOP = 4;

export type WidgetDrag =
  | { w: "scrub"; name: string; schema: NumberSchema; start: number; x0: number; last: number; dirty: boolean }
  | { w: "toggle"; name: string; p0: Vec }
  | { w: "chips"; name: string; options: string[]; p0: Vec };

type WNode = GNode<WidgetHostProps>;

/** Claims presses on rows whose registry entry claims them (scrub/toggle/
 *  chips); declines everywhere else so the card-move gesture (declared after
 *  it) still gets headers and "row" params. Scrub dispatches setParam per
 *  move — W0's coalescing key makes the whole drag ONE undo entry. Toggle/
 *  chips act on release iff the press stayed a click. The card's `.press`
 *  (select) still fires on any clean click, so clicking a widget also selects
 *  the node, exactly like a plain row.
 *
 *  T15: the per-kind behavior lives in the WIDGETS registry below; this spec
 *  is only the row hit-test + per-kind dispatch (`s.w` names the entry). */
export const widgetGesture: GestureSpec<WidgetHostProps, WidgetDrag> = {
  begin(n, p) {
    const l = hostLayout(n.props, n.view?.zoom ?? 1);
    const name = paramRowAt(l, n.rect.x, n.rect.y, p);
    if (!name) return null;
    const schema = n.props.info.params[name];
    return widgetImpl(widgetForSchema(schema)).begin?.(n, p, name, schema) ?? null;
  },
  move: (s, n, p, q) => widgetImpl(s.w).move?.(s, n, p, q) ?? s,
  during: (s, n): EditorIntent | void => widgetImpl(s.w).during?.(s, n),
  up: (s, n, p): EditorIntent | void => widgetImpl(s.w).up?.(s, n, p),
};

// ── per-instance resize (T15) ────────────────────────────────────────────────

/** Reset threshold: two clean grip clicks within this window = double-click. */
export const RESIZE_DBL_MS = 350;
let lastGripClick: { id: string; at: number } | null = null;

export interface ResizeDrag {
  /** Which handle claimed the press: "e" = width, "s" = body height, "se" = both. */
  hit: "e" | "s" | "se";
  w0: number; bh0: number; x0: number; y0: number;
  lastW: number; lastBh: number;
  /** Per-axis dirty flags for the last move (dispatch only what changed). */
  dw: boolean; dbh: boolean;
  /** The kind has a body at begin — a body-less card never edits bh, even
   *  from the corner grip. */
  hasBody: boolean;
  moved: boolean;
}

/** Drag the card's right edge ("e" → width), bottom edge ("s" → body height,
 *  body-carrying kinds only) or bottom-right grip ("se" → both) to set
 *  per-instance size. Declared in cards.ts BEFORE widgetGesture and the move
 *  gesture (same decline pattern), so an edge drag resizes instead of moving
 *  the card. Dispatches `resize` per move with only the changed axes — doc.ts's
 *  `resize:<id>` coalescing key makes a whole drag ONE undo entry across both
 *  axes. Width clamps to [NODE_W, NODE_W_MAX]; body height to
 *  [BODY_H_MIN, BODY_H_MAX]. Double-click on the GRIP resets BOTH to the kind
 *  defaults (`info.width ?? NODE_W`, `info.bodyHeight`) — note the wart: the
 *  resize intent cannot express "unset", so a reset stores the defaults as
 *  explicit values. Chip mode (zoom below the threshold) has no handle:
 *  resizeHandleAt declines. */
export const resizeGesture: GestureSpec<WidgetHostProps, ResizeDrag> = {
  begin(n, p) {
    const l = hostLayout(n.props, n.view?.zoom ?? 1);
    const hit = resizeHandleAt(l, n.rect.x, n.rect.y, p);
    if (!hit) return null;
    return { hit, w0: l.w, bh0: l.bodyH, x0: p.x, y0: p.y, lastW: l.w, lastBh: l.bodyH,
             dw: false, dbh: false, hasBody: l.bodyH > 0, moved: false };
  },
  move(s, _n, p) {
    const lastW = s.hit !== "s" ? clampNodeW(s.w0 + (p.x - s.x0)) : s.lastW;
    const lastBh = s.hit !== "e" && s.hasBody ? clampBodyH(s.bh0 + (p.y - s.y0)) : s.lastBh;
    return {
      ...s, lastW, lastBh, dw: lastW !== s.lastW, dbh: lastBh !== s.lastBh,
      moved: s.moved || Math.hypot(p.x - s.x0, p.y - s.y0) > CLICK_SLOP,
    };
  },
  during(s, n): EditorIntent | void {
    if (s.moved && (s.dw || s.dbh))
      return { k: "graph", intent: { t: "resize", node: n.props.id,
        ...(s.dw ? { w: s.lastW } : {}), ...(s.dbh ? { bh: s.lastBh } : {}) } };
  },
  up(s, n): EditorIntent | void {
    if (s.moved || s.hit !== "se") return;         // drag already dispatched live
    const now = Date.now();
    const dbl = lastGripClick !== null && lastGripClick.id === n.props.id
      && now - lastGripClick.at <= RESIZE_DBL_MS;
    lastGripClick = dbl ? null : { id: n.props.id, at: now };
    if (dbl) {
      const info = n.props.info;
      return { k: "graph", intent: { t: "resize", node: n.props.id,
        w: info.width ?? NODE_W,
        ...(info.bodyHeight !== undefined ? { bh: info.bodyHeight } : {}) } };
    }
  },
};

// ── row painters (cards.ts calls these from its param-row loop) ──────────────

export interface RowTones { dim: Color; bright: Color; accent: Color }

const truncate = (s: string, max: number) => (s.length > max ? s.slice(0, Math.max(1, max - 1)) + "…" : s);

const rowLabel = (p: Painter, row: Rect, name: string, dim: Color): number => {
  const label = truncate(name, 12) + ":";
  p.label(label, v(row.x + 5, row.center.y), dim, { align: "left", size: 10 });
  return p.measure.text(label, 10).x;
};

/** Number row: `name: value` plus a subtle ‹ › drag affordance on the right,
 *  and a min→max fill bar along the bottom edge when the schema bounds it. */
export function drawScrubRow(
  p: Painter, row: Rect, name: string, value: unknown, schema: NumberSchema, t: RowTones,
): void {
  const lw = rowLabel(p, row, name, t.dim);
  const num = Number(value ?? defaultOf(schema));
  p.label(truncate(String(num), 16), v(row.x + 5 + lw + 4, row.center.y), t.bright,
    { align: "left", size: 10 });
  p.label("‹ ›", v(row.right - 12, row.center.y), calpha(t.dim, 0.55), { size: 9 });
  if (schema.min !== undefined && schema.max !== undefined && schema.max > schema.min) {
    const f = Math.min(1, Math.max(0, (num - schema.min) / (schema.max - schema.min)));
    p.line(v(row.x + 2, row.bottom - 1), v(row.x + 2 + (row.w - 4) * f, row.bottom - 1),
      calpha(t.accent, 0.6), 1.5);
  }
}

/** Boolean row: `name: on|off` plus a small switch glyph at the right edge. */
export function drawToggleRow(
  p: Painter, row: Rect, name: string, on: boolean, t: RowTones,
): void {
  const lw = rowLabel(p, row, name, t.dim);
  p.label(on ? "on" : "off", v(row.x + 5 + lw + 4, row.center.y),
    on ? t.bright : calpha(t.dim, 0.85), { align: "left", size: 10 });
  const tr = toggleRect(row);
  p.box(tr, tr.h / 2, on ? calpha(t.accent, 0.55) : calpha(rgb(0, 0, 0), 0.08),
    on ? t.accent : t.dim, 1);
  const kx = on ? tr.right - tr.h / 2 : tr.x + tr.h / 2;
  p.dot(v(kx, tr.center.y), tr.h / 2 - 2, on ? rgb(255, 255, 255) : t.dim);  // white knob on the charcoal track
}

/** Enum row (≤5 options): the options as segmented chips; selected = filled. */
export function drawChipsRow(
  p: Painter, row: Rect, name: string, value: unknown, options: string[], t: RowTones,
): void {
  const labelChars = Math.max(3, Math.floor((row.w * CHIPS_LABEL_FRAC - 8) / 5.2));
  p.label(truncate(name, labelChars) + ":", v(row.x + 5, row.center.y), t.dim,
    { align: "left", size: 10 });
  const chips = chipRects(row, options.length);
  options.forEach((opt, i) => {
    const c = chips[i];
    const sel = opt === value;
    p.box(c, 3, sel ? calpha(t.accent, 0.8) : calpha(rgb(0, 0, 0), 0.05),
      sel ? t.accent : calpha(t.dim, 0.4), 1);
    p.label(truncate(opt, Math.max(1, Math.floor(c.w / 4.6))), c.center,
      sel ? rgb(255, 255, 255) : t.dim, { size: 8, weight: sel ? 600 : 400 });  // white on the charcoal chip
  });
}

/** Param value as the node draws it: current value, schema default when unset.
 *  (Lived in cards.ts until T15; moved here so the registry's "row" entry can
 *  draw without a cards→widgets cycle. cards.ts re-exports it.) */
export function paramDisplay(value: unknown, schema: ParamSchema): { text: string; isDefault: boolean } {
  const unset = value === undefined || value === null || value === "";
  const eff = unset ? defaultOf(schema) : value;
  const text =
    eff === true ? "on" :
    eff === false ? "off" :
    eff === "" || eff === undefined || eff === null ? "—" :
    String(eff);
  return { text, isDefault: unset };
}

/** Plain row: `name: value` (dimmed when showing the schema default). The
 *  fallback renderer — every WidgetKind without its own draw uses it, and so
 *  does any unknown kind string. */
export function drawPlainRow(p: Painter, row: Rect, ctx: WidgetDrawCtx): void {
  const disp = paramDisplay(ctx.raw, ctx.schema);
  const label = truncate(ctx.name, 12) + ":";
  p.label(label, v(row.x + 5, row.center.y), ctx.tones.dim, { align: "left", size: 10 });
  const lw = p.measure.text(label, 10).x;
  p.label(truncate(disp.text, 22), v(row.x + 5 + lw + 4, row.center.y),
    disp.isDefault ? calpha(ctx.tones.dim, 0.75) : ctx.tones.bright, { align: "left", size: 10 });
}

// ── the widget registry (T15, plan §11.2) ────────────────────────────────────
// geom.widgetFor (frozen) classifies schema → WidgetKind; WIDGETS maps that
// string to an implementation. cards.ts's param-row loop calls
// `widgetImpl(row.widget).draw(...)`; widgetGesture routes begin/move/during/
// up through the same entries. New widgets REGISTER here instead of editing
// switches in cards.ts.

/** What a row draw gets: the row's schema+values and the card's tones. */
export interface WidgetDrawCtx {
  name: string;
  /** Raw doc value (may be unset — draw decides how defaults look). */
  raw: unknown;
  /** Effective value (schema default substituted when unset). */
  value: unknown;
  schema: ParamSchema;
  tones: RowTones;
}

export interface WidgetImpl {
  draw(p: Painter, row: Rect, ctx: WidgetDrawCtx): void;
  /** Claim a press on this row: return drag state, or null/absent to decline
   *  (the press falls through to the card-move gesture / popover path). */
  begin?(n: WNode, p: Vec, name: string, schema: ParamSchema): WidgetDrag | null;
  move?(s: WidgetDrag, n: WNode, p: Vec, q: Query): WidgetDrag;
  during?(s: WidgetDrag, n: WNode): EditorIntent | void;
  up?(s: WidgetDrag, n: WNode, p: Vec): EditorIntent | void;
}

const noDraw = (): void => {};

export const WIDGETS: Record<WidgetKind, WidgetImpl> = {
  /** Wired param — row is h = 0, never drawn or hit. */
  hidden: { draw: noDraw },
  row: { draw: drawPlainRow },
  // Declared WidgetKinds the classifier doesn't emit yet (picker rows open the
  // DOM picker via the popover path; textPreview/gradient render elsewhere):
  // they draw as plain rows so a future classifier change degrades gracefully.
  picker: { draw: drawPlainRow },
  textPreview: { draw: drawPlainRow },
  gradient: { draw: drawPlainRow },

  scrub: {
    draw: (p, row, c) =>
      drawScrubRow(p, row, c.name, Number(c.value), c.schema as NumberSchema, c.tones),
    begin(n, p, name, schema) {
      const start = Number(currentOf(n.props, name));
      return { w: "scrub", name, schema: schema as NumberSchema, start, x0: p.x, last: start, dirty: false };
    },
    move(s, _n, p, q) {
      if (s.w !== "scrub") return s;
      const last = scrubValue(s.schema, s.start, p.x - s.x0, q.mods.shift);
      return { ...s, last, dirty: last !== s.last };
    },
    during(s, n): EditorIntent | void {
      if (s.w === "scrub" && s.dirty) return setParam(n.props.id, s.name, s.last);
      // scrub has no `up`: values were dispatched live (one undo entry via key)
    },
  },

  toggle: {
    draw: (p, row, c) => drawToggleRow(p, row, c.name, c.value === true, c.tones),
    begin: (_n, p, name) => ({ w: "toggle", name, p0: p }),
    up(s, n, p): EditorIntent | void {
      if (s.w !== "toggle") return;
      if (Math.hypot(p.x - s.p0.x, p.y - s.p0.y) > CLICK_SLOP) return;
      return setParam(n.props.id, s.name, !currentOf(n.props, s.name));
    },
  },

  chips: {
    draw: (p, row, c) =>
      drawChipsRow(p, row, c.name, c.value, (c.schema as EnumSchema).options, c.tones),
    begin: (_n, p, name, schema) =>
      ({ w: "chips", name, options: (schema as EnumSchema).options, p0: p }),
    up(s, n, p): EditorIntent | void {
      if (s.w !== "chips") return;
      if (Math.hypot(p.x - s.p0.x, p.y - s.p0.y) > CLICK_SLOP) return;
      // which segment was released on? (label zone / gap = no change)
      const l = hostLayout(n.props, n.view?.zoom ?? 1);
      const i = l.paramNames.indexOf(s.name);
      if (i < 0) return;
      const row = paramRowRect(l, rect(n.rect.x, n.rect.y, l.w, l.h), i);
      const hit = chipAt(row, s.options.length, p);
      if (hit !== null) return setParam(n.props.id, s.name, s.options[hit]);
    },
  },
};

/** Registry lookup with the "row" fallback for unknown kind strings. */
export const widgetImpl = (kind: string): WidgetImpl =>
  (WIDGETS as Record<string, WidgetImpl | undefined>)[kind] ?? WIDGETS.row;

// ── gradient ─────────────────────────────────────────────────────────────────
// Reserved for T5's colormap body (viz.colormap bodyHeight 40).

// ── mini-grid (T9, plan §3 W2 / study #10) ───────────────────────────────────
// A read-only ~5×6 table page drawn in the node BODY (bodyRect). The page
// itself (`NodeStatus.tablePreview`) is derived at eval time by main.ts via
// `tablePreviewOf`. Rendering degrades gracefully: a kind without `bodyHeight`
// has bodyH = 0, so no grid is drawn and the card looks exactly as before.
// Wave 10: rows are CLICKABLE — a clean click fires the onGridRowClick seam
// (main.ts maps it to a 3D highlight, mirroring the data-grid pane's
// row-click); a drag delegates to the real card-move gesture, so dragging a
// card by its grid body still moves it.

/** Preview-slice row cap. Sized to fill a body dragged to BODY_H_MAX (400):
 *  floor((400−4−GRID_HEADER_H)/GRID_ROW_H) = 29. gridLayout stays
 *  height-clamped, so the default 88px body still shows 5 rows — a taller
 *  per-instance body (GraphNode.bh) reveals more. */
export const GRID_MAX_ROWS = 29;
export const GRID_MAX_COLS = 6;
export const GRID_HEADER_H = 12;
export const GRID_ROW_H = 13;
const GRID_COL_GAP = 3;

/** The eval-side slice: first GRID_MAX_ROWS rows × GRID_MAX_COLS leading
 *  columns of a node's table output. Pure — main.ts calls it per table node;
 *  specs hit it directly. */
export function tablePreviewOf(t: TableValue): TableValue {
  return {
    columns: t.columns.slice(0, GRID_MAX_COLS),
    rows: t.rows.slice(0, GRID_MAX_ROWS).map((r) => r.slice(0, GRID_MAX_COLS)),
  };
}

export interface MiniGridLayout {
  /** one rect per column, header strip */
  header: Rect[];
  /** rows × cols cell rects (rows clipped to what fits the area) */
  cells: Rect[][];
  rows: number;
  cols: number;
  colW: number;
}

/** Pure cell geometry: equal-width columns across the body area, a header
 *  strip, then as many GRID_ROW_H rows as area height allows (≤ preview rows).
 *  Render and specs share it; nothing hit-tests it (read-only body). */
export function gridLayout(area: Rect, t: TableValue): MiniGridLayout {
  const cols = Math.min(t.columns.length, GRID_MAX_COLS);
  if (cols < 1 || area.h < GRID_HEADER_H + GRID_ROW_H) {
    return { header: [], cells: [], rows: 0, cols: 0, colW: 0 };
  }
  const colW = (area.w - GRID_COL_GAP * (cols - 1)) / cols;
  const colX = (c: number) => area.x + c * (colW + GRID_COL_GAP);
  const header = Array.from({ length: cols }, (_, c) =>
    rect(colX(c), area.y, colW, GRID_HEADER_H));
  const rows = Math.min(
    t.rows.length, GRID_MAX_ROWS,
    Math.floor((area.h - GRID_HEADER_H) / GRID_ROW_H));
  const cells = Array.from({ length: rows }, (_, r) =>
    Array.from({ length: cols }, (_, c) =>
      rect(colX(c), area.y + GRID_HEADER_H + r * GRID_ROW_H, colW, GRID_ROW_H)));
  return { header, cells, rows, cols, colW };
}

/** Visible row index under a world point, or null (header strip, area below
 *  the last visible row, outside the area). Pure — gesture, hover tint and
 *  specs all share gridLayout's geometry, so a drawn row and its hit-test can
 *  never drift. The index is into the PREVIEW page (what the user sees), not
 *  the full table. */
export function gridRowAt(area: Rect, t: TableValue, p: Vec): number | null {
  const g = gridLayout(area, t);
  if (!g.rows) return null;
  if (p.x < area.x || p.x > area.right) return null;
  const y0 = area.y + GRID_HEADER_H;
  if (p.y < y0) return null;
  const r = Math.floor((p.y - y0) / GRID_ROW_H);
  return r < g.rows ? r : null;
}

// ── grid-row click → 3D highlight seam (wave 10; wires.ts onWireHover shape) ─
// Module-state listener list: the gesture below reports clean row clicks here;
// the integration layer (main.ts, E fence) subscribes and maps
// (nodeId, rowIndex) → status.tablePreview row → entity indices →
// setRowHighlight, mirroring the data-grid pane's rowClicker.

export type GridRowClickListener = (nodeId: string, rowIndex: number) => void;

let lastGridRowClick: { node: string; row: number } | null = null;
const gridClickListeners: GridRowClickListener[] = [];

/** Subscribe to mini-grid row clicks; fires with the node id and the clicked
 *  PREVIEW row index. Returns an unsubscribe. */
export function onGridRowClick(cb: GridRowClickListener): () => void {
  gridClickListeners.push(cb);
  return () => {
    const i = gridClickListeners.indexOf(cb);
    if (i >= 0) gridClickListeners.splice(i, 1);
  };
}

/** Last clicked grid row, or null — the spec accessor (hoveredWireSource's
 *  role in the wave-7 hover seam). Updates even with zero listeners. */
export const clickedGridRow = (): { node: string; row: number } | null => lastGridRowClick;

function fireGridRowClick(node: string, row: number): void {
  lastGridRowClick = { node, row };
  for (const cb of gridClickListeners) cb(node, row);
}

// ── the grid-click gesture (cards.ts declares it between widget and move) ────

export interface GridClickDrag {
  /** Preview row index claimed at press. */
  row: number;
  p0: Vec;
  moved: boolean;
  /** Delegated card-move state — a drag from a grid row still moves the card
   *  (splice + multi-select extensions included), unlike toggle/chips rows
   *  which eat their drags. */
  mv: { off: Vec };
}

/** Claims presses on VISIBLE mini-grid rows: kinds with a body whose status
 *  carries a tablePreview and no chart (the chart body owns the area when both
 *  are present — mirrors the render branch). Declines chip mode, the header
 *  strip and empty body space (those fall through to card-move), and never
 *  sees the resize bands — resizeGesture is declared before it and the bands
 *  sit outside bodyRect anyway. A clean click (≤ CLICK_SLOP) fires the
 *  onGridRowClick seam on release; any real drag routes every hook through
 *  nodeMoveGesture, so card-move behavior from the body is byte-identical to
 *  before this gesture existed. */
export const gridClickGesture: GestureSpec<NodeCardProps, GridClickDrag> = {
  begin(n, p, q) {
    const l = hostLayout(n.props, n.view?.zoom ?? 1);
    if (l.chip || l.bodyH <= 0) return null;
    const st = n.props.status;
    const preview = st?.tablePreview;
    if (!preview?.columns.length) return null;          // nothing drawn to click
    if (st?.chart?.values.length) return null;          // chart body wins the area
    const area = bodyRect(l, rect(n.rect.x, n.rect.y, l.w, l.h));
    const row = gridRowAt(area, preview, p);
    if (row === null) return null;                      // header / empty → move
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
    // Delegate FIRST (moveExt.up must always pair with its begin), then fire
    // the seam on a clean click. The card's `.press` (select) also fires on a
    // clean click, so clicking a row selects the node too — like any row.
    const out = nodeMoveGesture.up?.(s.mv, n, p, q) as EditorIntent | EditorIntent[] | void;
    if (!s.moved) fireGridRowClick(n.props.id, s.row);
    return out;
  },
  view: (s, q) => (s.moved ? nodeMoveGesture.view?.(s.mv, q) ?? [] : []),
};

/** Draw the mini-grid into the body area: dimmer header row, separator line,
 *  truncated cell text, faint row rules. Null/empty cells render as "—".
 *  `pointer` (wave 10): when the card is hovered, the row under the pointer
 *  gets a faint accent tint — the click affordance. Same gridRowAt hit-test
 *  as the gesture, so the tinted row IS the row a click would fire. */
export function drawMiniGrid(
  p: Painter, area: Rect, t: TableValue, tones: RowTones, pointer?: Vec,
): void {
  const g = gridLayout(area, t);
  if (!g.cols) return;
  const hover = pointer ? gridRowAt(area, t, pointer) : null;
  const chars = Math.max(2, Math.floor(g.colW / 4.6));
  g.header.forEach((r, c) =>
    p.label(truncate(String(t.columns[c]), chars), v(r.x + 2, r.center.y),
      calpha(tones.dim, 0.8), { align: "left", size: 8, weight: 600 }));
  p.line(v(area.x, area.y + GRID_HEADER_H - 1), v(area.right, area.y + GRID_HEADER_H - 1),
    calpha(tones.dim, 0.35), 1);
  g.cells.forEach((rowRects, ri) => {
    if (ri === hover) {
      p.box(rect(area.x, rowRects[0].y, area.w, GRID_ROW_H), 2, calpha(tones.accent, 0.12));
    }
    if (ri > 0) {
      const y = rowRects[0].y;
      p.line(v(area.x, y), v(area.right, y), calpha(tones.dim, 0.12), 1);
    }
    rowRects.forEach((r, c) => {
      const cell = t.rows[ri]?.[c];
      const empty = cell === null || cell === undefined || cell === "";
      p.label(truncate(empty ? "—" : String(cell), chars), v(r.x + 2, r.center.y),
        empty ? calpha(tones.dim, 0.6) : tones.bright, { align: "left", size: 8 });
    });
  });
}
