// Click-to-edit param popover: ONE floating DOM editor positioned over the
// param row that was clicked. Field building is ported from the retired side
// panel (panel.ts) — text entry stays in the DOM (house precedent: Gratify
// draws the graph, HTML takes the typing).
//
// Commit policy (W1/T2, study item #4): `ParamSchema.commit` decides. "live"
// (the default) dispatches setParam on every input event, exactly as before.
// "explicit" buffers locally and commits on Ctrl-Enter or blur/close; Esc
// reverts. The policy is the pure function `commitPolicy` below — specs test
// it without any DOM.
//
// `text` params open the docked focus editor (focus.ts, study item #9) instead
// of the popover; this module stays the single routing point, so index.ts's
// close rules (Esc, click-away, selection change, external dispatch) cover
// both editors through the one ParamPopover handle.
//
// Closing detail: close() COMMITS a dirty explicit buffer (click-away keeps
// the edit); cancel() reverts FIRST and then closes. index.ts's single window-
// capture Esc listener calls cancel() — the old registration-order treaty
// (params' capture hook racing index.ts's) is gone (T14).
//
// Positioning (T14): none here. The popover root is absolutely positioned at
// its parent's origin; in the app that parent is the editor's island wrapper
// (index.ts), which the gratify runtime pins over the param row's WORLD rect
// through pan/zoom — the editors survive pan/zoom by construction.
import type { GraphNode, NodeKindInfo, NodeStatus, ParamSchema } from "../contracts";
import type { EditorIntent } from "./doc";
import { createFocusEditor, type FocusEditor } from "./focus";
import {
  createPicker, firstUnsetPickable, isPickable, optionValue,
  type Picker, type RawEnumOption,
} from "./picker";

export interface PopoverHooks {
  dispatch(i: EditorIntent): void;
  kindInfo(kind: string): NodeKindInfo | undefined;
  /** Options for dynamicEnum/modelPick. Entries may carry counts (the widened
   *  EditorOpts seam, T8) — the picker renders them; every other consumer
   *  (select fallback, focus-editor chips) sees bare values via optionValue. */
  getEnumOptions?(node: GraphNode, param: string, source: string): RawEnumOption[];
  /** Node's cached eval status — feeds the focus editor's result preview. */
  getStatus?(nodeId: string): NodeStatus | undefined;
  /** Route dynamicEnum/modelPick to the searchable picker (T8). Migration
   *  latch: default false keeps the legacy <select> path (and its pre-T8
   *  specs) intact; index.ts passes true at the W2 join. Delete the flag AND
   *  the select fallback once params.spec.ts is updated. */
  usePicker?: boolean;
}

export interface ParamPopover {
  /** Open the editor for `node.params[name]` (popover, picker or focus card —
   *  routing by schema kind; the island in index.ts owns where it appears). */
  open(node: GraphNode, name: string): void;
  /** Auto-open-on-drop (T8, study #13): if `node` has a required unset
   *  dynamicEnum/modelPick param, open the picker for the FIRST one (no anchor
   *  — the picker self-places). Returns the param name, or null when nothing
   *  needed asking. Works independently of `usePicker`. */
  openFirstUnset(node: GraphNode): string | null;
  close(): void;
  /** Esc semantics: revert any dirty explicit buffer, THEN close (close alone
   *  commits). The one Esc entry point — index.ts's window capture calls it. */
  cancel(): void;
  /** Which param is being edited (popover, picker OR focus editor), or null. */
  current(): { node: string; name: string } | null;
  /** True when the event target is inside the popover, picker or focus card. */
  contains(target: EventTarget | null): boolean;
  /** The world-anchored organ to pin over the edited param row (popover or
   *  picker — the focus editor stays a screen-docked card and is excluded).
   *  index.ts's island facet reads this every frame. */
  islandTarget(): { el: HTMLElement; node: string; name: string } | null;
  destroy(): void;
}

// ── commit policy (pure — the specs' main target) ────────────────────────────

export type EditEvent =
  | { type: "input" }
  | { type: "key"; key: string; ctrl?: boolean; meta?: boolean }
  | { type: "blur" };

export type CommitAction =
  | "dispatch"   // live: dispatch the new value now, editor stays open
  | "buffer"     // explicit: hold the value locally, dispatch nothing
  | "commit"     // explicit: dispatch the buffered value (if dirty) and close
  | "revert"     // explicit: restore the last committed value and close
  | "none";      // ignore

/** What a text-control event does under the schema's commit mode. */
export function commitPolicy(schema: ParamSchema, ev: EditEvent): CommitAction {
  const explicit = schema.commit === "explicit";
  switch (ev.type) {
    case "input": return explicit ? "buffer" : "dispatch";
    case "blur": return explicit ? "commit" : "none";
    case "key":
      if (!explicit) return "none";
      if (ev.key === "Enter" && (ev.ctrl || ev.meta)) return "commit";
      if (ev.key === "Escape") return "revert";
      return "none";
  }
}

