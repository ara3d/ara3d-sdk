// Wave-11 Track N: select.checklist / viz.boxes / viz.explode.
// Pinned semantics under test: unnamed sentinel "(none)", inverted excluded
// storage, exact-match exclusion, model-space Z-up explode offsets, natural
// (numeric-aware) level order, unnamed-level-stays-put.
import { describe, expect, it } from "vitest";
import type { Cell, ChannelValue, ModelData, SceneValue, ViewValue } from "../../contracts";
import { NODES } from "../nodes";
import { evaluateGraph } from "../evaluate";
import { UNNAMED_GROUP } from "../lib";
import { mockModel } from "../../fixtures/mockModel";
import { mkDoc, stubCtx } from "./harness";

const call = async (kind: string, params: Record<string, unknown>, inputs: Record<string, unknown>) =>
  NODES.get(kind)!({ id: "x", kind, x: 0, y: 0, params }, inputs as never, stubCtx());

const model = mockModel();                         // 0-7 walls, 8-13 doors, 14-19 windows, 20-23 slabs
const N = model.entityCount;
const ALL = Array.from({ length: N }, (_, i) => i);

const scene = (entities: number[], channels: Record<string, ChannelValue> = {}): SceneValue =>
  ({ model, entities: Uint32Array.from(entities), channels });

/** Tiny purpose-built model: an unnamed level, a coverings class, and level
 *  names that only NATURAL order sorts right ("Level 2" < "Level 10"). */
function miniModel(): ModelData {
  const types = ["IfcWall", "IfcWall", "IfcCovering", "IfcDoor", "IfcWall", "IfcSlab"];
  const levels: (string | null)[] = ["Level 2", "Level 10", "Level 2", null, "Level 1", "Level 10"];
  const n = types.length;
  return {
    id: "mini",
    entityCount: n,
    globalIds: types.map((_, i) => `MINI${i}`),
    types,
    names: types.map((t, i) => `${t} ${i}`),
    levels,
    paramNames: () => [],
    param: () => new Array<Cell>(n).fill(null),
  };
}

const mini = miniModel();
const miniScene = (): SceneValue =>
  ({ model: mini, entities: Uint32Array.from([0, 1, 2, 3, 4, 5]), channels: {} });

const view = (over: Partial<ViewValue> = {}): ViewValue => ({
  model: mini,
  entities: Uint32Array.from([0, 1, 2, 3, 4, 5]),
  ghostOthers: true,
  ...over,
});

// ── select.checklist ─────────────────────────────────────────────────────────

