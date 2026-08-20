// Focus editor for `text` params (UX study item #9): a docked card at the
// window's right edge that REPLACES the popover for multiline text (SQL,
// expressions, questions). Plain DOM per house rule. Three organs:
//   1. monospace textarea with explicit-commit semantics (commitPolicy),
//   2. an "input schema" list (SQL views + model columns/parameters, or the
//      expression's in-scope names + channels) — click a chip to insert it,
//   3. a result preview from the node's cached NodeStatus (summary/detail),
//      refreshed by polling while open (status flows on the editor doc, not
//      through DOM props).
// Also carries the visually-marked empty slot for the future AI-assist button
// (study §5.2) — build the surface now, the button drops in later.
//
// Opening/closing is routed by params.ts: param click on a `text` schema opens
// this card instead of the popover; popover.close() (selection change,
// click-away, external dispatch) closes it. close() COMMITS a dirty buffer;
// Esc reverts first (popover.cancel() — index.ts's window capture — calls
// revert() before close). The card is DELIBERATELY not an island: it is
// docked in screen space (fixed, right edge), so it survives pan/zoom by
// construction — pinning it to a world rect would pan it off-screen.
import type { GraphNode, NodeKindInfo, NodeStatus, ParamSchema } from "../contracts";
import type { EditorIntent } from "./doc";
import { commitPolicy } from "./params";

export interface FocusHooks {
  dispatch(i: EditorIntent): void;
  kindInfo(kind: string): NodeKindInfo | undefined;
  getEnumOptions?(node: GraphNode, param: string, source: string): string[];
  /** Node's cached eval status, for the result preview. Optional: without it
   *  the preview shows a placeholder. index.ts supplies `rt.doc.status[id]`. */
  getStatus?(nodeId: string): NodeStatus | undefined;
}

export interface FocusEditor {
  open(node: GraphNode, name: string, schema: ParamSchema): void;
  /** Commits a dirty buffer, then hides. */
  close(): void;
  /** Restore the buffer to the last committed value (call before close for Esc). */
  revert(): void;
  current(): { node: string; name: string } | null;
  contains(target: EventTarget | null): boolean;
  destroy(): void;
}

const CSS = `
.pf-focus { position: fixed; z-index: 60; top: 44px; right: 10px; bottom: 10px;
  width: 380px; max-width: 44vw; display: flex; flex-direction: column; gap: 8px;
  box-sizing: border-box; background: var(--pf-surface); border: 1px solid var(--pf-border-strong);
  border-radius: 7px; box-shadow: 0 4px 18px rgba(0,0,0,.08); padding: 10px 12px;
  color: var(--pf-text); font: 12px var(--pf-font); }
.pf-focus .pf-focus-head { display: flex; align-items: baseline; gap: 6px; }
.pf-focus .pf-focus-title { color: var(--pf-text); font-weight: 600; }
.pf-focus .pf-focus-param { color: var(--pf-dim); font-size: 11px; flex: 1; }
.pf-focus .pf-focus-close { cursor: pointer; color: var(--pf-dim); background: none;
  border: none; font: 13px var(--pf-font); padding: 0 2px; }
.pf-focus .pf-focus-close:hover { color: var(--pf-text); }
.pf-focus textarea { flex: 1; min-height: 110px; box-sizing: border-box; width: 100%;
  background: var(--pf-input); color: var(--pf-text); border: 1px solid var(--pf-border); border-radius: 5px;
  padding: 6px 8px; font: 12px ui-monospace, monospace; resize: none; }
.pf-focus .pf-focus-hint { color: var(--pf-faint); font-size: 10px; }
.pf-focus .pf-focus-sect { font-size: 10px; color: var(--pf-dim); text-transform: uppercase;
  letter-spacing: .06em; margin-top: 2px; }
.pf-focus .pf-focus-schema { overflow-y: auto; max-height: 28%; display: flex;
  flex-wrap: wrap; gap: 4px; align-content: flex-start; }
.pf-focus .pf-chip { background: var(--pf-input); border: 1px solid var(--pf-border); border-radius: 4px;
  padding: 1px 6px; font: 11px ui-monospace, monospace; color: var(--pf-text); cursor: pointer; }
.pf-focus .pf-chip:hover { border-color: var(--pf-border-strong); background: var(--pf-hover); }
.pf-focus .pf-focus-preview { overflow-y: auto; max-height: 26%; background: var(--pf-input);
  border: 1px solid var(--pf-border); border-radius: 5px; padding: 6px 8px;
  font: 11px ui-monospace, monospace; white-space: pre-wrap; }
.pf-focus .pf-focus-preview .pf-ok { color: var(--pf-green); }
.pf-focus .pf-focus-preview .pf-err { color: var(--pf-danger); }
.pf-focus .pf-focus-preview .pf-dim { color: var(--pf-dim); }
.pf-focus .pf-ai-slot { border: 1px dashed var(--pf-border-strong); border-radius: 6px; padding: 5px;
  color: var(--pf-dim); font-size: 10px; text-align: center; user-select: none; }
`;

