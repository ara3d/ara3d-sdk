// select.byParameter operator table + channel-over-parameter precedence.
import { describe, expect, it } from "vitest";
import type { Cell, GraphNode, SceneValue } from "../../contracts";
import { evaluateGraph } from "../evaluate";
import { NODES } from "../nodes";
import { mockModel } from "../../fixtures/mockModel";
import { mkDoc, msgOf, sceneAt, stateOf, stubCtx } from "./harness";

const run = async (parameter: string, op: string, value: string) => {
  const doc = mkDoc(
    [
      { id: "a", kind: "load.model", params: { model: "mock" } },
      { id: "b", kind: "select.byParameter", params: { parameter, op, value } },
    ],
    [["a", "b", "in"]],
  );
  return sceneAt(await evaluateGraph(doc, stubCtx()), "b").entities.length;
};

const area = mockModel().param("Area").map(Number);
const fire = mockModel().param("FireRating");
const countArea = (p: (n: number) => boolean) => area.filter(p).length;

describe("select.byParameter", () => {
  it("orders numerics", async () => {
    expect(await run("Area", ">", "20")).toBe(countArea(a => a > 20));
    expect(await run("Area", ">=", "26")).toBe(countArea(a => a >= 26));
    expect(await run("Area", "<", "12")).toBe(countArea(a => a < 12));
    expect(await run("Area", "<=", "12")).toBe(countArea(a => a <= 12));
    expect(await run("Area", ">", "20")).toBeGreaterThan(0);
  });

  it("compares equality numerically when both sides are numbers", async () => {
    expect(await run("Area", "==", "5")).toBe(countArea(a => a === 5));
    expect(await run("Area", "!=", "5")).toBe(countArea(a => a !== 5));
    // "5.0" is the same number, so a numeric compare must match where a string compare would not.
    expect(await run("Area", "==", "5.0")).toBe(countArea(a => a === 5));
  });

  it("compares strings exactly, and contains case-insensitively", async () => {
    const twoHr = fire.filter(f => f === "2HR").length;
    const oneHr = fire.filter(f => f === "1HR").length;
    expect(await run("FireRating", "==", "1HR")).toBe(oneHr);
    expect(await run("FireRating", "contains", "2h")).toBe(twoHr);
    expect(await run("FireRating", "contains", "hr")).toBe(twoHr + oneHr);
  });

  it("treats null as absent: exists is false, != is true, ordered ops drop", async () => {
    const rated = fire.filter(f => f !== null).length;
    expect(await run("FireRating", "exists", "")).toBe(rated);
    expect(await run("FireRating", "!=", "1HR")).toBe(mockModel().entityCount - fire.filter(f => f === "1HR").length);
    expect(await run("FireRating", ">", "1")).toBe(0);          // "1HR" is not numeric → dropped
  });

  it("errors on a parameter that is neither channel nor model column", async () => {
    const doc = mkDoc(
      [
        { id: "a", kind: "load.model", params: { model: "mock" } },
        { id: "b", kind: "select.byParameter", params: { parameter: "Nope", op: ">", value: "1" } },
      ],
      [["a", "b", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "b")).toBe("error");
    expect(msgOf(r, "b")).toContain("Nope");
  });

  it("lets an overlay channel shadow a model parameter of the same name", async () => {
    const model = mockModel();
    const boosted: Cell[] = model.globalIds.map((_, i) => (i < 3 ? 999 : null));
    const scene: SceneValue = {
      model,
      entities: Uint32Array.from(model.globalIds.map((_, i) => i)),
      channels: { Area: { values: boosted } },
    };
    const node: GraphNode = {
      id: "x", kind: "select.byParameter", x: 0, y: 0,
      params: { parameter: "Area", op: ">", value: "100" },
    };
    const out = await NODES.get("select.byParameter")!(node, { in: scene }, stubCtx());
    expect((out.value as SceneValue).entities.length).toBe(3);   // channel wins; raw Area maxes at 44
  });

  it("passes channels through untouched", async () => {
    const model = mockModel();
    const scene: SceneValue = {
      model,
      entities: Uint32Array.from(model.globalIds.map((_, i) => i)),
      channels: { carbon: { values: model.globalIds.map((_, i) => i) } },
    };
    const node: GraphNode = { id: "x", kind: "select.byType", x: 0, y: 0, params: { type: "IfcDoor" } };
    const out = await NODES.get("select.byType")!(node, { in: scene }, stubCtx());
    const s = out.value as SceneValue;
    expect(s.entities.length).toBe(6);
    expect(s.channels["carbon"]).toBe(scene.channels["carbon"]);   // same array, untouched
  });

  it("filters by level", async () => {
    const doc = mkDoc(
      [
        { id: "a", kind: "load.model", params: { model: "mock" } },
        { id: "b", kind: "select.byLevel", params: { level: "Level 2" } },
      ],
      [["a", "b", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(sceneAt(r, "b").entities.length).toBe(mockModel().levels.filter(l => l === "Level 2").length);
  });
});
