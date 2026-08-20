// Wave-10 defs: table.columns / table.stats / check.rule. (viz.colorBy's embedded
// ramp lives in defs-viz.ts.) Pure TS, follows the W9 honesty conventions: unset
// required params → needs-setup; recoverable data oddities → warning, never error;
// `source` provenance passes through transforms.
import type { Cell, TableValue } from "../contracts";
import { def } from "./registry";
import { asNumber, columnIndex, compare, strParam, tableIn, type Op } from "./lib";
import { needsSetup, type NodeOut } from "./types";

const plural = (n: number, word: string): string => `${n} ${word}${n === 1 ? "" : "s"}`;

/**
 * Column projection. Name matching: exact (case-sensitive) first; when that misses,
 * the FIRST case-insensitive match — SQL results and CSVs disagree on casing often
 * enough that a strict miss would be pure friction, but an exact hit must never be
 * stolen by a case-folded near-name. Duplicates in the list collapse to the first
 * mention. keep = listed order; drop = original order minus the listed set.
 * Unknown names are a warning ("N columns not found: …"), never an error — the
 * projection you asked for minus the impossible parts is still useful output.
 */
def("table.columns", async (n, inputs) => {
  const table = tableIn(inputs, "in");
  const names = strParam(n, "columns").split(",").map(s => s.trim()).filter(Boolean);
  if (names.length === 0) needsSetup("list the columns");
  const mode = strParam(n, "mode") || "keep";

  const resolve = (name: string): number => {
    const exact = table.columns.indexOf(name);
    if (exact >= 0) return exact;
    const lower = name.toLowerCase();
    return table.columns.findIndex(c => c.toLowerCase() === lower);
  };

  const hits: number[] = [];
  const missing: string[] = [];
  for (const name of names) {
    const i = resolve(name);
    if (i < 0) missing.push(name);
    else if (!hits.includes(i)) hits.push(i);
  }

  const listed = new Set(hits);
  const keep = mode === "drop"
    ? table.columns.map((_, i) => i).filter(i => !listed.has(i))
    : hits;

  const value: TableValue = {
    columns: keep.map(i => table.columns[i]),
    rows: table.rows.map(r => keep.map(i => r[i] ?? null)),
    source: table.source,
  };
  const out: NodeOut = { value };
  if (missing.length > 0)
    out.warning = `${plural(missing.length, "column")} not found: ${missing.join(", ")}`;
  return out;
});

/**
 * Per-numeric-column summary. A column counts as numeric by the W9 majority rule
 * (guessColumn / colorBy auto-tiebreak): over its NON-EMPTY cells, asNumber must
 * succeed on more than half. Within a numeric column, the non-numeric minority is
 * dropped from the stats and reported in the warning. Mean rounds to 3 decimals
 * (Number(x.toFixed(3))): division is the only op here that manufactures noise
 * digits — count/min/max/sum stay exact. Zero numeric columns is an OK empty
 * result with a warning, not an error: "nothing to summarize" is an answer.
 */
def("table.stats", async (_n, inputs) => {
  const table = tableIn(inputs, "in");
  const columns = ["column", "count", "min", "max", "mean", "sum"];
  const rows: Cell[][] = [];
  const drops: string[] = [];

  for (let c = 0; c < table.columns.length; c++) {
    let filled = 0, count = 0, min = Infinity, max = -Infinity, sum = 0;
    for (const row of table.rows) {
      const cell = row[c] ?? null;
      if (cell === null || cell === "") continue;
      filled++;
      const v = asNumber(cell);
      if (v === null) continue;
      count++; sum += v;
      if (v < min) min = v;
      if (v > max) max = v;
    }
    if (count * 2 <= filled || count === 0) continue;   // not a (majority-)numeric column
    if (filled - count > 0) drops.push(`${table.columns[c]}: ${filled - count}`);
    rows.push([table.columns[c], count, min, max, Number((sum / count).toFixed(3)), sum]);
  }

  const value: TableValue = { columns, rows, source: table.source };
  const out: NodeOut = {
    value,
    summary: `${plural(rows.length, "numeric column")} of ${table.columns.length}`,
  };
  if (rows.length === 0) out.warning = "no numeric columns";
  else if (drops.length > 0) out.warning = `non-numeric cells ignored — ${drops.join(", ")}`;
  return out;
});

/**
 * Row assertion, sharing table.filter's comparison table verbatim (lib.compare):
 * same null semantics, same numeric coercion. Violations are the rows FAILING the
 * condition — which deliberately includes rows whose cell cannot take an ordered
 * comparison (null or non-numeric): a row that cannot satisfy "cost >= 100" has
 * not satisfied it. Verdict rides the summary; on FAIL the warning names the first
 * violating row by its key cell (GlobalId column when present, else first cell).
 * Output provenance is "check" so downstream nodes can say where the rows came from.
 */
def("check.rule", async (n, inputs) => {
  const table = tableIn(inputs, "in");
  const ci = columnIndex(table, strParam(n, "column"), "rule");
  const op = (strParam(n, "op") || ">=") as Op;
  const raw = strParam(n, "value");

  const violations = table.rows.filter(r => !compare(r[ci] ?? null, op, raw));
  const total = table.rows.length;
  const value: TableValue = { columns: [...table.columns], rows: violations, source: "check" };

  if (violations.length === 0)
    return { value, summary: `PASS · ${plural(total, "row")} checked` };

  const gi = table.columns.indexOf("GlobalId");
  const key = violations[0][gi >= 0 ? gi : 0] ?? null;
  return {
    value,
    summary: `FAIL · ${violations.length} of ${total} rows violate`,
    warning: `first violation: ${key === null ? "(no key)" : String(key)}`,
  };
});
