// T11 specs: wire badges (pure text/tone/zoom helpers) + the hover → ghost-
// highlight seam (headless drive: pointer over a wire midpoint via anchor
// positions) + proof the badge changed nothing about splice hit-testing.
// Coordinates via drive.ts socketCenter — no magic numbers.
import { afterEach, beforeEach, describe, expect, it } from "vitest";
import { v, type Vec } from "gratify";
import type { GraphDoc, NodeStatus } from "../../contracts";
import {
  BADGE_MAX_CHARS, BADGE_MIN_WIRE_LEN, BADGE_MIN_ZOOM, badgeFits, badgeText, badgeTone,
  badgeVisible, hoveredWireSource, onWireHover, sourceOfAnchor, spliceTarget,
} from "../wires";
import { anchorId } from "../geom";
import { createDrive, type Drive } from "./drive";

const ok = (summary: string): NodeStatus => ({ state: "ok", summary });

// ── pure: badge text per wire type (source status.summary, hard-truncated) ───

describe("badgeText", () => {
  it("scene wire: source's entity-count summary", () => {
    expect(badgeText(ok("216 entities"))).toBe("216 entities");
  });

  it("table wire: rows×cols summary", () => {
    expect(badgeText(ok("14 rows × 3 cols"))).toBe("14 rows × 3 cols");
  });

  it("colormap/view wire: domain summary", () => {
    expect(badgeText(ok("221–412 viridis"))).toBe("221–412 viridis");
  });

  it("no status or no summary → no badge", () => {
    expect(badgeText(undefined)).toBeNull();
    expect(badgeText({ state: "ok" })).toBeNull();
    expect(badgeText({ state: "pending" })).toBeNull();
  });

  it("hard-truncates past the cap with an ellipsis", () => {
    const long = "x".repeat(BADGE_MAX_CHARS + 20);
    const t = badgeText(ok(long))!;
    expect(t.length).toBe(BADGE_MAX_CHARS);
    expect(t.endsWith("…")).toBe(true);
    // at the cap exactly: untouched
    const cap = "y".repeat(BADGE_MAX_CHARS);
    expect(badgeText(ok(cap))).toBe(cap);
  });
});

// ── pure: zoom gating ────────────────────────────────────────────────────────

describe("badge zoom gating", () => {
  it("hidden below the threshold, shown at and above it", () => {
    expect(badgeVisible(BADGE_MIN_ZOOM - 0.01)).toBe(false);
    expect(badgeVisible(BADGE_MIN_ZOOM)).toBe(true);
    expect(badgeVisible(1)).toBe(true);
    expect(badgeVisible(0.25)).toBe(false);
  });
});

// ── pure: tone (dim on pending, red tint on error) ───────────────────────────

describe("badgeTone", () => {
  it("ok → ok, pending → dim, error → error; no status dims", () => {
    expect(badgeTone({ state: "ok", summary: "s" })).toBe("ok");
    expect(badgeTone({ state: "pending", summary: "s" })).toBe("dim");
    expect(badgeTone({ state: "error", message: "boom", summary: "s" })).toBe("error");
    expect(badgeTone(undefined)).toBe("dim");
  });
});

// ── pure: short-wire suppression (W9-C — the T17 "badge on sockets" wart) ────

describe("badge short-wire suppression", () => {
  it("badgeFits: boundary at BADGE_MIN_WIRE_LEN, euclidean not per-axis", () => {
    expect(badgeFits(v(0, 0), v(BADGE_MIN_WIRE_LEN - 1, 0))).toBe(false);
    expect(badgeFits(v(0, 0), v(BADGE_MIN_WIRE_LEN, 0))).toBe(true);
    const d = BADGE_MIN_WIRE_LEN / Math.SQRT2;
    expect(badgeFits(v(0, 0), v(d - 1, d - 1))).toBe(false);
    expect(badgeFits(v(0, 0), v(d + 1, d + 1))).toBe(true);
  });

  it("the fixture's wires clear the threshold (their badges stay)", () => {
    const d = fresh();
    expect(badgeFits(
      d.socketCenter({ node: "n1", dir: "out", slot: "out" }),
      d.socketCenter({ node: "n2", dir: "in", slot: "in" }))).toBe(true);
    expect(badgeFits(
      d.socketCenter({ node: "n3", dir: "out", slot: "out" }),
      d.socketCenter({ node: "n4", dir: "in", slot: "in" }))).toBe(true);
  });

  it("a wire between adjacent cards is short → badge hidden", () => {
    const d = createDrive();
    d.load({
      name: "short-wire",
      nodes: [
        { id: "s1", kind: "load.model", params: { model: "duplex" }, x: 60, y: 80 },
        { id: "s2", kind: "select.byType", params: { type: "IfcWall" }, x: 320, y: 80 },
      ],
      edges: [{ from: { node: "s1", slot: "out" }, to: { node: "s2", slot: "in" } }],
      display: null,
    });
    d.dispatch({ k: "status", status: { s1: ok("11322 entities") } });
    d.settle();
    // out socket sits at s1.x + card width (188) = 248; in socket at 320 → 72px
    expect(badgeFits(
      d.socketCenter({ node: "s1", dir: "out", slot: "out" }),
      d.socketCenter({ node: "s2", dir: "in", slot: "in" }))).toBe(false);
  });
});

