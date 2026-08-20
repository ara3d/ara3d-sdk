// T4 undo UI: keyboard decode/guard, gesture-level history granularity,
// selection hygiene after undo, and caller-keyed coalescing (graphBatchKey).
// Headless drive for everything interactive (NOTES wave-5: DOM organs don't
// mount headless — the Edit-menu DOM is specced separately in chrome jsdom).
import { describe, expect, it } from "vitest";
import type { Intent } from "../../contracts";
import { initialDoc, makeUpdate, type EditorDoc, type EditorIntent } from "../doc";
import { historyKeyHandler, undoKeyIntent } from "../index";
import { createDrive } from "./drive";

const g = (intent: Intent): EditorIntent => ({ k: "graph", intent });

// ── keyboard decode (pure) ───────────────────────────────────────────────────

const ev = (key: string, mods: Partial<{ ctrlKey: boolean; metaKey: boolean; shiftKey: boolean }> = {}) =>
  ({ key, ctrlKey: false, metaKey: false, shiftKey: false, ...mods });

describe("undo keyboard decode", () => {
  it("Ctrl-Z → undo, Ctrl-Shift-Z → redo, Ctrl-Y → redo", () => {
    expect(undoKeyIntent(ev("z", { ctrlKey: true }))).toEqual({ k: "undo" });
    expect(undoKeyIntent(ev("Z", { ctrlKey: true, shiftKey: true }))).toEqual({ k: "redo" });
    expect(undoKeyIntent(ev("y", { ctrlKey: true }))).toEqual({ k: "redo" });
  });

  it("meta (⌘) counts as the modifier", () => {
    expect(undoKeyIntent(ev("z", { metaKey: true }))).toEqual({ k: "undo" });
  });

  it("plain z/y and unrelated chords decode to nothing", () => {
    expect(undoKeyIntent(ev("z"))).toBe(null);
    expect(undoKeyIntent(ev("y"))).toBe(null);
    expect(undoKeyIntent(ev("x", { ctrlKey: true }))).toBe(null);
    expect(undoKeyIntent(ev("y", { ctrlKey: true, shiftKey: true }))).toBe(null);
  });
});

describe("history key handler", () => {
  const fakeKey = (key: string, mods: Parameters<typeof ev>[1] = {}) => {
    let prevented = false;
    return { ...ev(key, mods), preventDefault: () => { prevented = true; },
             wasPrevented: () => prevented };
  };

  it("Ctrl-Z over the canvas undoes through the real doc (drive round-trip)", () => {
    const d = createDrive();
    const handle = historyKeyHandler((i) => d.dispatch(i), () => false);
    d.dispatch({ k: "addKind", kind: "load.model", at: { x: 200, y: 200 } });
    expect(d.graph().nodes.length).toBe(1);
    const e = fakeKey("z", { ctrlKey: true });
    handle(e);
    expect(e.wasPrevented()).toBe(true);
    expect(d.graph().nodes.length).toBe(0);
    handle(fakeKey("y", { ctrlKey: true }));
    expect(d.graph().nodes.length).toBe(1);
  });

  it("while typing in a DOM field the chord is ignored AND left native (no preventDefault)", () => {
    const d = createDrive();
    d.dispatch({ k: "addKind", kind: "load.model", at: { x: 200, y: 200 } });
    const handle = historyKeyHandler((i) => d.dispatch(i), () => true);
    const e = fakeKey("z", { ctrlKey: true });
    handle(e);
    expect(e.wasPrevented()).toBe(false);
    expect(d.graph().nodes.length).toBe(1);
  });
});

// ── gesture granularity + selection hygiene (headless drive) ─────────────────

