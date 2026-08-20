// Supervisor-owned contract types. Smallest-change edits only; log changes in NOTES.md.

// ---------- Graph document ----------

export interface GraphNode {
  id: string;
  kind: string;                    // key into kinds.ts
  params: Record<string, unknown>;
  x: number;
  y: number;
  /** kind === "graph.sub" only: the nested graph plus its promoted ports.
   *  Ports come from HERE, not from kinds.ts — the one kind with dynamic arity. */
  sub?: SubgraphSpec;
  /** per-instance width override (drag the card's right edge); wins over
   *  NodeKindInfo.width */
  w?: number;
  /** per-instance body-height override (drag the card's bottom edge); wins
   *  over NodeKindInfo.bodyHeight. Only meaningful on kinds with a body. */
  bh?: number;
}

/** A collapsed selection: inner nodes/edges plus the boundary crossings that
 *  became the subgraph node's ports (`inner` = the slot the port forwards to). */
export interface SubgraphSpec {
  nodes: GraphNode[];
  edges: GraphEdge[];
  inputs: { name: string; type: WireType; inner: SlotRef }[];
  outputs: { name: string; type: WireType; inner: SlotRef }[];
}

export interface SlotRef { node: string; slot: string; }

export interface GraphEdge { from: SlotRef; to: SlotRef; }

export interface GraphDoc {
  name: string;
  nodes: GraphNode[];
  edges: GraphEdge[];
  display: string | null;          // display-flagged node id (eye icon)
}

export type Intent =
  | { t: "addNode"; id: string; kind: string; x: number; y: number; params?: Record<string, unknown>; sub?: SubgraphSpec }
  | { t: "removeNode"; id: string }
  | { t: "connect"; from: SlotRef; to: SlotRef }
  | { t: "disconnect"; to: SlotRef }
  | { t: "setParam"; node: string; name: string; value: unknown }
  | { t: "move"; node: string; x: number; y: number }
  | { t: "resize"; node: string; w?: number; bh?: number }
  | { t: "setDisplay"; node: string | null }
  | { t: "load"; doc: GraphDoc }
  /** applied atomically; one undo entry (link-drag-search, paste, subgraph ops) */
  | { t: "batch"; intents: Intent[] };

// ---------- Values on wires ----------

export type WireType = "scene" | "table" | "colormap" | "view";

// Rebranded 2026-08-19 (cream theme): wires read as thin mid-gray curves with a
// hue whisper per type — type coding kept, saturation dropped for the light canvas.
export const WIRE_COLORS: Record<WireType, string> = {
  scene: "#7A98A8", table: "#A8906A", colormap: "#9A7FA5", view: "#7F9E82",
};

export type Cell = number | string | null;

export interface TableValue {
  columns: string[];
  rows: Cell[][];
  /** provenance: where the table came from ("carbon.csv", "sql", "ask");
   *  transforms pass it through, joins copy it into the channel's `source` */
  source?: string;
}

export interface ColormapValue {
  ramp: "viridis" | "heat" | "greenred";
  min: number;
  max: number;
}

/** An overlay column: full-length per-entity values plus provenance (design §1.4).
 *  A channel is a record, not a bare array — WritePset needs `numeric`, and the UI
 *  needs `source` to say "came from carbon.csv:embodied_carbon". */
export interface ChannelValue {
  values: Cell[];                           // full-length per-entity (sparse ok)
  source?: string;                          // e.g. "carbon.csv:embodied_carbon", "expr"
  numeric?: boolean;
  unit?: string;
}

/** A partition of the scene's entities: per-entity group label.
 *  null = entity has no value for the grouping key (the unnamed group — real data). */
export interface GroupChannel { name: string; values: (string | null)[]; }

/** Entity-level scene selection over one model, with overlay channels. */
export interface SceneValue {
  model: ModelData;
  entities: Uint32Array;                    // selected entity indices, ascending
  channels: Record<string, ChannelValue>;   // overlay columns, keyed by name
  groups?: GroupChannel;                    // group.by writes; categorical colorBy consumes
}

/** What the 3D viewer renders. colors are rgb triplets aligned with `entities`. */
export interface ViewValue {
  model: ModelData;
  entities: Uint32Array;
  colors?: Float32Array;                    // 3 * entities.length, 0..1
  ghostOthers: boolean;
  label?: string;
  /** effective colormap domain actually used (auto mode resolves it here; legend reads it) */
  domain?: [number, number];
  ramp?: string;
  /** categorical mode: one swatch per group/value, in assignment order (legend pane
   *  reads it). Colors are 0..1 rgb floats — the same space as `colors`. When both
   *  `legend` and `domain`/`ramp` are present, `legend` wins (categorical beats
   *  numeric); `domain` and `ramp` are only meaningful together. (W9-D) */
  legend?: { label: string; color: [number, number, number] }[];
  /** per-entity world-space translation (3 * entities.length), e.g. the level
   *  explode — the viewer offsets each instance, geometry otherwise untouched */
  offsets?: Float32Array;
  /** render each entity as its axis-aligned bounding box instead of its mesh */
  boxes?: boolean;
}

