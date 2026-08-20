// W13-B calm-pass specs. The complaint: "a lot going on in the UI". The cure:
// a resting card is title + category strip + ports + param rows + footer —
// chips (? / ✕ / ▶ Run) fade in only on engagement (hover/selection), the eye
// chip is gone entirely (selection drives the 3D view; GraphDoc.display stays
// in the schema as the MCP pinned fallback), and wire badges show only on
// hover unless their tone is error.
//
// Driven headless through drive.ts: hover/selection go through the real
// pointer pipeline, chip presence is read from live adorn instances
// (rectOfKey), and wire hover is read from the wire instance's own channel.
import { describe, expect, it } from "vitest";
import { v, walk, type Instance } from "gratify";
import type { GraphDoc, NodeStatus } from "../../contracts";
import { CHIP_FADE_MIN, CHIP_PRESS_MIN, chipAlpha } from "../cards";
import { BADGE_FADE_MIN, badgeAlpha, badgeTone } from "../wires";
import { createDrive, type Drive } from "./drive";

const ok = (summary: string): NodeStatus => ({ state: "ok", summary });

const DOC: GraphDoc = {
  name: "calm-fixture",
  nodes: [
    { id: "n1", kind: "load.model", params: { model: "duplex" }, x: 60, y: 80 },
    { id: "n2", kind: "select.byType", params: { type: "IfcWall" }, x: 560, y: 80 },
    { id: "w1", kind: "sink.writePset", params: {}, x: 60, y: 420 },
  ],
  edges: [{ from: { node: "n1", slot: "out" }, to: { node: "n2", slot: "in" } }],
  display: null,
};

const fresh = (): Drive => {
  const d = createDrive();
  d.load(DOC);
  d.dispatch({ k: "status", status: { n1: ok("11322 entities"), n2: ok("216 entities") } });
  d.settle();
  return d;
};

/** Park the pointer on the node's header until the fade-in completes. */
const hoverNode = (d: Drive, id: string) => {
  d.rt.pointerMove(d.headerCenter(id));
  d.step(40);
};

/** Park the pointer far from everything until fades decay and ghosts prune. */
const rest = (d: Drive) => {
  d.rt.pointerMove(v(1100, 750));
  d.settle();
};

/** Channels of a world instance by key (wire parts carry the edge key). */
const chOf = (d: Drive, key: string): Record<string, number> | null => {
  let found: Record<string, number> | null = null;
  walk(d.rt.root, (i: Instance) => { if (found === null && i.key === key) found = i.ch; });
  return found;
};

// ── the eye is gone ──────────────────────────────────────────────────────────

describe("eye chip removal", () => {
  it("no card adorns an eye — even engaged, even for display-capable kinds", () => {
    const d = fresh();
    hoverNode(d, "n1");                            // load.model outputs a scene
    expect(d.rectOfKey("n1::help")).not.toBeNull();   // engagement worked …
    expect(d.rectOfKey("n1::eye")).toBeNull();        // … but there is no eye
    hoverNode(d, "n2");
    expect(d.rectOfKey("n2::eye")).toBeNull();
  });

  it("clicking where the eye used to sit does not change graph.display", () => {
    const d = fresh();
    hoverNode(d, "n1");
    // pre-W13 spot: footer, right side — now just card surface (select)
    const { l, origin } = d.layoutOf("n1");
    d.click(v(origin.x + l.w - 30, origin.y + l.footerTop + 8));
    expect(d.graph().display).toBeNull();
  });
});

// ── chips only on engagement ─────────────────────────────────────────────────