// ── DOM ──────────────────────────────────────────────────────────────────────

const CSS = `
.pf-popover { position: absolute; left: 0; top: 0; z-index: 50; box-sizing: border-box;
  background: var(--pf-surface); border: 1px solid var(--pf-border-strong); border-radius: 7px;
  box-shadow: 0 4px 18px rgba(0,0,0,.08); padding: 8px 10px 9px;
  color: var(--pf-text); font: 12px var(--pf-font); min-width: 200px; }
.pf-popover label { display: block; margin: 0 0 4px; color: var(--pf-dim); font-size: 11px; }
.pf-popover .pf-doc { margin-top: 6px; color: var(--pf-faint); font-size: 10px; line-height: 1.4;
  max-width: 340px; }
.pf-popover input[type=text], .pf-popover input[type=number], .pf-popover select,
.pf-popover textarea {
  width: 100%; box-sizing: border-box; background: var(--pf-input); color: var(--pf-text);
  border: 1px solid var(--pf-border); border-radius: 5px; padding: 5px 7px;
  font: 12px var(--pf-font); }
.pf-popover textarea { width: 360px; min-height: 96px;
  font: 11px ui-monospace, monospace; resize: both; }
.pf-popover input[type=checkbox] { width: auto; }
`;

function ensureStyle(): void {
  if (document.getElementById("pf-popover-style")) return;
  const el = document.createElement("style");
  el.id = "pf-popover-style";
  el.textContent = CSS;
  document.head.appendChild(el);
}

