// Wave 2: view passthroughs, table filter/sort, compute.expr, and auto colormap domains.
import { describe, expect, it } from "vitest";
import type { Cell, SceneValue, TableValue, ViewValue } from "../../contracts";
import { evaluateGraph } from "../evaluate";
import { NODES } from "../nodes";
import { mockModel } from "../../fixtures/mockModel";
import { mkDoc, msgOf, sceneAt, stateOf, stubCtx, summaryOf, tableAt, viewAt } from "./harness";

const fullScene = (channels: Record<string, Cell[]> = {}): SceneValue => {
  const model = mockModel();
  const wrapped = Object.fromEntries(Object.entries(channels).map(([k, v]) => [k, { values: v }]));
  return { model, entities: Uint32Array.from(model.globalIds.map((_, i) => i)), channels: wrapped };
};

const call = async (kind: string, params: Record<string, unknown>, inputs: Record<string, unknown>) =>
  NODES.get(kind)!({ id: "x", kind, x: 0, y: 0, params }, inputs as never, stubCtx());

const T: TableValue = {
  columns: ["name", "n"],
  rows: [["b", 2], ["a", 10], ["c", null], ["d", 2], ["e", "7"], ["f", null]],
};

describe("registry coverage", () => {
  it("implements every declared kind and exports them verbatim", async () => {
    const { KINDS } = await import("../../kinds");
    const { kindsJson } = await import("../index");
    expect(KINDS.length).toBe(34);
    for (const k of KINDS) expect(NODES.has(k.kind), `no evaluate for ${k.kind}`).toBe(true);
    expect(NODES.size).toBe(KINDS.length);                          // no orphan implementations
    const exported = JSON.parse(kindsJson()) as { kind: string; description: string }[];
    expect(exported.map(k => k.kind)).toEqual(KINDS.map(k => k.kind));
    for (const k of exported) expect(k.description.length).toBeGreaterThan(0);
  });
});

describe("view passthroughs", () => {
  it("passes a scene through and reports channels", async () => {
    const scene = fullScene({ carbon: mockModel().globalIds.map((_, i) => i) });
    const out = await call("view.scene", {}, { in: scene });
    expect(out.value).toBe(scene);                                  // identity: no copy
    expect(out.summary).toBe("24 entities · channels: carbon");
  });

  it("says so when a scene has no channels", async () => {
    const out = await call("view.scene", {}, { in: fullScene() });
    expect(out.summary).toBe("24 entities · no channels");
  });

  it("passes a table through and reports rows × cols", async () => {
    const out = await call("view.table", {}, { in: T });
    expect(out.value).toBe(T);
    expect(out.summary).toBe("6 rows × 2 cols");
  });

  it("rejects a mismatched wire type", async () => {
    await expect(call("view.table", {}, { in: fullScene() })).rejects.toThrow(/not a table/);
    await expect(call("view.scene", {}, { in: T })).rejects.toThrow(/not a scene/);
  });
});

describe("table.filter", () => {
  const rows = async (op: string, value: string, column = "n") =>
    ((await call("table.filter", { column, op, value }, { in: T })).value as TableValue).rows;

  it("keeps rows passing the comparison and preserves columns", async () => {
    const out = (await call("table.filter", { column: "n", op: ">", value: "1" }, { in: T })).value as TableValue;
    expect(out.columns).toEqual(T.columns);
    expect(out.rows.map(r => r[0])).toEqual(["b", "a", "d", "e"]);   // "7" promotes to a number
  });

  it("shares select.byParameter's null rules", async () => {
    expect((await rows("exists", "")).length).toBe(4);              // two nulls drop out
    expect((await rows(">", "5")).map(r => r[0])).toEqual(["a", "e"]);
    expect((await rows("!=", "2")).map(r => r[0])).toEqual(["a", "c", "e", "f"]); // null != x is true
    expect((await rows("contains", "A", "name")).map(r => r[0])).toEqual(["a"]);
  });

  it("errors on a missing column; an unselected column is needs-setup", async () => {
    await expect(call("table.filter", { column: "nope", op: ">", value: "1" }, { in: T }))
      .rejects.toThrow(/no column "nope"/);
    await expect(call("table.filter", { op: ">", value: "1" }, { in: T }))
      .rejects.toThrow(/choose a filter column/);
  });
});

