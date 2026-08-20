// W9-C specs: setWireHighlight — the viewport-pick → editor reverse link
// (design §4.4). Module-state API in wires.ts (the onWireHover pattern);
// keys follow CONTRACTS.md `${from.node}.${from.slot}->${to.node}.${to.slot}`
// and normalize to geom edgeKey form. Render-state asserted through
// wireHighlighted (what WirePart's render reads), plus a headless render pass
// over a real doc with a highlighted wire.
import { afterEach, describe, expect, it, vi } from "vitest";
import type { GraphDoc } from "../../contracts";
import { normalizeWireKey, setWireHighlight, wireHighlighted } from "../wires";
import { edgeKey } from "../geom";
import { createDrive } from "./drive";

afterEach(() => setWireHighlight(null));         // module state must not leak

// ── pure: key normalization ──────────────────────────────────────────────────

describe("normalizeWireKey", () => {
  it("contract key → internal ekey (geom.edgeKey form)", () => {
    expect(normalizeWireKey("n1.out->n2.in")).toBe("n1|out>n2|in");
    expect(normalizeWireKey("cmap1.out->color1.colormap")).toBe("cmap1|out>color1|colormap");
  });

  it("matches what geom.edgeKey produces for the same edge", () => {
    const e = { from: { node: "n1", slot: "out" }, to: { node: "n2", slot: "in" } };
    expect(normalizeWireKey("n1.out->n2.in")).toBe(edgeKey(e));
  });

  it("keys without the arrow pass through unchanged (already-internal ok)", () => {
    expect(normalizeWireKey("n1|out>n2|in")).toBe("n1|out>n2|in");
    expect(normalizeWireKey("garbage")).toBe("garbage");
  });
});

// ── module state: set / clear / ignore ───────────────────────────────────────

describe("setWireHighlight", () => {
  it("marks matching wires (array or ReadonlySet input)", () => {
    setWireHighlight(["n1.out->n2.in"]);
    expect(wireHighlighted("n1|out>n2|in")).toBe(true);
    expect(wireHighlighted("n3|out>n4|in")).toBe(false);

    setWireHighlight(new Set(["n3.out->n4.in", "n1.out->n2.in"]));
    expect(wireHighlighted("n1|out>n2|in")).toBe(true);
    expect(wireHighlighted("n3|out>n4|in")).toBe(true);
  });

  it("null clears everything", () => {
    setWireHighlight(["n1.out->n2.in"]);
    setWireHighlight(null);
    expect(wireHighlighted("n1|out>n2|in")).toBe(false);
  });

  it("unknown keys are ignored — no throw, real wires untouched", () => {
    expect(() => setWireHighlight(["nope.q->zzz.w", "not-a-key"])).not.toThrow();
    expect(wireHighlighted("n1|out>n2|in")).toBe(false);
  });

  it("repaint: pokes the runtime waker on change, no-ops when unchanged", () => {
    const g = globalThis as { gratifyResume?: () => void };
    const wake = vi.fn();
    g.gratifyResume = wake;
    try {
      setWireHighlight(["n1.out->n2.in"]);
      expect(wake).toHaveBeenCalledTimes(1);
      setWireHighlight(["n1.out->n2.in"]);           // same set → no wake
      expect(wake).toHaveBeenCalledTimes(1);
      setWireHighlight(null);
      expect(wake).toHaveBeenCalledTimes(2);
      setWireHighlight(null);                        // already clear → no wake
      expect(wake).toHaveBeenCalledTimes(2);
    } finally {
      delete g.gratifyResume;
    }
  });
});

// ── headless: a real doc renders its highlighted wire ────────────────────────

const DOC: GraphDoc = {
  name: "wirehl-fixture",
  nodes: [
    { id: "n1", kind: "load.model", params: { model: "duplex" }, x: 60, y: 80 },
    { id: "n2", kind: "select.byType", params: { type: "IfcWall" }, x: 560, y: 80 },
    { id: "n3", kind: "data.csv", params: { url: "" }, x: 60, y: 420 },
    { id: "n4", kind: "table.filter", params: {}, x: 560, y: 420 },
  ],
  edges: [
    { from: { node: "n1", slot: "out" }, to: { node: "n2", slot: "in" } },
    { from: { node: "n3", slot: "out" }, to: { node: "n4", slot: "in" } },
  ],
  display: null,
};

describe("highlight over a live editor", () => {
  it("the highlighted wire's ekey reads true through the render frames", () => {
    const d = createDrive();
    d.load(DOC);
    d.settle();
    const scene = edgeKey(DOC.edges[0]);
    const table = edgeKey(DOC.edges[1]);

    setWireHighlight(["n1.out->n2.in"]);
    d.step(5);                                     // WirePart renders with hl = 1
    expect(wireHighlighted(scene)).toBe(true);
    expect(wireHighlighted(table)).toBe(false);

    setWireHighlight(null);
    d.step(5);
    expect(wireHighlighted(scene)).toBe(false);
  });

  it("deleting the graph under a live highlight is harmless (keys just miss)", () => {
    const d = createDrive();
    d.load(DOC);
    d.settle();
    setWireHighlight(["n1.out->n2.in"]);
    d.dispatch({ k: "graph", intent: { t: "disconnect", to: { node: "n2", slot: "in" } } });
    d.settle();                                    // renders with a stale key set
    expect(wireHighlighted(edgeKey(DOC.edges[0]))).toBe(true);   // set unchanged…
    expect(d.graph().edges).toHaveLength(1);                     // …wire gone; nothing to draw
  });
});
