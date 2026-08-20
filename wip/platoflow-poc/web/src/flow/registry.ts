// The kind → evaluate registry. Def modules call def() at module load; nodes.ts
// imports those modules for their side effects and re-exports NODES (wave-9 split
// so parallel tracks own separate def files — see CONTRACTS.md fences).
import type { NodeEvaluate } from "./types";

export const NODES = new Map<string, NodeEvaluate>();

export const def = (kind: string, fn: NodeEvaluate) => NODES.set(kind, fn);
