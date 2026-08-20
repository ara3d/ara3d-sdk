// Reusable node context menu (W13-D). Generic organ: the host opens it with
// viewport coordinates and a flat entry list; what the entries DO is entirely
// the host's business (index.ts wires right-click-on-card at integration).
//
// Visual twin of the chrome menu dropdowns (.pf-menu-list) — deliberately its
// own CSS classes: Track C owns chrome.ts and is editing those concurrently,
// so sharing class names would couple the tracks. Same look, own namespace.
//
// House popover voice (params.ts / picker.ts): id-guarded <style> injection,
// pointerdown inside the organ never leaks to the canvas, thorough destroy.

export type MenuEntry =
  | "---"                                    // separator
  | { label: string; kbd?: string; danger?: boolean; disabled?: boolean; action(): void };

type ItemEntry = Exclude<MenuEntry, "---">;

export interface ContextMenu {
  /** Open at viewport coordinates (clamped so the menu never overflows the
   *  window). Replaces any open menu. */
  openAt(x: number, y: number, entries: MenuEntry[]): void;
  close(): void;
  isOpen(): boolean;
  /** True if the event target is inside the menu (host uses this to gate its
   *  own handlers — e.g. canvas gestures must ignore clicks the menu owns). */
  contains(target: EventTarget | null): boolean;
  destroy(): void;
}

const CSS = `
.pf-ctx { position: fixed; z-index: 90; min-width: 168px; box-sizing: border-box;
  background: var(--pf-surface); border: 1px solid var(--pf-border); border-radius: 6px;
  padding: 4px; box-shadow: 0 4px 16px rgba(0,0,0,.07);
  color: var(--pf-text); font: 12px var(--pf-font); user-select: none; }
.pf-ctx-item { display: flex; align-items: baseline; gap: 18px; width: 100%;
  box-sizing: border-box; text-align: left; background: transparent; border: 0;
  border-radius: 5px; padding: 4px 9px; font: 12px var(--pf-font);
  color: var(--pf-text); cursor: pointer; }
.pf-ctx-item:hover:enabled, .pf-ctx-item.pf-ctx-active { background: var(--pf-hover); }
.pf-ctx-label { flex: 1; white-space: nowrap; }
.pf-ctx-kbd { color: var(--pf-faint); font-size: 11px; }
.pf-ctx-danger, .pf-ctx-danger:hover:enabled { color: var(--pf-danger); }
.pf-ctx-item:disabled { color: var(--pf-faint); cursor: default; }
.pf-ctx-sep { border-top: 1px solid var(--pf-border); margin: 4px 2px; }
`;

function ensureStyle(): void {
  if (document.getElementById("pf-ctx-style")) return;
  const el = document.createElement("style");
  el.id = "pf-ctx-style";
  el.textContent = CSS;
  document.head.appendChild(el);
}

export function createContextMenu(host: HTMLElement = document.body): ContextMenu {
  ensureStyle();
  const root = document.createElement("div");
  root.className = "pf-ctx";
  root.setAttribute("role", "menu");
  root.style.display = "none";
  host.appendChild(root);

  // Clicks inside the menu are not canvas gestures and must not trip the
  // window click-away below (dismissal-churn guard, same rule as the picker).
  root.addEventListener("pointerdown", (ev) => ev.stopPropagation());
  // Right-clicking the open menu must not stack the NATIVE menu on top of it.
  root.addEventListener("contextmenu", (ev) => { ev.preventDefault(); ev.stopPropagation(); });

  let open = false;
  let items: { el: HTMLButtonElement; entry: ItemEntry }[] = [];
  let active = -1;                             // keyboard highlight; -1 until first arrow

  const paint = () =>
    items.forEach((it, i) => it.el.classList.toggle("pf-ctx-active", i === active));

  const close = () => {
    open = false;
    active = -1;
    items = [];
    root.style.display = "none";
    root.replaceChildren();
  };

  const run = (entry: ItemEntry) => { entry.action(); close(); };

  // Arrow navigation over ENABLED items only (separators never made it into
  // `items`; disabled ones did — they render but are skipped here). Wraps.
  const move = (dir: 1 | -1) => {
    const ok = items.flatMap((it, i) => (it.entry.disabled ? [] : [i]));
    if (!ok.length) return;
    const at = ok.indexOf(active);
    active = at < 0
      ? (dir === 1 ? ok[0] : ok[ok.length - 1])  // first arrow lands at an end
      : ok[(at + dir + ok.length) % ok.length];
    paint();
  };

  const onKey = (ev: KeyboardEvent) => {
    if (!open) return;
    if (ev.key === "ArrowDown") { ev.preventDefault(); move(1); }
    else if (ev.key === "ArrowUp") { ev.preventDefault(); move(-1); }
    else if (ev.key === "Enter") {
      if (active >= 0) { ev.preventDefault(); run(items[active].entry); }
    } else if (ev.key === "Escape") { ev.preventDefault(); close(); }
  };
  // Hosts open on `contextmenu`, which fires AFTER its pointerdown completed —
  // so the opening gesture never reaches this and self-closes the fresh menu.
  const onAway = (ev: PointerEvent) => {
    if (open && !(ev.target instanceof Node && root.contains(ev.target))) close();
  };
  const onBlur = () => { if (open) close(); };

  window.addEventListener("keydown", onKey);
  window.addEventListener("pointerdown", onAway);
  window.addEventListener("blur", onBlur);

  return {
    openAt(x, y, entries) {
      close();                                 // replaces any open menu
      for (const entry of entries) {
        if (entry === "---") {
          const sep = document.createElement("div");
          sep.className = "pf-ctx-sep";
          root.appendChild(sep);
          continue;
        }
        const el = document.createElement("button");
        el.type = "button";
        el.className = "pf-ctx-item" + (entry.danger ? " pf-ctx-danger" : "");
        el.setAttribute("role", "menuitem");
        const label = document.createElement("span");
        label.className = "pf-ctx-label";
        label.textContent = entry.label;
        el.appendChild(label);
        if (entry.kbd) {
          const kbd = document.createElement("span");
          kbd.className = "pf-ctx-kbd";
          kbd.textContent = entry.kbd;
          el.appendChild(kbd);
        }
        // Disabled items get NO listener at all: even a synthetic click event
        // (which `disabled` alone would not block) cannot fire the action.
        if (entry.disabled) el.disabled = true;
        else el.addEventListener("click", () => run(entry));
        items.push({ el, entry });
        root.appendChild(el);
      }
      open = true;
      // Show first, THEN measure and place: the rect is 0×0 while display:none,
      // and clamping needs the real size to keep the menu fully in the viewport.
      root.style.left = "0px";
      root.style.top = "0px";
      root.style.display = "block";
      const { width, height } = root.getBoundingClientRect();
      const cx = Math.max(0, Math.min(x, window.innerWidth - width));
      const cy = Math.max(0, Math.min(y, window.innerHeight - height));
      root.style.left = `${cx}px`;
      root.style.top = `${cy}px`;
    },
    close,
    isOpen: () => open,
    contains: (t) => t instanceof Node && root.contains(t),
    destroy() {
      close();
      window.removeEventListener("keydown", onKey);
      window.removeEventListener("pointerdown", onAway);
      window.removeEventListener("blur", onBlur);
      root.remove();                           // style tag stays (id-guarded)
    },
  };
}
