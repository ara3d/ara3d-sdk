// T1 in-node widgets: scrub / toggle / chips. Pure math first (scrubValue,
// chip geometry, the schema→widget map), then the REAL gesture pipeline on the
// headless drive — a horizontal drag on a number row scrubs the value and no
// longer moves the card (NOTES wave-5 finding 2). Replaces nothing retired
// (widgets are new behavior); the smoke gate's trusted-drag slot flips from
// card-move to scrub at the W1 join (TODO T1 marker in tools/edgate-smoke.mjs).
import { describe, expect, it } from "vitest";
import { rect, v } from "gratify";
import type { GraphDoc, ParamSchema } from "../../contracts";
import { paramRowRect, WIDGET_H, type WidgetKind } from "../geom";
import {
  CHIPS_MAX, chipAt, chipRects, chipsArea, drawPlainRow, scrubValue, SCRUB_PX_PER_STEP,
  toggleRect, widgetForSchema, WIDGETS, widgetImpl,
} from "../widgets";
import { createDrive } from "./drive";

type NumberSchema = Extract<ParamSchema, { kind: "number" }>;

const DOC: GraphDoc = {
  name: "widget-fixture",
  nodes: [
    { id: "cmap1", kind: "viz.colormap", params: { ramp: "viridis", auto: true, min: 0, max: 100 }, x: 100, y: 80 },
    { id: "type1", kind: "select.byType", params: { type: "IfcWall" }, x: 480, y: 80 },
  ],
  edges: [],
  display: null,
};

const fresh = () => {
  const d = createDrive();
  d.load(DOC);
  d.settle();
  return d;
};

const param = (d: ReturnType<typeof fresh>, id: string, name: string) =>
  d.graph().nodes.find((n) => n.id === id)!.params[name];

describe("widget mapping + scrub math (pure)", () => {
  it("widgetForSchema: number→scrub, boolean→toggle, small enum→chips, rest→row", () => {
    expect(widgetForSchema({ kind: "number" })).toBe("scrub");
    expect(widgetForSchema({ kind: "boolean" })).toBe("toggle");
    expect(widgetForSchema({ kind: "enum", options: ["a", "b", "c"] })).toBe("chips");
    expect(widgetForSchema({ kind: "enum", options: Array.from({ length: CHIPS_MAX + 1 }, (_, i) => `o${i}`) })).toBe("row");
    expect(widgetForSchema({ kind: "string" })).toBe("row");
    expect(widgetForSchema({ kind: "text" })).toBe("row");
    expect(widgetForSchema({ kind: "dynamicEnum", source: "types" })).toBe("row");
    expect(widgetForSchema({ kind: "modelPick" })).toBe("row");
  });

  it("scrubValue quantizes to the step at SCRUB_PX_PER_STEP px per quantum", () => {
    const s: NumberSchema = { kind: "number" };                      // step defaults to 1
    expect(scrubValue(s, 10, 10 * SCRUB_PX_PER_STEP, false)).toBe(20);
    expect(scrubValue(s, 10, -3 * SCRUB_PX_PER_STEP, false)).toBe(7);
    expect(scrubValue(s, 10, 1, false)).toBe(11);                    // rounds to nearest step
  });

  it("scrubValue honors schema.step and Shift-fine (×0.1) without float dust", () => {
    const s: NumberSchema = { kind: "number", step: 0.5 };
    expect(scrubValue(s, 1, 4 * SCRUB_PX_PER_STEP, false)).toBe(3);
    expect(scrubValue(s, 1, 2 * SCRUB_PX_PER_STEP, true)).toBe(1.1); // fine: 0.05 quanta
    expect(String(scrubValue(s, 0.1, 2 * SCRUB_PX_PER_STEP, true))).not.toMatch(/000000|999999/);
  });

  it("scrubValue clamps to min/max", () => {
    const s: NumberSchema = { kind: "number", min: 0, max: 10 };
    expect(scrubValue(s, 8, 100 * SCRUB_PX_PER_STEP, false)).toBe(10);
    expect(scrubValue(s, 2, -100 * SCRUB_PX_PER_STEP, false)).toBe(0);
  });

  it("chip geometry round-trips: each chip center hits its own index", () => {
    const row = rect(10, 20, 160, 16);
    const chips = chipRects(row, 3);
    chips.forEach((c, i) => expect(chipAt(row, 3, c.center)).toBe(i));
    // the label zone (left of chipsArea) misses
    const a = chipsArea(row);
    expect(chipAt(row, 3, v(a.x - 4, row.center.y))).toBeNull();
    // the toggle glyph stays inside the row
    const t = toggleRect(row);
    expect(t.x).toBeGreaterThan(row.x);
    expect(t.right).toBeLessThanOrEqual(row.right);
  });
});

