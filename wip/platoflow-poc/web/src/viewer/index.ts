// Track B — 3D viewer bridge over @ara3d/ara3d-webgl (BOS loader + three.js viewer).
//
// IMPORTANT: the npm package BUNDLES its own copy of three (0.182). The repo's
// top-level `three` dependency is 0.169. Importing 'three' directly here would
// create a second three instance whose materials/geometries the library's
// WebGLRenderer cannot use. So every three.js value in this file comes from the
// package's re-exported `THREE` namespace.

import { Viewer, BimOpenSchemaLoader, THREE } from "@ara3d/ara3d-webgl";
import type { Cell, ModelData, ParamInfo, ViewerBridge, ViewValue } from "../contracts";

// The package's index only exports the loader class, not the data types it
// returns, so we recover them structurally. (Loader friction — see NOTES.md.)
export type BimData = Awaited<ReturnType<BimOpenSchemaLoader["load"]>>;
export type Instance = BimData["Instances"][number];

const GHOST_COLOR = 0x8899aa;
const HIGHLIGHT_COLOR = 0xffee33;

/** Descriptor type codes used by BimResolver.GetVal: 2 = entity ref, 3 = string. */
const isNumericDescriptorType = (t: number) => t !== 2 && t !== 3;

/**
 * Fetches the .bos body with retries and per-stage reporting. The loader's own
 * `load(url)` swallows the fetch, which makes a stalled body indistinguishable
 * from a slow parse — a real problem behind flaky/instrumented proxies.
 */
async function fetchArrayBuffer(
  url: string,
  report: (msg: string) => void,
  attempts = 3
): Promise<ArrayBuffer> {
  let last: unknown;
  for (let attempt = 1; attempt <= attempts; attempt++) {
    const t0 = performance.now();
    try {
      report(`fetching ${url}${attempt > 1 ? ` (attempt ${attempt})` : ""} …`);
      const res = await fetch(url);
      if (!res.ok) throw new Error(`HTTP ${res.status} ${res.statusText}`);
      report(`headers ${res.status} in ${Math.round(performance.now() - t0)} ms — reading body …`);
      const buf = await res.arrayBuffer();
      console.log(
        `[viewer] fetched ${url}: ${buf.byteLength} bytes in ${Math.round(performance.now() - t0)} ms`
      );
      return buf;
    } catch (e) {
      last = e;
      console.warn(`[viewer] fetch attempt ${attempt} for ${url} failed`, e);
    }
  }
  throw last instanceof Error ? last : new Error(String(last));
}

// ---------------------------------------------------------------- ModelData

