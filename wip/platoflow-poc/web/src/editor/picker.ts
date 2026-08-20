// Searchable option picker (UX study items #8, #13 — plan W2/T8): replaces the
// plain <select> popover for `dynamicEnum` / `modelPick` params. Type-to-filter
// list with counts ("Walls (216)" — count dim), keyboard arrows + Enter select,
// Esc cancels, current value highlighted. Commit is LIVE: picking dispatches
// setParam once and closes. Long lists render at most RENDER_CAP rows with a
// "…N more, keep typing" footer — `parameters` on a real model is hundreds of
// entries, which is exactly why the bare <select> was unusable (study §4).
//
// Routing lives in params.ts (the single param-editor routing point); this
// module owns the picker organ + the pure helpers the specs target.
//
// T14: the picker rides the editor's gratify `island()` (index.ts) — the
// runtime pins it over the param row's WORLD rect through pan/zoom, so this
// module never touches coordinates. The root is absolutely positioned at the
// island wrapper's origin; standalone (specs) it sits at the body's origin.
import type { GraphNode, NodeKindInfo, ParamSchema } from "../contracts";
import type { EditorIntent } from "./doc";

// ── pure helpers (spec targets) ──────────────────────────────────────────────

/** What providers may return per the widened EditorOpts.getEnumOptions seam. */
export type RawEnumOption = string | { value: string; count?: number };
export interface PickerOption { value: string; count?: number }

export const optionValue = (o: RawEnumOption): string =>
  typeof o === "string" ? o : o.value;

export const normalizeOptions = (raw: RawEnumOption[]): PickerOption[] =>
  raw.map((o) => (typeof o === "string" ? { value: o } : o));

/** Case-insensitive substring filter; empty/blank query keeps everything. */
export function filterOptions(opts: PickerOption[], query: string): PickerOption[] {
  const q = query.trim().toLowerCase();
  return q ? opts.filter((o) => o.value.toLowerCase().includes(q)) : opts;
}

/** Render at most this many rows; the rest collapse into the footer. */
export const RENDER_CAP = 50;

type PickableSchema = Extract<ParamSchema, { kind: "dynamicEnum" | "modelPick" }>;

export const isPickable = (s: ParamSchema): s is PickableSchema =>
  s.kind === "dynamicEnum" || s.kind === "modelPick";

/** The provider `source` for a pickable schema (modelPick reads the host's
 *  model list; dynamicEnum carries its own source). */
export const pickSource = (s: PickableSchema): string =>
  s.kind === "modelPick" ? "models" : s.source;

/** Auto-open-on-drop (study #13): the first param that is REQUIRED and unset —
 *  pickable kind, no current value, and no schema default to fall back on
 *  (load.model's `model` has default "snowdon", so it is never "required").
 *  A dropped By Type node IS a question — "which type?" — so ask it. */
export function firstUnsetPickable(info: NodeKindInfo, node: GraphNode): string | null {
  for (const [name, schema] of Object.entries(info.params)) {
    if (!isPickable(schema) || schema.default !== undefined) continue;
    const cur = node.params[name];
    if (cur === undefined || cur === null || cur === "") return name;
  }
  return null;
}

// ── DOM (house popover voice — see params.ts / focus.ts) ─────────────────────

const CSS = `
.pf-picker { position: absolute; left: 0; top: 0; z-index: 55; box-sizing: border-box; width: 260px;
  background: var(--pf-surface); border: 1px solid var(--pf-border-strong); border-radius: 7px;
  box-shadow: 0 4px 18px rgba(0,0,0,.08); padding: 8px 10px 9px;
  color: var(--pf-text); font: 12px var(--pf-font); }
.pf-picker label { display: block; margin: 0 0 4px; color: var(--pf-dim); font-size: 11px; }
.pf-picker input { width: 100%; box-sizing: border-box; background: var(--pf-input);
  color: var(--pf-text); border: 1px solid var(--pf-border); border-radius: 5px; padding: 5px 7px;
  font: 12px var(--pf-font); }
.pf-picker-list { margin-top: 6px; max-height: 264px; overflow-y: auto; }
.pf-picker-row { display: flex; gap: 6px; align-items: baseline; padding: 3px 6px;
  border-radius: 4px; cursor: pointer; }
.pf-picker-row:hover { background: var(--pf-hover); }
.pf-picker-active { background: var(--pf-hover); outline: 1px solid var(--pf-border-strong); }
.pf-picker-val { flex: 1; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.pf-picker-cur .pf-picker-val { color: var(--pf-text); font-weight: 600; }
.pf-picker-count { color: var(--pf-dim); font-size: 11px; }
.pf-picker-more { margin-top: 4px; color: var(--pf-dim); font-size: 10px; }
.pf-picker-empty { margin-top: 6px; color: var(--pf-dim); font-size: 11px; font-style: italic; }
`;

function ensureStyle(): void {
  if (document.getElementById("pf-picker-style")) return;
  const el = document.createElement("style");
  el.id = "pf-picker-style";
  el.textContent = CSS;
  document.head.appendChild(el);
}

