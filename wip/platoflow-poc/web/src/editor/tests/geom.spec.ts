// Layout goldens (plan §4.2 item 2): for every kind × {helpOpen} × {status},
// snapshot the row table + key layout offsets, and prove the hit-test
// round-trips — a drawn pixel and its hit test cannot drift. Pure functions
// only: no runtime, no Chrome, no DOM.
import { describe, expect, it } from "vitest";
import { rect, v } from "gratify";
import type { NodeKindInfo, NodeStatus } from "../../contracts";
import { KINDS } from "../../kinds";
import {
  CHIP_H, HEADER_H, NODE_W, nodeLayout, paramRowAt, paramRowRect, socketAt, socketPos,
  widgetFor, wiredInputsOf, ZOOM_CHIP_THRESHOLD, type LayoutNode, type LayoutUi,
} from "../geom";

const NO_STATUS: NodeStatus | undefined = undefined;
const CHART_STATUS: NodeStatus = {
  state: "ok",
  summary: "6 bars",
  detail: "SELECT Category, COUNT(*) AS n FROM EntityText GROUP BY Category",
  chart: {
    labels: ["Walls", "Doors", "Windows", "Slabs", "Beams", "Roofs"],
    values: [412, 128, 96, 260, 44, 180],
    title: "carbon by category",
  },
};

const bare = (_info: NodeKindInfo): LayoutNode =>
  ({ params: {}, wiredInputs: new Set<string>() });
const ui = (helpOpen: boolean): LayoutUi => ({ helpOpen, zoom: 1 });

describe("nodeLayout goldens: every kind × helpOpen × status", () => {
  for (const info of KINDS) {
    for (const helpOpen of [false, true]) {
      for (const [statusName, status] of [["no-status", NO_STATUS], ["chart-status", CHART_STATUS]] as const) {
        const label = `${info.kind} help=${helpOpen} ${statusName}`;

        it(`${label}: row table + offsets`, () => {
          const l = nodeLayout(info, bare(info), ui(helpOpen), status);
          expect({
            w: l.w, h: l.h,
            helpH: l.helpH, helpLineCount: l.helpLines.length,
            socketsTop: l.socketsTop, paramsTop: l.paramsTop,
            bodyTop: l.bodyTop, bodyH: l.bodyH, footerTop: l.footerTop,
            paramRows: l.paramRows,
          }).toMatchSnapshot();
        });

        it(`${label}: hit-test round-trips`, () => {
          const l = nodeLayout(info, bare(info), ui(helpOpen), status);
          const nx = 100, ny = 200;
          const card = rect(nx, ny, l.w, l.h);
          // every visible param row: center of its rect hits back to its name
          l.paramRows.forEach((row, i) => {
            if (row.h <= 0) return;
            const c = paramRowRect(l, card, i).center;
            expect(paramRowAt(l, nx, ny, c)).toBe(row.name);
          });
          // every socket: its published position hits back to (dir, index)
          (["in", "out"] as const).forEach((dir) => {
            const list = dir === "in" ? info.inputs : info.outputs;
            list.forEach((spec, i) => {
              const hit = socketAt(l, info, nx, ny, socketPos(l, card, dir, i));
              expect(hit).toMatchObject({ dir, index: i });
              expect(hit?.spec.name).toBe(spec.name);
            });
          });
        });
      }
    }
  }
});

describe("W0 layout invariants", () => {
  it("per-kind width is honored (table.sql/table.ask/chart.bar = 240, rest default)", () => {
    for (const info of KINDS) {
      const l = nodeLayout(info, bare(info), ui(false));
      expect(l.w).toBe(info.width ?? NODE_W);
    }
  });

  it("every visible row is 16px; widget follows the W1 schema mapping", () => {
    for (const info of KINDS) {
      const l = nodeLayout(info, bare(info), ui(false));
      for (const row of l.paramRows) {
        const schema = info.params[row.name];
        const expected = schema.kind === "number" ? "scrub"
          : schema.kind === "boolean" ? "toggle"
          : schema.kind === "enum" && schema.options.length <= 5 ? "chips"
          : "row";
        expect(row.widget).toBe(expected);
        expect(row.h).toBe(16);
      }
    }
  });

  it("zoom contract (T10): identical at/above threshold, chip mode below", () => {
    for (const info of KINDS) {
      const a = nodeLayout(info, bare(info), { helpOpen: false, zoom: 1 });
      // at and above the threshold the layout is byte-identical to zoom 1
      expect(nodeLayout(info, bare(info), { helpOpen: false, zoom: ZOOM_CHIP_THRESHOLD })).toEqual(a);
      expect(nodeLayout(info, bare(info), { helpOpen: false, zoom: 0.75 })).toEqual(a);
      expect(a.chip).toBe(false);
      // below the threshold: title + status chip only (details in zoom.spec.ts)
      const c = nodeLayout(info, bare(info), { helpOpen: false, zoom: 0.3 });
      expect(c.chip).toBe(true);
      expect(c.h).toBe(HEADER_H + CHIP_H);
    }
  });

  it("widgetFor: schema mapping, 'hidden' always wins when wired", () => {
    expect(widgetFor({ kind: "number" }, false)).toBe("scrub");
    expect(widgetFor({ kind: "number" }, true)).toBe("hidden");
    expect(widgetFor({ kind: "boolean" }, false)).toBe("toggle");
    expect(widgetFor({ kind: "enum", options: ["a", "b"] }, false)).toBe("chips");
    expect(widgetFor({ kind: "enum", options: ["1", "2", "3", "4", "5", "6"] }, false)).toBe("row");
    expect(widgetFor({ kind: "text" }, false)).toBe("row");
  });

  it("connected-input rule machinery: a wired param collapses its row and shifts the rest", () => {
    // Fabricated kind — today no real port shares a param's name (§2.4 note).
    const info: NodeKindInfo = {
      kind: "t.wired", label: "T", category: "data", description: "test",
      inputs: [{ name: "gain", type: "table" }], outputs: [],
      params: { gain: { kind: "number", default: 1 }, bias: { kind: "number", default: 0 } },
    };
    const edges = [{ from: { node: "a", slot: "out" }, to: { node: "b", slot: "gain" } }];
    const wired = wiredInputsOf(edges, "b", info);
    expect(wired).toEqual(new Set(["gain"]));

    const open = nodeLayout(info, { params: {}, wiredInputs: new Set() }, ui(false));
    const coll = nodeLayout(info, { params: {}, wiredInputs: wired }, ui(false));
    expect(coll.h).toBe(open.h - 16);
    expect(coll.paramRows[0]).toMatchObject({ name: "gain", widget: "hidden", h: 0 });
    expect(coll.paramRows[1].top).toBe(coll.paramRows[0].top);  // bias moved up
    // the hidden row is unhittable; the visible one still round-trips
    const card = rect(0, 0, coll.w, coll.h);
    expect(paramRowAt(coll, 0, 0, paramRowRect(coll, card, 1).center)).toBe("bias");
    expect(paramRowAt(coll, 0, 0, v(coll.w / 2, coll.paramRows[0].top + 1))).toBe("bias");
  });

  it("wiredInputsOf ignores edges landing on real ports (no param name collision today)", () => {
    for (const info of KINDS) {
      const id = "x";
      const edges = info.inputs.map((p) => ({ from: { node: "s", slot: "out" }, to: { node: id, slot: p.name } }));
      expect(wiredInputsOf(edges, id, info).size).toBe(0);
    }
  });
});
