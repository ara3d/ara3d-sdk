// Wave 9 — semantic honesty (design §1.4, §3): needs-setup vs error, waiting-on
// propagation, root-cause poison text, dropped-row counts, shadow warnings,
// channel provenance (source/numeric), and per-column CSV typing.
import { describe, expect, it } from "vitest";
import type { SceneValue, TableValue } from "../../contracts";
import { parseCsv } from "../csv";
import { evaluateGraph } from "../evaluate";
import { NODES } from "../nodes";
import { mockModel } from "../../fixtures/mockModel";
import { mkDoc, msgOf, sceneAt, stateOf, stubCtx, warnOf } from "./harness";

const fullScene = (): SceneValue => {
  const model = mockModel();
  return { model, entities: Uint32Array.from(model.globalIds.map((_, i) => i)), channels: {} };
};

const call = async (kind: string, params: Record<string, unknown>, inputs: Record<string, unknown>) =>
  NODES.get(kind)!({ id: "x", kind, x: 0, y: 0, params }, inputs as never, stubCtx());

// ── needs-setup vs error ────────────────────────────────────────────────────

describe("needs-setup vs error", () => {
  const pair = async (kind: string, params: Record<string, unknown>) => {
    const doc = mkDoc(
      [{ id: "a", kind: "load.model", params: { model: "mock" } }, { id: "b", kind, params }],
      [["a", "b", "in"]],
    );
    return evaluateGraph(doc, stubCtx());
  };

  it("unset required params report needs-setup with setup-flavored text", async () => {
    for (const [kind, params, msg] of [
      ["select.byType", {}, "choose a type"],
      ["select.byLevel", {}, "choose a level"],
      ["select.byParameter", {}, "choose a parameter"],
    ] as const) {
      const r = await pair(kind, params);
      expect(stateOf(r, "b"), kind).toBe("needs-setup");
      expect(msgOf(r, "b"), kind).toBe(msg);
    }
    // table.sql carries a schema default, so through the evaluator it never sees an
    // empty param — assert the def's own NeedsSetup directly.
    await expect(call("table.sql", { sql: "" }, { in: fullScene() }))
      .rejects.toThrow("enter a query");
  });

  it("table nodes: unselected column is needs-setup, missing column stays an error", async () => {
    const doc = (kind: string, params: Record<string, unknown>) =>
      mkDoc([{ id: "a", kind: "load.model", params: { model: "mock" } },
             { id: "t", kind: "table.fromScene" }, { id: "b", kind, params }],
            [["a", "t", "in"], ["t", "b", "in"]]);
    const sort = await evaluateGraph(doc("table.sort", {}), stubCtx());
    expect(stateOf(sort, "b")).toBe("needs-setup");
    expect(msgOf(sort, "b")).toBe("choose a sort column");

    const agg = await evaluateGraph(doc("table.aggregate", {}), stubCtx());
    expect(stateOf(agg, "b")).toBe("needs-setup");
    expect(msgOf(agg, "b")).toBe("choose a group-by column");

    const aggNoVal = await evaluateGraph(doc("table.aggregate", { groupBy: "Type", agg: "sum" }), stubCtx());
    expect(stateOf(aggNoVal, "b")).toBe("needs-setup");
    expect(msgOf(aggNoVal, "b")).toBe("choose a value column");

    // count needs no value column — still ok
    const count = await evaluateGraph(doc("table.aggregate", { groupBy: "Type", agg: "count" }), stubCtx());
    expect(stateOf(count, "b")).toBe("ok");

    const missing = await evaluateGraph(doc("table.sort", { column: "nope" }), stubCtx());
    expect(stateOf(missing, "b")).toBe("error");
    expect(msgOf(missing, "b")).toBe('table has no column "nope"');
  });

  it("attach.column without a value column is needs-setup; data errors stay errors", async () => {
    const doc = (valueColumn?: string) => mkDoc(
      [{ id: "a", kind: "load.model", params: { model: "mock" } },
       { id: "b", kind: "data.csv", params: { url: "carbon.csv" } },
       { id: "c", kind: "attach.column", params: valueColumn ? { valueColumn } : {} }],
      [["a", "c", "scene"], ["b", "c", "table"]]);
    const unset = await evaluateGraph(doc(), stubCtx());
    expect(stateOf(unset, "c")).toBe("needs-setup");
    expect(msgOf(unset, "c")).toBe("choose a value column");

    const bad = await evaluateGraph(doc("nope"), stubCtx());
    expect(stateOf(bad, "c")).toBe("error");                  // wrong name = broken, not unset
  });

  it("a wrong parameter NAME is an error, not needs-setup", async () => {
    const r = await pair("select.byParameter", { parameter: "Nope", op: ">", value: "1" });
    expect(stateOf(r, "b")).toBe("error");
    expect(msgOf(r, "b")).toBe('no parameter or channel named "Nope"');
  });
});

