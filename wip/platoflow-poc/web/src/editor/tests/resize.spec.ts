// T15 per-instance node resize. Contract under test:
//  - GraphNode.w wins over NodeKindInfo.width, which wins over NODE_W;
//  - the resize hit zone (right-edge band + bottom-right grip) is shared pure
//    geometry (resizeHandleAt) — render and gesture cannot drift;
//  - a right-edge drag through the REAL gesture pipeline sets node.w
//    (clamped to [NODE_W, NODE_W_MAX]), moves the node ZERO px, and is ONE
//    undo entry (doc.ts `resize:<id>` coalescing key);
//  - socket anchors, body area and hit-tests all track the new width because
//    they derive from nodeLayout;
//  - double-click on the grip resets to the kind default (`info.width ??
//    NODE_W`) — as an EXPLICIT w, since the resize intent cannot say "unset";
//  - chip mode (T10 zoom) has no resize affordance.
import { describe, expect, it } from "vitest";
import { rect, v, type Vec } from "gratify";
import type { GraphDoc } from "../../contracts";
import { kindInfo } from "../../kinds";
import {
  BODY_H_MAX, BODY_H_MIN, bodyRect, clampBodyH, clampNodeW, NODE_W, NODE_W_MAX, nodeLayout,
  paramRowAt, paramRowRect, RESIZE_BAND, resizeGripRect, resizeHandleAt, socketAt, socketPos,
  type LayoutNode, type LayoutUi,
} from "../geom";
import { copySelection, pasteIntent } from "../doc";
import {
  GRID_HEADER_H, GRID_ROW_H, gridLayout, resizeGesture, tablePreviewOf,
  type WidgetHostProps,
} from "../widgets";
import { createDrive } from "./drive";

const bare = (w?: number, bh?: number): LayoutNode =>
  ({ params: {}, wiredInputs: new Set<string>(), w, bh });
const ui = (zoom = 1): LayoutUi => ({ helpOpen: false, zoom });

const DOC: GraphDoc = {
  name: "resize-fixture",
  nodes: [
    { id: "type1", kind: "select.byType", params: { type: "IfcWall" }, x: 100, y: 80 },
    { id: "sql1", kind: "table.sql", params: { sql: "SELECT 1" }, x: 500, y: 80 },
    // the user's named vertical-resize case: a body-carrying aggregate node
    { id: "agg1", kind: "table.aggregate", params: {}, x: 100, y: 420 },
  ],
  edges: [],
  display: null,
};

const fresh = () => {
  const d = createDrive();
  d.load(DOC);
  d.settle();
  return d;
};

const nodeOf = (d: ReturnType<typeof fresh>, id: string) =>
  d.graph().nodes.find((n) => n.id === id)!;

/** Center of the bottom-right grip, from the LIVE instance rect — the width
 *  the card itself believes (props.w once makeView passes it; the kind
 *  default until then), so press points and the gesture's own hit-test always
 *  agree, before AND after the surface.ts caller wiring lands. */
const gripCenter = (d: ReturnType<typeof fresh>, id: string): Vec => {
  d.settle();
  const r = d.rectOfKey(id)!;
  return v(r.right - 5, r.bottom - 5);
};

/** Center of the bottom ("s") band — clear of the corner grip. */
const bandSCenter = (d: ReturnType<typeof fresh>, id: string): Vec => {
  d.settle();
  const r = d.rectOfKey(id)!;
  return v(r.x + r.w / 2, r.bottom - RESIZE_BAND / 2);
};

