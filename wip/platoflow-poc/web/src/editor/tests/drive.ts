// Headless interaction harness (plan §2.6): mounts the REAL editor app —
// initialDoc + makeUpdate + makeView — on a headless Gratify runtime and
// drives the actual input pipeline (pointerDown/Move/Up, key, step). No DOM,
// no Chrome: the runtime's header documents exactly this use.
//
// What is NOT mounted: the DOM organs index.ts wires around the runtime — the
// param popover (params.ts), the hover tooltip (help.ts), menu-bar chrome and
// the dispatch-wrap typing guard. They all live behind document/window seams,
// so headless specs stop at the intent boundary (e.g. clickParam proves the
// row hit-test + selection, not the popover). jsdom specs own the DOM organs.
//
// Coordinate rule (§4.4): every click point is computed through geom.nodeLayout
// offsets + the LIVE instance rect for the node's origin (position springs
// glide after a move, so doc x/y and drawn x/y differ mid-animation). Never
// magic numbers.
import { Runtime, rect, v, walk, type Instance, type Rect, type Vec } from "gratify";
import type { GraphDoc, GraphNode, NodeKindInfo } from "../../contracts";
import { KINDS, kindInfo } from "../../kinds";
import { initialDoc, makeUpdate, type EditorDoc, type EditorIntent } from "../doc";
import { makeView } from "../parts";
import {
  HEADER_H, nodeLayout, paramRowAt, paramRowRect, socketPos, wiredInputsOf,
  type NodeLayout, type SocketDir,
} from "../geom";

export interface SocketRef { node: string; dir: SocketDir; slot: string }

export interface NodeQuery {
  node: GraphNode;
  info: NodeKindInfo;
  l: NodeLayout;
  /** Live drawn origin (instance rect), not doc x/y — springs may be mid-glide. */
  origin: Vec;
}

export interface Drive {
  rt: Runtime<EditorDoc, EditorIntent>;
  doc(): EditorDoc;
  graph(): GraphDoc;
  dispatch(i: EditorIntent): void;
  load(doc: GraphDoc): void;
  step(n?: number, dt?: number): void;
  /** Run enough frames for springs/enter animations to settle. */
  settle(): void;
  /** Layout-query helper: doc node + kind info + nodeLayout + live origin. */
  layoutOf(id: string): NodeQuery;
  paramCenter(id: string, name: string): Vec;
  socketCenter(ref: SocketRef): Vec;
  headerCenter(id: string): Vec;
  click(p: Vec): void;
  drag(a: Vec, b: Vec, steps?: number): void;
  /** Click a param row (coords via nodeLayout); returns the row name the shared
   *  hit-test reports — asserting it === name is the round-trip proof. */
  clickParam(id: string, name: string): string | null;
  /** Drag from one socket to another through the real wire gesture. */
  dragSocket(from: SocketRef, to: SocketRef): void;
  /** Drag a node by its header through the real move gesture. */
  moveNode(id: string, dx: number, dy: number): void;
  /** Horizontal drag across a param row — for number params the widget
   *  gesture (T1) claims it and scrubs the value; `shift` = fine (×0.1). */
  scrub(id: string, name: string, dx: number, opts?: { shift?: boolean; steps?: number }): void;
  key(k: string): void;
  /** Live rect of any part instance by key (node id, palette row kind, …). */
  rectOfKey(key: string): Rect | null;
  /** Click the center of an instance by key (e.g. a palette row's kind). */
  clickKey(key: string): void;
}

