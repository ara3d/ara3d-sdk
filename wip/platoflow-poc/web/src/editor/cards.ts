// The node card and its adornment chips (help / run / delete), plus the
// param-value display helper. Split from parts.ts in W0 (plan §2.5). Fence:
// T1/T5/T9/T10 own the param-row render, gradient region, bodies, chip-mode.
//
// The node card IS the inspector: it draws its params (click to edit — the
// popover is DOM, see params.ts), an expandable help area under the title, an
// optional chart body, and a footer with status + controls. All geometry comes
// from nodeLayout() in geom.ts, shared with the external hit-testing.
import {
  at, calpha, cmix, Focusable, part, rect, rgb, tier, v,
  type Color, type Element, type Vec,
} from "gratify";
import type { NodeKindInfo, NodeStatus, WireType } from "../contracts";
import type { EditorIntent } from "./doc";
import { defaultOf } from "./doc";
import {
  anchorId, barRects, bodyRect, CATEGORY_COLOR, CHIP_H, FOOTER_H, HEADER_H,
  HELP_LINE_H, HELP_PAD, nodeLayout, paramRowRect, resizeGripRect, socketPos, wireColor,
  type ChartData, type NodeLayout, type SocketMeta,
} from "./geom";
import {
  drawMiniGrid, gridClickGesture, paramDisplay, resizeGesture, widgetGesture, widgetImpl,
  type GridClickDrag, type ResizeDrag, type WidgetDrag,
} from "./widgets";
// Re-export: paramDisplay lived here until T15 moved it into widgets.ts (the
// registry's "row" entry needs it without a cards→widgets cycle).
export { paramDisplay } from "./widgets";
import { colormapBody } from "./colormap";
import { checklistBody, checklistGesture, type ChecklistDrag } from "./checklist";
import { nodeMoveGesture, wireDragGesture, type WireDrag } from "./wires";

const SOCKET_R = 5;

// ── status ───────────────────────────────────────────────────────────────────

// Rebranded 2026-08-19 (cream theme): darker, desaturated so they carry on white cards.
export const STATUS_COLOR = {
  ok: rgb(63, 157, 95),
  error: rgb(192, 57, 43),
  pending: rgb(185, 138, 46),
  "needs-setup": rgb(152, 150, 142),   // neutral gray: unconfigured, not broken (wave 9)
};

/** Warning amber (W9-C) — the pending hue, reused so the palette stays small. */
export const WARN_AMBER = rgb(185, 138, 46);

/** Footer text (W9-C): error AND needs-setup show their message ("choose a
 *  type"), everything else shows the summary — "⚠ "-prefixed when the status
 *  carries a non-fatal warning (design §3: dropped rows get counted). */
export function footerText(status?: NodeStatus): string {
  if (!status) return "";
  if (status.state === "error") return status.message ?? "error";
  if (status.state === "needs-setup") return status.message ?? "needs setup";
  return (status.warning ? "⚠ " : "") + (status.summary ?? status.state);
}

/** Footer accent tone (W9-C): the state color, blended toward amber when a
 *  non-error status carries a warning. Errors keep their red — a warning
 *  never escalates or softens a failure. */
export function footerTone(status: NodeStatus | undefined, none: Color): Color {
  if (!status) return none;
  const base = STATUS_COLOR[status.state];
  return status.warning && status.state !== "error" ? cmix(base, WARN_AMBER, 0.65) : base;
}

const truncate = (s: string, max: number) => (s.length > max ? s.slice(0, max - 1) + "…" : s);

// ── calm pass (W13-B): chips only on engagement ──────────────────────────────
// The complaint was "a lot going on": every resting card carried ?/✕/Run
// chips. They now fade in with the card's engagement channels and fade back
// out at rest — a resting card is title, category strip, ports, param rows
// and footer, nothing else.

/** Below this the chip is not emitted at all (also: no hit target). */
export const CHIP_FADE_MIN = 0.02;
/** A mostly-faded chip must not take the click — its press is a noop until
 *  the fade-in has crossed this level. */
export const CHIP_PRESS_MIN = 0.5;

/** Chip alpha = the card's strongest engagement channel. `over` is the card's
 *  own pointer-in-rect channel, NOT ch.hover: interactive adorn chips capture
 *  hover from their host (no hover chain), so gating on ch.hover would make
 *  a chip evict itself the moment the pointer reached it — a flicker loop.
 *  Focus is included so keyboard (Tab-ring) users see the same affordances. */
