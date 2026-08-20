// T9 (plan §3 W2, study #10): mini-grid table bodies — pure specs.
// Three halves: (1) the eval-side slice `tablePreviewOf` (main.ts applies it
// to every table-outputting node), (2) the grid cell geometry `gridLayout`
// from a body rect, (3) graceful degradation — table kinds carry no
// `bodyHeight` until the supervisor applies it at the join, so today's
// layouts are byte-identical with or without a tablePreview on status.
import { describe, expect, it } from "vitest";
import { rect } from "gratify";
import type { NodeKindInfo, NodeStatus, TableValue } from "../../contracts";
import { kindInfo } from "../../kinds";
import { evaluateGraph } from "../../flow/evaluate";
import { stubCtx } from "../../flow/tests/harness";
import { bodyRect, nodeLayout } from "../geom";
import {
  GRID_HEADER_H, GRID_MAX_COLS, GRID_MAX_ROWS, GRID_ROW_H,
  gridLayout, tablePreviewOf,
} from "../widgets";

const wide: TableValue = {
  columns: ["GlobalId", "Type", "Name", "Level", "carbon", "cost", "extra1", "extra2"],
  rows: Array.from({ length: 12 }, (_, r) =>
    Array.from({ length: 8 }, (_, c) => (c < 4 ? `r${r}c${c}` : r * 10 + c))),
};

const layoutOf = (info: NodeKindInfo, status?: NodeStatus) =>
  nodeLayout(info, { params: {}, wiredInputs: new Set() }, { helpOpen: false, zoom: 1 }, status);

describe("tablePreviewOf: the main.ts eval-side slice", () => {
  it("slices to first GRID_MAX_ROWS rows × first 6 columns", () => {
    const p = tablePreviewOf(wide);
    expect(p.columns).toEqual(wide.columns.slice(0, GRID_MAX_COLS));
    // cap raised to fill a BODY_H_MAX body (vertical resize): 12-row fixture
    // passes through whole; a taller table is cut at GRID_MAX_ROWS
    expect(p.rows.length).toBe(Math.min(wide.rows.length, GRID_MAX_ROWS));
    const tall: TableValue = {
      columns: ["a"], rows: Array.from({ length: GRID_MAX_ROWS + 10 }, (_, i) => [i]),
    };
    expect(tablePreviewOf(tall).rows.length).toBe(GRID_MAX_ROWS);
    expect(p.rows[0].length).toBe(GRID_MAX_COLS);
    expect(p.rows[4][0]).toBe("r4c0");
  });

  it("passes small tables through unchanged (no padding, no mutation)", () => {
    const small: TableValue = { columns: ["a", "b"], rows: [[1, 2], [3, null]] };
    const p = tablePreviewOf(small);
    expect(p).toEqual(small);
    expect(p.rows).not.toBe(small.rows);            // sliced copies, source untouched
  });

  it("empty table → empty preview", () => {
    expect(tablePreviewOf({ columns: [], rows: [] })).toEqual({ columns: [], rows: [] });
  });
});

describe("gridLayout: cell rects from a preview page + body area", () => {
  const preview = tablePreviewOf(wide);
  const area = rect(608, 462, 224, 84);              // a bodyHeight-88 body, inset 2+2

  it("header strip + 5×6 cells, all inside the area, golden", () => {
    const g = gridLayout(area, preview);
    expect(g.cols).toBe(6);
    expect(g.rows).toBe(5);
    expect(g.header.length).toBe(6);
    expect(g.cells.length).toBe(5);
    const all = [...g.header, ...g.cells.flat()];
    for (const r of all) {
      expect(r.x).toBeGreaterThanOrEqual(area.x);
      expect(r.right).toBeLessThanOrEqual(area.right + 0.001);
      expect(r.y).toBeGreaterThanOrEqual(area.y);
      expect(r.bottom).toBeLessThanOrEqual(area.bottom + 0.001);
    }
    expect(g.cells[0][0].y).toBe(area.y + GRID_HEADER_H);
    expect(g.cells[1][0].y - g.cells[0][0].y).toBe(GRID_ROW_H);
    expect(all.map((r) => ({ x: +r.x.toFixed(2), y: +r.y.toFixed(2), w: +r.w.toFixed(2), h: +r.h.toFixed(2) })))
      .toMatchSnapshot();
  });

  it("short areas clip rows; too-short areas yield no grid", () => {
    const short = rect(0, 0, 224, GRID_HEADER_H + 2.5 * GRID_ROW_H);
    expect(gridLayout(short, preview).rows).toBe(2);
    const tiny = rect(0, 0, 224, GRID_HEADER_H + GRID_ROW_H - 1);
    expect(gridLayout(tiny, preview).cols).toBe(0);
  });

  it("no columns → no grid", () => {
    expect(gridLayout(area, { columns: [], rows: [] }).cols).toBe(0);
  });
});

