// @vitest-environment jsdom
// T7 replacement for the retired menu-bar browser checks: Help ▸ Show all /
// Hide all wiring, the examples menu, the palette panel rows, host status.
// The doc.ts side of helpAll is proven in chips.spec.ts (headless); this suite
// owns only the DOM half — which button calls which hook.
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { KINDS } from "../../kinds";
import { createChrome, type Chrome, type ChromeHooks, type ChromeSpec } from "../chrome";

let host: HTMLElement;
let chrome: Chrome;
let hooks: { [K in keyof ChromeHooks]: ReturnType<typeof vi.fn> };
let examples: string[];
let picked: string[];

const q = <T extends HTMLElement>(sel: string) => host.querySelector<T>(sel)!;

beforeEach(() => {
  host = document.createElement("div");
  document.body.appendChild(host);
  examples = ["carbon-walls", "sql-explore"];
  picked = [];
  hooks = { addKind: vi.fn(), setHelpAll: vi.fn(), undo: vi.fn(), redo: vi.fn(), fitGraph: vi.fn(), tidyGraph: vi.fn() };
  chrome = createChrome(host, KINDS, {
    title: "PlatoFlow",
    examples,
    onExample: (n) => picked.push(n),
  }, hooks);
});

afterEach(() => { chrome.destroy(); document.body.replaceChildren(); });

describe("menu bar (retired browser checks)", () => {
  it("Help ▸ Show all / Hide all call setHelpAll(true/false) and close the menu", () => {
    q(".pf-help-trigger").click();
    expect(q(".pf-menu:has(.pf-help-trigger)").classList.contains("pf-open") ||
      host.querySelectorAll(".pf-menu.pf-open").length === 1).toBe(true);
    q<HTMLButtonElement>("[data-help='true']").click();
    expect(hooks.setHelpAll).toHaveBeenCalledWith(true);
    expect(host.querySelectorAll(".pf-menu.pf-open").length).toBe(0);
    q(".pf-help-trigger").click();
    q<HTMLButtonElement>("[data-help='false']").click();
    expect(hooks.setHelpAll).toHaveBeenCalledWith(false);
  });

  it("Examples menu lists every demo and picking one fires onExample", () => {
    q(".pf-menu-trigger").click();
    const items = [...host.querySelectorAll<HTMLButtonElement>(".pf-menu-list .pf-menu-item")]
      .map((b) => b.textContent);
    for (const d of examples) expect(items).toContain(d);
    [...host.querySelectorAll<HTMLButtonElement>(".pf-menu-item")]
      .find((b) => b.textContent === "sql-explore")!.click();
    expect(picked).toEqual(["sql-explore"]);
  });

  it("Edit ▸ Undo/Redo start disabled; syncHistory enables; clicks hit the hooks (T4)", () => {
    const undoBtn = q<HTMLButtonElement>("[data-edit='undo']");
    const redoBtn = q<HTMLButtonElement>("[data-edit='redo']");
    expect(undoBtn.disabled).toBe(true);
    expect(redoBtn.disabled).toBe(true);
    chrome.syncHistory(true, false);
    expect(undoBtn.disabled).toBe(false);
    expect(redoBtn.disabled).toBe(true);
    q(".pf-edit-trigger").click();
    undoBtn.click();
    expect(hooks.undo).toHaveBeenCalledTimes(1);
  });

  it("setStatus writes the host status text", () => {
    chrome.setStatus("ready — 3 models");
    expect(q(".pf-hoststatus").textContent).toBe("ready — 3 models");
  });
});

describe("palette panel (retired browser checks)", () => {
  it("has one row per kind; clicking a row calls addKind with that kind", () => {
    const rows = [...host.querySelectorAll<HTMLButtonElement>(".pf-kind-row")];
    expect(rows.map((r) => r.dataset.kind).sort()).toEqual(KINDS.map((k) => k.kind).sort());
    chrome.setPaletteOpen(true);
    rows.find((r) => r.dataset.kind === "view.scene")!.click();
    expect(hooks.addKind).toHaveBeenCalledWith("view.scene");
  });

  it("'p' toggles the palette (open by default), but not while typing in a DOM field", () => {
    const panel = q(".pf-palette-panel");
    expect(panel.classList.contains("pf-open")).toBe(true);   // open by default
    window.dispatchEvent(new KeyboardEvent("keydown", { key: "p" }));
    expect(panel.classList.contains("pf-open")).toBe(false);
    window.dispatchEvent(new KeyboardEvent("keydown", { key: "p" }));
    expect(panel.classList.contains("pf-open")).toBe(true);
    // typing guard: a 'p' aimed at an input must not close it
    const input = document.createElement("input");
    document.body.appendChild(input);
    input.focus();
    const ev = new KeyboardEvent("keydown", { key: "p", bubbles: true });
    input.dispatchEvent(ev);
    expect(panel.classList.contains("pf-open")).toBe(true);
  });

  it("the ✕ in the palette head closes it", () => {
    chrome.setPaletteOpen(true);
    q(".pf-pal-close").click();
    expect(q(".pf-palette-panel").classList.contains("pf-open")).toBe(false);
  });
});

