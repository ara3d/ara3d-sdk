// Wave-10 Track C: table.columns / table.stats / check.rule + viz.colorBy's
// embedded ramp (optional colormap input). Pins the semantics documented in
// defs-wave10.ts: case-fallback column matching, majority-numeric stats, verdicts
// in summaries, wired-colormap-wins, and the optional-input evaluator seam.
import { describe, expect, it } from "vitest";
import type { SceneValue, TableValue, ViewValue } from "../../contracts";
import { NODES } from "../nodes";
import { evaluateGraph } from "../evaluate";
import { NeedsSetup } from "../types";
import { mockModel } from "../../fixtures/mockModel";
import { mkDoc, stateOf, msgOf, stubCtx, viewAt, warnOf, summaryOf } from "./harness";

const call = async (kind: string, params: Record<string, unknown>, inputs: Record<string, unknown>) =>
  NODES.get(kind)!({ id: "x", kind, x: 0, y: 0, params }, inputs as never, stubCtx());

const T: TableValue = {
  columns: ["GlobalId", "Level", "cost", "note"],
  rows: [
    ["g1", "L1", 10, "a"],
    ["g2", "L1", "20", "b"],                        // numeric-looking string: counts as 20
    ["g3", "L2", "n/a", "c"],                       // non-numeric cost cell
    ["g4", null, 40, null],
  ],
  source: "carbon.csv",
};

const fullScene = (): SceneValue => {
  const model = mockModel();
  const entities = new Uint32Array(model.entityCount);
  for (let i = 0; i < model.entityCount; i++) entities[i] = i;
  return { model, entities, channels: {} };
};

// ---------------------------------------------------------------- table.columns

describe("table.columns", () => {
  it("keep mode returns the listed columns in LIST order", async () => {
    const out = await call("table.columns", { columns: "cost, GlobalId" }, { in: T });
    const v = out.value as TableValue;
    expect(v.columns).toEqual(["cost", "GlobalId"]);
    expect(v.rows[0]).toEqual([10, "g1"]);
    expect(v.rows[2]).toEqual(["n/a", "g3"]);
    expect(out.warning).toBeUndefined();
  });

  it("drop mode returns the remaining columns in ORIGINAL order", async () => {
    const out = await call("table.columns", { columns: "note, GlobalId", mode: "drop" }, { in: T });
    expect((out.value as TableValue).columns).toEqual(["Level", "cost"]);
  });

  it("falls back to case-insensitive matching when the exact name misses", async () => {
    const out = await call("table.columns", { columns: "LEVEL, Cost" }, { in: T });
    expect((out.value as TableValue).columns).toEqual(["Level", "cost"]);
    expect(out.warning).toBeUndefined();
  });

  it("an exact match wins over a case-folded near-name", async () => {
    const t: TableValue = { columns: ["Level", "level"], rows: [["A", "B"]] };
    const out = await call("table.columns", { columns: "level" }, { in: t });
    const v = out.value as TableValue;
    expect(v.columns).toEqual(["level"]);
    expect(v.rows[0]).toEqual(["B"]);
  });

  it("unknown names warn (with count) and are skipped, never an error", async () => {
    const out = await call("table.columns", { columns: "cost, foo, bar" }, { in: T });
    expect((out.value as TableValue).columns).toEqual(["cost"]);
    expect(out.warning).toBe("2 columns not found: foo, bar");
  });

  it("a single unknown name reads singular", async () => {
    const out = await call("table.columns", { columns: "foo" }, { in: T });
    expect(out.warning).toBe("1 column not found: foo");
    expect((out.value as TableValue).columns).toEqual([]);
  });

  it("duplicate names in the list collapse to one column", async () => {
    const out = await call("table.columns", { columns: "cost, cost" }, { in: T });
    expect((out.value as TableValue).columns).toEqual(["cost"]);
  });

  it("an empty list is needs-setup, and provenance passes through otherwise", async () => {
    await expect(call("table.columns", { columns: "  " }, { in: T }))
      .rejects.toBeInstanceOf(NeedsSetup);
    const out = await call("table.columns", { columns: "cost" }, { in: T });
    expect((out.value as TableValue).source).toBe("carbon.csv");
  });
});