function ensureStyle(): void {
  if (document.getElementById("pf-focus-style")) return;
  const el = document.createElement("style");
  el.id = "pf-focus-style";
  el.textContent = CSS;
  document.head.appendChild(el);
}

/** What the param can legitimately reference — the "input schema" panel.
 *  SQL/question params see the host's DuckDB views plus whatever the wired
 *  model exposes; expressions see their documented in-scope names + channels. */
export function schemaGroups(
  node: GraphNode,
  param: string,
  opts: (source: string) => string[],
): { title: string; items: string[] }[] {
  const groups =
    node.kind === "compute.expr"
      ? [
          { title: "in scope", items: ["gid", "type", "name", "level", "param('Name')", "ch('name')"] },
          { title: "channels", items: opts("channels") },
        ]
      : [
          { title: "views", items: ["EntityText", "ParameterText", "RelationText"] },
          { title: "columns", items: opts("columns") },
          { title: "parameters", items: opts("parameters") },
        ];
  return groups.filter((g) => g.items.length > 0);
}

export function createFocusEditor(hooks: FocusHooks): FocusEditor {
  ensureStyle();
  const root = document.createElement("div");
  root.className = "pf-focus";
  root.style.display = "none";
  (document.getElementById("pf-focus-host") ?? document.body).appendChild(root);
  // clicks inside the card must not reach document/canvas handlers (dismissal churn)
  root.addEventListener("pointerdown", (ev) => ev.stopPropagation());

  let cur: { node: string; name: string } | null = null;
  let pending: { commit(): void; revert(): void } | null = null;
  let pollTimer: ReturnType<typeof setInterval> | null = null;

  const close = () => {
    const p = pending;
    pending = null;
    cur = null;
    if (pollTimer !== null) { clearInterval(pollTimer); pollTimer = null; }
    p?.commit();                                 // click-away/✕ keep the edit; Esc reverted first
    root.style.display = "none";
    root.replaceChildren();
  };

  const renderPreview = (el: HTMLElement, nodeId: string) => {
    const s = hooks.getStatus?.(nodeId);
    el.replaceChildren();
    if (!s) {
      const d = document.createElement("span");
      d.className = "pf-dim";
      d.textContent = "no result yet";
      el.appendChild(d);
      return;
    }
    const line = (text: string, cls: string) => {
      const div = document.createElement("div");
      div.className = cls;
      div.textContent = text;
      el.appendChild(div);
    };
    line(s.state + (s.summary ? ` — ${s.summary}` : ""), s.state === "error" ? "pf-err" : "pf-ok");
    if (s.detail) line(s.detail, "pf-dim");
    if (s.state === "error" && s.message) line(s.message, "pf-err");
  };

  return {
    open(node, name, schema) {
      close();                                   // one session at a time
      ensureStyle();
      root.replaceChildren();

      // ── header ────────────────────────────────────────────────────────────
      const head = document.createElement("div");
      head.className = "pf-focus-head";
      const title = document.createElement("span");
      title.className = "pf-focus-title";
      title.textContent = hooks.kindInfo(node.kind)?.label ?? node.kind;
      const pname = document.createElement("span");
      pname.className = "pf-focus-param";
      pname.textContent = name;
      const x = document.createElement("button");
      x.className = "pf-focus-close";
      x.textContent = "✕";
      x.addEventListener("click", () => close());
      head.append(title, pname, x);
      root.appendChild(head);

      // ── monospace editor with commit-policy semantics ─────────────────────
      const ta = document.createElement("textarea");
      const initial = String(node.params[name] ?? (schema.kind === "text" ? schema.default ?? "" : ""));
      ta.value = initial;
      let committed = initial;
      const commit = () => {
        if (ta.value === committed) return;
        committed = ta.value;
        hooks.dispatch({ k: "graph", intent: { t: "setParam", node: node.id, name, value: ta.value } });
      };
      const revert = () => { ta.value = committed; };
      pending = { commit, revert };
      ta.addEventListener("input", () => {
        if (commitPolicy(schema, { type: "input" }) === "dispatch") commit();
      });
      ta.addEventListener("keydown", (ev) => {
        const act = commitPolicy(schema, { type: "key", key: ev.key, ctrl: ev.ctrlKey, meta: ev.metaKey });
        if (act === "commit") { ev.preventDefault(); ev.stopPropagation(); close(); }
        else if (act === "revert") { revert(); close(); ev.stopPropagation(); }
      });
      ta.addEventListener("blur", (ev) => {
        if (commitPolicy(schema, { type: "blur" }) !== "commit") return;
        // focus moved within the card (schema chip, preview): commit but stay open
        if (ev.relatedTarget instanceof Node && root.contains(ev.relatedTarget)) { commit(); return; }
        if (cur) close();
      });
      root.appendChild(ta);

      const hint = document.createElement("div");
      hint.className = "pf-focus-hint";
      hint.textContent = schema.commit === "explicit"
        ? "Ctrl-Enter or click away to commit · Esc to revert"
        : "commits as you type";
      root.appendChild(hint);

      // ── input schema list ─────────────────────────────────────────────────
      const opts = (source: string) => hooks.getEnumOptions?.(node, name, source) ?? [];
      for (const group of schemaGroups(node, name, opts)) {
        const sect = document.createElement("div");
        sect.className = "pf-focus-sect";
        sect.textContent = group.title;
        root.appendChild(sect);
        const list = document.createElement("div");
        list.className = "pf-focus-schema";
        for (const item of group.items) {
          const chip = document.createElement("span");
          chip.className = "pf-chip";
          chip.textContent = item;
          // pointerdown preventDefault keeps the textarea focused (no blur-commit-close)
          chip.addEventListener("pointerdown", (ev) => ev.preventDefault());
          chip.addEventListener("click", () => {
            const at = ta.selectionStart ?? ta.value.length;
            ta.value = ta.value.slice(0, at) + item + ta.value.slice(ta.selectionEnd ?? at);
            const end = at + item.length;
            ta.setSelectionRange(end, end);
            ta.focus();
            ta.dispatchEvent(new Event("input", { bubbles: true }));  // buffer (or live-commit)
          });
          list.appendChild(chip);
        }
        root.appendChild(list);
      }

      // ── result preview (cached status; polled — status lives on EditorDoc) ─
      const psect = document.createElement("div");
      psect.className = "pf-focus-sect";
      psect.textContent = "result";
      root.appendChild(psect);
      const preview = document.createElement("div");
      preview.className = "pf-focus-preview";
      renderPreview(preview, node.id);
      root.appendChild(preview);
      pollTimer = setInterval(() => renderPreview(preview, node.id), 500);

      // ── AI-assist slot (study §5.2 — surface now, button later) ───────────
      const ai = document.createElement("div");
      ai.className = "pf-ai-slot";
      ai.textContent = "◇ AI assist — reserved";
      root.appendChild(ai);

      cur = { node: node.id, name };
      root.style.display = "flex";
      ta.focus();
    },
    close,
    revert() { pending?.revert(); },
    current: () => cur,
    contains: (t) => t instanceof Node && root.contains(t),
    destroy() { close(); root.remove(); },
  };
}
