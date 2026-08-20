// @vitest-environment jsdom
// W13-D: the generic context-menu organ. Rendering (order, separators, kbd,
// danger/disabled), action + close semantics, dismissal (click-away / Esc /
// blur / re-open), viewport clamping (mocked viewport + rect — jsdom rects
// are 0×0, so the size is stubbed), keyboard navigation, and destroy hygiene.
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { createContextMenu, type ContextMenu, type MenuEntry } from "../contextmenu";

let menu: ContextMenu;

const root = () => document.querySelector<HTMLElement>(".pf-ctx")!;
const buttons = () => [...root().querySelectorAll<HTMLButtonElement>(".pf-ctx-item")];
const labels = () => buttons().map((b) => b.querySelector(".pf-ctx-label")!.textContent);

const item = (label: string, extra: Partial<Exclude<MenuEntry, "---">> = {}): MenuEntry =>
  ({ label, action: () => {}, ...extra });

const key = (k: string) =>
  window.dispatchEvent(new KeyboardEvent("keydown", { key: k, bubbles: true, cancelable: true }));
// jsdom has no PointerEvent constructor; listeners key on the event TYPE, so a
// MouseEvent-typed "pointerdown" exercises the same path the browser takes.
const pointerdown = (el: EventTarget) =>
  el.dispatchEvent(new MouseEvent("pointerdown", { bubbles: true, cancelable: true }));

beforeEach(() => {
  menu = createContextMenu();
});

afterEach(() => {
  menu.destroy();
  document.body.replaceChildren();
});

// ── rendering ────────────────────────────────────────────────────────────────

describe("rendering", () => {
  it("renders entries in order with separators between item buttons", () => {
    menu.openAt(10, 10, [item("Copy"), "---", item("Delete")]);
    const kinds = [...root().children].map((c) => c.className.split(" ")[0]);
    expect(kinds).toEqual(["pf-ctx-item", "pf-ctx-sep", "pf-ctx-item"]);
    expect(labels()).toEqual(["Copy", "Delete"]);
    expect(menu.isOpen()).toBe(true);
  });

  it("kbd hints render right of the label; absent when not given", () => {
    menu.openAt(10, 10, [item("Copy", { kbd: "Ctrl+C" }), item("Rename")]);
    const [copy, rename] = buttons();
    expect(copy.querySelector(".pf-ctx-kbd")!.textContent).toBe("Ctrl+C");
    expect(rename.querySelector(".pf-ctx-kbd")).toBeNull();
  });

  it("danger and disabled entries get their class / inert state", () => {
    menu.openAt(10, 10, [item("Delete", { danger: true }), item("Paste", { disabled: true })]);
    const [del, paste] = buttons();
    expect(del.classList.contains("pf-ctx-danger")).toBe(true);
    expect(del.disabled).toBe(false);
    expect(paste.disabled).toBe(true);
    expect(paste.classList.contains("pf-ctx-danger")).toBe(false);
  });

  it("injects exactly one id-guarded style tag across instances", () => {
    const second = createContextMenu();
    expect(document.querySelectorAll("#pf-ctx-style").length).toBe(1);
    second.destroy();
  });
});

// ── actions ──────────────────────────────────────────────────────────────────

describe("actions", () => {
  it("clicking an item runs its action then closes", () => {
    const action = vi.fn();
    menu.openAt(10, 10, [item("Copy", { action })]);
    buttons()[0].click();
    expect(action).toHaveBeenCalledTimes(1);
    expect(menu.isOpen()).toBe(false);
    expect(root().style.display).toBe("none");
  });

  it("a disabled item's action never fires — not even via a synthetic click", () => {
    const action = vi.fn();
    menu.openAt(10, 10, [item("Paste", { disabled: true, action })]);
    buttons()[0].click();
    buttons()[0].dispatchEvent(new MouseEvent("click", { bubbles: true }));
    expect(action).not.toHaveBeenCalled();
    expect(menu.isOpen()).toBe(true);          // inert: does not dismiss either
  });
});

// ── dismissal ────────────────────────────────────────────────────────────────

describe("dismissal", () => {
  beforeEach(() => menu.openAt(10, 10, [item("Copy"), item("Delete")]));

  it("pointerdown outside closes; inside does not (contains gates the host)", () => {
    expect(menu.contains(buttons()[0])).toBe(true);
    pointerdown(buttons()[0]);
    expect(menu.isOpen()).toBe(true);
    expect(menu.contains(document.body)).toBe(false);
    pointerdown(document.body);
    expect(menu.isOpen()).toBe(false);
  });

  it("Escape closes", () => {
    key("Escape");
    expect(menu.isOpen()).toBe(false);
  });

  it("window blur closes", () => {
    window.dispatchEvent(new Event("blur"));
    expect(menu.isOpen()).toBe(false);
  });

  it("a second openAt replaces the content", () => {
    menu.openAt(20, 20, [item("Rename")]);
    expect(labels()).toEqual(["Rename"]);
    expect(menu.isOpen()).toBe(true);
  });

  it("right-click on the menu itself is suppressed (no native menu stacking)", () => {
    const ev = new MouseEvent("contextmenu", { bubbles: true, cancelable: true });
    buttons()[0].dispatchEvent(ev);
    expect(ev.defaultPrevented).toBe(true);
  });
});

