// Track A's public seam. `createEditor` mounts the Gratify graph editor on a
// canvas and hands back a small imperative handle: external intents (MCP, the
// demo loader) go in through `dispatch`, eval statuses through `setResults`.
//
// There is no side inspector panel: the node card itself shows params, help,
// status and controls. Clicking a param row opens ONE floating DOM editor
// (params.ts) positioned over the row via the shared geom layout.
//
// Every graph mutation — gesture, palette, popover or external — is routed
// through `applyIntent` in ../reducer; nothing here edits a GraphDoc directly.
//
// T14 (upstream seams): DOM chrome stays in step via AppSpec.onCommit (no
// dispatch wrapping); the typing guard is gratify's isEditableDom (the
// runtime's own window keydown consults it, so canvas keys never fire while a
// DOM field has focus); popover + picker ride an island() — the runtime pins
// them over the edited param row's WORLD rect through pan/zoom, so editors
// survive pan/zoom and track a dragged node. Close rules that remain: Esc,
// clean-click-away, selection change, external dispatch. Close-on-pan and
// close-on-wheel are deleted — they were workarounds for fixed-position DOM.
import { isEditableDom, mount, part, rect, v, type IslandSpec } from "gratify";
import type { EvalResult, GraphDoc, GraphNode, Intent, NodeKindInfo, NodeStatus } from "../contracts";
import { createChrome, MENUBAR_H, type Chrome, type ChromeSpec } from "./chrome";
import {
  anyPending, copySelection, initialDoc, makeUpdate, pasteIntent, selectedNode,
  selectedNodeIds, selFromIds, type EditorDoc, type EditorIntent, type NodeClip,
} from "./doc";
import {
  HEADER_H, inHeader, nodeLayout, paramRowRect, resizeHandleAt, socketAt, subInfo,
  widgetFor, wiredInputsOf, type NodeLayout,
} from "./geom";
import {
  collapseSelection, commitExit, createBreadcrumb, enterDoc, expandNode, subgraphKeyAction,
} from "./subgraph";
import { placeFree, tidyLayout } from "./layout";
import { createContextMenu, type MenuEntry } from "./contextmenu";
import { createNodeTip, type NodeTip } from "./help";
import { createParamPopover, type ParamPopover } from "./params";
import { makeView } from "./parts";

export interface EditorOpts {
  onDocChange?(doc: GraphDoc): void;
  /** Write Pset (or any `effect: "write"` kind) run button clicked. */
  onRunEffect?(nodeId: string): void;
  /** Options for `dynamicEnum` / `modelPick` params. `source` is the schema's.
   *  Entries may carry a count ("Walls (216)") — plain strings stay valid, so
   *  existing providers are untouched; consumers normalize. */
  getEnumOptions?(node: GraphNode, param: string, source: string): (string | { value: string; count?: number })[];
  /** Selected node changed (null = nothing selected, the selection vanished,
   *  or a MULTI-selection — a set has no "the" node, so multi reports null and
   *  closes the param popover; display/help/popover stay single-node keyed). */
  onSelect?(nodeId: string | null): void;
  /** Provide to mount the menu bar + node palette. Omit for a bare canvas. */
  chrome?: ChromeSpec;
}

export interface EditorApp {
  /** External intents (MCP, demo loader) enter here. Pass `coalesceKey` to
   *  collapse a burst of consecutive same-key dispatches (e.g. one MCP agent
   *  turn) into a single undo step; omit for one undo step per intent. */
  dispatch(intent: Intent, coalesceKey?: string): void;
  getDoc(): GraphDoc;
  /** Animate pan/zoom so the whole graph fits the visible canvas (insets for
   *  the menu bar and the open Add Node panel). No-op on an empty graph. */
  fitToFrame(): void;
  /** W13: auto-layout the graph (one undoable move batch), then fit. Same as
   *  View ▸ Tidy graph / T — exposed so example loads arrive pre-tidied. */
  tidy(): void;
  /** Status chips + result badges. */
  setResults(r: EvalResult): void;
  /** Host status text in the menu bar (no-op without chrome). */
  setStatus(msg: string): void;
  destroy(): void;
}

/** Typing guard for OUR OWN window listeners (history, clipboard, subgraph
 *  keys): upstream `isEditableDom` semantics over the focused element. The
 *  canvas surface's key maps (Delete/Backspace/Escape) need no guard anymore —
 *  gratify's window keydown consults isEditableDom(ev.target) before routing. */
