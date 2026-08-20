// Node geometry + socket naming. EVERY consumer — the parts (render/anchors),
// the wire gesture, the hover tooltip and the param-row click — computes
// positions through `nodeLayout`, so a drawn pixel and its hit-test can never
// drift apart. Geometry is a pure function of (NodeKindInfo, node state, ui state, status).
//
// ── FROZEN (W0, 2026-08-09) ──────────────────────────────────────────────────
// This file is SUPERVISOR-OWNED after the W0 signature change. Tracks READ it;
// they extend behavior through `widgetFor` return values and per-row heights,
// never through signature edits. T10/T15's later edits here are supervisor-
// delegated, one track at a time (plan §2.4 / risk §5.1).
import { rgb, rect, v, type Color, type Rect, type Vec } from "gratify";
import { WIRE_COLORS, type GraphEdge, type NodeKindInfo, type NodeStatus, type ParamSchema, type SubgraphSpec, type WireType } from "../contracts";

// ── T16 dynamic ports (supervisor-delegated edit, 2026-08-09) ────────────────
/** The ONE dynamic-arity kind: a graph.sub node's ports come from its own
 *  SubgraphSpec, not from kinds.ts (which declares it with empty port lists).
 *  Every layout/hit-test/anchor consumer builds the card from THIS effective
 *  info; the layout walk itself is untouched — it still just reads
 *  info.inputs/outputs. No sub (every other kind) returns `info` unchanged. */
export const subInfo = (info: NodeKindInfo, sub: SubgraphSpec | undefined): NodeKindInfo =>
  !sub ? info : {
    ...info,
    inputs: sub.inputs.map((p) => ({ name: p.name, type: p.type, doc: `promoted → ${p.inner.node}.${p.inner.slot}` })),
    outputs: sub.outputs.map((p) => ({ name: p.name, type: p.type, doc: `promoted ← ${p.inner.node}.${p.inner.slot}` })),
  };

export const NODE_W = 188;
export const HEADER_H = 26;
export const ROW_H = 18;
export const PARAM_H = 16;
export const FOOTER_H = 20;
export const HELP_LINE_H = 13;
export const HELP_PAD = 5;
export const HELP_MAX_LINES = 16;
/** Chars per wrapped help line at font size 10 inside NODE_W − 20px padding. */
export const HELP_WRAP_CHARS = 32;
// ── T10 semantic zoom (supervisor-delegated edit, 2026-08-09) ────────────────
/** Below this view zoom a node renders as title + status chip only ("chip
 *  mode"). At exactly the threshold the full layout still applies. */
export const ZOOM_CHIP_THRESHOLD = 0.5;
/** Height of the status-chip strip under the header in chip mode. */
export const CHIP_H = 10;

export type SocketDir = "in" | "out";

export interface SocketMeta {
  node: string;
  dir: SocketDir;
  slot: string;
  type: WireType;
}

export const slotRows = (info: NodeKindInfo): number =>
  Math.max(1, info.inputs.length, info.outputs.length);

// ── help text (canvas-rendered, so wrapping must be deterministic) ───────────

export interface HelpLine { text: string; tone: "doc" | "detail" | "warn" | "error" }

/** Greedy word wrap on a fixed character budget. Pure — geometry and render
 *  both call it, so the help area's height always matches its text. */
export function wrapPlain(text: string, maxChars = HELP_WRAP_CHARS): string[] {
  const out: string[] = [];
  for (const para of text.split("\n")) {
    let line = "";
    for (const word of para.split(/\s+/).filter(Boolean)) {
      let w = word;
      while (w.length > maxChars) {              // hard-break pathological words
        if (line) { out.push(line); line = ""; }
        out.push(w.slice(0, maxChars));
        w = w.slice(maxChars);
      }
      if (!line) line = w;
      else if (line.length + 1 + w.length <= maxChars) line += " " + w;
      else { out.push(line); line = w; }
    }
    out.push(line);                              // empty paragraph = blank line
  }
  while (out.length && out[out.length - 1] === "") out.pop();
  return out;
}

/** The expando's content: kind description, then status.detail (e.g. Ask AI's
 *  generated SQL), then the full warning text when the status carries one
 *  (W9-C — the footer only fits a truncated "⚠ " line), then the error
 *  message when the node is red. */
