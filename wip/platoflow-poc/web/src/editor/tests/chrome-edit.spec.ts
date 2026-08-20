// @vitest-environment jsdom
// T4 Edit menu: enable/disable tracks history depth via Chrome.syncHistory;
// items dispatch through ChromeHooks. jsdom because chrome is plain DOM
// (NOTES wave-5: DOM organs don't mount headless).
import { afterEach, describe, expect, it } from "vitest";
import { createChrome, type Chrome } from "../chrome";

const make = () => {
  const calls: string[] = [];
  const chrome = createChrome(document.body, [], { examples: [], onExample: () => {} }, {
    addKind: () => calls.push("addKind"),
    setHelpAll: () => calls.push("setHelpAll"),
    undo: () => calls.push("undo"),
    redo: () => calls.push("redo"),
    fitGraph: () => calls.push("fitGraph"),
    tidyGraph: () => calls.push("tidyGraph"),
  });
  return { chrome, calls };
};

const item = (which: string): HTMLButtonElement => {
  const el = document.querySelector(`[data-edit="${which}"]`);
  if (!(el instanceof HTMLButtonElement)) throw new Error(`no ${which} item`);
  return el;
};

let active: Chrome | null = null;
afterEach(() => { active?.destroy(); active = null; document.body.innerHTML = ""; });

describe("Edit menu", () => {
  it("Undo and Redo start disabled on a fresh doc", () => {
    ({ chrome: active } = make());
    expect(item("undo").disabled).toBe(true);
    expect(item("redo").disabled).toBe(true);
  });

  it("syncHistory drives enabled state independently per direction", () => {
    ({ chrome: active } = make());
    active.syncHistory(true, false);
    expect(item("undo").disabled).toBe(false);
    expect(item("redo").disabled).toBe(true);
    active.syncHistory(false, true);
    expect(item("undo").disabled).toBe(true);
    expect(item("redo").disabled).toBe(false);
  });

  it("clicking an enabled item calls the hook and closes the menu", () => {
    const { chrome, calls } = make();
    active = chrome;
    chrome.syncHistory(true, true);
    const menu = item("undo").closest(".pf-menu")!;
    (menu.querySelector(".pf-edit-trigger") as HTMLButtonElement).click();
    expect(menu.classList.contains("pf-open")).toBe(true);
    item("undo").click();
    expect(calls).toEqual(["undo"]);
    expect(menu.classList.contains("pf-open")).toBe(false);
    item("redo").click();
    expect(calls).toEqual(["undo", "redo"]);
  });

  it("a disabled item does not dispatch", () => {
    const { chrome, calls } = make();
    active = chrome;
    chrome.syncHistory(false, false);
    item("undo").click();                        // jsdom honors disabled: no event
    expect(calls).toEqual([]);
  });
});
