// W13-A specs: pure layout module — no jsdom, no canvas, fixtures inline.
import { describe, expect, it } from "vitest";
import type { GraphDoc, GraphEdge, GraphNode } from "../../contracts";
import { COL_GAP, COMPONENT_GAP, DROP_MARGIN, placeFree, tidyLayout, type Placed, type Size } from "../layout";

// ── fixtures ─────────────────────────────────────────────────────────────────

const node = (id: string, x = 0, y = 0): GraphNode => ({ id, kind: "k", params: {}, x, y });
const edge = (from: string, to: string): GraphEdge =>
  ({ from: { node: from, slot: "out" }, to: { node: to, slot: "in" } });
const doc = (nodes: GraphNode[], edges: GraphEdge[]): GraphDoc =>
  ({ name: "t", nodes, edges, display: null });

const CARD: Size = { w: 188, h: 100 };
const uniform = (_n: GraphNode): Size => CARD;

const at = (placed: Placed[], id: string): Placed => placed.find((p) => p.id === id)!;

/** The whole point of tidyLayout: no two cards may overlap. Strict overlap —
 *  shared edges are fine. */
const assertNoOverlap = (placed: Placed[], graph: GraphDoc, sizeOf: (n: GraphNode) => Size) => {
  const rects = graph.nodes.map((n) => {
    const p = at(placed, n.id), s = sizeOf(n);
    return { id: n.id, x: p.x, y: p.y, w: s.w, h: s.h };
  });
  for (let i = 0; i < rects.length; i++)
    for (let j = i + 1; j < rects.length; j++) {
      const a = rects[i], b = rects[j];
      const overlap = a.x < b.x + b.w && a.x + a.w > b.x && a.y < b.y + b.h && a.y + a.h > b.y;
      expect(overlap, `${a.id} overlaps ${b.id}`).toBe(false);
    }
};

// ── tidyLayout ───────────────────────────────────────────────────────────────

describe("tidyLayout", () => {
  it("linear chain: strictly increasing x, wires left-to-right, no overlap", () => {
    const g = doc([node("a"), node("b"), node("c")], [edge("a", "b"), edge("b", "c")]);
    const p = tidyLayout(g, uniform);
    expect(at(p, "a").x).toBeLessThan(at(p, "b").x);
    expect(at(p, "b").x).toBeLessThan(at(p, "c").x);
    // Wire tails must clear the previous card entirely, not just its x origin.
    expect(at(p, "a").x + CARD.w).toBeLessThan(at(p, "b").x);
    expect(at(p, "b").x + CARD.w).toBeLessThan(at(p, "c").x);
    assertNoOverlap(p, g, uniform);
  });

  it("diamond: b,c share a column, d after both, short columns centred", () => {
    const g = doc(
      [node("a"), node("b"), node("c"), node("d")],
      [edge("a", "b"), edge("a", "c"), edge("b", "d"), edge("c", "d")],
    );
    const p = tidyLayout(g, uniform);
    expect(at(p, "b").x).toBe(at(p, "c").x);
    expect(at(p, "d").x).toBeGreaterThan(at(p, "b").x + CARD.w);
    // Same upstream + same current y → id tie-break: b above c, deterministic.
    expect(at(p, "b").y).toBeLessThan(at(p, "c").y);
    // The single-card columns (a, d) sit on the tall column's midline, not at
    // its top: middle column height = 100 + 36 + 100 = 236 → a.y = 68.
    const midColH = 2 * CARD.h + 36;
    expect(at(p, "a").y).toBeCloseTo((midColH - CARD.h) / 2);
    expect(at(p, "d").y).toBeCloseTo((midColH - CARD.h) / 2);
    assertNoOverlap(p, g, uniform);
  });

  it("disconnected components stack vertically with the component gap", () => {
    const g = doc([node("a"), node("b"), node("c")], [edge("a", "b")]);
    const p = tidyLayout(g, uniform);
    // Component 1 (a→b) occupies one row of cards; c starts a full gap below.
    const comp1Bottom = Math.max(at(p, "a").y, at(p, "b").y) + CARD.h;
    expect(at(p, "c").y - comp1Bottom).toBeGreaterThanOrEqual(COMPONENT_GAP);
    // Independent layout: c's column restarts at x = 0.
    expect(at(p, "c").x).toBe(0);
    assertNoOverlap(p, g, uniform);
  });

  it("cycle a→b→a terminates and places both left-to-right", () => {
    const g = doc([node("a"), node("b")], [edge("a", "b"), edge("b", "a")]);
    const p = tidyLayout(g, uniform);
    expect(p).toHaveLength(2);
    // The back edge b→a is dropped, so a stays the source column.
    expect(at(p, "a").x).toBeLessThan(at(p, "b").x);
    assertNoOverlap(p, g, uniform);
  });

  it("column gap respects the widest card of the previous column", () => {
    const sizes: Record<string, Size> = {
      a: { w: 320, h: 100 }, b: { w: 120, h: 60 }, c: { w: 188, h: 100 },
    };
    const sizeOf = (n: GraphNode): Size => sizes[n.id];
    const g = doc([node("a"), node("b"), node("c")], [edge("a", "c"), edge("b", "c")]);
    const p = tidyLayout(g, sizeOf);
    // a and b share the source column; the next column clears the WIDER of
    // the two (320) plus the gap — not the narrow one.
    expect(at(p, "a").x).toBe(at(p, "b").x);
    expect(at(p, "c").x).toBe(320 + COL_GAP);
    assertNoOverlap(p, g, sizeOf);
  });

  it("is deterministic: same input, deep-equal output", () => {
    const g = doc(
      [node("a", 5, 40), node("b", 9, 12), node("c"), node("d"), node("e")],
      [edge("a", "b"), edge("a", "c"), edge("c", "d"), edge("b", "d")],
    );
    expect(tidyLayout(g, uniform)).toEqual(tidyLayout(g, uniform));
  });
});

