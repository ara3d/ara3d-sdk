// One-line status text per wire value. Used for NodeStatus.summary on every ok node.
import type { ColormapValue, SceneValue, TableValue, Value, ViewValue } from "../contracts";

export const isTable = (v: Value): v is TableValue => "columns" in v && "rows" in v;
export const isColormap = (v: Value): v is ColormapValue => "ramp" in v && "min" in v;
export const isView = (v: Value): v is ViewValue => "ghostOthers" in v;
export const isScene = (v: Value): v is SceneValue => "channels" in v && "entities" in v;

export function summarize(v: Value): string {
  if (isTable(v)) return `${v.rows.length} rows`;
  if (isColormap(v)) return `${v.min}–${v.max} ${v.ramp}`;
  if (isView(v)) return `${v.entities.length} colored`;
  if (isScene(v)) return `${v.entities.length} entities`;
  return "";
}
