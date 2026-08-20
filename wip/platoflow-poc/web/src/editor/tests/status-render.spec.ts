// W9-C specs: needs-setup renders neutral (gray, message, never red) and a
// status warning shows as an amber "⚠ "-prefixed footer line + the full text
// in the help expando (pinning the geom.helpLines delegation). Pure helpers
// (footerText/footerTone — the badgeText/badgeTone pattern) + the headless
// drive for the status → doc.status → layout path.
import { describe, expect, it } from "vitest";
import type { GraphDoc, NodeStatus } from "../../contracts";
import { kindInfo } from "../../kinds";
import { footerText, footerTone, STATUS_COLOR, WARN_AMBER } from "../cards";
import { HELP_LINE_H, HELP_PAD, helpLines } from "../geom";
import { createDrive, type Drive } from "./drive";

const DIM = { r: 120, g: 124, b: 132, a: 1 };   // stand-in for the theme's textDim

const NEEDS_SETUP: NodeStatus = { state: "needs-setup", message: "choose a type" };
const OK_WARN: NodeStatus = {
  state: "ok", summary: "170 rows",
  warning: "42 rows dropped as non-numeric",
};

// ── pure: footer text ────────────────────────────────────────────────────────

describe("footerText", () => {
  it("needs-setup shows its message (what setup is needed), not an error look", () => {
    expect(footerText(NEEDS_SETUP)).toBe("choose a type");
    expect(footerText({ state: "needs-setup" })).toBe("needs setup");   // fallback
  });

  it("error keeps its message (unchanged behavior)", () => {
    expect(footerText({ state: "error", message: "boom" })).toBe("boom");
    expect(footerText({ state: "error" })).toBe("error");
  });

  it("ok shows the summary; a warning prefixes it with ⚠", () => {
    expect(footerText({ state: "ok", summary: "170 rows" })).toBe("170 rows");
    expect(footerText(OK_WARN)).toBe("⚠ 170 rows");
  });

  it("an error's warning never rewrites the error text", () => {
    expect(footerText({ state: "error", message: "boom", warning: "w" })).toBe("boom");
  });

  it("no status → no text", () => {
    expect(footerText(undefined)).toBe("");
  });
});

// ── pure: footer tone ────────────────────────────────────────────────────────

describe("footerTone", () => {
  it("needs-setup is the neutral gray entry — near-equal channels, not red", () => {
    const t = footerTone(NEEDS_SETUP, DIM);
    expect(t).toEqual(STATUS_COLOR["needs-setup"]);
    const spread = Math.max(t.r, t.g, t.b) - Math.min(t.r, t.g, t.b);
    expect(spread).toBeLessThan(30);                       // gray, no hue shouting
    expect(t).not.toEqual(STATUS_COLOR.error);
  });

  it("a warning shifts a non-error tone toward amber", () => {
    const t = footerTone(OK_WARN, DIM);
    expect(t).not.toEqual(STATUS_COLOR.ok);
    // amber pull: more red, less blue than the plain ok green
    expect(t.r).toBeGreaterThan(STATUS_COLOR.ok.r);
    expect(t.b).toBeLessThan(STATUS_COLOR.ok.b);
    // subtle blend, not a replacement — still short of full amber
    expect(t.r).toBeLessThan(WARN_AMBER.r);
  });

  it("errors keep their red even with a warning attached", () => {
    expect(footerTone({ state: "error", message: "boom", warning: "w" }, DIM))
      .toEqual(STATUS_COLOR.error);
  });

  it("no status falls back to the caller's dim tone", () => {
    expect(footerTone(undefined, DIM)).toEqual(DIM);
  });
});

// ── pure: help expando gets the full warning (geom.helpLines delegation) ─────

describe("helpLines warning delegation", () => {
  const info = kindInfo("select.byType")!;

  it("a warning folds in as amber lines: blank spacer + ⚠-prefixed text", () => {
    const lines = helpLines(info, OK_WARN);
    const warn = lines.filter((l) => l.tone === "warn");
    expect(warn.length).toBeGreaterThanOrEqual(2);         // spacer + ≥1 text line
    expect(warn[0].text).toBe("");
    const joined = warn.map((l) => l.text).join(" ").trim();
    expect(joined).toContain("⚠");
    expect(joined).toContain("42 rows dropped as non-numeric");
  });

  it("warning sits between detail and error in the expando order", () => {
    const lines = helpLines(info, {
      state: "error", message: "boom", detail: "SELECT 1", warning: "w1",
    });
    const tones = lines.map((l) => l.tone);
    expect(tones.lastIndexOf("detail")).toBeLessThan(tones.indexOf("warn"));
    expect(tones.lastIndexOf("warn")).toBeLessThan(tones.indexOf("error"));
  });

  it("statuses without a warning produce zero warn lines (golden-suite guard)", () => {
    for (const status of [undefined, { state: "ok", summary: "s" } as NodeStatus,
      { state: "error", message: "boom" } as NodeStatus, NEEDS_SETUP]) {
      expect(helpLines(info, status).every((l) => l.tone !== "warn")).toBe(true);
    }
  });
});

// ── headless drive: status dispatch → doc.status → layout ────────────────────

const DOC: GraphDoc = {
  name: "status-fixture",
  nodes: [
    { id: "n1", kind: "select.byType", params: { type: "IfcWall" }, x: 60, y: 80 },
  ],
  edges: [],
  display: null,
};

const fresh = (): Drive => {
  const d = createDrive();
  d.load(DOC);
  d.settle();
  return d;
};

describe("status → layout through the editor's results path", () => {
  it("open help + warning status: the layout's helpLines carry the warning", () => {
    const d = fresh();
    d.dispatch({ k: "status", status: { n1: OK_WARN } });
    d.dispatch({ k: "toggleHelp", node: "n1" });
    d.settle();
    const { l } = d.layoutOf("n1");
    const warn = l.helpLines.filter((x) => x.tone === "warn");
    expect(warn.length).toBeGreaterThanOrEqual(2);
    expect(warn.map((x) => x.text).join(" ")).toContain("42 rows dropped as non-numeric");
    expect(l.helpH).toBe(l.helpLines.length * HELP_LINE_H + 2 * HELP_PAD);
  });

  it("same node, same status minus the warning: helpLines identical minus warn", () => {
    const d = fresh();
    d.dispatch({ k: "status", status: { n1: { state: "ok", summary: "170 rows" } } });
    d.dispatch({ k: "toggleHelp", node: "n1" });
    d.settle();
    const plain = d.layoutOf("n1").l.helpLines;
    d.dispatch({ k: "status", status: { n1: OK_WARN } });
    const withWarn = d.layoutOf("n1").l.helpLines;
    expect(plain.every((x) => x.tone !== "warn")).toBe(true);
    expect(withWarn.filter((x) => x.tone !== "warn")).toEqual(plain);
  });

  it("needs-setup status renders (layout + render pass) without exploding", () => {
    const d = fresh();
    d.dispatch({ k: "status", status: { n1: NEEDS_SETUP } });
    d.settle();                                    // render pass runs headless
    expect(d.doc().status.n1.state).toBe("needs-setup");
  });
});
