// Wave-9 Track B: group.by — the hinge node (design §3). GroupChannel is FULL-LENGTH
// over all model entities; the summary counts distinct labels over the SELECTION;
// null/empty is the unnamed group — real data, never dropped.
import { describe, expect, it } from "vitest";
import type { Cell, ChannelValue, SceneValue } from "../../contracts";
import { NODES } from "../nodes";
import { NeedsSetup } from "../types";
import { mockModel } from "../../fixtures/mockModel";
import { stubCtx } from "./harness";

const call = async (kind: string, params: Record<string, unknown>, inputs: Record<string, unknown>) =>
  NODES.get(kind)!({ id: "x", kind, x: 0, y: 0, params }, inputs as never, stubCtx());

const model = mockModel();
const N = model.entityCount;

const scene = (entities: number[], channels: Record<string, ChannelValue> = {}): SceneValue =>
  ({ model, entities: Uint32Array.from(entities), channels });

const ALL = Array.from({ length: N }, (_, i) => i);
const DOORS = [8, 9, 10, 11, 12, 13];

const groupBy = async (by: string, s: SceneValue = scene(ALL)) => {
  const out = await call("group.by", { by }, { in: s });
  return { out, s: out.value as SceneValue };
};

describe("group.by", () => {
  it("groups by Type: full-length labels straight off the model", async () => {
    const { out, s } = await groupBy("Type");
    expect(s.groups!.name).toBe("Type");
    expect(s.groups!.values).toEqual(model.types);
    expect(s.groups!.values.length).toBe(N);
    expect(out.summary).toBe("4 groups over 24 entities");
  });

  it("groups by Level", async () => {
    const { out, s } = await groupBy("Level");
    expect(s.groups!.values[0]).toBe("Level 1");
    expect(s.groups!.values[1]).toBe("Level 2");
    expect(out.summary).toBe("2 groups over 24 entities");
  });

  it("groups by a model parameter; nulls form the unnamed group", async () => {
    const { out, s } = await groupBy("FireRating");
    expect(s.groups!.values[0]).toBe("2HR");       // wall, i % 3 === 0
    expect(s.groups!.values[1]).toBe("1HR");
    expect(s.groups!.values[8]).toBe(null);        // doors have no FireRating
    expect(out.summary).toBe("2 groups over 24 entities + unnamed");
  });

  it("groups by a channel, stringifying cells; empty string counts as null", async () => {
    const values: Cell[] = ALL.map(i => (i < 8 ? "A" : i < 14 ? "" : i < 20 ? 7 : null));
    const { out, s } = await groupBy("cat", scene(ALL, { cat: { values } }));
    expect(s.groups!.values[0]).toBe("A");
    expect(s.groups!.values[8]).toBe(null);        // "" → the unnamed group
    expect(s.groups!.values[14]).toBe("7");        // number → String(cell)
    expect(s.groups!.values[20]).toBe(null);
    expect(out.summary).toBe("2 groups over 24 entities + unnamed");
  });

  it("summarizes over the SELECTION but labels ALL entities", async () => {
    const { out, s } = await groupBy("FireRating", scene(DOORS));
    expect(out.summary).toBe("0 groups over 6 entities + unnamed");
    expect(s.groups!.values.length).toBe(N);       // full-length regardless of selection
    expect(s.groups!.values[0]).toBe("2HR");       // unselected walls still labeled
  });

  it("passes selection and channels through untouched", async () => {
    const c: ChannelValue = { values: new Array<Cell>(N).fill(1) };
    const input = scene(DOORS, { c });
    const { s } = await groupBy("Type", input);
    expect(s.entities).toBe(input.entities);
    expect(s.channels["c"]).toBe(c);
  });

  it("needs setup while the key is unset — unconfigured, not broken", async () => {
    const pending = call("group.by", {}, { in: scene(ALL) });
    await expect(pending).rejects.toBeInstanceOf(NeedsSetup);
    await expect(call("group.by", {}, { in: scene(ALL) })).rejects.toThrow(/choose a grouping key/);
  });

  it("errors on an unknown grouping key", async () => {
    await expect(call("group.by", { by: "Nope" }, { in: scene(ALL) }))
      .rejects.toThrow(/no parameter or channel named "Nope"/);
  });
});
