// T19 — pane splitters. The shell (#app in index.html) is a CSS grid whose
// right-column width and bottom-row height are the CSS variables --pf-right /
// --pf-bottom. Two 6px handle strips (#vsplit, #hsplit) are real grid tracks;
// dragging one just rewrites the variable. Both canvases then resize on their
// own: Gratify has a ResizeObserver on the editor canvas, and ara3d-webgl's
// Viewport observes the viewer canvas's parent element — no imperative resize
// calls needed here.
//
// Pure drag/clamp/persistence functions are exported for the jsdom spec
// (editor/tests/splitter.spec.ts).

export interface SplitState { right: number; bottom: number; }

export const DEFAULT_SPLIT: SplitState = { right: 480, bottom: 220 };
/** Must match the 6px splitter tracks in index.html's grid-template. */
export const HANDLE_PX = 6;
export const MIN_GRAPH = 320;   // graph canvas never collapses
export const MIN_RIGHT = 240;   // viewer column stays usable
export const MIN_VIEWER = 120;  // viewer row above the grid
export const MIN_BOTTOM = 80;   // data grid row

export const SPLIT_KEY = "pf_split";

/** Handle sits left/above its pane, so dragging toward +x/+y shrinks it. */
export const dragSize = (startSize: number, startPos: number, curPos: number): number =>
  startSize - (curPos - startPos);

/** Clamp a pane size into [min, container - handle - otherMin]. A non-positive
 *  container (jsdom, display:none) means "unknown" — leave the size alone. */
export const clampPane = (px: number, container: number, min: number, otherMin: number): number => {
  if (container <= 0) return px;
  const max = Math.max(min, container - HANDLE_PX - otherMin);
  return Math.min(Math.max(px, min), max);
};

export const clampRight = (px: number, appWidth: number): number =>
  clampPane(px, appWidth, MIN_RIGHT, MIN_GRAPH);
export const clampBottom = (px: number, appHeight: number): number =>
  clampPane(px, appHeight, MIN_BOTTOM, MIN_VIEWER);

type StorageLike = Pick<Storage, "getItem" | "setItem">;

/** Restore a persisted split; anything malformed degrades to {} (defaults win). */
export const loadSplit = (storage: StorageLike): Partial<SplitState> => {
  try {
    const raw = storage.getItem(SPLIT_KEY);
    if (!raw) return {};
    const p = JSON.parse(raw) as Record<string, unknown>;
    const out: Partial<SplitState> = {};
    if (typeof p.right === "number" && Number.isFinite(p.right) && p.right > 0) out.right = p.right;
    if (typeof p.bottom === "number" && Number.isFinite(p.bottom) && p.bottom > 0) out.bottom = p.bottom;
    return out;
  } catch { return {}; }
};

export const saveSplit = (storage: StorageLike, s: SplitState): void => {
  try { storage.setItem(SPLIT_KEY, JSON.stringify(s)); } catch { /* private mode */ }
};

export interface Splitters {
  state: SplitState;
  /** Re-clamp + write both CSS variables (exposed for specs/console). */
  apply(): void;
  destroy(): void;
}

/**
 * Wires #vsplit / #hsplit inside `app`: pointer-capture drag, documentElement
 * cursor class while dragging (so the cursor never flickers off the strip),
 * double-click resets that axis to the default, and the split persists in
 * localStorage under "pf_split".
 */
export function installSplitters(app: HTMLElement, storage: StorageLike = localStorage): Splitters {
  const state: SplitState = { ...DEFAULT_SPLIT, ...loadSplit(storage) };

  const apply = () => {
    state.right = clampRight(state.right, app.clientWidth);
    state.bottom = clampBottom(state.bottom, app.clientHeight);
    app.style.setProperty("--pf-right", `${state.right}px`);
    app.style.setProperty("--pf-bottom", `${state.bottom}px`);
  };

  const cleanups: (() => void)[] = [];
  const on = <K extends string>(el: EventTarget, type: K, fn: (ev: any) => void) => {
    el.addEventListener(type, fn);
    cleanups.push(() => el.removeEventListener(type, fn));
  };

  const wire = (id: "vsplit" | "hsplit") => {
    const handle = app.querySelector<HTMLElement>(`#${id}`);
    if (!handle) return;
    const col = id === "vsplit";
    const dragClass = col ? "pf-drag-col" : "pf-drag-row";
    let drag: { pointerId: number; startPos: number; startSize: number } | null = null;

    on(handle, "pointerdown", (ev: PointerEvent) => {
      if (ev.button !== 0 && ev.button !== undefined) return;
      ev.preventDefault();
      handle.setPointerCapture?.(ev.pointerId);
      drag = {
        pointerId: ev.pointerId,
        startPos: col ? ev.clientX : ev.clientY,
        startSize: col ? state.right : state.bottom,
      };
      document.documentElement.classList.add(dragClass);
      handle.classList.add("drag");
    });

    on(handle, "pointermove", (ev: PointerEvent) => {
      if (!drag) return;
      const next = dragSize(drag.startSize, drag.startPos, col ? ev.clientX : ev.clientY);
      if (col) state.right = clampRight(next, app.clientWidth);
      else state.bottom = clampBottom(next, app.clientHeight);
      apply();
    });

    const end = () => {
      if (!drag) return;
      drag = null;
      document.documentElement.classList.remove(dragClass);
      handle.classList.remove("drag");
      saveSplit(storage, state);
    };
    on(handle, "pointerup", end);
    on(handle, "pointercancel", end);

    on(handle, "dblclick", () => {
      if (col) state.right = DEFAULT_SPLIT.right;
      else state.bottom = DEFAULT_SPLIT.bottom;
      apply();
      saveSplit(storage, state);
    });
  };

  wire("vsplit");
  wire("hsplit");
  on(window, "resize", apply);  // window shrink re-clamps so the graph keeps MIN_GRAPH
  apply();

  return { state, apply, destroy: () => cleanups.forEach((f) => f()) };
}
