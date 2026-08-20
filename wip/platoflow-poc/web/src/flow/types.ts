// Evaluator-internal types. Track C owns these; nothing here leaks into contracts.ts.
import type { ColormapValue, EvalCtx, GraphNode, NodeStatus, Value } from "../contracts";

/**
 * A colormap on the wire, plus the auto flag its node was configured with.
 * `auto` is evaluator-internal: viz.colorBy needs it to decide whether to resolve
 * the domain from data, but ViewValue.domain is what any consumer actually reads.
 */
export interface ColormapCfg extends ColormapValue { auto: boolean; }

/** What one node's evaluate returns: the wire value plus an optional custom summary. */
export interface NodeOut {
  value: Value;
  /** Overrides the generic per-value summary (e.g. "matched 24 of 24"). */
  summary?: string;
  /** Non-fatal honesty note copied onto NodeStatus.warning (wave 9): dropped rows,
   *  shadowed parameters, channel-name clashes on merge. */
  warning?: string;
  /** Bar-chart payload copied onto NodeStatus (chart.bar fills it). */
  chart?: NodeStatus["chart"];
  /** Extra detail line copied onto NodeStatus (e.g. table.ask's generated SQL). */
  detail?: string;
  /** Live checkbox payload copied onto NodeStatus (select.checklist fills it). */
  checklist?: NodeStatus["checklist"];
  /** T16: per-slot values for multi-output nodes (graph.sub promoted outputs).
   *  When present, an outgoing edge resolves `outputs[edge.from.slot]` instead
   *  of `value`; `value` stays the single-output default every other kind uses. */
  outputs?: Record<string, Value>;
}

/** Inputs keyed by the node's declared input slot name (see kinds.ts). */
export type NodeInputs = Record<string, Value | undefined>;

export type NodeEvaluate = (node: GraphNode, inputs: NodeInputs, ctx: EvalCtx) => Promise<NodeOut>;

export const fail = (msg: string): never => { throw new Error(msg); };

/** "Unconfigured, not broken" (wave 9): a def throws this when a REQUIRED param is
 *  still unset; the evaluator reports state "needs-setup" (neutral gray), reserving
 *  red for genuinely broken graphs. */
export class NeedsSetup extends Error {}

export const needsSetup = (msg: string): never => { throw new NeedsSetup(msg); };