export function helpLines(info: NodeKindInfo, status?: NodeStatus): HelpLine[] {
  const lines: HelpLine[] = wrapPlain(info.description).map((text) => ({ text, tone: "doc" as const }));
  if (status?.detail) {
    lines.push({ text: "", tone: "detail" });
    for (const text of wrapPlain(status.detail)) lines.push({ text, tone: "detail" });
  }
  if (status?.warning) {
    lines.push({ text: "", tone: "warn" });
    for (const text of wrapPlain(`⚠ ${status.warning}`)) lines.push({ text, tone: "warn" });
  }
  if (status?.state === "error" && status.message) {
    lines.push({ text: "", tone: "error" });
    for (const text of wrapPlain(`error: ${status.message}`)) lines.push({ text, tone: "error" });
  }
  if (lines.length > HELP_MAX_LINES) {
    const cut = lines.slice(0, HELP_MAX_LINES - 1);
    cut.push({ text: "…", tone: lines[HELP_MAX_LINES - 1].tone });
    return cut;
  }
  return lines;
}

// ── widgets ──────────────────────────────────────────────────────────────────

/** How a param row renders in the node card. W0: everything is a plain "row";
 *  "hidden" applies only when the connected-input rule collapses a wired row.
 *  W1+ tracks (T1/T5/T8) extend the switch — heights come from WIDGET_H, so a
 *  new widget kind never touches the layout walk itself. */
export type WidgetKind =
  | "hidden" | "row" | "scrub" | "toggle" | "chips" | "picker" | "textPreview" | "gradient";

/** Per-widget row height. Every visible widget is 16px today — pixel-identical
 *  to the old flat PARAM_H walk. Widget tracks adjust their own entry only. */
export const WIDGET_H: Record<WidgetKind, number> = {
  hidden: 0, row: PARAM_H, scrub: PARAM_H, toggle: PARAM_H, chips: PARAM_H,
  picker: PARAM_H, textPreview: PARAM_H, gradient: PARAM_H,
};

/** The schema→widget switch the study blesses for the PoC. "hidden" (the
 *  connected-input rule) always wins; then T1's W1 mapping: numbers scrub,
 *  booleans toggle, small enums chip. Everything else stays a "row" (popover). */
export function widgetFor(schema: ParamSchema, wired: boolean): WidgetKind {
  if (wired) return "hidden";
  switch (schema.kind) {
    case "number":  return "scrub";
    case "boolean": return "toggle";
    case "enum":    return schema.options.length <= 5 ? "chips" : "row";
    default:        return "row";
  }
}

// ── layout ───────────────────────────────────────────────────────────────────

/** One param row in the card: name, widget choice, height, offset from the
 *  node's top edge. Hidden rows stay in the list with h = 0 so indexes always
 *  line up with `Object.keys(info.params)`. */
export interface ParamRow { name: string; widget: WidgetKind; h: number; top: number }

/** Per-node state the layout depends on (current values + which params are
 *  driven by wires — a wired param row collapses via widgetFor).
 *  `w` (T15) is the per-instance width override (`GraphNode.w`); it wins over
 *  `NodeKindInfo.width`. OPTIONAL so pre-T15 callers keep compiling — but any
 *  caller that has the GraphNode should pass `w: node.w` or resized cards
 *  render at their kind default. */
export interface LayoutNode { params: Record<string, unknown>; wiredInputs: Set<string>; w?: number; bh?: number }

/** UI state the layout depends on. `zoom` is accepted and ignored until T10's
 *  semantic-zoom branch — in the signature now so it changes exactly once. */
export interface LayoutUi { helpOpen: boolean; zoom: number }

/** Params driven by wires: edges landing on a slot that names a param. Today
 *  ports and params never share names, so this is always empty — the seam for
 *  widget↔socket promotion (design doc §9). */
export const wiredInputsOf = (edges: GraphEdge[], id: string, info: NodeKindInfo): Set<string> => {
  const out = new Set<string>();
  for (const e of edges) if (e.to.node === id && info.params[e.to.slot]) out.add(e.to.slot);
  return out;
};

/** Vertical layout of one node card. All `*Top` values are offsets from the
 *  node's top edge; `h` is the full card height. */