/** Builds the contract-level ModelData over a loaded BimData. Pure — Node-testable. */
export function buildModelData(id: string, bim: BimData): ModelData {
  const res = bim.Resolver;
  const strings = bim.Strings ?? [];
  const str = (i: number): string => (i >= 0 ? strings[i] ?? "" : "");

  const entityCount = res.EntityCount;
  const globalIds = new Array<string>(entityCount);
  const types = new Array<string>(entityCount);
  const names = new Array<string>(entityCount);

  const ents = bim.Entities;
  for (let i = 0; i < entityCount; i++) {
    globalIds[i] = str(ents.GlobalId?.[i] ?? -1);
    names[i] = str(ents.Name?.[i] ?? -1);
    const cat = ents.Category?.[i] ?? -1;
    types[i] = cat >= 0 ? str(ents.Name?.[cat] ?? -1) : "";
  }

  // Parameter metadata comes from the Descriptors table (name / group / type).
  const infos: ParamInfo[] = [];
  const seen = new Set<string>();
  const desc = bim.Descriptors;
  if (desc?.Name) {
    for (let d = 0; d < desc.Name.length; d++) {
      const name = str(desc.Name[d]);
      if (!name || seen.has(name)) continue;
      seen.add(name);
      infos.push({
        name,
        pset: str(desc.Group?.[d] ?? -1),
        numeric: isNumericDescriptorType(desc.Type?.[d] ?? 0),
      });
    }
    infos.sort((a, b) => a.name.localeCompare(b.name));
  }

  // Values live in Resolver.ParameterMap (entity -> [{Name, Value}]). There is no
  // by-name index, so a column costs one scan of the map; we cache each column.
  const columns = new Map<string, Cell[]>();
  const param = (name: string): Cell[] => {
    const hit = columns.get(name);
    if (hit) return hit;
    const t0 = performance.now();
    const col: Cell[] = new Array(entityCount).fill(null);
    for (const [entity, params] of res.ParameterMap) {
      if (entity < 0 || entity >= entityCount) continue;
      for (const p of params) {
        if (p.Name !== name) continue;
        const v = p.Value;
        col[entity] = typeof v === "number" || typeof v === "string" ? v : v == null ? null : String(v);
        break;
      }
    }
    columns.set(name, col);
    console.log(`[viewer] param column "${name}" built in ${(performance.now() - t0).toFixed(1)} ms`);
    return col;
  };

  // Level: BOS/Revit models store it as "Rvt:Element:Level"; fall back to anything
  // whose descriptor name ends in ":Level" or is exactly "Level".
  const levelName =
    infos.find((p) => p.name === "Rvt:Element:Level")?.name ??
    infos.find((p) => p.name.endsWith(":Level") || p.name === "Level")?.name;
  const levels: (string | null)[] = levelName
    ? param(levelName).map((c) => (c == null || c === "" ? null : String(c)))
    : new Array(entityCount).fill(null);

  return { id, entityCount, globalIds, types, names, levels, paramNames: () => infos, param };
}

// ------------------------------------------------------------ view planning

export interface ViewMaterials {
  /** shared material for one (quantized) rgb colour, components 0..1 */
  solid(r: number, g: number, b: number): THREE.Material;
  ghost: THREE.Material;
}

/** Model-space AABB context for box views (W10). `unitBox` is a shared
 *  position+index-only cube spanning [-0.5,0.5]^3 — the same attribute set BOS
 *  geometry carries, so the library's merge path accepts it. */
export interface BoxContext {
  unitBox: THREE.BufferGeometry;
  /** entity index -> model-space AABB over all its instances (drawables only) */
  entityBoxes: Map<number, THREE.Box3>;
}

/** Centered unit cube with ONLY position + index (BOS-shaped; mergeable). */
export function makeUnitBoxGeometry(): THREE.BufferGeometry {
  const p: number[] = [];
  for (const z of [-0.5, 0.5]) for (const y of [-0.5, 0.5]) for (const x of [-0.5, 0.5]) p.push(x, y, z);
  // 12 triangles over the 8 corners (winding irrelevant: materials are DoubleSide)
  const idx = [0,1,3, 0,3,2, 4,6,7, 4,7,5, 0,4,5, 0,5,1, 2,3,7, 2,7,6, 0,2,6, 0,6,4, 1,5,7, 1,7,3];
  const g = new THREE.BufferGeometry();
  g.setAttribute("position", new THREE.BufferAttribute(Float32Array.from(p), 3));
  g.setIndex(new THREE.BufferAttribute(Uint32Array.from(idx), 1));
  return g;
}

/**
 * Per-entity model-space AABBs: union of each instance's geometry bounding box
 * transformed by its matrix. `computeBoundingBox` caches on the geometry, so the
 * cost is one Box3.applyMatrix4 per instance — never a raw vertex sweep.
 */
export function computeEntityBoxes(byEntity: Map<number, Instance[]>): Map<number, THREE.Box3> {
  const t0 = performance.now();
  const out = new Map<number, THREE.Box3>();
  const tmp = new THREE.Box3();
  for (const [e, insts] of byEntity) {
    const box = new THREE.Box3();
    for (const inst of insts) {
      if (!inst.geometry.boundingBox) inst.geometry.computeBoundingBox();
      tmp.copy(inst.geometry.boundingBox!);
      if (!inst.isIdentity) tmp.applyMatrix4(inst.transform);
      box.union(tmp);
    }
    if (!box.isEmpty()) out.set(e, box);
  }
  console.log(`[viewer] entity boxes: ${out.size} in ${(performance.now() - t0).toFixed(1)} ms`);
  return out;
}

