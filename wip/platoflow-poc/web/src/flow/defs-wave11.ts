// Wave-11 defs: select.checklist / viz.boxes / viz.explode.
//
// Semantics pinned here (Track N):
// - Unnamed level sentinel = "(none)" (lib.UNNAMED_GROUP) — the same label the
//   categorical legend already uses for the null group. It round-trips through
//   the `excluded` comma list verbatim (contains no comma).
// - Checklist matching is EXACT string match against the displayed value: the
//   checklist UI writes back exactly what status.checklist showed, so no
//   normalization is wanted (unlike select.byType, whose param is hand-typed).
// - Explode offsets are MODEL-space (IFC Z-up): the viewer's loader applies a
//   single `rotation.x = -PI/2` Z-up→Y-up flip on the group (viewer/index.ts),
//   so "up" in the data the flow layer emits is +Z — offsets[3k+2].
// - Level order = natural sort (Intl.Collator numeric) of the distinct named
//   levels present AFTER hiding; index × spacing = offset. The unnamed level is
//   NOT part of the index sequence and is forced to offset 0 explicitly
//   ("entities with no level stay put"), as is the first named level (index 0).
import type { SceneValue, ViewValue } from "../contracts";
import { def } from "./registry";
import {
  derive, filterScene, fmtNum, naturalCompare, normType, numParam, sceneIn, strParam,
  UNNAMED_GROUP, viewIn,
} from "./lib";
import type { NodeOut } from "./types";

// ---------- select.checklist ----------

/** Displayed checklist value for entity i: the raw string, or the unnamed
 *  sentinel for null/empty (levels use null per W9 convention; an empty type
 *  string gets the same treatment for uniformity). */
const checklistValue = (scene: SceneValue, source: string, i: number): string => {
  const raw = source === "levels" ? scene.model.levels[i] : scene.model.types[i];
  return raw === null || raw === undefined || raw === "" ? UNNAMED_GROUP : String(raw);
};

def("select.checklist", async (n, inputs) => {
  const scene = sceneIn(inputs, "in");
  const source = strParam(n, "source") || "types";
  // INVERTED storage: the param lists UNTICKED values, so anything new arriving
  // from upstream defaults to ticked (absent from the list = on).
  const excluded = new Set(strParam(n, "excluded").split(",").map(s => s.trim()).filter(Boolean));

  const counts = new Map<string, number>();        // discovered value → entity count
  for (const i of scene.entities) {
    const v = checklistValue(scene, source, i);
    counts.set(v, (counts.get(v) ?? 0) + 1);
  }

  const entities = filterScene(scene, i => !excluded.has(checklistValue(scene, source, i)));

  const checklist = [...counts.entries()]
    .sort((a, b) => b[1] - a[1] || naturalCompare(a[0], b[0]))
    .map(([value, count]) => ({ value, on: !excluded.has(value), count }));

  // Honesty choice: excluded names no longer present upstream are dropped from
  // the checklist (it only shows live values) and filter nothing, but the node
  // SAYS so instead of silently carrying dead state in its param.
  const stale = [...excluded].filter(v => !counts.has(v));
  const ticked = checklist.filter(c => c.on).length;

  const out: NodeOut = {
    value: derive(scene, entities),                // channels + groups pass through
    summary: `${ticked} of ${checklist.length} ticked · ${entities.length} entities`,
    checklist,
  };
  if (stale.length > 0)
    out.warning = `excluded value${stale.length === 1 ? "" : "s"} no longer present: ${stale.join(", ")}`;
  return out;
});

// ---------- viz.boxes ----------

def("viz.boxes", async (_n, inputs) => {
  const view = viewIn(inputs, "in");
  const value: ViewValue = { ...view, boxes: true }; // immutable: never mutate the input
  return { value, summary: `${view.entities.length} entities · boxes` };
});

// ---------- viz.explode ----------

def("viz.explode", async (n, inputs) => {
  const view = viewIn(inputs, "in");
  const spacing = Math.max(0, numParam(n, "spacing", 15));   // bad spacing clamps ≥ 0
  const hidden = new Set(strParam(n, "hide").split(",").map(normType).filter(Boolean));

  // Drop hidden IFC classes first. ModelData.types IS the IFC class ("IfcWall");
  // the Type-is-family wart applies to the SQL EntityText view, not here. Match
  // like select.byType: case-insensitive, optional "Ifc" prefix (shared normType).
  const kept: number[] = [];                       // positions k into view.entities
  for (let k = 0; k < view.entities.length; k++) {
    const cls = normType(view.model.types[view.entities[k]] ?? "");
    if (!hidden.has(cls)) kept.push(k);
  }
  const entities = Uint32Array.from(kept, k => view.entities[k]);

  // Distinct NAMED levels among the survivors, natural order → index.
  const named = new Set<string>();
  for (const i of entities) {
    const l = view.model.levels[i];
    if (l !== null && l !== "") named.add(l);
  }
  const order = [...named].sort(naturalCompare);
  const index = new Map(order.map((l, k) => [l, k]));

  // Per-entity offsets aligned with the SURVIVING entities; compose additively
  // with any incoming offsets (filtered the same way colors are).
  const offsets = new Float32Array(entities.length * 3);
  for (let k = 0; k < kept.length; k++) {
    const src = kept[k];
    if (view.offsets) {
      offsets[k * 3] = view.offsets[src * 3];
      offsets[k * 3 + 1] = view.offsets[src * 3 + 1];
      offsets[k * 3 + 2] = view.offsets[src * 3 + 2];
    }
    const l = view.model.levels[entities[k]];
    const idx = l === null || l === "" ? 0 : index.get(l)!;  // unnamed stays put
    offsets[k * 3 + 2] += idx * spacing;                     // model space: Z is up
  }

  const value: ViewValue = { ...view, entities, offsets };
  if (view.colors) {                               // keep each survivor's color
    const colors = new Float32Array(entities.length * 3);
    for (let k = 0; k < kept.length; k++) {
      const src = kept[k];
      colors[k * 3] = view.colors[src * 3];
      colors[k * 3 + 1] = view.colors[src * 3 + 1];
      colors[k * 3 + 2] = view.colors[src * 3 + 2];
    }
    value.colors = colors;                         // legend/domain/ramp ride the spread
  }
  return {
    value,
    summary: `${entities.length} entities · ${order.length} levels × ${fmtNum(spacing)}`,
  };
});