const typingInDom = (): boolean => isEditableDom(document.activeElement);

/** Keyboard → history intent: Ctrl/⌘-Z undo, Ctrl/⌘-Y or Ctrl/⌘-Shift-Z redo.
 *  Pure decode, exported for specs. */
export const undoKeyIntent = (
  ev: { key: string; ctrlKey: boolean; metaKey: boolean; shiftKey: boolean },
): EditorIntent | null => {
  if (!(ev.ctrlKey || ev.metaKey)) return null;
  const k = ev.key.toLowerCase();
  if (k === "z") return { k: ev.shiftKey ? "redo" : "undo" };
  if (k === "y" && !ev.shiftKey) return { k: "redo" };
  return null;
};

/** The window keydown handler for undo/redo, factored so headless specs can
 *  drive it with a synthetic event + a Drive dispatch. While a DOM field has
 *  focus, history keys are ignored entirely (no preventDefault) so Ctrl-Z in
 *  a textarea stays the browser's native text undo. */
export const historyKeyHandler = (
  dispatch: (i: EditorIntent) => void,
  typing: () => boolean = typingInDom,
) => (ev: { key: string; ctrlKey: boolean; metaKey: boolean; shiftKey: boolean;
            preventDefault(): void }): void => {
  const intent = undoKeyIntent(ev);
  if (!intent || typing()) return;
  ev.preventDefault();
  dispatch(intent);
};

/** Keyboard → clipboard action: Ctrl/⌘-C copy, Ctrl/⌘-V paste, Ctrl/⌘-D
 *  duplicate. Pure decode, exported for specs (T13). */
export const clipboardKeyAction = (
  ev: { key: string; ctrlKey: boolean; metaKey: boolean; shiftKey: boolean },
): "copy" | "paste" | "duplicate" | null => {
  if (!(ev.ctrlKey || ev.metaKey) || ev.shiftKey) return null;
  switch (ev.key.toLowerCase()) {
    case "c": return "copy";
    case "v": return "paste";
    case "d": return "duplicate";
    default: return null;
  }
};

/** +24,+24 world px from the ORIGINALS on every paste/duplicate (T13). */
export const PASTE_OFFSET = 24;

