// @vitest-environment jsdom
// T8: the searchable picker organ (study #8) + auto-open-on-drop (study #13).
// One suite owns picker behavior: pure helpers, the DOM organ standalone, and
// the params.ts routing/integration (usePicker latch, openFirstUnset seam).
// The legacy <select> path keeps its coverage in params.spec.ts until the
// supervisor flips `usePicker` and applies the spec diffs from the T8 report.
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import type { GraphNode, ParamSchema } from "../../contracts";
import { kindInfo } from "../../kinds";
import type { EditorIntent } from "../doc";
import { createParamPopover, type ParamPopover } from "../params";
import {
  createPicker, filterOptions, firstUnsetPickable, normalizeOptions,
  RENDER_CAP, type Picker, type RawEnumOption,
} from "../picker";

const TYPES: RawEnumOption[] = [
  { value: "IfcWall", count: 216 }, { value: "IfcDoor", count: 14 }, "IfcSlab",
];
const DYN_TYPES: ParamSchema & { kind: "dynamicEnum" } =
  { kind: "dynamicEnum", source: "types" };

const node = (kind: string, id: string, params: Record<string, unknown> = {}): GraphNode =>
  ({ id, kind, params, x: 0, y: 0 });

const setParams = (sent: EditorIntent[]) =>
  sent.flatMap((i) => (i.k === "graph" && i.intent.t === "setParam" ? [i.intent] : []));

const pickerEl = () => document.querySelector<HTMLElement>(".pf-picker")!;
const search = () => pickerEl().querySelector("input")!;
const rows = () => [...pickerEl().querySelectorAll<HTMLElement>(".pf-picker-row")];
const rowTexts = () => rows().map((r) => r.querySelector(".pf-picker-val")!.textContent);
const type = (text: string) => {
  search().value = text;
  search().dispatchEvent(new Event("input", { bubbles: true }));
};
const key = (el: Element | Window, k: string) =>
  el.dispatchEvent(new KeyboardEvent("keydown", { key: k, bubbles: true, cancelable: true }));

// ── pure helpers ─────────────────────────────────────────────────────────────

describe("picker pure helpers", () => {
  it("normalizeOptions lifts bare strings, keeps counts", () => {
    expect(normalizeOptions(["a", { value: "b", count: 3 }]))
      .toEqual([{ value: "a" }, { value: "b", count: 3 }]);
  });

  it("filterOptions: case-insensitive substring; blank keeps all", () => {
    const opts = normalizeOptions(["IfcWall", "IfcDoor", "IfcWallStandardCase"]);
    expect(filterOptions(opts, "wall").map((o) => o.value))
      .toEqual(["IfcWall", "IfcWallStandardCase"]);
    expect(filterOptions(opts, "  ")).toEqual(opts);
  });

  it("firstUnsetPickable: unset required dynamicEnum found; defaults + set values skip", () => {
    const byType = kindInfo("select.byType")!;
    expect(firstUnsetPickable(byType, node("select.byType", "t1", { type: "" }))).toBe("type");
    expect(firstUnsetPickable(byType, node("select.byType", "t1", { type: "IfcWall" }))).toBeNull();
    // modelPick with a schema default is not "required" — never auto-asks
    const load = kindInfo("load.model")!;
    expect(firstUnsetPickable(load, node("load.model", "l1", { model: "" }))).toBeNull();
    // multi-param kind: the defaulted column is skipped, the required one found
    const attach = kindInfo("attach.column")!;
    expect(firstUnsetPickable(attach,
      node("attach.column", "a1", { keyColumn: "GlobalId", valueColumn: "" }))).toBe("valueColumn");
  });
});

// ── the DOM organ, standalone ────────────────────────────────────────────────

