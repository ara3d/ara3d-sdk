// Track B — HTML data panes: table grid, colormap legend, status line.
// Plain DOM, no framework. Modelled on ara3d-webgl's examples/parameterGrid.js.

import type { Cell, ColormapValue, Panes, TableValue, ViewValue } from "../contracts";
// The legend MUST sample the same function the geometry colours do, or it lies.
// Track C owns that function (`flow/nodes.ts → ramp`); this is a read-only import.
import { ramp } from "../flow/nodes";

const MAX_ROWS = 500;

type Rgb = [number, number, number];

/** rgb triplet (0..1, the wire convention) as a CSS color. */
export const rgbCss = ([r, g, b]: Rgb): string =>
  `rgb(${Math.round(r * 255)},${Math.round(g * 255)},${Math.round(b * 255)})`;

/** Samples a named ramp at t in 0..1. Thin alias so callers do not reach into flow/. */
export const sampleRamp = (name: ColormapValue["ramp"], t: number): Rgb =>
  ramp(name, t) as Rgb;

export const rampCss = (name: ColormapValue["ramp"], t: number): string =>
  rgbCss(sampleRamp(name, t));

// --------------------------------------------------------------- legend model

/** What the legend pane will draw — pure, so specs assert on it without pixels.
 *  ticks run top-to-bottom (max, mid, min), pre-formatted. */
export type LegendPlan =
  | { kind: "none" }
  | { kind: "categorical"; title?: string; entries: { label: string; css: string }[] }
  | { kind: "numeric"; title: string; ramp: ColormapValue["ramp"];
      ticks: [string, string, string] };

const numericPlan = (c: ColormapValue, title?: string): LegendPlan => ({
  kind: "numeric",
  title: title ?? c.ramp,
  ramp: c.ramp,
  ticks: [fmt(c.max), fmt((c.min + c.max) / 2), fmt(c.min)],
});

/** Render plan for a displayed view. Categorical swatches win over the numeric
 *  gradient (a categorical view's domain would be meaningless); numeric needs the
 *  EFFECTIVE domain+ramp the view actually used — never the configured min/max. */
export function legendModel(v: ViewValue | null): LegendPlan {
  if (!v) return { kind: "none" };
  if (v.legend) return {
    kind: "categorical",
    title: v.label,
    entries: v.legend.map(e => ({ label: e.label, css: rgbCss(e.color) })),
  };
  if (v.domain && v.ramp) return numericPlan(
    { ramp: v.ramp as ColormapValue["ramp"], min: v.domain[0], max: v.domain[1] },
    v.label,
  );
  return { kind: "none" };
}

// ---------------------------------------------------------------------- panes

const isNumeric = (c: Cell) => typeof c === "number";

