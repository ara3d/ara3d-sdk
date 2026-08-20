// @vitest-environment jsdom
// W13-C toast stack: render, tones, max-3 eviction, click-dismiss, timed
// auto-dismiss (fake timers), clearToasts. The module is standalone — no
// chrome/editor mounting needed.
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { clearToasts, showToast } from "../toast";

const toasts = () => [...document.querySelectorAll<HTMLElement>(".pf-toast")];
const texts = () => toasts().map((t) => t.querySelector(".pf-toast-msg")!.textContent);

beforeEach(() => { vi.useFakeTimers(); });
afterEach(() => {
  clearToasts();
  vi.useRealTimers();
  document.body.replaceChildren();
});

describe("toast stack", () => {
  it("renders the message in a fixed stack, newest at the bottom", () => {
    showToast("first");
    showToast("second");
    expect(texts()).toEqual(["first", "second"]);
    const stack = document.getElementById("pf-toast-stack")!;
    expect(stack.contains(toasts()[0])).toBe(true);
    expect(document.getElementById("pf-toast-style")).not.toBeNull();
  });

  it("carries a tone class; info is the default", () => {
    showToast("a");
    showToast("b", "ok");
    showToast("c", "error");
    expect(toasts().map((t) => t.className)).toEqual([
      "pf-toast pf-toast-info",
      "pf-toast pf-toast-ok",
      "pf-toast pf-toast-error",
    ]);
  });

  it("keeps at most 3 toasts, evicting the oldest", () => {
    for (const m of ["one", "two", "three", "four"]) showToast(m);
    expect(texts()).toEqual(["two", "three", "four"]);
  });

  it("click dismisses that toast only", () => {
    showToast("stay");
    showToast("go");
    toasts()[1].click();
    expect(texts()).toEqual(["stay"]);
  });

  it("auto-dismisses: info/ok ~4s, error ~7s, ms overrides", () => {
    showToast("info-toast");
    showToast("err-toast", "error");
    showToast("quick", "info", 100);
    vi.advanceTimersByTime(100);
    expect(texts()).toEqual(["info-toast", "err-toast"]);
    vi.advanceTimersByTime(3900);                    // t=4000: info gone
    expect(texts()).toEqual(["err-toast"]);
    vi.advanceTimersByTime(2999);
    expect(texts()).toEqual(["err-toast"]);
    vi.advanceTimersByTime(1);                       // t=7000: error gone
    expect(texts()).toEqual([]);
  });

  it("an evicted toast's timer never resurrects anything", () => {
    for (const m of ["one", "two", "three", "four"]) showToast(m);
    vi.runAllTimers();                               // includes "one"'s orphaned timer
    expect(texts()).toEqual([]);
  });

  it("clearToasts drops everything and cancels timers", () => {
    showToast("a");
    showToast("b", "error");
    clearToasts();
    expect(texts()).toEqual([]);
    expect(vi.getTimerCount()).toBe(0);
  });
});
