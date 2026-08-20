// T5 (W1): the viz.colormap node body — the ramp drawn as itself (study §4:
// "the actual gradient, domain numerals at both ends, auto badge"; principle U1:
// the value is the interface).
//
// Fence: this file is T5's contention escape hatch (plan §3 / CONTRACTS.md) —
// widgets.ts belongs to T1 this wave. cards.ts hooks the body in with ONE line
// in NodeCard's `.adorn` (the supervisor applies it at the join):
//
//   if (info.kind === "viz.colormap") out.push(...colormapBody({ id, params: n.props.params }, l, r, n.props.status));
//
// The body MUST sample the same ramp function the evaluator colours with, or it
// lies (the legend rule — see panes/index.ts, which does the same read-only
// import). Source of truth for the ramp tables: web/src/flow/nodes.ts → RAMPS.
//
// Interactions (all inside the frozen geom bodyRect; no private geometry):
// - auto OFF → the gradient bar IS a range control (T18): drag the left/right
//   thumb to edit `min`/`max` (setParam per move — the `setParam:<id>:min` key
//   coalesces the whole drag into ONE undo step), drag the gradient between the
//   thumbs to shift the whole window (both params ride one `graphBatchKey`
//   intent keyed per-drag, so a window drag is also one undo step). The thumb
//   math is a local replica of gratify's pure range math
//   (gratify/examples/shared/range-math.ts) — examples aren't part of the
//   published dist, so the ~40 lines are copied, not imported.
// - auto ON  → the bar is display-only (a small `auto` tag rides its right
//   end); the drag gesture declines, so a drag on the bar falls through to
//   whatever the card does with body drags (node move).
// - click the ramp-name label (bottom-centre, where the old auto tag sat) →
//   cycle the `ramp` param (viridis → heat → greenred). Cycling moved OFF the
//   bar because the bar is now a slider; a text label is the standard "click to
//   cycle" affordance. The setParam rides inside a `batch` intent: doc.ts
//   coalesces consecutive `setParam:<id>:ramp` intents into ONE undo step, and
//   a batch never coalesces — so every click is its own undo entry.
// - click the min/max labels → dispatch the same node-select intent a param-row
//   click dispatches today. The DOM popover itself is opened by index.ts's
//   pointer listener (outside Gratify); routing body-label clicks into it is a
//   join-time index.ts concern — `cmapBodyLayout` exports the label rects it
//   would need. Until then min/max stay editable via their ordinary param rows.
//
// Slider domain heuristic (documented per T18): the drag maps the bar to
// [min - pad, max + pad] where pad = 0.5·(max - min), or max(1, |min|, |max|)
// when the span is degenerate — so both thumbs always have room to move in
// either direction. The domain is FROZEN at gesture begin (px↔value stays
// stable under the pointer) and recomputed from the new values on release —
// module-level `liveDrag` is the freeze seam (same module-state pattern as
// wires.ts's registerMoveExt).
import { at, calpha, part, rect, rgb, v, type Element, type Rect, type Vec } from "gratify";
import type { NodeStatus } from "../contracts";
import type { EditorIntent } from "./doc";
import { defaultOf } from "./doc";
import { bodyRect, type NodeLayout } from "./geom";
import { kindInfo } from "../kinds";
// Read-only import; Track C owns this function (CONTRACTS.md fence C).
import { ramp } from "../flow/nodes";

// W10 Track B: the same organ now renders on viz.colorBy from its EMBEDDED
// ramp/auto/min/max params (kinds.ts gave colorBy the identical param quartet +
// bodyHeight: 40). The node arg grew optional `kind` (schema-default source;
// defaults to viz.colormap) and `wired` (colorBy's OPTIONAL colormap INPUT is
// connected → the embedded settings are overridden by the upstream node). Wired
// state renders a neutral dim bar + a "wired ▸ colormap" tag and every gesture
// declines. LIMITATION (documented on purpose): the body cannot see the
// UPSTREAM node's params from here — parts render from their own node's props
// only — so the wired bar is a neutral well, not a preview of the wired ramp.
// cards.ts hookup (Track A owns the file; wired needs a new NodeCardProps field
// fed from surface.ts, where the edges are — see NOTES.md W10-B).

