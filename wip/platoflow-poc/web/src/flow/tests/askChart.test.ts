// Wave 3: table.ask (NL → SQL via host sidecar) and chart.bar (in-node bar chart),
// plus the evaluator's new ParamSchema.default fallback and status.chart/detail plumbing.
import { describe, expect, it } from "vitest";
import type { EvalCtx, SceneValue, TableValue } from "../../contracts";
import { evaluateGraph } from "../evaluate";
import { NODES } from "../nodes";
import { mockModel } from "../../fixtures/mockModel";
import { CANNED_SQL, mkDoc, msgOf, stateOf, stubCtx, summaryOf } from "./harness";

const fullScene = (): SceneValue => {
  const model = mockModel();
  return { model, entities: Uint32Array.from(model.globalIds.map((_, i) => i)), channels: {} };
};

const call = async (
  kind: string, params: Record<string, unknown>, inputs: Record<string, unknown>, ctx?: EvalCtx,
) => NODES.get(kind)!({ id: "x", kind, x: 0, y: 0, params }, inputs as never, ctx ?? stubCtx());

describe("table.ask", () => {
  const GEN_SQL = "SELECT Category, COUNT(*) AS n FROM EntityText GROUP BY Category";

  it("asks the sidecar, runs the returned SQL, and shows the SQL as detail", async () => {
    const seen: unknown[] = [];
    const ctx = stubCtx({
      ask: async (modelId, question) => { seen.push(modelId, question); return { sql: GEN_SQL }; },
      sql: async (modelId, sql) => { seen.push(modelId, sql); return CANNED_SQL; },
    });
    const out = await call("table.ask", { question: "how many walls?" }, { in: fullScene() }, ctx);
    expect(seen).toEqual(["mock", "how many walls?", "mock", GEN_SQL]);
    expect(out.value).toBe(CANNED_SQL);
    expect(out.detail).toBe(GEN_SQL);
    expect(out.summary).toBe("4 rows");
  });

  it("errors clearly when the host has no AI sidecar (ctx.ask absent)", async () => {
    await expect(call("table.ask", { question: "q" }, { in: fullScene() }))
      .rejects.toThrow("host AI sidecar unavailable");
  });

  it("surfaces a host error verbatim as the node error", async () => {
    const ctx = stubCtx({
      ask: async () => { throw new Error("ANTHROPIC_API_KEY not set on host"); },
    });
    const doc = mkDoc(
      [
        { id: "a", kind: "load.model", params: { model: "mock" } },
        { id: "q", kind: "table.ask", params: { question: "q" } },
      ],
      [["a", "q", "in"]],
    );
    const r = await evaluateGraph(doc, ctx);
    expect(stateOf(r, "q")).toBe("error");
    expect(msgOf(r, "q")).toBe("ANTHROPIC_API_KEY not set on host");
  });

  it("needs setup without a question", async () => {
    const ctx = stubCtx({ ask: async () => ({ sql: GEN_SQL }) });
    await expect(call("table.ask", { question: "" }, { in: fullScene() }, ctx))
      .rejects.toThrow("ask a question");
  });

  it("threads detail onto NodeStatus through the evaluator", async () => {
    const ctx = stubCtx({ ask: async () => ({ sql: GEN_SQL }) });
    const doc = mkDoc(
      [
        { id: "a", kind: "load.model", params: { model: "mock" } },
        { id: "q", kind: "table.ask", params: { question: "count by category" } },
      ],
      [["a", "q", "in"]],
    );
    const r = await evaluateGraph(doc, ctx);
    expect(stateOf(r, "q"), msgOf(r, "q")).toBe("ok");
    expect(r.status.get("q")?.detail).toBe(GEN_SQL);
    expect(summaryOf(r, "q")).toBe("4 rows");
  });
});

