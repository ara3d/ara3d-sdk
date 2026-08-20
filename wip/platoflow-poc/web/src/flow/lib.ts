// Shared node-def helpers: params, cells, input guards, scene ops, ramps, comparison.
// Pure TS: no DOM, no host calls. Every defs-*.ts module builds on these.
import type { Cell, ColormapValue, GraphNode, SceneValue, TableValue, Value, ViewValue } from "../contracts";
import { numericish } from "./csv";
import { isColormap, isScene, isTable, isView } from "./summaries";
import { fail, needsSetup, type NodeInputs } from "./types";

// ---------- param + cell helpers ----------

export const strParam = (n: GraphNode, name: string): string => {
  const v = n.params[name];
  return v === undefined || v === null ? "" : String(v).trim();
};

export const numParam = (n: GraphNode, name: string, dflt: number): number => {
  const v = Number(n.params[name]);
  return Number.isFinite(v) ? v : dflt;
};

export const boolParam = (n: GraphNode, name: string, dflt: boolean): boolean => {
  const v = n.params[name];
  if (typeof v === "boolean") return v;
  if (typeof v === "string") return v === "true";
  return dflt;
};

/** Numeric view of a cell, or null when it is not a finite number. */
export const asNumber = (c: Cell): number | null => {
  if (typeof c === "number") return Number.isFinite(c) ? c : null;
  if (typeof c === "string" && numericish(c)) {
    const n = Number(c.trim());
    return Number.isFinite(n) ? n : null;
  }
  return null;
};

/** Numbers stay numbers, numeric-looking strings are promoted, everything else passes through. */
export const coerceCell = (c: Cell): Cell => {
  const n = asNumber(c);
  return n === null ? c : n;
};

// ---------- input helpers ----------

export const need = (inputs: NodeInputs, slot: string): Value =>
  inputs[slot] ?? fail(`missing input "${slot}"`);

export const sceneIn = (inputs: NodeInputs, slot: string): SceneValue => {
  const v = need(inputs, slot);
  return isScene(v) ? v : fail(`input "${slot}" is not a scene`);
};

export const tableIn = (inputs: NodeInputs, slot: string): TableValue => {
  const v = need(inputs, slot);
  return isTable(v) ? v : fail(`input "${slot}" is not a table`);
};

export const colormapIn = (inputs: NodeInputs, slot: string): ColormapValue => {
  const v = need(inputs, slot);
  return isColormap(v) ? v : fail(`input "${slot}" is not a colormap`);
};

export const viewIn = (inputs: NodeInputs, slot: string): ViewValue => {
  const v = need(inputs, slot);
  return isView(v) ? v : fail(`input "${slot}" is not a view`);
};

/** Same scene, new selection. Spread keeps channels AND groups — selection and
 *  data stay separate (narrowing later never loses data). */
export const derive = (scene: SceneValue, entities: Uint32Array): SceneValue =>
  ({ ...scene, entities });

/**
 * The column a node means by `name`: an overlay channel wins over a model parameter.
 * `param()` hands back an all-null array for names it does not know, so `paramNames()`
 * is the only way to tell "no such parameter" from "parameter that is empty here".
 */
export const columnFor = (scene: SceneValue, name: string): Cell[] | null => {
  const chan = scene.channels[name];
  if (chan) return chan.values;
  if (!scene.model.paramNames().some(p => p.name === name)) return null;
  return scene.model.param(name);
};

export const filterScene = (scene: SceneValue, keep: (i: number) => boolean): Uint32Array => {
  const out: number[] = [];
  for (const i of scene.entities) if (keep(i)) out.push(i);
  return Uint32Array.from(out);
};

// ---------- color ramps ----------

export type RGB = [number, number, number];

const VIRIDIS: RGB[] = [
  [0.267, 0.005, 0.329], [0.275, 0.196, 0.494], [0.212, 0.361, 0.553], [0.153, 0.498, 0.557],
  [0.122, 0.631, 0.531], [0.290, 0.757, 0.427], [0.624, 0.855, 0.227], [0.992, 0.906, 0.145],
];
const HEAT: RGB[] = [[0, 0, 0], [1, 0, 0], [1, 1, 0], [1, 1, 1]];
const GREENRED: RGB[] = [[0, 0.7, 0.15], [1, 0.9, 0], [0.85, 0.1, 0.1]];

const RAMPS: Record<string, RGB[]> = { viridis: VIRIDIS, heat: HEAT, greenred: GREENRED };

const lerp = (a: number, b: number, t: number) => a + (b - a) * t;

/** Sample a named ramp at t in [0,1]; unknown names fall back to viridis. */
export function ramp(name: string, t: number): RGB {
  const stops = RAMPS[name] ?? VIRIDIS;
  const x = Math.min(1, Math.max(0, Number.isFinite(t) ? t : 0)) * (stops.length - 1);
  const i = Math.min(stops.length - 2, Math.floor(x));
  const f = x - i;
  const a = stops[i], b = stops[i + 1];
  return [lerp(a[0], b[0], f), lerp(a[1], b[1], f), lerp(a[2], b[2], f)];
}

