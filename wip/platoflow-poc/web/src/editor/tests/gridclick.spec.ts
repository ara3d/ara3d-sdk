// Wave 10 Track A: mini-grid row click → 3D highlight seam. Contract under test:
//  - gridRowAt is pure shared geometry (gridLayout's rects ARE the hit zones):
//    row centers round-trip, the header strip / empty space / outside-x miss;
//  - a clean CLICK on a visible grid row fires onGridRowClick with
//    (nodeId, PREVIEW row index) and updates clickedGridRow(); the card does
//    not move and the click still selects the node (card `.press` composes);
//  - a DRAG from a grid row never fires the seam and STILL MOVES the card —
//    the gesture delegates to nodeMoveGesture instead of eating the drag
//    (unlike toggle/chips rows);
//  - the resize bottom band still wins (declared before the grid gesture and
//    outside bodyRect): a band drag sets bh, no seam fire;
//  - a chart body never claims (chart wins the area when both are present),
//    nor does a body with no tablePreview, nor chip mode (zoom < threshold);
//  - unsubscribe works; clickedGridRow() updates even with zero listeners.
//
// Row-index semantics: tablePreview is a ≤GRID_MAX_ROWS-row PAGE of the real
// table (tablePreviewOf), so the fired index maps to the preview — exactly the
// row the user clicked. main.ts resolves it against status.tablePreview.rows.
import { describe, expect, it } from "vitest";
import { rect, v, type Query, type Vec } from "gratify";
import type { GraphDoc, TableValue } from "../../contracts";
import { kindInfo } from "../../kinds";
import { bodyRect, nodeLayout, RESIZE_BAND } from "../geom";
import {
  clickedGridRow, GRID_HEADER_H, GRID_ROW_H, gridClickGesture, gridLayout, gridRowAt,
  onGridRowClick, tablePreviewOf,
} from "../widgets";
import { createDrive, type Drive } from "./drive";

const preview: TableValue = tablePreviewOf({
  columns: ["GlobalId", "Type", "Level"],
  rows: Array.from({ length: 8 }, (_, i) => [`gid-${i}`, `T${i}`, `L${i % 2}`]),
});

const DOC: GraphDoc = {
  name: "gridclick-fixture",
  nodes: [
    // body-carrying table kind (bodyHeight 88 → 5 visible preview rows)
    { id: "agg1", kind: "table.aggregate", params: {}, x: 100, y: 80 },
    { id: "agg2", kind: "table.aggregate", params: {}, x: 480, y: 80 },
  ],
  edges: [],
  display: null,
};

const fresh = (status: Record<string, object> = { agg1: { state: "ok", tablePreview: preview } }) => {
  const d = createDrive();
  d.load(DOC);
  d.dispatch({ k: "status", status: status as never });
  d.settle();
  return d;
};

const nodeOf = (d: Drive, id: string) => d.graph().nodes.find((n) => n.id === id)!;

/** Center of visible preview row `row` (first column), from the LIVE instance
 *  rect through the same bodyRect + gridLayout the render uses — §4.4: no
 *  magic coordinates. Viewport is identity in these specs (no pan/zoom), so
 *  world coords are screen coords. */
const rowCenter = (d: Drive, id: string, row: number): Vec => {
  const { l, origin } = d.layoutOf(id);
  const area = bodyRect(l, rect(origin.x, origin.y, l.w, l.h));
  const g = gridLayout(area, preview);
  if (row >= g.rows) throw new Error(`row ${row} not visible (${g.rows} rows)`);
  return g.cells[row][0].center;
};