describe("table.sort", () => {
  const order = async (params: Record<string, unknown>) =>
    ((await call("table.sort", params, { in: T })).value as TableValue).rows.map(r => r[0]);

  it("sorts numerically, descending by default, nulls last", async () => {
    expect(await order({ column: "n" })).toEqual(["a", "e", "b", "d", "c", "f"]);
  });

  it("keeps nulls last when ascending too", async () => {
    expect(await order({ column: "n", descending: false })).toEqual(["b", "d", "e", "a", "c", "f"]);
  });

  it("sorts strings lexically and case-insensitively", async () => {
    const t: TableValue = { columns: ["s"], rows: [["Beta"], ["alpha"], ["Gamma"]] };
    const out = (await call("table.sort", { column: "s", descending: false }, { in: t })).value as TableValue;
    expect(out.rows.flat()).toEqual(["alpha", "Beta", "Gamma"]);
  });

  it("is stable: equal keys keep input order in both directions", async () => {
    const desc = await order({ column: "n" });
    expect(desc.slice(2, 4)).toEqual(["b", "d"]);                   // both 2, input order
    const asc = await order({ column: "n", descending: false });
    expect(asc.slice(0, 2)).toEqual(["b", "d"]);
    expect(asc.slice(4)).toEqual(["c", "f"]);                       // nulls keep input order
  });
});

describe("compute.expr", () => {
  const carbon: Cell[] = mockModel().globalIds.map((_, i) => (i < 6 ? (i + 1) * 100 : null));

  it("writes a channel from param and channel lookups", async () => {
    const scene = fullScene({ operational_carbon: carbon });
    const out = await call("compute.expr",
      { channel: "intensity", expr: "ch('operational_carbon') === null ? null : ch('operational_carbon') / param('Area')" },
      { in: scene });
    const s = out.value as SceneValue;
    const area = mockModel().param("Area");
    expect(s.channels["intensity"].values[0]).toBeCloseTo(100 / Number(area[0]), 9);
    expect(s.channels["intensity"].values[10]).toBe(null);
    expect(s.channels["operational_carbon"].values).toBe(carbon);   // input channels survive
    expect(out.summary).toBe("wrote intensity: 6 non-null of 24");
  });

  it("exposes gid/type/name/level and accepts string results", async () => {
    const out = await call("compute.expr", { channel: "tag", expr: "type + '@' + level + '#' + gid + '/' + name" },
      { in: fullScene() });
    const s = out.value as SceneValue;
    expect(s.channels["tag"].values[0]).toBe("IfcWall@Level 1#GID001/Wall 1");
    expect(out.summary).toBe("wrote tag: 24 non-null of 24");
  });

  it("makes a compile error a node error", async () => {
    await expect(call("compute.expr", { channel: "c", expr: "1 +" }, { in: fullScene() }))
      .rejects.toThrow(/expression error/);
  });

  it("nulls the entity when the expression throws or returns a non-value", async () => {
    const out = await call("compute.expr",
      { channel: "c", expr: "gid === 'GID001' ? (null).boom : (gid === 'GID002' ? {} : 1)" },
      { in: fullScene() });
    const s = out.value as SceneValue;
    expect(s.channels["c"].values[0]).toBe(null);                   // threw
    expect(s.channels["c"].values[1]).toBe(null);                   // object result
    expect(s.channels["c"].values[2]).toBe(1);
    expect(out.summary).toBe("wrote c: 22 non-null of 24");
  });

  it("drops NaN and Infinity to null", async () => {
    const out = await call("compute.expr", { channel: "c", expr: "gid === 'GID001' ? 0/0 : 1/0" },
      { in: fullScene() });
    expect((out.value as SceneValue).channels["c"].values.every(v => v === null)).toBe(true);
  });

  it("lets ch() see a channel that shadows a parameter, while param() stays raw", async () => {
    const scene = fullScene({ Area: mockModel().globalIds.map(() => 999) });
    const out = await call("compute.expr", { channel: "d", expr: "ch('Area') - param('Area')" }, { in: scene });
    const area = mockModel().param("Area");
    expect((out.value as SceneValue).channels["d"].values[0]).toBe(999 - Number(area[0]));
  });

  it("needs setup without a channel name or an expression", async () => {
    await expect(call("compute.expr", { expr: "1" }, { in: fullScene() }))
      .rejects.toThrow(/name the output channel/);
    await expect(call("compute.expr", { channel: "c" }, { in: fullScene() }))
      .rejects.toThrow(/enter an expression/);
  });
});