const MIN_BOX = 1e-4; // zero-thickness AABBs break InstancedMesh raycasting

/** unitBox -> this entity's AABB (optionally offset). */
function boxTransform(box: THREE.Box3, off?: [number, number, number]): THREE.Matrix4 {
  const c = box.getCenter(new THREE.Vector3());
  const s = box.getSize(new THREE.Vector3());
  if (off) c.set(c.x + off[0], c.y + off[1], c.z + off[2]);
  return new THREE.Matrix4().compose(
    c,
    new THREE.Quaternion(),
    new THREE.Vector3(Math.max(s.x, MIN_BOX), Math.max(s.y, MIN_BOX), Math.max(s.z, MIN_BOX))
  );
}

/**
 * Turns a ViewValue into the instance list `BimData.rebuildGeometry` wants.
 * Pure apart from the material factory — this is the whole recolour algorithm,
 * so it is exported and exercised directly by the Node perf harness.
 *
 * W10: honours `view.offsets` (per-entity model-space translation — instances
 * get a pre-multiplied translation matrix, geometry untouched) and `view.boxes`
 * (entities become synthetic unit-box instances scaled to their AABB; `boxes`
 * ctx required). Neither present → exactly the original path.
 */
export function planInstances(
  bim: BimData,
  view: ViewValue,
  mats: ViewMaterials,
  byEntity: Map<number, Instance[]>,
  boxes?: BoxContext
): Instance[] {
  const selected = new Map<number, THREE.Material>();
  const colors = view.colors;
  for (let k = 0; k < view.entities.length; k++) {
    const e = view.entities[k];
    const mat = colors
      ? mats.solid(colors[k * 3], colors[k * 3 + 1], colors[k * 3 + 2])
      : undefined;
    selected.set(e, mat ?? byEntity.get(e)?.[0]?.material ?? mats.ghost);
  }

  const offs = view.offsets ?? null;
  const offsetIndex = new Map<number, number>();
  if (offs) for (let k = 0; k < view.entities.length; k++) offsetIndex.set(view.entities[k], k);
  const offsetOf = (e: number): [number, number, number] | undefined => {
    if (!offs) return undefined;
    const k = offsetIndex.get(e);
    if (k === undefined) return undefined;
    return [offs[k * 3], offs[k * 3 + 1], offs[k * 3 + 2]];
  };

  // ---- box mode: every drawn entity becomes one synthetic AABB instance
  if (view.boxes && boxes) {
    const instances: Instance[] = [];
    const emit = (e: number, mat: THREE.Material, off?: [number, number, number]) => {
      const box = boxes.entityBoxes.get(e);
      const rep = byEntity.get(e)?.[0];
      if (!box || !rep) return; // non-drawable entity: nothing to box
      instances.push({
        isIdentity: false,
        instance: rep.instance, // picking maps this back to the entity
        entity: rep.entity, // === e, but branded EntityIndex
        geometry: boxes.unitBox,
        material: mat,
        transform: boxTransform(box, off),
        visible: true,
      });
    };
    for (const [e, mat] of selected) emit(e, mat, offsetOf(e));
    if (view.ghostOthers)
      for (const e of boxes.entityBoxes.keys()) if (!selected.has(e)) emit(e, mats.ghost);
    return instances;
  }

  // ---- mesh mode (original path; offsets pre-translate the instance matrix)
  const instances: Instance[] = [];
  for (const inst of bim.Instances) {
    const mat = selected.get(inst.entity);
    if (mat) {
      const off = offsetOf(inst.entity);
      if (off) {
        const t = new THREE.Matrix4().makeTranslation(off[0], off[1], off[2]).multiply(inst.transform);
        instances.push({ ...inst, material: mat, transform: t, isIdentity: false });
      } else instances.push(mat === inst.material ? inst : { ...inst, material: mat });
    } else if (view.ghostOthers) instances.push({ ...inst, material: mats.ghost });
    // else: hidden — simply not emitted
  }
  return instances;
}