describe("table kinds carry the mini-grid body (bodyHeight 88 applied at the W2 join)", () => {
  const preview = tablePreviewOf(wide);
  const withPreview: NodeStatus = { state: "ok", summary: "12 rows", tablePreview: preview };

  for (const kind of ["table.sql", "table.ask", "table.fromScene",
    "table.filter", "table.sort", "table.aggregate"]) {
    it(`${kind}: body reserves 88px and the full 5×6 grid fits`, () => {
      const info = kindInfo(kind)!;
      expect(info.bodyHeight).toBe(88);
      const l = layoutOf(info, withPreview);
      expect(l.bodyH).toBe(88);
      const area = bodyRect(l, rect(0, 0, l.w, l.h));
      const g = gridLayout(area, preview);
      expect(area.y + GRID_HEADER_H + g.rows * GRID_ROW_H).toBeLessThanOrEqual(area.bottom);
    });
  }

  it("with the recommended bodyHeight 88, the 5×6 grid fits the body rect", () => {
    const info = { ...kindInfo("table.sql")!, bodyHeight: 88 };
    const l = layoutOf(info, withPreview);
    expect(l.bodyH).toBe(88);
    const area = bodyRect(l, rect(0, 0, l.w, l.h));
    const g = gridLayout(area, preview);
    expect(g.rows).toBe(5);                          // height-clamped at 88px (not the cap)
    expect(g.cols).toBe(GRID_MAX_COLS);
    expect(area.y + GRID_HEADER_H + g.rows * GRID_ROW_H).toBeLessThanOrEqual(area.bottom);
  });

  it("chart status wins the body over tablePreview (chart.bar unchanged)", () => {
    // Pinned as layout truth: chart.bar reserves its body via the kind; the
    // render branch prefers chart. Here we assert the kind still carries its
    // own bodyHeight so the chart path is unaffected by T9.
    expect(kindInfo("chart.bar")!.bodyHeight).toBe(96);
  });
});

describe("integration: the main.ts derivation loop over a real demo eval", () => {
  it("filter-sort: every table-outputting node gains a ≤5×6 preview", async () => {
    const { readFileSync } = await import("node:fs");
    const { fileURLToPath } = await import("node:url");
    const doc = JSON.parse(readFileSync(
      fileURLToPath(new URL("../../../../demo/filter-sort.json", import.meta.url)), "utf8"));
    // same stub-CSV rule as demos.test.ts: carry the column the demo joins on
    const { mockModel } = await import("../../fixtures/mockModel");
    const csv = "GlobalId,operational_carbon\n" +
      mockModel().globalIds.map((g, i) => `${g},${50 + (i * 37) % 400}`).join("\n");
    const res = await evaluateGraph(doc, stubCtx({ fetchText: async () => csv }));
    // mirror of main.ts's post-eval loop
    const isTable = (v: unknown): v is TableValue => !!v && typeof v === "object" && "rows" in v;
    for (const [id, st] of res.status) {
      const v = res.values.get(id);
      if (isTable(v)) st.tablePreview = tablePreviewOf(v);
    }
    for (const id of ["n4", "n5", "n6", "n8"]) {     // fromScene, filter, sort, sink
      const p = res.status.get(id)?.tablePreview;
      expect(p, `${id} preview`).toBeTruthy();
      expect(p!.rows.length).toBeLessThanOrEqual(GRID_MAX_ROWS);
      expect(p!.columns.length).toBeLessThanOrEqual(GRID_MAX_COLS);
      expect(p!.columns.length).toBeGreaterThan(0);
    }
    // scene nodes carry none
    expect(res.status.get("n1")?.tablePreview).toBeUndefined();
    expect(res.status.get("n3")?.tablePreview).toBeUndefined();
  });
});
