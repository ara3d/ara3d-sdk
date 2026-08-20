// @vitest-environment jsdom
// T14: the param popover and the T8 picker ride a gratify `island()` — an
// invisible zero-size part (index.ts's EditorIsland) whose facet reports the
// edited row's WORLD rect every frame; the runtime pins the DOM wrapper over
// it with a translate+scale. These specs drive the REAL createEditor and the
// runtime's deterministic `gratifyStep` (the debug hook attach() installs) to
// prove the glue: organs mount into the island wrapper, the wrapper's
// transform is exactly islandCss(rowRect, viewport), and pan/zoom RETARGET the
// transform instead of closing the editor. editor-dom.spec owns the
// listener-level close rules; this file owns the geometry seam.
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import { rect, type Runtime } from "gratify";
import type { GraphDoc } from "../../contracts";
import { KINDS, kindInfo } from "../../kinds";
import type { EditorDoc, EditorIntent } from "../doc";
import { createEditor, type EditorApp } from "../index";
import { nodeLayout, paramRowRect, wiredInputsOf } from "../geom";

const DOC: GraphDoc = {
  name: "island-fixture",
  nodes: [
    { id: "load1", kind: "load.model", params: { model: "duplex" }, x: 60, y: 90 },
    { id: "csv1", kind: "data.csv", params: { url: "carbon.csv" }, x: 60, y: 260 },
  ],
  edges: [],
  display: null,
};

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

type W = { gratify: Runtime<EditorDoc, EditorIntent>; gratifyStep: (n?: number) => void };
const rt = () => (window as unknown as W).gratify;
const step = (n = 1) => (window as unknown as W).gratifyStep(n);

const popover = () => document.querySelector<HTMLElement>(".pf-popover")!;
const picker = () => document.querySelector<HTMLElement>(".pf-picker")!;

/** World rect of a param row via the same shared layout index.ts uses. */
const rowRect = (id: string, name: string) => {
  const n = app.getDoc().nodes.find((x) => x.id === id)!;
  const info = kindInfo(n.kind)!;
  const l = nodeLayout(info,
    { params: n.params, wiredInputs: wiredInputsOf(app.getDoc().edges, n.id, info) },
    { helpOpen: false, zoom: 1 });
  return paramRowRect(l, rect(n.x, n.y, l.w, l.h), l.paramNames.indexOf(name));
};

/** What islandCss must produce for a world rect under the live viewport. */
const expectedTransform = (r: { x: number; y: number }) => {
  const { pan, zoom } = rt().viewport;
  return `translate(${r.x * zoom + pan.x}px, ${r.y * zoom + pan.y}px) scale(${zoom})`;
};

const pointer = (target: EventTarget, type: string, x = 0, y = 0) =>
  target.dispatchEvent(new MouseEvent(type, { clientX: x, clientY: y, bubbles: true }));

const clickRow = (id: string, name: string) => {
  const c = rowRect(id, name).center;
  pointer(canvas, "pointerdown", c.x, c.y);
  pointer(canvas, "pointerup", c.x, c.y);
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

describe("editor island (T14): organs glued to the row's world rect", () => {
  it("the popover mounts into the island wrapper, pinned over its row", () => {
    clickRow("csv1", "url");
    expect(popover().style.display).toBe("block");
    step(2);                                     // islands sync on ticks
    const wrap = popover().parentElement!;
    expect(wrap).not.toBe(document.body);        // reparented off the body…
    expect(wrap.style.transform).toBe(expectedTransform(rowRect("csv1", "url")));
    // …into the runtime's island layer (a child of the canvas host)
    expect(wrap.parentElement?.parentElement).toBe(canvas.parentElement);
  });

  it("pan retargets the transform — the editor tracks, it does not close", () => {
    clickRow("csv1", "url");
    step(2);
    const wrap = popover().parentElement!;
    const before = wrap.style.transform;
    rt().viewport.pan = { x: -40, y: 25 };       // deterministic pan (no springs)
    step(1);
    expect(popover().style.display).toBe("block");
    expect(wrap.style.transform).not.toBe(before);
    expect(wrap.style.transform).toBe(expectedTransform(rowRect("csv1", "url")));
  });

  it("zoom scales the island (world-sized DOM, scaled by the camera)", () => {
    clickRow("csv1", "url");
    step(2);
    rt().viewport.zoom = 0.8;                    // above the chip threshold
    step(1);
    const wrap = popover().parentElement!;
    expect(wrap.style.transform).toContain("scale(0.8)");
    expect(wrap.style.transform).toBe(expectedTransform(rowRect("csv1", "url")));
    expect(popover().style.display).toBe("block");
  });

  it("a node MOVE carries its open editor along (facet reads the live doc)", () => {
    clickRow("csv1", "url");
    step(2);
    const wrap = popover().parentElement!;
    const before = wrap.style.transform;
    app.dispatch({ t: "move", node: "csv1", x: 300, y: 400 });
    // external dispatch closes the editor (unchanged rule) — reopen and check
    clickRow("csv1", "url");
    step(2);
    expect(wrap.style.transform).not.toBe(before);
    expect(wrap.style.transform).toBe(expectedTransform(rowRect("csv1", "url")));
  });

  it("the picker rides the same island (modelPick row)", () => {
    clickRow("load1", "model");
    expect(picker().style.display).toBe("block");
    step(2);
    const wrap = picker().parentElement!;
    expect(wrap.style.transform).toBe(expectedTransform(rowRect("load1", "model")));
  });

  it("closing hides the organ; the wrapper stays mounted for the next session", () => {
    clickRow("csv1", "url");
    step(2);
    const wrap = popover().parentElement!;
    pointer(canvas, "pointerdown", 900, 600);    // clean empty-canvas click…
    pointer(canvas, "pointerup", 900, 600);      // …= click-away
    expect(popover().style.display).toBe("none");
    step(2);
    expect(wrap.isConnected).toBe(true);         // island never detaches (focus race)
    clickRow("load1", "model");
    step(2);
    expect(picker().parentElement).toBe(wrap);   // same wrapper serves the picker
  });
});
