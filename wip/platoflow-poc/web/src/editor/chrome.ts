// Editor chrome: the top menu bar (title · Examples · Add Node · host status)
// and the collapsible left node palette. Plain DOM overlaying the canvas host.
//
// Chrome never touches a GraphDoc: it calls back out (`onExample`) or dispatches
// the same `addKind` editor intent the canvas palette does.
import type { NodeKindInfo } from "../contracts";
import { CATEGORY_CSS, CATEGORY_ORDER } from "./geom";
import { firstSentence } from "./help";

export const MENUBAR_H = 34;

const CSS = `
.pf-menubar { position: absolute; top: 0; left: 0; right: 0; height: ${MENUBAR_H}px;
  box-sizing: border-box; z-index: 30; display: flex; align-items: center; gap: 4px;
  padding: 0 10px; background: var(--pf-bg); border-bottom: 1px solid var(--pf-border);
  color: var(--pf-text); font: 12px var(--pf-font); user-select: none; }
.pf-menubar .pf-brand { color: var(--pf-text); font-weight: 600; letter-spacing: .02em;
  margin-right: 10px; }
.pf-menubar button { background: transparent; color: var(--pf-text); border: 1px solid transparent;
  border-radius: 5px; padding: 4px 9px; font: 12px var(--pf-font); cursor: pointer; }
.pf-menubar button:hover { background: var(--pf-hover); }
.pf-menubar button.pf-on { background: var(--pf-active); border-color: var(--pf-border-strong); color: var(--pf-text); }
.pf-menubar .pf-spacer { flex: 1; }
.pf-menubar .pf-hoststatus { color: var(--pf-dim); font-size: 11px; max-width: 46%;
  overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.pf-menu { position: relative; }
.pf-menu-list { display: none; position: absolute; top: 100%; left: 0; min-width: 168px;
  background: var(--pf-surface); border: 1px solid var(--pf-border); border-radius: 6px; padding: 4px;
  box-shadow: 0 4px 16px rgba(0,0,0,.07); }
.pf-menu.pf-open .pf-menu-list { display: block; }
.pf-menu-list button { display: block; width: 100%; text-align: left; }
.pf-menu-list button:disabled { color: var(--pf-faint); cursor: default; }
.pf-menu-list button:disabled:hover { background: transparent; border-color: transparent; }
.pf-menu-item .pf-kbd { float: right; color: var(--pf-faint); margin-left: 18px; font-size: 11px; }
.pf-menu-item .pf-check { display: inline-block; width: 14px; color: var(--pf-text); }
.pf-menu-list .pf-menu-sep { border-top: 1px solid var(--pf-border); margin: 4px 2px; }
.pf-menu-list .pf-menu-sect { padding: 2px 9px; color: var(--pf-dim); font-size: 10px;
  letter-spacing: .08em; text-transform: uppercase; }

.pf-palette-panel { position: absolute; left: 0; top: ${MENUBAR_H}px; bottom: 0; width: 236px;
  box-sizing: border-box; z-index: 25; display: none; flex-direction: column;
  background: var(--pf-surface); border-right: 1px solid var(--pf-border); color: var(--pf-text);
  font: 12px var(--pf-font); }
.pf-palette-panel.pf-open { display: flex; }
.pf-palette-panel .pf-pal-head { display: flex; align-items: center; justify-content: space-between;
  padding: 8px 10px 6px; border-bottom: 1px solid var(--pf-border); }
.pf-palette-panel .pf-pal-head b { color: var(--pf-text); font-size: 12px; }
.pf-palette-panel .pf-pal-head button { background: transparent; color: var(--pf-dim); border: 0;
  cursor: pointer; font-size: 14px; line-height: 1; padding: 2px 4px; }
.pf-palette-panel .pf-pal-search { padding: 8px 8px 2px; }
.pf-palette-panel .pf-pal-search input { width: 100%; box-sizing: border-box; background: var(--pf-input);
  color: var(--pf-text); border: 1px solid var(--pf-border); border-radius: 5px; padding: 5px 7px;
  font: 12px var(--pf-font); }
.pf-palette-panel .pf-pal-body { overflow-y: auto; padding: 6px 6px 14px; }
.pf-pal-empty { margin: 10px 6px 0; color: var(--pf-dim); font-size: 11px; font-style: italic; }
.pf-cat { margin: 10px 4px 4px; font-size: 10px; letter-spacing: .08em; text-transform: uppercase;
  color: var(--pf-dim); font-weight: 600; }
.pf-kind-row { display: block; width: 100%; text-align: left; background: transparent;
  border: 1px solid transparent; border-radius: 5px; padding: 5px 7px; cursor: pointer;
  color: var(--pf-text); font: 12px var(--pf-font); border-left-width: 3px; }
.pf-kind-row:hover { background: var(--pf-hover); }
.pf-kind-row .pf-kind-label { display: block; color: var(--pf-text); font-weight: 600; }
.pf-kind-row .pf-kind-desc { display: block; color: var(--pf-dim); font-size: 11px; line-height: 1.35;
  margin-top: 1px; }
.pf-pal-hint { margin: 10px 6px 0; color: var(--pf-faint); font-size: 10px; line-height: 1.4; }

.pf-dialog-veil { position: fixed; inset: 0; z-index: 80; background: rgba(26,26,24,.25);
  display: none; align-items: center; justify-content: center; }
.pf-dialog-veil.pf-open { display: flex; }
.pf-dialog { width: 440px; max-width: 86vw; max-height: 76vh; overflow-y: auto; box-sizing: border-box;
  background: var(--pf-surface); border: 1px solid var(--pf-border-strong); border-radius: 8px;
  box-shadow: 0 8px 30px rgba(0,0,0,.12); padding: 14px 16px; color: var(--pf-text);
  font: 13px var(--pf-font); line-height: 1.5; }
.pf-dialog h3 { margin: 0 0 8px; font-size: 14px; }
.pf-dialog ul { margin: 0 0 10px; padding-left: 18px; }
.pf-dialog li { margin: 3px 0; }
.pf-dialog .pf-dialog-ok { display: block; margin: 6px 0 0 auto; background: var(--pf-input);
  border: 1px solid var(--pf-border-strong); border-radius: 6px; padding: 5px 14px; cursor: pointer;
  font: 12px var(--pf-font); color: var(--pf-text); }
.pf-dialog .pf-dialog-ok:hover { background: var(--pf-hover); }
`;