describe("chart.bar", () => {
  const T: TableValue = {
    columns: ["Type", "count"],
    rows: [["Wall", 8], ["Door", 6], ["Window", "6"], ["Slab", null], ["Beam", "n/a"]],
  };

  it("charts explicit columns, coercing numeric strings and dropping non-numeric rows", async () => {
    const out = await call("chart.bar", { labelColumn: "Type", valueColumn: "count" }, { in: T });
    expect(out.chart).toEqual({
      labels: ["Wall", "Door", "Window"],
      values: [8, 6, 6],
      title: "count",
    });
    expect(out.summary).toBe("3 bars");
    expect(out.value).toBe(T);                                       // sink passes the table through
  });

  it("guesses label = first mostly-string column, value = first mostly-numeric column", async () => {
    const t: TableValue = {
      columns: ["name", "score", "other"],
      rows: [["a", 10, 1], ["b", 20, 2], ["c", 30, 3]],
    };
    const out = await call("chart.bar", {}, { in: t });
    expect(out.chart!.labels).toEqual(["a", "b", "c"]);
    expect(out.chart!.values).toEqual([10, 20, 30]);                 // score, not other
    expect(out.chart!.title).toBe("score");
  });

  it("truncates at 24 bars, keeps input order, and notes it in the summary", async () => {
    const t: TableValue = {
      columns: ["name", "n"],
      rows: Array.from({ length: 57 }, (_, i) => [`row${i}`, i + 1]),
    };
    const out = await call("chart.bar", { labelColumn: "name", valueColumn: "n" }, { in: t });
    expect(out.chart!.values.length).toBe(24);
    expect(out.chart!.labels[0]).toBe("row0");
    expect(out.chart!.labels[23]).toBe("row23");
    expect(out.summary).toBe("24 of 57 rows");
  });

  it("errors when no column matches the heuristic", async () => {
    const allText: TableValue = { columns: ["a", "b"], rows: [["x", "y"]] };
    await expect(call("chart.bar", {}, { in: allText })).rejects.toThrow(/no numeric column/);
    const allNum: TableValue = { columns: ["a", "b"], rows: [[1, 2]] };
    await expect(call("chart.bar", {}, { in: allNum })).rejects.toThrow(/no text column/);
  });

  it("errors on an explicit column the table does not have", async () => {
    await expect(call("chart.bar", { labelColumn: "nope", valueColumn: "count" }, { in: T }))
      .rejects.toThrow(/no column "nope"/);
  });

  it("threads status.chart through the evaluator", async () => {
    const ctx = stubCtx({ ask: async () => ({ sql: "SELECT 1" }) });
    const doc = mkDoc(
      [
        { id: "a", kind: "load.model", params: { model: "mock" } },
        { id: "q", kind: "table.ask", params: { question: "counts" } },
        { id: "c", kind: "chart.bar", params: {} },
      ],
      [["a", "q", "in"], ["q", "c", "in"]],
    );
    const r = await evaluateGraph(doc, ctx);
    expect(stateOf(r, "c"), msgOf(r, "c")).toBe("ok");
    expect(r.status.get("c")?.chart).toEqual({
      labels: ["IfcWall", "IfcDoor", "IfcWindow", "IfcSlab"],
      values: [8, 6, 6, 4],
      title: "n",
    });
  });
});

describe("param defaults from kinds.ts", () => {
  it("resolves load.model to its modelPick default when the param is unset", async () => {
    const loaded: string[] = [];
    const ctx = stubCtx({
      loadModel: async (id) => { loaded.push(id); return mockModel(); },
    });
    const doc = mkDoc([{ id: "a", kind: "load.model" }]);
    const r = await evaluateGraph(doc, ctx);
    expect(stateOf(r, "a"), msgOf(r, "a")).toBe("ok");
    expect(loaded).toEqual(["snowdon"]);
  });

  it("does not override a param the user set explicitly", async () => {
    const loaded: string[] = [];
    const ctx = stubCtx({
      loadModel: async (id) => { loaded.push(id); return mockModel(); },
    });
    const doc = mkDoc([{ id: "a", kind: "load.model", params: { model: "mock" } }]);
    await evaluateGraph(doc, ctx);
    expect(loaded).toEqual(["mock"]);
  });
});