/** Domain endpoints read best short: integers bare, fractions to two places. */
export const fmtNum = (n: number): string =>
  Number.isInteger(n) ? String(n) : String(Number(n.toFixed(2)));

// ---------- pset rows (used by integration when the Run button fires) ----------

export function buildPsetRows(
  scene: SceneValue,
  channels: string[],
): { globalId: string; props: Record<string, number | string> }[] {
  const out: { globalId: string; props: Record<string, number | string> }[] = [];
  for (const i of scene.entities) {
    const props: Record<string, number | string> = {};
    let any = false;
    for (const name of channels) {
      const v = columnFor(scene, name)?.[i] ?? null;
      if (v === null || v === "") continue;
      props[name] = v;
      any = true;
    }
    if (any) out.push({ globalId: scene.model.globalIds[i], props });
  }
  return out;
}

export const channelList = (n: GraphNode, scene: SceneValue): string[] => {
  const raw = strParam(n, "channels");
  const named = raw.split(",").map(s => s.trim()).filter(Boolean);
  return named.length > 0 ? named : Object.keys(scene.channels);
};

// ---------- comparison, shared by select.byParameter and table.filter ----------

export type Op = "==" | "!=" | ">" | ">=" | "<" | "<=" | "contains" | "exists";

/** null means "no value": it fails every test except `!=` and (trivially) not-exists.
 *  `onDrop` (wave 9 honesty) fires when an ordered op drops a NON-NULL cell because it
 *  is not numeric — the caller counts those and surfaces "N dropped as non-numeric". */
export function compare(cell: Cell, op: Op, raw: string, onDrop?: () => void): boolean {
  if (op === "exists") return cell !== null && cell !== "";
  if (cell === null) return op === "!=";
  if (op === "contains") return String(cell).toLowerCase().includes(raw.toLowerCase());

  const a = asNumber(cell);
  const b = numericish(raw) ? Number(raw.trim()) : null;

  if (op === "==" || op === "!=") {
    const eq = a !== null && b !== null ? a === b : String(cell) === raw;
    return op === "==" ? eq : !eq;
  }
  if (a === null || b === null) {                  // ordered ops drop non-numeric rows
    if (a === null) onDrop?.();                    // cell is non-null here; count the drop
    return false;
  }
  switch (op) {
    case ">": return a > b;
    case ">=": return a >= b;
    case "<": return a < b;
    case "<=": return a <= b;
    default: return false;
  }
}

// ---------- optional inputs (wave 10) ----------

/** Slots the evaluator treats as OPTIONAL: an unwired one is not needs-setup; the
 *  def decides what absence means (viz.colorBy falls back to its embedded ramp).
 *  Every other declared input stays required — this set is the single source of
 *  truth the evaluator consults. */
const OPTIONAL_INPUTS = new Set(["viz.colorBy|colormap"]);

export const isOptionalInput = (kind: string, slot: string): boolean =>
  OPTIONAL_INPUTS.has(`${kind}|${slot}`);

// ---------- shared labels + ordering (wave 11) ----------

/** Sentinel label for the null/empty group everywhere a per-entity value is
 *  displayed as a string: the categorical legend already renders "(none)";
 *  select.checklist uses the same string for the unnamed level so the two
 *  organs agree on what "no value" is called. */
export const UNNAMED_GROUP = "(none)";

const collator = new Intl.Collator(undefined, { numeric: true, sensitivity: "base" });

/** Numeric-aware string order ("Level 2" < "Level 10"): level ordering in
 *  viz.explode and checklist tie-breaks. */
export const naturalCompare = (a: string, b: string): number => collator.compare(a, b);

// ---------- IFC type matching ----------

export const normType = (s: string): string => {
  const t = s.trim().toLowerCase();
  return t.startsWith("ifc") ? t.slice(3) : t;
};

/** The index of a required column. An UNSET name is needs-setup ("choose a … column");
 *  a name the table does not have is a real data error. */
export const columnIndex = (table: TableValue, name: string, what: string): number => {
  if (!name) needsSetup(`choose a ${what} column`);
  const i = table.columns.indexOf(name);
  return i < 0 ? fail(`table has no column "${name}"`) : i;
};

/**
 * Column-guess heuristic (chart.bar): when a column param is unset, labels come from
 * the first column whose non-null cells are mostly (>50%) non-numeric, and values from
 * the first column whose non-null cells are mostly numeric (asNumber succeeds — numbers
 * or numeric-looking strings). Empty columns match neither guess.
 */
export const guessColumn = (table: TableValue, want: "text" | "numeric"): string | null => {
  for (let c = 0; c < table.columns.length; c++) {
    let numeric = 0, filled = 0;
    for (const row of table.rows) {
      const cell = row[c] ?? null;
      if (cell === null || cell === "") continue;
      filled++;
      if (asNumber(cell) !== null) numeric++;
    }
    if (filled === 0) continue;
    const mostlyNumeric = numeric * 2 > filled;
    if (want === "numeric" ? mostlyNumeric : !mostlyNumeric) return table.columns[c];
  }
  return null;
};
