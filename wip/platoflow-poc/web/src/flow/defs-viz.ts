// Viz node semantics: colormaps and colorBy. Wave-9 Track B: colorBy gained a mode
// param — the numeric ramp path is byte-identical to pre-wave-9 behavior; categorical
// mode assigns per-value swatches (golden-angle hue walk) and fills ViewValue.legend.
import type { Cell, ColormapValue, GraphNode, SceneValue, ViewValue } from "../contracts";
import { def } from "./registry";
import {
  asNumber, boolParam, colormapIn, columnFor, fmtNum, numParam, ramp, sceneIn, strParam,
  type RGB,
} from "./lib";
import { fail, needsSetup, type ColormapCfg, type NodeOut } from "./types";

/** Ramp/domain config from a node's own params. ONE resolution rule for both the
 *  standalone viz.colormap node and viz.colorBy's wave-10 embedded ramp — the same
 *  params (ramp/auto/min/max, same defaults) mean the same thing in both places. */
const embeddedCfg = (n: GraphNode): ColormapCfg => ({
  ramp: (strParam(n, "ramp") || "viridis") as ColormapValue["ramp"],
  min: numParam(n, "min", 0),
  max: numParam(n, "max", 100),
  auto: boolParam(n, "auto", true),
});

def("viz.colormap", async (n) => ({ value: embeddedCfg(n) }));

/** Ramp domain actually used: from the data when auto is on, else from the colormap's params. */
const effectiveDomain = (
  cmap: ColormapValue & { auto?: boolean },
  values: (number | null)[],
): [number, number] => {
  const nums = cmap.auto ? values.filter((v): v is number => v !== null) : [];
  const lo = nums.length > 0 ? Math.min(...nums) : cmap.min;
  const hi = nums.length > 0 ? Math.max(...nums) : cmap.max;
  return lo === hi ? [lo, lo + 1] : [lo, hi];
};

/** No-value gray — shared by the numeric path (null cells), the categorical
 *  unnamed group, and values beyond the categorical cap. */
const GRAY: RGB = [0.35, 0.35, 0.35];

// ---------- categorical palette (implemented locally, by design) ----------

/** Standard HSL → RGB; h/s/l in [0,1]. */
function hslToRgb(h: number, s: number, l: number): RGB {
  const q = l < 0.5 ? l * (1 + s) : l + s - l * s;
  const p = 2 * l - q;
  const f = (t: number): number => {
    const u = ((t % 1) + 1) % 1;
    if (u < 1 / 6) return p + (q - p) * 6 * u;
    if (u < 1 / 2) return q;
    if (u < 2 / 3) return p + (q - p) * (2 / 3 - u) * 6;
    return p;
  };
  return [f(h + 1 / 3), f(h), f(h - 1 / 3)];
}

/** k-th categorical swatch: a golden-angle hue walk keeps neighbors far apart. */
const swatch = (k: number): RGB => hslToRgb(((k * 137.508) / 360) % 1, 0.65, 0.55);

/** Distinct categorical values beyond this render gray (mirrors chart.bar's cap). */
const MAX_GROUPS = 24;

// ---------- the two colorBy paths ----------

/** Pre-wave-9 behavior, verbatim: ramp over the effective domain, null → gray. */
function numericView(
  scene: SceneValue, cmap: ColormapValue & { auto?: boolean }, cells: Cell[],
  label: string, ghostOthers: boolean,
): NodeOut {
  const nums = [...scene.entities].map(i => asNumber(cells[i] ?? null));
  const [lo, hi] = effectiveDomain(cmap, nums);
  const span = hi - lo;
  const colors = new Float32Array(nums.length * 3);
  for (let k = 0; k < nums.length; k++) {
    const v = nums[k];
    const rgb: RGB = v === null ? GRAY : ramp(cmap.ramp, span === 0 ? 0 : (v - lo) / span);
    colors[k * 3] = rgb[0]; colors[k * 3 + 1] = rgb[1]; colors[k * 3 + 2] = rgb[2];
  }
  const value: ViewValue = {
    model: scene.model,
    entities: scene.entities,
    colors,
    ghostOthers,
    label,
    domain: [lo, hi],
    ramp: cmap.ramp,
  };
  return { value, summary: `${scene.entities.length} colored · ${fmtNum(lo)}–${fmtNum(hi)}` };
}