function ensureStyle(): void {
  if (document.getElementById("pf-chrome-style")) return;
  const el = document.createElement("style");
  el.id = "pf-chrome-style";
  el.textContent = CSS;
  document.head.appendChild(el);
}

export interface ChromeSpec {
  /** Named graphs the host can load (demo/example picker). */
  examples: string[];
  onExample(name: string): void;
  /** W9-E: when present, the Examples dropdown ignores the static `examples`
   *  list and rebuilds itself from this EVERY time it opens — demos first,
   *  saved graphs in a separated section below — so a just-saved graph is in
   *  the menu without a reload. */
  getExamples?: () => Promise<{ demos: string[]; saved: string[] }>;
  /** W9-E: when present, the dropdown gains a "Save graph…" item. Chrome only
   *  invokes the callback — naming/prompting is the app's problem. */
  onSaveGraph?: () => void;
  /** When present, the Help menu gains a "Run walkthrough…" item. The app owns
   *  the tour itself (it needs the editor + example loader). */
  onWalkthrough?: () => void;
  /** App title in the bar. */
  title?: string;
}

export interface ChromeHooks {
  /** Add a node of this kind at the current viewport centre. */
  addKind(kind: string): void;
  /** Help menu: open/close the help expando on every node. */
  setHelpAll(open: boolean): void;
  /** Edit menu items. */
  undo(): void;
  redo(): void;
  /** W13: View menu / F key — animate pan+zoom so the graph fits the visible canvas. */
  fitGraph(): void;
  /** W13: View menu / T key — auto-layout the graph (one undoable intent), then fit. */
  tidyGraph(): void;
}

export interface Chrome {
  /** Host status text on the right of the bar. */
  setStatus(msg: string): void;
  /** Edit-menu enabled state; the editor pushes this on every commit. */
  syncHistory(canUndo: boolean, canRedo: boolean): void;
  setPaletteOpen(open: boolean): void;
  togglePalette(): void;
  destroy(): void;
}

