// Supervisor-owned node vocabulary. Track A/F renders palette/inspector/help from this;
// Track C/G implements evaluate() per kind; Track D serves it over MCP list_node_kinds.
import type { NodeKindInfo } from "./contracts";

export const KINDS: NodeKindInfo[] = [
  {
    kind: "load.model", label: "Load Model", category: "source",
    description: "Loads a BIM model (BIM Open Schema) from the host and outputs it as a scene containing every entity. The starting point of most graphs.",
    inputs: [], outputs: [{ name: "out", type: "scene", doc: "Scene containing every entity of the model." }],
    params: { model: { kind: "modelPick", default: "snowdon", doc: "Which converted model to load (served by the host)." } },
  },
  {
    kind: "select.byType", label: "By Type", category: "select",
    description: "Keeps only entities whose IFC type matches (case-insensitive, 'Ifc' prefix optional). Channels pass through untouched.",
    inputs: [{ name: "in", type: "scene", doc: "Scene to filter." }], outputs: [{ name: "out", type: "scene", doc: "Entities of the chosen type; channels pass through." }],
    params: { type: { kind: "dynamicEnum", source: "types", doc: "IFC entity type, e.g. IfcWallStandardCase." } },
  },
  {
    kind: "select.byLevel", label: "By Level", category: "select",
    description: "Keeps only entities on the chosen building storey/level.",
    inputs: [{ name: "in", type: "scene", doc: "Scene to filter." }], outputs: [{ name: "out", type: "scene", doc: "Entities on the chosen level; channels pass through." }],
    params: { level: { kind: "dynamicEnum", source: "levels", doc: "Storey name as authored in the model." } },
  },
  {
    kind: "select.byParameter", label: "By Parameter", category: "select",
    description: "Filters entities by a parameter or attached channel value. Null means 'absent': it fails every test except '!='. Non-numeric values are dropped by ordered comparisons.",
    inputs: [{ name: "in", type: "scene", doc: "Scene to filter." }], outputs: [{ name: "out", type: "scene", doc: "Entities passing the comparison; channels pass through." }],
    params: {
      parameter: { kind: "dynamicEnum", source: "parameters", doc: "Model parameter or channel name (channel wins if both exist)." },
      op: { kind: "enum", options: ["==", "!=", ">", ">=", "<", "<=", "contains", "exists"], default: ">", doc: "Comparison operator; 'exists' keeps entities where the value is non-null." },
      value: { kind: "string", default: "", doc: "Value to compare against (coerced to number for ordered ops)." },
    },
  },
  {
    kind: "select.union", label: "Union", category: "select",
    description: "Combines two selections over the same model: keeps entities present in either input. Channels merge; on a name clash the second input wins and the node reports a warning.",
    inputs: [{ name: "a", type: "scene", doc: "First selection." }, { name: "b", type: "scene", doc: "Second selection (its channels win a name clash)." }],
    outputs: [{ name: "out", type: "scene", doc: "Entities in either input." }],
    params: {},
  },
  {
    kind: "select.intersect", label: "Intersect", category: "select",
    description: "Keeps only entities present in both inputs (same model). Channels merge; on a name clash the second input wins and the node reports a warning.",
    inputs: [{ name: "a", type: "scene", doc: "First selection." }, { name: "b", type: "scene", doc: "Second selection (its channels win a name clash)." }],
    outputs: [{ name: "out", type: "scene", doc: "Entities in both inputs." }],
    params: {},
  },
  {
    kind: "select.subtract", label: "Subtract", category: "select",
    description: "Keeps entities of the first input that are NOT in the second (same model). The first input's channels pass through.",
    inputs: [{ name: "a", type: "scene", doc: "Selection to subtract from." }, { name: "b", type: "scene", doc: "Entities to remove." }],
    outputs: [{ name: "out", type: "scene", doc: "A minus B." }],
    params: {},
  },
  {
    kind: "select.invert", label: "Invert", category: "select",
    description: "Keeps every model entity NOT in the input selection. Channels pass through untouched (they are full-length, so no data is lost).",
    inputs: [{ name: "in", type: "scene", doc: "Selection to complement." }],
    outputs: [{ name: "out", type: "scene", doc: "The model's other entities." }],
    params: {},
  },
  {
    kind: "select.checklist", label: "Checklist", category: "select",
    description: "Filters the scene with live check-boxes: every Type (or Level) actually present in the input renders as a checkbox on the node, with its entity count. Untick to exclude. New values arriving from upstream default to ticked.",
    inputs: [{ name: "in", type: "scene", doc: "Scene whose live values become the check-boxes." }],
    outputs: [{ name: "out", type: "scene", doc: "Only entities whose value is ticked; channels pass through." }],
    params: {
      source: { kind: "enum", options: ["types", "levels"], default: "types", doc: "Which per-entity value the check-boxes enumerate." },
      excluded: { kind: "string", default: "", doc: "Comma-separated UNTICKED values (inverted storage: new values default to ticked)." },
    },
    width: 240, bodyHeight: 120,
  },
  {
    kind: "group.by", label: "Group By", category: "group",
    description: "Partitions the scene's entities into named groups by Type, Level, or any parameter/channel value. Color By colors per group in categorical mode; entities with no value form the unnamed group — real data, never dropped.",
    inputs: [{ name: "in", type: "scene", doc: "Scene to partition." }],
    outputs: [{ name: "out", type: "scene", doc: "Same selection, with a group channel attached." }],
    params: {
      by: { kind: "dynamicEnum", source: "groupKeys", doc: "Type, Level, or a parameter/channel whose values name the groups." },
    },
  },
  {
    kind: "data.csv", label: "Load CSV", category: "data",
    description: "Fetches and parses a CSV file into a table. First row is the header; quoted cells stay strings, unquoted numeric cells become numbers.",
    inputs: [], outputs: [{ name: "out", type: "table", doc: "Parsed table; first CSV row is the header." }],
    params: { url: { kind: "string", placeholder: "/api/file?path=carbon.csv", doc: "URL of the CSV (typically served from the host's data folder)." } },
  },
  {
    kind: "table.sql", label: "SQL Query", category: "table",
    description: "Runs a read-only DuckDB SQL query against the input scene's model on the host. Views: EntityText, ParameterText, RelationText. Note: 'Category' holds the IFC class; 'Type' is the family/type-object string.",
    inputs: [{ name: "in", type: "scene", doc: "Scene identifying which model the query runs against." }], outputs: [{ name: "out", type: "table", doc: "Query result rows." }],
    params: { sql: { kind: "text", commit: "explicit", default: "SELECT Category, COUNT(*) AS count FROM EntityText GROUP BY Category ORDER BY count DESC", doc: "A single SELECT or WITH statement." } },
    width: 240, bodyHeight: 88,
  },
  {
    kind: "attach.column", label: "Attach Column", category: "data",
    description: "Joins a table column onto the scene as a named channel, matching rows to entities by key (default GlobalId). The join hinge for enriching models with external analytics.",
    inputs: [{ name: "scene", type: "scene", doc: "Scene to enrich." }, { name: "table", type: "table", doc: "Table supplying the joined column." }],
    outputs: [{ name: "out", type: "scene", doc: "Scene with the new channel attached." }],
    params: {
      keyColumn: { kind: "dynamicEnum", source: "columns", default: "GlobalId", doc: "Table column matched against entity GlobalIds." },
      valueColumn: { kind: "dynamicEnum", source: "columns", doc: "Table column whose values become the new channel." },
    },
  },
  {
    kind: "compute.expr", label: "Expression", category: "data",
    description: "Computes a new channel per entity from a JavaScript expression. In scope: gid, type, name, level, param('Name'), ch('Name'). Example: ch('operational_carbon') / (param('Area') || 1).",
    inputs: [{ name: "in", type: "scene", doc: "Scene to compute over." }], outputs: [{ name: "out", type: "scene", doc: "Scene with the computed channel added." }],
    params: {
      channel: { kind: "string", default: "result", doc: "Name of the channel to write." },
      expr: { kind: "text", commit: "explicit", default: "ch('operational_carbon')", doc: "JS expression evaluated per selected entity; return null to leave an entity blank." },
    },
  },
  {
    kind: "table.fromScene", label: "To Table", category: "table",
    description: "Projects the scene's selected entities into a table: GlobalId, Type, Name, Level, plus one column per attached channel.",
    inputs: [{ name: "in", type: "scene", doc: "Scene to project." }], outputs: [{ name: "out", type: "table", doc: "One row per selected entity." }],
    params: {}, bodyHeight: 88,
  },
  {
    kind: "table.filter", label: "Filter Rows", category: "table",
    description: "Keeps table rows where a column passes a comparison. Same null and numeric-coercion rules as By Parameter.",
    inputs: [{ name: "in", type: "table", doc: "Table to filter." }], outputs: [{ name: "out", type: "table", doc: "Rows passing the comparison." }],
    params: {
      column: { kind: "dynamicEnum", source: "columns", doc: "Column to test." },
      op: { kind: "enum", options: ["==", "!=", ">", ">=", "<", "<=", "contains", "exists"], default: ">", doc: "Comparison operator." },
      value: { kind: "string", default: "", doc: "Value to compare against." },
    },
    bodyHeight: 88,
  },
  {
    kind: "table.sort", label: "Sort Rows", category: "table",
    description: "Sorts table rows by a column. Numbers sort numerically, strings lexically, nulls last.",
    inputs: [{ name: "in", type: "table", doc: "Table to sort." }], outputs: [{ name: "out", type: "table", doc: "Sorted rows." }],
    params: {
      column: { kind: "dynamicEnum", source: "columns", doc: "Column to sort by." },
      descending: { kind: "boolean", default: true, doc: "Largest/last first." },
    },
    bodyHeight: 88,
  },
  {
    kind: "table.aggregate", label: "Aggregate", category: "table",
    description: "Groups table rows by a column and aggregates another (sum/avg/min/max/count). The empty group is real data: rows with no value for the group key are reported as an unnamed group.",
    inputs: [{ name: "in", type: "table", doc: "Table to group." }], outputs: [{ name: "out", type: "table", doc: "One row per group with the aggregated value." }],
    params: {
      groupBy: { kind: "dynamicEnum", source: "columns", doc: "Column whose distinct values form the groups." },
      value: { kind: "dynamicEnum", source: "columns", doc: "Column to aggregate (ignored for count)." },
      agg: { kind: "enum", options: ["sum", "avg", "min", "max", "count"], default: "sum", doc: "Aggregation function." },
    },
    bodyHeight: 88,
  },
  {
    kind: "table.columns", label: "Select Columns", category: "table",
    description: "Keeps (or drops) named table columns, in the order given. The projection tool: trim a wide SQL result to the columns a chart or join actually needs.",
    inputs: [{ name: "in", type: "table", doc: "Table to project." }], outputs: [{ name: "out", type: "table", doc: "Table with only (or without) the named columns, ordered as listed." }],
    params: {
      columns: { kind: "string", placeholder: "GlobalId, Level, cost", doc: "Comma-separated column names. Unknown names are reported as a warning, not an error." },
      mode: { kind: "enum", options: ["keep", "drop"], default: "keep", doc: "keep = only the listed columns (in list order); drop = everything except them." },
    },
  },
  {
    kind: "table.stats", label: "Column Stats", category: "table",
    description: "Summarizes every numeric column of the input table: one row per column with count, min, max, mean, and sum. Non-numeric cells are ignored per column (and reported).",
    inputs: [{ name: "in", type: "table", doc: "Table to summarize." }], outputs: [{ name: "out", type: "table", doc: "One row per numeric column: column, count, min, max, mean, sum." }],
    params: {},
    bodyHeight: 88,
  },
  {
    kind: "check.rule", label: "Check Rule", category: "table",
    description: "Asserts a condition over every table row and reports a verdict: PASS when all rows satisfy it, FAIL with the violating rows as output. The compliance tool — wire the violations onward to color or list the offenders.",
    inputs: [{ name: "in", type: "table", doc: "Table whose rows are checked." }], outputs: [{ name: "out", type: "table", doc: "The VIOLATING rows (empty on PASS)." }],
    params: {
      column: { kind: "dynamicEnum", source: "columns", doc: "Column the rule tests." },
      op: { kind: "enum", options: ["==", "!=", ">", ">=", "<", "<=", "contains", "exists"], default: ">=", doc: "Comparison every row must satisfy." },
      value: { kind: "string", default: "", doc: "Value to compare against (coerced to number for ordered ops)." },
    },
    bodyHeight: 88,
  },
  {
    kind: "table.count", label: "Count", category: "table",
    description: "Counts the scene's selected entities into a one-row table. The quickest sanity check on a filter chain.",
    inputs: [{ name: "in", type: "scene", doc: "Scene whose selection gets counted." }],
    outputs: [{ name: "out", type: "table", doc: "One row: the entity count." }],
    params: {}, bodyHeight: 88,
  },
  {
    kind: "view.scene", label: "View Scene", category: "view",
    description: "Pass-through inspector for scenes: wire it mid-flow to watch the selection count and channels at that point. Select it to see its entities in the data grid; flag its eye to see them in 3D.",
    inputs: [{ name: "in", type: "scene", doc: "Scene to inspect." }], outputs: [{ name: "out", type: "scene", doc: "Unchanged pass-through of the input." }],
    params: {},
  },
  {
    kind: "view.table", label: "View Table", category: "view",
    description: "Pass-through inspector for tables: wire it mid-flow to watch row/column counts. Select it to see the rows in the data grid.",
    inputs: [{ name: "in", type: "table", doc: "Table to inspect." }], outputs: [{ name: "out", type: "table", doc: "Unchanged pass-through of the input." }],
    params: {},
  },
  {
    kind: "viz.colormap", label: "Colormap", category: "viz",
    description: "Maps numbers to colors over a domain. Auto mode reads the domain from the data feeding Color By, so values never silently clamp to one color.",
    inputs: [], outputs: [{ name: "out", type: "colormap", doc: "Colormap for Color By." }],
    params: {
      ramp: { kind: "enum", options: ["viridis", "heat", "greenred"], default: "viridis", doc: "Color ramp." },
      auto: { kind: "boolean", default: true, doc: "Take min/max from the incoming channel's actual values." },
      min: { kind: "number", default: 0, doc: "Domain minimum (used when auto is off)." },
      max: { kind: "number", default: 100, doc: "Domain maximum (used when auto is off)." },
    },
    bodyHeight: 40,                                // reserved for the gradient body (W1/T5)
  },
  {
    kind: "viz.colorBy", label: "Color By", category: "viz",
    description: "Colors the scene's entities by a channel (or numeric parameter), producing a 3D view. The ramp and domain live ON this node (gradient body); wire a Colormap input only to share one ramp across several views — a wired colormap overrides the embedded settings. Entities with no value render gray.",
    inputs: [{ name: "scene", type: "scene", doc: "Scene whose entities get colored." }, { name: "colormap", type: "colormap", doc: "OPTIONAL — overrides the embedded ramp/domain when wired (share one colormap across views)." }],
    outputs: [{ name: "out", type: "view", doc: "3D view for the viewer (flag the eye to display it)." }],
    params: {
      channel: { kind: "dynamicEnum", source: "channels", doc: "Channel or numeric parameter supplying the values. Leave unset on a grouped scene to color by group." },
      mode: { kind: "enum", options: ["auto", "numeric", "categorical"], default: "auto", doc: "auto = numeric ramp when the values are numbers, categorical swatches when they are text or the scene is grouped." },
      ramp: { kind: "enum", options: ["viridis", "heat", "greenred"], default: "viridis", doc: "Embedded color ramp (ignored when a colormap is wired)." },
      auto: { kind: "boolean", default: true, doc: "Take the domain from the channel's actual values (ignored when a colormap is wired)." },
      min: { kind: "number", default: 0, doc: "Embedded domain minimum (auto off)." },
      max: { kind: "number", default: 100, doc: "Embedded domain maximum (auto off)." },
      ghostOthers: { kind: "boolean", default: true, doc: "Show unselected entities as translucent gray (off = hide them)." },
    },
    bodyHeight: 40,                                // embedded gradient body (same organ as viz.colormap)
  },
  {
    kind: "viz.boxes", label: "Bounding Boxes", category: "viz",
    description: "Re-renders the view's entities as their axis-aligned bounding boxes — the massing/abstraction view. Colors, ghosting, and the legend pass through untouched.",
    inputs: [{ name: "in", type: "view", doc: "View to abstract (chain after Color By)." }],
    outputs: [{ name: "out", type: "view", doc: "Same view, rendered as boxes." }],
    params: {},
  },
  {
    kind: "viz.explode", label: "Explode Levels", category: "viz",
    description: "Separates the building's storeys vertically: each level's entities are displaced upward by level index × spacing (hidden categories — ceilings/coverings by default — are dropped first). Entities with no level stay put.",
    inputs: [{ name: "in", type: "view", doc: "View to explode (chain after Color By)." }],
    outputs: [{ name: "out", type: "view", doc: "Same view with per-entity vertical offsets." }],
    params: {
      spacing: { kind: "number", default: 15, min: 0, max: 100, step: 1, doc: "Vertical gap added per storey (world units)." },
      hide: { kind: "string", default: "IFCCOVERING", doc: "Comma-separated IFC categories dropped before exploding (ceilings by default)." },
    },
  },
  {
    kind: "table.ask", label: "Ask AI", category: "table",
    description: "Asks a natural-language question about the input scene's model; an LLM on the host writes a DuckDB SQL query which runs read-only and outputs the result table. The generated SQL is shown on the node. Requires ANTHROPIC_API_KEY on the host.",
    inputs: [{ name: "in", type: "scene", doc: "Scene identifying which model the question is about." }], outputs: [{ name: "out", type: "table", doc: "Result of the generated SQL." }],
    params: { question: { kind: "text", commit: "explicit", default: "How many entities of each category are there?", doc: "Plain-English question about the model; answered via the EntityText/ParameterText/RelationText SQL views." } },
    width: 240, bodyHeight: 88,
  },
  {
    kind: "chart.bar", label: "Bar Chart", category: "sink",
    description: "Draws a bar chart of the input table directly in the node body: one bar per row, labels from one column, heights from another. Hover a bar for its exact value.",
    inputs: [{ name: "in", type: "table", doc: "Table to chart — one bar per row." }], outputs: [],
    params: {
      labelColumn: { kind: "dynamicEnum", source: "columns", doc: "Column supplying bar labels (defaults to the first text column)." },
      valueColumn: { kind: "dynamicEnum", source: "columns", doc: "Numeric column supplying bar heights (defaults to the first numeric column)." },
    },
    bodyHeight: 96,
    width: 240,
  },
  {
    kind: "graph.sub", label: "Subgraph", category: "data",
    description: "A collapsed group of nodes with its boundary wires promoted to ports. Double-click (or press G on a selection) to enter and edit the inner graph; ports come from the node's own sub spec, not this declaration.",
    inputs: [], outputs: [],                       // dynamic — see GraphNode.sub
    params: { label: { kind: "string", default: "group", doc: "Display name on the subgraph node." } },
  },
  {
    kind: "sink.table", label: "Data Grid", category: "sink",
    description: "Shows the input table in the data-grid pane. Rows with a GlobalId or Level column highlight the matching elements in 3D when clicked.",
    inputs: [{ name: "in", type: "table", doc: "Table to show in the data-grid pane." }], outputs: [],
    params: {},
  },
  {
    kind: "sink.writePset", label: "Write Pset", category: "sink",
    description: "Writes the scene's channels back into the source IFC as a property set per element (byte-exact patch; produces an enriched copy plus a diff). Runs only when you press Run.",
    inputs: [{ name: "in", type: "scene", doc: "Scene whose channels get written back to the IFC." }], outputs: [],
    params: {
      psetName: { kind: "string", default: "Ara3D_Analytics", doc: "Property-set name written onto each element." },
      channels: { kind: "string", placeholder: "comma-separated channel names", doc: "Which channels become properties." },
    },
    effect: "write",
  },
  {
    kind: "sink.exportCsv", label: "Export CSV", category: "sink",
    description: "Writes the input table to a CSV file in the host's output folder. Runs only when you press Run; the configured export stays part of the graph.",
    inputs: [{ name: "in", type: "table", doc: "Table to write." }], outputs: [],
    params: {
      filename: { kind: "string", default: "export.csv", doc: "File name written under the host's data/out folder (path parts are stripped)." },
    },
    effect: "write",
  },
];

export const kindInfo = (kind: string): NodeKindInfo | undefined =>
  KINDS.find(k => k.kind === kind);
