// Toast stack: transient feedback (save, write-pset, eval errors) bottom-left,
// sitting just above the #status line. Deliberately self-contained — main.ts
// calls it from host plumbing where no editor organ exists, so this module
// imports nothing from the rest of the editor.
//
// Newest toast lands at the BOTTOM of the stack (nearest the status line, where
// the eye already is); when a fourth arrives the oldest — the top one — leaves.

export type ToastTone = "info" | "ok" | "error";

const STACK_ID = "pf-toast-stack";
const MAX_VISIBLE = 3;
const INFO_MS = 4000;
const ERROR_MS = 7000; // errors linger: the user may be mid-gesture when one fires

// var() fallbacks: the stack can render before (or without) the editor's theme
// block, e.g. a host error during boot.
const CSS = `
#${STACK_ID} { position: fixed; left: 12px; bottom: 28px; z-index: 90;
  display: flex; flex-direction: column; gap: 6px; align-items: flex-start; }
.pf-toast { display: flex; align-items: center; gap: 8px; max-width: 340px;
  box-sizing: border-box; background: var(--pf-surface, #fff);
  border: 1px solid var(--pf-border, #ddd); border-radius: 6px; padding: 7px 10px;
  color: var(--pf-text, #26241f); font: 12px var(--pf-font, system-ui, sans-serif);
  box-shadow: 0 2px 10px rgba(0,0,0,.08); cursor: pointer; }
.pf-toast-dot { flex: none; width: 8px; height: 8px; border-radius: 50%;
  background: var(--pf-dim, #8a857b); }
.pf-toast-ok .pf-toast-dot { background: var(--pf-green, #4a7c47); }
.pf-toast-error .pf-toast-dot { background: var(--pf-danger, #b3423a); }
.pf-toast-msg { overflow-wrap: anywhere; }
`;

function ensureStyle(): void {
  if (document.getElementById("pf-toast-style")) return;
  const el = document.createElement("style");
  el.id = "pf-toast-style";
  el.textContent = CSS;
  document.head.appendChild(el);
}

function stack(): HTMLElement {
  let el = document.getElementById(STACK_ID);
  if (!el) {
    el = document.createElement("div");
    el.id = STACK_ID;
    document.body.appendChild(el);
  }
  return el;
}

// per-toast auto-dismiss timers, so eviction/click/clear can cancel them
const timers = new Map<HTMLElement, ReturnType<typeof setTimeout>>();

function dismiss(el: HTMLElement): void {
  const t = timers.get(el);
  if (t !== undefined) clearTimeout(t);
  timers.delete(el);
  el.remove();
}

export function showToast(msg: string, tone: ToastTone = "info", ms?: number): void {
  ensureStyle();
  const host = stack();
  const el = document.createElement("div");
  el.className = `pf-toast pf-toast-${tone}`;
  const dot = document.createElement("span");
  dot.className = "pf-toast-dot";
  const text = document.createElement("span");
  text.className = "pf-toast-msg";
  text.textContent = msg;
  el.append(dot, text);
  el.addEventListener("click", () => dismiss(el));
  host.appendChild(el);                              // newest at bottom
  while (host.children.length > MAX_VISIBLE)
    dismiss(host.children[0] as HTMLElement);        // oldest (top) evicted
  timers.set(el, setTimeout(() => dismiss(el), ms ?? (tone === "error" ? ERROR_MS : INFO_MS)));
}

/** Drop every toast and cancel its timer — tests and editor destroy. */
export function clearToasts(): void {
  const host = document.getElementById(STACK_ID);
  while (host?.firstElementChild) dismiss(host.firstElementChild as HTMLElement);
  // stragglers whose container was removed out from under them (jsdom resets)
  for (const t of timers.values()) clearTimeout(t);
  timers.clear();
}
