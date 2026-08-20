// Guided walkthrough (Help ▸ Run walkthrough). A small floating card steps
// through the demo beats; each step may run an action (load an example, open
// the palette) on entry. Plain DOM in the house popover voice — the canvas
// stays fully interactive so the user can poke at what each step shows.
//
// main.ts owns the step list (it has the editor + example loader in scope);
// this module owns only the card UI and the step walking.

export interface TourStep {
  title: string;
  body: string;
  /** Runs when the step is shown (load a demo, open a panel). Errors are
   *  swallowed — a dead host must not kill the tour. */
  run?: () => void | Promise<void>;
}

export interface Tour {
  start(): void;
  destroy(): void;
}

const CSS = `
.pf-tour { position: fixed; z-index: 70; left: 50%; bottom: 18px; transform: translateX(-50%);
  width: 420px; max-width: 88vw; box-sizing: border-box; display: none;
  background: var(--pf-surface); border: 1px solid var(--pf-border-strong); border-radius: 8px;
  box-shadow: 0 6px 24px rgba(0,0,0,.12); padding: 12px 14px;
  color: var(--pf-text); font: 13px var(--pf-font); line-height: 1.5; }
.pf-tour.pf-open { display: block; }
.pf-tour .pf-tour-head { display: flex; align-items: baseline; gap: 8px; margin-bottom: 4px; }
.pf-tour .pf-tour-title { font-weight: 600; flex: 1; }
.pf-tour .pf-tour-count { color: var(--pf-dim); font-size: 11px; }
.pf-tour .pf-tour-close { cursor: pointer; color: var(--pf-dim); background: none; border: 0;
  font: 14px var(--pf-font); padding: 0 2px; }
.pf-tour .pf-tour-close:hover { color: var(--pf-text); }
.pf-tour .pf-tour-body { margin-bottom: 10px; }
.pf-tour .pf-tour-nav { display: flex; gap: 6px; justify-content: flex-end; }
.pf-tour .pf-tour-nav button { background: var(--pf-input); color: var(--pf-text);
  border: 1px solid var(--pf-border-strong); border-radius: 6px; padding: 4px 12px;
  cursor: pointer; font: 12px var(--pf-font); }
.pf-tour .pf-tour-nav button:hover { background: var(--pf-hover); }
.pf-tour .pf-tour-nav button:disabled { color: var(--pf-faint); cursor: default; background: var(--pf-input); }
`;

function ensureStyle(): void {
  if (document.getElementById("pf-tour-style")) return;
  const el = document.createElement("style");
  el.id = "pf-tour-style";
  el.textContent = CSS;
  document.head.appendChild(el);
}

export function createTour(steps: TourStep[]): Tour {
  ensureStyle();

  const card = document.createElement("div");
  card.className = "pf-tour";
  const head = document.createElement("div");
  head.className = "pf-tour-head";
  const title = document.createElement("span");
  title.className = "pf-tour-title";
  const count = document.createElement("span");
  count.className = "pf-tour-count";
  const close = document.createElement("button");
  close.className = "pf-tour-close";
  close.textContent = "✕";
  close.title = "End walkthrough (Esc)";
  head.append(title, count, close);
  const body = document.createElement("div");
  body.className = "pf-tour-body";
  const nav = document.createElement("div");
  nav.className = "pf-tour-nav";
  const back = document.createElement("button");
  back.className = "pf-tour-back";
  back.textContent = "Back";
  const next = document.createElement("button");
  next.className = "pf-tour-next";
  nav.append(back, next);
  card.append(head, body, nav);
  document.body.appendChild(card);

  let i = -1;                                    // -1 = not running
  const stop = () => { i = -1; card.classList.remove("pf-open"); };
  const show = (n: number) => {
    i = n;
    const s = steps[n];
    title.textContent = s.title;
    count.textContent = `${n + 1} / ${steps.length}`;
    body.textContent = s.body;
    back.disabled = n === 0;
    next.textContent = n === steps.length - 1 ? "Finish" : "Next";
    card.classList.add("pf-open");
    try { void s.run?.(); } catch { /* keep touring */ }
  };

  back.addEventListener("click", () => { if (i > 0) show(i - 1); });
  next.addEventListener("click", () => { i < steps.length - 1 ? show(i + 1) : stop(); });
  close.addEventListener("click", stop);
  const onKey = (ev: KeyboardEvent) => { if (ev.key === "Escape" && i >= 0) stop(); };
  window.addEventListener("keydown", onKey);

  return {
    start: () => show(0),
    destroy() {
      window.removeEventListener("keydown", onKey);
      card.remove();
    },
  };
}