describe("resize layout (pure)", () => {
  it("node.w wins over info.width wins over NODE_W", () => {
    const plain = kindInfo("select.byType")!;         // no kind width
    const wide = kindInfo("table.sql")!;              // kind width 240
    expect(nodeLayout(plain, bare(), ui()).w).toBe(NODE_W);
    expect(nodeLayout(plain, bare(300), ui()).w).toBe(300);
    expect(wide.width).toBe(240);
    expect(nodeLayout(wide, bare(), ui()).w).toBe(240);
    expect(nodeLayout(wide, bare(320), ui()).w).toBe(320);
  });

  it("chip mode preserves the per-instance width too", () => {
    const info = kindInfo("select.byType")!;
    const l = nodeLayout(info, bare(300), ui(0.4));
    expect(l.chip).toBe(true);
    expect(l.w).toBe(300);
  });

  it("clampNodeW clamps to [NODE_W, NODE_W_MAX] and rounds", () => {
    expect(clampNodeW(40)).toBe(NODE_W);
    expect(clampNodeW(1000)).toBe(NODE_W_MAX);
    expect(clampNodeW(250.4)).toBe(250);
    expect(clampNodeW(NODE_W)).toBe(NODE_W);
    expect(clampNodeW(NODE_W_MAX)).toBe(NODE_W_MAX);
  });

  it("sockets, body and rows track a resized width", () => {
    const info = kindInfo("table.sql")!;
    const l0 = nodeLayout(info, bare(), ui());
    const l1 = nodeLayout(info, bare(400), ui());
    const nx = 50, ny = 60;
    const c0 = rect(nx, ny, l0.w, l0.h), c1 = rect(nx, ny, l1.w, l1.h);
    // out-socket anchor sits at the (moved) right edge and stays hit-testable
    expect(socketPos(l1, c1, "out", 0).x).toBe(nx + 400);
    expect(socketAt(l1, info, nx, ny, socketPos(l1, c1, "out", 0)))
      .toMatchObject({ dir: "out", index: 0 });
    // body area widens by exactly the width delta
    expect(bodyRect(l1, c1).w - bodyRect(l0, c0).w).toBe(400 - l0.w);
    // param rows span the new width; their centers still hit their own names
    l1.paramRows.forEach((row, i) => {
      if (row.h <= 0) return;
      const rr = paramRowRect(l1, c1, i);
      expect(rr.right).toBe(nx + 400 - 5);
      expect(paramRowAt(l1, nx, ny, rr.center)).toBe(row.name);
    });
  });

  it("resizeHandleAt: grip beats band, band is the right edge, interior misses", () => {
    const info = kindInfo("select.byType")!;
    const l = nodeLayout(info, bare(), ui());
    const nx = 10, ny = 20;
    expect(resizeHandleAt(l, nx, ny, resizeGripRect(l, nx, ny).center)).toBe("se");
    expect(resizeHandleAt(l, nx, ny, v(nx + l.w - RESIZE_BAND / 2, ny + 4))).toBe("e");
    expect(resizeHandleAt(l, nx, ny, v(nx + l.w / 2, ny + l.h / 2))).toBeNull();
    expect(resizeHandleAt(l, nx, ny, v(nx - 2, ny + 4))).toBeNull(); // left edge is not a handle
  });

  it("bottom band: 's' on body-carrying kinds, ABSENT on body-less kinds", () => {
    const nx = 10, ny = 20;
    const agg = kindInfo("table.aggregate")!;         // bodyHeight 88
    const la = nodeLayout(agg, bare(), ui());
    const bottomMid = v(nx + la.w / 2, ny + la.h - RESIZE_BAND / 2);
    expect(resizeHandleAt(la, nx, ny, bottomMid)).toBe("s");
    // corner still wins over the bottom band
    expect(resizeHandleAt(la, nx, ny, resizeGripRect(la, nx, ny).center)).toBe("se");
    // body-less kind: no vertical resize anywhere on the bottom edge
    const plain = kindInfo("select.byType")!;
    const lp = nodeLayout(plain, bare(), ui());
    expect(lp.bodyH).toBe(0);
    expect(resizeHandleAt(lp, nx, ny, v(nx + lp.w / 2, ny + lp.h - RESIZE_BAND / 2))).toBeNull();
  });

  it("node.bh wins over info.bodyHeight; clamped only when overridden", () => {
    const agg = kindInfo("table.aggregate")!;
    expect(nodeLayout(agg, bare(), ui()).bodyH).toBe(88);            // un-overridden: kind default
    expect(nodeLayout(agg, bare(undefined, 200), ui()).bodyH).toBe(200);
    expect(nodeLayout(agg, bare(undefined, 5), ui()).bodyH).toBe(BODY_H_MIN);
    expect(nodeLayout(agg, bare(undefined, 9999), ui()).bodyH).toBe(BODY_H_MAX);
    // card height grows by exactly the body delta; body rect tracks it
    const l0 = nodeLayout(agg, bare(), ui());
    const l1 = nodeLayout(agg, bare(undefined, 200), ui());
    expect(l1.h - l0.h).toBe(200 - 88);
    expect(bodyRect(l1, rect(0, 0, l1.w, l1.h)).h - bodyRect(l0, rect(0, 0, l0.w, l0.h)).h)
      .toBe(200 - 88);
  });

  it("clampBodyH clamps to [BODY_H_MIN, BODY_H_MAX] and rounds", () => {
    expect(clampBodyH(0)).toBe(BODY_H_MIN);
    expect(clampBodyH(9999)).toBe(BODY_H_MAX);
    expect(clampBodyH(120.6)).toBe(121);
  });

  it("a taller body shows MORE mini-grid rows (gridLayout is height-clamped)", () => {
    const agg = kindInfo("table.aggregate")!;
    const preview = tablePreviewOf({
      columns: ["k", "v"],
      rows: Array.from({ length: 20 }, (_, i) => [`g${i}`, i]),
    });
    const rowsAt = (bh?: number) => {
      const l = nodeLayout(agg, bare(undefined, bh), ui());
      return gridLayout(bodyRect(l, rect(0, 0, l.w, l.h)), preview).rows;
    };
    expect(rowsAt(undefined)).toBe(5);               // default 88px body: 5 rows
    expect(rowsAt(200)).toBeGreaterThan(rowsAt(undefined));
    // height-clamped exactly: bodyRect insets 4px, then header strip + N rows
    expect(rowsAt(200)).toBe(Math.floor((200 - 4 - GRID_HEADER_H) / GRID_ROW_H));
  });

  it("chip mode has NO resize handle (hit zone + gesture both decline)", () => {
    const info = kindInfo("select.byType")!;
    const chip = nodeLayout(info, bare(), ui(0.4));
    expect(resizeHandleAt(chip, 0, 0, v(chip.w - 2, 5))).toBeNull();
    // gesture-level proof: begin() with a zoomed-out view declines the press
    const props: WidgetHostProps = {
      id: "n1", info, params: {}, wiredInputs: new Set(), helpOpen: false,
    };
    const fake = (zoom: number) => ({
      props, rect: rect(0, 0, chip.w, chip.h), view: { zoom },
    }) as unknown as Parameters<typeof resizeGesture.begin>[0];
    const q = { mods: {} } as unknown as Parameters<typeof resizeGesture.begin>[2];
    const full = nodeLayout(info, bare(), ui(1));
    const edge = v(full.w - 2, full.h - 2);
    expect(resizeGesture.begin(fake(0.4), edge, q)).toBeNull();
    expect(resizeGesture.begin(fake(1), edge, q)).not.toBeNull();
  });
});