// ── poison propagation ──────────────────────────────────────────────────────

describe("poison propagation", () => {
  it("downstream of needs-setup waits on the ROOT node, staying gray", async () => {
    const doc = mkDoc(
      [{ id: "n1", kind: "load.model", params: { model: "mock" } },
       { id: "n2", kind: "select.byType" },                    // unset → needs-setup
       { id: "n3", kind: "table.fromScene" },
       { id: "n4", kind: "sink.table" }],
      [["n1", "n2", "in"], ["n2", "n3", "in"], ["n3", "n4", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "n2")).toBe("needs-setup");
    expect(msgOf(r, "n2")).toBe("choose a type");
    expect(stateOf(r, "n3")).toBe("needs-setup");
    expect(msgOf(r, "n3")).toBe("waiting on n2");
    expect(stateOf(r, "n4")).toBe("needs-setup");
    expect(msgOf(r, "n4")).toBe("waiting on n2");              // root, not n3
  });

  it("downstream of an error names the ROOT id and message three hops down", async () => {
    const doc = mkDoc(
      [{ id: "n1", kind: "load.model", params: { model: "mock" } },
       { id: "n2", kind: "table.sql", params: { sql: "SELECT boom" } },
       { id: "n3", kind: "table.filter", params: { column: "x", op: ">", value: "1" } },
       { id: "n4", kind: "view.table" },
       { id: "n5", kind: "sink.table" }],
      [["n1", "n2", "in"], ["n2", "n3", "in"], ["n3", "n4", "in"], ["n4", "n5", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx({
      sql: async () => { throw new Error("Binder Error: boom"); },
    }));
    expect(stateOf(r, "n2")).toBe("error");
    for (const id of ["n3", "n4", "n5"]) {
      expect(stateOf(r, id)).toBe("error");
      expect(msgOf(r, id)).toBe("upstream error in n2: Binder Error: boom");
    }
  });

  it("an upstream error beats an upstream needs-setup on the same node", async () => {
    const doc = mkDoc(
      [{ id: "a", kind: "load.model", params: { model: "mock" } },
       { id: "s", kind: "select.byType" },                     // needs-setup branch
       { id: "b", kind: "data.csv", params: { url: "x.csv" } }, // error branch
       { id: "c", kind: "attach.column", params: { valueColumn: "v" } }],
      [["a", "s", "in"], ["s", "c", "scene"], ["b", "c", "table"]],
    );
    const r = await evaluateGraph(doc, stubCtx({
      fetchText: async () => { throw new Error("404"); },
    }));
    expect(stateOf(r, "c")).toBe("error");
    expect(msgOf(r, "c")).toBe("upstream error in b: 404");
  });

  it("an unwired input is needs-setup and propagates as waiting", async () => {
    const doc = mkDoc(
      [{ id: "n2", kind: "select.byType", params: { type: "IfcWall" } },
       { id: "n3", kind: "table.fromScene" }],
      [["n2", "n3", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "n2")).toBe("needs-setup");
    expect(msgOf(r, "n2")).toBe('missing input "in"');
    expect(stateOf(r, "n3")).toBe("needs-setup");
    expect(msgOf(r, "n3")).toBe("waiting on n2");
  });
});

// ── dropped-row honesty ─────────────────────────────────────────────────────

describe("dropped-row warnings", () => {
  it("select.byParameter counts non-null non-numeric entities dropped by ordered ops", async () => {
    const doc = mkDoc(
      [{ id: "a", kind: "load.model", params: { model: "mock" } },
       { id: "b", kind: "select.byParameter", params: { parameter: "FireRating", op: ">", value: "1" } }],
      [["a", "b", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "b")).toBe("ok");
    expect(sceneAt(r, "b").entities.length).toBe(0);           // "1HR"/"2HR" are not numbers
    expect(warnOf(r, "b")).toBe("8 entities dropped as non-numeric");  // the 8 walls; 16 nulls not counted
  });

  it("numeric comparisons carry no warning", async () => {
    const doc = mkDoc(
      [{ id: "a", kind: "load.model", params: { model: "mock" } },
       { id: "b", kind: "select.byParameter", params: { parameter: "Area", op: ">", value: "20" } }],
      [["a", "b", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "b")).toBe("ok");
    expect(warnOf(r, "b")).toBeUndefined();
  });

  it("table.filter counts dropped rows and threads the warning onto NodeStatus", async () => {
    const t: TableValue = {
      columns: ["name", "n"],
      rows: [["a", 5], ["b", "n/a"], ["c", null], ["d", "x"], ["e", 2]],
    };
    const out = await call("table.filter", { column: "n", op: ">", value: "1" }, { in: t });
    expect((out.value as TableValue).rows.map(r => r[0])).toEqual(["a", "e"]);
    expect(out.warning).toBe("2 rows dropped as non-numeric"); // "n/a" and "x"; null not counted
  });
});

// ── shadow warnings + channel provenance ────────────────────────────────────

describe("channel shadowing and provenance", () => {
  it("attach.column warns when the channel shadows a model parameter", async () => {
    const csv = "GlobalId,Area\nGID001,7\nGID002,9\n";
    const doc = mkDoc(
      [{ id: "a", kind: "load.model", params: { model: "mock" } },
       { id: "b", kind: "data.csv", params: { url: "areas.csv" } },
       { id: "c", kind: "attach.column", params: { valueColumn: "Area" } }],
      [["a", "c", "scene"], ["b", "c", "table"]],
    );
    const r = await evaluateGraph(doc, stubCtx({ fetchText: async () => csv }));
    expect(stateOf(r, "c")).toBe("ok");
    expect(warnOf(r, "c")).toBe('channel "Area" shadows model parameter');
  });

  it("compute.expr warns on shadow; a fresh name carries no warning", async () => {
    const shadow = await call("compute.expr", { channel: "FireRating", expr: "1" }, { in: fullScene() });
    expect(shadow.warning).toBe('channel "FireRating" shadows model parameter');
    const fresh = await call("compute.expr", { channel: "fresh", expr: "1" }, { in: fullScene() });
    expect(fresh.warning).toBeUndefined();
  });

  it("attach.column records source and infers numeric per channel", async () => {
    const doc = (valueColumn: string) => mkDoc(
      [{ id: "a", kind: "load.model", params: { model: "mock" } },
       { id: "b", kind: "data.csv", params: { url: "data/carbon.csv" } },
       { id: "c", kind: "attach.column", params: { valueColumn } }],
      [["a", "c", "scene"], ["b", "c", "table"]]);
    const num = await evaluateGraph(doc("embodied_carbon"), stubCtx());
    const numChan = sceneAt(num, "c").channels["embodied_carbon"];
    expect(numChan.source).toBe("carbon.csv:embodied_carbon");
    expect(numChan.numeric).toBe(true);

    const txt = await evaluateGraph(doc("category"), stubCtx());
    const txtChan = sceneAt(txt, "c").channels["category"];
    expect(txtChan.source).toBe("carbon.csv:category");
    expect(txtChan.numeric).toBe(false);                       // "A"/"B" are not numbers
  });

  it("compute.expr infers numeric from what it wrote", async () => {
    const num = await call("compute.expr", { channel: "c", expr: "param('Area') * 2" }, { in: fullScene() });
    expect((num.value as SceneValue).channels["c"].numeric).toBe(true);
    expect((num.value as SceneValue).channels["c"].source).toBe("expr");

    const txt = await call("compute.expr", { channel: "c", expr: "type" }, { in: fullScene() });
    expect((txt.value as SceneValue).channels["c"].numeric).toBe(false);
  });
});

// ── per-column CSV typing ───────────────────────────────────────────────────

describe("per-column csv typing", () => {
  it("preserves id-like headers as strings even when all cells look numeric", () => {
    const t = parseCsv("GlobalId,part_id,guid\n001,007,42\n002,008,43\n");
    expect(t.rows[0]).toEqual(["001", "007", "42"]);
    expect(t.rows[1]).toEqual(["002", "008", "43"]);
  });

  it("coerces a fully numeric column, quoted cells included", () => {
    const t = parseCsv('score\n"7"\n8.5\n-2e1\n');
    expect(t.rows.flat()).toEqual([7, 8.5, -20]);
  });

  it("keeps a mixed column entirely as strings", () => {
    const t = parseCsv("v\n7\nseven\n9\n");
    expect(t.rows.flat()).toEqual(["7", "seven", "9"]);
  });

  it("leaves empty cells null and ignores them for inference", () => {
    const t = parseCsv("a,b\n1,\n,x\n3,y\n");
    expect(t.rows).toEqual([[1, null], [null, "x"], [3, "y"]]);
  });
});
