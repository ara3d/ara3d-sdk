// @vitest-environment jsdom
// T7 replacement for the retired edgate popover checks (CONTRACTS.md retirement
// list): the param popover organ, mounted in jsdom against a stub dispatch,
// exercised with real DOM events. One suite owns the popover's FIELD behavior;
// the close-rule wiring (Esc / click-away / Backspace guard) lives in
// editor-dom.spec.ts because index.ts owns those listeners.
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import type { GraphNode, NodeKindInfo, NodeStatus } from "../../contracts";
import { KINDS, kindInfo } from "../../kinds";
import type { EditorIntent } from "../doc";
import { createFocusEditor, schemaGroups } from "../focus";
import { commitPolicy, createParamPopover, type ParamPopover } from "../params";

const node = (kind: string, id: string, params: Record<string, unknown> = {}): GraphNode =>
  ({ id, kind, params, x: 0, y: 0 });

let dispatched: EditorIntent[] = [];
let pop: ParamPopover;

const setParams = () =>
  dispatched.filter((i) => i.k === "graph" && i.intent.t === "setParam")
    .map((i) => (i.k === "graph" && i.intent.t === "setParam" ? i.intent : null)!);

const root = () => document.querySelector<HTMLElement>(".pf-popover")!;

beforeEach(() => {
  dispatched = [];
  pop = createParamPopover({
    dispatch: (i) => dispatched.push(i),
    kindInfo,
    getEnumOptions: (_n, _p, source) =>
      source === "models" ? ["duplex", "rac_basic", "snowdon"] :
      source === "types" ? ["IfcWall", "IfcDoor"] : [],
  });
});

afterEach(() => { pop.destroy(); document.body.replaceChildren(); });

describe("param popover: field per schema kind (retired edgate checks)", () => {
  it("modelPick opens a select labelled with the param name, options from the source", () => {
    pop.open(node("load.model", "load1", { model: "duplex" }), "model");
    expect(root().style.display).toBe("block");
    expect(root().querySelector("label")?.textContent?.trim()).toBe("model");
    const sel = root().querySelector("select")!;
    expect([...sel.options].map((o) => o.value)).toEqual(["", "duplex", "rac_basic", "snowdon"]);
    expect(sel.value).toBe("duplex");
    expect(pop.current()).toEqual({ node: "load1", name: "model" });
  });

  it("select change commits setParam live", () => {
    pop.open(node("load.model", "load1", { model: "duplex" }), "model");
    const sel = root().querySelector("select")!;
    sel.value = "rac_basic";
    sel.dispatchEvent(new Event("change", { bubbles: true }));
    expect(setParams()).toEqual([{ t: "setParam", node: "load1", name: "model", value: "rac_basic" }]);
  });

  it("dynamicEnum keeps a current value that the option source no longer lists", () => {
    pop.open(node("select.byType", "t1", { type: "IfcGhost" }), "type");
    const sel = root().querySelector("select")!;
    expect([...sel.options].map((o) => o.value)).toContain("IfcGhost");
    expect(sel.value).toBe("IfcGhost");
  });

  it("number param gets a number input carrying the schema's min/max/step", () => {
    pop.open(node("viz.colormap", "cmap1", { min: 5 }), "min");
    const inp = root().querySelector("input")!;
    expect(inp.type).toBe("number");
    expect(inp.value).toBe("5");
  });

  it("number input commits live as a number, not a string", () => {
    pop.open(node("viz.colormap", "cmap1", { min: 0 }), "min");
    const inp = root().querySelector("input")!;
    inp.value = "42";
    inp.dispatchEvent(new Event("input", { bubbles: true }));
    expect(setParams()).toEqual([{ t: "setParam", node: "cmap1", name: "min", value: 42 }]);
  });

  it("boolean gets a checkbox that commits on change", () => {
    pop.open(node("viz.colorBy", "c1", { ghostOthers: true }), "ghostOthers");
    const box = root().querySelector<HTMLInputElement>("input[type=checkbox]")!;
    expect(box.checked).toBe(true);
    box.checked = false;
    box.dispatchEvent(new Event("change", { bubbles: true }));
    expect(setParams()).toEqual([{ t: "setParam", node: "c1", name: "ghostOthers", value: false }]);
  });

  it("text schema routes to the docked focus editor, not the popover (T2)", () => {
    pop.open(node("table.sql", "sql1", { sql: "SELECT 1" }), "sql");
    expect(root().style.display).toBe("none");     // popover stays closed
    const card = document.querySelector<HTMLElement>(".pf-focus")!;
    expect(card).toBeTruthy();
    expect(card.style.display).not.toBe("none");
    const ta = card.querySelector("textarea")!;
    expect(ta.value).toBe("SELECT 1");
    expect(getComputedStyle(ta).fontFamily).toMatch(/mono/i);
    expect(pop.current()).toEqual({ node: "sql1", name: "sql" });
  });

  it("open on an unknown param is a no-op", () => {
    pop.open(node("load.model", "load1"), "nope");
    expect(root().style.display).toBe("none");
    expect(pop.current()).toBeNull();
  });

  it("close() hides, clears current(), and empties the DOM", () => {
    pop.open(node("load.model", "load1"), "model");
    pop.close();
    expect(root().style.display).toBe("none");
    expect(pop.current()).toBeNull();
    expect(root().childElementCount).toBe(0);
  });

  it("contains() distinguishes inside from outside (click-away's test)", () => {
    pop.open(node("load.model", "load1"), "model");
    expect(pop.contains(root().querySelector("select"))).toBe(true);
    expect(pop.contains(document.body)).toBe(false);
  });

  it("focuses the field on open", () => {
    pop.open(node("viz.colormap", "cmap1"), "min");
    expect(document.activeElement).toBe(root().querySelector("input"));
  });
});