describe("undo over real gestures", () => {
  it("drop a node then immediately drag it = TWO undo steps (add, move)", () => {
    const d = createDrive();
    d.dispatch({ k: "addKind", kind: "load.model", at: { x: 200, y: 200 } });
    d.settle();
    const id = d.graph().nodes[0].id;
    d.moveNode(id, 80, 40);
    d.settle();
    expect(d.doc().past.length).toBe(2);         // add + one coalesced drag
    d.dispatch({ k: "undo" });
    const n = d.graph().nodes[0];
    expect(n.x).toBe(200);
    expect(n.y).toBe(200);                       // move undone, node survives
    d.dispatch({ k: "undo" });
    expect(d.graph().nodes.length).toBe(0);      // add undone
  });

  it("undoing an add clears the (now-dangling) node selection", () => {
    const d = createDrive();
    d.dispatch({ k: "addKind", kind: "load.model", at: { x: 200, y: 200 } });
    expect(d.doc().sel).toEqual({ kind: "node", id: d.graph().nodes[0].id });
    d.dispatch({ k: "undo" });
    expect(d.doc().sel).toBe(null);
  });

  it("undo that keeps the selected node alive keeps the selection", () => {
    const d = createDrive();
    d.dispatch({ k: "addKind", kind: "load.model", at: { x: 200, y: 200 } });
    const id = d.graph().nodes[0].id;
    d.dispatch(g({ t: "move", node: id, x: 300, y: 300 }));
    d.dispatch({ k: "undo" });                   // undoes the move only
    expect(d.doc().sel).toEqual({ kind: "node", id });
  });

  it("undoing a connect clears a dangling edge selection", () => {
    const update = makeUpdate({ kindInfo: () => undefined });
    const run = (doc: EditorDoc, is: EditorIntent[]) => is.reduce(update, doc);
    const doc = run(initialDoc(), [
      g({ t: "addNode", id: "a", kind: "k", x: 0, y: 0 }),
      g({ t: "addNode", id: "b", kind: "k", x: 0, y: 0 }),
      g({ t: "connect", from: { node: "a", slot: "out" }, to: { node: "b", slot: "in" } }),
      { k: "select", sel: { kind: "edge", key: "a|out>b|in" } },
    ]);
    expect(doc.sel?.kind).toBe("edge");
    const u = update(doc, { k: "undo" });
    expect(u.graph.edges.length).toBe(0);
    expect(u.sel).toBe(null);
  });
});

// ── caller-keyed coalescing (graphBatchKey) ──────────────────────────────────

describe("graphBatchKey coalescing", () => {
  const update = makeUpdate({ kindInfo: () => undefined });
  const run = (doc: EditorDoc, is: EditorIntent[]) => is.reduce(update, doc);
  const add = (id: string, key: string): EditorIntent =>
    ({ k: "graphBatchKey", intent: { t: "addNode", id, kind: "k", x: 0, y: 0 }, key });

  it("consecutive same-key dispatches form ONE undo step (MCP burst)", () => {
    const doc = run(initialDoc(), [add("a", "mcp:1"), add("b", "mcp:1"), add("c", "mcp:1")]);
    expect(doc.graph.nodes.length).toBe(3);
    expect(doc.past.length).toBe(1);
    const u = update(doc, { k: "undo" });
    expect(u.graph.nodes.length).toBe(0);        // whole burst reverts at once
  });

  it("a key change starts a new step; a plain graph intent breaks the run", () => {
    const doc = run(initialDoc(), [
      add("a", "mcp:1"), add("b", "mcp:2"),      // key change → 2 steps
      g({ t: "move", node: "a", x: 5, y: 5 }),   // plain intent → its own step
      add("c", "mcp:2"),                         // same key, but run was broken
    ]);
    expect(doc.past.length).toBe(4);
  });

  it("no-op intents under a key leave history untouched", () => {
    const doc = run(initialDoc(), [add("a", "mcp:1"),
      { k: "graphBatchKey", intent: { t: "removeNode", id: "missing" }, key: "mcp:1" }]);
    expect(doc.past.length).toBe(1);
  });

  it("a keyed load still clears history", () => {
    const doc = run(initialDoc(), [add("a", "mcp:1"),
      { k: "graphBatchKey", key: "mcp:1",
        intent: { t: "load", doc: { name: "d", nodes: [], edges: [], display: null } } }]);
    expect(doc.past.length).toBe(0);
    expect(doc.future.length).toBe(0);
    expect(doc.histKey).toBe(null);
  });
});
