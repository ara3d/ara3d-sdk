// T7 replacement for the retired intgate wave-4 assertions: the analytics
// demos' CHART PAYLOADS, proven against the mock evaluator (demos.test.ts
// already proves every demo evaluates green; these pin the payload shape the
// browser checks used to wait on). Node ids repeat across demos — asserting
// per-demo payloads here is what lets intgate-smoke drop those demo loads.
//
// Lives in editor/tests because tools + tests are T7's fence; the flow suite
// (Track C fence) is the natural long-term home — supervisor may relocate.
import { readFileSync } from "node:fs";
import { fileURLToPath } from "node:url";
import { describe, expect, it } from "vitest";
import type { GraphDoc, NodeStatus } from "../../contracts";
import { evaluateGraph } from "../../flow/evaluate";
import { mockModel } from "../../fixtures/mockModel";
import { stubCtx } from "../../flow/tests/harness";

const demo = (name: string): GraphDoc => JSON.parse(readFileSync(
  fileURLToPath(new URL(`../../../../demo/${name}.json`, import.meta.url)), "utf8"));

/** Same stub CSV rule as demos.test.ts: carry every column a demo joins. */
const demoCsv = "GlobalId,operational_carbon,cost\n" +
  mockModel().globalIds.map((g, i) => `${g},${50 + (i * 37) % 400},${100 + (i * 53) % 900}`).join("\n");

const run = (name: string) => evaluateGraph(demo(name), stubCtx({ fetchText: async () => demoCsv }));
const chartOf = (r: Awaited<ReturnType<typeof run>>, id: string) =>
  (r.status.get(id) as NodeStatus | undefined)?.chart;

describe("wave-4 analytics demos: chart payloads against the mock", () => {
  it("quality-audit: both chart nodes carry labels + values", async () => {
    const r = await run("quality-audit");
    for (const id of ["n4", "n7"]) {
      const c = chartOf(r, id);
      expect(c?.values.length, `${id} chart values`).toBeGreaterThan(0);
      expect(c?.labels.length).toBe(c?.values.length);
    }
  });

  it("cost-estimate: colorBy ok + the per-level chart carries values", async () => {
    const r = await run("cost-estimate");
    expect(r.status.get("n5")?.state).toBe("ok");
    const c = chartOf(r, "n8");
    expect(c?.values.length).toBeGreaterThan(0);
    expect(c?.labels.length).toBe(c?.values.length);
  });

  it("disciplines: all three charts carry values", async () => {
    const r = await run("disciplines");
    for (const id of ["n7", "n8", "n9"]) {
      const c = chartOf(r, id);
      expect(c?.values.length, `${id} chart values`).toBeGreaterThan(0);
    }
  });
});
