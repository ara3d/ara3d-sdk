// Shared test fixture: a tiny in-memory ModelData for Track A's harness and Track C's tests.
import type { Cell, ModelData, ParamInfo } from "../contracts";

const TYPES = ["IfcWall", "IfcWall", "IfcWall", "IfcWall", "IfcWall", "IfcWall", "IfcWall", "IfcWall",
  "IfcDoor", "IfcDoor", "IfcDoor", "IfcDoor", "IfcDoor", "IfcDoor",
  "IfcWindow", "IfcWindow", "IfcWindow", "IfcWindow", "IfcWindow", "IfcWindow",
  "IfcSlab", "IfcSlab", "IfcSlab", "IfcSlab"];

export function mockModel(id = "mock"): ModelData {
  const n = TYPES.length;
  const globalIds = Array.from({ length: n }, (_, i) => `GID${String(i + 1).padStart(3, "0")}`);
  const levels = Array.from({ length: n }, (_, i) => (i % 2 === 0 ? "Level 1" : "Level 2"));
  const names = TYPES.map((t, i) => `${t.slice(3)} ${i + 1}`);
  const area: Cell[] = Array.from({ length: n }, (_, i) => 5 + (i * 7) % 40);
  const fire: Cell[] = TYPES.map((t, i) => (t === "IfcWall" ? (i % 3 === 0 ? "2HR" : "1HR") : null));
  const params: Record<string, Cell[]> = { Area: area, FireRating: fire };
  return {
    id,
    entityCount: n,
    globalIds,
    types: [...TYPES],
    names,
    levels,
    paramNames: (): ParamInfo[] => [
      { name: "Area", pset: "Dimensions", numeric: true },
      { name: "FireRating", pset: "Pset_WallCommon", numeric: false },
    ],
    param: (name: string) => params[name] ?? new Array<Cell>(n).fill(null),
  };
}

/** CSV matching the mock model's GlobalIds, for join tests. */
export const mockCarbonCsv =
  "GlobalId,embodied_carbon,category\n" +
  mockModel().globalIds.map((g, i) => `${g},${(i * 13) % 97 + 3},${i % 2 ? "A" : "B"}`).join("\n");
