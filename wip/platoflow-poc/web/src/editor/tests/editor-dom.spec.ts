// @vitest-environment jsdom
// T7 replacement for the retired edgate/intgate close-rule checks that live in
// index.ts's listener wiring (params.spec covers the popover organ itself):
// Esc closes, click-away closes, external dispatch closes, selection change
// closes, and the Backspace/typing guard does not delete the node under edit.
//
// The REAL createEditor is mounted: jsdom supplies the DOM, a Proxy stands in
// for the 2d canvas context (Gratify's CanvasPainter only ever draws into it),
// and pointer events are MouseEvents with pointer type names — listener
// dispatch is by type string, so the real handlers run. What jsdom cannot
// prove — pixels and the DOM↔canvas coordinate seam — is edgate-smoke's job.
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import { rect } from "gratify";
import type { GraphDoc } from "../../contracts";
import { KINDS, kindInfo } from "../../kinds";
import { createEditor, type EditorApp } from "../index";
import { nodeLayout, paramRowRect, wiredInputsOf } from "../geom";

const DOC: GraphDoc = {
  name: "editor-dom-fixture",
  nodes: [
    { id: "load1", kind: "load.model", params: { model: "duplex" }, x: 60, y: 90 },
    // string param → "row" widget → the classic popover (modelPick routes to
    // the T8 picker now, so close-rule specs anchor on a plain string param)
    { id: "csv1", kind: "data.csv", params: { url: "carbon.csv" }, x: 60, y: 260 },
    { id: "type1", kind: "select.byType", params: { type: "IfcWall" }, x: 420, y: 90 },
  ],
  edges: [],
  display: null,
};

// ── jsdom shims (2d context, pointer capture, ResizeObserver, rAF) ───────────
const ctxStub = new Proxy({} as Record<PropertyKey, unknown>, {
  get: (t, prop) => {
    if (prop === "measureText") return () => ({ width: 10 });
    if (prop === "canvas") return undefined;
    return (t[prop] ??= () => undefined);
  },
  set: () => true,                                 // font/fillStyle/… assignments
}) as unknown as CanvasRenderingContext2D;

let canvas: HTMLCanvasElement;
let app: EditorApp;

const popover = () => document.querySelector<HTMLElement>(".pf-popover")!;

/** Canvas-coordinate center of a param row, via the shared layout (§4.4). */
const rowCenter = (id: string, name: string) => {
  const n = app.getDoc().nodes.find((x) => x.id === id)!;
  const info = kindInfo(n.kind)!;
  const l = nodeLayout(info,
    { params: n.params, wiredInputs: wiredInputsOf(app.getDoc().edges, n.id, info) },
    { helpOpen: false, zoom: 1 });
  const r = paramRowRect(l, rect(n.x, n.y, l.w, l.h), l.paramNames.indexOf(name));
  return r.center;                                 // canvas rect is at (0,0) in jsdom
};

const pointer = (target: EventTarget, type: string, x = 0, y = 0) =>
  target.dispatchEvent(new MouseEvent(type, { clientX: x, clientY: y, bubbles: true }));

/** A clean click through index.ts's pointerdown/up pair (no drag slop). */
const clickCanvas = (x: number, y: number) => {
  pointer(canvas, "pointerdown", x, y);
  pointer(canvas, "pointerup", x, y);
};

const key = (k: string, target: EventTarget = window) =>
  target.dispatchEvent(new KeyboardEvent("keydown", { key: k, bubbles: true }));

const openPopover = (id: string, name: string) => {
  const c = rowCenter(id, name);
  clickCanvas(c.x, c.y);
  expect(popover().style.display).toBe("block");
};