// ── ramp cycling + sampling (pure, spec'd in tests/colormap.spec.ts) ─────────

const cmapSchema = (kind = "viz.colormap") => kindInfo(kind)?.params ?? {};

/** Ramp order comes from the kinds.ts enum schema — one source of truth. */
export const RAMP_CYCLE: string[] = (() => {
  const s = cmapSchema().ramp;
  return s?.kind === "enum" ? [...s.options] : ["viridis", "heat", "greenred"];
})();

/** Next ramp in the cycle; unknown names restart at the first entry. */
export const nextRamp = (cur: string): string =>
  RAMP_CYCLE[(RAMP_CYCLE.indexOf(cur) + 1) % RAMP_CYCLE.length];

/** `n` evenly spaced samples of a named ramp, t = 0..1. Same table the
 *  evaluator and legend use (flow/nodes.ts ramp — unknown names → viridis). */
export const rampStops = (name: string, n = 24): [number, number, number][] =>
  Array.from({ length: Math.max(2, n) }, (_, i) => ramp(name, i / (Math.max(2, n) - 1)));

// ── domain labels ────────────────────────────────────────────────────────────

/** Domain endpoints read best short: integers bare, fractions to two places
 *  (mirrors flow/nodes.ts fmtNum, which is not exported). */
export const fmtDomain = (x: number): string =>
  Number.isInteger(x) ? String(x) : String(Number(x.toFixed(2)));

export interface CmapState {
  rampName: string;
  minText: string;
  maxText: string;
  auto: boolean;
  /** Effective numeric params (schema defaults filled in) — the slider edits
   *  THESE, never the resolved auto readout. */
  min: number;
  max: number;
}

/** What the body shows for a node's params + eval status. Auto mode prefers a
 *  resolved `lo–hi` readout from the node's status summary when one is present
 *  (summarize() prints `${min}–${max} ${ramp}`; a future resolved-domain
 *  summary parses the same way); otherwise both modes fall back to the params.
 *  Unset params read as their kinds.ts schema defaults, like the rest of the
 *  card. */
export function cmapState(
  params: Record<string, unknown>, status?: NodeStatus, kind = "viz.colormap",
): CmapState {
  const schema = cmapSchema(kind);
  const eff = (name: string): unknown => {
    const v0 = params[name];
    if (v0 !== undefined && v0 !== null && v0 !== "") return v0;
    return schema[name] ? defaultOf(schema[name]) : undefined;
  };
  const rampName = typeof eff("ramp") === "string" && eff("ramp") ? String(eff("ramp")) : "viridis";
  const auto = !!(eff("auto") ?? true);
  const min = typeof eff("min") === "number" ? (eff("min") as number) : 0;
  const max = typeof eff("max") === "number" ? (eff("max") as number) : 100;
  if (auto) {
    // en dash only — summarize() prints one, and "-" would be ambiguous with minus
    const m = status?.summary?.match(/(-?\d+(?:\.\d+)?)\s*–\s*(-?\d+(?:\.\d+)?)/);
    if (m) return { rampName, minText: fmtDomain(Number(m[1])), maxText: fmtDomain(Number(m[2])), auto, min, max };
  }
  return { rampName, minText: fmtDomain(min), maxText: fmtDomain(max), auto, min, max };
}

// ── slider thumb math (pure; replica of gratify examples/shared/range-math.ts) ─
// Copied rather than imported: gratify's examples/ directory is outside its
// published dist surface. Keep semantics in sync with the upstream module.

export interface SliderDomain { lo: number; hi: number }