export function createChrome(
  host: HTMLElement,
  kinds: NodeKindInfo[],
  spec: ChromeSpec,
  hooks: ChromeHooks,
): Chrome {
  ensureStyle();

  // ── menu bar ───────────────────────────────────────────────────────────────
  const bar = document.createElement("div");
  bar.className = "pf-menubar";

  const brand = document.createElement("span");
  brand.className = "pf-brand";
  brand.textContent = spec.title ?? "PlatoFlow";

  const menu = document.createElement("div");
  menu.className = "pf-menu";
  const menuBtn = document.createElement("button");
  menuBtn.className = "pf-menu-trigger";
  menuBtn.textContent = "Examples ▾";
  const list = document.createElement("div");
  list.className = "pf-menu-list";

  // Examples content. The static `examples` array fills once (pre-W9 behavior
  // the harness and old specs pin); with `getExamples` the list is rebuilt from
  // the host on every open — see rebuildExamples below.
  const exampleItem = (name: string): HTMLButtonElement => {
    const item = document.createElement("button");
    item.className = "pf-menu-item";
    item.textContent = name;
    item.addEventListener("click", () => { closeMenu(); spec.onExample(name); });
    return item;
  };
  const noteItem = (text: string, cls: string): HTMLButtonElement => {
    const item = document.createElement("button");
    item.className = cls;
    item.textContent = text;
    item.disabled = true;
    return item;
  };
  const sep = (): HTMLElement => {
    const el = document.createElement("div");
    el.className = "pf-menu-sep";
    return el;
  };
  const saveItems = (): HTMLElement[] => {
    if (!spec.onSaveGraph) return [];
    const item = document.createElement("button");
    item.className = "pf-menu-item pf-save-graph";
    item.textContent = "Save graph…";
    item.addEventListener("click", () => { closeMenus(); spec.onSaveGraph!(); });
    return [sep(), item];
  };
  const fillExamples = (demos: string[], saved: string[]): void => {
    const items: HTMLElement[] = demos.map(exampleItem);
    if (!demos.length && !saved.length) items.push(noteItem("(none)", "pf-menu-empty"));
    if (saved.length) {
      const label = document.createElement("div");
      label.className = "pf-menu-sect";
      label.textContent = "saved";
      items.push(sep(), label, ...saved.map(exampleItem));
    }
    items.push(...saveItems());
    list.replaceChildren(...items);
  };
  let fillSeq = 0;                                    // stale async fills lose
  const rebuildExamples = async (): Promise<void> => {
    if (!spec.getExamples) return;
    const seq = ++fillSeq;
    list.replaceChildren(noteItem("…", "pf-menu-pending"), ...saveItems());
    try {
      const { demos, saved } = await spec.getExamples();
      if (seq === fillSeq) fillExamples(demos, saved);
    } catch {
      if (seq === fillSeq)
        list.replaceChildren(noteItem("(failed to load)", "pf-menu-empty"), ...saveItems());
    }
  };
  fillExamples(spec.examples, []);
  menu.append(menuBtn, list);

  // Edit ▾ — Undo/Redo; enabled state pushed in via syncHistory (same shape as
  // the Help menu: static DOM, behavior through hooks).
  const editMenu = document.createElement("div");
  editMenu.className = "pf-menu";
  const editBtn = document.createElement("button");
  editBtn.className = "pf-edit-trigger";
  editBtn.textContent = "Edit ▾";
  const editList = document.createElement("div");
  editList.className = "pf-menu-list";
  const editItem = (label: string, kbd: string, action: () => void): HTMLButtonElement => {
    const item = document.createElement("button");
    item.className = "pf-menu-item";
    item.dataset.edit = label.toLowerCase();
    item.disabled = true;                        // fresh doc: no history either way
    const text = document.createElement("span");
    text.textContent = label;
    const hint = document.createElement("span");
    hint.className = "pf-kbd";
    hint.textContent = kbd;
    item.append(text, hint);
    item.addEventListener("click", () => { closeMenus(); action(); });
    editList.appendChild(item);
    return item;
  };
  const undoItem = editItem("Undo", "Ctrl+Z", hooks.undo);
  const redoItem = editItem("Redo", "Ctrl+Y", hooks.redo);
  editMenu.append(editBtn, editList);

  // View ▾ — check items for panels. "Add Node" toggles the left palette
  // (open by default; P still toggles it).
  const viewMenu = document.createElement("div");
  viewMenu.className = "pf-menu";
  const viewBtn = document.createElement("button");
  viewBtn.className = "pf-view-trigger";
  viewBtn.textContent = "View ▾";
  const viewList = document.createElement("div");
  viewList.className = "pf-menu-list";
  const addNodeItem = document.createElement("button");
  addNodeItem.className = "pf-menu-item pf-view-addnode";
  const addNodeCheck = document.createElement("span");
  addNodeCheck.className = "pf-check";
  const addNodeText = document.createElement("span");
  addNodeText.textContent = "Add Node";
  const addNodeKbd = document.createElement("span");
  addNodeKbd.className = "pf-kbd";
  addNodeKbd.textContent = "P";
  addNodeItem.append(addNodeCheck, addNodeText, addNodeKbd);
  addNodeItem.addEventListener("click", () => { closeMenus(); togglePalette(); });
  viewList.appendChild(addNodeItem);
  // W13: camera/layout actions. Same hooks the F/T keys hit — menu is the
  // discoverable path, keys are the fast one.
  viewList.appendChild(sep());
  const viewAction = (label: string, kbd: string, cls: string, action: () => void): void => {
    const item = document.createElement("button");
    item.className = `pf-menu-item ${cls}`;
    const text = document.createElement("span");
    text.textContent = label;
    const hint = document.createElement("span");
    hint.className = "pf-kbd";
    hint.textContent = kbd;
    item.append(text, hint);
    item.addEventListener("click", () => { closeMenus(); action(); });
    viewList.appendChild(item);
  };
  viewAction("Fit graph", "F", "pf-view-fit", hooks.fitGraph);
  viewAction("Tidy graph", "T", "pf-view-tidy", hooks.tidyGraph);
  viewMenu.append(viewBtn, viewList);

  // Help ▾ — Show all / Hide all node help expandos.
  const helpMenu = document.createElement("div");
  helpMenu.className = "pf-menu";
  const helpBtn = document.createElement("button");
  helpBtn.className = "pf-help-trigger";
  helpBtn.textContent = "Help ▾";
  const helpList = document.createElement("div");
  helpList.className = "pf-menu-list";
  for (const [text, open] of [["Show all", true], ["Hide all", false]] as const) {
    const item = document.createElement("button");
    item.className = "pf-menu-item";
    item.dataset.help = String(open);
    item.textContent = text;
    item.addEventListener("click", () => { closeMenus(); hooks.setHelpAll(open); });
    helpList.appendChild(item);
  }

  // "Show instructions" → a simple modal text dialog (the old canvas HUD hint
  // text lives here now). Esc, OK, or a click on the veil closes it.
  const INSTRUCTIONS = [
    "Drag from a socket to another node's socket to wire nodes.",
    "Click a param row on a card to edit it; \"?\" on the header toggles the card's help.",
    "Right-click the canvas to add a node where you click; rows in the Add Node panel (P) drop at the view centre.",
    "Drag empty canvas to pan; scroll to zoom; Shift-drag for a marquee selection.",
    "Del removes the selection; Ctrl+Z / Ctrl+Y undo and redo; Ctrl+C / V / D copy, paste, duplicate.",
    "Double-click a group node's header to enter its subgraph; double-click empty canvas to exit.",
  ];
  const veil = document.createElement("div");
  veil.className = "pf-dialog-veil";
  const dialog = document.createElement("div");
  dialog.className = "pf-dialog";
  const dlgTitle = document.createElement("h3");
  dlgTitle.textContent = "Instructions";
  const dlgList = document.createElement("ul");
  for (const line of INSTRUCTIONS) {
    const li = document.createElement("li");
    li.textContent = line;
    dlgList.appendChild(li);
  }
  const dlgOk = document.createElement("button");
  dlgOk.className = "pf-dialog-ok";
  dlgOk.textContent = "OK";
  dialog.append(dlgTitle, dlgList, dlgOk);
  veil.appendChild(dialog);
  document.body.appendChild(veil);
  const setInstructionsOpen = (open: boolean) => veil.classList.toggle("pf-open", open);
  dlgOk.addEventListener("click", () => setInstructionsOpen(false));
  veil.addEventListener("click", (ev) => { if (ev.target === veil) setInstructionsOpen(false); });

  helpList.appendChild(sep());
  if (spec.onWalkthrough) {
    const tourItem = document.createElement("button");
    tourItem.className = "pf-menu-item pf-help-walkthrough";
    tourItem.textContent = "Run walkthrough…";
    tourItem.addEventListener("click", () => { closeMenus(); spec.onWalkthrough!(); });
    helpList.appendChild(tourItem);
  }
  const instrItem = document.createElement("button");
  instrItem.className = "pf-menu-item pf-help-instructions";
  instrItem.textContent = "Show instructions…";
  instrItem.addEventListener("click", () => { closeMenus(); setInstructionsOpen(true); });
  helpList.appendChild(instrItem);
  helpMenu.append(helpBtn, helpList);

  const spacer = document.createElement("span");
  spacer.className = "pf-spacer";

  const status = document.createElement("span");
  status.className = "pf-hoststatus";
  status.id = "pf-host-status";

  bar.append(brand, menu, editMenu, viewMenu, helpMenu, spacer, status);
  host.appendChild(bar);

  const menus = [menu, editMenu, viewMenu, helpMenu];
  const closeMenus = () => menus.forEach((m) => m.classList.remove("pf-open"));
  const closeMenu = closeMenus;
  const trigger = (own: HTMLElement) => (ev: Event) => {
    ev.stopPropagation();
    menus.forEach((m) => m !== own && m.classList.remove("pf-open"));
    own.classList.toggle("pf-open");
  };
  menuBtn.addEventListener("click", trigger(menu));
  // Dynamic mode rebuilds on every OPEN. Registered after trigger(), which has
  // already toggled the class by the time this runs (stopPropagation does not
  // stop same-element listeners).
  menuBtn.addEventListener("click", () => {
    if (menu.classList.contains("pf-open")) void rebuildExamples();
  });
  editBtn.addEventListener("click", trigger(editMenu));
  viewBtn.addEventListener("click", trigger(viewMenu));
  helpBtn.addEventListener("click", trigger(helpMenu));

  // ── palette panel ──────────────────────────────────────────────────────────
  const panel = document.createElement("aside");
  panel.className = "pf-palette-panel";

  const head = document.createElement("div");
  head.className = "pf-pal-head";
  const headTitle = document.createElement("b");
  headTitle.textContent = "Add Node";
  const close = document.createElement("button");
  close.className = "pf-pal-close";
  close.textContent = "×";
  close.title = "Close (P)";
  head.append(headTitle, close);

  // W13: type-to-filter over the rows (same substring rule as picker.ts —
  // the house pattern for searchable lists).
  const searchWrap = document.createElement("div");
  searchWrap.className = "pf-pal-search";
  const search = document.createElement("input");
  search.type = "text";
  search.placeholder = "search nodes…";
  searchWrap.appendChild(search);

  const body = document.createElement("div");
  body.className = "pf-pal-body";

  // Rows keyed by group so a fully-filtered category can take its header down
  // with it — a lone header over nothing reads like a rendering bug.
  interface PalRow { el: HTMLButtonElement; text: string; kind: string }
  const palGroups: { header: HTMLElement; rows: PalRow[] }[] = [];

  for (const cat of CATEGORY_ORDER) {
    const inCat = kinds.filter((k) => k.category === cat);
    if (!inCat.length) continue;
    const h = document.createElement("div");
    h.className = "pf-cat";
    h.textContent = cat;
    h.style.color = CATEGORY_CSS[cat];
    body.appendChild(h);
    const group = { header: h, rows: [] as PalRow[] };
    palGroups.push(group);
    for (const k of inCat) {
      const row = document.createElement("button");
      row.className = "pf-kind-row";
      row.dataset.kind = k.kind;
      row.style.borderLeftColor = CATEGORY_CSS[cat];
      row.title = `${k.label} — ${k.description}`;
      const label = document.createElement("span");
      label.className = "pf-kind-label";
      label.textContent = k.label;
      const desc = document.createElement("span");
      desc.className = "pf-kind-desc";
      desc.textContent = firstSentence(k.description);
      row.append(label, desc);
      // panel stays open: adding several nodes in a row is the common case
      row.addEventListener("click", () => hooks.addKind(k.kind));
      body.appendChild(row);
      // match over the FULL description, not just the rendered first sentence —
      // "clearance" may only appear in sentence two
      group.rows.push({ el: row, text: `${k.label} ${k.description}`.toLowerCase(), kind: k.kind });
    }
  }
  const emptyNote = document.createElement("div");
  emptyNote.className = "pf-pal-empty";
  emptyNote.textContent = "(no matches)";
  emptyNote.style.display = "none";
  body.appendChild(emptyNote);
  const hint = document.createElement("div");
  hint.className = "pf-pal-hint";
  hint.textContent = "Click a row to drop the node at the view centre. P toggles this panel; right-click the canvas adds where you click.";
  body.appendChild(hint);

  const applyFilter = (): void => {
    const q = search.value.trim().toLowerCase();
    let any = false;
    for (const g of palGroups) {
      let vis = 0;
      for (const r of g.rows) {
        const hit = !q || r.text.includes(q);
        r.el.style.display = hit ? "" : "none";
        if (hit) vis++;
      }
      g.header.style.display = vis ? "" : "none";
      any = any || vis > 0;
    }
    emptyNote.style.display = any ? "none" : "";
    hint.style.display = q ? "none" : "";       // hint is resting-state text only
  };
  const topVisibleKind = (): string | null => {
    for (const g of palGroups)
      for (const r of g.rows)
        if (r.el.style.display !== "none") return r.kind;
    return null;
  };
  search.addEventListener("input", applyFilter);
  search.addEventListener("keydown", (ev) => {
    if (ev.key === "Enter") {
      ev.preventDefault(); ev.stopPropagation();
      const kind = topVisibleKind();
      if (kind) { hooks.addKind(kind); search.value = ""; applyFilter(); }
    } else if (ev.key === "Escape") {
      ev.stopPropagation();                     // chrome/canvas Esc must not also fire
      if (search.value) { search.value = ""; applyFilter(); }
      else search.blur();
    }
  });

  panel.append(head, searchWrap, body);
  host.appendChild(panel);

  const setPaletteOpen = (open: boolean) => {
    panel.classList.toggle("pf-open", open);
    addNodeItem.classList.toggle("pf-on", open);
    addNodeCheck.textContent = open ? "✓" : "";
  };
  const togglePalette = () => setPaletteOpen(!panel.classList.contains("pf-open"));

  close.addEventListener("click", () => setPaletteOpen(false));
  setPaletteOpen(true);                          // Add Node panel shows by default

  // ── global listeners ───────────────────────────────────────────────────────
  const typing = (t: EventTarget | null) =>
    t instanceof HTMLElement && /^(INPUT|TEXTAREA|SELECT)$/.test(t.tagName);

  const onKey = (ev: KeyboardEvent) => {
    if (typing(ev.target)) return;
    // F/T check modifiers so Ctrl+F (find) / Ctrl+T (new tab) stay the browser's
    const plain = !ev.ctrlKey && !ev.metaKey && !ev.altKey;
    if (ev.key === "p" || ev.key === "P") { ev.preventDefault(); togglePalette(); }
    else if (plain && (ev.key === "f" || ev.key === "F")) { ev.preventDefault(); hooks.fitGraph(); }
    else if (plain && (ev.key === "t" || ev.key === "T")) { ev.preventDefault(); hooks.tidyGraph(); }
    else if (ev.key === "Escape") { setInstructionsOpen(false); closeMenu(); }
  };
  const onDocClick = (ev: MouseEvent) => {
    if (!menus.some((m) => m.contains(ev.target as Node))) closeMenus();
  };

  window.addEventListener("keydown", onKey);
  document.addEventListener("click", onDocClick);

  return {
    setStatus: (msg) => { status.textContent = msg; status.title = msg; },
    syncHistory(canUndo, canRedo) {
      undoItem.disabled = !canUndo;
      redoItem.disabled = !canRedo;
    },
    setPaletteOpen,
    togglePalette,
    destroy() {
      window.removeEventListener("keydown", onKey);
      document.removeEventListener("click", onDocClick);
      bar.remove();
      panel.remove();
      veil.remove();
    },
  };
}
