// Shared test scaffolding: stub EvalCtx over the mock model + tiny graph builder.
import type {
  EvalCtx, EvalResult, GraphDoc, SceneValue, TableValue, ViewValue,
} from "../../contracts";
import { mockCarbonCsv, mockModel } from "../../fixtures/mockModel";

export const CANNED_SQL: TableValue = {
  columns: ["Type", "n"],
  rows: [["IfcWall", 8], ["IfcDoor", 6], ["IfcWindow", 6], ["IfcSlab", 4]],
};

export function stubCtx(over: Partial<EvalCtx> = {}): EvalCtx {
  return {
    loadModel: async () => mockModel(),
    sql: async () => CANNED_SQL,
    fetchText: async () => mockCarbonCsv,
    listModels: async () => [{ id: "mock", name: "Mock" }],
    ...over,
  };
}

export interface NodeSpec { id: string; kind: string; params?: Record<string, unknown>; }
/** [fromNode, toNode, toSlot] — every kind has a single output named "out". */
export type Wire = [string, string, string];

export const mkDoc = (nodes: NodeSpec[], wires: Wire[] = []): GraphDoc => ({
  name: "test",
  nodes: nodes.map((n, i) => ({ id: n.id, kind: n.kind, params: n.params ?? {}, x: i * 160, y: 0 })),
  edges: wires.map(([f, t, slot]) => ({ from: { node: f, slot: "out" }, to: { node: t, slot } })),
  display: null,
});

export const sceneAt = (r: EvalResult, id: string): SceneValue => r.values.get(id) as SceneValue;
export const tableAt = (r: EvalResult, id: string): TableValue => r.values.get(id) as TableValue;
export const viewAt = (r: EvalResult, id: string): ViewValue => r.values.get(id) as ViewValue;
export const stateOf = (r: EvalResult, id: string) => r.status.get(id)?.state;
export const msgOf = (r: EvalResult, id: string) => r.status.get(id)?.message ?? "";
export const summaryOf = (r: EvalResult, id: string) => r.status.get(id)?.summary ?? "";
export const warnOf = (r: EvalResult, id: string) => r.status.get(id)?.warning;

/** CSV rows keyed by GlobalId, parsed independently of the code under test. */
export const carbonByGid = (csv = mockCarbonCsv): Map<string, number> => {
  const lines = csv.trim().split("\n").slice(1);
  return new Map(lines.map(l => {
    const [gid, carbon] = l.split(",");
    return [gid, Number(carbon)] as const;
  }));
};