export interface PickerHooks {
  dispatch(i: EditorIntent): void;
  getEnumOptions?(node: GraphNode, param: string, source: string): RawEnumOption[];
}

export interface Picker {
  /** Open for `node.params[name]`. Positioning is the island's job (index.ts
   *  computes the row's world rect per frame) — no anchor crosses this seam. */
  open(node: GraphNode, name: string, schema: PickableSchema): void;
  close(): void;
  current(): { node: string; name: string } | null;
  contains(target: EventTarget | null): boolean;
  /** The organ's root element — what the editor's island facet pins. */
  element(): HTMLElement;
  destroy(): void;
}

export function createPicker(hooks: PickerHooks): Picker {
  ensureStyle();
  const root = document.createElement("div");
  root.className = "pf-picker";
  root.style.display = "none";
  document.body.appendChild(root);
  // clicks inside the picker are not canvas gestures (dismissal-churn guard,
  // same rule as the popover and focus card)
  root.addEventListener("pointerdown", (ev) => ev.stopPropagation());

  let cur: { node: string; name: string } | null = null;
  let done = false;                                // one dispatch per session

  const close = () => {
    cur = null;
    root.style.display = "none";
    root.replaceChildren();
  };

  return {
    open(node, name, schema) {
      close();
      done = false;
      const source = pickSource(schema);
      const provided = normalizeOptions(hooks.getEnumOptions?.(node, name, source) ?? []);
      // current value: the param, or the schema default it will fall back to
      const raw = node.params[name];
      const curVal = typeof raw === "string" && raw !== "" ? raw
        : schema.default ?? "";
      // keep a current value the source no longer lists (old select behavior),
      // and offer "—" (clear) ahead of everything — typing filters it away
      const all: PickerOption[] = [
        { value: "" },
        ...(curVal && !provided.some((o) => o.value === curVal) ? [{ value: curVal }] : []),
        ...provided.filter((o) => o.value !== ""),
      ];

      const commit = (value: string) => {
        if (done) return;
        done = true;
        hooks.dispatch({ k: "graph", intent: { t: "setParam", node: node.id, name, value } });
        close();
      };

      const label = document.createElement("label");
      label.textContent = name;
      const search = document.createElement("input");
      search.type = "text";
      search.placeholder = "type to filter…";
      const list = document.createElement("div");
      list.className = "pf-picker-list";
      const footer = document.createElement("div");

      let visible: PickerOption[] = [];
      let active = 0;

      const paint = () => {
        for (let i = 0; i < list.children.length; i++)
          list.children[i].classList.toggle("pf-picker-active", i === active);
        const row = list.children[active];
        if (row instanceof HTMLElement && typeof row.scrollIntoView === "function")
          row.scrollIntoView({ block: "nearest" });
      };

      const render = () => {
        const filtered = filterOptions(all, search.value);
        visible = filtered.slice(0, RENDER_CAP);
        active = Math.max(0, visible.findIndex((o) => o.value === curVal));
        list.replaceChildren();
        for (const opt of visible) {
          const row = document.createElement("div");
          row.className = "pf-picker-row" + (opt.value === curVal ? " pf-picker-cur" : "");
          const val = document.createElement("span");
          val.className = "pf-picker-val";
          val.textContent = opt.value === "" ? "—" : opt.value;
          row.appendChild(val);
          if (opt.count !== undefined) {
            const count = document.createElement("span");
            count.className = "pf-picker-count";
            count.textContent = `(${opt.count})`;
            row.appendChild(count);
          }
          row.addEventListener("click", () => commit(opt.value));
          list.appendChild(row);
        }
        const over = filtered.length - visible.length;
        footer.className = over > 0 ? "pf-picker-more" : "pf-picker-empty";
        footer.textContent = over > 0 ? `…${over} more, keep typing`
          : visible.length === 0 ? "no matches" : "";
        footer.style.display = footer.textContent ? "block" : "none";
        paint();
      };

      search.addEventListener("input", render);
      search.addEventListener("keydown", (ev) => {
        if (ev.key === "ArrowDown" || ev.key === "ArrowUp") {
          ev.preventDefault();
          if (!visible.length) return;
          active = Math.min(visible.length - 1,
            Math.max(0, active + (ev.key === "ArrowDown" ? 1 : -1)));
          paint();
        } else if (ev.key === "Enter") {
          ev.preventDefault(); ev.stopPropagation();
          if (visible[active]) commit(visible[active].value);
        } else if (ev.key === "Escape") {
          ev.stopPropagation();                    // cancel: no dispatch
          close();
        }
      });

      root.replaceChildren(label, search, list, footer);
      cur = { node: node.id, name };
      render();
      root.style.display = "block";
      search.focus();
    },
    close,
    current: () => cur,
    contains: (t) => t instanceof Node && root.contains(t),
    element: () => root,
    destroy() { close(); root.remove(); },
  };
}