export interface NodeLayout {
  w: number;
  h: number;
  /** True below ZOOM_CHIP_THRESHOLD: title + status chip only. Width is
   *  PRESERVED (`info.width ?? NODE_W`) so wire endpoints don't jump
   *  horizontally when crossing the threshold. All socket anchors collapse to
   *  the header's centre line (Blender collapsed-node style); param/body/help/
   *  footer heights are 0 so their hit-tests fall out for free. */
  chip: boolean;
  /** wrapped help text ([] when the expando is closed) */
  helpLines: HelpLine[];
  helpTop: number;
  helpH: number;
  socketsTop: number;
  paramsTop: number;
  /** Row table: one entry per param, in schema order (hidden rows h = 0). */
  paramRows: ParamRow[];
  /** Convenience view of paramRows (same order). */
  paramNames: string[];
  bodyTop: number;
  bodyH: number;
  footerTop: number;
}

export function nodeLayout(
  info: NodeKindInfo,
  node: LayoutNode,
  ui: LayoutUi,
  status?: NodeStatus,
): NodeLayout {
  // T15: per-instance width (node.w) wins over the kind width; both branches
  // use it, so chip mode preserves a resized card's width too.
  const w = node.w ?? info.width ?? NODE_W;
  // ── chip mode (T10): title + status chip only. Rows keep their names and
  // widget classification (indexes still line up with the schema) but h = 0,
  // so paramRowAt/paramRowRect can never hit them. helpOpen is ignored.
  if (ui.zoom < ZOOM_CHIP_THRESHOLD) {
    const paramRows: ParamRow[] = Object.entries(info.params).map(([name, schema]) => ({
      name, widget: widgetFor(schema, node.wiredInputs.has(name)), h: 0, top: HEADER_H,
    }));
    return {
      chip: true, w, h: HEADER_H + CHIP_H,
      helpLines: [], helpTop: HEADER_H, helpH: 0,
      socketsTop: HEADER_H, paramsTop: HEADER_H,
      paramRows, paramNames: paramRows.map((r) => r.name),
      bodyTop: HEADER_H, bodyH: 0, footerTop: HEADER_H,
    };
  }
  const lines = ui.helpOpen ? helpLines(info, status) : [];
  const helpH = lines.length ? lines.length * HELP_LINE_H + 2 * HELP_PAD : 0;
  const socketsTop = HEADER_H + helpH;
  const paramsTop = socketsTop + slotRows(info) * ROW_H;
  const paramRows: ParamRow[] = [];
  let top = paramsTop;
  for (const [name, schema] of Object.entries(info.params)) {
    const widget = widgetFor(schema, node.wiredInputs.has(name));
    const h = WIDGET_H[widget];
    paramRows.push({ name, widget, h, top });
    top += h;
  }
  const bodyTop = top;
  // Per-instance body height (node.bh) wins over the kind's bodyHeight; only
  // clamped when overridden, so un-overridden layouts stay byte-identical.
  const bodyH = node.bh !== undefined ? clampBodyH(node.bh) : info.bodyHeight ?? 0;
  const footerTop = bodyTop + bodyH;
  return {
    chip: false, w, h: footerTop + FOOTER_H,
    helpLines: lines, helpTop: HEADER_H, helpH,
    socketsTop, paramsTop, paramRows, paramNames: paramRows.map((r) => r.name),
    bodyTop, bodyH, footerTop,
  };
}

export const nodeSize = (info: NodeKindInfo, node: LayoutNode, ui: LayoutUi, status?: NodeStatus): Vec => {
  const l = nodeLayout(info, node, ui, status);
  return v(l.w, l.h);
};

/** Centre-line of socket row `i`, given the node's top edge. In chip mode
 *  every socket collapses to the header's centre line (row index ignored) —
 *  wires stay anchored and sockets stay hit-testable; stacked same-side
 *  sockets overlap, and `socketAt` resolves to the lowest index. */
export const socketY = (l: NodeLayout, top: number, i: number): number =>
  l.chip ? top + HEADER_H / 2 : top + l.socketsTop + ROW_H / 2 + i * ROW_H;

export const socketPos = (l: NodeLayout, r: Rect, dir: SocketDir, i: number): Vec =>
  v(dir === "in" ? r.x : r.right, socketY(l, r.y, i));

/** Clickable band of param row `i` (node-card rect in, world rect out).
 *  Hidden rows (h = 0) yield a zero-height band that can never be hit. */