// ── T2 commit policy: the pure function is the contract (params.ts header
// says exactly this). DOM-level explicit-buffer specs stay skip-marked until
// the T2 join because the focus-editor surface is still moving.
describe("commitPolicy (pure — T2 commit-mode contract)", () => {
  const live = { kind: "text" } as const;
  const explicit = { kind: "text", commit: "explicit" } as const;

  it("live: input dispatches, blur and keys do nothing", () => {
    expect(commitPolicy(live, { type: "input" })).toBe("dispatch");
    expect(commitPolicy(live, { type: "blur" })).toBe("none");
    expect(commitPolicy(live, { type: "key", key: "Enter", ctrl: true })).toBe("none");
    expect(commitPolicy(live, { type: "key", key: "Escape" })).toBe("none");
  });

  it("explicit: input buffers, Ctrl/Cmd-Enter commits, Esc reverts, blur commits", () => {
    expect(commitPolicy(explicit, { type: "input" })).toBe("buffer");
    expect(commitPolicy(explicit, { type: "key", key: "Enter", ctrl: true })).toBe("commit");
    expect(commitPolicy(explicit, { type: "key", key: "Enter", meta: true })).toBe("commit");
    expect(commitPolicy(explicit, { type: "key", key: "Enter" })).toBe("none");   // plain Enter = newline
    expect(commitPolicy(explicit, { type: "key", key: "Escape" })).toBe("revert");
    expect(commitPolicy(explicit, { type: "blur" })).toBe("commit");
  });

  it("the three text params in kinds.ts are explicit; everything else is live", () => {
    const explicitOnes: string[] = [];
    for (const info of KINDS) {
      for (const [pname, schema] of Object.entries(info.params)) {
        if (schema.commit === "explicit") explicitOnes.push(`${info.kind}.${pname}`);
      }
    }
    expect(explicitOnes.sort()).toEqual(["compute.expr.expr", "table.ask.question", "table.sql.sql"]);
  });
});