describe("searchable picker (DOM)", () => {
  let sent: EditorIntent[] = [];
  let picker: Picker;
  let options: RawEnumOption[] = TYPES;

  beforeEach(() => {
    sent = [];
    options = TYPES;
    picker = createPicker({
      dispatch: (i) => sent.push(i),
      getEnumOptions: (_n, _p, source) => (source === "types" ? options : []),
    });
  });
  afterEach(() => { picker.destroy(); document.body.replaceChildren(); });

  const open = (params: Record<string, unknown> = {}) =>
    picker.open(node("select.byType", "t1", params), "type", DYN_TYPES);

  it("opens with all options plus the clear row, focuses the filter input", () => {
    open();
    expect(pickerEl().style.display).toBe("block");
    expect(rowTexts()).toEqual(["—", "IfcWall", "IfcDoor", "IfcSlab"]);
    expect(document.activeElement).toBe(search());
    expect(picker.current()).toEqual({ node: "t1", name: "type" });
  });

  it("renders counts dim when the provider supplies them", () => {
    open();
    const wall = rows().find((r) => r.textContent?.includes("IfcWall"))!;
    expect(wall.querySelector(".pf-picker-count")!.textContent).toBe("(216)");
    const slab = rows().find((r) => r.textContent?.includes("IfcSlab"))!;
    expect(slab.querySelector(".pf-picker-count")).toBeNull();
  });

  it("typing filters the list (clear row filtered away too)", () => {
    open();
    type("door");
    expect(rowTexts()).toEqual(["IfcDoor"]);
    type("");
    expect(rowTexts()).toEqual(["—", "IfcWall", "IfcDoor", "IfcSlab"]);
  });

  it("keyboard: arrows move the active row, Enter commits ONCE and closes", () => {
    open();
    key(search(), "ArrowDown");                  // — → IfcWall
    key(search(), "ArrowDown");                  // IfcWall → IfcDoor
    key(search(), "ArrowUp");                    // back to IfcWall
    key(search(), "Enter");
    expect(setParams(sent)).toEqual(
      [{ t: "setParam", node: "t1", name: "type", value: "IfcWall" }]);
    expect(picker.current()).toBeNull();
    expect(pickerEl().style.display).toBe("none");
  });

  it("Esc cancels: closed, nothing dispatched", () => {
    open();
    key(search(), "Escape");
    expect(setParams(sent)).toEqual([]);
    expect(picker.current()).toBeNull();
  });

  it("click commits live: one setParam, closed", () => {
    open();
    rows().find((r) => r.textContent?.includes("IfcSlab"))!
      .dispatchEvent(new MouseEvent("click", { bubbles: true }));
    expect(setParams(sent)).toEqual(
      [{ t: "setParam", node: "t1", name: "type", value: "IfcSlab" }]);
    expect(pickerEl().style.display).toBe("none");
  });

  it("current value is highlighted and is the keyboard start row", () => {
    open({ type: "IfcDoor" });
    const cur = pickerEl().querySelector(".pf-picker-cur")!;
    expect(cur.textContent).toContain("IfcDoor");
    expect(cur.classList.contains("pf-picker-active")).toBe(true);
    key(search(), "ArrowDown");                  // IfcDoor → IfcSlab
    key(search(), "Enter");
    expect(setParams(sent).map((i) => i.value)).toEqual(["IfcSlab"]);
  });

  it("keeps a current value the source no longer lists (injected row)", () => {
    open({ type: "IfcGhost" });
    expect(rowTexts()).toContain("IfcGhost");
    expect(pickerEl().querySelector(".pf-picker-cur")!.textContent).toContain("IfcGhost");
  });

  it("clear row commits the empty string", () => {
    open({ type: "IfcWall" });
    rows()[0].dispatchEvent(new MouseEvent("click", { bubbles: true }));
    expect(setParams(sent)).toEqual(
      [{ t: "setParam", node: "t1", name: "type", value: "" }]);
  });

  it(`caps rendering at ${RENDER_CAP} with a keep-typing footer; narrowing clears it`, () => {
    options = Array.from({ length: 120 }, (_, i) => ({ value: `item${i}`, count: i }));
    open();
    expect(rows().length).toBe(RENDER_CAP);      // clear row + 49 options
    const more = pickerEl().querySelector<HTMLElement>(".pf-picker-more")!;
    expect(more.textContent).toBe("…71 more, keep typing");   // 121 total − 50 shown
    type("item11");                              // item11, item110..item119
    expect(rows().length).toBe(11);
    expect(pickerEl().querySelector(".pf-picker-more")).toBeNull();
  });

  it("pointerdown inside the picker does not propagate (dismissal-churn guard)", () => {
    open();
    let seen = 0;
    const count = () => { seen++; };
    document.addEventListener("pointerdown", count);
    search().dispatchEvent(new Event("pointerdown", { bubbles: true }));
    document.removeEventListener("pointerdown", count);
    expect(seen).toBe(0);
  });
});

