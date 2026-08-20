// Core node semantics: sources, selects, data, tables, views, sinks, subgraph.
// (viz.* lives in defs-viz.ts; set algebra + grouping in defs-sets.ts; exports in
// defs-export.ts — wave-9 split, see CONTRACTS.md fences.)
// Pure TS: no DOM, no host calls except through EvalCtx.
import type {
  Cell, EvalCtx, GraphDoc, GraphNode, SceneValue, TableValue, Value,
} from "../contracts";
import { evaluateGraphSeeded, valueFrom, type InputSeed } from "./evaluate";
import { parseCsv } from "./csv";
import { def } from "./registry";
import {
  asNumber, boolParam, buildPsetRows, channelList, coerceCell, columnFor, columnIndex,
  compare, derive, filterScene, guessColumn, normType, sceneIn, strParam, tableIn,
  type Op,
} from "./lib";
import { fail, needsSetup, type NodeInputs, type NodeOut } from "./types";

// ---------- wave-9 honesty helpers (local: only defs-core creates channels) ----------

/** Amber note when a new channel hides a model parameter of the same name (design §1.4). */
const shadowWarning = (scene: SceneValue, channelName: string): string | undefined =>
  scene.model.paramNames().some(p => p.name === channelName)
    ? `channel "${channelName}" shadows model parameter`
    : undefined;

/** Multiple warnings on one node join with "; ". */
const joinWarnings = (...ws: (string | undefined)[]): string | undefined => {
  const list = ws.filter((w): w is string => w !== undefined && w !== "");
  return list.length > 0 ? list.join("; ") : undefined;
};

/** Channel provenance: numeric iff there is at least one non-null cell and every
 *  non-null cell passes asNumber (numbers or numeric-looking strings). */
const inferNumeric = (values: Cell[]): boolean => {
  let nonNull = 0;
  for (const v of values) {
    if (v === null) continue;
    nonNull++;
    if (asNumber(v) === null) return false;
  }
  return nonNull > 0;
};

def("load.model", async (n: GraphNode, _i: NodeInputs, ctx: EvalCtx): Promise<NodeOut> => {
  const id = strParam(n, "model");
  if (!id) needsSetup("choose a model");
  const model = await ctx.loadModel(id);
  const entities = new Uint32Array(model.entityCount);
  for (let i = 0; i < model.entityCount; i++) entities[i] = i;
  return { value: { model, entities, channels: {} } };
});

def("select.byType", async (n, inputs) => {
  const scene = sceneIn(inputs, "in");
  const want = strParam(n, "type");
  if (!want) needsSetup("choose a type");
  const target = normType(want);
  return { value: derive(scene, filterScene(scene, i => normType(scene.model.types[i] ?? "") === target)) };
});

def("select.byLevel", async (n, inputs) => {
  const scene = sceneIn(inputs, "in");
  const want = strParam(n, "level");
  if (!want) needsSetup("choose a level");
  return { value: derive(scene, filterScene(scene, i => (scene.model.levels[i] ?? "").trim() === want)) };
});

def("select.byParameter", async (n, inputs) => {
  const scene = sceneIn(inputs, "in");
  const name = strParam(n, "parameter");
  if (!name) needsSetup("choose a parameter");
  const op = (strParam(n, "op") || ">") as Op;
  const raw = strParam(n, "value");
  const col = columnFor(scene, name);
  if (!col) fail(`no parameter or channel named "${name}"`);
  let dropped = 0;
  const value = derive(scene,
    filterScene(scene, i => compare(col![i] ?? null, op, raw, () => dropped++)));
  const out: NodeOut = { value };
  if (dropped > 0) out.warning = `${dropped} entities dropped as non-numeric`;
  return out;
});

def("data.csv", async (n, _i, ctx) => {
  const url = strParam(n, "url");
  if (!url) needsSetup("enter a csv url");
  const table = parseCsv(await ctx.fetchText(url));
  table.source = url.split("/").pop() || url;
  return { value: table };
});

def("table.sql", async (n, inputs, ctx) => {
  const scene = sceneIn(inputs, "in");
  const sql = strParam(n, "sql");
  if (!sql) needsSetup("enter a query");
  const table = await ctx.sql(scene.model.id, sql);
  table.source = "sql";
  return { value: table };
});