/** The bar's drag domain: [min - pad, max + pad], pad = half the span (or a
 *  magnitude-based pad when min === max) — see the header heuristic note. */
export function cmapSliderDomain(min: number, max: number): SliderDomain {
  const span = max - min;
  const pad = span > 0 ? span * 0.5 : Math.max(1, Math.abs(min), Math.abs(max));
  return { lo: min - pad, hi: max + pad };
}

/** Values snap to a grid of ~1/100 of the domain span, at a power of ten —
 *  drags produce readable numbers (12.34, not 12.3391702). */
export function cmapSnapStep(d: SliderDomain): number {
  const span = Math.max(d.hi - d.lo, 1e-12);
  return Math.pow(10, Math.floor(Math.log10(span)) - 2);
}

const clamp01 = (f: number) => Math.min(1, Math.max(0, f));

/** Domain value → 0..1 bar fraction (0 when the domain is degenerate). */
export function sliderFrac(value: number, d: SliderDomain): number {
  const span = d.hi - d.lo;
  return span > 0 ? clamp01((value - d.lo) / span) : 0;
}

/** Screen x inside bar [x, x+w] → snapped, clamped domain value. */
export function sliderValue(px: number, x: number, w: number, d: SliderDomain): number {
  const raw = d.lo + clamp01(w > 0 ? (px - x) / w : 0) * (d.hi - d.lo);
  const step = cmapSnapStep(d);
  const snapped = Math.round(raw / step) * step;
  return Math.min(d.hi, Math.max(d.lo, Number(snapped.toFixed(12))));
}

export type SliderZone = "min" | "max" | "window";

/** Which zone a press at fraction `f` grabs (thumbs at fMin/fMax, grab radius
 *  in fraction units): outside → nearer thumb; between, near an edge → that
 *  thumb; middle → window (drag shifts both). Same rule as upstream zoneAt. */
export function sliderZone(f: number, fMin: number, fMax: number, grab: number): SliderZone {
  if (f <= fMin) return "min";
  if (f >= fMax) return "max";
  const dMin = f - fMin, dMax = fMax - f;
  if (dMin <= grab && dMin <= dMax) return "min";
  if (dMax <= grab) return "max";
  return "window";
}

/** New min from a drag — can't cross max. */
export const sliderDragMin = (value: number, max: number): { min: number; max: number } =>
  ({ min: Math.min(value, max), max });

/** New max from a drag — can't cross min. */
export const sliderDragMax = (min: number, value: number): { min: number; max: number } =>
  ({ min, max: Math.max(value, min) });

/** Shift the whole window by `delta` domain units: width preserved exactly,
 *  clamped inside the domain, min snapped to the domain's step grid. */
export function sliderShiftWindow(
  min: number, max: number, delta: number, d: SliderDomain,
): { min: number; max: number } {
  const width = Math.min(max - min, d.hi - d.lo);
  const step = cmapSnapStep(d);
  const clamped = Math.min(d.hi - width, Math.max(d.lo, min + delta));
  const nmin = Math.max(d.lo,
    Math.min(d.hi - width, Number((Math.round(clamped / step) * step).toFixed(12))));
  return { min: nmin, max: Number((nmin + width).toFixed(12)) };
}

// ── body geometry (pure; render and hit-tests share it, per the widget rule) ─

export interface CmapBodyLayout { bar: Rect; min: Rect; max: Rect; tag: Vec }

/** Split the frozen geom bodyRect into: gradient bar on top, min label bottom-
 *  left, max label bottom-right, auto tag centred between them. */
export function cmapBodyLayout(area: Rect): CmapBodyLayout {
  const barH = Math.min(16, Math.max(8, area.h - 16));
  const bar = rect(area.x + 2, area.y + 2, area.w - 4, barH);
  const ly = bar.bottom + 2;
  const lh = Math.max(10, area.bottom - ly);
  const lw = Math.min(56, Math.floor(area.w / 3));
  return {
    bar,
    min: rect(area.x + 2, ly, lw, lh),
    max: rect(area.right - 2 - lw, ly, lw, lh),
    tag: v(area.center.x, ly + lh / 2),
  };
}