export const paramRowRect = (l: NodeLayout, r: Rect, i: number): Rect => {
  const row = l.paramRows[i];
  return row.h > 0
    ? rect(r.x + 5, r.y + row.top + 1, r.w - 10, row.h - 2)
    : rect(r.x + 5, r.y + row.top, 0, 0);
};

/** The in-node body area (chart.bar draws here). */
export const bodyRect = (l: NodeLayout, r: Rect): Rect =>
  rect(r.x + 8, r.y + l.bodyTop + 2, r.w - 16, l.bodyH - 4);

// ── bar chart geometry (render + hover share it) ─────────────────────────────

export const CHART_LABEL_H = 11;

export interface ChartData { labels: unknown[]; values: number[]; title?: string }

/** One rect per bar, bottom-aligned above the label strip, scaled to max. */
export function barRects(l: NodeLayout, r: Rect, chart: ChartData): Rect[] {
  const area = bodyRect(l, r);
  const n = chart.values.length;
  if (!n || area.h <= CHART_LABEL_H + 4) return [];
  const titleH = chart.title ? 12 : 0;
  const plotH = area.h - CHART_LABEL_H - titleH;
  const max = Math.max(...chart.values.map((x) => (Number.isFinite(x) ? x : 0)), 0);
  const gap = 2;
  const bw = Math.max(1, (area.w - gap * (n - 1)) / n);
  const base = area.y + titleH + plotH;
  return chart.values.map((val, i) => {
    const t = max > 0 ? Math.max(0, val) / max : 0;
    const bh = Math.max(1, t * plotH);
    return rect(area.x + i * (bw + gap), base - bh, bw, bh);
  });
}

// ── hit tests (world point vs a node at n.x/n.y) ─────────────────────────────

const contains = (r: Rect, p: Vec) => p.x >= r.x && p.x <= r.right && p.y >= r.y && p.y <= r.bottom;

// ── T15 per-instance resize (supervisor-delegated edit, 2026-08-09) ──────────
/** Width clamp for a resize drag: [NODE_W, NODE_W_MAX] — a wide-by-default
 *  kind (width 240) may be shrunk down to the global minimum. */
export const NODE_W_MAX = 480;
export const clampNodeW = (w: number): number =>
  Math.min(NODE_W_MAX, Math.max(NODE_W, Math.round(w)));

/** Body-height clamp for a vertical resize (`GraphNode.bh`): applied only when
 *  the override is present, so an un-overridden layout never moves. */
export const BODY_H_MIN = 40;
export const BODY_H_MAX = 400;
export const clampBodyH = (h: number): number =>
  Math.min(BODY_H_MAX, Math.max(BODY_H_MIN, Math.round(h)));

/** Right-edge drag band width and bottom-right grip square, in card px. */
export const RESIZE_BAND = 6;
export const RESIZE_GRIP = 12;

/** The bottom-right grip square (render draws the glyph here; double-click on
 *  it resets the width to the kind default). */
export const resizeGripRect = (l: NodeLayout, nx: number, ny: number): Rect =>
  rect(nx + l.w - RESIZE_GRIP, ny + l.h - RESIZE_GRIP, RESIZE_GRIP, RESIZE_GRIP);

/** Resize hit zone under a world point: `"se"` = the corner grip (both axes),
 *  `"e"` = the right-edge band (width), `"s"` = the bottom-edge band (body
 *  height — present ONLY when the layout has a body; no body, no vertical
 *  resize), else null. Render and the resize gesture share it (no private
 *  geometry). Chip mode (T10 zoom) has no resize affordance at all. */
export function resizeHandleAt(l: NodeLayout, nx: number, ny: number, p: Vec): "e" | "s" | "se" | null {
  if (l.chip) return null;
  if (contains(resizeGripRect(l, nx, ny), p)) return "se";
  if (contains(rect(nx + l.w - RESIZE_BAND, ny, RESIZE_BAND, l.h), p)) return "e";
  if (l.bodyH > 0 && contains(rect(nx, ny + l.h - RESIZE_BAND, l.w, RESIZE_BAND), p)) return "s";
  return null;
}

export const inHeader = (l: NodeLayout, nx: number, ny: number, p: Vec): boolean =>
  contains(rect(nx, ny, l.w, HEADER_H), p);

