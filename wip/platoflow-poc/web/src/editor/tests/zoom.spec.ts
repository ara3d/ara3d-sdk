// T10 semantic zoom (study item #11): below ZOOM_CHIP_THRESHOLD a node lays
// out as title + status chip only. Contract under test:
//  - threshold is exact: zoom < 0.5 = chip; zoom >= 0.5 = full layout,
//    byte-identical to zoom 1 (geom.spec.ts pins the >= half across kinds);
//  - chip mode: h = HEADER_H + CHIP_H; width PRESERVED (info.width ?? NODE_W)
//    so wire endpoints don't jump horizontally at the threshold crossing;
//  - sockets collapse to the header's centre line (Blender collapsed-node
//    style) but stay hit-testable; stacked same-side sockets resolve to the
//    lowest index;
//  - param rows keep names/order but h = 0 — paramRowAt can hit nothing;
//  - help/body/footer heights are 0; helpOpen is ignored in chip mode.
// Pure functions only: no runtime, no Chrome, no DOM.
import { describe, expect, it } from "vitest";
import { rect, v } from "gratify";
import type { NodeKindInfo } from "../../contracts";
import { KINDS, kindInfo } from "../../kinds";
import {
  CHIP_H, HEADER_H, NODE_W, nodeLayout, paramRowAt, paramRowRect,
  socketAt, socketPos, socketY, ZOOM_CHIP_THRESHOLD, type LayoutNode,
} from "../geom";

const bare = (): LayoutNode => ({ params: {}, wiredInputs: new Set<string>() });
const at = (zoom: number, helpOpen = false) => ({ helpOpen, zoom });

describe("T10 semantic zoom: chip-mode layout", () => {
  it("threshold boundary is exact (chip strictly below 0.5)", () => {
    const info = KINDS[0];
    expect(ZOOM_CHIP_THRESHOLD).toBe(0.5);
    expect(nodeLayout(info, bare(), at(ZOOM_CHIP_THRESHOLD)).chip).toBe(false);
    expect(nodeLayout(info, bare(), at(0.4999999)).chip).toBe(true);
    expect(nodeLayout(info, bare(), at(0.4)).chip).toBe(true);
  });

  it("every kind: chip height fixed, width preserved, zero-height regions", () => {
    for (const info of KINDS) {
      const l = nodeLayout(info, bare(), at(0.4));
      expect(l.chip).toBe(true);
      expect(l.h).toBe(HEADER_H + CHIP_H);
      expect(l.w).toBe(info.width ?? NODE_W);      // wide kinds STAY wide
      expect(l.helpH).toBe(0);
      expect(l.helpLines).toEqual([]);
      expect(l.bodyH).toBe(0);
      expect(l.footerTop).toBe(HEADER_H);
      for (const row of l.paramRows) expect(row.h).toBe(0);
      // names/order still line up with the schema (indexes stay valid)
      expect(l.paramNames).toEqual(Object.keys(info.params));
    }
  });

  it("helpOpen is ignored in chip mode", () => {
    for (const info of KINDS) {
      expect(nodeLayout(info, bare(), at(0.4, true)))
        .toEqual(nodeLayout(info, bare(), at(0.4, false)));
    }
  });

  it("sockets collapse to the header centre line and stay hit-testable", () => {
    const nx = 40, ny = 70;
    for (const info of KINDS) {
      const l = nodeLayout(info, bare(), at(0.4));
      const card = rect(nx, ny, l.w, l.h);
      const seen = new Set<string>();
      (["in", "out"] as const).forEach((dir) => {
        const list = dir === "in" ? info.inputs : info.outputs;
        list.forEach((spec, i) => {
          const c = socketPos(l, card, dir, i);
          expect(c.y).toBe(ny + HEADER_H / 2);     // all anchors on the header line
          expect(c.x).toBe(dir === "in" ? card.x : card.right);
          const hit = socketAt(l, info, nx, ny, c);
          expect(hit).not.toBeNull();
          expect(hit!.dir).toBe(dir);
          // stacked same-side sockets overlap; lowest index wins
          if (seen.has(dir)) expect(hit!.index).toBe(0);
          else expect(hit!.index).toBe(i);
          seen.add(dir);
        });
      });
      // row index is ignored by socketY in chip mode
      expect(socketY(l, ny, 0)).toBe(socketY(l, ny, 3));
    }
  });

  it("param hit-testing returns nothing anywhere on the chip", () => {
    for (const info of KINDS) {
      if (!Object.keys(info.params).length) continue;
      const full = nodeLayout(info, bare(), at(1));
      const chip = nodeLayout(info, bare(), at(0.4));
      const nx = 10, ny = 20;
      const fullCard = rect(nx, ny, full.w, full.h);
      // where the rows WOULD be at zoom 1, and a sweep over the chip itself
      full.paramRows.forEach((_row, i) => {
        const c = paramRowRect(full, fullCard, i).center;
        expect(paramRowAt(chip, nx, ny, c)).toBeNull();
      });
      for (let y = 0; y <= chip.h; y += 4) {
        expect(paramRowAt(chip, nx, ny, v(nx + chip.w / 2, ny + y))).toBeNull();
      }
      // zero-area row rects (unhittable by construction)
      const chipCard = rect(nx, ny, chip.w, chip.h);
      chip.paramRows.forEach((_row, i) => {
        const r = paramRowRect(chip, chipCard, i);
        expect(r.w * r.h).toBe(0);
      });
    }
  });

  it("goldens: one wide kind + one normal kind at low zoom", () => {
    const wide = kindInfo("table.sql")!;         // width: 240
    const normal = kindInfo("load.model")!;      // default NODE_W
    for (const info of [wide, normal] as NodeKindInfo[]) {
      const l = nodeLayout(info, bare(), at(0.4));
      expect({
        kind: info.kind, chip: l.chip, w: l.w, h: l.h,
        helpH: l.helpH, socketsTop: l.socketsTop, paramsTop: l.paramsTop,
        bodyTop: l.bodyTop, bodyH: l.bodyH, footerTop: l.footerTop,
        paramRows: l.paramRows,
      }).toMatchSnapshot();
    }
  });
});
