// Wave-9 Track B: set algebra over entity selections (union/intersect/subtract/invert)
// plus table.count. Channel-merge policy under test: last-wins with a warning
// (design §14 open question — see NOTES.md "W9-B").
import { describe, expect, it } from "vitest";
import type { ChannelValue, GroupChannel, SceneValue, TableValue } from "../../contracts";
import { NODES } from "../nodes";
import { mockModel } from "../../fixtures/mockModel";
import { stubCtx } from "./harness";

const call = async (kind: string, params: Record<string, unknown>, inputs: Record<string, unknown>) =>
  NODES.get(kind)!({ id: "x", kind, x: 0, y: 0, params }, inputs as never, stubCtx());

const model = mockModel();                         // 0-7 walls, 8-13 doors, 14-19 windows, 20-23 slabs
const N = model.entityCount;

const scene = (
  entities: number[],
  channels: Record<string, ChannelValue> = {},
  groups?: GroupChannel,
): SceneValue =>
  ({ model, entities: Uint32Array.from(entities), channels, ...(groups ? { groups } : {}) });

const chan = (fill: string | number | null): ChannelValue => ({ values: new Array(N).fill(fill) });

const range = (lo: number, hi: number) => Array.from({ length: hi - lo }, (_, i) => lo + i);
const WALLS = range(0, 8), DOORS = range(8, 14), ALL = range(0, N);

const entitiesOf = async (kind: string, a: number[], b: number[]) =>
  [...((await call(kind, {}, { a: scene(a), b: scene(b) })).value as SceneValue).entities];

describe("set algebra entity math", () => {
  it("union combines disjoint selections ascending", async () => {
    const out = await call("select.union", {}, { a: scene(WALLS), b: scene(DOORS) });
    const s = out.value as SceneValue;
    expect(s.entities).toBeInstanceOf(Uint32Array);
    expect([...s.entities]).toEqual(range(0, 14));
    expect(out.summary).toBe("8 ∪ 6 → 14 entities");
  });

  it("union dedupes overlaps and stays ascending", async () => {
    expect(await entitiesOf("select.union", [0, 2, 4], [1, 2, 3])).toEqual([0, 1, 2, 3, 4]);
  });

  it("intersect keeps only common entities", async () => {
    const evens = ALL.filter(i => i % 2 === 0);
    const out = await call("select.intersect", {}, { a: scene(WALLS), b: scene(evens) });
    expect([...(out.value as SceneValue).entities]).toEqual([0, 2, 4, 6]);
    expect(out.summary).toBe("8 ∩ 12 → 4 entities");
  });

  it("subtract removes the second selection", async () => {
    const out = await call("select.subtract", {}, { a: scene(ALL), b: scene(DOORS) });
    expect([...(out.value as SceneValue).entities]).toEqual(ALL.filter(i => i < 8 || i >= 14));
    expect(out.summary).toBe("24 − 6 → 18 entities");
  });

  it("all three fail on inputs from different models", async () => {
    const other: SceneValue = { model: mockModel("other"), entities: Uint32Array.from([0]), channels: {} };
    for (const kind of ["select.union", "select.intersect", "select.subtract"]) {
      await expect(call(kind, {}, { a: scene(WALLS), b: other }))
        .rejects.toThrow(/inputs come from different models/);
    }
  });
});

describe("channel merge (§14: last-wins + warning)", () => {
  it("merges channels with the second input winning a clash, and warns", async () => {
    const first = chan(1), second = chan(2), only = chan("y");
    const out = await call("select.union", {},
      { a: scene(WALLS, { x: first, y: only }), b: scene(DOORS, { x: second }) });
    const s = out.value as SceneValue;
    expect(s.channels["x"]).toBe(second);          // b wins
    expect(s.channels["y"]).toBe(only);            // non-clashing survives
    expect(out.warning).toBe('channel "x": second input wins');
  });

  it("joins multiple clashes with '; '", async () => {
    const out = await call("select.intersect", {},
      { a: scene(WALLS, { x: chan(1), y: chan(1) }), b: scene(WALLS, { x: chan(2), y: chan(2) }) });
    expect(out.warning).toBe('channel "x": second input wins; channel "y": second input wins');
  });

  it("does not warn when both inputs carry the SAME ChannelValue (diamond)", async () => {
    const shared = chan(7);
    const out = await call("select.union", {},
      { a: scene(WALLS, { x: shared }), b: scene(DOORS, { x: shared }) });
    expect((out.value as SceneValue).channels["x"]).toBe(shared);
    expect(out.warning).toBeUndefined();
  });

  it("subtract passes the first input's channels through untouched (no merge)", async () => {
    const mine = chan(1);
    const out = await call("select.subtract", {},
      { a: scene(ALL, { x: mine }), b: scene(DOORS, { x: chan(2), y: chan(3) }) });
    const s = out.value as SceneValue;
    expect(s.channels["x"]).toBe(mine);
    expect("y" in s.channels).toBe(false);
    expect(out.warning).toBeUndefined();
  });

  it("union/intersect keep the FIRST input's groups, falling back to the second's", async () => {
    const ga: GroupChannel = { name: "ga", values: new Array<string | null>(N).fill("a") };
    const gb: GroupChannel = { name: "gb", values: new Array<string | null>(N).fill("b") };
    const both = await call("select.union", {}, { a: scene(WALLS, {}, ga), b: scene(DOORS, {}, gb) });
    expect((both.value as SceneValue).groups).toBe(ga);
    const onlyB = await call("select.union", {}, { a: scene(WALLS), b: scene(DOORS, {}, gb) });
    expect((onlyB.value as SceneValue).groups).toBe(gb);
    const neither = await call("select.intersect", {}, { a: scene(WALLS), b: scene(DOORS) });
    expect((neither.value as SceneValue).groups).toBeUndefined();
  });
});

describe("select.invert", () => {
  it("complements the selection over the whole model, ascending", async () => {
    const out = await call("select.invert", {}, { in: scene(WALLS) });
    expect([...(out.value as SceneValue).entities]).toEqual(range(8, N));
    expect(out.summary).toBe("16 of 24 entities");
  });

  it("preserves channels and groups untouched (full-length: no data loss)", async () => {
    const c = chan("v");
    const g: GroupChannel = { name: "g", values: new Array<string | null>(N).fill("x") };
    const out = await call("select.invert", {}, { in: scene(WALLS, { c }, g) });
    const s = out.value as SceneValue;
    expect(s.channels["c"]).toBe(c);
    expect(s.groups).toBe(g);
  });

  it("full ↔ empty round-trips", async () => {
    expect([...((await call("select.invert", {}, { in: scene(ALL) })).value as SceneValue).entities])
      .toEqual([]);
    expect([...((await call("select.invert", {}, { in: scene([]) })).value as SceneValue).entities])
      .toEqual(ALL);
  });
});

describe("table.count", () => {
  it("counts the selection into a one-row table", async () => {
    const out = await call("table.count", {}, { in: scene(WALLS) });
    expect(out.value as TableValue).toEqual({ columns: ["count"], rows: [[8]] });
    expect(out.summary).toBe("8 entities");
  });

  it("rejects a non-scene input", async () => {
    await expect(call("table.count", {}, { in: { columns: [], rows: [] } }))
      .rejects.toThrow(/not a scene/);
  });
});