export const chipAlpha = (ch: Record<string, number>): number =>
  Math.min(1, Math.max(ch.over || 0, ch.sel || 0, ch.drag || 0, ch.focus || 0));

// ── node card ────────────────────────────────────────────────────────────────

export interface NodeCardProps {
  id: string;
  info: NodeKindInfo;
  pos: Vec;
  params: Record<string, unknown>;
  /** Params driven by wires (geom.wiredInputsOf) — collapses their rows. */
  wiredInputs: Set<string>;
  /** W13-B: UNUSED — the eye chip and display glow are gone (selection drives
   *  the 3D view; GraphDoc.display is the MCP-set pinned fallback, wired in
   *  main.ts). Kept optional so older view builders/specs still compile. */
  display?: boolean;
  helpOpen: boolean;
  status?: NodeStatus;
  /** Per-instance width override (GraphNode.w, T15) — wins over
   *  info.width. OPTIONAL so existing view builders keep compiling, but
   *  makeView (surface.ts) must pass `w: n.w` or resized cards render at the
   *  kind default. */
  w?: number;
  /** Per-instance body-height override (GraphNode.bh) — wins over
   *  info.bodyHeight; same makeView wiring rule as `w`. */
  bh?: number;
  /** viz.colorBy only: its `colormap` PORT is wired, so the embedded gradient
   *  body renders dimmed/overridden. Edge-derived in makeView — the layout's
   *  wiredInputs seam is param-only and never sees ports (W10-B finding). */
  cmapWired?: boolean;
  /** Adornments share one overlay layer, so they'd sit on top of the palette. */
  hideChips?: boolean;
  states?: Record<string, boolean>;
}

/** The card's layout under the W0 signature. Callbacks without a view (e.g.
 *  `.size`) pass zoom = 1, so the node's logical rect stays full-size; only
 *  render/anchors/adorn (which see the view) go chip-mode below the
 *  threshold. */
const cardLayout = (p: NodeCardProps, zoom = 1): NodeLayout =>
  nodeLayout(p.info, { params: p.params, wiredInputs: p.wiredInputs, w: p.w, bh: p.bh },
    { helpOpen: p.helpOpen, zoom }, p.status);

const HELP_TONE = {
  doc: rgb(110, 108, 100), detail: rgb(70, 110, 140),
  warn: WARN_AMBER, error: rgb(192, 57, 43),
};

