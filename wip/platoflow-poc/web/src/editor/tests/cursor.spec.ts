// @vitest-environment jsdom
// Resize-affordance mouse cursor (index.ts): hovering a card's right band /
// bottom band / corner grip sets the canvas cursor per axis (ew/ns/nwse), the
// interior clears it, a socket wins its overlap with the right band (the wire
// gesture would claim that press), and pressing on a handle PINS the cursor
// via a documentElement class (T19 splitter pattern; index.html owns the CSS)
// until pointerup. Same jsdom mount recipe as editor-dom.spec.ts; coordinates
// come from the shared nodeLayout — never magic numbers.
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import { v } from "gratify";
import type { GraphDoc } from "../../contracts";
import { KINDS, kindInfo } from "../../kinds";
import { createEditor, type EditorApp } from "../index";
import {
  nodeLayout, RESIZE_BAND, resizeGripRect, socketPos, wiredInputsOf,
} from "../geom";
import { rect } from "gratify";

const DOC: GraphDoc = {
  name: "cursor-fixture",
  nodes: [
    // body-less kind: horizontal resize only
    { id: "type1", kind: "select.byType", params: { type: "IfcWall" }, x: 60, y: 90 },
    // the user's named case: body-carrying aggregate → vertical resize too
    { id: "agg1", kind: "table.aggregate", params: {}, x: 420, y: 90 },
  ],
  edges: [],
  display: null,
};

// ── jsdom shims (same recipe as editor-dom.spec.ts) ──────────────────────────
const ctxStub = new Proxy({} as Record<PropertyKey, unknown>, {
  get: (t, prop) => {
    if (prop === "measureText") return () => ({ width: 10 });
    if (prop === "canvas") return undefined;
    return (t[prop] ??= () => undefined);
  },
  set: () => true,
}) as unknown as CanvasRenderingContext2D;

let canvas: HTMLCanvasElement;
let app: EditorApp;

const layoutOf = (id: string) => {
  const n = app.getDoc().nodes.find((x) => x.id === id)!;
  const info = kindInfo(n.kind)!;
  const l = nodeLayout(info,
    { params: n.params, wiredInputs: wiredInputsOf(app.getDoc().edges, n.id, info), w: n.w, bh: n.bh },
    { helpOpen: false, zoom: 1 });
  return { n, info, l };
};

const pointer = (target: EventTarget, type: string, x = 0, y = 0) =>
  target.dispatchEvent(new MouseEvent(type, { clientX: x, clientY: y, bubbles: true }));

const hover = (p: { x: number; y: number }) => pointer(canvas, "pointermove", p.x, p.y);

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

describe("resize-affordance cursor (index.ts pointermove)", () => {
  it("right band → ew-resize; interior clears it", () => {
    const { n, l } = layoutOf("type1");
    // mid-height, clear of sockets (socket rows sit near the top)
    hover(v(n.x + l.w - RESIZE_BAND / 2, n.y + l.h - 30));
    expect(canvas.style.cursor).toBe("ew-resize");
    hover(v(n.x + l.w / 2, n.y + l.h / 2));
    expect(canvas.style.cursor).toBe("");
  });

  it("bottom band on the aggregate (body) node → ns-resize; body-less kind → none", () => {
    const agg = layoutOf("agg1");
    hover(v(agg.n.x + agg.l.w / 2, agg.n.y + agg.l.h - RESIZE_BAND / 2));
    expect(canvas.style.cursor).toBe("ns-resize");
    const plain = layoutOf("type1");
    hover(v(plain.n.x + plain.l.w / 2, plain.n.y + plain.l.h - RESIZE_BAND / 2));
    expect(canvas.style.cursor).toBe("");
  });

  it("corner grip → nwse-resize (both kinds)", () => {
    for (const id of ["type1", "agg1"]) {
      const { n, l } = layoutOf(id);
      hover(resizeGripRect(l, n.x, n.y).center);
      expect(canvas.style.cursor, id).toBe("nwse-resize");
    }
  });

  it("a socket wins its overlap with the right band (wire gesture claims first)", () => {
    const { n, l } = layoutOf("type1");
    const out = socketPos(l, rect(n.x, n.y, l.w, l.h), "out", 0);
    hover(out);                                    // dead on the out socket
    expect(canvas.style.cursor).toBe("");
  });

  it("pressing a handle pins the cursor class on <html> until pointerup", () => {
    const { n, l } = layoutOf("agg1");
    const html = document.documentElement;
    pointer(canvas, "pointerdown", n.x + l.w / 2, n.y + l.h - RESIZE_BAND / 2);
    expect(html.classList.contains("pf-drag-ns")).toBe(true);
    pointer(window, "pointerup");
    expect(html.classList.contains("pf-drag-ns")).toBe(false);
    // per-axis classes: right band pins ew, grip pins nwse
    pointer(canvas, "pointerdown", n.x + l.w - RESIZE_BAND / 2, n.y + l.h - 30);
    expect(html.classList.contains("pf-drag-ew")).toBe(true);
    pointer(window, "pointerup");
    const g = resizeGripRect(l, n.x, n.y).center;
    pointer(canvas, "pointerdown", g.x, g.y);
    expect(html.classList.contains("pf-drag-nwse")).toBe(true);
    pointer(window, "pointerup");
    expect(html.className).toBe("");
  });

  it("a press on the card interior pins nothing", () => {
    const { n, l } = layoutOf("agg1");
    pointer(canvas, "pointerdown", n.x + l.w / 2, n.y + l.h / 2);
    expect(document.documentElement.className).toBe("");
    pointer(window, "pointerup");
  });
});
