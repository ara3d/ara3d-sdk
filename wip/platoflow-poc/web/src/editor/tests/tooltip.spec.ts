// @vitest-environment jsdom
// T7 replacement for the retired edgate hover checks: socket tooltip
// (`name · type — doc`), chart-bar tooltip (`label: value`), header tooltip.
// createNodeTip is a DOM organ over pure geom hit-testing, so jsdom + fake
// timers replaces the 800ms browser sleeps wholesale. Coordinates come from
// geom (socketPos/barRects) — never magic numbers (§4.4).
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { rect } from "gratify";
import type { GraphDoc, NodeStatus } from "../../contracts";
import { kindInfo } from "../../kinds";
import { createNodeTip, type NodeTip } from "../help";
import { barRects, nodeLayout, socketPos, wiredInputsOf } from "../geom";

const CHART: NodeStatus = {
  state: "ok",
  summary: "6 bars",
  chart: {
    labels: ["Walls", "Doors", "Windows", "Slabs", "Beams", "Roofs"],
    values: [412, 128, 96, 260, 44, 180],
    title: "carbon by category",
  },
};

const DOC: GraphDoc = {
  name: "tip-fixture",
  nodes: [
    { id: "load1", kind: "load.model", params: { model: "duplex" }, x: 40, y: 60 },
    { id: "chart1", kind: "chart.bar", params: {}, x: 400, y: 60 },
  ],
  edges: [],
  display: null,
};
const STATUS: Record<string, NodeStatus | undefined> = { chart1: CHART };

const layoutOf = (id: string) => {
  const n = DOC.nodes.find((x) => x.id === id)!;
  const info = kindInfo(n.kind)!;
  const l = nodeLayout(info,
    { params: n.params, wiredInputs: wiredInputsOf(DOC.edges, id, info) },
    { helpOpen: false, zoom: 1 }, STATUS[id]);
  return { n, info, l };
};

let canvas: HTMLCanvasElement;
let tip: NodeTip;
const tipEl = () => document.querySelector<HTMLElement>(".pf-tip")!;

/** Hover a world point (canvas rect is at 0,0 in jsdom; toWorld = identity). */
const hover = (x: number, y: number) =>
  canvas.dispatchEvent(new MouseEvent("mousemove", { clientX: x, clientY: y, bubbles: true }));

beforeEach(() => {
  vi.useFakeTimers();
  canvas = document.createElement("canvas");
  document.body.appendChild(canvas);
  tip = createNodeTip(canvas, {
    getDoc: () => DOC,
    kindInfo,
    getStatus: (id) => STATUS[id],
    helpOpen: () => false,
    toWorld: (p) => p,
  });
});

afterEach(() => { tip.destroy(); vi.useRealTimers(); document.body.replaceChildren(); });

describe("hover tooltip (retired edgate checks)", () => {
  it("socket hover shows `name · type — doc` after the 500ms dwell", () => {
    const { n, l } = layoutOf("load1");
    const p = socketPos(l, rect(n.x, n.y, l.w, l.h), "out", 0);
    hover(p.x, p.y);
    expect(tipEl().style.display).toBe("none");    // not yet — dwell first
    vi.advanceTimersByTime(600);
    expect(tipEl().style.display).toBe("block");
    expect(tipEl().textContent).toMatch(/^out · scene — /);
  });

  it("bar hover shows `label: value` from the chart payload", () => {
    const { n, l } = layoutOf("chart1");
    const bars = barRects(l, rect(n.x, n.y, l.w, l.h), CHART.chart!);
    hover(bars[0].center.x, bars[0].center.y);
    vi.advanceTimersByTime(600);
    expect(tipEl().textContent).toBe("Walls: 412");
    expect(tipEl().querySelector(".pf-tip-mono")).toBeTruthy();
  });

  it("every bar's center hits back to its own label (hit-test round-trip)", () => {
    const { n, l } = layoutOf("chart1");
    const bars = barRects(l, rect(n.x, n.y, l.w, l.h), CHART.chart!);
    expect(bars.length).toBe(6);
    bars.forEach((b, i) => {
      hover(b.center.x, b.center.y);
      vi.advanceTimersByTime(600);
      expect(tipEl().textContent).toBe(`${CHART.chart!.labels[i]}: ${CHART.chart!.values[i]}`);
    });
  });

  it("header hover shows label, kind and description", () => {
    const { n, info, l } = layoutOf("load1");
    hover(n.x + l.w / 2, n.y + 10);                // inside the header band
    vi.advanceTimersByTime(600);
    expect(tipEl().querySelector(".pf-tip-title")?.textContent).toContain(info.label);
    expect(tipEl().textContent).toContain(info.description.slice(0, 20));
  });

  it("pointerdown and mouseleave take the tooltip down", () => {
    const { n, l } = layoutOf("load1");
    const p = socketPos(l, rect(n.x, n.y, l.w, l.h), "out", 0);
    hover(p.x, p.y);
    vi.advanceTimersByTime(600);
    expect(tipEl().style.display).toBe("block");
    canvas.dispatchEvent(new MouseEvent("pointerdown", { bubbles: true }));
    expect(tipEl().style.display).toBe("none");
    hover(p.x, p.y);
    vi.advanceTimersByTime(600);
    canvas.dispatchEvent(new MouseEvent("mouseleave", { bubbles: true }));
    expect(tipEl().style.display).toBe("none");
  });

  it("empty canvas space shows nothing even after the dwell", () => {
    hover(5, 5);
    vi.advanceTimersByTime(1000);
    expect(tipEl().style.display).toBe("none");
  });
});