export function createPanes(
  gridEl: HTMLElement,
  legendEl: HTMLElement,
  statusEl?: HTMLElement
): Panes {
  gridEl.classList.add("pf-grid");
  legendEl.classList.add("pf-legend");
  injectStyles();

  const showTable: Panes["showTable"] = (t, opts) => {
    gridEl.replaceChildren();
    if (!t) return;

    const table = document.createElement("table");
    const thead = document.createElement("thead");
    const hrow = document.createElement("tr");
    for (const c of t.columns) {
      const th = document.createElement("th");
      th.textContent = c;
      hrow.appendChild(th);
    }
    thead.appendChild(hrow);
    table.appendChild(thead);

    // Column alignment is decided from the first row that has a value.
    const numericCol = t.columns.map((_, ci) => {
      for (const row of t.rows) if (row[ci] != null) return isNumeric(row[ci]);
      return false;
    });

    const tbody = document.createElement("tbody");
    const shown = Math.min(t.rows.length, MAX_ROWS);
    for (let r = 0; r < shown; r++) {
      const row = t.rows[r];
      const tr = document.createElement("tr");
      for (let c = 0; c < t.columns.length; c++) {
        const td = document.createElement("td");
        const v = row[c];
        td.textContent = v == null ? "" : String(v);
        if (numericCol[c]) td.className = "num";
        tr.appendChild(td);
      }
      if (opts?.onRowClick) {
        tr.classList.add("clickable");
        tr.addEventListener("click", () => {
          for (const el of tbody.querySelectorAll("tr.sel")) el.classList.remove("sel");
          tr.classList.add("sel");
          opts.onRowClick!(row);
        });
      }
      tbody.appendChild(tr);
    }
    table.appendChild(tbody);
    gridEl.appendChild(table);

    if (t.rows.length > shown) {
      const more = document.createElement("div");
      more.className = "pf-more";
      more.textContent = `… ${t.rows.length - shown} more rows (showing first ${shown})`;
      gridEl.appendChild(more);
    }
  };

  const renderLegend = (plan: LegendPlan) => {
    legendEl.replaceChildren();
    if (plan.kind === "none") return;

    const wrap = document.createElement("div");
    wrap.className = "pf-legend-wrap";

    if (plan.title) {
      const title = document.createElement("div");
      title.className = "pf-legend-title";
      title.textContent = plan.title;
      wrap.appendChild(title);
    }

    if (plan.kind === "numeric") {
      const body = document.createElement("div");
      body.className = "pf-legend-body";

      // Vertical bar: max at the top, so stop 0 (min) goes last.
      const steps = 32;
      const stops: string[] = [];
      for (let i = 0; i <= steps; i++) stops.push(rampCss(plan.ramp, 1 - i / steps));
      const bar = document.createElement("div");
      bar.className = "pf-legend-bar";
      bar.style.background = `linear-gradient(to bottom, ${stops.join(",")})`;
      body.appendChild(bar);

      const ticks = document.createElement("div");
      ticks.className = "pf-legend-ticks";
      for (const t of plan.ticks) {
        const d = document.createElement("div");
        d.textContent = t;
        ticks.appendChild(d);
      }
      body.appendChild(ticks);
      wrap.appendChild(body);
    } else {
      const list = document.createElement("div");
      list.className = "pf-legend-swatches";
      for (const e of plan.entries) {
        const row = document.createElement("div");
        row.className = "pf-legend-swatch";
        const chip = document.createElement("span");
        chip.className = "pf-legend-chip";
        chip.style.background = e.css;
        row.appendChild(chip);
        const label = document.createElement("span");
        label.className = "pf-legend-swatch-label";
        label.textContent = e.label;
        label.title = e.label;
        row.appendChild(label);
        list.appendChild(row);
      }
      wrap.appendChild(list);
    }

    legendEl.appendChild(wrap);
  };

  const showLegend: Panes["showLegend"] = (c, label) =>
    renderLegend(c ? numericPlan(c, label) : { kind: "none" });

  const showViewLegend: NonNullable<Panes["showViewLegend"]> = (v) =>
    renderLegend(legendModel(v));

  const setStatus: Panes["setStatus"] = (msg) => {
    if (statusEl) statusEl.textContent = msg;
    else console.log(`[status] ${msg}`);
  };

  return { showTable, showLegend, showViewLegend, setStatus };
}

const fmt = (v: number): string => {
  if (!Number.isFinite(v)) return String(v);
  const a = Math.abs(v);
  if (a === 0) return "0";
  if (a >= 1000 || a < 0.01) return v.toExponential(2);
  return v.toFixed(a >= 100 ? 0 : 2);
};

let stylesInjected = false;
function injectStyles() {
  if (stylesInjected) return;
  stylesInjected = true;
  const css = `
.pf-grid { font: 12px var(--pf-font); font-variant-numeric: tabular-nums; }
.pf-grid table { border-collapse: collapse; width: 100%; }
.pf-grid th { position: sticky; top: 0; background: var(--pf-surface); color: var(--pf-dim);
  font-weight: 500; text-align: left; padding: 4px 8px; border-bottom: 1px solid var(--pf-border); white-space: nowrap; }
.pf-grid td { padding: 3px 8px; border-bottom: 1px solid var(--pf-border); color: var(--pf-text);
  white-space: nowrap; max-width: 340px; overflow: hidden; text-overflow: ellipsis; }
.pf-grid td.num { text-align: right; font-variant-numeric: tabular-nums; }
.pf-grid tr.clickable { cursor: pointer; }
.pf-grid tr.clickable:hover td { background: var(--pf-hover); }
.pf-grid tr.sel td { background: var(--pf-active); color: var(--pf-text); }
.pf-more { padding: 4px 8px; color: var(--pf-dim); }
.pf-legend-title { color: var(--pf-text); margin-bottom: 6px; font: 11px var(--pf-font); }
.pf-legend-body { display: flex; gap: 6px; height: 160px; }
.pf-legend-bar { width: 18px; border: 1px solid var(--pf-border-strong); }
.pf-legend-ticks { display: flex; flex-direction: column; justify-content: space-between;
  font: 10px ui-monospace, Consolas, monospace; color: var(--pf-dim); }
.pf-legend-swatches { display: flex; flex-direction: column; gap: 2px;
  max-height: 220px; overflow-y: auto; padding-right: 4px; }
.pf-legend-swatch { display: flex; align-items: center; gap: 6px;
  font: 11px var(--pf-font); color: var(--pf-text); line-height: 14px; }
.pf-legend-chip { width: 12px; height: 12px; flex: none;
  border: 1px solid var(--pf-border-strong); border-radius: 2px; }
.pf-legend-swatch-label { white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
`;
  const el = document.createElement("style");
  el.textContent = css;
  document.head.appendChild(el);
}