describe("resize through the real gesture pipeline (headless drive)", () => {
  it("edge drag sets node.w, does not move the node, is ONE undo entry", () => {
    const d = fresh();
    const before = nodeOf(d, "type1");
    expect(before.w).toBeUndefined();
    const past0 = d.doc().past.length;
    const a = gripCenter(d, "type1");
    d.drag(a, v(a.x + 60, a.y), 6);
    const after = nodeOf(d, "type1");
    expect(after.w).toBe(NODE_W + 60);
    expect(v(after.x, after.y)).toEqual(v(before.x, before.y));   // resize ≠ move
    expect(d.doc().past.length).toBe(past0 + 1);                  // coalesced drag
    d.dispatch({ k: "undo" });
    expect(nodeOf(d, "type1").w).toBeUndefined();                 // back to unset
  });

  it("layout + socket anchors track the new width", () => {
    const d = fresh();
    const a = gripCenter(d, "type1");
    d.drag(a, v(a.x + 60, a.y), 6);
    d.settle();
    const { l, origin } = d.layoutOf("type1");
    expect(l.w).toBe(NODE_W + 60);
    expect(d.socketCenter({ node: "type1", dir: "out", slot: "out" }).x)
      .toBeCloseTo(origin.x + NODE_W + 60, 5);
  });

  it("clamps: cannot grow past NODE_W_MAX or shrink below NODE_W", () => {
    const d = fresh();
    const a = gripCenter(d, "type1");
    d.drag(a, v(a.x + 2000, a.y), 6);
    expect(nodeOf(d, "type1").w).toBe(NODE_W_MAX);
    d.settle();
    // wide-by-default kind shrinks below its kind width, floors at NODE_W
    const b = gripCenter(d, "sql1");
    d.drag(b, v(b.x - 2000, b.y), 6);
    expect(nodeOf(d, "sql1").w).toBe(NODE_W);
  });

  it("double-click on the grip resets to the kind default (explicit w — the intent cannot unset)", () => {
    const d = fresh();
    const a = gripCenter(d, "type1");
    d.drag(a, v(a.x + 60, a.y), 6);
    d.settle();
    expect(nodeOf(d, "type1").w).toBe(NODE_W + 60);
    const g = gripCenter(d, "type1");                 // grip moved with the edge
    d.click(g);
    d.click(gripCenter(d, "type1"));
    expect(nodeOf(d, "type1").w).toBe(NODE_W);        // reset = kind default, still defined
    // same for a kind with its own width
    const b = gripCenter(d, "sql1");
    d.drag(b, v(b.x + 40, b.y), 6);
    d.settle();
    d.click(gripCenter(d, "sql1"));
    d.click(gripCenter(d, "sql1"));
    expect(nodeOf(d, "sql1").w).toBe(240);
  });

  it("bottom-band drag on table.aggregate sets bh — no move, no w, ONE undo entry", () => {
    const d = fresh();
    const before = nodeOf(d, "agg1");
    expect(before.bh).toBeUndefined();
    const past0 = d.doc().past.length;
    const a = bandSCenter(d, "agg1");
    d.drag(a, v(a.x, a.y + 60), 6);
    const after = nodeOf(d, "agg1");
    expect(after.bh).toBe(88 + 60);                               // from the kind's 88
    expect(after.w).toBeUndefined();                              // "s" never edits width
    expect(v(after.x, after.y)).toEqual(v(before.x, before.y));   // resize ≠ move
    expect(d.doc().past.length).toBe(past0 + 1);                  // coalesced drag
    d.dispatch({ k: "undo" });
    expect(nodeOf(d, "agg1").bh).toBeUndefined();
  });

  it("taller body = more mini-grid rows through the real layout pipeline", () => {
    const d = fresh();
    const preview = tablePreviewOf({
      columns: ["k", "v"],
      rows: Array.from({ length: 20 }, (_, i) => [`g${i}`, i]),
    });
    d.dispatch({ k: "status", status: { agg1: { state: "ok", tablePreview: preview } } });
    const rowsNow = () => {
      const { l, origin } = d.layoutOf("agg1");
      return gridLayout(bodyRect(l, rect(origin.x, origin.y, l.w, l.h)), preview).rows;
    };
    const r0 = rowsNow();
    expect(r0).toBe(5);
    const a = bandSCenter(d, "agg1");
    d.drag(a, v(a.x, a.y + 112), 6);                              // 88 → 200
    expect(nodeOf(d, "agg1").bh).toBe(200);
    expect(rowsNow()).toBeGreaterThan(r0);
  });

  it("corner grip drags BOTH axes at once — still one undo entry", () => {
    const d = fresh();
    const past0 = d.doc().past.length;
    const a = gripCenter(d, "agg1");
    d.drag(a, v(a.x + 40, a.y + 30), 6);
    const after = nodeOf(d, "agg1");
    expect(after.w).toBe(NODE_W + 40);
    expect(after.bh).toBe(88 + 30);
    expect(d.doc().past.length).toBe(past0 + 1);
    d.dispatch({ k: "undo" });
    const undone = nodeOf(d, "agg1");
    expect(undone.w).toBeUndefined();
    expect(undone.bh).toBeUndefined();                            // both axes, one Ctrl-Z
  });

  it("corner grip on a body-less kind edits width only, never bh", () => {
    const d = fresh();
    const a = gripCenter(d, "type1");
    d.drag(a, v(a.x + 50, a.y + 50), 6);
    const after = nodeOf(d, "type1");
    expect(after.w).toBe(NODE_W + 50);
    expect(after.bh).toBeUndefined();
  });

  it("bh clamps to [BODY_H_MIN, BODY_H_MAX]", () => {
    const d = fresh();
    const a = bandSCenter(d, "agg1");
    d.drag(a, v(a.x, a.y + 2000), 6);
    expect(nodeOf(d, "agg1").bh).toBe(BODY_H_MAX);
    d.settle();
    const b = bandSCenter(d, "agg1");
    d.drag(b, v(b.x, b.y - 2000), 6);
    expect(nodeOf(d, "agg1").bh).toBe(BODY_H_MIN);
  });

  it("double-click on the grip resets BOTH axes to the kind defaults", () => {
    const d = fresh();
    const a = gripCenter(d, "agg1");
    d.drag(a, v(a.x + 40, a.y + 60), 6);
    d.settle();
    expect(nodeOf(d, "agg1").w).toBe(NODE_W + 40);
    expect(nodeOf(d, "agg1").bh).toBe(88 + 60);
    d.click(gripCenter(d, "agg1"));
    d.click(gripCenter(d, "agg1"));
    const reset = nodeOf(d, "agg1");
    expect(reset.w).toBe(NODE_W);                  // explicit — intent cannot unset
    expect(reset.bh).toBe(88);                     // kind bodyHeight, explicit
  });

  it("clipboard: copy/paste round-trips bh (trailing resize intent)", () => {
    const d = fresh();
    const a = bandSCenter(d, "agg1");
    d.drag(a, v(a.x, a.y + 60), 6);
    expect(nodeOf(d, "agg1").bh).toBe(148);
    const clip = copySelection(d.graph(), ["agg1"])!;
    expect(clip.nodes[0].bh).toBe(148);
    const { intent, ids } = pasteIntent(d.graph(), clip);
    d.dispatch({ k: "graph", intent });
    const pasted = nodeOf(d, ids[0]);
    expect(pasted.bh).toBe(148);
    expect(pasted.w).toBeUndefined();              // w untouched when only bh was set
  });

  it("a resize drag never starts the card-move or scrub gesture", () => {
    const d = fresh();
    const before = nodeOf(d, "sql1");
    const { l } = d.layoutOf("sql1");
    const live = d.rectOfKey("sql1")!;
    // press in the BAND next to a param row's right end (the 1px-overlap zone)
    const row0 = l.paramRows.findIndex((r) => r.h > 0);
    const y = live.y + l.paramRows[row0].top + l.paramRows[row0].h / 2;
    const a = v(live.right - 2, y);
    d.drag(a, v(a.x + 30, a.y), 6);
    const after = nodeOf(d, "sql1");
    expect(after.w).toBe(240 + 30);
    expect(v(after.x, after.y)).toEqual(v(before.x, before.y));
    expect(after.params.sql).toBe("SELECT 1");
  });
});