// ── T2 landed: explicit buffering + focus editor, against a popover instance
// with the full hook set (getStatus feeds the result preview). This instance's
// DOM roots are appended AFTER the shared `pop`'s, so selectors take `.at(-1)`.
describe("explicit-buffer DOM behavior + focus editor (T2)", () => {
  let sent: EditorIntent[] = [];
  let status: Record<string, NodeStatus> = {};
  let pop2: ParamPopover;

  const ENUMS: Record<string, string[]> = {
    columns: ["GlobalId", "Category", "Level"],
    parameters: ["Area", "Volume"],
    channels: ["operational_carbon", "embodied_carbon"],
  };

  const explicitStringKind: NodeKindInfo = {
    ...kindInfo("data.csv")!,
    kind: "test.explicitString",
    params: { url: { kind: "string", commit: "explicit" } },
  };

  beforeEach(() => {
    sent = [];
    status = {};
    pop2 = createParamPopover({
      dispatch: (i) => sent.push(i),
      kindInfo: (k) => (k === "test.explicitString" ? explicitStringKind : kindInfo(k)),
      getEnumOptions: (_n, _p, source) => ENUMS[source] ?? [],
      getStatus: (id) => status[id],
    });
  });
  afterEach(() => pop2.destroy());

  const sent2 = () =>
    sent.filter((i) => i.k === "graph" && i.intent.t === "setParam")
      .map((i) => (i.k === "graph" ? i.intent : null)!);
  const root2 = () => [...document.querySelectorAll<HTMLElement>(".pf-popover")].at(-1)!;
  const card = () => [...document.querySelectorAll<HTMLElement>(".pf-focus")].at(-1)!;
  const ta = () => card().querySelector("textarea")!;
  const chips = () => [...card().querySelectorAll<HTMLElement>(".pf-chip")].map((c) => c.textContent);
  const type = (el: HTMLInputElement | HTMLTextAreaElement, text: string) => {
    el.value = text;
    el.dispatchEvent(new Event("input", { bubbles: true }));
  };
  const chord = (el: Element | Window, k: string, mods: { ctrl?: boolean; meta?: boolean } = {}) =>
    el.dispatchEvent(new KeyboardEvent("keydown", {
      key: k, ctrlKey: !!mods.ctrl, metaKey: !!mods.meta, bubbles: true, cancelable: true,
    }));

  // — popover path: synthetic explicit `string` proves the policy is schema-driven —

  const openExplicitString = () => {
    pop2.open(node("test.explicitString", "n1", { url: "a.csv" }), "url");
    return root2().querySelector<HTMLInputElement>("input[type=text]")!;
  };

  it("explicit popover field buffers keystrokes — nothing dispatches", () => {
    const el = openExplicitString();
    type(el, "b.csv");
    type(el, "bb.csv");
    expect(sent2()).toEqual([]);
    expect(pop2.current()).toEqual({ node: "n1", name: "url" });
  });

  it("Ctrl-Enter commits ONCE and closes on commit", () => {
    const el = openExplicitString();
    type(el, "b.csv");
    chord(el, "Enter", { ctrl: true });
    expect(sent2()).toEqual([{ t: "setParam", node: "n1", name: "url", value: "b.csv" }]);
    expect(pop2.current()).toBeNull();
    expect(root2().style.display).toBe("none");
  });

  it("blur commits the buffer and closes", () => {
    const el = openExplicitString();
    type(el, "c.csv");
    el.dispatchEvent(new FocusEvent("blur"));
    expect(sent2()).toEqual([{ t: "setParam", node: "n1", name: "url", value: "c.csv" }]);
    expect(pop2.current()).toBeNull();
  });

  it("close() commits a dirty buffer (click-away keeps the edit)", () => {
    const el = openExplicitString();
    type(el, "d.csv");
    pop2.close();
    expect(sent2()).toEqual([{ t: "setParam", node: "n1", name: "url", value: "d.csv" }]);
  });

  // T14: Esc arrives via popover.cancel() — index.ts's single window-capture
  // listener calls it (editor-dom.spec proves the end-to-end key path). The
  // old params.ts capture hook and its registration-order treaty are gone.
  it("cancel() (the Esc pathway) reverts — no dispatch, closed", () => {
    const el = openExplicitString();
    type(el, "junk");
    pop2.cancel();
    expect(sent2()).toEqual([]);
    expect(pop2.current()).toBeNull();
  });

  it("field-level Esc in the input reverts and closes too (same semantics)", () => {
    const el = openExplicitString();
    type(el, "junk");
    chord(el, "Escape");
    expect(sent2()).toEqual([]);
    expect(pop2.current()).toBeNull();
  });

  it("a clean buffer commits nothing on close", () => {
    openExplicitString();
    pop2.close();
    expect(sent2()).toEqual([]);
  });

  it("pointerdown inside the popover does not propagate (dismissal churn guard)", () => {
    openExplicitString();
    const seen = vi.fn();
    document.addEventListener("pointerdown", seen);
    root2().dispatchEvent(new Event("pointerdown", { bubbles: true }));
    document.removeEventListener("pointerdown", seen);
    expect(seen).not.toHaveBeenCalled();
  });

  // — focus editor: the docked card for `text` params —

  it("renders the input schema list: SQL views + model columns/parameters", () => {
    pop2.open(node("table.sql", "sql1", { sql: "" }), "sql");
    for (const v of ["EntityText", "ParameterText", "RelationText", "GlobalId", "Category", "Area"])
      expect(chips()).toContain(v);
  });

  it("compute.expr lists in-scope names + channels instead of views", () => {
    pop2.open(node("compute.expr", "x1", { channel: "r", expr: "1" }), "expr");
    for (const v of ["gid", "type", "name", "level", "param('Name')", "ch('name')", "operational_carbon"])
      expect(chips()).toContain(v);
    expect(chips()).not.toContain("EntityText");
  });

  it("renders the result preview from the node's cached status", () => {
    status.sql1 = { state: "ok", summary: "14 rows", detail: "SELECT 1" };
    pop2.open(node("table.sql", "sql1", { sql: "" }), "sql");
    const prev = card().querySelector(".pf-focus-preview")!;
    expect(prev.textContent).toContain("14 rows");
    expect(prev.textContent).toContain("SELECT 1");
  });

  it("marks the reserved AI-assist slot (study §5.2)", () => {
    pop2.open(node("table.sql", "sql1", { sql: "" }), "sql");
    expect(card().querySelector(".pf-ai-slot")?.textContent).toContain("AI assist");
  });

  it("focus editor buffers typing; Ctrl-Enter commits once and closes the card", () => {
    pop2.open(node("table.sql", "sql1", { sql: "SELECT 1" }), "sql");
    type(ta(), "SELECT 2");
    expect(sent2()).toEqual([]);
    chord(ta(), "Enter", { ctrl: true });
    expect(sent2()).toEqual([{ t: "setParam", node: "sql1", name: "sql", value: "SELECT 2" }]);
    expect(pop2.current()).toBeNull();
    expect(card().style.display).toBe("none");
  });

  it("cancel() reverts the focus editor's buffer without dispatching (Esc pathway)", () => {
    pop2.open(node("table.sql", "sql1", { sql: "SELECT 1" }), "sql");
    type(ta(), "garbage(");
    pop2.cancel();
    expect(sent2()).toEqual([]);
    expect(pop2.current()).toBeNull();
  });

  it("the selection-change close path (popover.close) commits a dirty buffer", () => {
    pop2.open(node("table.ask", "ask1", { question: "q?" }), "question");
    type(ta(), "how many walls?");
    pop2.close();                                  // what index.ts calls on selection change
    expect(sent2()).toEqual(
      [{ t: "setParam", node: "ask1", name: "question", value: "how many walls?" }]);
  });

  it("clicking a schema chip inserts into the buffer without dispatching", () => {
    pop2.open(node("table.sql", "sql1", { sql: "" }), "sql");
    const chip = [...card().querySelectorAll<HTMLElement>(".pf-chip")]
      .find((c) => c.textContent === "EntityText")!;
    chip.dispatchEvent(new MouseEvent("click", { bubbles: true }));
    expect(ta().value).toBe("EntityText");
    expect(sent2()).toEqual([]);
  });

  it("pointerdown inside the card does not propagate (dismissal churn guard)", () => {
    pop2.open(node("table.sql", "sql1", { sql: "" }), "sql");
    const seen = vi.fn();
    document.addEventListener("pointerdown", seen);
    ta().dispatchEvent(new Event("pointerdown", { bubbles: true }));
    document.removeEventListener("pointerdown", seen);
    expect(seen).not.toHaveBeenCalled();
  });

  it("preview polls while open and stops after close", () => {
    vi.useFakeTimers();
    try {
      pop2.open(node("table.sql", "sql1", { sql: "" }), "sql");
      status.sql1 = { state: "error", message: "boom" };
      vi.advanceTimersByTime(600);
      expect(card().querySelector(".pf-focus-preview")!.textContent).toContain("boom");
      pop2.close();
      status.sql1 = { state: "ok", summary: "later" };
      vi.advanceTimersByTime(1200);                // no timer left; no throw, card empty
      expect(card().childElementCount).toBe(0);
    } finally {
      vi.useRealTimers();
    }
  });
});