// ── W13-C: palette search ────────────────────────────────────────────────────
describe("palette search (W13-C)", () => {
  const input = () => q<HTMLInputElement>(".pf-pal-search input");
  const visibleRows = () => [...host.querySelectorAll<HTMLButtonElement>(".pf-kind-row")]
    .filter((r) => r.style.display !== "none");
  const type = (text: string) => {
    input().value = text;
    input().dispatchEvent(new Event("input", { bubbles: true }));
  };
  // the module's filter rule, restated over KINDS, so expectations track the vocabulary
  const matching = (needle: string) =>
    KINDS.filter((k) => `${k.label} ${k.description}`.toLowerCase().includes(needle));

  it("filters rows live by case-insensitive substring over label + description", () => {
    type("BOUNDING");
    const vis = visibleRows().map((r) => r.dataset.kind);
    expect(vis).toContain("viz.boxes");
    expect([...vis].sort()).toEqual(matching("bounding").map((k) => k.kind).sort());
  });

  it("hides category headers whose whole group is filtered out", () => {
    type("bounding");
    const shown = [...host.querySelectorAll<HTMLElement>(".pf-cat")]
      .filter((h) => h.style.display !== "none").map((h) => h.textContent);
    const expected = [...new Set(matching("bounding").map((k) => k.category))];
    expect(shown.sort()).toEqual(expected.sort());
  });

  it("shows (no matches) when nothing survives; clearing the query restores all", () => {
    type("zzzz-nope");
    expect(visibleRows()).toEqual([]);
    expect(q(".pf-pal-empty").style.display).not.toBe("none");
    type("");
    expect(q(".pf-pal-empty").style.display).toBe("none");
    expect(visibleRows().length).toBe(KINDS.length);
  });

  it("Enter adds the top visible match and clears the input; no match = no add", () => {
    type("bounding");
    input().dispatchEvent(new KeyboardEvent("keydown", { key: "Enter", bubbles: true }));
    expect(hooks.addKind).toHaveBeenCalledWith("viz.boxes");
    expect(input().value).toBe("");
    expect(visibleRows().length).toBe(KINDS.length);   // filter reset with the input
    type("zzzz-nope");
    input().dispatchEvent(new KeyboardEvent("keydown", { key: "Enter", bubbles: true }));
    expect(hooks.addKind).toHaveBeenCalledTimes(1);
  });

  it("Escape clears a non-empty query; a second Escape blurs", () => {
    const i = input();
    i.focus();
    type("sql");
    i.dispatchEvent(new KeyboardEvent("keydown", { key: "Escape", bubbles: true }));
    expect(i.value).toBe("");
    expect(document.activeElement).toBe(i);            // first Esc only clears
    i.dispatchEvent(new KeyboardEvent("keydown", { key: "Escape", bubbles: true }));
    expect(document.activeElement).not.toBe(i);
  });

  it("typing p/f/t in the search input never triggers chrome shortcuts", () => {
    const i = input();
    i.focus();
    for (const key of ["p", "f", "t"])
      i.dispatchEvent(new KeyboardEvent("keydown", { key, bubbles: true }));
    expect(q(".pf-palette-panel").classList.contains("pf-open")).toBe(true);
    expect(hooks.fitGraph).not.toHaveBeenCalled();
    expect(hooks.tidyGraph).not.toHaveBeenCalled();
  });
});