describe("gridRowAt (pure shared hit-test)", () => {
  const info = kindInfo("table.aggregate")!;
  const l = nodeLayout(info, { params: {}, wiredInputs: new Set() }, { helpOpen: false, zoom: 1 });
  const area = bodyRect(l, rect(50, 60, l.w, l.h));
  const g = gridLayout(area, preview);

  it("every visible row's cell centers report their own index", () => {
    g.cells.forEach((rowRects, ri) =>
      rowRects.forEach((r) => expect(gridRowAt(area, preview, r.center)).toBe(ri)));
  });

  it("header strip, outside-x and area above/below all miss", () => {
    expect(gridRowAt(area, preview, v(area.center.x, area.y + GRID_HEADER_H / 2))).toBeNull();
    expect(gridRowAt(area, preview, v(area.x - 2, area.y + GRID_HEADER_H + GRID_ROW_H / 2))).toBeNull();
    expect(gridRowAt(area, preview, v(area.right + 2, area.y + GRID_HEADER_H + GRID_ROW_H / 2))).toBeNull();
    expect(gridRowAt(area, preview, v(area.center.x, area.y - 5))).toBeNull();
    // just past the last VISIBLE row (height-clamped, not preview-length)
    expect(gridRowAt(area, preview, v(area.center.x, area.y + GRID_HEADER_H + g.rows * GRID_ROW_H + 1)))
      .toBeNull();
  });

  it("empty table / too-short area yield no rows to hit", () => {
    const empty: TableValue = { columns: [], rows: [] };
    expect(gridRowAt(area, empty, area.center)).toBeNull();
    const short = rect(area.x, area.y, area.w, GRID_HEADER_H); // no room for a row
    expect(gridRowAt(short, preview, short.center)).toBeNull();
  });
});

describe("grid-row click through the real gesture pipeline (headless drive)", () => {
  it("clean click fires the seam with (node, preview row); card does not move; node selected", () => {
    const d = fresh();
    const fired: [string, number][] = [];
    const unsub = onGridRowClick((n, r) => fired.push([n, r]));
    const before = nodeOf(d, "agg1");
    d.click(rowCenter(d, "agg1", 2));
    expect(fired).toEqual([["agg1", 2]]);
    expect(clickedGridRow()).toEqual({ node: "agg1", row: 2 });
    const after = nodeOf(d, "agg1");
    expect(v(after.x, after.y)).toEqual(v(before.x, before.y));   // click ≠ move
    expect(d.doc().sel).toEqual({ kind: "node", id: "agg1" });    // press composes
    d.click(rowCenter(d, "agg1", 0));
    expect(fired).toEqual([["agg1", 2], ["agg1", 0]]);
    unsub();
  });

  it("drag from a grid row: seam does NOT fire, card STILL moves (delegation)", () => {
    const d = fresh();
    const fired: [string, number][] = [];
    const unsub = onGridRowClick((n, r) => fired.push([n, r]));
    const before = nodeOf(d, "agg1");
    const a = rowCenter(d, "agg1", 1);
    d.drag(a, v(a.x + 60, a.y + 40), 6);
    d.settle();
    expect(fired).toEqual([]);                                    // no click fired
    const after = nodeOf(d, "agg1");
    expect(after.x).toBeCloseTo(before.x + 60, 0);                // move delegated
    expect(after.y).toBeCloseTo(before.y + 40, 0);
    unsub();
  });

  it("resize bottom band still wins: band drag sets bh, no seam fire", () => {
    const d = fresh();
    const fired: [string, number][] = [];
    const unsub = onGridRowClick((n, r) => fired.push([n, r]));
    const r = d.rectOfKey("agg1")!;
    const a = v(r.x + r.w / 2, r.bottom - RESIZE_BAND / 2);
    d.drag(a, v(a.x, a.y + 40), 6);
    expect(nodeOf(d, "agg1").bh).toBe(88 + 40);                   // resize claimed
    expect(fired).toEqual([]);
    unsub();
  });

  it("chart body never claims: click with chart+tablePreview fires nothing", () => {
    const d = fresh({
      agg1: {
        state: "ok",
        tablePreview: preview,
        chart: { labels: ["a", "b"], values: [1, 2] },            // chart wins the area
      },
    });
    const fired: [string, number][] = [];
    const unsub = onGridRowClick((n, r) => fired.push([n, r]));
    d.click(rowCenter(d, "agg1", 1));                             // same coords as a grid row
    expect(fired).toEqual([]);
    expect(d.doc().sel).toEqual({ kind: "node", id: "agg1" });    // plain select still works
    unsub();
  });

  it("body without a tablePreview never claims; drags still move the card", () => {
    const d = fresh({ agg1: { state: "pending" } });              // bodyH 88, no preview
    const fired: [string, number][] = [];
    const unsub = onGridRowClick((n, r) => fired.push([n, r]));
    const before = nodeOf(d, "agg1");
    const a = rowCenter(d, "agg1", 1);                            // geometry exists regardless
    d.click(a);
    expect(fired).toEqual([]);
    d.drag(a, v(a.x + 50, a.y), 6);
    d.settle();
    expect(nodeOf(d, "agg1").x).toBeCloseTo(before.x + 50, 0);
    unsub();
  });

  it("seam is per-node: clicking agg2's grid names agg2", () => {
    const d = fresh({
      agg1: { state: "ok", tablePreview: preview },
      agg2: { state: "ok", tablePreview: preview },
    });
    const fired: [string, number][] = [];
    const unsub = onGridRowClick((n, r) => fired.push([n, r]));
    d.click(rowCenter(d, "agg2", 3));
    expect(fired).toEqual([["agg2", 3]]);
    unsub();
  });

  it("unsubscribe works; clickedGridRow() updates even with zero listeners", () => {
    const d = fresh();
    const fired: [string, number][] = [];
    const unsub = onGridRowClick((n, r) => fired.push([n, r]));
    unsub();
    d.click(rowCenter(d, "agg1", 4));
    expect(fired).toEqual([]);                                    // unsubscribed
    expect(clickedGridRow()).toEqual({ node: "agg1", row: 4 });   // accessor still tracks
  });
});