export const NodeCard = part("pf-node")
  .props<NodeCardProps>()
  .size((p) => {
    const l = cardLayout(p);
    return v(l.w, l.h);
  })

  // Sockets publish their world position every layout pass; wires, the magnetic
  // snap and the connect gesture all read them back through the query.
  .anchors((n) => {
    const { id, info } = n.props;
    const l = cardLayout(n.props, n.view?.zoom);
    const one = (dir: "in" | "out", slot: { name: string; type: WireType }, i: number) => ({
      id: anchorId(id, dir, slot.name),
      pos: socketPos(l, n.rect, dir, i),
      meta: { node: id, dir, slot: slot.name, type: slot.type } as SocketMeta,
    });
    return [
      ...info.inputs.map((s, i) => one("in", s, i)),
      ...info.outputs.map((s, i) => one("out", s, i)),
    ];
  })

  // W13-B: `over` = pointer geometrically inside the card. Replaces ch.hover
  // for chip gating (see chipAlpha) because a hovered CHIP steals ch.hover
  // from the card; the pointer is still over the card either way.
  .channels({ over: {
    target: (n) => (n.pointer && n.rect.contains(n.pointer) ? 1 : 0), rate: 14,
  } })

  .style((t, ch, p) => ({
    fill: t.mix(t.surface, t.surfaceHi, 0.25 * ch.hover + 0.5 * ch.drag),
    edge: t.mix(t.muted, t.accent, Math.max(0.5 * ch.hover, ch.sel || 0, 0.8 * (ch.focus || 0))),
    edgeW: 1.2 + 1.2 * Math.max(ch.sel || 0, 0.6 * (ch.focus || 0)),
    text: t.mix(t.text, t.textBright, ch.hover),
    dim: t.textDim,
    bright: t.textBright,
    accent: CATEGORY_COLOR[p.info.category],
    lift: 3 * ch.drag,
  }))

  .render((n, p, s) => {
    const r = n.rect.raise(s.lift);
    const { info, status, params } = n.props;
    const l = cardLayout(n.props, n.view?.zoom);

    // ── chip mode (T10 semantic zoom): below ZOOM_CHIP_THRESHOLD the card is
    // title + status accent only — no rows, chips, help, or bodies. Sockets
    // draw as dots at their collapsed header-line anchors so wires stay
    // visually attached. Note n.rect keeps the zoom-1 size (`.size` has no
    // view), so we draw the chip-sized card from the layout, not the rect.
    if (l.chip) {
      const cr = rect(r.x, r.y, l.w, l.h);
      p.box(cr, 8, s.fill, s.edge, s.edgeW);
      // Inset accent bar (T17): kept clear of the 8px corners — a full-width
      // radius-3 strip used to poke past the card outline at the corners.
      p.box(rect(r.x + 4, r.y + 2.5, l.w - 8, 3), 1.5, s.accent);
      p.label(info.label, v(r.x + 10, r.y + HEADER_H / 2 + 3), s.text,
        { align: "left", weight: 600, size: 13 });
      if (status) {
        // footerTone, not the raw state color: a warning tints the chip amber
        // and needs-setup reads neutral gray at any zoom (W9-C).
        p.box(rect(r.x + 6, r.y + HEADER_H + 1, l.w - 12, CHIP_H - 4), 2,
          calpha(footerTone(status, s.dim), 0.75));
      }
      info.inputs.forEach((slot, i) =>
        p.dot(socketPos(l, cr, "in", i), SOCKET_R, wireColor(slot.type)));
      info.outputs.forEach((slot, i) =>
        p.dot(socketPos(l, cr, "out", i), SOCKET_R, wireColor(slot.type)));
      return;
    }

    // W13-B: the display glow ring is gone with the eye chip — selection is
    // the one highlight channel a card carries now.
    p.box(r, 10, s.fill, s.edge, s.edgeW);
    // Category accent (T17): an INSET rounded bar. The old full-width
    // `rect(r.x, r.y, r.w, 5)` at radius 3 poked ~3px past the card's
    // radius-10 corner curve on both sides and covered the top border stroke —
    // the "messy top edge" at zoom. Inset keeps the outline crisp at any zoom.
    p.box(rect(r.x + 5, r.y + 3, r.w - 10, 4), 2, s.accent);
    p.label(info.label, v(r.x + 10, r.y + HEADER_H / 2 + 3), s.text,
      { align: "left", weight: 600, size: 13 });
    // T17: graph.sub enter affordance (T16 wish) — a dim hint after the title.
    // Double-click on the header is the gesture; this just advertises it.
    if (info.kind === "graph.sub") {
      const tw = p.measure.text(info.label, 13).x;
      p.label("dbl-click to enter", v(r.x + 10 + tw + 8, r.y + HEADER_H / 2 + 4),
        calpha(s.dim, 0.75), { align: "left", size: 9 });
    }
    // The header status dot is GONE (T17): it sat directly under the relocated
    // ✕ chip and read as a mystery blob on the delete button. Status lives in
    // ONE place now — the footer accent line + text (pulse moved there too).

    // ── help expando (canvas text, wrapped by geom so height always matches)
    if (l.helpLines.length) {
      const hy = r.y + l.helpTop;
      p.box(rect(r.x + 4, hy + 1, r.w - 8, l.helpH - 2), 5, calpha(rgb(0, 0, 0), 0.05));
      l.helpLines.forEach((line, i) => {
        if (!line.text) return;
        p.label(line.text, v(r.x + 10, hy + HELP_PAD + HELP_LINE_H / 2 + i * HELP_LINE_H),
          HELP_TONE[line.tone], { align: "left", size: 10 });
      });
    }

    // ── sockets
    info.inputs.forEach((slot, i) => {
      const c = socketPos(l, r, "in", i);
      p.dot(c, SOCKET_R, wireColor(slot.type));
      p.label(slot.name, v(c.x + 10, c.y), s.dim, { align: "left", size: 11 });
    });
    info.outputs.forEach((slot, i) => {
      const c = socketPos(l, r, "out", i);
      p.dot(c, SOCKET_R, wireColor(slot.type));
      p.label(slot.name, v(c.x - 10, c.y), s.dim, { align: "right", size: 11 });
    });

    // ── param rows — per-type widgets via the T15 registry: the layout row's
    // WidgetKind (geom's frozen widgetFor) picks the entry; unknown kinds fall
    // back to the plain row. The widget gesture routes through the same
    // registry, so a drawn widget and its gesture can never disagree.
    l.paramRows.forEach((prow, i) => {
      if (prow.h <= 0) return;                    // wired param — row collapsed
      const row = paramRowRect(l, r, i);
      p.box(row, 4, calpha(rgb(0, 0, 0), 0.03));
      const schema = info.params[prow.name];
      const raw = params[prow.name];
      const value = raw === undefined || raw === null || raw === "" ? defaultOf(schema) : raw;
      widgetImpl(prow.widget).draw(p, row, {
        name: prow.name, raw, value, schema,
        tones: { dim: s.dim, bright: s.bright, accent: s.accent },
      });
    });

    // ── body: chart bars (chart.bar) or a read-only mini-grid (T9 — any kind
    // whose status carries tablePreview AND whose kind reserves bodyHeight;
    // without bodyHeight the layout has bodyH = 0 and nothing here runs).
    if (l.bodyH > 0) {
      const area = bodyRect(l, r);
      const chart = status?.chart as ChartData | undefined;
      const preview = status?.tablePreview;
      if (chart && chart.values.length) {
        if (chart.title) {
          p.label(truncate(chart.title, 30), v(area.x, area.y + 5), s.dim, { align: "left", size: 9 });
        }
        const bars = barRects(l, r, chart);
        bars.forEach((b, i) => {
          p.box(b, 1, calpha(s.accent, 0.85));
          if (b.w >= 18) {
            p.label(truncate(String(chart.labels[i] ?? ""), Math.floor(b.w / 5)),
              v(b.center.x, area.bottom - 4), s.dim, { align: "center", size: 8 });
          }
        });
      } else if (preview && preview.columns.length) {
        // Wave 10: the pointer (only while the card is hovered) tints the row
        // under it — the grid-click affordance; widgets.ts hit-tests it.
        drawMiniGrid(p, area, preview, { dim: s.dim, bright: s.bright, accent: s.accent },
          n.ch.hover > 0.01 ? n.pointer : undefined);
      } else {
        p.label("no data", area.center, calpha(s.dim, 0.7), { size: 10 });
      }
    }

    // ── footer: status accent bar + `state · summary`; chips are adornments
    const fy = r.y + l.footerTop;
    // W9-C: tone + text via the shared helpers — needs-setup shows its message
    // in neutral gray (never red), a warning amber-blends the accent and
    // "⚠ "-prefixes the summary.
    const tone = footerTone(status, s.dim);
    // Pending pulse rides the footer accent line (was the deleted header dot).
    const pulse = status?.state === "pending" ? 0.45 + 0.45 * Math.sin((n.time ?? 0) * 6) : 1;
    // W13-B calm: the ok accent line whispers at rest; error/warning keep
    // their full strength (failure is feedback, not decoration) and the
    // pending pulse stays for the same reason.
    const lineA = !status ? 0.12
      : status.state === "error" || status.state === "pending" || status.warning ? 0.55
      : 0.32;
    p.line(v(r.x + 6, fy), v(r.right - 6, fy), calpha(tone, lineA * pulse), 1);
    const runW = info.effect === "write" ? 52 : 0;
    const chips = 36;                                    // resize grip on the right
    const text = footerText(status);
    if (text) {
      const maxChars = Math.floor((r.w - 16 - runW - chips) / 5.2);
      // W13-B calm: counts are data but they don't need to shout — the ok
      // summary dims at rest and brightens with the card's hover; error text
      // keeps its red, a warning keeps its amber tone at full strength.
      const textTone =
        status!.state === "error" ? STATUS_COLOR.error
        : status!.warning ? tone
        : calpha(s.dim, 0.65 + 0.35 * (n.ch.over || 0));
      p.label(truncate(text, Math.max(4, maxChars)),
        v(r.x + 10 + runW, fy + FOOTER_H / 2 + 1),
        textTone, { align: "left", size: 10 });
    }

    // ── resize grip (T15): two diagonal strokes in the bottom-right corner of
    // the card, inside resizeGripRect — render and gesture share the zone.
    // Drag it (or the right-edge band) to set a per-instance width;
    // double-click resets to the kind default.
    {
      const g = resizeGripRect(l, r.x, r.y);
      // T17: brighten with card hover so the affordance is discoverable
      // without shouting at rest.
      const tone = calpha(s.dim, 0.4 + 0.45 * n.ch.hover);
      p.line(v(g.right - 3, g.y + 4), v(g.x + 4, g.bottom - 3), tone, 1.2);
      p.line(v(g.right - 3, g.y + 8), v(g.x + 8, g.bottom - 3), tone, 1.2);
      // Bottom-band affordance (body-carrying kinds only — the "s" handle):
      // two short centered horizontal strokes on the bottom edge, same T17
      // hover-brightened voice as the grip diagonals.
      if (l.bodyH > 0) {
        const cx = r.x + l.w / 2, by = r.y + l.h;
        p.line(v(cx - 7, by - 5), v(cx + 7, by - 5), tone, 1.2);
        p.line(v(cx - 4, by - 2.5), v(cx + 4, by - 2.5), tone, 1.2);
      }
    }
  })

  // Help = expando toggle (header), ✕ = delete (top-right, standard corner),
  // Run = effectful sink (footer). Adornments so they sit above the card and
  // take their own clicks. The eye chip is GONE (W13-B): selection drives the
  // 3D view now, and no card UI produces setDisplay any more (MCP still may).
  .adorn((n) => {
    let out: Element[] = [];
    if (n.props.hideChips) return out;
    const { id, info, helpOpen } = n.props;
    // Mirror the render pass's drag lift (style `lift: 3 * ch.drag`) so the
    // chips and bodies ride the raised card instead of staying behind.
    const r = n.rect.raise(3 * n.ch.drag);
    const l = cardLayout(n.props, n.view?.zoom);
    if (l.chip) return out;                        // T10: no adornments below the zoom threshold
    const fy = r.y + l.footerTop;
    if (info.kind === "viz.colormap" || info.kind === "viz.colorBy")
      out.push(...colormapBody({ id, params: n.props.params, kind: info.kind, wired: n.props.cmapWired }, l, r, n.props.status));
    if (info.kind === "select.checklist")
      out.push(...checklistBody({ id, params: n.props.params }, l, r, n.props.status));
    // W13-B calm: chips fade with the card's engagement (adorn runs every
    // frame, so the channel drives a real fade, not a toggle). Below the
    // floor the chip is not emitted — no element, no hit target; during the
    // fade window each chip's press guards on CHIP_PRESS_MIN itself.
    const a = chipAlpha(n.ch);
    if (a > CHIP_FADE_MIN) {
      out.push(at(DelChip("del", { node: id, a }), v(r.right - 24, r.y + 5)));
      out.push(at(HelpChip("help", { node: id, on: helpOpen, a }), v(r.right - 42, r.y + 5)));
      if (info.effect === "write") {
        out.push(at(RunChip("run", { node: id, a }), v(r.x + 8, fy + 2)));
      }
    }
    // The adorn layer paints above every card, so a neighbor's chips would sit
    // on top of a card dragged across them — the interacted card's adornments
    // ride a higher z-tier to stay on top.
    // `over`, not hover: a hovered chip must keep its host's adorns on top.
    const zt = n.ch.drag > 0.05 ? 2 : (n.ch.over || 0) > 0.05 ? 1 : 0;
    if (zt) out = out.map((el) => tier(el, zt));
    return out;
  })

  // GESTURE 1 — drag from a socket to wire it up (wires.ts owns the spec).
  // Declines unless the press landed on a socket, so the later gestures get
  // the rest of the card. Unsnapped drop on empty canvas = link-drag-search.
  .gesture<WireDrag>(wireDragGesture)

  // GESTURE 2 — resize (T15): right-edge band + bottom-right grip. Before the
  // widget gesture so the extreme edge wins the ~1px overlap with a param
  // row's right end; declines everywhere else (including chip mode).
  .gesture<ResizeDrag>(resizeGesture)

  // GESTURE 3 — in-row widgets (scrub drag / toggle click / chip click).
  // MUST precede the move gesture: begin() claims presses on widget rows so a
  // horizontal scrub edits the value instead of dragging the node (NOTES
  // wave-5 finding 2); it declines plain rows, which fall through to move.
  .gesture<WidgetDrag>(widgetGesture)

  // GESTURE 4 — mini-grid row click (wave 10): claims presses on visible grid
  // rows (tablePreview body, not chart, not chip mode) and fires the
  // onGridRowClick seam on a clean click. Drags DELEGATE to nodeMoveGesture
  // internally, so dragging a card by its grid body still moves it. Declared
  // after resize (bands win) and before move (else move claims everything).
  .gesture<GridClickDrag>(gridClickGesture)

  // GESTURE 4b — checklist row toggle (wave 11): claims presses on visible
  // checklist rows of select.checklist cards; clean click toggles the value
  // via setParam `excluded` in a batch (one undo entry per toggle); drags
  // DELEGATE to nodeMoveGesture. Declines chip mode, "…N more", placeholder.
  .gesture<ChecklistDrag>(checklistGesture)

  // GESTURE 5 — move the card (wires.ts owns the spec). `during` dispatches
  // live; the position spring is what makes it glide. Releasing over a
  // type-compatible wire splices the node in (one batch intent).
  .gesture<{ off: Vec }>(nodeMoveGesture)

  // T14: joins the upstream Tab ring; Enter/Space route to press (select) and
  // index.ts's onEnterKey opens the focused card's editor.
  .on(Focusable())

  .press((n): EditorIntent => ({ k: "select", sel: { kind: "node", id: n.props.id } }));