/** Param row name under a world point, or null. */
export function paramRowAt(l: NodeLayout, nx: number, ny: number, p: Vec): string | null {
  const r = rect(nx, ny, l.w, l.h);
  for (let i = 0; i < l.paramRows.length; i++) {
    if (l.paramRows[i].h > 0 && contains(paramRowRect(l, r, i), p)) return l.paramRows[i].name;
  }
  return null;
}

/** Socket (with its port spec) within `grab` px of a world point, or null. */
export function socketAt(
  l: NodeLayout, info: NodeKindInfo, nx: number, ny: number, p: Vec, grab = 9,
): { dir: SocketDir; index: number; spec: NodeKindInfo["inputs"][number] } | null {
  const r = rect(nx, ny, l.w, l.h);
  const test = (dir: SocketDir, list: NodeKindInfo["inputs"]) => {
    for (let i = 0; i < list.length; i++) {
      const c = socketPos(l, r, dir, i);
      if (Math.hypot(c.x - p.x, c.y - p.y) <= grab) return { dir, index: i, spec: list[i] };
    }
    return null;
  };
  return test("in", info.inputs) ?? test("out", info.outputs);
}

/** Bar index under a world point (chart.bar hover), or null. Full column
 *  counts, not just the filled part — small values stay hoverable. */
export function barAt(l: NodeLayout, nx: number, ny: number, chart: ChartData, p: Vec): number | null {
  const r = rect(nx, ny, l.w, l.h);
  const area = bodyRect(l, r);
  if (!contains(area, p)) return null;
  const bars = barRects(l, r, chart);
  for (let i = 0; i < bars.length; i++) {
    const b = bars[i];
    if (p.x >= b.x && p.x <= b.right) return i;
  }
  return null;
}

// ── ids ──────────────────────────────────────────────────────────────────────

/** Anchor ids are `node|dir|slot`; node ids from the reducer are `n1`, `n2`, … */
export const anchorId = (node: string, dir: SocketDir, slot: string): string => `${node}|${dir}|${slot}`;

export const edgeKey = (e: GraphEdge): string =>
  `${e.from.node}|${e.from.slot}>${e.to.node}|${e.to.slot}`;

// ── colors ───────────────────────────────────────────────────────────────────

/** #rrggbb → Color. The contract states wire colors as CSS hex; the painter wants Color. */
export function fromHex(hex: string): Color {
  const n = parseInt(hex.replace("#", ""), 16);
  return rgb((n >> 16) & 255, (n >> 8) & 255, n & 255);
}

const WIRE_COLOR_CACHE: Record<WireType, Color> = {
  scene: fromHex(WIRE_COLORS.scene),
  table: fromHex(WIRE_COLORS.table),
  colormap: fromHex(WIRE_COLORS.colormap),
  view: fromHex(WIRE_COLORS.view),
};

export const wireColor = (t: WireType): Color => WIRE_COLOR_CACHE[t];

/** Header accent per node category — the editor's only categorical palette. */
// Rebranded 2026-08-19 (cream theme): desaturated warm-muted hues — category
// coding survives, but the canvas reads essentially monochrome from a distance.
export const CATEGORY_COLOR: Record<NodeKindInfo["category"], Color> = {
  source: rgb(96, 130, 150),
  select: rgb(118, 144, 114),
  group: rgb(120, 124, 158),                     // indigo — the partition hinge (wave 9)
  data: rgb(160, 136, 92),
  table: rgb(146, 116, 152),
  view: rgb(96, 142, 146),                       // cyan — the pass-through inspectors
  viz: rgb(160, 120, 138),
  sink: rgb(164, 116, 108),
};

/** CSS form of the same palette, for the HTML chrome (menu bar, palette, help). */
export const CATEGORY_CSS: Record<NodeKindInfo["category"], string> = {
  source: "#608296", select: "#769072", group: "#787C9E", data: "#A0885C", table: "#927498",
  view: "#608E92", viz: "#A0788A", sink: "#A4746C",
};

/** Group order for palettes and help — matches the order kinds.ts declares them. */
export const CATEGORY_ORDER: NodeKindInfo["category"][] =
  ["source", "select", "group", "data", "table", "view", "viz", "sink"];

/** Nodes that produce something visible get the eye toggle. */
export const canDisplay = (info: NodeKindInfo): boolean =>
  info.outputs.some((o) => o.type === "scene" || o.type === "view");