export type Value = SceneValue | TableValue | ColormapValue | ViewValue;

// ---------- Model data (B implements over ara3d-webgl loader) ----------

export interface ParamInfo { name: string; pset: string; numeric: boolean; }

export interface ModelData {
  id: string;
  entityCount: number;
  globalIds: string[];                      // per entity index
  types: string[];                          // IFC type per entity, e.g. "IfcWall"
  names: string[];
  levels: (string | null)[];                // storey/level name per entity
  paramNames(): ParamInfo[];
  param(name: string): Cell[];              // full-length per-entity column, cached
}

// ---------- Evaluator (C implements) ----------

export interface NodeStatus {
  /** "needs-setup" = unconfigured, not broken: a freshly dropped node waiting on a
   *  required param or input renders neutral gray, never red (design §3). */
  state: "ok" | "error" | "pending" | "needs-setup";
  message?: string;                         // error text / what setup is needed
  /** non-fatal honesty note rendered amber: "42 rows dropped as non-numeric",
   *  "channel 'Area' shadows model parameter", channel-name clashes on merge */
  warning?: string;
  summary?: string;                         // e.g. "312 entities" / "14 rows"
  /** bar-chart payload for in-node visualization (chart.bar fills it) */
  chart?: { labels: Cell[]; values: number[]; title?: string };
  /** extra detail line, e.g. ask.llm's generated SQL */
  detail?: string;
  /** small page of the node's table output for in-node mini-grid bodies
   *  (~5 rows × leading columns; the integration layer derives it at eval) */
  tablePreview?: TableValue;
  /** live check-box list for in-node checklist bodies (select.checklist fills
   *  it from the scene's actual values: one entry per discovered value, with
   *  its enabled state and entity count) */
  checklist?: { value: string; on: boolean; count?: number }[];
}

export interface EvalResult {
  values: Map<string, Value>;               // per node id (output value)
  status: Map<string, NodeStatus>;
}

export interface EvalCtx {
  loadModel(modelId: string): Promise<ModelData>;
  sql(modelId: string, query: string): Promise<TableValue>;
  fetchText(url: string): Promise<string>;  // CSV etc.
  listModels(): Promise<{ id: string; name: string }[]>;
  /** NL question over the model's SQL views; host LLM sidecar returns the SQL it wrote.
   *  Optional: evaluator errors the node with a clear message when absent. */
  ask?(modelId: string, question: string): Promise<{ sql: string }>;
}

export type EvaluateGraph = (doc: GraphDoc, ctx: EvalCtx) => Promise<EvalResult>;

// ---------- Viewer bridge + panes (B implements) ----------

export interface ViewerBridge {
  load(bosUrl: string, id: string): Promise<ModelData>;
  applyView(view: ViewValue | null): void;  // null → default full-model view
  highlight(entities: Uint32Array | null): void;
  onPick(cb: (entityIndex: number | null) => void): void;
}

export interface Panes {
  showTable(t: TableValue | null, opts?: { onRowClick?: (row: Cell[]) => void }): void;
  showLegend(c: ColormapValue | null, label?: string): void;
  /** preferred over showLegend when a view is displayed: renders the effective numeric
   *  domain from ViewValue.domain/ramp, or categorical swatches when `legend` is set */
  showViewLegend?(v: ViewValue | null): void;
  setStatus(msg: string): void;
}

// ---------- Node definitions (kinds.ts declares; C attaches evaluate) ----------

export type ParamSchema = (
  | { kind: "string"; default?: string; placeholder?: string }
  | { kind: "number"; default?: number; min?: number; max?: number; step?: number }
  | { kind: "boolean"; default?: boolean }
  | { kind: "enum"; options: string[]; default?: string }
  | { kind: "text"; default?: string }                     // multiline (SQL, expressions)
  | { kind: "modelPick"; default?: string }                // dropdown of ctx.listModels()
  | { kind: "dynamicEnum"; source: "types" | "levels" | "parameters" | "channels" | "columns" | "groupKeys"; default?: string }
) & {
  doc?: string;                                            // shown in help tooltip/descriptor
  /** commit policy: "live" (default) = every input event dispatches setParam;
   *  "explicit" = buffer locally, commit on Ctrl-Enter/blur (SQL, expressions). */
  commit?: "live" | "explicit";
};

export interface PortSpec { name: string; type: WireType; doc?: string; }

export interface NodeKindInfo {
  kind: string;
  label: string;
  category: "source" | "select" | "group" | "data" | "table" | "viz" | "view" | "sink";
  /** 1-2 sentences for hover tooltip; first sentence is the short form */
  description: string;
  inputs: PortSpec[];
  outputs: PortSpec[];
  params: Record<string, ParamSchema>;
  /** effectful sinks render a Run button instead of auto-evaluating side effects */
  effect?: "write";
  /** extra node-body pixels for kinds that render content in the node (e.g. chart.bar) */
  bodyHeight?: number;
  /** per-kind card width in px; default NODE_W (188) */
  width?: number;
}
