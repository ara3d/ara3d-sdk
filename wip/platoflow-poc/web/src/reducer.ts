// Supervisor-owned. The ONLY way a GraphDoc changes — editor gestures, MCP intents,
// and demo loading all pass through here. Returns a new doc (shallow-copied where touched).
import type { GraphDoc, Intent, SlotRef } from "./contracts";

const sameRef = (a: SlotRef, b: SlotRef) => a.node === b.node && a.slot === b.slot;

export function emptyDoc(name = "untitled"): GraphDoc {
  return { name, nodes: [], edges: [], display: null };
}

export function applyIntent(doc: GraphDoc, intent: Intent): GraphDoc {
  switch (intent.t) {
    case "load":
      return structuredClone(intent.doc);
    case "addNode": {
      if (doc.nodes.some(n => n.id === intent.id)) return doc;
      const node = {
        id: intent.id, kind: intent.kind, params: { ...(intent.params ?? {}) },
        x: intent.x, y: intent.y,
        ...(intent.sub ? { sub: structuredClone(intent.sub) } : {}),
      };
      return { ...doc, nodes: [...doc.nodes, node] };
    }
    case "removeNode":
      if (!doc.nodes.some(n => n.id === intent.id)) return doc;
      return {
        ...doc,
        nodes: doc.nodes.filter(n => n.id !== intent.id),
        edges: doc.edges.filter(e => e.from.node !== intent.id && e.to.node !== intent.id),
        display: doc.display === intent.id ? null : doc.display,
      };
    case "connect": {
      if (!doc.nodes.some(n => n.id === intent.from.node)) return doc;
      if (!doc.nodes.some(n => n.id === intent.to.node)) return doc;
      const edges = doc.edges.filter(e => !sameRef(e.to, intent.to)); // one wire per input
      return { ...doc, edges: [...edges, { from: intent.from, to: intent.to }] };
    }
    case "disconnect":
      if (!doc.edges.some(e => sameRef(e.to, intent.to))) return doc;
      return { ...doc, edges: doc.edges.filter(e => !sameRef(e.to, intent.to)) };
    case "setParam":
      return {
        ...doc,
        nodes: doc.nodes.map(n =>
          n.id === intent.node ? { ...n, params: { ...n.params, [intent.name]: intent.value } } : n),
      };
    case "move":
      return {
        ...doc,
        nodes: doc.nodes.map(n =>
          n.id === intent.node ? { ...n, x: intent.x, y: intent.y } : n),
      };
    case "resize":
      return {
        ...doc,
        nodes: doc.nodes.map(n =>
          n.id === intent.node
            ? {
                ...n,
                ...(intent.w !== undefined ? { w: intent.w } : {}),
                ...(intent.bh !== undefined ? { bh: intent.bh } : {}),
              }
            : n),
      };
    case "setDisplay":
      return { ...doc, display: intent.node };
    case "batch":
      return intent.intents.reduce(applyIntent, doc);
    default:
      return doc;
  }
}

let counter = 0;
export function freshNodeId(doc: GraphDoc): string {
  let id: string;
  do { id = `n${++counter}`; } while (doc.nodes.some(n => n.id === id));
  return id;
}