export function createParamPopover(hooks: PopoverHooks): ParamPopover {
  ensureStyle();
  const root = document.createElement("div");
  root.className = "pf-popover";
  root.style.display = "none";
  document.body.appendChild(root);
  // clicks inside the popover must not reach document/canvas handlers — closing
  // or using an editor is not a canvas gesture (the study's "dismissal churn")
  root.addEventListener("pointerdown", (ev) => ev.stopPropagation());

  // bare-value adapter: focus.ts chips and the legacy select consume strings;
  // only the picker sees the {value, count?} objects
  const enumValues = hooks.getEnumOptions
    ? (node: GraphNode, param: string, source: string) =>
        hooks.getEnumOptions!(node, param, source).map(optionValue)
    : undefined;

  const focusEd: FocusEditor = createFocusEditor({
    dispatch: hooks.dispatch,
    kindInfo: hooks.kindInfo,
    getEnumOptions: enumValues,
    getStatus: hooks.getStatus,
  });

  const picker: Picker = createPicker({
    dispatch: hooks.dispatch,
    getEnumOptions: hooks.getEnumOptions,
  });

  let cur: { node: string; name: string } | null = null;
  /** Dirty-buffer hooks of the OPEN popover control (explicit mode only). */
  let pending: { commit(): void; revert(): void } | null = null;

  const close = () => {
    const p = pending;
    pending = null;
    cur = null;
    p?.commit();                                 // no-op when clean or reverted
    root.style.display = "none";
    root.replaceChildren();
    focusEd.close();                             // commits its own buffer
    picker.close();                              // no buffer — nothing to commit
  };

  /** Esc: revert before close (close commits). ONE entry point — index.ts's
   *  window-capture Esc handler and the field-level Esc handlers both land on
   *  the same semantics; no listener-registration-order treaty anymore. */
  const cancel = () => {
    if (!cur && !focusEd.current() && !picker.current()) return;
    pending?.revert();
    focusEd.revert();
    close();
  };

  const setParam = (node: string, name: string, value: unknown) =>
    hooks.dispatch({ k: "graph", intent: { t: "setParam", node, name, value } });

  /** Ported from panel.ts `field()` — one control per ParamSchema kind. */
  function field(node: GraphNode, name: string, schema: ParamSchema): HTMLElement {
    const wrap = document.createElement("div");
    const label = document.createElement("label");
    label.textContent = name;
    wrap.appendChild(label);
    const current = node.params[name];

    const options = (source: string): string[] => {
      const dyn = enumValues?.(node, name, source) ?? [];
      const cur = typeof current === "string" ? current : "";
      return cur && !dyn.includes(cur) ? [cur, ...dyn] : dyn;
    };

    const select = (opts: string[]): HTMLElement => {
      const el = document.createElement("select");
      for (const o of ["", ...opts.filter((o) => o !== "")]) {
        const opt = document.createElement("option");
        opt.value = o; opt.textContent = o === "" ? "—" : o;
        el.appendChild(opt);
      }
      el.value = typeof current === "string" ? current : "";
      el.addEventListener("change", () => setParam(node.id, name, el.value));
      return el;
    };

    /** Text-ish control wired through commitPolicy: live = dispatch per input
     *  (today's behavior, exactly); explicit = buffer + Ctrl-Enter/blur commit. */
    const wireText = (el: HTMLInputElement | HTMLTextAreaElement) => {
      const box = el as HTMLTextAreaElement;     // union collapses listener overloads
      let committed = el.value;
      const commit = () => {
        if (el.value === committed) return;
        committed = el.value;
        setParam(node.id, name, el.value);
      };
      const revert = () => { el.value = committed; };
      if (schema.commit === "explicit") pending = { commit, revert };
      box.addEventListener("input", () => {
        if (commitPolicy(schema, { type: "input" }) === "dispatch") commit();
      });
      box.addEventListener("keydown", (ev) => {
        const act = commitPolicy(schema, { type: "key", key: ev.key, ctrl: ev.ctrlKey, meta: ev.metaKey });
        if (act === "commit") { ev.preventDefault(); ev.stopPropagation(); close(); }
        else if (act === "revert") { revert(); close(); ev.stopPropagation(); }
      });
      box.addEventListener("blur", (ev) => {
        if (commitPolicy(schema, { type: "blur" }) !== "commit") return;
        if (ev.relatedTarget instanceof Node && root.contains(ev.relatedTarget)) { commit(); return; }
        if (cur) close();
      });
    };

    switch (schema.kind) {
      case "boolean": {
        const el = document.createElement("input");
        el.type = "checkbox";
        el.checked = current === true;
        el.addEventListener("change", () => setParam(node.id, name, el.checked));
        label.prepend(el, " ");
        break;
      }
      case "number": {
        const el = document.createElement("input");
        el.type = "number";
        if (schema.min !== undefined) el.min = String(schema.min);
        if (schema.max !== undefined) el.max = String(schema.max);
        if (schema.step !== undefined) el.step = String(schema.step);
        el.value = String(current ?? schema.default ?? 0);
        el.addEventListener("input", () => setParam(node.id, name, Number(el.value)));
        wrap.appendChild(el);
        break;
      }
      case "text": {
        const el = document.createElement("textarea");
        el.value = String(current ?? schema.default ?? "");
        wireText(el);
        wrap.appendChild(el);
        break;
      }
      case "enum":
        wrap.appendChild(select(schema.options));
        break;
      case "dynamicEnum":
        wrap.appendChild(select(options(schema.source)));
        break;
      case "modelPick": {
        const el = select(options("models"));
        if (!node.params[name] && schema.default) {
          (el as HTMLSelectElement).value = schema.default;
        }
        wrap.appendChild(el);
        break;
      }
      case "string": {
        const el = document.createElement("input");
        el.type = "text";
        if (schema.placeholder) el.placeholder = schema.placeholder;
        el.value = String(current ?? "");
        wireText(el);
        wrap.appendChild(el);
        break;
      }
    }

    if (schema.doc) {
      const doc = document.createElement("div");
      doc.className = "pf-doc";
      doc.textContent = schema.doc;
      wrap.appendChild(doc);
    }
    return wrap;
  }

  return {
    open(node, name) {
      const schema = hooks.kindInfo(node.kind)?.params[name];
      if (!schema) return;
      close();                                   // commit any previous session first
      if (schema.kind === "text") {              // study #9: text params get the focus editor
        focusEd.open(node, name, schema);
        return;
      }
      if (hooks.usePicker && isPickable(schema)) {  // study #8: searchable picker
        picker.open(node, name, schema);
        return;
      }
      root.replaceChildren(field(node, name, schema));
      cur = { node: node.id, name };
      root.style.display = "block";              // where = the island's business
      const focus = root.querySelector<HTMLElement>("input, textarea, select");
      focus?.focus();
      if (focus instanceof HTMLInputElement && focus.type === "text") focus.select();
    },
    openFirstUnset(node) {
      const info = hooks.kindInfo(node.kind);
      if (!info) return null;
      const name = firstUnsetPickable(info, node);
      if (!name) return null;
      const schema = info.params[name];
      if (!isPickable(schema)) return null;      // firstUnsetPickable guarantees this
      close();                                   // commit any open session first
      picker.open(node, name, schema);           // the island anchors it to the row
      return name;
    },
    close,
    cancel,
    current: () => cur ?? picker.current() ?? focusEd.current(),
    contains: (t) => (t instanceof Node && root.contains(t)) || picker.contains(t)
      || focusEd.contains(t),
    islandTarget() {
      if (cur) return { el: root, ...cur };
      const p = picker.current();
      return p ? { el: picker.element(), ...p } : null;
    },
    destroy() {
      close();
      focusEd.destroy();
      picker.destroy();
      root.remove();
    },
  };
}
