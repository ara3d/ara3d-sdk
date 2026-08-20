// Beat 3: scene → table → aggregate → data grid.
import { describe, expect, it } from "vitest";
import { evaluateGraph } from "../evaluate";
import { mockModel } from "../../fixtures/mockModel";
import { carbonByGid, mkDoc, msgOf, stateOf, stubCtx, summaryOf, tableAt } from "./harness";

const pipeline = (agg: string, groupBy = "Level", value = "embodied_carbon") => mkDoc(
  [
    { id: "n1", kind: "load.model", params: { model: "mock" } },
    { id: "n2", kind: "data.csv", params: { url: "carbon.csv" } },
    { id: "n3", kind: "attach.column", params: { valueColumn: "embodied_carbon" } },
    { id: "n4", kind: "table.fromScene" },
    { id: "n5", kind: "table.aggregate", params: { groupBy, value, agg } },
    { id: "n6", kind: "sink.table" },
  ],
  [["n1", "n3", "scene"], ["n2", "n3", "table"], ["n3", "n4", "in"], ["n4", "n5", "in"], ["n5", "n6", "in"]],
);

/** Independent oracle: carbon per level straight from the fixture. */
function expectedByLevel(): Map<string, number[]> {
  const m = mockModel();
  const carbon = carbonByGid();
  const out = new Map<string, number[]>();
  for (let i = 0; i < m.entityCount; i++) {
    const level = m.levels[i] ?? "";
    const v = carbon.get(m.globalIds[i])!;
    out.set(level, [...(out.get(level) ?? []), v]);
  }
  return out;
}

describe("beat 3 — table + aggregate", () => {
  it("runs the whole chain green", async () => {
    const r = await evaluateGraph(pipeline("sum"), stubCtx());
    for (const id of ["n1", "n2", "n3", "n4", "n5", "n6"]) {
      expect(`${id}:${stateOf(r, id)}`, msgOf(r, id)).toBe(`${id}:ok`);
    }
    expect(summaryOf(r, "n3")).toBe("matched 24 of 24");
  });

  it("fromScene emits the fixed columns plus one per channel", async () => {
    const r = await evaluateGraph(pipeline("sum"), stubCtx());
    const t = tableAt(r, "n4");
    expect(t.columns).toEqual(["GlobalId", "Type", "Name", "Level", "embodied_carbon"]);
    expect(t.rows.length).toBe(mockModel().entityCount);
    expect(t.rows[0]).toEqual(["GID001", "IfcWall", "Wall 1", "Level 1", 3]);
  });

  it("groups by level and sums embodied carbon", async () => {
    const r = await evaluateGraph(pipeline("sum"), stubCtx());
    const t = tableAt(r, "n5");
    const want = expectedByLevel();
    expect(t.columns).toEqual(["Level", "sum_embodied_carbon"]);
    expect(t.rows.map(row => row[0])).toEqual(["Level 1", "Level 2"]);   // sorted by group
    for (const [level, sum] of t.rows) {
      const vals = want.get(String(level))!;
      expect(sum).toBe(vals.reduce((a, b) => a + b, 0));
    }
    expect(summaryOf(r, "n5")).toBe("2 rows");
  });

  it("supports avg / min / max / count", async () => {
    const want = expectedByLevel();
    for (const agg of ["avg", "min", "max", "count"] as const) {
      const r = await evaluateGraph(pipeline(agg), stubCtx());
      const t = tableAt(r, "n5");
      expect(t.columns[1]).toBe(`${agg}_embodied_carbon`);
      for (const [level, got] of t.rows) {
        const v = want.get(String(level))!;
        const expected =
          agg === "avg" ? v.reduce((a, b) => a + b, 0) / v.length
            : agg === "min" ? Math.min(...v)
              : agg === "max" ? Math.max(...v)
                : v.length;
        expect(got, `${agg} ${level}`).toBeCloseTo(expected, 9);
      }
    }
  });

  it("groups by type too", async () => {
    const r = await evaluateGraph(pipeline("count", "Type", "embodied_carbon"), stubCtx());
    const t = tableAt(r, "n5");
    expect(t.rows).toEqual([["IfcDoor", 6], ["IfcSlab", 4], ["IfcWall", 8], ["IfcWindow", 6]]);
  });

  it("sink.table passes the table through and reports row count", async () => {
    const r = await evaluateGraph(pipeline("sum"), stubCtx());
    expect(tableAt(r, "n6")).toEqual(tableAt(r, "n5"));
    expect(summaryOf(r, "n6")).toBe("2 rows");
  });

  it("table.sql takes the model id from the incoming scene", async () => {
    let seen = "";
    const doc = mkDoc(
      [
        { id: "a", kind: "load.model", params: { model: "mock" } },
        { id: "b", kind: "table.sql", params: { sql: "SELECT 1" } },
      ],
      [["a", "b", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx({
      sql: async (modelId, q) => { seen = `${modelId}|${q}`; return { columns: ["x"], rows: [[1]] }; },
    }));
    expect(seen).toBe("mock|SELECT 1");
    expect(stateOf(r, "b")).toBe("ok");
    expect(summaryOf(r, "b")).toBe("1 rows");
  });

  it("surfaces sql errors as the node message", async () => {
    const doc = mkDoc(
      [
        { id: "a", kind: "load.model", params: { model: "mock" } },
        { id: "b", kind: "table.sql", params: { sql: "SELECT bogus" } },
        { id: "c", kind: "sink.table" },
      ],
      [["a", "b", "in"], ["b", "c", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx({
      sql: async () => { throw new Error('Binder Error: no column "bogus"'); },
    }));
    expect(stateOf(r, "b")).toBe("error");
    expect(msgOf(r, "b")).toContain("bogus");
    expect(stateOf(r, "c")).toBe("error");
    expect(msgOf(r, "c")).toBe('upstream error in b: Binder Error: no column "bogus"');
  });
});