def("attach.column", async (n, inputs) => {
  const scene = sceneIn(inputs, "scene");
  const table = tableIn(inputs, "table");
  const keyColumn = strParam(n, "keyColumn") || "GlobalId";
  const valueColumn = strParam(n, "valueColumn");
  if (!valueColumn) needsSetup("choose a value column");
  const ki = table.columns.indexOf(keyColumn);
  const vi = table.columns.indexOf(valueColumn);
  if (ki < 0) fail(`table has no column "${keyColumn}"`);
  if (vi < 0) fail(`table has no column "${valueColumn}"`);

  const byKey = new Map<string, Cell>();
  for (const row of table.rows) {
    const k = row[ki];
    if (k !== null) byKey.set(String(k).trim(), coerceCell(row[vi] ?? null));
  }

  const channel: Cell[] = new Array<Cell>(scene.model.entityCount).fill(null);
  let matched = 0;
  for (const i of scene.entities) {
    const hit = byKey.get((scene.model.globalIds[i] ?? "").trim());
    if (hit !== undefined) { channel[i] = hit; matched++; }
  }
  const value: SceneValue = {
    ...scene,
    channels: {
      ...scene.channels,
      [valueColumn]: {
        values: channel,
        source: `${table.source ?? "table"}:${valueColumn}`,
        numeric: inferNumeric(channel),
      },
    },
  };
  const out: NodeOut = { value, summary: `matched ${matched} of ${scene.entities.length}` };
  const warning = joinWarnings(shadowWarning(scene, valueColumn));
  if (warning !== undefined) out.warning = warning;
  return out;
});

def("table.fromScene", async (_n, inputs) => {
  const scene = sceneIn(inputs, "in");
  const chanNames = Object.keys(scene.channels);
  const columns = ["GlobalId", "Type", "Name", "Level", ...chanNames];
  const rows: Cell[][] = [];
  for (const i of scene.entities) {
    rows.push([
      scene.model.globalIds[i] ?? null,
      scene.model.types[i] ?? null,
      scene.model.names[i] ?? null,
      scene.model.levels[i] ?? null,
      ...chanNames.map(c => scene.channels[c]?.values[i] ?? null),
    ]);
  }
  return { value: { columns, rows } };
});

def("table.filter", async (n, inputs) => {
  const table = tableIn(inputs, "in");
  const ci = columnIndex(table, strParam(n, "column"), "filter");
  const op = (strParam(n, "op") || ">") as Op;
  const raw = strParam(n, "value");
  let dropped = 0;
  const rows = table.rows.filter(r => compare(r[ci] ?? null, op, raw, () => dropped++));
  const out: NodeOut = { value: { ...table, rows } };
  if (dropped > 0) out.warning = `${dropped} rows dropped as non-numeric`;
  return out;
});

/**
 * Nulls sort last in both directions (they are absent, not extreme).
 * Numbers compare numerically, everything else case-insensitively as text;
 * ties keep input order, so a sort is a stable refinement of the previous one.
 */
def("table.sort", async (n, inputs) => {
  const table = tableIn(inputs, "in");
  const ci = columnIndex(table, strParam(n, "column"), "sort");
  const dir = boolParam(n, "descending", true) ? -1 : 1;

  const rank = (a: Cell, b: Cell): number => {
    const an = asNumber(a), bn = asNumber(b);
    if (an !== null && bn !== null) return an < bn ? -1 : an > bn ? 1 : 0;
    return String(a).toLowerCase().localeCompare(String(b).toLowerCase());
  };

  const rows = table.rows
    .map((row, i) => ({ row, i, key: row[ci] ?? null }))
    .sort((x, y) => {
      if (x.key === null || y.key === null) {
        if (x.key === y.key) return x.i - y.i;
        return x.key === null ? 1 : -1;
      }
      return rank(x.key, y.key) * dir || x.i - y.i;
    })
    .map(e => e.row);
  return { value: { ...table, rows } };
});

type Agg = "sum" | "avg" | "min" | "max" | "count";

def("table.aggregate", async (n, inputs) => {
  const table = tableIn(inputs, "in");
  const groupBy = strParam(n, "groupBy");
  const valueCol = strParam(n, "value");
  const agg = (strParam(n, "agg") || "sum") as Agg;
  if (!groupBy) needsSetup("choose a group-by column");
  const gi = table.columns.indexOf(groupBy);
  if (gi < 0) fail(`table has no column "${groupBy}"`);
  if (agg !== "count" && !valueCol) needsSetup("choose a value column");
  const vi = valueCol ? table.columns.indexOf(valueCol) : -1;
  if (valueCol && vi < 0) fail(`table has no column "${valueCol}"`);

  const groups = new Map<string, { key: Cell; nums: number[]; count: number }>();
  for (const row of table.rows) {
    const key = row[gi] ?? null;
    const k = key === null ? "" : String(key);
    let g = groups.get(k);
    if (!g) { g = { key, nums: [], count: 0 }; groups.set(k, g); }
    g.count++;
    if (vi >= 0) {
      const num = asNumber(row[vi] ?? null);
      if (num !== null) g.nums.push(num);
    }
  }

  const reduce = (g: { nums: number[]; count: number }): Cell => {
    if (agg === "count") return g.count;
    if (g.nums.length === 0) return null;
    switch (agg) {
      case "sum": return g.nums.reduce((a, b) => a + b, 0);
      case "avg": return g.nums.reduce((a, b) => a + b, 0) / g.nums.length;
      case "min": return Math.min(...g.nums);
      case "max": return Math.max(...g.nums);
      default: return null;
    }
  };

  const sorted = [...groups.values()].sort((a, b) => {
    const an = asNumber(a.key), bn = asNumber(b.key);
    if (an !== null && bn !== null) return an - bn;
    return String(a.key ?? "").localeCompare(String(b.key ?? ""));
  });

  const outCol = valueCol ? `${agg}_${valueCol}` : agg;
  return {
    value: { columns: [groupBy, outCol], rows: sorted.map(g => [g.key, reduce(g)]), source: table.source },
  };
});