// ── W13-C: View menu fit/tidy + F/T keys ─────────────────────────────────────
describe("View menu fit/tidy (W13-C)", () => {
  it("Fit graph and Tidy graph call their hooks and close the menu", () => {
    q(".pf-view-trigger").click();
    q<HTMLButtonElement>(".pf-view-fit").click();
    expect(hooks.fitGraph).toHaveBeenCalledTimes(1);
    expect(host.querySelectorAll(".pf-menu.pf-open").length).toBe(0);
    q(".pf-view-trigger").click();
    q<HTMLButtonElement>(".pf-view-tidy").click();
    expect(hooks.tidyGraph).toHaveBeenCalledTimes(1);
    expect(host.querySelectorAll(".pf-menu.pf-open").length).toBe(0);
  });

  it("Add Node check item still toggles the palette", () => {
    q<HTMLButtonElement>(".pf-view-addnode").click();
    expect(q(".pf-palette-panel").classList.contains("pf-open")).toBe(false);
    q<HTMLButtonElement>(".pf-view-addnode").click();
    expect(q(".pf-palette-panel").classList.contains("pf-open")).toBe(true);
  });

  it("F/T keys hit the hooks; modifiers and typing targets are ignored", () => {
    window.dispatchEvent(new KeyboardEvent("keydown", { key: "f" }));
    expect(hooks.fitGraph).toHaveBeenCalledTimes(1);
    window.dispatchEvent(new KeyboardEvent("keydown", { key: "T" }));
    expect(hooks.tidyGraph).toHaveBeenCalledTimes(1);
    window.dispatchEvent(new KeyboardEvent("keydown", { key: "f", ctrlKey: true })); // browser find
    expect(hooks.fitGraph).toHaveBeenCalledTimes(1);
    const inp = document.createElement("input");
    document.body.appendChild(inp);
    inp.focus();
    inp.dispatchEvent(new KeyboardEvent("keydown", { key: "f", bubbles: true }));
    expect(hooks.fitGraph).toHaveBeenCalledTimes(1);
  });
});

// ── W9-E: dynamic examples + save ────────────────────────────────────────────
describe("dynamic examples + Save graph (W9-E)", () => {
  const tick = () => new Promise((r) => setTimeout(r, 0));
  const dyn: Chrome[] = [];
  afterEach(() => { while (dyn.length) dyn.pop()!.destroy(); });

  const mkDyn = (over: Partial<ChromeSpec>): HTMLElement => {
    const el = document.createElement("div");
    document.body.appendChild(el);
    dyn.push(createChrome(el, KINDS, {
      examples: ["static-only"],
      onExample: (n) => picked.push(n),
      ...over,
    }, hooks));
    return el;
  };

  const listNames = (el: HTMLElement) => {
    const menu = el.querySelector(".pf-menu-trigger")!.closest(".pf-menu")!;
    return [...menu.querySelectorAll<HTMLButtonElement>(".pf-menu-item:not(.pf-save-graph)")]
      .map((b) => b.textContent);
  };

  it("rebuilds the list from getExamples on every open; saved graphs in their own section", async () => {
    let data = { demos: ["d1"], saved: [] as string[] };
    const getExamples = vi.fn(async () => data);
    const el = mkDyn({ getExamples });
    const btn = el.querySelector<HTMLButtonElement>(".pf-menu-trigger")!;

    btn.click();
    expect(el.querySelector(".pf-menu-pending")).not.toBeNull(); // pending state, synchronously
    await tick();
    expect(listNames(el)).toEqual(["d1"]);
    expect(el.querySelector(".pf-menu-sect")).toBeNull();        // nothing saved: no section

    btn.click();                                                 // close (no refetch)
    data = { demos: ["d1", "d2"], saved: ["mine"] };
    btn.click();                                                 // reopen: rebuild
    await tick();
    expect(getExamples).toHaveBeenCalledTimes(2);
    expect(listNames(el)).toEqual(["d1", "d2", "mine"]);
    expect(el.querySelector(".pf-menu-sect")!.textContent).toBe("saved");

    [...el.querySelectorAll<HTMLButtonElement>(".pf-menu-item")]
      .find((b) => b.textContent === "mine")!.click();           // saved loads like a demo
    expect(picked).toEqual(["mine"]);
  });

  it("Save graph… appears with onSaveGraph, fires the callback, closes the menu; static list intact", () => {
    const onSaveGraph = vi.fn();
    const el = mkDyn({ onSaveGraph });                           // no getExamples: static path
    el.querySelector<HTMLButtonElement>(".pf-menu-trigger")!.click();
    expect(listNames(el)).toEqual(["static-only"]);
    el.querySelector<HTMLButtonElement>(".pf-save-graph")!.click();
    expect(onSaveGraph).toHaveBeenCalledTimes(1);
    expect(el.querySelectorAll(".pf-menu.pf-open").length).toBe(0);
  });

  it("save stays clickable while a dynamic fill is still pending", () => {
    const onSaveGraph = vi.fn();
    const el = mkDyn({ onSaveGraph, getExamples: () => new Promise(() => {}) });
    el.querySelector<HTMLButtonElement>(".pf-menu-trigger")!.click();
    expect(el.querySelector(".pf-menu-pending")).not.toBeNull();
    el.querySelector<HTMLButtonElement>(".pf-save-graph")!.click();
    expect(onSaveGraph).toHaveBeenCalledTimes(1);
  });
});
