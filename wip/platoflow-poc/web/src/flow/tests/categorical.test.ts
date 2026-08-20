// Wave-9 Track B: viz.colorBy mode param — auto/numeric/categorical. The numeric ramp
// path stays byte-identical to pre-wave-9; categorical assigns first-appearance swatches,
// caps at 24, and keeps the unnamed group visible as "(none)" (gray, in the legend).
import { describe, expect, it } from "vitest";
import type {
  Cell, ChannelValue, GroupChannel, ModelData, SceneValue, ViewValue,
} from "../../contracts";
import { NODES } from "../nodes";
import { NeedsSetup } from "../types";
import { mockModel } from "../../fixtures/mockModel";
import { stubCtx } from "./harness";

const call = async (kind: string, params: Record<string, unknown>, inputs: Record<string, unknown>) =>
  NODES.get(kind)!({ id: "x", kind, x: 0, y: 0, params }, inputs as never, stubCtx());

const model = mockModel();
const N = model.entityCount;
const ALL = Array.from({ length: N }, (_, i) => i);

const scene = (
  channels: Record<string, ChannelValue> = {},
  groups?: GroupChannel,
  entities: number[] = ALL,
  m: ModelData = model,
): SceneValue =>
  ({ model: m, entities: Uint32Array.from(entities), channels, ...(groups ? { groups } : {}) });

const cmap = (over: Record<string, unknown> = {}) =>
  ({ ramp: "viridis", min: 0, max: 100, auto: true, ...over });

const colorBy = async (params: Record<string, unknown>, s: SceneValue, cm = cmap()) => {
  const out = await call("viz.colorBy", params, { scene: s, colormap: cm });
  return { out, v: out.value as ViewValue };
};

const GRAY = [0.35, 0.35, 0.35];
const colorAt = (v: ViewValue, k: number) =>
  [v.colors![k * 3], v.colors![k * 3 + 1], v.colors![k * 3 + 2]];
const expectRgb = (actual: number[], expected: number[]) =>
  expected.forEach((e, i) => expect(actual[i]).toBeCloseTo(e, 5));

/** Synthetic model bigger than the mock's 24 entities (for the 24-group cap). */
const bigModel = (n: number): ModelData => ({
  id: "big",
  entityCount: n,
  globalIds: Array.from({ length: n }, (_, i) => `B${i}`),
  types: new Array<string>(n).fill("IfcWall"),
  names: new Array<string>(n).fill(""),
  levels: new Array<string | null>(n).fill(null),
  paramNames: () => [],
  param: () => new Array<Cell>(n).fill(null),
});

describe("auto mode picks a path from the data", () => {
  it("numbers → numeric ramp, summary and domain unchanged from pre-wave-9", async () => {
    const { out, v } = await colorBy({ channel: "Area" }, scene());
    expect(v.domain).toEqual([5, 44]);
    expect(v.ramp).toBe("viridis");
    expect(v.legend).toBeUndefined();
    expect(out.summary).toBe("24 colored · 5–44");
  });

  it("a sparse numeric channel stays numeric (majority over non-empty cells)", async () => {
    const values: Cell[] = ALL.map(i => (i < 6 ? (i + 1) * 10 : null));
    const { out, v } = await colorBy({ channel: "c" }, scene({ c: { values } }));
    expect(v.domain).toEqual([10, 60]);
    expect(out.summary).toBe("24 colored · 10–60");
    expectRgb(colorAt(v, 6), GRAY);                // null entity renders gray
  });

  it("text → categorical: legend, no domain/ramp", async () => {
    const values: Cell[] = ALL.map(i => (i % 2 === 0 ? "A" : "B"));
    const { out, v } = await colorBy({ channel: "cat" }, scene({ cat: { values } }));
    expect(v.domain).toBeUndefined();
    expect(v.ramp).toBeUndefined();
    expect(v.legend!.map(e => e.label)).toEqual(["A", "B"]);
    expect(out.summary).toBe("24 colored · 2 groups");
    expectRgb(colorAt(v, 0), v.legend![0].color);
    expectRgb(colorAt(v, 1), v.legend![1].color);
    expect(v.legend![0].color).not.toEqual(v.legend![1].color);
  });
});