def("view.scene", async (_n, inputs) => {
  const scene = sceneIn(inputs, "in");
  const names = Object.keys(scene.channels);
  const chans = names.length > 0 ? `channels: ${names.join(", ")}` : "no channels";
  return { value: scene, summary: `${scene.entities.length} entities · ${chans}` };
});

def("view.table", async (_n, inputs) => {
  const table = tableIn(inputs, "in");
  return { value: table, summary: `${table.rows.length} rows × ${table.columns.length} cols` };
});

/**
 * A user expression becomes a channel. The function is compiled once (a syntax error is a
 * node error), then run per selected entity: anything that throws, or that is not a number
 * or string, leaves that entity null — one bad element must not take the graph down.
 */
def("compute.expr", async (n, inputs) => {
  const scene = sceneIn(inputs, "in");
  const target = strParam(n, "channel");
  if (!target) needsSetup("name the output channel");
  const src = strParam(n, "expr");
  if (!src) needsSetup("enter an expression");

  let compiled: (gid: string, type: string, name: string, level: string | null,
                 param: (s: string) => Cell, ch: (s: string) => Cell) => unknown;
  try {
    // eslint-disable-next-line @typescript-eslint/no-implied-eval
    compiled = new Function("gid", "type", "name", "level", "param", "ch",
      `return (${src});`) as typeof compiled;
  } catch (e) {
    fail(`expression error: ${e instanceof Error ? e.message : String(e)}`);
  }

  const paramCols = new Map<string, Cell[] | null>();
  const colOf = (cache: Map<string, Cell[] | null>, get: () => Cell[] | null, key: string) => {
    if (!cache.has(key)) cache.set(key, get());
    return cache.get(key)!;
  };

  const channel: Cell[] = new Array<Cell>(scene.model.entityCount).fill(null);
  let written = 0;
  for (const i of scene.entities) {
    const param = (name: string): Cell =>
      colOf(paramCols, () => (scene.model.paramNames().some(p => p.name === name)
        ? scene.model.param(name) : null), `p:${name}`)?.[i] ?? null;
    const ch = (name: string): Cell =>
      colOf(paramCols, () => columnFor(scene, name), `c:${name}`)?.[i] ?? null;
    let out: unknown;
    try {
      out = compiled!(scene.model.globalIds[i] ?? "", scene.model.types[i] ?? "",
        scene.model.names[i] ?? "", scene.model.levels[i] ?? null, param, ch);
    } catch {
      out = null;
    }
    const cell: Cell = typeof out === "number" && Number.isFinite(out) ? out
      : typeof out === "string" ? out
      : null;
    channel[i] = cell;
    if (cell !== null) written++;
  }

  const value: SceneValue = {
    ...scene,
    channels: {
      ...scene.channels,
      [target]: { values: channel, source: "expr", numeric: inferNumeric(channel) },
    },
  };
  const out: NodeOut = {
    value, summary: `wrote ${target}: ${written} non-null of ${scene.entities.length}`,
  };
  const warning = joinWarnings(shadowWarning(scene, target));
  if (warning !== undefined) out.warning = warning;
  return out;
});

/**
 * NL question → host LLM writes SQL → the SQL runs → table. The generated SQL travels
 * on `detail` so the node can show its work. `ctx.ask` is optional in EvalCtx: when the
 * host has no AI sidecar the node errors up front, and any host-side error (missing API
 * key, bad generated SQL) surfaces verbatim as the node's error message.
 */
def("table.ask", async (n, inputs, ctx) => {
  const scene = sceneIn(inputs, "in");
  const question = strParam(n, "question");
  if (!question) needsSetup("ask a question");
  if (!ctx.ask) fail("host AI sidecar unavailable");
  const { sql } = await ctx.ask!(scene.model.id, question);
  const table = await ctx.sql(scene.model.id, sql);
  table.source = "ask";
  return { value: table, summary: `${table.rows.length} rows`, detail: sql };
});