// ── parts ────────────────────────────────────────────────────────────────────

/** Opaque backing panel: covers the card's default "no data" body text. Not
 *  interactive — clicks fall through to the card (select), like any other body
 *  background. (The `auto` tag moved onto the bar; the ramp label took the
 *  centre slot.) */
const CmapBacking = part("pf-cmap-bg")
  .props<{ w: number; h: number }>()
  .size((p) => v(p.w, p.h))
  .style((t) => ({ fill: t.mix(t.surface, rgb(0, 0, 0), 0.06) }))
  .render((n, p, s) => p.box(n.rect, 5, s.fill));

/** Fraction units treated as a thumb grab on the bar (≈9px on a 168px bar). */
const THUMB_GRAB_FRAC = 0.055;
/** Thumb handle width in px. */
const THUMB_W = 5;

/** The one drag in flight, if any — freezes the slider domain from gesture
 *  begin to release so px↔value stays stable under the pointer (header note).
 *  Module state on purpose (the registerMoveExt pattern): render + gesture
 *  share it, specs never see it (they use the pure math). */
let liveDrag: { node: string; dom: SliderDomain } | null = null;

interface BarDrag {
  zone: SliderZone;
  dom: SliderDomain;
  startMin: number;
  startMax: number;
  startX: number;
  seq: number;
}

let dragSeq = 0;

/** The gradient bar. Auto ON: display-only ramp preview spanning the bar, with
 *  a small `auto` tag (drags decline → fall through to the card). Auto OFF:
 *  a dual-thumb range control — the gradient spans min..max inside the padded
 *  slider domain, thumbs edit min/max, the span between them window-shifts. */
