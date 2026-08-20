// Beat 1: load → select walls → csv → attach column → colormap → colorBy.
import { describe, expect, it } from "vitest";
import { evaluateGraph } from "../evaluate";
import { ramp } from "../nodes";
import { mockCarbonCsv, mockModel } from "../../fixtures/mockModel";
import {
  carbonByGid, mkDoc, msgOf, sceneAt, stateOf, stubCtx, summaryOf, tableAt, viewAt,
} from "./harness";

const chain = (typeParam: string, csv?: string) => ({
  doc: mkDoc(
    [
      { id: "n1", kind: "load.model", params: { model: "mock" } },
      { id: "n2", kind: "select.byType", params: { type: typeParam } },
      { id: "n3", kind: "data.csv", params: { url: "/api/file?path=carbon.csv" } },
      { id: "n4", kind: "attach.column", params: { keyColumn: "GlobalId", valueColumn: "embodied_carbon" } },
      // auto off: this chain asserts an explicit 0–100 domain (see the ramp checks below).
      { id: "n5", kind: "viz.colormap", params: { ramp: "viridis", auto: false, min: 0, max: 100 } },
      { id: "n6", kind: "viz.colorBy", params: { channel: "embodied_carbon", ghostOthers: true } },
    ],
    [["n1", "n2", "in"], ["n2", "n4", "scene"], ["n3", "n4", "table"],
     ["n4", "n6", "scene"], ["n5", "n6", "colormap"]],
  ),
  ctx: stubCtx(csv === undefined ? {} : { fetchText: async () => csv }),
});

describe("beat 1 — csv join and colorBy", () => {
  it("runs the whole chain green", async () => {
    const { doc, ctx } = chain("IfcWall");
    const r = await evaluateGraph(doc, ctx);
    for (const id of ["n1", "n2", "n3", "n4", "n5", "n6"]) {
      expect(`${id}:${stateOf(r, id)}`, msgOf(r, id)).toBe(`${id}:ok`);
    }
  });

  it("loads all entities then narrows to walls", async () => {
    const { doc, ctx } = chain("IfcWall");
    const r = await evaluateGraph(doc, ctx);
    expect(sceneAt(r, "n1").entities.length).toBe(mockModel().entityCount);
    expect(sceneAt(r, "n2").entities.length).toBe(8);
    expect([...sceneAt(r, "n2").entities]).toEqual([0, 1, 2, 3, 4, 5, 6, 7]);
    expect(summaryOf(r, "n2")).toBe("8 entities");
  });

  it("matches types case-insensitively and with or without the Ifc prefix", async () => {
    for (const t of ["IfcWall", "ifcwall", "Wall", "WALL", " wall "]) {
      const { doc, ctx } = chain(t);
      const r = await evaluateGraph(doc, ctx);
      expect(sceneAt(r, "n2").entities.length, t).toBe(8);
    }
  });

  it("parses the csv into a table", async () => {
    const { doc, ctx } = chain("IfcWall");
    const r = await evaluateGraph(doc, ctx);
    const t = tableAt(r, "n3");
    expect(t.columns).toEqual(["GlobalId", "embodied_carbon", "category"]);
    expect(t.rows.length).toBe(24);
    expect(t.rows[0]).toEqual(["GID001", 3, "B"]);
    expect(typeof t.rows[0][1]).toBe("number");
    expect(summaryOf(r, "n3")).toBe("24 rows");
  });

  it("joins the column into a full-length channel and reports matched N of M", async () => {
    const { doc, ctx } = chain("IfcWall");
    const r = await evaluateGraph(doc, ctx);
    const s = sceneAt(r, "n4");
    const chan = s.channels["embodied_carbon"].values;
    expect(summaryOf(r, "n4")).toBe("matched 8 of 8");
    expect(chan.length).toBe(mockModel().entityCount);   // full length, sparse
    const expected = carbonByGid();
    for (const i of s.entities) expect(chan[i]).toBe(expected.get(mockModel().globalIds[i]));
    expect(chan[10]).toBe(null);                          // outside the selection
    expect(s.entities.length).toBe(8);                    // join does not change selection
  });

  it("colors one rgb triplet per selected entity", async () => {
    const { doc, ctx } = chain("IfcWall");
    const r = await evaluateGraph(doc, ctx);
    const v = viewAt(r, "n6");
    expect(v.colors!.length).toBe(8 * 3);
    expect(v.entities.length).toBe(8);
    expect(v.ghostOthers).toBe(true);
    expect(v.label).toBe("embodied_carbon");
    expect(summaryOf(r, "n6")).toBe("8 colored · 0–100");
    for (const c of v.colors!) expect(c).toBeGreaterThanOrEqual(0), expect(c).toBeLessThanOrEqual(1);
    // GID001 carbon = 3 with min 0 max 100 → very low on the ramp
    const lo = ramp("viridis", 0.03);
    expect(v.colors![0]).toBeCloseTo(lo[0], 5);
    expect(v.colors![1]).toBeCloseTo(lo[1], 5);
    expect(v.colors![2]).toBeCloseTo(lo[2], 5);
  });

  it("greys entities with no value and keeps them in the view", async () => {
    // CSV covering only the first four GlobalIds: walls 4..7 stay null.
    const partial = mockCarbonCsv.split("\n").slice(0, 5).join("\n");
    const { doc, ctx } = chain("IfcWall", partial);
    const r = await evaluateGraph(doc, ctx);
    expect(summaryOf(r, "n4")).toBe("matched 4 of 8");
    const v = viewAt(r, "n6");
    expect(v.entities.length).toBe(8);
    for (let k = 4; k < 8; k++) {
      expect(v.colors![k * 3]).toBeCloseTo(0.35, 6);
      expect(v.colors![k * 3 + 1]).toBeCloseTo(0.35, 6);
      expect(v.colors![k * 3 + 2]).toBeCloseTo(0.35, 6);
    }
    expect(v.colors![0]).not.toBeCloseTo(0.35, 3);
  });

  it("clamps values outside the colormap range", async () => {
    const doc = mkDoc(
      [
        { id: "a", kind: "load.model", params: { model: "mock" } },
        { id: "b", kind: "viz.colormap", params: { ramp: "heat", auto: false, min: 0, max: 10 } },
        { id: "c", kind: "viz.colorBy", params: { channel: "Area", ghostOthers: false } },
      ],
      [["a", "c", "scene"], ["b", "c", "colormap"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    const v = viewAt(r, "c");
    expect(v.ghostOthers).toBe(false);
    // Area is 5..44; everything above 10 clamps to the top of the heat ramp (white).
    const top = ramp("heat", 1);
    expect(top).toEqual([1, 1, 1]);
    const big = [...mockModel().param("Area")].findIndex(a => Number(a) > 10);
    expect(v.colors![big * 3]).toBeCloseTo(1, 6);
  });
});