// ── params.ts routing + the auto-open seam ───────────────────────────────────

describe("params.ts routing (usePicker latch) + openFirstUnset", () => {
  let sent: EditorIntent[] = [];
  let pop: ParamPopover;

  const make = (usePicker: boolean) =>
    createParamPopover({
      dispatch: (i) => sent.push(i),
      kindInfo,
      getEnumOptions: (_n, _p, source) =>
        source === "types" ? TYPES :
        source === "models" ? ["duplex", "snowdon"] : [],
      usePicker,
    });

  beforeEach(() => { sent = []; });
  afterEach(() => { pop.destroy(); document.body.replaceChildren(); });

  it("usePicker: dynamicEnum routes to the picker, popover stays closed", () => {
    pop = make(true);
    pop.open(node("select.byType", "t1", { type: "" }), "type");
    expect(pickerEl().style.display).toBe("block");
    expect(document.querySelector<HTMLElement>(".pf-popover")!.style.display).toBe("none");
    expect(pop.current()).toEqual({ node: "t1", name: "type" });
    expect(pop.contains(search())).toBe(true);   // click-away must not close it
  });

  it("usePicker: modelPick routes too, with the schema default highlighted", () => {
    pop = make(true);
    pop.open(node("load.model", "l1", { model: "" }), "model");
    expect(rowTexts()).toEqual(["—", "duplex", "snowdon"]);
    expect(pickerEl().querySelector(".pf-picker-cur")!.textContent).toContain("snowdon");
  });

  it("without the latch the legacy <select> path is untouched (params.spec.ts pin)", () => {
    pop = make(false);
    pop.open(node("select.byType", "t1", { type: "" }), "type");
    const popover = document.querySelector<HTMLElement>(".pf-popover")!;
    expect(popover.style.display).toBe("block");
    // counts are stripped for the select: bare values only
    expect([...popover.querySelectorAll("option")].map((o) => o.value))
      .toEqual(["", "IfcWall", "IfcDoor", "IfcSlab"]);
  });

  it("openFirstUnset asks the right question and its pick dispatches once", () => {
    pop = make(true);
    const n = node("select.byType", "t9", { type: "" });
    expect(pop.openFirstUnset(n)).toBe("type");
    expect(pickerEl().style.display).toBe("block");
    expect(pop.current()).toEqual({ node: "t9", name: "type" });
    type("slab");
    key(search(), "Enter");
    expect(setParams(sent)).toEqual(
      [{ t: "setParam", node: "t9", name: "type", value: "IfcSlab" }]);
  });

  it("openFirstUnset: configured node or defaulted modelPick asks nothing", () => {
    pop = make(true);
    expect(pop.openFirstUnset(node("select.byType", "t1", { type: "IfcWall" }))).toBeNull();
    expect(pop.openFirstUnset(node("load.model", "l1", { model: "" }))).toBeNull();
    expect(pop.current()).toBeNull();
    expect(pickerEl().style.display).toBe("none");
  });

  it("cancel() (the app's Esc pathway, T14) closes the picker without dispatch", () => {
    pop = make(true);
    pop.open(node("select.byType", "t1", { type: "" }), "type");
    pop.cancel();
    expect(sent.filter((i) => i.k === "graph")).toEqual([]);
    expect(pop.current()).toBeNull();
  });

  it("openFirstUnset works with the latch OFF (auto-open is picker-native)", () => {
    pop = make(false);
    expect(pop.openFirstUnset(node("select.byType", "t2", { type: "" }))).toBe("type");
    expect(pickerEl().style.display).toBe("block");
  });
});