const CmapBar = part("pf-cmap-bar")
  .props<{ node: string; rampName: string; w: number; h: number; min: number; max: number;
    auto: boolean; wired: boolean }>()
  .size((p) => v(p.w, p.h))
  .style((t, ch) => ({
    edge: t.mix(t.muted, t.accent, ch.hover),
    edgeW: 1 + ch.hover,
    well: t.mix(t.surface, rgb(0, 0, 0), 0.12),
    thumb: t.mix(t.textBright, t.accent, 0.4 * ch.hover),
    dim: t.textDim,
  }))
  .render((n, p, s) => {
    const r = n.rect;
    const { rampName, auto, min, max, node, wired } = n.props;
    const stops = rampStops(rampName, 24);
    const paintRamp = (x: number, w: number) => {
      const sw = w / stops.length;
      stops.forEach(([cr, cg, cb], i) => {
        p.box(rect(x + i * sw, r.y, sw + 0.6, r.h), 0,
          rgb(Math.round(cr * 255), Math.round(cg * 255), Math.round(cb * 255)));
      });
    };
    if (wired) {
      // Overridden by the wired colormap input: neutral dim well only — the
      // upstream ramp is NOT visible from this node's props (header note).
      p.box(r, 3, calpha(s.well, 0.6));
      p.box(r, 3, calpha(rgb(0, 0, 0), 0), calpha(s.edge, 0.5), 1);
      return;
    }
    if (auto) {
      paintRamp(r.x, r.w);
      // T17: darker backing — the tag sits on viridis's bright yellow end and
      // washed out at 0.55 alpha.
      p.box(rect(r.right - 26, r.y + 2, 24, r.h - 4), 3, calpha(rgb(0, 0, 0), 0.72));
      p.label("auto", v(r.right - 14, r.center.y), calpha(rgb(255, 255, 255), 0.92), { size: 8 });
    } else {
      // range mode: dim well across the domain, gradient between the thumbs
      const dom = liveDrag?.node === node ? liveDrag.dom : cmapSliderDomain(min, max);
      const xMin = r.x + sliderFrac(min, dom) * r.w;
      const xMax = r.x + sliderFrac(max, dom) * r.w;
      p.box(r, 3, s.well);
      paintRamp(xMin, Math.max(0, xMax - xMin));
      for (const tx of [xMin, xMax])
        p.box(rect(tx - THUMB_W / 2, r.y - 2, THUMB_W, r.h + 4), 1.5, s.thumb);
    }
    p.box(r, 3, calpha(rgb(0, 0, 0), 0), s.edge, s.edgeW);
  })
  .gesture<BarDrag>({
    begin(n, p) {
      // display-only (auto) or overridden (wired) → decline, card handles it
      if (n.props.auto || n.props.wired) return null;
      const { min, max, node } = n.props;
      const dom = cmapSliderDomain(min, max);
      const r = n.rect;
      const f = r.w > 0 ? (p.x - r.x) / r.w : 0;
      const zone = sliderZone(f, sliderFrac(min, dom), sliderFrac(max, dom), THUMB_GRAB_FRAC);
      liveDrag = { node, dom };
      return { zone, dom, startMin: min, startMax: max, startX: p.x, seq: dragSeq++ };
    },
    during(st, n, p): EditorIntent | void {
      const r = n.rect;
      const { node, min, max } = n.props;
      if (st.zone === "min") {
        const next = sliderDragMin(sliderValue(p.x, r.x, r.w, st.dom), max);
        if (next.min === min) return;
        // coalesces on setParam:<node>:min — the whole drag is ONE undo entry
        return { k: "graph", intent: { t: "setParam", node, name: "min", value: next.min } };
      }
      if (st.zone === "max") {
        const next = sliderDragMax(min, sliderValue(p.x, r.x, r.w, st.dom));
        if (next.max === max) return;
        return { k: "graph", intent: { t: "setParam", node, name: "max", value: next.max } };
      }
      // window: both params in one batch, coalesced per-drag by the seq key
      const delta = ((p.x - st.startX) / Math.max(r.w, 1)) * (st.dom.hi - st.dom.lo);
      const next = sliderShiftWindow(st.startMin, st.startMax, delta, st.dom);
      if (next.min === min && next.max === max) return;
      return {
        k: "graphBatchKey",
        key: `cmapWin:${node}:${st.seq}`,
        intent: { t: "batch", intents: [
          { t: "setParam", node, name: "min", value: next.min },
          { t: "setParam", node, name: "max", value: next.max },
        ] },
      };
    },
    up(): void {
      liveDrag = null;                             // domain re-centres on release
    },
  });

/** Wired-override tag (colorBy only): sits where the min/ramp/max labels would,
 *  says the embedded settings are inert. Not interactive — like the backing,
 *  clicks fall through to the card (select). */
const CmapWired = part("pf-cmap-wired")
  .props<{ w: number; h: number }>()
  .size((p) => v(p.w, p.h))
  .style((t) => ({ text: calpha(t.textDim, 0.75) }))
  .render((n, p, s) => p.label("wired ▸ colormap", n.rect.center, s.text, { size: 8 }));

/** The ramp-name label (bottom-centre) — click cycles the ramp param. */
const CmapRamp = part("pf-cmap-ramp")
  .props<{ node: string; rampName: string; w: number; h: number }>()
  .size((p) => v(p.w, p.h))
  .style((t, ch) => ({
    fill: calpha(rgb(255, 255, 255), 0.05 * ch.hover),
    text: t.mix(t.textDim, t.textBright, ch.hover),
  }))
  .render((n, p, s) => {
    p.box(n.rect, 3, s.fill);
    p.label(n.props.rampName, n.rect.center, s.text, { size: 8 });
  })
  // batch on purpose: defeats setParam coalescing so each click = one undo entry
  .press((n): EditorIntent => ({
    k: "graph",
    intent: {
      t: "batch",
      intents: [{ t: "setParam", node: n.props.node, name: "ramp", value: nextRamp(n.props.rampName) }],
    },
  }));