// ── T2: schemaGroups + standalone focus API (what T14/T8 build against) ─────

describe("schemaGroups / standalone focus editor", () => {
  it("drops empty groups (no model wired = no columns section)", () => {
    const g = schemaGroups(node("table.sql", "s1", {}), "sql", () => []);
    expect(g.map((x) => x.title)).toEqual(["views"]);
  });

  it("standalone focus editor works without a popover (direct API)", () => {
    const sent: EditorIntent[] = [];
    const fe = createFocusEditor({
      dispatch: (i) => sent.push(i),
      kindInfo,
      getEnumOptions: () => [],
    });
    fe.open(node("compute.expr", "x1", { channel: "c", expr: "1" }), "expr",
      kindInfo("compute.expr")!.params.expr);
    const ta = [...document.querySelectorAll<HTMLTextAreaElement>(".pf-focus textarea")].at(-1)!;
    expect([...document.querySelectorAll(".pf-focus-preview")].at(-1)!.textContent)
      .toContain("no result yet");                 // no getStatus hook = placeholder
    ta.value = "2";
    ta.dispatchEvent(new Event("input", { bubbles: true }));
    fe.revert();
    fe.close();
    expect(sent).toEqual([]);                      // reverted, so close commits nothing
    fe.destroy();
  });
});