export function createDrive(opts: { width?: number; height?: number } = {}): Drive {
  const rt = new Runtime<EditorDoc, EditorIntent>(null, {
    init: initialDoc(),
    update: makeUpdate({ kindInfo }),
    view: makeView(KINDS, kindInfo),
  }, { headless: true, width: opts.width ?? 1200, height: opts.height ?? 800 });

  const step = (n = 1, dt = 1 / 60) => rt.step(n, dt);
  const dispatch = (i: EditorIntent) => { rt.dispatch(i); step(1); };
  const graph = () => rt.doc.graph;

  const rectOfKey = (key: string): Rect | null => {
    let found: Rect | null = null;
    const look = (i: Instance) => { if (found === null && i.key === key) found = i.rect; };
    walk(rt.root, look);
    if (found === null) {
      // Adornment chips (help/eye/✕) live under the runtime's internal adorn
      // root, keyed `<host>::<chip>` — private field, but this is a test rig.
      const adornRoot = (rt as unknown as { adornRoot: Instance | null }).adornRoot;
      if (adornRoot) walk(adornRoot, look);
    }
    return found;
  };

  const layoutOf = (id: string): NodeQuery => {
    step(1);                                       // flush any pending doc → tree
    const node = graph().nodes.find((x) => x.id === id);
    if (!node) throw new Error(`drive: no node ${id}`);
    const info = kindInfo(node.kind);
    if (!info) throw new Error(`drive: unknown kind ${node.kind}`);
    const l = nodeLayout(info,
      { params: node.params, wiredInputs: wiredInputsOf(graph().edges, id, info), w: node.w, bh: node.bh },
      { helpOpen: !!rt.doc.helpOpen[id], zoom: rt.viewport.zoom },
      rt.doc.status[id]);
    const r = rectOfKey(id);
    return { node, info, l, origin: r ? v(r.x, r.y) : v(node.x, node.y) };
  };

  /** World → screen through the live viewport (identity unless a spec pans). */
  const toScreen = (p: Vec): Vec =>
    v(p.x * rt.viewport.zoom + rt.viewport.pan.x, p.y * rt.viewport.zoom + rt.viewport.pan.y);

  const click = (p: Vec) => { rt.pointerDown(p); rt.pointerUp(p); step(2); };

  const drag = (a: Vec, b: Vec, steps = 4) => {
    rt.pointerDown(a);
    for (let i = 1; i <= steps; i++) {
      rt.pointerMove(v(a.x + ((b.x - a.x) * i) / steps, a.y + ((b.y - a.y) * i) / steps));
      step(1);
    }
    rt.pointerUp(b);
    step(2);
  };

  const paramCenter = (id: string, name: string): Vec => {
    const { l, origin } = layoutOf(id);
    const i = l.paramNames.indexOf(name);
    if (i < 0) throw new Error(`drive: node ${id} has no param ${name}`);
    const r = paramRowRect(l, rect(origin.x, origin.y, l.w, l.h), i);
    return toScreen(r.center);
  };

  const socketCenter = (ref: SocketRef): Vec => {
    const { l, info, origin } = layoutOf(ref.node);
    const list = ref.dir === "in" ? info.inputs : info.outputs;
    const i = list.findIndex((s) => s.name === ref.slot);
    if (i < 0) throw new Error(`drive: node ${ref.node} has no ${ref.dir} socket ${ref.slot}`);
    return toScreen(socketPos(l, rect(origin.x, origin.y, l.w, l.h), ref.dir, i));
  };

  const headerCenter = (id: string): Vec => {
    const { l, origin } = layoutOf(id);
    return toScreen(v(origin.x + l.w / 2, origin.y + HEADER_H / 2));
  };

  return {
    rt,
    doc: () => rt.doc,
    graph,
    dispatch,
    load: (doc) => dispatch({ k: "graph", intent: { t: "load", doc } }),
    step,
    settle: () => step(90),                        // 1.5s simulated — springs at rest
    layoutOf,
    paramCenter,
    socketCenter,
    headerCenter,
    click,
    drag,
    clickParam(id, name) {
      const { l, origin } = layoutOf(id);
      const p = paramCenter(id, name);
      const w = rt.toWorld(p);
      click(p);
      return paramRowAt(l, origin.x, origin.y, w);
    },
    dragSocket: (from, to) => drag(socketCenter(from), socketCenter(to)),
    moveNode(id, dx, dy) {
      const a = headerCenter(id);
      drag(a, v(a.x + dx, a.y + dy));
    },
    scrub(id, name, dx, opts = {}) {
      const a = paramCenter(id, name);
      const steps = opts.steps ?? 4;
      const mods = { shift: !!opts.shift };
      rt.pointerDown(a, mods);
      for (let i = 1; i <= steps; i++) {
        rt.pointerMove(v(a.x + (dx * i) / steps, a.y), mods);
        step(1);
      }
      rt.pointerUp(v(a.x + dx, a.y));
      step(2);
    },
    key: (k) => { rt.key(k); step(2); },
    rectOfKey,
    clickKey(key) {
      const r = rectOfKey(key);
      if (!r) throw new Error(`drive: no instance with key ${key}`);
      click(toScreen(r.center));
    },
  };
}
