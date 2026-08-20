// Track C's public surface. Everything the app (Track E) or a harness (Track A) needs.
export { evaluateGraph } from "./evaluate";
export { NODES, ramp, buildPsetRows, asNumber } from "./nodes";
export { summarize, isScene, isTable, isView, isColormap } from "./summaries";
export { parseCsv } from "./csv";
export type { NodeEvaluate, NodeInputs, NodeOut } from "./types";

import { KINDS } from "../kinds";
/** Verbatim node vocabulary for Track D's `data/kinds.json` / `list_node_kinds`. */
export const kindsJson = (): string => JSON.stringify(KINDS, null, 2);
