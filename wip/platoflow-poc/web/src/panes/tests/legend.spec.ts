// @vitest-environment jsdom
// W9-D legend rework: categorical swatches (ViewValue.legend), numeric gradient
// from the EFFECTIVE domain (ViewValue.domain/.ramp — never the colormap node's
// configured min/max), null clears, and the legacy showLegend path unchanged.
import { beforeEach, describe, expect, it } from "vitest";
import type { ModelData, Panes, ViewValue } from "../../contracts";
import { createPanes, legendModel, rampCss, rgbCss } from "../index";

// The legend never touches ModelData; a cast keeps the fixture honest about that.
const model = { id: "mock", entityCount: 0 } as unknown as ModelData;

const view = (over: Partial<ViewValue>): ViewValue => ({
  model, entities: new Uint32Array(0), ghostOthers: true, ...over,
});

const RED: [number, number, number] = [1, 0, 0];
const GREEN: [number, number, number] = [0, 1, 0];
const BLUE: [number, number, number] = [0, 0, 1];

let legendEl: HTMLElement;
let panes: Panes;
const showView = (v: ViewValue | null) => panes.showViewLegend!(v);

beforeEach(() => {
  document.body.innerHTML = "";
  const grid = document.createElement("div");
  legendEl = document.createElement("div");
  document.body.append(grid, legendEl);
  panes = createPanes(grid, legendEl);
});

// jsdom normalizes css color serialization ("rgb(255,0,0)" → "rgb(255, 0, 0)");
// round-trip expectations through a style so both sides speak jsdom's dialect.
const cssNorm = (css: string) => {
  const el = document.createElement("i");
  el.style.backgroundColor = css;
  return el.style.backgroundColor;
};

const swatchRows = () => [...legendEl.querySelectorAll(".pf-legend-swatch")];
const rowLabel = (r: Element) => r.querySelector(".pf-legend-swatch-label")!.textContent;
const rowChipCss = (r: Element) =>
  (r.querySelector(".pf-legend-chip") as HTMLElement).style.background;
const titleText = () => legendEl.querySelector(".pf-legend-title")?.textContent;
const tickTexts = () =>
  [...legendEl.querySelectorAll(".pf-legend-ticks div")].map(d => d.textContent);

describe("showViewLegend — categorical swatches", () => {
  const v = view({
    label: "by Level",
    legend: [
      { label: "Level 1", color: RED },
      { label: "Level 2", color: GREEN },
      { label: "(none)", color: BLUE },
    ],
  });

  it("renders one chip+label row per entry, in the given order", () => {
    showView(v);
    const rows = swatchRows();
    expect(rows.map(rowLabel)).toEqual(["Level 1", "Level 2", "(none)"]);
    expect(rows.map(rowChipCss)).toEqual([RED, GREEN, BLUE].map(c => cssNorm(rgbCss(c))));
  });

  it("titles from v.label and draws no gradient bar", () => {
    showView(v);
    expect(titleText()).toBe("by Level");
    expect(legendEl.querySelector(".pf-legend-bar")).toBeNull();
  });

  it("categorical wins over a numeric domain when both are present", () => {
    showView(view({ legend: [{ label: "A", color: RED }], domain: [0, 9], ramp: "heat" }));
    expect(swatchRows()).toHaveLength(1);
    expect(legendEl.querySelector(".pf-legend-bar")).toBeNull();
  });

  it("an empty legend[] is categorical-with-no-rows, not a gradient fallback", () => {
    showView(view({ legend: [], domain: [0, 9], ramp: "heat" }));
    expect(swatchRows()).toHaveLength(0);
    expect(legendEl.querySelector(".pf-legend-bar")).toBeNull();
  });
});

describe("showViewLegend — numeric effective domain", () => {
  it("ticks come from v.domain (the domain the view actually used)", () => {
    showView(view({ label: "carbon", domain: [100, 300], ramp: "heat" }));
    expect(titleText()).toBe("carbon");
    expect(tickTexts()).toEqual(["300", "200", "100"]);
    expect(legendEl.querySelector(".pf-legend-bar")).not.toBeNull();
    expect(swatchRows()).toHaveLength(0);
  });

  it("gradient samples the shared ramp function (max at the top)", () => {
    showView(view({ domain: [0, 1], ramp: "greenred" }));
    const bg = (legendEl.querySelector(".pf-legend-bar") as HTMLElement).style.background;
    const top = cssNorm(rampCss("greenred", 1));
    const bottom = cssNorm(rampCss("greenred", 0));
    expect(bg).toContain(top);
    expect(bg).toContain(bottom);
    expect(bg.indexOf(top)).toBeLessThan(bg.lastIndexOf(bottom)); // max stop first = top
  });

  it("falls back to the ramp name as title when the view has no label", () => {
    showView(view({ domain: [0, 10], ramp: "viridis" }));
    expect(titleText()).toBe("viridis");
  });
});

describe("showViewLegend — clearing", () => {
  it("null clears a previously drawn legend", () => {
    showView(view({ legend: [{ label: "A", color: RED }] }));
    expect(legendEl.children).not.toHaveLength(0);
    showView(null);
    expect(legendEl.children).toHaveLength(0);
  });

  it("a view with neither legend nor domain+ramp clears", () => {
    showView(view({ domain: [0, 1], ramp: "heat" }));
    showView(view({}));
    expect(legendEl.children).toHaveLength(0);
  });

  it("domain without ramp (and ramp without domain) is not numeric", () => {
    showView(view({ domain: [0, 1] }));
    expect(legendEl.children).toHaveLength(0);
    showView(view({ ramp: "heat" }));
    expect(legendEl.children).toHaveLength(0);
  });
});

describe("showLegend — legacy colormap path unchanged", () => {
  it("draws title, gradient bar and max/mid/min ticks from the ColormapValue", () => {
    panes.showLegend({ ramp: "viridis", min: 0, max: 1 }, "demo");
    expect(titleText()).toBe("demo");
    expect(legendEl.querySelector(".pf-legend-bar")).not.toBeNull();
    expect(tickTexts()).toEqual(["1.00", "0.50", "0"]);
  });

  it("titles from the ramp name when no label is given, and null clears", () => {
    panes.showLegend({ ramp: "heat", min: 0, max: 100 });
    expect(titleText()).toBe("heat");
    panes.showLegend(null);
    expect(legendEl.children).toHaveLength(0);
  });
});

describe("legendModel (pure plan)", () => {
  it("maps null and empty views to none", () => {
    expect(legendModel(null)).toEqual({ kind: "none" });
    expect(legendModel(view({}))).toEqual({ kind: "none" });
  });

  it("builds a categorical plan preserving order and converting colors to css", () => {
    const plan = legendModel(view({
      label: "t", legend: [{ label: "A", color: RED }, { label: "B", color: GREEN }],
    }));
    expect(plan).toEqual({
      kind: "categorical",
      title: "t",
      entries: [{ label: "A", css: rgbCss(RED) }, { label: "B", css: rgbCss(GREEN) }],
    });
  });

  it("builds a numeric plan with pre-formatted top-to-bottom ticks", () => {
    const plan = legendModel(view({ label: "x", domain: [221, 413], ramp: "heat" }));
    expect(plan).toEqual({
      kind: "numeric", title: "x", ramp: "heat", ticks: ["413", "317", "221"],
    });
  });
});