describe("T15 widget registry (re-keys the T1 switch)", () => {
  it("every WidgetKind has a registry entry (keys match WIDGET_H exactly)", () => {
    expect(Object.keys(WIDGETS).sort()).toEqual(Object.keys(WIDGET_H).sort());
  });

  it("unknown kind strings fall back to the 'row' entry (plain-row draw)", () => {
    expect(widgetImpl("no-such-widget")).toBe(WIDGETS.row);
    expect(WIDGETS.row.draw).toBe(drawPlainRow);
  });

  it("only scrub/toggle/chips claim presses; passive kinds decline by absence", () => {
    const interactive: WidgetKind[] = ["scrub", "toggle", "chips"];
    for (const k of interactive) expect(WIDGETS[k].begin).toBeTypeOf("function");
    const passive: WidgetKind[] = ["hidden", "row", "picker", "textPreview", "gradient"];
    for (const k of passive) expect(WIDGETS[k].begin).toBeUndefined();
  });
});

describe("widgets through the real gesture pipeline (headless drive)", () => {
  it("scrub: horizontal drag on a number row edits the value, quantized", () => {
    const d = fresh();
    d.scrub("cmap1", "min", 20 * SCRUB_PX_PER_STEP);
    expect(param(d, "cmap1", "min")).toBe(20);
  });

  it("scrub does NOT move the card (the widget gesture claims the press)", () => {
    const d = fresh();
    const before = d.graph().nodes.find((n) => n.id === "cmap1")!;
    d.scrub("cmap1", "min", 30);
    const after = d.graph().nodes.find((n) => n.id === "cmap1")!;
    expect(v(after.x, after.y)).toEqual(v(before.x, before.y));
  });

  it("Shift-scrub is fine (×0.1 of the step)", () => {
    const d = fresh();
    d.scrub("cmap1", "min", 20 * SCRUB_PX_PER_STEP, { shift: true });
    expect(param(d, "cmap1", "min")).toBe(2);
  });

  it("a whole scrub is ONE undo step (W0 coalescing key)", () => {
    const d = fresh();
    const before = d.doc().past.length;
    d.scrub("cmap1", "min", 40 * SCRUB_PX_PER_STEP, { steps: 8 });
    expect(param(d, "cmap1", "min")).toBe(40);
    expect(d.doc().past.length).toBe(before + 1);
    d.dispatch({ k: "undo" });
    expect(param(d, "cmap1", "min")).toBe(0);
  });

  it("toggle: a click on a boolean row flips it, no popover involved", () => {
    const d = fresh();
    d.click(d.paramCenter("cmap1", "auto"));
    expect(param(d, "cmap1", "auto")).toBe(false);
    d.settle();
    d.click(d.paramCenter("cmap1", "auto"));
    expect(param(d, "cmap1", "auto")).toBe(true);
  });

  it("chips: clicking a segment sets that option", () => {
    const d = fresh();
    const { l, origin } = d.layoutOf("cmap1");
    const i = l.paramNames.indexOf("ramp");
    const row = paramRowRect(l, rect(origin.x, origin.y, l.w, l.h), i);
    const chips = chipRects(row, 3);                // ["viridis","heat","greenred"]
    d.click(chips[1].center);
    expect(param(d, "cmap1", "ramp")).toBe("heat");
    d.settle();
    d.click(chips[2].center);
    expect(param(d, "cmap1", "ramp")).toBe("greenred");
  });

  it("plain rows still fall through to the card-move gesture (dynamicEnum)", () => {
    const d = fresh();
    const a = d.paramCenter("type1", "type");
    d.drag(a, v(a.x + 60, a.y));
    const n = d.graph().nodes.find((x) => x.id === "type1")!;
    expect(n.x).toBeGreaterThan(480 + 40);          // the card moved
    expect(param(d, "type1", "type")).toBe("IfcWall");
  });
});
