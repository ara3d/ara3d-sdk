// Wave-9 Track B fence: set algebra over entity selections, grouping, count.
//
// Channel-merge policy (design §14 open question, decided here as a data point):
// union/intersect merge `{ ...a.channels, ...b.channels }` — LAST-WINS, the second
// input's ChannelValue replaces the first's on a name clash, surfaced via
// NodeOut.warning (never silently). subtract passes the FIRST input's channels
// through untouched (per its kinds.ts description: B is a removal mask, its data
// does not travel). groups on union/intersect: FIRST input wins (a.groups ?? b.groups).
import type { Cell, SceneValue } from "../contracts";
import { def } from "./registry";
import { columnFor, derive, sceneIn, strParam } from "./lib";
import { fail, needsSetup, type NodeInputs, type NodeOut } from "./types";

// ---------- binary set ops ----------

/** Both scene inputs, guarded: set algebra is only defined within one model. */
const bothScenes = (inputs: NodeInputs): readonly [SceneValue, SceneValue] => {
  const a = sceneIn(inputs, "a");
  const b = sceneIn(inputs, "b");
  if (a.model.id !== b.model.id) fail("inputs come from different models");
  return [a, b] as const;
};

/** Merged channels (b wins) plus a warning naming each genuine clash — a name in
 *  both inputs bound to two DIFFERENT ChannelValue objects. The same object arriving
 *  down both branches (a diamond) is not a clash. */
function mergeChannels(a: SceneValue, b: SceneValue): { channels: SceneValue["channels"]; warning?: string } {
  const clashes = Object.keys(a.channels)
    .filter(k => k in b.channels && a.channels[k] !== b.channels[k])
    .map(k => `channel "${k}": second input wins`);
  return {
    channels: { ...a.channels, ...b.channels },
    warning: clashes.length > 0 ? clashes.join("; ") : undefined,
  };
}

/** Union/intersect result: merged channels (b wins, warned), a's groups preferred. */
function mergedScene(a: SceneValue, b: SceneValue, entities: Uint32Array): NodeOut {
  const { channels, warning } = mergeChannels(a, b);
  const groups = a.groups ?? b.groups;
  const value: SceneValue = { model: a.model, entities, channels, ...(groups ? { groups } : {}) };
  return warning !== undefined ? { value, warning } : { value };
}

def("select.union", async (_n, inputs) => {
  const [a, b] = bothScenes(inputs);
  const entities = Uint32Array.from(new Set([...a.entities, ...b.entities])).sort();
  return {
    ...mergedScene(a, b, entities),
    summary: `${a.entities.length} ∪ ${b.entities.length} → ${entities.length} entities`,
  };
});

def("select.intersect", async (_n, inputs) => {
  const [a, b] = bothScenes(inputs);
  const inB = new Set(b.entities);
  const entities = Uint32Array.from([...a.entities].filter(i => inB.has(i)));
  return {
    ...mergedScene(a, b, entities),
    summary: `${a.entities.length} ∩ ${b.entities.length} → ${entities.length} entities`,
  };
});

def("select.subtract", async (_n, inputs) => {
  const [a, b] = bothScenes(inputs);
  const inB = new Set(b.entities);
  const entities = Uint32Array.from([...a.entities].filter(i => !inB.has(i)));
  return {
    value: derive(a, entities),                    // a's channels and groups pass through
    summary: `${a.entities.length} − ${b.entities.length} → ${entities.length} entities`,
  };
});

def("select.invert", async (_n, inputs) => {
  const scene = sceneIn(inputs, "in");
  const selected = new Set(scene.entities);
  const out: number[] = [];
  for (let i = 0; i < scene.model.entityCount; i++) if (!selected.has(i)) out.push(i);
  const entities = Uint32Array.from(out);          // ascending by construction
  return {
    value: derive(scene, entities),                // channels/groups are full-length: no data loss
    summary: `${entities.length} of ${scene.model.entityCount} entities`,
  };
});

// ---------- grouping ----------

/** The full-length column the grouping key names. Type and Level come straight off
 *  the model; anything else resolves like By Parameter (channel wins over parameter). */
const groupSource = (scene: SceneValue, by: string): Cell[] => {
  if (by === "Type") return scene.model.types;
  if (by === "Level") return scene.model.levels;
  return columnFor(scene, by) ?? fail(`no parameter or channel named "${by}"`);
};

def("group.by", async (n, inputs) => {
  const scene = sceneIn(inputs, "in");
  const by = strParam(n, "by");
  if (!by) needsSetup("choose a grouping key");
  const src = groupSource(scene, by);

  // Full-length per entity; empty string counts as null (the unnamed group — real data).
  const values: (string | null)[] = new Array<string | null>(scene.model.entityCount).fill(null);
  for (let i = 0; i < scene.model.entityCount; i++) {
    const c = src[i] ?? null;
    values[i] = c === null || c === "" ? null : String(c);
  }

  const labels = new Set<string>();
  let unnamed = false;
  for (const i of scene.entities) {
    const v = values[i];
    if (v === null) unnamed = true;
    else labels.add(v);
  }

  const value: SceneValue = { ...scene, groups: { name: by, values } };
  return {
    value,
    summary: `${labels.size} groups over ${scene.entities.length} entities${unnamed ? " + unnamed" : ""}`,
  };
});

// ---------- count ----------

def("table.count", async (_n, inputs) => {
  const scene = sceneIn(inputs, "in");
  return {
    value: { columns: ["count"], rows: [[scene.entities.length]] },
    summary: `${scene.entities.length} entities`,
  };
});