export function createEditor(
  canvas: HTMLCanvasElement,
  kinds: NodeKindInfo[],
  opts: EditorOpts = {},
): EditorApp {
  const byKind = new Map(kinds.map((k) => [k.kind, k]));
  const kindInfo = (k: string) => byKind.get(k);

  const host = canvas.parentElement ?? document.body;
  if (getComputedStyle(host).position === "static") host.style.position = "relative";

  const topInset = opts.chrome ? MENUBAR_H : 0;

  let tip: NodeTip | null = null;
  let popover: ParamPopover | null = null;
  let chrome: Chrome | null = null;

  // ── T14 island: popover + picker ride ONE island wrapper ───────────────────
  // The runtime pins `islandWrap` over the edited param row's WORLD rect every
  // frame (pan/zoom/node-drag all tracked); the organ roots are absolutely
  // positioned at its origin and keep their natural size, overflowing the
  // zero-height anchor. The facet always returns a spec (empty rect when no
  // editor is open) so the wrapper stays mounted — reopening after a close
  // must not lose the organ's focus to a detached-DOM race. The part is an
  // invisible zero-size leaf appended to the view root: an app-level singleton
  // has no natural card host, so it gets a dedicated chrome part.
  const islandWrap = document.createElement("div");
  islandWrap.style.overflow = "visible";
  const EditorIsland = part("pf-editor-island")
    .props<Record<string, never>>()
    .intrinsic(0, 0)
    .island((): IslandSpec => {
      const t = popover?.islandTarget() ?? null;
      if (!t) return { el: islandWrap, rect: rect(0, 0, 0, 0) };
      if (t.el.parentElement !== islandWrap) islandWrap.appendChild(t.el);
      return { el: islandWrap, rect: rowWorldRect(t.node, t.name) };
    });
  const baseView = makeView(kinds, kindInfo, topInset);

  /** External (MCP/demo) and history dispatches must not trigger the
   *  auto-open picker — they are not user gestures. Synchronous flag. */
  let quiet = false;
  const runQuiet = (f: () => void) => { quiet = true; try { f(); } finally { quiet = false; } };

  /** rAF id of the running fit-to-frame tween — ambient() keeps the runtime
   *  awake while it is set, so the viewport mutations actually paint. */
  let fitAnim: number | null = null;

  const rt = mount(canvas, {
    init: initialDoc(),
    update: makeUpdate({ kindInfo, onDocChange: opts.onDocChange, onRunEffect: opts.onRunEffect }),
    view: (doc: EditorDoc) => {
      const el = baseView(doc);
      return { ...el, children: [...(el.children ?? []), EditorIsland("pf-editor-island", {})] };
    },
    ambient: (doc) => anyPending(doc) || fitAnim !== null,  // pending pulse + fit tween keep the loop awake
    // The upstream embedding seam (T14): called after every COMMITTED update.
    // Everything the old dispatch wrap did with the NEW doc lives here.
    onCommit: (doc: EditorDoc, prev: EditorDoc) => {
      syncSel();
      syncChrome();
      // T8 auto-open (study #13): a user-gesture node add with a required
      // unset pickable param asks its question immediately. `quiet` excludes
      // external + undo/redo; `past.length === 0` excludes `load` (load
      // clears history — the demo loader and subgraph enter/exit ride it).
      if (quiet || doc.graph === prev.graph || doc.past.length === 0) return;
      const had = new Set(prev.graph.nodes.map((n) => n.id));
      const added = doc.graph.nodes.filter((n) => !had.has(n.id));
      if (added.length) popover?.openFirstUnset(added[added.length - 1]);
    },
  });

  // Edit-menu enabled state tracks history depth; cheap enough to push on
  // every commit (same pattern as syncSel below).
  const syncChrome = () =>
    chrome?.syncHistory(rt.doc.past.length > 0, rt.doc.future.length > 0);

  // A selection change is anything that leaves a DIFFERENT node selected —
  // including one that removed the node the selection pointed at.
  const selId = (d: EditorDoc) => selectedNode(d)?.id ?? null;
  let lastSel = selId(rt.doc);
  const syncSel = () => {
    const id = selId(rt.doc);
    if (id === lastSel) return;
    lastSel = id;
    popover?.close();                            // selection change closes the editor
    opts.onSelect?.(id);
  };

  // T14: the dispatch wrap shrank to its ONE remaining intent-drop guard.
  // The surface's press fires select-null on the very click that dismisses an
  // open editor (click-away) — dropping it keeps dismissal from deselecting
  // (T2's "dismissal churn" rule). It cannot move into `update` (pure — no
  // DOM state) and upstream modal capture is adornment-only, so islands have
  // no consume-the-press seam; one line stays. Everything else the wrap did
  // (typing guard → upstream isEditableDom; chrome sync + auto-open →
  // AppSpec.onCommit) is gone.
  const commit = rt.dispatch;
  rt.dispatch = (i: EditorIntent) => {
    if (i.k === "select" && i.sel === null && popover?.current()) return;
    commit(i);
  };

  popover = createParamPopover({
    dispatch: (i) => rt.dispatch(i),
    kindInfo,
    getEnumOptions: opts.getEnumOptions,   // full {value, count?} objects — the picker renders counts
    usePicker: true,
    getStatus: (id) => rt.doc.status[id],
  });

  tip = createNodeTip(canvas, {
    getDoc: () => rt.doc.graph,
    kindInfo,
    getStatus: (id) => rt.doc.status[id],
    helpOpen: (id) => !!rt.doc.helpOpen[id],
    toWorld: (p) => rt.toWorld(v(p.x, p.y)),
  });

  // ── param-row click → popover ──────────────────────────────────────────────
  // Click detection lives OUTSIDE Gratify (like the tooltip): pointerdown/up
  // with little movement, hit-tested through the same nodeLayout the card drew
  // with. Gratify's own press has already selected the node by the time our
  // pointerup listener runs (listener order: mount() wired its handlers first).
  const layoutOf = (n: GraphNode): { info: NodeKindInfo; l: NodeLayout } | null => {
    const info0 = kindInfo(n.kind);
    if (!info0) return null;
    const info = subInfo(info0, n.sub);          // T16: graph.sub ports from node.sub
    const l = nodeLayout(info,
      { params: n.params, wiredInputs: wiredInputsOf(rt.doc.graph.edges, n.id, info), w: n.w, bh: n.bh },
      { helpOpen: !!rt.doc.helpOpen[n.id], zoom: rt.viewport.zoom },
      rt.doc.status[n.id]);
    return { info, l };
  };

  const paramHit = (cx: number, cy: number): { node: GraphNode; name: string; l: NodeLayout } | null => {
    const w = rt.toWorld(v(cx, cy));
    const nodes = rt.doc.graph.nodes;
    for (let i = nodes.length - 1; i >= 0; i--) {
      const n = nodes[i];
      const found = layoutOf(n);
      if (!found) continue;
      const { l } = found;
      if (w.x < n.x || w.x > n.x + l.w || w.y < n.y || w.y > n.y + l.h) continue;
      const card = rect(n.x, n.y, l.w, l.h);
      for (let pi = 0; pi < l.paramNames.length; pi++) {
        const r = paramRowRect(l, card, pi);
        if (w.x >= r.x && w.x <= r.right && w.y >= r.y && w.y <= r.bottom) {
          return { node: n, name: l.paramNames[pi], l };
        }
      }
      return null;                               // hit the node, but not a param row
    }
    return null;
  };

  /** WORLD rect of the row under edit — the island facet calls this every
   *  frame, so a dragged node carries its editor and pan/zoom track free.
   *  Fallbacks (param not a row: wired input, or chip-mode below the zoom
   *  threshold) anchor to the card header instead of closing. */
  const rowWorldRect = (nodeId: string, name: string) => {
    const n = rt.doc.graph.nodes.find((x) => x.id === nodeId);
    const found = n && layoutOf(n);
    if (!n || !found) return rect(0, 0, 0, 0);   // node vanished: syncSel closes next commit
    const idx = found.l.paramNames.indexOf(name);
    if (idx < 0 || found.l.chip) return rect(n.x, n.y, found.l.w, HEADER_H);
    return paramRowRect(found.l, rect(n.x, n.y, found.l.w, found.l.h), idx);
  };

  // ── resize cursor (hover + drag pin) ───────────────────────────────────────
  // Hover: pointermove hit-tests resizeHandleAt on the topmost node under the
  // pointer (same layoutOf plumbing as paramHit/the tooltip — no throttle
  // needed, it's one layout walk per event, same cost as the tooltip's) and
  // sets the canvas cursor per axis. A socket wins the ~overlap with the right
  // band exactly like the gesture order (wire before resize), so the cursor
  // never promises a resize the gesture would decline.
  // Drag pin: while a press that started on a handle is down, a class on
  // documentElement pins the cursor everywhere (the T19 splitter pattern —
  // index.html owns the pf-drag-* CSS), so it never flickers off the band.
  const RESIZE_CURSOR = { e: "ew-resize", s: "ns-resize", se: "nwse-resize" } as const;
  const RESIZE_PIN = { e: "pf-drag-ew", s: "pf-drag-ns", se: "pf-drag-nwse" } as const;
  const resizeHitAt = (cx: number, cy: number): "e" | "s" | "se" | null => {
    const w = rt.toWorld(v(cx, cy));
    const nodes = rt.doc.graph.nodes;
    for (let i = nodes.length - 1; i >= 0; i--) {
      const n = nodes[i];
      const found = layoutOf(n);
      if (!found) continue;
      const { info, l } = found;
      if (w.x < n.x || w.x > n.x + l.w || w.y < n.y || w.y > n.y + l.h) continue;
      if (socketAt(l, info, n.x, n.y, w)) return null;   // wire gesture claims first
      return resizeHandleAt(l, n.x, n.y, w);
    }
    return null;
  };
  let pinned: string | null = null;
  const pinCursor = (cls: string | null) => {
    if (pinned) document.documentElement.classList.remove(pinned);
    pinned = cls;
    if (cls) document.documentElement.classList.add(cls);
  };
  const onCursorMove = (ev: PointerEvent) => {
    if (pinned || ev.buttons) return;              // mid-drag: the pin (or nothing) rules
    const b = canvas.getBoundingClientRect();
    const hit = resizeHitAt(ev.clientX - b.left, ev.clientY - b.top);
    canvas.style.cursor = hit ? RESIZE_CURSOR[hit] : "";
  };
  canvas.addEventListener("pointermove", onCursorMove);
  const onPinUp = () => pinCursor(null);
  window.addEventListener("pointerup", onPinUp);
  window.addEventListener("pointercancel", onPinUp);

  let down: { x: number; y: number } | null = null;
  const onDown = (ev: PointerEvent) => {
    down = { x: ev.clientX, y: ev.clientY };
    const b = canvas.getBoundingClientRect();
    const hit = resizeHitAt(ev.clientX - b.left, ev.clientY - b.top);
    if (hit) pinCursor(RESIZE_PIN[hit]);
  };
  const onUp = (ev: PointerEvent) => {
    const wasDrag = !down || Math.hypot(ev.clientX - down.x, ev.clientY - down.y) > 4;
    down = null;
    // T14: a DRAG on the canvas (pan, marquee, node move) leaves the editor
    // open — the island keeps it glued to its row. Close-on-pan is dead.
    if (wasDrag) return;
    const b = canvas.getBoundingClientRect();
    const hit = paramHit(ev.clientX - b.left, ev.clientY - b.top);
    // In-row widgets own their clicks: toggle/chips never open an editor, and a
    // scrub row wants a double-click for precise entry (single click = select).
    const schema = hit && kindInfo(hit.node.kind)?.params[hit.name];
    const widget = schema ? widgetFor(schema, false) : "row";
    const opens = !!hit && widget !== "toggle" && widget !== "chips"
      && !(widget === "scrub" && ev.detail < 2);
    // A clean canvas click that does NOT open an editor is the click-away:
    // close (commits a dirty buffer). The select-null it may carry was dropped
    // by the dispatch guard, so dismissal still never deselects.
    if (!opens) { popover?.close(); return; }
    popover!.open(hit.node, hit.name);           // where it appears = the island
  };
  const onDocDown = (ev: PointerEvent) => {
    if (ev.target !== canvas && !popover?.contains(ev.target)) popover?.close();
  };
  canvas.addEventListener("pointerdown", onDown);
  canvas.addEventListener("pointerup", onUp);
  document.addEventListener("pointerdown", onDocDown);
  // Esc = cancel (revert-then-close) — the ONE Esc entry point for all three
  // editor organs. Capture: runs before gratify's window keydown; the
  // stopPropagation keeps the surface's Escape key map (palette close) from
  // double-consuming the same press. Replaces the old params.ts/index.ts
  // capture-registration-order treaty.
  const onKey = (ev: KeyboardEvent) => {
    if (ev.key === "Escape" && popover?.current()) { popover.cancel(); ev.stopPropagation(); }
  };
  window.addEventListener("keydown", onKey, true);

  // Ctrl-Z / Ctrl-Y / Ctrl-Shift-Z → undo/redo (dropped while typing in DOM;
  // quiet: a redo that re-adds a node must not pop the auto-open picker).
  const onHistKey = historyKeyHandler((i) => runQuiet(() => rt.dispatch(i)));
  window.addEventListener("keydown", onHistKey);

  // T14 focus-model seam (upstream Tab ring): Enter on a FOCUSED node card
  // opens its param editor (first unset pickable, else the first row). Inert
  // until NodeCard declares Focusable() — that one-liner is T15's cards.ts
  // fence (diff in the T14 report); row-level Tab needs per-row parts (T15's
  // registry restructure) and stays upstream-ready here.
  const onEnterKey = (ev: KeyboardEvent) => {
    if (ev.key !== "Enter" || isEditableDom(ev.target)) return;
    const id = rt.focusedKey;
    const n = id ? rt.doc.graph.nodes.find((x) => x.id === id) : undefined;
    if (!n) return;
    if (popover?.openFirstUnset(n)) return;
    const first = layoutOf(n)?.l.paramNames[0];
    if (first) popover?.open(n, first);
  };
  window.addEventListener("keydown", onEnterKey);

  // ── T13 clipboard: Ctrl-C copy, Ctrl-V paste, Ctrl-D duplicate ─────────────
  // In-memory only (a NodeClip value — no OS clipboard): selected nodes +
  // the edges among them, params deep-copied. Paste/duplicate is ONE batch
  // (fresh ids, +24/+24 from the originals, edges remapped) = one undo step;
  // the copies become the selection. Dropped while typing in DOM chrome.
  let clip: NodeClip | null = null;
  const pasteClip = (c: NodeClip) => {
    const { intent, ids } = pasteIntent(rt.doc.graph, c, PASTE_OFFSET, PASTE_OFFSET);
    rt.dispatch({ k: "graph", intent });
    rt.dispatch({ k: "select", sel: selFromIds(ids) });
  };
  const onClipKey = (ev: KeyboardEvent) => {
    const action = clipboardKeyAction(ev);
    if (!action || typingInDom()) return;
    if (action === "copy") {
      const c = copySelection(rt.doc.graph, selectedNodeIds(rt.doc));
      if (!c) return;                            // nothing selected: keys stay native
      clip = c;
    } else if (action === "paste") {
      if (!clip) return;
      pasteClip(clip);
    } else {
      const c = copySelection(rt.doc.graph, selectedNodeIds(rt.doc));
      if (!c) return;
      pasteClip(c);                              // duplicate = copy+paste, clip untouched
    }
    ev.preventDefault();                         // Ctrl-D would bookmark the page
  };
  window.addEventListener("keydown", onClipKey);

  // ── T16 subgraphs — REGION: one key block + breadcrumb state. T14 is
  // refactoring this file's dispatch/keyboard plumbing concurrently; keep this
  // block self-contained (its own listeners, removed in destroy) so the merge
  // is additive. Interaction adopted from the Kea editor: G = collapse the
  // multi-selection, U = expand a selected subgraph in place, double-click a
  // subgraph header = ENTER (scratch-doc; commit-on-exit), Esc / double-click
  // empty canvas = exit a level. History is per-level (`load` clears — see
  // subgraph.ts header for the argument).
  const crumbs: { parent: GraphDoc; groupId: string; title: string }[] = [];
  const breadcrumb = createBreadcrumb(host, topInset);
  const syncCrumb = () => breadcrumb.set(crumbs.length ? crumbs.map((c) => c.title) : null);
  const enterSub = (id: string) => {
    const e = enterDoc(rt.doc.graph, id);
    if (!e) return;
    popover?.close();
    crumbs.push({ parent: rt.doc.graph, groupId: id, title: e.title });
    rt.dispatch({ k: "graph", intent: { t: "load", doc: e.doc } });
    syncCrumb();
  };
  const exitSub = () => {
    const top = crumbs.pop();
    if (!top) return;
    popover?.close();
    rt.dispatch({ k: "graph", intent: { t: "load", doc: commitExit(top.parent, top.groupId, rt.doc.graph) } });
    rt.dispatch({ k: "select", sel: { kind: "node", id: top.groupId } });
    syncCrumb();
  };
  const onSubKey = (ev: KeyboardEvent) => {
    const action = subgraphKeyAction(ev);
    if (!action || typingInDom()) return;
    if (action === "collapse") {
      const c = collapseSelection(rt.doc.graph, selectedNodeIds(rt.doc), kindInfo);
      if (!c) return;
      rt.dispatch({ k: "graph", intent: c.intent });      // ONE batch = one Ctrl-Z
      rt.dispatch({ k: "select", sel: { kind: "node", id: c.id } });
    } else if (action === "expand") {
      const sel = selectedNode(rt.doc);
      const x = sel?.sub ? expandNode(rt.doc.graph, sel.id) : null;
      if (!x) return;
      rt.dispatch({ k: "graph", intent: x.intent });
      rt.dispatch({ k: "select", sel: selFromIds(x.ids) });
    } else {
      // Esc priority (Kea): popover > palette > subgraph level. The popover's
      // capture handler already consumed its Esc; palette gets this one.
      if (!crumbs.length || popover?.current() || rt.doc.palette) return;
      exitSub();
    }
    ev.preventDefault();
  };
  window.addEventListener("keydown", onSubKey);
  const onDblClick = (ev: MouseEvent) => {
    const b = canvas.getBoundingClientRect();
    const w = rt.toWorld(v(ev.clientX - b.left, ev.clientY - b.top));
    let overNode = false;
    for (let i = rt.doc.graph.nodes.length - 1; i >= 0; i--) {
      const n = rt.doc.graph.nodes[i];
      const found = layoutOf(n);
      if (!found) continue;
      if (w.x < n.x || w.x > n.x + found.l.w || w.y < n.y || w.y > n.y + found.l.h) continue;
      overNode = true;
      if (n.sub && inHeader(found.l, n.x, n.y, w)) enterSub(n.id);
      break;
    }
    if (!overNode && crumbs.length) exitSub();     // dbl-click empty canvas = exit
  };
  canvas.addEventListener("dblclick", onDblClick);

  // Chrome adds a node at the centre of what the user is currently looking at
  // (offset by half a card, so the drop lands centred rather than top-left).
  const viewCenter = () =>
    rt.toWorld(v((canvas.clientWidth || canvas.width) / 2, (canvas.clientHeight || canvas.height) / 2));

  // ── fit-to-frame ────────────────────────────────────────────────────────────
  // Animate pan/zoom so the graph's bounding box fills the VISIBLE canvas: the
  // menu bar and the open Add Node panel are opaque overlays, so both inset the
  // target rect. 350 ms ease-in-out tween; ambient() keeps the runtime awake.
  const fitToFrame = () => {
    const nodes = rt.doc.graph.nodes;
    if (!nodes.length) return;
    let x0 = Infinity, y0 = Infinity, x1 = -Infinity, y1 = -Infinity;
    for (const n of nodes) {
      const f = layoutOf(n);
      x0 = Math.min(x0, n.x); y0 = Math.min(y0, n.y);
      x1 = Math.max(x1, n.x + (f?.l.w ?? 188)); y1 = Math.max(y1, n.y + (f?.l.h ?? 90));
    }
    const pad = 28;
    const leftInset = host.querySelector(".pf-palette-panel.pf-open")?.clientWidth ?? 0;
    const vw = canvas.clientWidth || canvas.width, vh = canvas.clientHeight || canvas.height;
    const availW = Math.max(80, vw - leftInset - pad * 2);
    const availH = Math.max(80, vh - topInset - pad * 2);
    const bw = Math.max(1, x1 - x0), bh = Math.max(1, y1 - y0);
    const zoom = Math.min(1.25, Math.max(0.2, Math.min(availW / bw, availH / bh)));
    const px = leftInset + pad + (availW - bw * zoom) / 2 - x0 * zoom;
    const py = topInset + pad + (availH - bh * zoom) / 2 - y0 * zoom;
    const from = { x: rt.viewport.pan.x, y: rt.viewport.pan.y, z: rt.viewport.zoom };
    const t0 = performance.now(), DUR = 350;
    if (fitAnim !== null) cancelAnimationFrame(fitAnim);
    const step = (now: number) => {
      const t = Math.min(1, (now - t0) / DUR);
      const e = t < 0.5 ? 4 * t * t * t : 1 - Math.pow(-2 * t + 2, 3) / 2;  // easeInOutCubic
      rt.viewport.zoom = from.z + (zoom - from.z) * e;
      rt.viewport.pan = v(from.x + (px - from.x) * e, from.y + (py - from.y) * e);
      fitAnim = t < 1 ? requestAnimationFrame(step) : null;
    };
    fitAnim = requestAnimationFrame(step);
  };

  /** W13: auto-layout then fit (View ▸ Tidy graph / T). ONE batch of move
   *  intents = one Ctrl-Z step; the fit tween frames the tidied graph. */
  const tidyGraph = () => {
    const nodes = rt.doc.graph.nodes;
    if (!nodes.length) return;
    const placed = tidyLayout(rt.doc.graph, (n) => {
      const f = layoutOf(n);
      return { w: f?.l.w ?? 188, h: f?.l.h ?? 90 };
    });
    const cur = new Map(nodes.map((n) => [n.id, n]));
    const moves: Intent[] = [];
    for (const p of placed) {
      const n = cur.get(p.id);
      if (n && (Math.abs(n.x - p.x) > 0.5 || Math.abs(n.y - p.y) > 0.5))
        moves.push({ t: "move", node: p.id, x: p.x, y: p.y });
    }
    if (moves.length)
      runQuiet(() => rt.dispatch({ k: "graph", intent: { t: "batch", intents: moves } }));
    fitToFrame();
  };

  if (opts.chrome) {
    chrome = createChrome(host, kinds, opts.chrome, {
      addKind: (kind) => {
        // W13: collision-avoiding drop — never lands a card on top of another.
        const c = viewCenter();
        const info = kindInfo(kind);
        const size = info
          ? (() => { const l = nodeLayout(info, { params: {}, wiredInputs: new Set() }, { helpOpen: false, zoom: 1 }); return { w: l.w, h: l.h }; })()
          : { w: 188, h: 90 };
        const boxes = rt.doc.graph.nodes.map((n) => {
          const f = layoutOf(n);
          return { x: n.x, y: n.y, w: f?.l.w ?? 188, h: f?.l.h ?? 90 };
        });
        const at = placeFree({ x: c.x - size.w / 2, y: c.y - size.h / 2 }, size, boxes);
        rt.dispatch({ k: "addKind", kind, at: v(at.x, at.y) });
      },
      setHelpAll: (open) => rt.dispatch({ k: "helpAll", open }),
      undo: () => runQuiet(() => rt.dispatch({ k: "undo" })),
      redo: () => runQuiet(() => rt.dispatch({ k: "redo" })),
      fitGraph: fitToFrame,
      tidyGraph,
    });
    syncChrome();                                // both disabled on a fresh doc
  }

  // W13: right-click on a NODE opens its context menu (the self-documenting
  // index of node operations); right-click on empty canvas keeps the add
  // palette. Hit-test walks nodes top-most first, same as onDblClick.
  const ctxMenu = createContextMenu(host);
  const nodeAtScreen = (cx: number, cy: number): GraphNode | null => {
    const w = rt.toWorld(v(cx, cy));
    for (let i = rt.doc.graph.nodes.length - 1; i >= 0; i--) {
      const n = rt.doc.graph.nodes[i];
      const f = layoutOf(n);
      if (!f) continue;
      if (w.x >= n.x && w.x <= n.x + f.l.w && w.y >= n.y && w.y <= n.y + f.l.h) return n;
    }
    return null;
  };
  const onContextMenu = (ev: MouseEvent) => {
    ev.preventDefault();
    const b = canvas.getBoundingClientRect();
    const n = nodeAtScreen(ev.clientX - b.left, ev.clientY - b.top);
    if (!n) {
      rt.dispatch({ k: "palette", at: rt.toWorld(v(ev.clientX - b.left, ev.clientY - b.top)) });
      return;
    }
    const info = kindInfo(n.kind);
    const entries: MenuEntry[] = [
      { label: "Show in 3D", action: () => runQuiet(() => rt.dispatch({ k: "select", sel: { kind: "node", id: n.id } })) },
      ...(info?.effect === "write"
        ? [{ label: "Run", action: () => rt.dispatch({ k: "run", node: n.id }) }]
        : []),
      { label: "Toggle help", kbd: "?", action: () => rt.dispatch({ k: "toggleHelp", node: n.id }) },
      {
        label: "Duplicate", kbd: "Ctrl+D",
        action: () => { const c = copySelection(rt.doc.graph, [n.id]); if (c) pasteClip(c); },
      },
      "---",
      { label: "Delete", kbd: "Del", danger: true, action: () => runQuiet(() => rt.dispatch({ k: "deleteNode", node: n.id })) },
    ];
    ctxMenu.openAt(ev.clientX, ev.clientY, entries);
  };
  canvas.addEventListener("contextmenu", onContextMenu);

  return {
    dispatch(intent: Intent, coalesceKey?: string) {
      popover?.close();                          // params may have changed under the DOM
      runQuiet(() => rt.dispatch(coalesceKey !== undefined
        ? { k: "graphBatchKey", intent, key: coalesceKey }
        : { k: "graph", intent }));              // quiet: external adds never auto-open
    },
    getDoc: () => rt.doc.graph,
    fitToFrame,
    tidy: tidyGraph,
    setResults(r: EvalResult) {
      const status: Record<string, NodeStatus> = {};
      r.status.forEach((s, id) => { status[id] = s; });
      rt.dispatch({ k: "status", status });
    },
    setStatus: (msg: string) => chrome?.setStatus(msg),
    destroy() {
      if (fitAnim !== null) { cancelAnimationFrame(fitAnim); fitAnim = null; }
      window.removeEventListener("keydown", onSubKey);   // T16
      canvas.removeEventListener("dblclick", onDblClick);
      breadcrumb.destroy();
      ctxMenu.destroy();
      canvas.removeEventListener("contextmenu", onContextMenu);
      canvas.removeEventListener("pointerdown", onDown);
      canvas.removeEventListener("pointerup", onUp);
      canvas.removeEventListener("pointermove", onCursorMove);
      window.removeEventListener("pointerup", onPinUp);
      window.removeEventListener("pointercancel", onPinUp);
      pinCursor(null);
      document.removeEventListener("pointerdown", onDocDown);
      window.removeEventListener("keydown", onKey, true);
      window.removeEventListener("keydown", onHistKey);
      window.removeEventListener("keydown", onClipKey);
      window.removeEventListener("keydown", onEnterKey);
      islandWrap.remove();
      rt.stop();
      popover?.destroy();
      popover = null;
      tip?.destroy();
      tip = null;
      chrome?.destroy();
      chrome = null;
    },
  };
}