// ------------------------------------------------------------------ table.stats

describe("table.stats", () => {
  it("summarizes majority-numeric columns with the exact output schema", async () => {
    const out = await call("table.stats", {}, { in: T });
    const v = out.value as TableValue;
    expect(v.columns).toEqual(["column", "count", "min", "max", "mean", "sum"]);
    // cost: 10, "20"→20, "n/a" dropped, 40 → count 3, mean 70/3 rounded to 3 decimals
    expect(v.rows).toEqual([["cost", 3, 10, 40, 23.333, 70]]);
    expect(out.summary).toBe("1 numeric column of 4");
  });

  it("reports per-column non-numeric drops in the warning", async () => {
    const out = await call("table.stats", {}, { in: T });
    expect(out.warning).toBe("non-numeric cells ignored — cost: 1");
  });

  it("nulls and empty strings are absent, not drops", async () => {
    const t: TableValue = { columns: ["v"], rows: [[1], [null], [""], [3]] };
    const out = await call("table.stats", {}, { in: t });
    expect((out.value as TableValue).rows).toEqual([["v", 2, 1, 3, 2, 4]]);
    expect(out.warning).toBeUndefined();
  });

  it("a mostly-text column with a stray number is not numeric", async () => {
    const t: TableValue = { columns: ["m"], rows: [["x"], ["y"], [5]] };
    const out = await call("table.stats", {}, { in: t });
    expect((out.value as TableValue).rows).toEqual([]);
    expect(out.warning).toBe("no numeric columns");
  });

  it("zero numeric columns is ok-with-warning, and provenance passes through", async () => {
    const t: TableValue = { columns: ["a"], rows: [["x"]], source: "sql" };
    const out = await call("table.stats", {}, { in: t });
    expect((out.value as TableValue).rows).toEqual([]);
    expect((out.value as TableValue).source).toBe("sql");
    expect(out.warning).toBe("no numeric columns");
  });
});

// ------------------------------------------------------------------- check.rule

describe("check.rule", () => {
  it("PASS: empty violations table, verdict in the summary, no warning", async () => {
    const out = await call("check.rule", { column: "cost", op: "exists" }, { in: T });
    const v = out.value as TableValue;
    expect(v.rows).toEqual([]);
    expect(v.columns).toEqual(T.columns);
    expect(out.summary).toBe("PASS · 4 rows checked");
    expect(out.warning).toBeUndefined();
    expect(v.source).toBe("check");
  });

  it("FAIL: outputs the violating rows and names the first violator's GlobalId", async () => {
    const out = await call("check.rule", { column: "cost", op: ">=", value: "15" }, { in: T });
    const v = out.value as TableValue;
    // g1 (10 < 15) and g3 ("n/a" cannot satisfy an ordered op) violate; "20" coerces and passes.
    expect(v.rows.map(r => r[0])).toEqual(["g1", "g3"]);
    expect(out.summary).toBe("FAIL · 2 of 4 rows violate");
    expect(out.warning).toBe("first violation: g1");
  });

  it("violations are the exact complement of table.filter under the same predicate", async () => {
    const params = { column: "cost", op: ">=", value: "15" };
    const kept = (await call("table.filter", params, { in: T })).value as TableValue;
    const bad = (await call("check.rule", params, { in: T })).value as TableValue;
    expect(kept.rows.length + bad.rows.length).toBe(T.rows.length);
    const keptKeys = new Set(kept.rows.map(r => r[0]));
    for (const r of bad.rows) expect(keptKeys.has(r[0])).toBe(false);
  });

  it("a null cell fails the rule (it is a violation, not skipped)", async () => {
    const out = await call("check.rule", { column: "Level", op: "exists" }, { in: T });
    const v = out.value as TableValue;
    expect(v.rows.map(r => r[0])).toEqual(["g4"]);
    expect(out.warning).toBe("first violation: g4");
  });

  it("without a GlobalId column the first cell is the key", async () => {
    const t: TableValue = { columns: ["name", "n"], rows: [["x", 1], ["y", 9]] };
    const out = await call("check.rule", { column: "n", op: ">", value: "5" }, { in: t });
    expect(out.warning).toBe("first violation: x");
  });

  it("empty column param is needs-setup; an unknown column is a real error", async () => {
    await expect(call("check.rule", { op: ">" }, { in: T }))
      .rejects.toBeInstanceOf(NeedsSetup);
    await expect(call("check.rule", { column: "nope", op: ">" }, { in: T }))
      .rejects.toThrow(/table has no column "nope"/);
  });
});