describe("sourceOfAnchor", () => {
  it("recovers the node id from a geom anchor id", () => {
    expect(sourceOfAnchor(anchorId("n7", "out", "out"))).toBe("n7");
  });
});

// ── headless: hover seam ─────────────────────────────────────────────────────

// scene wire n1→n2, table wire n3→n4 (fixture mirrors linkdrag.spec).
const DOC: GraphDoc = {
  name: "badges-fixture",
  nodes: [
    { id: "n1", kind: "load.model", params: { model: "duplex" }, x: 60, y: 80 },
    { id: "n2", kind: "select.byType", params: { type: "IfcWall" }, x: 560, y: 80 },
    { id: "n3", kind: "data.csv", params: { url: "" }, x: 60, y: 420 },
    { id: "n4", kind: "table.filter", params: {}, x: 560, y: 420 },
  ],
  edges: [
    { from: { node: "n1", slot: "out" }, to: { node: "n2", slot: "in" } },
    { from: { node: "n3", slot: "out" }, to: { node: "n4", slot: "in" } },
  ],
  display: null,
};

const mid = (d: Drive, a: Vec, b: Vec): Vec => v((a.x + b.x) / 2, (a.y + b.y) / 2);

const sceneWireMid = (d: Drive): Vec =>
  mid(d,
    d.socketCenter({ node: "n1", dir: "out", slot: "out" }),
    d.socketCenter({ node: "n2", dir: "in", slot: "in" }));

const tableWireMid = (d: Drive): Vec =>
  mid(d,
    d.socketCenter({ node: "n3", dir: "out", slot: "out" }),
    d.socketCenter({ node: "n4", dir: "in", slot: "in" }));

/** Empty canvas far from both wires. */
const AWAY = v(900, 700);

const fresh = (): Drive => {
  const d = createDrive();
  d.load(DOC);
  d.dispatch({
    k: "status",
    status: { n1: ok("11322 entities"), n2: ok("216 entities"), n3: ok("14 rows × 3 cols") },
  });
  d.settle();
  return d;
};

/** Hover transitions ride the animated hover channel (approach rate 16), so
 *  give it a few frames to cross the threshold after each pointer move. */
const hoverAt = (d: Drive, p: Vec) => { d.rt.pointerMove(p); d.step(20); };

describe("hover → ghost-highlight seam", () => {
  let d: Drive;
  let fired: (string | null)[];
  let unsub: () => void;

  beforeEach(() => {
    d = fresh();
    fired = [];
    // subscribe AFTER settle so enter animations don't pollute the log
    unsub = onWireHover((id) => fired.push(id));
    hoverAt(d, AWAY);                              // known clean baseline
    fired = [];
  });

  afterEach(() => unsub());

  it("hovering the scene wire's midpoint fires with the SOURCE node id", () => {
    hoverAt(d, sceneWireMid(d));
    expect(fired).toEqual(["n1"]);
    expect(hoveredWireSource()).toBe("n1");
  });

  it("hover-out clears (fires null exactly once)", () => {
    hoverAt(d, sceneWireMid(d));
    hoverAt(d, AWAY);
    expect(fired).toEqual(["n1", null]);
    expect(hoveredWireSource()).toBeNull();
  });

  it("a table wire never fires the seam (scene wires only)", () => {
    hoverAt(d, tableWireMid(d));
    expect(fired).toEqual([]);
    expect(hoveredWireSource()).toBeNull();
  });

  it("deleting the hovered wire releases the highlight via registerScene", () => {
    hoverAt(d, sceneWireMid(d));
    expect(hoveredWireSource()).toBe("n1");
    d.dispatch({ k: "graph", intent: { t: "disconnect", to: { node: "n2", slot: "in" } } });
    d.settle();
    expect(hoveredWireSource()).toBeNull();
    expect(fired).toEqual(["n1", null]);
  });
});

// ── splice unaffected: badges are visual only ────────────────────────────────

describe("badge does not break splice hit-testing", () => {
  it("dragging a compatible node over a badged wire still arms + splices", () => {
    const d = fresh();                             // statuses set → badge renders
    d.dispatch({ k: "addKind", kind: "view.scene", at: v(300, 600) });
    d.settle();
    const vs = d.graph().nodes.find((n) => n.kind === "view.scene")!;
    const a = d.headerCenter(vs.id), m = sceneWireMid(d);
    d.rt.pointerDown(a);
    for (let i = 1; i <= 4; i++) {
      d.rt.pointerMove(v(a.x + ((m.x - a.x) * i) / 4, a.y + ((m.y - a.y) * i) / 4));
      d.step(1);
    }
    expect(spliceTarget()).toBe("n1|out>n2|in");   // corridor unchanged mid-drag
    d.rt.pointerUp(m);
    d.step(2);
    expect(d.graph().edges).toContainEqual(
      { from: { node: "n1", slot: "out" }, to: { node: vs.id, slot: "in" } });
    expect(d.graph().edges).toContainEqual(
      { from: { node: vs.id, slot: "out" }, to: { node: "n2", slot: "in" } });
  });

  it("clicking the wire midpoint (through the badge) still selects the wire", () => {
    const d = fresh();
    d.click(sceneWireMid(d));
    expect(d.doc().sel).toEqual({ kind: "edge", key: "n1|out>n2|in" });
  });
});