// ── placeFree ────────────────────────────────────────────────────────────────

describe("placeFree", () => {
  const size: Size = { w: 100, h: 60 };
  const clearOf = (p: { x: number; y: number }, boxes: { x: number; y: number; w: number; h: number }[]) =>
    boxes.every((b) =>
      p.x - DROP_MARGIN >= b.x + b.w || p.x + size.w + DROP_MARGIN <= b.x ||
      p.y - DROP_MARGIN >= b.y + b.h || p.y + size.h + DROP_MARGIN <= b.y);

  it("returns desired unchanged when the spot is free", () => {
    expect(placeFree({ x: 40, y: 40 }, size, [])).toEqual({ x: 40, y: 40 });
    // Free even with a box nearby, as long as the margin clears it.
    const far = [{ x: 400, y: 400, w: 50, h: 50 }];
    expect(placeFree({ x: 40, y: 40 }, size, far)).toEqual({ x: 40, y: 40 });
  });

  it("moves off a colliding box to a genuinely free spot", () => {
    const boxes = [{ x: 0, y: 0, w: 120, h: 80 }];
    const p = placeFree({ x: 10, y: 10 }, size, boxes);
    expect(p).not.toEqual({ x: 10, y: 10 });
    expect(clearOf(p, boxes)).toBe(true);
  });

  it("terminates on a crowded field and still finds a clear spot", () => {
    // A dense 10×10 wall of cards around the drop point — the cascade fails
    // everywhere inside it, forcing the ring sweep to walk out past the field.
    const boxes: { x: number; y: number; w: number; h: number }[] = [];
    for (let i = 0; i < 10; i++)
      for (let j = 0; j < 10; j++)
        boxes.push({ x: i * 110 - 550, y: j * 70 - 350, w: 110, h: 70 });
    const p = placeFree({ x: 0, y: 0 }, size, boxes);
    expect(clearOf(p, boxes)).toBe(true);
  });

  it("is deterministic", () => {
    const boxes = [{ x: 0, y: 0, w: 200, h: 200 }, { x: 220, y: 0, w: 200, h: 200 }];
    expect(placeFree({ x: 50, y: 50 }, size, boxes))
      .toEqual(placeFree({ x: 50, y: 50 }, size, boxes));
  });
});
