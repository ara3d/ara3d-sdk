// Track B — standalone harness for viewer.html. Loads a real .bos and exercises
// the ViewerBridge + Panes contracts end to end. No dependency on the host being up.

import { createViewer } from "./index";
import { levelView, wallsView } from "./views";
import { createPanes } from "../panes/index";
import type { Cell, ModelData, TableValue } from "../contracts";

const el = (id: string) => document.getElementById(id)!;

// ------------------------------------------------------------- model source

interface ModelEntry {
  id: string;
  name: string;
  bosUrl: string;
}

/** Host first, then ?bos=, then the bundled sample copy. */
async function resolveModel(): Promise<ModelEntry> {
  const q = new URLSearchParams(location.search).get("bos");
  if (q) return { id: "query", name: q, bosUrl: q };

  try {
    const res = await fetch("/api/models", { signal: AbortSignal.timeout(1500) });
    if (res.ok) {
      const list = (await res.json()) as ModelEntry[];
      if (Array.isArray(list) && list.length > 0) {
        console.log("[harness] using host model list", list);
        return list[0];
      }
    }
  } catch {
    console.log("[harness] /api/models unavailable — falling back to /sample.bos");
  }
  return { id: "sample", name: "rac_basic (bundled sample)", bosUrl: "/sample.bos" };
}

// ----------------------------------------------------------------------- ui

function toolbar(host: HTMLElement) {
  const bar = document.createElement("div");
  bar.style.cssText =
    "position:absolute;top:8px;left:8px;z-index:10;display:flex;gap:6px;align-items:center;" +
    "background:rgba(20,20,22,.85);padding:6px 8px;border-radius:8px;font:12px system-ui,sans-serif";
  host.appendChild(bar);

  const button = (label: string, fn: () => void) => {
    const b = document.createElement("button");
    b.textContent = label;
    b.style.cssText =
      "background:#fff;border:1px solid #D6D3CC;color:#1A1A18;padding:5px 9px;border-radius:6px;cursor:pointer;font:12px var(--pf-font)";
    b.addEventListener("click", fn);
    bar.appendChild(b);
    return b;
  };

  const status = document.createElement("span");
  status.style.cssText = "color:#C9C7C0;margin-left:8px;max-width:60vw";
  bar.appendChild(status);

  const swatches = document.createElement("div");
  swatches.style.cssText =
    "position:absolute;top:52px;left:8px;z-index:10;display:none;flex-direction:column;gap:2px;" +
    "background:rgba(20,20,22,.85);padding:6px 8px;border-radius:8px;font:11px system-ui,sans-serif;color:#ddd;max-height:50vh;overflow:auto";
  host.appendChild(swatches);

  return { button, status, swatches };
}

// --------------------------------------------------------------------- main

async function main() {
  const viewerEl = el("viewer");
  const bridge = createViewer(viewerEl);
  const ui = toolbar(viewerEl);
  const panes = createPanes(el("grid"), el("legend"), ui.status);

  (window as any).viewerBridge = bridge;
  (window as any).panes = panes;
  bridge.setPhaseReporter((m) => panes.setStatus(m));

  const entry = await resolveModel();
  panes.setStatus(`loading ${entry.name} …`);

  let model: ModelData;
  try {
    model = await bridge.load(entry.bosUrl, entry.id);
  } catch (e) {
    panes.setStatus(`load failed: ${String(e)}`);
    console.error("[harness] load failed", e);
    return;
  }
  (window as any).model = model;

  const typeCounts = new Map<string, number>();
  for (const t of model.types) typeCounts.set(t, (typeCounts.get(t) ?? 0) + 1);
  console.log(
    "[harness] top types:",
    [...typeCounts.entries()].sort((a, b) => b[1] - a[1]).slice(0, 12)
  );
  console.log("[harness] levels:", [...new Set(model.levels)].slice(0, 20));
  console.log("[harness] first 5 params:", model.paramNames().slice(0, 5));

  panes.setStatus(
    `${entry.name} — ${model.entityCount} entities, ${model.paramNames().length} params`
  );

  // ---- entity table (first 100 drawable entities), row click highlights.
  // Entity 0..N of a BOS model are mostly non-geometric (materials, views, levels,
  // family types), so listing raw indices gives 100 rows that highlight nothing.
  const drawable = bridge.geometryEntities();
  console.log(
    `[harness] ${drawable.length} of ${model.entityCount} entities have geometry`
  );
  const rows: Cell[][] = [];
  for (let k = 0; k < Math.min(100, drawable.length); k++) {
    const i = drawable[k];
    rows.push([i, model.globalIds[i], model.types[i], model.names[i], model.levels[i]]);
  }
  const table: TableValue = {
    columns: ["#", "GlobalId", "Type", "Name", "Level"],
    rows,
  };
  panes.showTable(table, {
    onRowClick: (row) => {
      const i = Number(row[0]);
      bridge.highlight(Uint32Array.of(i));
      panes.setStatus(`highlight entity ${i} — ${row[2]} "${row[3]}" (${row[1]})`);
    },
  });

  panes.showLegend({ ramp: "viridis", min: 0, max: 1 }, "viridis (demo)");

  // ---- picking
  bridge.onPick((entity) => {
    if (entity == null) {
      bridge.highlight(null);
      panes.setStatus("picked: nothing");
      console.log("[harness] pick -> null");
      return;
    }
    bridge.highlight(Uint32Array.of(entity));
    const msg = `picked entity ${entity} — ${model.types[entity]} "${model.names[entity]}" ` +
      `level=${model.levels[entity] ?? "-"} id=${model.globalIds[entity]}`;
    panes.setStatus(msg);
    console.log("[harness] " + msg);
  });

  // ---- buttons
  ui.button("Walls red / ghost rest", () => {
    const view = wallsView(model);
    bridge.applyView(view);
    ui.swatches.style.display = "none";
    panes.setStatus(`walls: ${view.entities.length} entities red, rest ghosted`);
  });

  ui.button("Color by level", () => {
    const { view, legend } = levelView(model);
    bridge.applyView(view);
    ui.swatches.replaceChildren(
      ...legend.map(([name, css]) => {
        const d = document.createElement("div");
        d.style.cssText = "display:flex;gap:6px;align-items:center";
        const sw = document.createElement("span");
        sw.style.cssText = `width:12px;height:12px;background:${css};display:inline-block;border:1px solid #000`;
        d.append(sw, document.createTextNode(name));
        return d;
      })
    );
    ui.swatches.style.display = "flex";
    panes.setStatus(`levels: ${legend.length} distinct`);
  });

  ui.button("Clear", () => {
    bridge.applyView(null);
    bridge.highlight(null);
    ui.swatches.style.display = "none";
    panes.setStatus("default view");
  });

  console.log("[harness] ready — window.viewerBridge / window.model available");
  (window as any).harnessReady = true;
}

main().catch((e) => {
  console.error("[harness] fatal", e);
});
