// T7: pure chart-body geometry goldens — the "chart data shape" half of the
// retired edgate checks (flow vitest owns the payload; edgate-smoke keeps one
// pixel check). barRects/bodyRect/barAt are pure geom functions.
import { describe, expect, it } from "vitest";
import { rect } from "gratify";
import type { NodeStatus } from "../../contracts";
import { kindInfo } from "../../kinds";
import { barAt, barRects, bodyRect, nodeLayout, type ChartData } from "../geom";

const CHART = {
  labels: ["Walls", "Doors", "Windows", "Slabs", "Beams", "Roofs"],
  values: [412, 128, 96, 260, 44, 180],
  title: "carbon by category",
} satisfies ChartData & NonNullable<NodeStatus["chart"]>;
const STATUS: NodeStatus = { state: "ok", summary: "6 bars", chart: CHART };

const layout = () => {
  const info = kindInfo("chart.bar")!;
  const l = nodeLayout(info, { params: {}, wiredInputs: new Set() },
    { helpOpen: false, zoom: 1 }, STATUS);
  return { l, card: rect(600, 360, l.w, l.h) };
};

describe("chart.bar body geometry (pure)", () => {
  it("barRects: one bar per value, all inside the body, golden", () => {
    const { l, card } = layout();
    const area = bodyRect(l, card);
    const bars = barRects(l, card, CHART);
    expect(bars.length).toBe(CHART.values.length);
    for (const b of bars) {
      expect(b.x).toBeGreaterThanOrEqual(area.x);
      expect(b.right).toBeLessThanOrEqual(area.right + 0.001);
      expect(b.y).toBeGreaterThanOrEqual(area.y);
      expect(b.bottom).toBeLessThanOrEqual(area.bottom + 0.001);
    }
    expect(bars.map((b) => ({ x: +b.x.toFixed(2), y: +b.y.toFixed(2), w: +b.w.toFixed(2), h: +b.h.toFixed(2) })))
      .toMatchSnapshot();
  });

  it("bar heights are proportional to values (max value = tallest bar)", () => {
    const { l, card } = layout();
    const bars = barRects(l, card, CHART);
    const hs = bars.map((b) => b.h);
    expect(hs[0]).toBe(Math.max(...hs));               // Walls: 412
    expect(hs[4]).toBe(Math.min(...hs));               // Beams: 44
    expect(hs[1] / hs[0]).toBeCloseTo(128 / 412, 1);
  });

  it("barAt round-trips every bar center and misses between bars", () => {
    const { l, card } = layout();
    const bars = barRects(l, card, CHART);
    bars.forEach((b, i) => expect(barAt(l, card.x, card.y, CHART, b.center)).toBe(i));
    const gap = { x: (bars[0].right + bars[1].x) / 2, y: bars[0].center.y };
    expect(barAt(l, card.x, card.y, CHART, gap)).toBeNull();
  });

  it("no chart status → no body bars; bodyHeight kinds still reserve the body", () => {
    const info = kindInfo("chart.bar")!;
    const l = nodeLayout(info, { params: {}, wiredInputs: new Set() }, { helpOpen: false, zoom: 1 });
    expect(l.bodyH).toBeGreaterThan(0);                // reserved by the kind
    expect(barRects(l, rect(0, 0, l.w, l.h), { labels: [], values: [] })).toEqual([]);
  });
});