beforeEach(() => {
  (globalThis as Record<string, unknown>).ResizeObserver ??=
    class { observe() {} unobserve() {} disconnect() {} };
  (globalThis as Record<string, unknown>).requestAnimationFrame ??=
    ((cb: FrameRequestCallback) => setTimeout(() => cb(performance.now()), 16));
  HTMLCanvasElement.prototype.getContext =
    (() => ctxStub) as unknown as typeof HTMLCanvasElement.prototype.getContext;
  HTMLCanvasElement.prototype.setPointerCapture ??= () => undefined;
  HTMLCanvasElement.prototype.releasePointerCapture ??= () => undefined;

  const host = document.createElement("div");
  canvas = document.createElement("canvas");
  host.appendChild(canvas);
  document.body.appendChild(host);
  app = createEditor(canvas, KINDS, {});
  app.dispatch({ t: "load", doc: DOC });
});

afterEach(() => { app.destroy(); document.body.replaceChildren(); });

describe("index.ts close rules (retired browser checks)", () => {
  it("clicking a param row opens the popover for that param", () => {
    openPopover("csv1", "url");
    expect(popover().querySelector("label")?.textContent?.trim()).toBe("url");
  });

  it("Esc closes the popover (and does not reach the graph)", () => {
    openPopover("csv1", "url");
    key("Escape");
    expect(popover().style.display).toBe("none");
    expect(app.getDoc().nodes.length).toBe(3);
  });

  it("click-away on the document closes the popover", () => {
    openPopover("csv1", "url");
    pointer(document.body, "pointerdown", 900, 700);
    expect(popover().style.display).toBe("none");
  });

  it("a click inside the popover does NOT close it", () => {
    openPopover("csv1", "url");
    pointer(popover().querySelector("input")!, "pointerdown");
    expect(popover().style.display).toBe("block");
  });

  it("Backspace while a popover field has focus does not delete the node", () => {
    openPopover("csv1", "url");
    const field = popover().querySelector<HTMLInputElement>("input")!;
    field.focus();
    key("Backspace", field);
    expect(app.getDoc().nodes.find((n) => n.id === "csv1")).toBeTruthy();
  });

  it("external dispatch (MCP / demo load) closes the popover", () => {
    openPopover("csv1", "url");
    app.dispatch({ t: "setParam", node: "type1", name: "type", value: "IfcDoor" });
    expect(popover().style.display).toBe("none");
  });

  // ── T14 flips: the editor rides an island glued to its row's WORLD rect,
  // so pan and zoom no longer close it (close-on-pan/wheel were workarounds
  // for fixed-position DOM). A clean empty-canvas CLICK is still click-away.

  it("a canvas pan (drag on empty space) leaves the popover open", () => {
    openPopover("csv1", "url");
    pointer(canvas, "pointerdown", 900, 600);
    pointer(canvas, "pointermove", 950, 660);    // >4px slop = a drag, not a click
    pointer(canvas, "pointerup", 950, 660);
    expect(popover().style.display).toBe("block");
  });

  it("wheel-zoom over the canvas leaves the popover open", () => {
    openPopover("csv1", "url");
    canvas.dispatchEvent(new WheelEvent("wheel", { deltaY: 40, bubbles: true }));
    expect(popover().style.display).toBe("block");
  });

  it("a clean click on empty canvas closes it (click-away survives the flip)", () => {
    openPopover("csv1", "url");
    clickCanvas(900, 600);
    expect(popover().style.display).toBe("none");
  });

  it("dismissal click-away does not deselect the edited node (T2 rule)", () => {
    let sel: string | null = "unset";
    app.destroy();
    app = createEditor(canvas, KINDS, { onSelect: (id) => { sel = id; } });
    app.dispatch({ t: "load", doc: DOC });
    // Gratify's press hit-tests INSTANCE rects, which only exist after layout
    // ticks (the popover's own hit-test reads the doc, so other specs skip
    // this). Settle springs deterministically, then click.
    (window as unknown as { gratifyStep(n?: number): void }).gratifyStep(200);
    openPopover("csv1", "url");                  // the row click selected csv1
    expect(sel).toBe("csv1");
    clickCanvas(900, 600);                       // dismisses, must NOT select-null
    expect(popover().style.display).toBe("none");
    expect(sel).toBe("csv1");
  });
});