describe("colormap auto domain", () => {
  const chain = async (cmap: Record<string, unknown>, channel = "carbon") => {
    const doc = mkDoc(
      [
        { id: "a", kind: "load.model", params: { model: "mock" } },
        { id: "e", kind: "compute.expr", params: { channel: "carbon", expr: "ch('Area') * 10" } },
        { id: "b", kind: "viz.colormap", params: cmap },
        { id: "c", kind: "viz.colorBy", params: { channel } },
      ],
      [["a", "e", "in"], ["e", "c", "scene"], ["b", "c", "colormap"]],
    );
    return evaluateGraph(doc, stubCtx());
  };

  it("takes the domain from the data when auto is on", async () => {
    const r = await chain({ ramp: "viridis", auto: true, min: 0, max: 1 });
    const v = viewAt(r, "c");
    const area = mockModel().param("Area").map(Number);
    expect(v.domain).toEqual([Math.min(...area) * 10, Math.max(...area) * 10]);
    expect(v.ramp).toBe("viridis");
    expect(summaryOf(r, "c")).toBe(`24 colored · ${Math.min(...area) * 10}–${Math.max(...area) * 10}`);
  });

  it("takes the domain from the params when auto is off", async () => {
    const r = await chain({ ramp: "heat", auto: false, min: 10, max: 20 });
    expect(viewAt(r, "c").domain).toEqual([10, 20]);
    expect(viewAt(r, "c").ramp).toBe("heat");
    expect(summaryOf(r, "c")).toBe("24 colored · 10–20");
  });

  it("defaults to auto when the param is absent", async () => {
    const r = await chain({ ramp: "viridis" });
    expect(viewAt(r, "c").domain![1]).toBeGreaterThan(100);
  });

  it("widens a degenerate domain by one", async () => {
    const doc = mkDoc(
      [
        { id: "a", kind: "load.model", params: { model: "mock" } },
        { id: "e", kind: "compute.expr", params: { channel: "flat", expr: "7" } },
        { id: "b", kind: "viz.colormap", params: { auto: true } },
        { id: "c", kind: "viz.colorBy", params: { channel: "flat" } },
      ],
      [["a", "e", "in"], ["e", "c", "scene"], ["b", "c", "colormap"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(viewAt(r, "c").domain).toEqual([7, 8]);
    const fixed = await chain({ auto: false, min: 5, max: 5 });
    expect(viewAt(fixed, "c").domain).toEqual([5, 6]);
  });

  it("falls back to the configured domain when no value is numeric", async () => {
    // Wave 9: auto mode now renders text channels categorically, so pin the
    // numeric path explicitly — the fallback under test lives there.
    const doc = mkDoc(
      [
        { id: "a", kind: "load.model", params: { model: "mock" } },
        { id: "b", kind: "viz.colormap", params: { auto: true, min: 2, max: 9 } },
        { id: "c", kind: "viz.colorBy", params: { channel: "FireRating", mode: "numeric" } },
      ],
      [["a", "c", "scene"], ["b", "c", "colormap"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "c")).toBe("ok");
    expect(viewAt(r, "c").domain).toEqual([2, 9]);
  });

  it("keeps the colormap node's own summary on its configured range", async () => {
    const r = await chain({ ramp: "greenred", auto: true, min: 0, max: 100 });
    expect(summaryOf(r, "b")).toBe("0–100 greenred");
  });
});

describe("wave-2 kinds in a whole graph", () => {
  it("runs scene → expr → view → table → filter → sort → view green", async () => {
    const doc = mkDoc(
      [
        { id: "n1", kind: "load.model", params: { model: "mock" } },
        { id: "n2", kind: "compute.expr", params: { channel: "score", expr: "param('Area') * 2" } },
        { id: "n3", kind: "view.scene", params: {} },
        { id: "n4", kind: "table.fromScene", params: {} },
        { id: "n5", kind: "table.filter", params: { column: "score", op: ">", value: "40" } },
        { id: "n6", kind: "table.sort", params: { column: "score", descending: true } },
        { id: "n7", kind: "view.table", params: {} },
        { id: "n8", kind: "sink.table", params: {} },
      ],
      [["n1", "n2", "in"], ["n2", "n3", "in"], ["n3", "n4", "in"], ["n4", "n5", "in"],
       ["n5", "n6", "in"], ["n6", "n7", "in"], ["n7", "n8", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    for (const id of ["n1", "n2", "n3", "n4", "n5", "n6", "n7", "n8"]) {
      expect(`${id}:${stateOf(r, id)}`, msgOf(r, id)).toBe(`${id}:ok`);
    }
    expect(sceneAt(r, "n3").channels["score"]).toBeDefined();
    const sorted = tableAt(r, "n6");
    const scores = sorted.rows.map(row => Number(row[sorted.columns.indexOf("score")]));
    expect(scores.every(s => s > 40)).toBe(true);
    expect([...scores].sort((a, b) => b - a)).toEqual(scores);
    expect(summaryOf(r, "n7")).toBe(`${scores.length} rows × 5 cols`);
  });

  it("carries a view value's domain and ramp for the legend", async () => {
    const doc = mkDoc(
      [
        { id: "a", kind: "load.model", params: { model: "mock" } },
        { id: "b", kind: "viz.colormap", params: { ramp: "greenred", auto: true } },
        { id: "c", kind: "viz.colorBy", params: { channel: "Area", ghostOthers: false } },
      ],
      [["a", "c", "scene"], ["b", "c", "colormap"]],
    );
    const v = (await evaluateGraph(doc, stubCtx())).values.get("c") as ViewValue;
    expect(v.ramp).toBe("greenred");
    expect(v.domain).toEqual([5, 44]);
  });
});