describe("select.checklist", () => {
  it("enumerates every live value with counts, sorted count desc then natural asc", async () => {
    const out = await call("select.checklist", { source: "types" }, { in: scene(ALL) });
    expect(out.checklist).toEqual([
      { value: "IfcWall", on: true, count: 8 },
      { value: "IfcDoor", on: true, count: 6 },    // count tie with windows →
      { value: "IfcWindow", on: true, count: 6 },  // natural asc breaks it
      { value: "IfcSlab", on: true, count: 4 },
    ]);
  });

  it("inverted storage: empty excluded means everything ticked and passed through", async () => {
    const out = await call("select.checklist", { source: "types", excluded: "" }, { in: scene(ALL) });
    expect([...(out.value as SceneValue).entities]).toEqual(ALL);
    expect(out.summary).toBe("4 of 4 ticked · 24 entities");
    expect(out.warning).toBeUndefined();
  });

  it("excluding a value drops its entities and unticks its row", async () => {
    const out = await call("select.checklist", { source: "types", excluded: "IfcWall" }, { in: scene(ALL) });
    expect([...(out.value as SceneValue).entities]).toEqual(ALL.filter(i => i >= 8));
    expect(out.checklist!.find(c => c.value === "IfcWall")).toEqual({ value: "IfcWall", on: false, count: 8 });
    expect(out.summary).toBe("3 of 4 ticked · 16 entities");
  });

  it("comma list parses with whitespace and excludes each named value", async () => {
    const out = await call("select.checklist", { source: "types", excluded: " IfcWall , IfcSlab " }, { in: scene(ALL) });
    expect([...(out.value as SceneValue).entities]).toEqual(ALL.filter(i => i >= 8 && i < 20));
    expect(out.summary).toBe("2 of 4 ticked · 12 entities");
  });

  it("matching is EXACT — a case-folded name matches nothing and reads as stale", async () => {
    const out = await call("select.checklist", { source: "types", excluded: "ifcwall" }, { in: scene(ALL) });
    expect((out.value as SceneValue).entities.length).toBe(24);   // filters nothing
    expect(out.warning).toBe("excluded value no longer present: ifcwall");
    expect(out.checklist!.map(c => c.value)).not.toContain("ifcwall");
  });

  it("channels and groups pass through untouched (derive)", async () => {
    const chan: ChannelValue = { values: new Array<Cell>(N).fill(1), source: "expr" };
    const s: SceneValue = { ...scene(ALL, { c: chan }), groups: { name: "g", values: new Array(N).fill("x") } };
    const out = await call("select.checklist", { source: "types", excluded: "IfcSlab" }, { in: s });
    const v = out.value as SceneValue;
    expect(v.channels["c"]).toBe(chan);
    expect(v.groups).toBe(s.groups);
  });

  it("levels source shows the unnamed level as the (none) sentinel", async () => {
    const out = await call("select.checklist", { source: "levels" }, { in: miniScene() });
    expect(out.checklist!.map(c => c.value)).toContain(UNNAMED_GROUP);
    expect(out.checklist!.find(c => c.value === UNNAMED_GROUP)!.count).toBe(1);
  });

  it("excluding (none) removes exactly the null-level entities", async () => {
    const out = await call("select.checklist", { source: "levels", excluded: UNNAMED_GROUP }, { in: miniScene() });
    expect([...(out.value as SceneValue).entities]).toEqual([0, 1, 2, 4, 5]);  // entity 3 has level null
  });

  it("stale excluded values warn, filter nothing, and stay out of the checklist", async () => {
    const out = await call("select.checklist", { source: "types", excluded: "IfcGhost,IfcSlab" }, { in: scene(ALL) });
    expect((out.value as SceneValue).entities.length).toBe(20);   // only IfcSlab filtered
    expect(out.warning).toBe("excluded value no longer present: IfcGhost");
    expect(out.checklist!.map(c => c.value)).not.toContain("IfcGhost");
  });

  it("empty input scene is ok with an empty checklist", async () => {
    const out = await call("select.checklist", { source: "types" }, { in: scene([]) });
    expect(out.checklist).toEqual([]);
    expect(out.summary).toBe("0 of 0 ticked · 0 entities");
  });

  it("evaluator copies the checklist onto NodeStatus", async () => {
    const doc = mkDoc(
      [{ id: "m", kind: "load.model", params: { model: "mock" } },
       { id: "c", kind: "select.checklist", params: {} }],
      [["m", "c", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    const st = r.status.get("c")!;
    expect(st.state).toBe("ok");
    expect(st.checklist!.length).toBe(4);          // default source "types" via kinds default
    expect(st.checklist![0]).toEqual({ value: "IfcWall", on: true, count: 8 });
  });
});

// ── viz.boxes ────────────────────────────────────────────────────────────────

describe("viz.boxes", () => {
  it("sets boxes and passes everything else through by reference", async () => {
    const colors = new Float32Array(18).fill(0.5);
    const input = view({ colors, label: "L", domain: [0, 9], ramp: "heat" });
    const out = await call("viz.boxes", {}, { in: input });
    const v = out.value as ViewValue;
    expect(v.boxes).toBe(true);
    expect(v.entities).toBe(input.entities);
    expect(v.colors).toBe(colors);
    expect(v.domain).toEqual([0, 9]);
    expect(out.summary).toBe("6 entities · boxes");
  });

  it("never mutates the input view", async () => {
    const input = view();
    const out = await call("viz.boxes", {}, { in: input });
    expect("boxes" in input).toBe(false);
    expect(out.value).not.toBe(input);
  });

  it("rejects a non-view input", async () => {
    await expect(call("viz.boxes", {}, { in: scene(ALL) })).rejects.toThrow('not a view');
  });
});

// ── viz.explode ──────────────────────────────────────────────────────────────

const offsetsOf = (v: ViewValue): number[] => [...v.offsets!];

describe("viz.explode", () => {
  it("hides listed IFC classes case-insensitively with or without the Ifc prefix", async () => {
    for (const hide of ["IFCCOVERING", "covering", "IfcCovering"]) {
      const v = (await call("viz.explode", { spacing: 10, hide }, { in: view() })).value as ViewValue;
      expect([...v.entities]).toEqual([0, 1, 3, 4, 5]);          // entity 2 = IfcCovering
    }
  });

  it("offsets are 3 per entity, levelIndex × spacing on Z only (model space, Z-up)", async () => {
    const v = (await call("viz.explode", { spacing: 10, hide: "" }, { in: view() })).value as ViewValue;
    expect(v.offsets!.length).toBe(v.entities.length * 3);
    // natural order: Level 1 → 0, Level 2 → 1, Level 10 → 2
    // entities 0..5 levels: L2, L10, L2, null, L1, L10
    expect(offsetsOf(v)).toEqual([0, 0, 10, 0, 0, 20, 0, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, 20]);
  });

  it("natural order pins Level 2 below Level 10 (lexicographic would flip them)", async () => {
    const v = (await call("viz.explode", { spacing: 1, hide: "" }, { in: view() })).value as ViewValue;
    const z = (k: number) => offsetsOf(v)[k * 3 + 2];
    expect(z(4)).toBe(0);                          // Level 1
    expect(z(0)).toBe(1);                          // Level 2
    expect(z(1)).toBe(2);                          // Level 10
  });

  it("the unnamed level stays put", async () => {
    const v = (await call("viz.explode", { spacing: 50, hide: "" }, { in: view() })).value as ViewValue;
    expect(offsetsOf(v).slice(9, 12)).toEqual([0, 0, 0]);        // entity 3, level null
  });

  it("composes additively with incoming offsets and filters colors to survivors", async () => {
    const colors = new Float32Array(18);
    for (let i = 0; i < 18; i++) colors[i] = i / 100;
    const offsets = new Float32Array(18);
    offsets.fill(1);                               // pre-existing unit shift on every axis
    const input = view({ colors, offsets, legend: [{ label: "a", color: [1, 0, 0] }] });
    const v = (await call("viz.explode", { spacing: 10, hide: "IfcCovering" }, { in: input })).value as ViewValue;
    expect([...v.entities]).toEqual([0, 1, 3, 4, 5]);
    // survivor colors keep their ORIGINAL triplets (entity 2's slice is gone)
    expect([...v.colors!]).toEqual([0, .01, .02, .03, .04, .05, .09, .1, .11, .12, .13, .14, .15, .16, .17]
      .map(x => Math.fround(x)));
    // composed: prior 1 on every axis + explode on Z (L2→10, L10→20, null→0, L1→0, L10→20)
    expect([...v.offsets!]).toEqual([1, 1, 11, 1, 1, 21, 1, 1, 1, 1, 1, 1, 1, 1, 21]);
    expect(v.legend).toBe(input.legend);           // legend passes through
    expect(input.offsets).toBe(offsets);           // input untouched
    expect([...offsets]).toEqual(new Array(18).fill(1));
  });

  it("negative spacing clamps to 0 (everything stays put)", async () => {
    const v = (await call("viz.explode", { spacing: -5, hide: "" }, { in: view() })).value as ViewValue;
    expect(offsetsOf(v)).toEqual(new Array(18).fill(0));
  });

  it("summary reports entities, distinct named levels, and spacing", async () => {
    const out = await call("viz.explode", { spacing: 15, hide: "IfcCovering" }, { in: view() });
    expect(out.summary).toBe("5 entities · 3 levels × 15");
  });
});
