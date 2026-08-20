// Error propagation: unknown kinds, missing inputs, cycles, bad params.
import { describe, expect, it } from "vitest";
import { evaluateGraph } from "../evaluate";
import { applyIntent } from "../../reducer";
import { mkDoc, msgOf, stateOf, stubCtx, summaryOf } from "./harness";

describe("error handling", () => {
  it("poisons everything downstream of an unknown kind", async () => {
    const doc = mkDoc(
      [
        { id: "n1", kind: "load.model", params: { model: "mock" } },
        { id: "n2", kind: "wat.bogus" },
        { id: "n3", kind: "select.byType", params: { type: "IfcWall" } },
        { id: "n4", kind: "table.fromScene" },
      ],
      [["n1", "n2", "in"], ["n2", "n3", "in"], ["n3", "n4", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "n1")).toBe("ok");
    expect(stateOf(r, "n2")).toBe("error");
    expect(msgOf(r, "n2")).toBe('unknown node kind "wat.bogus"');
    expect(stateOf(r, "n3")).toBe("error");
    expect(msgOf(r, "n3")).toBe('upstream error in n2: unknown node kind "wat.bogus"');
    expect(stateOf(r, "n4")).toBe("error");
    // Two hops down, the ROOT (n2) is still named — not the immediate parent n3.
    expect(msgOf(r, "n4")).toBe('upstream error in n2: unknown node kind "wat.bogus"');
    expect(r.values.has("n3")).toBe(false);
  });

  it("reports a missing input after a disconnect", async () => {
    const wired = mkDoc(
      [
        { id: "n1", kind: "load.model", params: { model: "mock" } },
        { id: "n2", kind: "select.byType", params: { type: "IfcWall" } },
      ],
      [["n1", "n2", "in"]],
    );
    expect(stateOf(await evaluateGraph(wired, stubCtx()), "n2")).toBe("ok");

    const cut = applyIntent(wired, { t: "disconnect", to: { node: "n2", slot: "in" } });
    const r = await evaluateGraph(cut, stubCtx());
    expect(stateOf(r, "n1")).toBe("ok");
    // Unwired input = unconfigured, not broken (wave 9).
    expect(stateOf(r, "n2")).toBe("needs-setup");
    expect(msgOf(r, "n2")).toBe('missing input "in"');
  });

  it("reports a half-wired two-input node", async () => {
    const doc = mkDoc(
      [
        { id: "n1", kind: "load.model", params: { model: "mock" } },
        { id: "n2", kind: "attach.column", params: { valueColumn: "embodied_carbon" } },
      ],
      [["n1", "n2", "scene"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "n2")).toBe("needs-setup");
    expect(msgOf(r, "n2")).toBe('missing input "table"');
  });

  it("flags cycles and poisons what hangs off them", async () => {
    const doc = mkDoc(
      [
        { id: "n1", kind: "load.model", params: { model: "mock" } },
        { id: "a", kind: "select.byType", params: { type: "IfcWall" } },
        { id: "b", kind: "select.byLevel", params: { level: "Level 1" } },
        { id: "c", kind: "table.fromScene" },
      ],
      [["n1", "a", "in"], ["b", "a", "in"], ["a", "b", "in"], ["b", "c", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "n1")).toBe("ok");
    expect(msgOf(r, "a")).toBe("cycle detected");
    expect(msgOf(r, "b")).toBe("cycle detected");
    expect(stateOf(r, "c")).toBe("error");
    expect(msgOf(r, "c")).toMatch(/^upstream error in (a|b): cycle detected$/);
  });

  it("treats a self-loop as a cycle", async () => {
    const doc = mkDoc([{ id: "a", kind: "select.byType", params: { type: "IfcWall" } }], [["a", "a", "in"]]);
    const r = await evaluateGraph(doc, stubCtx());
    expect(msgOf(r, "a")).toBe("cycle detected");
  });

  it("reports needs-setup, not error, for the missing param on source nodes", async () => {
    const doc = mkDoc([
      { id: "a", kind: "load.model" },
      { id: "b", kind: "data.csv" },
      { id: "c", kind: "viz.colormap" },
    ]);
    const r = await evaluateGraph(doc, stubCtx());
    expect(stateOf(r, "a")).toBe("ok");                 // modelPick default kicks in
    expect(stateOf(r, "b")).toBe("needs-setup");
    expect(msgOf(r, "b")).toBe("enter a csv url");
    expect(stateOf(r, "c")).toBe("ok");                 // colormap has defaults
    expect(summaryOf(r, "c")).toBe("0–100 viridis");
  });

  it("catches a throwing loadModel", async () => {
    const doc = mkDoc([{ id: "a", kind: "load.model", params: { model: "ghost" } }]);
    const r = await evaluateGraph(doc, stubCtx({
      loadModel: async id => { throw new Error(`no such model: ${id}`); },
    }));
    expect(stateOf(r, "a")).toBe("error");
    expect(msgOf(r, "a")).toBe("no such model: ghost");
  });

  it("errors when a join column is absent from the table", async () => {
    const doc = mkDoc(
      [
        { id: "a", kind: "load.model", params: { model: "mock" } },
        { id: "b", kind: "data.csv", params: { url: "carbon.csv" } },
        { id: "c", kind: "attach.column", params: { valueColumn: "nope" } },
      ],
      [["a", "c", "scene"], ["b", "c", "table"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(msgOf(r, "c")).toBe('table has no column "nope"');
  });

  it("gives every ok node a summary and leaves failed nodes without a value", async () => {
    const doc = mkDoc(
      [
        { id: "a", kind: "load.model", params: { model: "mock" } },
        { id: "b", kind: "select.byType" },                  // no type param
      ],
      [["a", "b", "in"]],
    );
    const r = await evaluateGraph(doc, stubCtx());
    expect(summaryOf(r, "a")).toBe("24 entities");
    expect(stateOf(r, "b")).toBe("needs-setup");
    expect(msgOf(r, "b")).toBe("choose a type");
    expect(r.values.has("b")).toBe(false);
  });
});