describe("adorn chips fade with engagement", () => {
  it("at rest no chips render (help / del / run all absent)", () => {
    const d = fresh();
    rest(d);
    expect(d.rectOfKey("n1::help")).toBeNull();
    expect(d.rectOfKey("n1::del")).toBeNull();
    expect(d.rectOfKey("w1::run")).toBeNull();     // write-effect kind too
  });

  it("hover fades them in (help + del; run on the write-effect card)", () => {
    const d = fresh();
    hoverNode(d, "n1");
    expect(d.rectOfKey("n1::help")).not.toBeNull();
    expect(d.rectOfKey("n1::del")).not.toBeNull();
    hoverNode(d, "w1");
    expect(d.rectOfKey("w1::run")).not.toBeNull();
    // the un-hovered card's chips are gone again after the fade-out
    d.settle();
    expect(d.rectOfKey("n1::help")).toBeNull();
  });

  it("selection keeps chips visible after the pointer leaves", () => {
    const d = fresh();
    d.click(d.headerCenter("n2"));                 // select via the real press
    expect(d.doc().sel).toEqual({ kind: "node", id: "n2" });
    rest(d);                                       // hover fully decayed
    expect(d.rectOfKey("n2::help")).not.toBeNull();
    expect(d.rectOfKey("n2::del")).not.toBeNull();
  });

  it("a faded-out chip cannot be clicked (no phantom delete)", () => {
    const d = fresh();
    hoverNode(d, "n1");
    const delRect = d.rectOfKey("n1::del")!;
    const target = v(delRect.center.x, delRect.center.y);
    rest(d);                                       // chip fully faded + pruned
    d.click(target);                               // lands on the bare card
    expect(d.graph().nodes.map((n) => n.id)).toEqual(["n1", "n2", "w1"]);
    expect(d.doc().helpOpen.n1).toBeFalsy();
  });

  it("pure: chipAlpha is the strongest engagement channel; thresholds sane", () => {
    expect(chipAlpha({})).toBe(0);
    expect(chipAlpha({ over: 0.3 })).toBeCloseTo(0.3);
    expect(chipAlpha({ over: 0.2, sel: 0.9 })).toBeCloseTo(0.9);
    expect(chipAlpha({ focus: 1 })).toBe(1);       // keyboard users see chips too
    expect(chipAlpha({ drag: 2 })).toBe(1);        // clamped
    // ch.hover is deliberately IGNORED: an interactive chip steals hover from
    // its host card, so gating on it would flicker (see cards.chipAlpha).
    expect(chipAlpha({ hover: 1 })).toBe(0);
    expect(CHIP_FADE_MIN).toBeGreaterThan(0);
    expect(CHIP_FADE_MIN).toBeLessThan(CHIP_PRESS_MIN);
    expect(CHIP_PRESS_MIN).toBeLessThanOrEqual(1);
  });
});

// ── wire badges: hover-only, error always ────────────────────────────────────

describe("wire badges on demand", () => {
  it("pure: hidden at rest, follows hover, error pinned at full", () => {
    expect(badgeAlpha("ok", 0)).toBe(0);
    expect(badgeAlpha("dim", 0)).toBe(0);
    expect(badgeAlpha("ok", 0.7)).toBeCloseTo(0.7);
    expect(badgeAlpha("ok", 3)).toBe(1);           // clamped
    expect(badgeAlpha("error", 0)).toBe(1);        // error ignores hover entirely
    expect(badgeAlpha("error", 0.2)).toBe(1);
    expect(BADGE_FADE_MIN).toBeGreaterThan(0);
    expect(BADGE_FADE_MIN).toBeLessThan(1);
  });

  it("headless: the wire's hover channel gates its badge", () => {
    const d = fresh();
    rest(d);
    const ch0 = chOf(d, "n1|out>n2|in")!;
    expect(ch0).not.toBeNull();
    expect(badgeAlpha("ok", ch0.hover || 0)).toBeLessThan(BADGE_FADE_MIN);  // hidden at rest
    // hover the wire midpoint (horizontal-tangent cubic: t=0.5 IS the midpoint)
    const a = d.socketCenter({ node: "n1", dir: "out", slot: "out" });
    const b = d.socketCenter({ node: "n2", dir: "in", slot: "in" });
    d.rt.pointerMove(v((a.x + b.x) / 2, (a.y + b.y) / 2));
    d.step(40);
    const ch1 = chOf(d, "n1|out>n2|in")!;
    expect(badgeAlpha("ok", ch1.hover || 0)).toBeGreaterThan(0.5);          // shown on hover
  });

  it("an error badge shows regardless of hover state", () => {
    const d = fresh();
    d.dispatch({ k: "status", status: { n1: { state: "error", message: "boom", summary: "boom" } } });
    rest(d);
    const ch = chOf(d, "n1|out>n2|in")!;
    expect(badgeAlpha(badgeTone(d.doc().status.n1), ch.hover || 0)).toBe(1);
  });
});
