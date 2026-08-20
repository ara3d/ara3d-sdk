// Every shipped demo graph must parse, match its web/public copy, and evaluate without
// structural errors (unknown kind, missing input, bad wire type) against the mock model.
import { readFileSync, readdirSync } from "node:fs";
import { fileURLToPath } from "node:url";
import { describe, expect, it } from "vitest";
import type { GraphDoc } from "../../contracts";
import { kindInfo } from "../../kinds";
import { evaluateGraph } from "../evaluate";
import { isOptionalInput } from "../lib";
import { mockModel } from "../../fixtures/mockModel";
import { stubCtx, summaryOf } from "./harness";

const dir = (rel: string) => fileURLToPath(new URL(rel, import.meta.url));
const DEMO = dir("../../../../demo/");
const PUBLIC = dir("../../../public/demo/");

const names = readdirSync(DEMO).filter(f => f.endsWith(".json")).map(f => f.replace(/\.json$/, ""));
const read = (root: string, name: string) => readFileSync(`${root}${name}.json`, "utf8");
const doc = (name: string): GraphDoc => JSON.parse(read(DEMO, name)) as GraphDoc;

/** The demos join on operational_carbon (wave 2) and cost (wave 4), so the stub CSV carries both. */
const demoCsv = "GlobalId,operational_carbon,cost\n" +
  mockModel().globalIds.map((g, i) => `${g},${50 + (i * 37) % 400},${100 + (i * 53) % 900}`).join("\n");

describe("demo graphs", () => {
  it("ships the wave-1 and wave-2 demos", () => {
    expect(names).toEqual(expect.arrayContaining(
      ["carbon-walls", "carbon-by-level", "sql-explore", "write-pset",
       "whatif-expr", "inspect-flow", "filter-sort"]));
  });

  it.each(names)("%s: demo/ and web/public/demo/ copies are identical", name => {
    expect(read(PUBLIC, name).trim()).toBe(read(DEMO, name).trim());
  });

  it.each(names)("%s: is a well-formed graph over known kinds", name => {
    const d = doc(name);
    expect(d.name).toBe(name);
    const ids = new Set(d.nodes.map(n => n.id));
    expect(ids.size).toBe(d.nodes.length);
    for (const n of d.nodes) {
      const info = kindInfo(n.kind);
      expect(info, `${name}: unknown kind ${n.kind}`).toBeDefined();
      for (const p of Object.keys(n.params)) {
        expect(Object.keys(info!.params), `${name}/${n.id}: stray param ${p}`).toContain(p);
      }
    }
    for (const e of d.edges) {
      expect(ids).toContain(e.from.node);
      expect(ids).toContain(e.to.node);
      const to = kindInfo(d.nodes.find(n => n.id === e.to.node)!.kind)!;
      const from = kindInfo(d.nodes.find(n => n.id === e.from.node)!.kind)!;
      const inPort = to.inputs.find(p => p.name === e.to.slot);
      const outPort = from.outputs.find(p => p.name === e.from.slot);
      expect(inPort, `${name}: no input ${e.to.slot}`).toBeDefined();
      expect(outPort?.type, `${name}: wire type mismatch into ${e.to.node}`).toBe(inPort!.type);
    }
    // Every declared input is wired — except registry-optional ones (W10:
    // viz.colorBy's colormap override port; see lib.isOptionalInput).
    for (const n of d.nodes) {
      for (const port of kindInfo(n.kind)!.inputs) {
        if (isOptionalInput(n.kind, port.name)) continue;
        const wired = d.edges.some(e => e.to.node === n.id && e.to.slot === port.name);
        expect(wired, `${name}/${n.id}: input ${port.name} not wired`).toBe(true);
      }
    }
    if (d.display !== null) expect(ids).toContain(d.display);
  });

  it.each(names)("%s: evaluates green against the mock model", async name => {
    const r = await evaluateGraph(doc(name), stubCtx({ fetchText: async () => demoCsv }));
    for (const [id, st] of r.status) {
      expect(`${name}/${id}: ${st.state} ${st.message ?? ""}`.trim()).toBe(`${name}/${id}: ok`);
    }
  });

  it("whatif-expr computes an intensity channel and an auto domain", async () => {
    const r = await evaluateGraph(doc("whatif-expr"), stubCtx({ fetchText: async () => demoCsv }));
    expect(summaryOf(r, "n4")).toBe("wrote carbon_intensity: 24 non-null of 24");
    expect(summaryOf(r, "n6")).toMatch(/^24 colored · [\d.]+–[\d.]+$/);
  });

  it("inspect-flow shows the channel appearing between the two view.scene nodes", async () => {
    const r = await evaluateGraph(doc("inspect-flow"), stubCtx({ fetchText: async () => demoCsv }));
    // The mock model has no IfcWallStandardCase, so counts are 0 — the shape is what matters.
    expect(summaryOf(r, "n3")).toMatch(/entities · no channels$/);
    expect(summaryOf(r, "n6")).toMatch(/entities · channels: operational_carbon$/);
  });

  it("filter-sort narrows and orders the rows", async () => {
    const r = await evaluateGraph(doc("filter-sort"), stubCtx({ fetchText: async () => demoCsv }));
    const t = r.values.get("n6") as { columns: string[]; rows: (number | string | null)[][] };
    const ci = t.columns.indexOf("operational_carbon");
    const vals = t.rows.map(row => Number(row[ci]));
    expect(vals.length).toBeGreaterThan(0);
    expect(vals.every(v => v > 100)).toBe(true);
    expect([...vals].sort((a, b) => b - a)).toEqual(vals);
  });
});