// ── adornment chips ──────────────────────────────────────────────────────────
// Every chip carries `a` — the host card's engagement alpha (chipAlpha). The
// style bakes it into every color so the whole chip fades as one unit, and
// press guards on CHIP_PRESS_MIN so a mostly-invisible chip never takes the
// click (adorn hit-tests would otherwise still route it during the fade).
//
// The EyeChip is deleted (W13-B): selection drives the 3D view; the reducer's
// setDisplay intent stays valid for MCP agents, but no card UI emits it.

const RunChip = part("pf-run")
  .props<{ node: string; a: number }>()
  .intrinsic(46, 16)
  .style((t, ch, p) => ({
    // Cream theme: flat light pill, hairline edge — dark text stays readable.
    fill: calpha(t.mix(t.surface, t.accent, 0.04 + 0.06 * ch.hover + 0.08 * ch.press), p.a),
    edge: calpha(t.mix(t.muted, t.accent, 0.3 + 0.7 * ch.hover), p.a),
    text: calpha(t.mix(t.text, t.textBright, ch.hover), p.a),
  }))
  .render((n, p, s) => {
    p.box(n.rect, 6, s.fill, s.edge, 1);
    p.label("▶ Run", n.rect.center, s.text, { size: 10, weight: 600 });
  })
  .press((n): EditorIntent =>
    n.props.a < CHIP_PRESS_MIN ? { k: "noop" } : { k: "run", node: n.props.node });