describe("grouped scenes", () => {
  const storeys: GroupChannel = { name: "storey", values: ALL.map(i => (i % 2 === 0 ? "1" : "2")) };

  it("colors by group when the channel is unset — even numeric-looking labels stay categorical", async () => {
    const { out, v } = await colorBy({}, scene({}, storeys));
    expect(v.label).toBe("storey");
    expect(v.domain).toBeUndefined();
    expect(v.legend!.map(e => e.label)).toEqual(["1", "2"]);
    expect(out.summary).toBe("24 colored · 2 groups");
    expectRgb(colorAt(v, 0), v.legend![0].color);
    expectRgb(colorAt(v, 3), v.legend![1].color);
  });

  it("a set channel param wins over the groups", async () => {
    const { v } = await colorBy({ channel: "Area" }, scene({}, storeys));
    expect(v.label).toBe("Area");
    expect(v.domain).toEqual([5, 44]);
  });

  it("needs setup (not error) when the channel is unset and the scene has no groups", async () => {
    const pending = call("viz.colorBy", {}, { scene: scene(), colormap: cmap() });
    await expect(pending).rejects.toBeInstanceOf(NeedsSetup);
    await expect(call("viz.colorBy", {}, { scene: scene(), colormap: cmap() }))
      .rejects.toThrow(/choose a channel/);
  });
});

describe("the unnamed group and the 24 cap", () => {
  it("nulls render gray and appear in the legend as \"(none)\"", async () => {
    const { out, v } = await colorBy({ channel: "FireRating" }, scene());
    expect(v.legend!.map(e => e.label)).toEqual(["2HR", "1HR", "(none)"]);
    expect(v.legend![2].color).toEqual(GRAY);
    expectRgb(colorAt(v, 8), GRAY);                // a door: no FireRating
    expect(out.summary).toBe("24 colored · 2 groups");
    expect(out.warning).toBeUndefined();
  });

  it("legend order is first appearance over the selected entities, not alphabetical", async () => {
    const values: Cell[] = ALL.map(i => (i === 0 ? "B" : "A"));
    const { v } = await colorBy({ channel: "c" }, scene({ c: { values } }));
    expect(v.legend!.map(e => e.label)).toEqual(["B", "A"]);
  });

  it("exactly 24 distinct values fit without a warning", async () => {
    const { out, v } = await colorBy({ channel: "Area", mode: "categorical" }, scene());
    expect(v.legend!.length).toBe(24);             // the mock's 24 Area values are all distinct
    expect(out.summary).toBe("24 colored · 24 groups");
    expect(out.warning).toBeUndefined();
  });

  it("values beyond 24 render gray with a warning; the first 24 keep their swatches", async () => {
    const big = bigModel(30);
    const all30 = Array.from({ length: 30 }, (_, i) => i);
    const values: Cell[] = all30.map(i => `v${i}`);
    const { out, v } = await colorBy({ channel: "c" }, scene({ c: { values } }, undefined, all30, big));
    expect(out.warning).toBe("6 values beyond 24 render gray");
    expect(out.summary).toBe("30 colored · 30 groups");
    expect(v.legend!.length).toBe(24);
    expectRgb(colorAt(v, 23), v.legend![23].color);
    for (const k of [24, 29]) expectRgb(colorAt(v, k), GRAY);
  });
});

describe("forced modes", () => {
  it("mode numeric on a text channel keeps the old fallback: configured domain, all gray", async () => {
    const { out, v } = await colorBy(
      { channel: "FireRating", mode: "numeric" }, scene(), cmap({ min: 2, max: 9 }));
    expect(v.domain).toEqual([2, 9]);
    expect(v.legend).toBeUndefined();
    expect(out.summary).toBe("24 colored · 2–9");
    expectRgb(colorAt(v, 0), GRAY);                // "2HR" has no numeric view
  });

  it("mode categorical on numbers stringifies them into labels", async () => {
    const values: Cell[] = ALL.map(i => (i % 2 === 0 ? 1 : 2));
    const { v } = await colorBy({ channel: "c", mode: "categorical" }, scene({ c: { values } }));
    expect(v.legend!.map(e => e.label)).toEqual(["1", "2"]);
    expect(v.domain).toBeUndefined();
  });
});