// ----------------------------------------------- viz.colorBy embedded ramp (W10)

describe("viz.colorBy embedded ramp", () => {
  it("with no colormap input, the node's own params drive the ramp (auto domain)", async () => {
    const out = await call("viz.colorBy", { channel: "Area" }, { scene: fullScene() });
    const v = out.value as ViewValue;
    expect(v.domain).toEqual([5, 44]);              // mock Area values, resolved from data
    expect(v.ramp).toBe("viridis");
  });

  it("embedded auto=false uses the node's min/max params", async () => {
    const out = await call("viz.colorBy",
      { channel: "Area", ramp: "heat", auto: false, min: 2, max: 9 }, { scene: fullScene() });
    const v = out.value as ViewValue;
    expect(v.domain).toEqual([2, 9]);
    expect(v.ramp).toBe("heat");
  });

  it("embedded path equals the colormap-node path exactly (same resolution code)", async () => {
    const cmapOut = await call("viz.colormap", { ramp: "viridis", auto: true }, {});
    const wired = (await call("viz.colorBy", { channel: "Area" },
      { scene: fullScene(), colormap: cmapOut.value })).value as ViewValue;
    const embedded = (await call("viz.colorBy", { channel: "Area" },
      { scene: fullScene() })).value as ViewValue;
    expect(embedded.domain).toEqual(wired.domain);
    expect(embedded.ramp).toBe(wired.ramp);
    expect([...embedded.colors!]).toEqual([...wired.colors!]);
  });

  it("a wired colormap overrides the embedded params ENTIRELY", async () => {
    const cmapOut = await call("viz.colormap", { ramp: "viridis", auto: true }, {});
    const out = await call("viz.colorBy",
      { channel: "Area", ramp: "heat", auto: false, min: 0, max: 10 },
      { scene: fullScene(), colormap: cmapOut.value });
    const v = out.value as ViewValue;
    expect(v.ramp).toBe("viridis");                 // wired ramp, not embedded heat
    expect(v.domain).toEqual([5, 44]);              // wired auto, not embedded 0–10
  });
});

// -------------------------------------------- optional-input evaluator mechanics

describe("optional colormap input through the evaluator", () => {
  it("colorBy with no colormap wire evaluates ok (embedded ramp)", async () => {
    const doc = mkDoc(
      [{ id: "m", kind: "load.model", params: { model: "mock" } },
       { id: "cb", kind: "viz.colorBy", params: { channel: "Area" } }],
      [["m", "cb", "scene"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "cb")).toBe("ok");
    expect(viewAt(r, "cb").domain).toEqual([5, 44]);
    expect(summaryOf(r, "cb")).toBe("24 colored · 5–44");
    expect(warnOf(r, "cb")).toBeUndefined();
  });

  it("the scene input stays required: unwired scene is needs-setup", async () => {
    const doc = mkDoc([{ id: "cb", kind: "viz.colorBy", params: { channel: "Area" } }]);
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "cb")).toBe("needs-setup");
    expect(msgOf(r, "cb")).toBe('missing input "scene"');
  });

  it("a wired-but-broken colormap still poisons colorBy (optional ≠ ignorable)", async () => {
    const doc = mkDoc(
      [{ id: "m", kind: "load.model", params: { model: "mock" } },
       { id: "bad", kind: "table.sql", params: { sql: "" } },   // needs-setup source
       { id: "cb", kind: "viz.colorBy", params: { channel: "Area" } }],
      [["m", "cb", "scene"], ["bad", "cb", "colormap"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "cb")).toBe("needs-setup");
    expect(msgOf(r, "cb")).toBe("waiting on bad");
  });
});