/** entity index -> instances that draw it. */
export function indexInstancesByEntity(bim: BimData): Map<number, Instance[]> {
  const byEntity = new Map<number, Instance[]>();
  for (const inst of bim.Instances) {
    const list = byEntity.get(inst.entity);
    if (list) list.push(inst);
    else byEntity.set(inst.entity, [inst]);
  }
  return byEntity;
}

/** One shared material per quantized colour keeps the merge groups small. */
export function makeMaterialCache(): ViewMaterials & { size(): number } {
  const cache = new Map<number, THREE.MeshStandardMaterial>();
  const ghost = new THREE.MeshStandardMaterial({
    color: GHOST_COLOR,
    transparent: true,
    opacity: 0.12,
    roughness: 0.9,
    metalness: 0,
    depthWrite: false,
    side: THREE.DoubleSide,
  });
  const solid = (r: number, g: number, b: number): THREE.MeshStandardMaterial => {
    const key = (Math.round(r * 255) << 16) | (Math.round(g * 255) << 8) | Math.round(b * 255);
    let m = cache.get(key);
    if (!m) {
      m = new THREE.MeshStandardMaterial({
        color: new THREE.Color(r, g, b),
        roughness: 0.85,
        metalness: 0,
        flatShading: true,
        side: THREE.DoubleSide,
      });
      cache.set(key, m);
    }
    return m;
  };
  return { solid, ghost, size: () => cache.size };
}

// ------------------------------------------------------------------ Viewer

