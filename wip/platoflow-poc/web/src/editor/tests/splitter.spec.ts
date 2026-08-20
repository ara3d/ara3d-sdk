// @vitest-environment jsdom
// T19 — pane splitter: pure drag/clamp math, pf_split persistence round-trip,
// and DOM-level drag / double-click-reset / cursor-class behavior against the
// real installSplitters wiring. Pixels (the canvases actually repainting) are
// intgate-smoke's job.
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import {
  DEFAULT_SPLIT, HANDLE_PX, MIN_BOTTOM, MIN_GRAPH, MIN_RIGHT, SPLIT_KEY,
  clampBottom, clampRight, dragSize, installSplitters, loadSplit, saveSplit,
  type Splitters,
} from "../../panes/splitter";

// ── pure drag math ───────────────────────────────────────────────────────────

describe("drag math", () => {
  it("dragging the handle right shrinks the right pane 1:1", () => {
    expect(dragSize(480, 700, 750)).toBe(430);
  });
  it("dragging left grows it", () => {
    expect(dragSize(480, 700, 620)).toBe(560);
  });
  it("clampRight floors at MIN_RIGHT", () => {
    expect(clampRight(10, 1200)).toBe(MIN_RIGHT);
  });
  it("clampRight leaves MIN_GRAPH + the handle for the graph", () => {
    expect(clampRight(5000, 1200)).toBe(1200 - HANDLE_PX - MIN_GRAPH);
  });
  it("in-range sizes pass through untouched", () => {
    expect(clampRight(480, 1200)).toBe(480);
    expect(clampBottom(220, 800)).toBe(220);
  });
  it("unknown container size (jsdom clientWidth=0) does not clamp", () => {
    expect(clampRight(480, 0)).toBe(480);
  });
  it("clampBottom floors at MIN_BOTTOM", () => {
    expect(clampBottom(-50, 800)).toBe(MIN_BOTTOM);
  });
});

// ── persistence ──────────────────────────────────────────────────────────────

const fakeStorage = () => {
  const m = new Map<string, string>();
  return {
    getItem: (k: string) => m.get(k) ?? null,
    setItem: (k: string, v: string) => void m.set(k, v),
    dump: () => m.get(SPLIT_KEY),
  };
};

describe("pf_split persistence", () => {
  it("round-trips a split", () => {
    const s = fakeStorage();
    saveSplit(s, { right: 555, bottom: 123 });
    expect(loadSplit(s)).toEqual({ right: 555, bottom: 123 });
  });
  it("empty storage → {} (defaults win)", () => {
    expect(loadSplit(fakeStorage())).toEqual({});
  });
  it("garbage JSON / wrong shapes degrade field-by-field", () => {
    const s = fakeStorage();
    s.setItem(SPLIT_KEY, "not json");
    expect(loadSplit(s)).toEqual({});
    s.setItem(SPLIT_KEY, JSON.stringify({ right: "wide", bottom: 150 }));
    expect(loadSplit(s)).toEqual({ bottom: 150 });
    s.setItem(SPLIT_KEY, JSON.stringify({ right: -20, bottom: NaN }));
    expect(loadSplit(s)).toEqual({});
  });
});

// ── DOM wiring ───────────────────────────────────────────────────────────────

let app: HTMLElement;
let vsplit: HTMLElement;
let hsplit: HTMLElement;
let split: Splitters;
let storage: ReturnType<typeof fakeStorage>;

const pointer = (target: EventTarget, type: string, x = 0, y = 0) =>
  target.dispatchEvent(new MouseEvent(type, { clientX: x, clientY: y, bubbles: true }));

const rightVar = () => app.style.getPropertyValue("--pf-right");
const bottomVar = () => app.style.getPropertyValue("--pf-bottom");

beforeEach(() => {
  (HTMLElement.prototype as any).setPointerCapture ??= () => undefined;
  (HTMLElement.prototype as any).releasePointerCapture ??= () => undefined;
  app = document.createElement("div");
  app.id = "app";
  vsplit = document.createElement("div");
  vsplit.id = "vsplit";
  hsplit = document.createElement("div");
  hsplit.id = "hsplit";
  app.append(vsplit, hsplit);
  document.body.appendChild(app);
  storage = fakeStorage();
});