/** One swatch per distinct value in first-appearance order over the selected
 *  entities; null/empty is the unnamed group — gray, kept in the legend as
 *  "(none)", never dropped. No domain/ramp: the legend IS the scale. */
function categoricalView(
  scene: SceneValue, cells: Cell[], label: string, ghostOthers: boolean,
): NodeOut {
  const index = new Map<string, number>();       // value → swatch slot, first appearance
  let hasNone = false;
  for (const i of scene.entities) {
    const c = cells[i] ?? null;
    if (c === null || c === "") { hasNone = true; continue; }
    const key = String(c);
    if (!index.has(key)) index.set(key, index.size);
  }

  const colors = new Float32Array(scene.entities.length * 3);
  let k = 0;
  for (const i of scene.entities) {
    const c = cells[i] ?? null;
    const slot = c === null || c === "" ? -1 : index.get(String(c))!;
    const rgb = slot < 0 || slot >= MAX_GROUPS ? GRAY : swatch(slot);
    colors[k * 3] = rgb[0]; colors[k * 3 + 1] = rgb[1]; colors[k * 3 + 2] = rgb[2];
    k++;
  }

  const legend: NonNullable<ViewValue["legend"]> = [...index.keys()]
    .slice(0, MAX_GROUPS)
    .map((name, slot) => ({ label: name, color: swatch(slot) }));
  if (hasNone) legend.push({ label: "(none)", color: GRAY });

  const value: ViewValue = {
    model: scene.model,
    entities: scene.entities,
    colors,
    ghostOthers,
    label,
    legend,
  };
  const over = index.size - MAX_GROUPS;
  return {
    value,
    summary: `${scene.entities.length} colored · ${index.size} groups`,
    ...(over > 0 ? { warning: `${over} values beyond ${MAX_GROUPS} render gray` } : {}),
  };
}

/** Auto-mode tiebreak: over the selected entities' non-empty cells, do numbers win? */
function majorityNumeric(scene: SceneValue, cells: Cell[]): boolean {
  let filled = 0, nums = 0;
  for (const i of scene.entities) {
    const c = cells[i] ?? null;
    if (c === null || c === "") continue;
    filled++;
    if (asNumber(c) !== null) nums++;
  }
  return nums * 2 > filled;
}

def("viz.colorBy", async (n, inputs) => {
  const scene = sceneIn(inputs, "scene");
  // Wave 10: the colormap input is OPTIONAL. Wired wins ENTIRELY (share one ramp
  // across views); absent falls back to the node's own embedded ramp/auto/min/max
  // params, resolved by the exact rule viz.colormap uses.
  const cmap: ColormapValue & { auto?: boolean } = inputs["colormap"] !== undefined
    ? colormapIn(inputs, "colormap") as ColormapValue & { auto?: boolean }
    : embeddedCfg(n);
  const name = strParam(n, "channel");
  const mode = strParam(n, "mode") || "auto";
  const ghostOthers = boolParam(n, "ghostOthers", true);

  // Value source: the channel param when set; a grouped scene's labels otherwise.
  let cells: Cell[];
  let label: string;
  let fromGroups = false;
  if (name) {
    cells = columnFor(scene, name) ?? fail(`no channel or parameter named "${name}"`);
    label = name;
  } else if (scene.groups) {
    cells = scene.groups.values;
    label = scene.groups.name;
    fromGroups = true;
  } else {
    return needsSetup("choose a channel");         // unconfigured, not broken (design §3)
  }

  const numeric = mode === "numeric"
    || (mode === "auto" && !fromGroups && majorityNumeric(scene, cells));
  return numeric
    ? numericView(scene, cmap, cells, label, ghostOthers)
    : categoricalView(scene, cells, label, ghostOthers);
});