/** Returns the contract's ViewerBridge, widened with a few debug handles. */
export function createViewer(container: HTMLElement): ViewerBridge & DebugHandles {
  // Viewport.getOrCreateCanvas() looks the canvas up by DOM id, so the element
  // has to exist inside our container before the Viewer is constructed.
  const canvasId = `ara3d-canvas-${Math.random().toString(36).slice(2, 8)}`;
  const canvas = document.createElement("canvas");
  canvas.id = canvasId;
  canvas.tabIndex = 0;
  canvas.style.display = "block";
  container.appendChild(canvas);

  const viewer = new Viewer({
    // library default debounces resize 200ms — visible lag while dragging the
    // pane splitter (T19); tighten it
    canvas: { id: canvasId, resizeDelay: 40 },
    fileDrop: { enable: false },
  });

  let bim: BimData | null = null;
  let model: ModelData | null = null;
  /** entity index -> instances that draw it */
  let byEntity = new Map<number, Instance[]>();
  /** InstanceIndex -> Instance. Instances[] is filtered, so array position != InstanceIndex. */
  let byInstanceIndex = new Map<number, Instance>();
  /** geometries owned by ANY loaded model's loader; never dispose these.
   *  CUMULATIVE across models (W13 multi-model) — a cached model's geometry
   *  must survive being swapped out. */
  const loaderGeometries = new Set<THREE.BufferGeometry>();

  // W13 multi-model: every parsed model is kept; applyView switches the active
  // one to match the view's model, so a multi-discipline graph shows the
  // building each clicked node actually belongs to.
  interface CachedModel {
    bim: BimData;
    model: ModelData;
    byEntity: Map<number, Instance[]>;
    byInstanceIndex: Map<number, Instance>;
    baseGroup: THREE.Group;
    entityBoxes: Map<number, THREE.Box3> | null;
  }
  const modelCache = new Map<string, CachedModel>();
  /** Base groups of every cached model — disposeGroup must never touch them. */
  const allBaseGroups = new Set<THREE.Group>();
  let activeCache: CachedModel | null = null;

  let baseGroup: THREE.Group | null = null; // the loader's original group
  let currentGroup: THREE.Group | null = null; // what is in the scene now
  let overlay: THREE.Group | null = null; // highlight overlay
  let currentView: ViewValue | null = null;
  /** lazy per-entity AABBs for box views; reset on load */
  let entityBoxes: Map<number, THREE.Box3> | null = null;
  /** entity -> offset of the CURRENT view (highlight follows exploded positions) */
  let viewOffsets: Map<number, [number, number, number]> | null = null;
  // Shared unit cube for box views. Must never be disposed: singleton box
  // instances reference it directly in the rebuilt group.
  const unitBox = makeUnitBoxGeometry();

  const materials = makeMaterialCache();
  // Unlit on purpose: BOS geometry carries no normals (the loader builds only
  // position + index), so a lit material renders the overlay black or not at all.
  const highlightMaterial = new THREE.MeshBasicMaterial({
    color: HIGHLIGHT_COLOR,
    side: THREE.DoubleSide,
    depthTest: false,
    transparent: true,
    opacity: 0.85,
  });

  /** Frees the merged geometries a rebuild allocated (loader-owned ones stay). */
  const disposeGroup = (g: THREE.Group | null) => {
    if (!g || g === baseGroup || allBaseGroups.has(g)) return;
    g.traverse((o: THREE.Object3D) => {
      const mesh = o as THREE.Mesh;
      if (mesh.geometry && !loaderGeometries.has(mesh.geometry) && mesh.geometry !== unitBox)
        mesh.geometry.dispose();
    });
  };

  const swapGroup = (next: THREE.Group) => {
    if (currentGroup) viewer.remove(currentGroup);
    disposeGroup(currentGroup);
    currentGroup = next;
    viewer.add(next);
  };

  const frameModel = (g: THREE.Group) => {
    try {
      g.updateMatrixWorld(true);
      const box = new THREE.Box3().setFromObject(g);
      if (box.isEmpty()) return;
      viewer.camera.do().frame(box);
    } catch (e) {
      console.warn("[viewer] framing failed", e);
    }
  };

  // ---------------------------------------------------------------- load

  /** Progress/phase reporter — the harness wires this to the status line. */
  let onPhase: (msg: string) => void = () => {};

  const loadBuffer = async (buffer: ArrayBuffer, id: string): Promise<ModelData> => {
    const t0 = performance.now();
    onPhase(`parsing ${(buffer.byteLength / 1024) | 0} KB …`);
    const data = await new BimOpenSchemaLoader().loadFromArrayBuffer(buffer, {
      loadParameters: true,
    });
    const tLoad = performance.now() - t0;

    const t1 = performance.now();
    const cached: CachedModel = {
      bim: data,
      model: buildModelData(id, data),
      byEntity: indexInstancesByEntity(data),
      byInstanceIndex: new Map(),
      baseGroup: data.ThreeGeometry,
      entityBoxes: null,
    };
    const tModel = performance.now() - t1;
    for (const inst of data.Instances) {
      loaderGeometries.add(inst.geometry);
      cached.byInstanceIndex.set(inst.instance, inst);
    }
    modelCache.set(id, cached);
    allBaseGroups.add(cached.baseGroup);
    activate(id, true);

    console.log(
      `[viewer] loaded "${id}": ${cached.model.entityCount} entities, ${data.Instances.length} instances, ` +
        `${cached.model.paramNames().length} parameters — parse ${tLoad.toFixed(0)} ms, model ${tModel.toFixed(0)} ms`
    );
    return cached.model;
  };

  /** Make a cached model the ACTIVE one (module singletons + scene group).
   *  False when the id was never loaded. No-op when already active. */
  const activate = (id: string, frame = false): boolean => {
    const c = modelCache.get(id);
    if (!c) return false;
    if (activeCache === c) return true;
    activeCache = c;
    bim = c.bim;
    model = c.model;
    byEntity = c.byEntity;
    byInstanceIndex = c.byInstanceIndex;
    baseGroup = c.baseGroup;
    entityBoxes = c.entityBoxes;
    currentView = null;
    viewOffsets = null;
    highlight(null);
    swapGroup(c.baseGroup);
    if (frame) frameModel(c.baseGroup);
    return true;
  };

  const load = async (bosUrl: string, id: string): Promise<ModelData> => {
    const hit = modelCache.get(id);
    if (hit) { activate(id, true); return hit.model; }   // cached: no re-parse
    return loadBuffer(await fetchArrayBuffer(bosUrl, onPhase), id);
  };

  // ------------------------------------------------------------ applyView

  const getEntityBoxes = (): Map<number, THREE.Box3> => {
    entityBoxes ??= computeEntityBoxes(byEntity);
    if (activeCache) activeCache.entityBoxes = entityBoxes;  // write back: survives a model switch
    return entityBoxes;
  };

  const applyView = (view: ViewValue | null): void => {
    // W13 multi-model: a view knows its model — switch the active one first so
    // a multi-discipline graph shows the building the clicked node belongs to.
    const vid = view?.model?.id;
    if (vid && model?.id !== vid && !activate(vid, true)) {
      console.warn(`[viewer] applyView: model "${vid}" not loaded — keeping current`);
      return;
    }
    if (!bim || !baseGroup) return;
    currentView = view;
    viewOffsets = null;

    if (!view) {
      swapGroup(baseGroup);
      return;
    }

    if (view.offsets) {
      viewOffsets = new Map();
      for (let k = 0; k < view.entities.length; k++)
        viewOffsets.set(view.entities[k], [
          view.offsets[k * 3],
          view.offsets[k * 3 + 1],
          view.offsets[k * 3 + 2],
        ]);
    }

    const t0 = performance.now();
    const boxCtx = view.boxes ? { unitBox, entityBoxes: getEntityBoxes() } : undefined;
    const instances = planInstances(bim, view, materials, byEntity, boxCtx);
    const tPrep = performance.now() - t0;
    const t1 = performance.now();
    swapGroup(bim.rebuildGeometry(instances));
    const tBuild = performance.now() - t1;

    console.log(
      `[viewer] applyView "${view.label ?? ""}": ${view.entities.length} entities / ` +
        `${instances.length} instances, ${materials.size()} materials — ` +
        `prep ${tPrep.toFixed(1)} ms, rebuild ${tBuild.toFixed(1)} ms` +
        `${view.boxes ? " [boxes]" : ""}${view.offsets ? " [offsets]" : ""}`
    );

    // The highlight overlay lives outside the group, so it survives the swap.
    if (overlay) viewer.renderer.needsUpdate = true;
  };

  // ------------------------------------------------------------ highlight

  const highlight = (entities: Uint32Array | null): void => {
    if (overlay) {
      viewer.renderer.remove(overlay);
      disposeGroup(overlay);
      overlay = null;
    }
    if (!entities || entities.length === 0 || !bim) {
      viewer.requestRender();
      return;
    }

    // Small overlay group drawn on top; leaves the current view untouched.
    // Follows the current view's shape: exploded entities highlight at their
    // offset position, box views highlight the AABB rather than the mesh.
    const g = new THREE.Group();
    let count = 0;
    const asBoxes = currentView?.boxes === true;
    for (const e of entities) {
      const off = viewOffsets?.get(e);
      if (asBoxes) {
        const box = getEntityBoxes().get(e);
        if (!box) continue;
        const mesh = new THREE.Mesh(unitBox, highlightMaterial);
        mesh.matrixAutoUpdate = false;
        mesh.matrix.copy(boxTransform(box, off));
        mesh.renderOrder = 999;
        g.add(mesh);
        count++;
        continue;
      }
      for (const inst of byEntity.get(e) ?? []) {
        const mesh = new THREE.Mesh(inst.geometry, highlightMaterial);
        mesh.matrixAutoUpdate = false;
        if (off)
          mesh.matrix
            .makeTranslation(off[0], off[1], off[2])
            .multiply(inst.transform);
        else mesh.matrix.copy(inst.transform);
        mesh.renderOrder = 999;
        g.add(mesh);
        count++;
      }
    }
    if (count === 0) {
      viewer.requestRender();
      return;
    }
    g.rotation.x = -Math.PI / 2; // same Z-up -> Y-up flip buildGeometry applies
    overlay = g;
    viewer.renderer.add(g);
    viewer.renderer.needsUpdate = true;
    viewer.requestRender();
  };

  // --------------------------------------------------------------- picking

  const raycaster = new THREE.Raycaster();
  const pickCallbacks: ((e: number | null) => void)[] = [];
  const DRAG_THRESHOLD = 5;
  let downAt: { x: number; y: number } | null = null;

  const decodeHit = (hit: THREE.Intersection): number | null => {
    const pick = (hit.object.userData as any)?.pick;
    if (!pick) return null;
    if (pick.kind === "single") return pick.instanceIndex ?? null;
    if (pick.kind === "instanced")
      return hit.instanceId !== undefined ? pick.instanceIndices?.[hit.instanceId] ?? null : null;
    if (pick.kind === "merged")
      return hit.faceIndex !== undefined ? pick.triToInstanceIndex?.[hit.faceIndex] ?? null : null;
    return null;
  };

  canvas.addEventListener("pointerdown", (ev) => {
    if (ev.button === 0) downAt = { x: ev.clientX, y: ev.clientY };
  });

  canvas.addEventListener("pointerup", (ev) => {
    if (ev.button !== 0 || !downAt) return;
    const moved = Math.hypot(ev.clientX - downAt.x, ev.clientY - downAt.y);
    downAt = null;
    if (moved > DRAG_THRESHOLD || !currentGroup || !bim) return;

    const rect = canvas.getBoundingClientRect();
    const ndc = new THREE.Vector2(
      ((ev.clientX - rect.left) / rect.width) * 2 - 1,
      -((ev.clientY - rect.top) / rect.height) * 2 + 1
    );
    raycaster.setFromCamera(ndc, viewer.camera.three as THREE.Camera);

    let entity: number | null = null;
    for (const hit of raycaster.intersectObject(currentGroup, true)) {
      const instanceIndex = decodeHit(hit);
      if (instanceIndex == null) continue;
      const match = byInstanceIndex.get(instanceIndex);
      if (match) {
        entity = match.entity;
        break;
      }
    }
    for (const cb of pickCallbacks) cb(entity);
  });

  const onPick = (cb: (entityIndex: number | null) => void): void => {
    pickCallbacks.push(cb);
  };

  const bridge: ViewerBridge & DebugHandles = {
    load,
    applyView,
    highlight,
    onPick,
    viewer,
    loadBuffer,
    geometryEntities: () =>
      Uint32Array.from([...byEntity.keys()].sort((a, b) => a - b)),
    setPhaseReporter: (fn) => {
      onPhase = fn;
    },
    get bim() {
      return bim;
    },
    get model() {
      return model;
    },
    get view() {
      return currentView;
    },
  };
  return bridge;
}

/** Escape hatches for the harness / console poking; not part of the contract. */
export interface DebugHandles {
  viewer: Viewer;
  /** Skip the fetch (useful when the network layer is the flaky part). */
  loadBuffer(buffer: ArrayBuffer, id: string): Promise<ModelData>;
  /**
   * Entity indices that actually have geometry, ascending. `ModelData` has no
   * such notion, and in real BOS models most entities are non-drawable
   * (materials, views, levels, family types) — see NOTES.md.
   */
  geometryEntities(): Uint32Array;
  setPhaseReporter(fn: (msg: string) => void): void;
  readonly bim: unknown;
  readonly model: ModelData | null;
  readonly view: ViewValue | null;
}
