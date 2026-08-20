// Canvas hover tooltip: node headers (kind description), sockets
// (`name · type — doc` from PortSpec.doc), and chart bars (`label: value`).
// One mechanism, one 500 ms timer; hit-testing goes toWorld → geom's pure
// layout functions, so tooltips can never disagree with what was drawn.
//
// Plain DOM on purpose (house precedent: Gratify draws the graph, HTML does text).
import type { GraphDoc, NodeKindInfo, NodeStatus } from "../contracts";
import { CATEGORY_CSS, barAt, inHeader, nodeLayout, socketAt, wiredInputsOf, type ChartData } from "./geom";

const HOVER_MS = 500;

const CSS = `
.pf-tip { position: fixed; z-index: 40; max-width: 300px; pointer-events: none;
  background: var(--pf-surface); border: 1px solid var(--pf-border-strong); border-radius: 7px;
  box-shadow: 0 4px 16px rgba(0,0,0,.08); padding: 8px 10px;
  color: var(--pf-text); font: 12px var(--pf-font); line-height: 1.45; }
.pf-tip .pf-tip-title { color: var(--pf-text); font-weight: 600; margin-bottom: 3px; }
.pf-tip .pf-tip-kind { color: var(--pf-dim); font-size: 10px; margin-left: 6px; font-weight: 400; }
.pf-tip .pf-tip-mono { font-family: ui-monospace, monospace; font-size: 11px; }
.pf-tip .pf-tip-warn { color: #9A7420; margin-top: 3px; }
`;

export function ensureHelpStyle(): void {
  if (document.getElementById("pf-help-style")) return;
  const el = document.createElement("style");
  el.id = "pf-help-style";
  el.textContent = CSS;
  document.head.appendChild(el);
}

/** First sentence of a description — the palette's one-line form. */
export function firstSentence(text: string, max = 96): string {
  const dot = text.search(/\.\s/);
  const s = dot > 0 ? text.slice(0, dot + 1) : text;
  return s.length > max ? s.slice(0, max - 1).trimEnd() + "…" : s;
}

// ── hover tooltip ────────────────────────────────────────────────────────────

export interface NodeTipHooks {
  getDoc(): GraphDoc;
  kindInfo(kind: string): NodeKindInfo | undefined;
  getStatus(id: string): NodeStatus | undefined;
  helpOpen(id: string): boolean;
  /** Canvas-relative screen point → world point (the runtime's inverse viewport). */
  toWorld(p: { x: number; y: number }): { x: number; y: number };
}

export interface NodeTip { hide(): void; destroy(): void; }

type Hit =
  | { key: string; kind: "header"; info: NodeKindInfo; warning?: string }
  | { key: string; kind: "socket"; text: string }
  | { key: string; kind: "bar"; text: string };

/** Hovering a node's header, a socket, or a chart bar for ~500ms pops a
 *  tooltip; any move away, pan, zoom or press takes it down again. */