afterEach(() => {
  split?.destroy();
  document.body.replaceChildren();
  document.documentElement.className = "";
});

describe("installSplitters", () => {
  it("boot applies defaults as CSS variables", () => {
    split = installSplitters(app, storage);
    expect(rightVar()).toBe(`${DEFAULT_SPLIT.right}px`);
    expect(bottomVar()).toBe(`${DEFAULT_SPLIT.bottom}px`);
  });

  it("boot restores a persisted split", () => {
    saveSplit(storage, { right: 600, bottom: 300 });
    split = installSplitters(app, storage);
    expect(rightVar()).toBe("600px");
    expect(bottomVar()).toBe("300px");
  });

  it("vertical drag moves --pf-right, toggles cursor class, persists on release", () => {
    split = installSplitters(app, storage);
    pointer(vsplit, "pointerdown", 700, 100);
    expect(document.documentElement.classList.contains("pf-drag-col")).toBe(true);
    expect(vsplit.classList.contains("drag")).toBe(true);

    pointer(vsplit, "pointermove", 650, 100);   // 50px left → right pane +50
    expect(rightVar()).toBe(`${DEFAULT_SPLIT.right + 50}px`);
    expect(storage.dump()).toBeUndefined();      // not saved mid-drag

    pointer(vsplit, "pointerup", 650, 100);
    expect(document.documentElement.classList.contains("pf-drag-col")).toBe(false);
    expect(vsplit.classList.contains("drag")).toBe(false);
    expect(loadSplit(storage).right).toBe(DEFAULT_SPLIT.right + 50);
  });

  it("horizontal drag moves --pf-bottom with row cursor class", () => {
    split = installSplitters(app, storage);
    pointer(hsplit, "pointerdown", 900, 500);
    expect(document.documentElement.classList.contains("pf-drag-row")).toBe(true);
    pointer(hsplit, "pointermove", 900, 460);   // 40px up → bottom pane +40
    expect(bottomVar()).toBe(`${DEFAULT_SPLIT.bottom + 40}px`);
    pointer(hsplit, "pointerup", 900, 460);
    expect(loadSplit(storage).bottom).toBe(DEFAULT_SPLIT.bottom + 40);
  });

  it("moves without a preceding pointerdown are ignored", () => {
    split = installSplitters(app, storage);
    pointer(vsplit, "pointermove", 300, 100);
    expect(rightVar()).toBe(`${DEFAULT_SPLIT.right}px`);
  });

  it("double-click resets that axis to the default and persists", () => {
    saveSplit(storage, { right: 700, bottom: 320 });
    split = installSplitters(app, storage);
    vsplit.dispatchEvent(new MouseEvent("dblclick", { bubbles: true }));
    expect(rightVar()).toBe(`${DEFAULT_SPLIT.right}px`);
    expect(bottomVar()).toBe("320px");           // other axis untouched
    expect(loadSplit(storage)).toEqual({ right: DEFAULT_SPLIT.right, bottom: 320 });
  });

  it("pointercancel ends the drag cleanly", () => {
    split = installSplitters(app, storage);
    pointer(vsplit, "pointerdown", 700, 100);
    vsplit.dispatchEvent(new Event("pointercancel", { bubbles: true }));
    expect(document.documentElement.classList.contains("pf-drag-col")).toBe(false);
    // a stray move after cancel does nothing
    pointer(vsplit, "pointermove", 200, 100);
    expect(rightVar()).toBe(`${DEFAULT_SPLIT.right}px`);
  });

  it("destroy removes listeners", () => {
    split = installSplitters(app, storage);
    split.destroy();
    pointer(vsplit, "pointerdown", 700, 100);
    expect(document.documentElement.classList.contains("pf-drag-col")).toBe(false);
  });
});