describe("gesture-level declines (fake-node probes, resize.spec pattern)", () => {
  const info = kindInfo("table.aggregate")!;
  const q = { mods: {} } as unknown as Query;
  const mkNode = (zoom: number, status: object | undefined) => {
    const l = nodeLayout(info, { params: {}, wiredInputs: new Set() }, { helpOpen: false, zoom: 1 });
    const props = {
      id: "n1", info, pos: v(0, 0), params: {}, wiredInputs: new Set<string>(),
      display: false, helpOpen: false, status,
    };
    return {
      n: { props, rect: rect(0, 0, l.w, l.h), view: { zoom } } as unknown as
        Parameters<typeof gridClickGesture.begin>[0],
      l,
    };
  };
  const rowPoint = (l: ReturnType<typeof nodeLayout>) => {
    const area = bodyRect(l, rect(0, 0, l.w, l.h));
    return gridLayout(area, preview).cells[0][0].center;
  };

  it("claims a row press at zoom 1, declines the SAME point in chip mode", () => {
    const ok = mkNode(1, { state: "ok", tablePreview: preview });
    const p = rowPoint(ok.l);
    expect(gridClickGesture.begin(ok.n, p, q)).not.toBeNull();
    const chip = mkNode(0.4, { state: "ok", tablePreview: preview });
    expect(gridClickGesture.begin(chip.n, p, q)).toBeNull();
  });

  it("declines the header strip and points below the last visible row", () => {
    const { n, l } = mkNode(1, { state: "ok", tablePreview: preview });
    const area = bodyRect(l, rect(0, 0, l.w, l.h));
    expect(gridClickGesture.begin(n, v(area.center.x, area.y + GRID_HEADER_H / 2), q)).toBeNull();
    const g = gridLayout(area, preview);
    expect(gridClickGesture.begin(
      n, v(area.center.x, area.y + GRID_HEADER_H + g.rows * GRID_ROW_H + 1), q)).toBeNull();
  });

  it("declines without status / without preview / with a chart", () => {
    const { l } = mkNode(1, undefined);
    const p = rowPoint(l);
    expect(gridClickGesture.begin(mkNode(1, undefined).n, p, q)).toBeNull();
    expect(gridClickGesture.begin(mkNode(1, { state: "ok" }).n, p, q)).toBeNull();
    expect(gridClickGesture.begin(mkNode(1, {
      state: "ok", tablePreview: preview, chart: { labels: ["a"], values: [1] },
    }).n, p, q)).toBeNull();
  });
});