/** A domain endpoint label — click enters the param-edit pathway (selects the
 *  node, same intent a param-row click dispatches; index.ts's DOM listener owns
 *  actually opening the popover — see the header note). */
const CmapLabel = part("pf-cmap-label")
  .props<{ node: string; param: "min" | "max"; text: string; align: "left" | "right"; w: number; h: number }>()
  .size((p) => v(p.w, p.h))
  .style((t, ch) => ({
    fill: calpha(rgb(255, 255, 255), 0.05 * ch.hover),
    text: t.mix(t.textDim, t.textBright, ch.hover),
  }))
  .render((n, p, s) => {
    const r = n.rect;
    p.box(r, 3, s.fill);
    const x = n.props.align === "left" ? r.x + 2 : r.right - 2;
    p.label(n.props.text, v(x, r.center.y), s.text, { align: n.props.align, size: 9 });
  })
  .press((n): EditorIntent => ({ k: "select", sel: { kind: "node", id: n.props.node } }));

// ── assembly (the one-line cards.ts hookup calls this) ───────────────────────

/** The gradient body (viz.colormap AND viz.colorBy): backing + gradient bar +
 *  min/max labels, positioned inside the frozen geom bodyRect of `cardRect`.
 *  Returns [] when the kind reserves no body height. Narrow seam by design:
 *  node identity + params (+ `kind` for schema defaults, default viz.colormap,
 *  and `wired` = colorBy's colormap input is connected → dim override state),
 *  the shared layout, the card's world rect, and the eval status. */
export function colormapBody(
  node: { id: string; params: Record<string, unknown>; kind?: string; wired?: boolean },
  l: NodeLayout,
  cardRect: Rect,
  status?: NodeStatus,
): Element[] {
  if (l.bodyH <= 0) return [];
  const area = bodyRect(l, cardRect);
  const st = cmapState(node.params, status, node.kind ?? "viz.colormap");
  const g = cmapBodyLayout(area);
  if (node.wired) {
    // Embedded settings overridden by the wired colormap input: dim bar (no
    // ramp preview — see header limitation), one tag, no editing affordances.
    const stripW = g.max.right - g.min.x;
    return [
      at(CmapBacking("cmap-bg", { w: area.w, h: area.h }), v(area.x, area.y)),
      at(CmapBar("cmap-bar", { node: node.id, rampName: st.rampName, w: g.bar.w, h: g.bar.h,
        min: st.min, max: st.max, auto: st.auto, wired: true }), v(g.bar.x, g.bar.y)),
      at(CmapWired("cmap-wired", { w: stripW, h: g.min.h }), v(g.min.x, g.min.y)),
    ];
  }
  const rampW = Math.max(0, g.max.x - g.min.right - 4);
  return [
    at(CmapBacking("cmap-bg", { w: area.w, h: area.h }), v(area.x, area.y)),
    at(CmapBar("cmap-bar", { node: node.id, rampName: st.rampName, w: g.bar.w, h: g.bar.h,
      min: st.min, max: st.max, auto: st.auto, wired: false }), v(g.bar.x, g.bar.y)),
    at(CmapLabel("cmap-min", { node: node.id, param: "min", text: st.minText, align: "left", w: g.min.w, h: g.min.h }), v(g.min.x, g.min.y)),
    at(CmapLabel("cmap-max", { node: node.id, param: "max", text: st.maxText, align: "right", w: g.max.w, h: g.max.h }), v(g.max.x, g.max.y)),
    ...(rampW > 12
      ? [at(CmapRamp("cmap-ramp", { node: node.id, rampName: st.rampName, w: rampW, h: g.min.h }),
          v(g.min.right + 2, g.min.y))]
      : []),
  ];
}