// ── clamping ─────────────────────────────────────────────────────────────────

describe("clamping", () => {
  const W = 500, H = 300, MW = 120, MH = 80;

  beforeEach(() => {
    Object.defineProperty(window, "innerWidth", { value: W, configurable: true, writable: true });
    Object.defineProperty(window, "innerHeight", { value: H, configurable: true, writable: true });
    // jsdom computes no layout — stub the menu's measured size so the clamp
    // math has something real to work with.
    root().getBoundingClientRect = () =>
      ({ width: MW, height: MH, left: 0, top: 0, right: MW, bottom: MH, x: 0, y: 0,
         toJSON: () => ({}) }) as DOMRect;
  });

  const rect = () => {
    const left = parseFloat(root().style.left);
    const top = parseFloat(root().style.top);
    return { left, top, right: left + MW, bottom: top + MH };
  };

  it("keeps the requested position when the menu fits", () => {
    menu.openAt(40, 60, [item("Copy")]);
    expect(rect()).toMatchObject({ left: 40, top: 60 });
  });

  it("shifts fully inside when opened past the right/bottom edges", () => {
    menu.openAt(W - 5, H - 5, [item("Copy")]);
    const r = rect();
    expect(r.right).toBeLessThanOrEqual(W);
    expect(r.bottom).toBeLessThanOrEqual(H);
    expect(r.left).toBeGreaterThanOrEqual(0);
    expect(r.top).toBeGreaterThanOrEqual(0);
  });

  it("never goes negative even if the menu is larger than the viewport slot", () => {
    menu.openAt(-30, -30, [item("Copy")]);
    const r = rect();
    expect(r.left).toBeGreaterThanOrEqual(0);
    expect(r.top).toBeGreaterThanOrEqual(0);
  });
});

// ── keyboard ─────────────────────────────────────────────────────────────────

describe("keyboard", () => {
  const active = () => root().querySelector<HTMLElement>(".pf-ctx-active");

  it("opening sets no active item; Enter before any arrow does nothing", () => {
    const action = vi.fn();
    menu.openAt(10, 10, [item("Copy", { action })]);
    expect(active()).toBeNull();
    key("Enter");
    expect(action).not.toHaveBeenCalled();
    expect(menu.isOpen()).toBe(true);
  });

  it("ArrowDown highlights the first ENABLED item, skipping separator + disabled", () => {
    menu.openAt(10, 10, ["---", item("Paste", { disabled: true }), item("Copy"), item("Del")]);
    key("ArrowDown");
    expect(active()!.querySelector(".pf-ctx-label")!.textContent).toBe("Copy");
  });

  it("Enter fires the highlighted item and closes", () => {
    const action = vi.fn();
    menu.openAt(10, 10, [item("Paste", { disabled: true }), item("Copy", { action })]);
    key("ArrowDown");
    key("Enter");
    expect(action).toHaveBeenCalledTimes(1);
    expect(menu.isOpen()).toBe(false);
  });

  it("navigation wraps and ArrowUp from no-active lands on the LAST enabled item", () => {
    menu.openAt(10, 10, [item("A"), item("B", { disabled: true }), item("C")]);
    key("ArrowUp");                            // -1 → last enabled (C)
    expect(active()!.querySelector(".pf-ctx-label")!.textContent).toBe("C");
    key("ArrowDown");                          // wraps past disabled B → A
    expect(active()!.querySelector(".pf-ctx-label")!.textContent).toBe("A");
    key("ArrowUp");                            // back around → C
    expect(active()!.querySelector(".pf-ctx-label")!.textContent).toBe("C");
  });
});

// ── destroy ──────────────────────────────────────────────────────────────────

describe("destroy", () => {
  it("removes the root; later window events neither throw nor resurrect it", () => {
    menu.openAt(10, 10, [item("Copy")]);
    menu.destroy();
    expect(document.querySelector(".pf-ctx")).toBeNull();
    expect(() => {
      key("Escape");
      key("ArrowDown");
      pointerdown(document.body);
      window.dispatchEvent(new Event("blur"));
    }).not.toThrow();
    expect(menu.isOpen()).toBe(false);
    menu.destroy();                            // idempotent (afterEach calls it again)
  });
});