/** "?" on the header — toggles the help expando under the title. */
const HelpChip = part("pf-help")
  .props<{ node: string; on: boolean; a: number }>()
  .intrinsic(16, 16)
  .style((t, ch, p) => ({
    // Cream theme: stays a light pill in every state (dark "?" mark needs it).
    fill: calpha(t.mix(t.surface, t.accent, 0.03 + 0.06 * ch.hover + (p.on ? 0.08 : 0)), p.a),
    edge: calpha(t.mix(t.muted, t.accent, Math.max(ch.hover, p.on ? 1 : 0)), p.a),
    mark: calpha(t.mix(t.textDim, t.textBright, Math.max(ch.hover, p.on ? 1 : 0)), p.a),
  }))
  .render((n, p, s) => {
    p.box(n.rect, 8, s.fill, s.edge, 1);
    p.label("?", v(n.rect.center.x, n.rect.center.y + 0.5), s.mark, { size: 10, weight: 600 });
  })
  .press((n): EditorIntent =>
    n.props.a < CHIP_PRESS_MIN ? { k: "noop" } : { k: "toggleHelp", node: n.props.node });

/** "✕" in the footer — removes the node (Del on a selected node still works). */
const DelChip = part("pf-del")
  .props<{ node: string; a: number }>()
  .intrinsic(16, 14)
  .style((t, ch, p) => ({
    fill: calpha(rgb(192, 57, 43), (0.08 + 0.18 * ch.hover) * p.a),
    edge: calpha(t.mix(t.muted, rgb(192, 57, 43), ch.hover), p.a),
    mark: calpha(t.mix(t.textDim, rgb(192, 57, 43), ch.hover), p.a),
  }))
  .render((n, p, s) => {
    p.box(n.rect, 5, s.fill, s.edge, 1);
    const c = n.rect.center, w = 3.2;
    p.line(v(c.x - w, c.y - w), v(c.x + w, c.y + w), s.mark, 1.4);
    p.line(v(c.x - w, c.y + w), v(c.x + w, c.y - w), s.mark, 1.4);
  })
  .press((n): EditorIntent =>
    n.props.a < CHIP_PRESS_MIN ? { k: "noop" } : { k: "deleteNode", node: n.props.node });
