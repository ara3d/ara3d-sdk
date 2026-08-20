// CSV parsing, ramps, and the pset row builder / write sink.
import { describe, expect, it } from "vitest";
import type { Cell, SceneValue } from "../../contracts";
import { parseCsv } from "../csv";
import { buildPsetRows, ramp } from "../nodes";
import { evaluateGraph } from "../evaluate";
import { mockModel } from "../../fixtures/mockModel";
import { mkDoc, stubCtx, summaryOf, tableAt } from "./harness";

describe("parseCsv", () => {
  it("handles quotes, embedded commas, escaped quotes and CRLF", () => {
    const t = parseCsv('a,b,c\r\n1,"x,y","he said ""hi"""\r\n2,z,w\r\n');
    expect(t.columns).toEqual(["a", "b", "c"]);
    expect(t.rows).toEqual([[1, "x,y", 'he said "hi"'], [2, "z", "w"]]);
  });

  it("types columns as a whole: all-numeric columns coerce regardless of quoting", () => {
    // Wave 9: quoting no longer decides types. "code" is not id-like and every cell
    // is numeric-looking, so the whole column coerces — including the quoted "007".
    const t = parseCsv('n,code\n3.5,"007"\n-2e3,08\n');
    expect(t.rows[0]).toEqual([3.5, 7]);
    expect(t.rows[1]).toEqual([-2000, 8]);
  });

  it("keeps id-like columns as strings even when every cell looks numeric", () => {
    const t = parseCsv("part_id,GlobalId,n\n007,101,1\n008,102,2\n");
    expect(t.rows[0]).toEqual(["007", "101", 1]);
    expect(t.rows[1]).toEqual(["008", "102", 2]);
  });

  it("keeps a mixed column entirely as strings", () => {
    const t = parseCsv("v\n7\nx\n9\n");
    expect(t.rows.flat()).toEqual(["7", "x", "9"]);
  });

  it("pads ragged rows and drops blank lines", () => {
    const t = parseCsv("a,b,c\n1\n\n2,3,4\n");
    expect(t.rows).toEqual([[1, null, null], [2, 3, 4]]);
  });

  it("returns an empty table for empty text", () => {
    expect(parseCsv("")).toEqual({ columns: [], rows: [] });
  });
});

describe("ramp", () => {
  it("hits the documented endpoints", () => {
    expect(ramp("heat", 0)).toEqual([0, 0, 0]);
    expect(ramp("heat", 1 / 3)).toEqual([1, 0, 0]);
    expect(ramp("heat", 2 / 3).map(v => Math.round(v))).toEqual([1, 1, 0]);
    expect(ramp("heat", 1)).toEqual([1, 1, 1]);
    const [gr, gg] = ramp("greenred", 0);
    expect(gg).toBeGreaterThan(gr);
    const [rr, rg] = ramp("greenred", 1);
    expect(rr).toBeGreaterThan(rg);
  });

  it("clamps out-of-range and non-finite inputs, always returning 0..1 triplets", () => {
    for (const t of [-5, 0, 0.5, 1, 42, NaN]) {
      const c = ramp("viridis", t);
      expect(c.length).toBe(3);
      for (const v of c) { expect(v).toBeGreaterThanOrEqual(0); expect(v).toBeLessThanOrEqual(1); }
    }
    expect(ramp("viridis", -1)).toEqual(ramp("viridis", 0));
    expect(ramp("viridis", 2)).toEqual(ramp("viridis", 1));
  });

  it("falls back to viridis for an unknown ramp name", () => {
    expect(ramp("nope", 0.5)).toEqual(ramp("viridis", 0.5));
  });
});

describe("pset rows", () => {
  const scene = (channels: Record<string, Cell[]>, count = 4): SceneValue => {
    const model = mockModel();
    const wrapped = Object.fromEntries(Object.entries(channels).map(([k, v]) => [k, { values: v }]));
    return { model, entities: Uint32Array.from({ length: count }, (_, i) => i), channels: wrapped };
  };

  it("skips entities whose channels are all empty", () => {
    const rows = buildPsetRows(scene({ carbon: [10, null, 30, null], grade: [null, null, "A", ""] }), ["carbon", "grade"]);
    expect(rows).toEqual([
      { globalId: "GID001", props: { carbon: 10 } },
      { globalId: "GID003", props: { carbon: 30, grade: "A" } },
    ]);
  });

  it("ignores channels that do not exist", () => {
    expect(buildPsetRows(scene({ carbon: [1, null, null, null] }), ["ghost"])).toEqual([]);
  });

  it("sink.writePset reports what a run would write without doing it", async () => {
    let called = 0;
    const doc = mkDoc(
      [
        { id: "a", kind: "load.model", params: { model: "mock" } },
        { id: "b", kind: "data.csv", params: { url: "carbon.csv" } },
        { id: "c", kind: "attach.column", params: { valueColumn: "embodied_carbon" } },
        { id: "d", kind: "sink.writePset", params: { psetName: "Ara3D_Analytics", channels: "embodied_carbon" } },
      ],
      [["a", "c", "scene"], ["b", "c", "table"], ["c", "d", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx({
      fetchText: async () => { called++; return "GlobalId,embodied_carbon\nGID001,7\nGID002,9\n"; },
    }));
    expect(called).toBe(1);
    expect(summaryOf(r, "d")).toBe("ready: 2 entities × channels [embodied_carbon]");
    // the write sink passes its scene through so downstream/debug views still work
    expect((r.values.get("d") as SceneValue).entities.length).toBe(mockModel().entityCount);
  });

  it("defaults the channel list to every channel on the scene", async () => {
    const doc = mkDoc(
      [
        { id: "a", kind: "load.model", params: { model: "mock" } },
        { id: "b", kind: "data.csv", params: { url: "carbon.csv" } },
        { id: "c", kind: "attach.column", params: { valueColumn: "embodied_carbon" } },
        { id: "d", kind: "sink.writePset" },
      ],
      [["a", "c", "scene"], ["b", "c", "table"], ["c", "d", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(summaryOf(r, "d")).toBe("ready: 24 entities × channels [embodied_carbon]");
  });

  it("keeps table.fromScene column order stable across channels", async () => {
    const doc = mkDoc(
      [
        { id: "a", kind: "load.model", params: { model: "mock" } },
        { id: "b", kind: "data.csv", params: { url: "carbon.csv" } },
        { id: "c", kind: "attach.column", params: { valueColumn: "embodied_carbon" } },
        { id: "d", kind: "attach.column", params: { valueColumn: "category" } },
        { id: "e", kind: "table.fromScene" },
      ],
      [["a", "c", "scene"], ["b", "c", "table"], ["c", "d", "scene"], ["b", "d", "table"], ["d", "e", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(tableAt(r, "e").columns).toEqual(["GlobalId", "Type", "Name", "Level", "embodied_carbon", "category"]);
  });
});