const MAX_BARS = 24;

// Sink: draws in the node body via status.chart. Non-numeric value cells drop their row;
// bars keep input order and cap at MAX_BARS (summary notes the truncation).
def("chart.bar", async (n, inputs) => {
  const table = tableIn(inputs, "in");
  const labelName = strParam(n, "labelColumn")
    || (guessColumn(table, "text") ?? fail("no text column to label bars"));
  const valueName = strParam(n, "valueColumn")
    || (guessColumn(table, "numeric") ?? fail("no numeric column to chart"));
  const li = columnIndex(table, labelName, "label");
  const vi = columnIndex(table, valueName, "value");

  const labels: Cell[] = [];
  const values: number[] = [];
  let charted = 0;
  for (const row of table.rows) {
    const v = asNumber(row[vi] ?? null);
    if (v === null) continue;                        // non-numeric rows drop out
    charted++;
    if (values.length < MAX_BARS) { labels.push(row[li] ?? null); values.push(v); }
  }
  const summary = charted > MAX_BARS
    ? `${MAX_BARS} of ${charted} rows` : `${values.length} bars`;
  return {
    value: table,
    summary,
    chart: { labels, values, title: valueName },
  };
});

def("sink.table", async (_n, inputs) => {
  const table = tableIn(inputs, "in");
  return { value: table, summary: `${table.rows.length} rows` };
});

// effect: "write" — evaluate never performs the side effect; the Run button does.
def("sink.writePset", async (n, inputs) => {
  const scene = sceneIn(inputs, "in");
  const channels = channelList(n, scene);
  const rows = buildPsetRows(scene, channels);
  return { value: scene, summary: `ready: ${rows.length} entities × channels [${channels.join(", ")}]` };
});

// ---------- subgraph (T16) ----------
// The inner graph evaluates through the same machinery, with promoted input
// ports pre-bound (seeded) to the OUTER wire values via sub.inputs[].inner.
// Any inner error takes the whole node down (naming the root culprit, not a
// poisoned bystander); promoted outputs surface per slot through NodeOut.outputs.
// The evaluate↔nodes import cycle is call-time only (both sides use the other
// inside a function body), which ESM live bindings resolve fine.

def("graph.sub", async (n, inputs, ctx) => {
  const sub = n.sub;
  if (!sub || !sub.nodes.length) fail("empty subgraph");
  const seed: InputSeed = new Map();
  for (const p of sub!.inputs) {
    const v = inputs[p.name];
    if (v === undefined) fail(`missing input "${p.name}"`);
    seed.set(`${p.inner.node}|${p.inner.slot}`, v!);
  }
  const inner: GraphDoc = { name: "sub", nodes: sub!.nodes, edges: sub!.edges, display: null };
  const run = await evaluateGraphSeeded(inner, ctx, seed);
  // Root culprit first: a status that is NOT itself poison ("upstream error in …" /
  // "waiting on …"). An inner error makes the sub an error; an inner needs-setup
  // makes the sub needs-setup — unconfigured inside is unconfigured outside.
  const isRoot = (m?: string) =>
    !(m ?? "").startsWith("upstream error in ") && !(m ?? "").startsWith("waiting on ");
  const bad = sub!.nodes
    .map(m => ({ id: m.id, st: run.status.get(m.id) }))
    .filter(x => x.st !== undefined);
  const errs = bad.filter(x => x.st!.state === "error");
  if (errs.length) {
    const root = errs.find(x => isRoot(x.st!.message)) ?? errs[0];
    fail(`in subgraph: ${root.id}: ${root.st!.message ?? "error"}`);
  }
  const setups = bad.filter(x => x.st!.state === "needs-setup");
  if (setups.length) {
    const root = setups.find(x => isRoot(x.st!.message)) ?? setups[0];
    needsSetup(`in subgraph: ${root.id}: ${root.st!.message ?? "needs setup"}`);
  }
  const outputs: Record<string, Value> = {};
  for (const p of sub!.outputs) {
    const v = valueFrom(run, p.inner);
    if (v === undefined) fail(`subgraph output "${p.name}" has no value`);
    outputs[p.name] = v!;
  }
  // `value` (the single-output default) is the first promoted output; a
  // sink-only group has none, so an empty table stands in — the summary is
  // the real signal there.
  const first = sub!.outputs[0];
  return {
    value: first ? outputs[first.name] : { columns: [], rows: [] },
    outputs,
    summary: `${sub!.nodes.length} nodes · ${sub!.inputs.length} in / ${sub!.outputs.length} out`,
  };
});
