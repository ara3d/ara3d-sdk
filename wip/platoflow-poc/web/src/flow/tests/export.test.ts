// Wave-9 Track E: sink.exportCsv stays pure at eval time (design §6) — table
// passthrough + readiness summary only; the host POST happens on explicit Run.
import { describe, expect, it } from "vitest";
import type { TableValue } from "../../contracts";
import { evaluateGraph } from "../evaluate";
import { NODES } from "../nodes";
import { NeedsSetup } from "../types";
import { CANNED_SQL, mkDoc, stateOf, stubCtx, summaryOf, tableAt } from "./harness";

const T: TableValue = {
  columns: ["name", "n"],
  rows: [["a", 1], ["b", 2], ["c", null]],
};

const call = async (params: Record<string, unknown>, inputs: Record<string, unknown>) =>
  NODES.get("sink.exportCsv")!(
    { id: "x", kind: "sink.exportCsv", x: 0, y: 0, params },
    inputs as never,
    stubCtx(),
  );

describe("sink.exportCsv", () => {
  it("passes the table through untouched and reports readiness", async () => {
    const out = await call({ filename: "walls.csv" }, { in: T });
    expect(out.value).toBe(T);                       // identity: no copy, no write
    expect(out.summary).toBe("ready: 3 rows → walls.csv");
  });

  it("rejects a non-table input", async () => {
    await expect(call({ filename: "x.csv" }, { in: { ramp: "viridis", min: 0, max: 1 } }))
      .rejects.toThrow(/not a table/);
  });

  it("is needs-setup (not broken) when the filename is empty and no default applied", async () => {
    await expect(call({}, { in: T })).rejects.toThrow(NeedsSetup);
    await expect(call({ filename: "  " }, { in: T })).rejects.toThrow(/choose a filename/);
  });

  it("gets the schema default filename through a full graph evaluation", async () => {
    const doc = mkDoc(
      [
        { id: "m", kind: "load.model", params: { model: "mock" } },
        { id: "t", kind: "table.sql", params: { sql: "SELECT 1" } },
        { id: "x", kind: "sink.exportCsv" },            // filename unset: default wins
      ],
      [["m", "t", "in"], ["t", "x", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "x")).toBe("ok");
    expect(summaryOf(r, "x")).toBe(`ready: ${CANNED_SQL.rows.length} rows → export.csv`);
    expect(tableAt(r, "x")).toEqual(CANNED_SQL);
  });
});