export function createNodeTip(canvas: HTMLCanvasElement, hooks: NodeTipHooks): NodeTip {
  ensureHelpStyle();
  const tip = document.createElement("div");
  tip.className = "pf-tip";
  tip.style.display = "none";
  document.body.appendChild(tip);

  let timer: number | undefined;
  let shownFor: string | null = null;

  const hide = () => {
    clearTimeout(timer);
    timer = undefined;
    shownFor = null;
    tip.style.display = "none";
  };

  /** What is under this canvas-relative point, topmost node first.
   *  Priority within a node: bar → socket → header. */
  const hitTest = (cx: number, cy: number): Hit | null => {
    const w = hooks.toWorld({ x: cx, y: cy });
    const doc = hooks.getDoc();
    for (let i = doc.nodes.length - 1; i >= 0; i--) {
      const n = doc.nodes[i];
      const info = hooks.kindInfo(n.kind);
      if (!info) continue;
      const status = hooks.getStatus(n.id);
      const l = nodeLayout(info,
        { params: n.params, wiredInputs: wiredInputsOf(doc.edges, n.id, info), w: n.w },
        { helpOpen: hooks.helpOpen(n.id), zoom: 1 }, status);
      if (w.x < n.x - 8 || w.x > n.x + l.w + 8 || w.y < n.y || w.y > n.y + l.h) continue;

      const chart = status?.chart as ChartData | undefined;
      if (l.bodyH > 0 && chart?.values.length) {
        const bi = barAt(l, n.x, n.y, chart, w);
        if (bi !== null) {
          return { key: `${n.id}|bar|${bi}`, kind: "bar", text: `${chart.labels[bi] ?? ""}: ${chart.values[bi]}` };
        }
      }
      const sock = socketAt(l, info, n.x, n.y, w);
      if (sock) {
        const { name, type, doc: pdoc } = sock.spec;
        return { key: `${n.id}|${sock.dir}|${name}`, kind: "socket", text: `${name} · ${type}${pdoc ? ` — ${pdoc}` : ""}` };
      }
      if (inHeader(l, n.x, n.y, w)) {
        // W9-C: the header tooltip carries the status warning (full text —
        // the footer only fits a truncated line).
        return { key: `${n.id}|header`, kind: "header", info, warning: status?.warning };
      }
      return null;                        // over the node but nothing tooltippable
    }
    return null;
  };

  const place = (clientX: number, clientY: number) => {
    tip.style.display = "block";
    // keep it on screen: flip left/up near the edges
    const r = tip.getBoundingClientRect();
    const x = Math.min(clientX + 16, window.innerWidth - r.width - 8);
    const y = clientY + 22 + r.height > window.innerHeight ? clientY - r.height - 12 : clientY + 22;
    tip.style.left = `${Math.max(8, x)}px`;
    tip.style.top = `${Math.max(8, y)}px`;
  };

  const show = (hit: Hit, clientX: number, clientY: number) => {
    tip.replaceChildren();
    if (hit.kind === "header") {
      const title = document.createElement("div");
      title.className = "pf-tip-title";
      title.textContent = hit.info.label;
      title.style.color = CATEGORY_CSS[hit.info.category];
      const kind = document.createElement("span");
      kind.className = "pf-tip-kind";
      kind.textContent = hit.info.kind;
      title.appendChild(kind);
      const body = document.createElement("div");
      body.textContent = hit.info.description;
      tip.append(title, body);
      if (hit.warning) {
        const warn = document.createElement("div");
        warn.className = "pf-tip-warn";
        warn.textContent = `⚠ ${hit.warning}`;
        tip.appendChild(warn);
      }
    } else {
      const body = document.createElement("div");
      if (hit.kind === "bar") body.className = "pf-tip-mono";
      body.textContent = hit.text;
      tip.appendChild(body);
    }
    place(clientX, clientY);
  };

  const onMove = (ev: MouseEvent) => {
    const b = canvas.getBoundingClientRect();
    const hit = hitTest(ev.clientX - b.left, ev.clientY - b.top);
    if (!hit) { if (shownFor || timer) hide(); return; }
    if (hit.key === shownFor) return;                       // already showing this one
    clearTimeout(timer);
    const { clientX, clientY } = ev;
    timer = window.setTimeout(() => { shownFor = hit.key; show(hit, clientX, clientY); }, HOVER_MS);
  };

  canvas.addEventListener("mousemove", onMove);
  canvas.addEventListener("mouseleave", hide);
  canvas.addEventListener("pointerdown", hide);
  canvas.addEventListener("wheel", hide, { passive: true });

  return {
    hide,
    destroy() {
      hide();
      canvas.removeEventListener("mousemove", onMove);
      canvas.removeEventListener("mouseleave", hide);
      canvas.removeEventListener("pointerdown", hide);
      canvas.removeEventListener("wheel", hide);
      tip.remove();
    },
  };
}
