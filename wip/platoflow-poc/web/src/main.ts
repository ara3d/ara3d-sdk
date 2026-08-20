// Track E (supervisor) integration entry. Wires editor (A) + viewer/panes (B) +
// evaluator (C) + host (D) into the five-beat demo app.
import "./theme"; // register + snap the cream gratify palette before any island mounts
import { createTour, type TourStep } from "./tour";
import { showToast } from "./editor/toast";
import type {
  Cell, EvalCtx, EvalResult, GraphDoc, GraphNode, Intent, ModelData,
  SceneValue, TableValue, ColormapValue, ViewValue, Value,
} from "./contracts";
import { KINDS } from "./kinds";
import { createEditor } from "./editor";
import { createViewer } from "./viewer";
import { createPanes } from "./panes";
import { installSplitters } from "./panes/splitter";
import { evaluateGraph } from "./flow/evaluate";
import { onGridRowClick, tablePreviewOf } from "./editor/widgets";
import { onWireHover, setWireHighlight } from "./editor/wires";
import { buildPsetRows } from "./flow/nodes";
import { kindsJson } from "./flow";

const statusEl = document.getElementById("status")!;
let statusSink: ((s: string) => void) | null = null;
const say = (s: string) => { statusEl.textContent = s; statusSink?.(s); };

interface ModelEntry { id: string; name: string; bosUrl: string; ifcPath?: string; }

async function boot() {
  say("connecting to host…");
  let models: ModelEntry[] = [];
  try {
    models = await (await fetch("/api/models")).json();
    fetch("/api/kinds", {
      method: "POST", headers: { "content-type": "application/json" }, body: kindsJson(),
    }).catch(() => {});
  } catch {
    say("host not reachable on :5214 — start it with `dotnet run --project host`");
  }

  // T19: restore the persisted pane split BEFORE the canvases mount so they
  // size to the right tracks on first paint.
  installSplitters(document.getElementById("app")!);

  const viewer = createViewer(document.getElementById("viewer")!);
  const panes = createPanes(document.getElementById("grid")!, document.getElementById("legend")!);
  const loaded = new Map<string, ModelData>();

  const ctx: EvalCtx = {
    async loadModel(id) {
      const hit = loaded.get(id);
      if (hit) return hit;
      const entry = models.find(m => m.id === id);
      if (!entry) throw new Error(`unknown model '${id}'`);
      say(`loading ${entry.name}…`);
      const md = await viewer.load(entry.bosUrl, id);
      loaded.set(id, md);
      say(`${entry.name}: ${md.entityCount} entities`);
      return md;
    },
    async sql(modelId, query) {
      const r = await (await fetch("/api/sql", {
        method: "POST", headers: { "content-type": "application/json" },
        body: JSON.stringify({ model: modelId, sql: query }),
      })).json();
      if (r.error) throw new Error(r.error);
      return r as TableValue;
    },
    fetchText: async (url) => await (await fetch(url)).text(),
    listModels: async () => models.map(m => ({ id: m.id, name: m.name })),
    async ask(modelId, question) {
      const r = await (await fetch("/api/ask", {
        method: "POST", headers: { "content-type": "application/json" },
        body: JSON.stringify({ model: modelId, question }),
      })).json();
      if (r.error) throw new Error(r.error);
      return r as { sql: string };
    },
  };

  let lastResult: EvalResult | null = null;
  let lastDoc: GraphDoc | null = null;
  let selectedNodeId: string | null = null;

  const isScene = (v: Value | undefined): v is SceneValue => !!v && "entities" in (v as any) && "channels" in (v as any);
  const isView = (v: Value | undefined): v is ViewValue => !!v && "ghostOthers" in (v as any);
  const isTable = (v: Value | undefined): v is TableValue => !!v && "rows" in (v as any);
  const isColormap = (v: Value | undefined): v is ColormapValue => !!v && "ramp" in (v as any);

  // W13: selection drives the 3D view. doc.display stays as the pinned
  // fallback (MCP still sets it); when neither points at a drawable value the
  // LAST view/scene-producing node shows — so wiring a Color By is instantly
  // visible without hunting for a display toggle.
  const shownId = (doc: GraphDoc, res: EvalResult): string | null => {
    const drawable = (id: string | null | undefined): string | null => {
      if (!id) return null;
      const v = res.values.get(id);
      return isView(v) || isScene(v) ? id : null;
    };
    return drawable(selectedNodeId) ?? drawable(doc.display) ??
      [...doc.nodes].reverse().map((n) => drawable(n.id)).find(Boolean) ?? null;
  };

  function routeOutputs(doc: GraphDoc, res: EvalResult) {
    // 3D display: selected > pinned > last drawable (see shownId above)
    const shown = shownId(doc, res);
    const dv = shown ? res.values.get(shown) : undefined;
    if (isView(dv)) viewer.applyView(dv);
    else if (isScene(dv)) {
      const colors = new Float32Array(dv.entities.length * 3);
      for (let i = 0; i < dv.entities.length; i++) { colors[i * 3] = 0.31; colors[i * 3 + 1] = 0.76; colors[i * 3 + 2] = 0.97; }
      viewer.applyView({ model: dv.model, entities: dv.entities, colors, ghostOthers: true, label: "selection" });
    } else viewer.applyView(null);

    // Legend (W9-D): showViewLegend draws the EFFECTIVE numeric domain or the
    // categorical swatches straight off the displayed ViewValue; the old
    // ColormapValue path stays as the fallback for panes without it.
    if (panes.showViewLegend) {
      panes.showViewLegend(isView(dv) ? dv : null);
    } else {
      let legend: ColormapValue | null = null; let legendLabel: string | undefined;
      if (isView(dv) && dv.domain && dv.ramp) {
        legend = { ramp: dv.ramp as ColormapValue["ramp"], min: dv.domain[0], max: dv.domain[1] };
        legendLabel = dv.label;
      } else if (shown) {
        for (const e of doc.edges) {
          if (e.to.node === shown) {
            const v = res.values.get(e.from.node);
            if (isColormap(v)) { legend = v; legendLabel = (dv as ViewValue | undefined)?.label; }
          }
        }
      }
      panes.showLegend(legend, legendLabel);
    }

    updateGrid();
  }

  const projectScene = (s: SceneValue): TableValue => {
    const chans = Object.keys(s.channels);
    const columns = ["GlobalId", "Type", "Name", "Level", ...chans];
    const rows: Cell[][] = [];
    const n = Math.min(s.entities.length, 500);
    for (let i = 0; i < n; i++) {
      const e = s.entities[i];
      rows.push([s.model.globalIds[e], s.model.types[e], s.model.names[e], s.model.levels[e],
        ...chans.map(c => s.channels[c].values[e] ?? null)]);
    }
    return { columns, rows };
  };

  // T11: wire-hover ghosts an entity set; hover-out restores the last row-click
  // highlight, so the two highlight sources compose instead of clobbering.
  let lastRowHighlight: Uint32Array | null = null;
  const setRowHighlight = (u: Uint32Array | null) => { lastRowHighlight = u; viewer.highlight(u); };

  const rowClicker = (table: TableValue) => (row: Cell[]) => {
    if (!lastDoc || !lastResult) return;
    const model = firstModel(lastDoc, lastResult);
    if (!model) return;
    const gidCol = table.columns.indexOf("GlobalId");
    const lvlCol = table.columns.indexOf("Level");
    if (gidCol >= 0) {
      const idx = model.globalIds.indexOf(String(row[gidCol]));
      setRowHighlight(idx >= 0 ? new Uint32Array([idx]) : null);
    } else if (lvlCol >= 0) {
      const cell = row[lvlCol];
      const lvl = cell == null || cell === "" ? null : String(cell);
      const idxs: number[] = [];
      model.levels.forEach((l, i) => { if (l === lvl) idxs.push(i); });
      setRowHighlight(new Uint32Array(idxs));
    }
  };

  // Selection-driven preview: a selected node's output wins the grid; otherwise
  // the first Data Grid sink shows.
  function updateGrid() {
    if (!lastDoc || !lastResult) return;
    let table: TableValue | null = null;
    if (selectedNodeId) {
      const v = lastResult.values.get(selectedNodeId);
      if (isTable(v)) table = v;
      else if (isScene(v)) table = projectScene(v);
    }
    if (!table) {
      const sink = lastDoc.nodes.find(n => n.kind === "sink.table" && isTable(lastResult!.values.get(n.id)));
      table = sink ? lastResult.values.get(sink.id) as TableValue : null;
    }
    panes.showTable(table, table ? { onRowClick: rowClicker(table) } : undefined);
  }

  function firstModel(doc: GraphDoc, res: EvalResult): ModelData | null {
    for (const n of doc.nodes) {
      const v = res.values.get(n.id);
      if (isScene(v)) return v.model;
      if (isView(v)) return v.model;
    }
    return null;
  }

  function pushState(doc: GraphDoc, res: EvalResult) {
    const results: Record<string, unknown> = {};
    for (const [id, st] of res.status) {
      const v = res.values.get(id);
      results[id] = {
        ...st,
        table: isTable(v) ? { columns: v.columns, rows: v.rows.slice(0, 50) } : undefined,
      };
    }
    fetch("/api/state", {
      method: "POST", headers: { "content-type": "application/json" },
      body: JSON.stringify({ doc, results }),
    }).catch(() => {});
  }

  let evalTimer: number | undefined;
  const onDocChange = (doc: GraphDoc) => {
    lastDoc = doc;
    clearTimeout(evalTimer);
    evalTimer = window.setTimeout(async () => {
      try {
        const res = await evaluateGraph(doc, ctx);
        // T9: table-outputting nodes carry a small page for the in-node
        // mini-grid body (kinds without bodyHeight simply never draw it).
        for (const [id, st] of res.status) {
          const v = res.values.get(id);
          if (isTable(v)) st.tablePreview = tablePreviewOf(v);
        }
        lastResult = res;
        editor.setResults(res);
        routeOutputs(doc, res);
        pushState(doc, res);
      } catch (e) {
        say(`eval failed: ${(e as Error).message}`);
        showToast(`eval failed: ${(e as Error).message}`, "error");
      }
    }, 60);
  };

  function enumOptions(node: GraphNode, _param: string, source: string): string[] {
    if (source === "models") return models.map(m => m.id);
    if (!lastDoc || !lastResult) return [];
    const inputVals = lastDoc.edges.filter(e => e.to.node === node.id)
      .map(e => lastResult!.values.get(e.from.node));
    const scene = inputVals.find(isScene) as SceneValue | undefined;
    const table = inputVals.find(isTable) as TableValue | undefined;
    switch (source) {
      case "types": return scene ? [...new Set(scene.model.types)].sort() : [];
      case "levels": return scene ? [...new Set(scene.model.levels.filter((l): l is string => !!l))].sort() : [];
      case "parameters": return scene ? scene.model.paramNames().map(p => p.name) : [];
      case "channels": return scene ? [...Object.keys(scene.channels), ...scene.model.paramNames().filter(p => p.numeric).map(p => p.name)] : [];
      case "columns": return table ? table.columns : [];
      // W9: group.by keys — built-ins first, then channels, then every parameter.
      case "groupKeys": return scene
        ? ["Type", "Level", ...Object.keys(scene.channels), ...scene.model.paramNames().map(p => p.name)]
        : [];
      default: return [];
    }
  }

  const runEffect = async (nodeId: string) => {
    if (!lastDoc || !lastResult) return;
    const node = lastDoc.nodes.find(n => n.id === nodeId);
    if (!node) return;

    // W9-E: CSV export — the node's evaluate is pure; the Run button does the write.
    if (node.kind === "sink.exportCsv") {
      const t = lastResult.values.get(nodeId);
      if (!isTable(t)) { say("export-csv: no table input"); return; }
      const name = String(node.params.filename ?? "export.csv");
      say(`exporting ${t.rows.length} rows…`);
      try {
        const r = await (await fetch("/api/export-csv", {
          method: "POST", headers: { "content-type": "application/json" },
          body: JSON.stringify({ name, table: { columns: t.columns, rows: t.rows } }),
        })).json();
        say(r.error ? `export failed: ${r.error}` : `wrote ${r.rows} rows → ${r.outPath}`);
        showToast(r.error ? `export failed: ${r.error}` : `Exported ${r.rows} rows → ${r.outPath}`, r.error ? "error" : "ok");
      } catch (e) { say(`export failed: ${(e as Error).message}`); showToast(`export failed: ${(e as Error).message}`, "error"); }
      return;
    }

    const v = lastResult.values.get(nodeId);
    if (!isScene(v)) { say("write-pset: no scene input"); return; }
    const channels = String(node.params.channels ?? "").split(",").map(s => s.trim()).filter(Boolean);
    const rows = buildPsetRows(v, channels);
    say(`writing ${rows.length} psets…`);
    try {
      const r = await (await fetch("/api/append-psets", {
        method: "POST", headers: { "content-type": "application/json" },
        body: JSON.stringify({ model: v.model.id, psetName: node.params.psetName ?? "Ara3D_Analytics", rows }),
      })).json();
      say(r.error ? `pset write failed: ${r.error}` : `wrote ${r.entitiesAdded ?? "?"} entities → ${r.outPath} (${r.diffSummary ?? ""})`);
      showToast(r.error ? `pset write failed: ${r.error}`
        : `Wrote ${r.entitiesAdded ?? "?"} entities → ${r.outPath} (${r.diffSummary ?? ""})`, r.error ? "error" : "ok", r.error ? undefined : 8000);
    } catch (e) { say(`pset write failed: ${(e as Error).message}`); showToast(`pset write failed: ${(e as Error).message}`, "error"); }
  };

  const demos = ["carbon-walls", "carbon-by-level", "sql-explore", "write-pset",
    "whatif-expr", "inspect-flow", "filter-sort",
    "quality-audit", "cost-estimate", "disciplines",
    "group-color", "set-algebra",
    "door-egress-check", "wall-stats-explore", "simple-color",
    "wall-roles-overlap", "level-takeoff-export", "hvac-composition",
    "checklist-live", "massing-boxes", "explode-levels"];

  // Shared by the Examples menu and the walkthrough: host copy first (saved
  // graphs live there), static /demo copy as the no-host fallback.
  const loadExample = async (d: string): Promise<void> => {
    try {
      const doc = await (await fetch(`/api/graph?name=${encodeURIComponent(d)}`)).json();
      if (doc && !doc.error) {
        editor.dispatch({ t: "load", doc } as Intent);
        editor.tidy();                           // auto-layout, then animate into view
        return;
      }
    } catch { /* fall through to the static copy */ }
    const doc = await (await fetch(`/demo/${d}.json`)).json();
    editor.dispatch({ t: "load", doc } as Intent);
    editor.tidy();
  };

  // ── walkthrough (Help ▸ Run walkthrough) ───────────────────────────────────
  // The five demo beats as a guided tour; steps load real examples, so the
  // user watches each feature happen and can poke at it before moving on.
  const openAddNodePanel = () => {
    const panel = document.querySelector(".pf-palette-panel");
    if (panel && !panel.classList.contains("pf-open"))
      document.querySelector<HTMLButtonElement>(".pf-view-addnode")?.click();
  };
  const tourSteps: TourStep[] = [
    {
      title: "Welcome to Studio Graph",
      body: "A dataflow graph over BIM models: nodes on the canvas compute selections, tables and colorings; the 3D view and data grid on the right follow your selection live. The app stays fully interactive during this tour — poke at anything, then hit Next.",
    },
    {
      title: "Wire it",
      body: "carbon-walls: Load Model → By Type (walls) → a CSV join attaches embodied-carbon numbers → Colormap → Color By. Click any node to preview its output in 3D and the grid; with nothing selected the final Color By view shows — recolored by carbon, legend and counts on the right.",
      run: () => loadExample("carbon-walls"),
    },
    {
      title: "Scrub it",
      body: "On the Colormap card, toggle auto off and drag the ramp's min/max thumbs: the recolor is instant — the interactive tier runs entirely in the browser, no host round-trip, no spinner.",
    },
    {
      title: "Reduce it",
      body: "carbon-by-level: Group By (level) → Aggregate (sum) → Table. The datagrid under the 3D view shows the result; click a row to highlight that storey in 3D.",
      run: () => loadExample("carbon-by-level"),
    },
    {
      title: "Ask it in SQL",
      body: "sql-explore: a DuckDB query over the model is just another node — its table output feeds the same downstream nodes as any other. Click the SQL node to see its output in the grid.",
      run: () => loadExample("sql-explore"),
    },
    {
      title: "Store it",
      body: "write-pset: the Write Pset sink patches computed values back into the IFC file. Press its ▶ Run button to write — the status bar reports the byte-exact diff summary.",
      run: () => loadExample("write-pset"),
    },
    {
      title: "Build your own",
      body: "Add nodes from the Add Node panel (P) or right-click the canvas to add in place. Click any node to preview its output in the grid; click a param row to edit it. Help ▸ Show instructions lists every shortcut. That's the tour!",
      run: openAddNodePanel,
    },
  ];
  const tour = createTour(tourSteps);

  const editor = createEditor(
    document.getElementById("graph-canvas") as HTMLCanvasElement, KINDS, {
      onDocChange,
      getEnumOptions: enumOptions,
      onRunEffect: runEffect,
      // W13: selection re-routes ALL outputs (3D + legend + grid), not just the grid.
      onSelect: (id) => {
        selectedNodeId = id;
        if (lastDoc && lastResult) routeOutputs(lastDoc, lastResult);
        else updateGrid();
      },
      chrome: {
        title: "Studio Graph",
        examples: demos,
        // W9-E: the demo folder + data/graphs ARE the registry (design §7);
        // the static list above is only the no-host fallback.
        getExamples: async () => {
          try {
            const r = await (await fetch("/api/graphs")).json();
            if (Array.isArray(r.demos)) return { demos: r.demos, saved: r.saved ?? [] };
          } catch { /* host down */ }
          return { demos, saved: [] };
        },
        onExample: (d) => void loadExample(d),
        onWalkthrough: () => tour.start(),
        onSaveGraph: async () => {
          if (!lastDoc) return;
          const name = window.prompt("Save graph as:", lastDoc.name || "my-graph");
          if (!name) return;
          try {
            const r = await (await fetch("/api/graphs", {
              method: "POST", headers: { "content-type": "application/json" },
              body: JSON.stringify({ name, doc: lastDoc }),
            })).json();
            say(r.error ? `save failed: ${r.error}` : `saved "${r.name}"`);
            showToast(r.error ? `save failed: ${r.error}` : `Saved "${r.name}"`, r.error ? "error" : "ok");
          } catch (e) { say(`save failed: ${(e as Error).message}`); showToast(`save failed: ${(e as Error).message}`, "error"); }
        },
      },
    });
  statusSink = (s) => editor.setStatus?.(s);

  // T11: hovering a scene wire ghosts its entity set in 3D; hover-out restores.
  onWireHover((sourceNode) => {
    const v = sourceNode ? lastResult?.values.get(sourceNode) : undefined;
    if (v && isScene(v)) viewer.highlight(v.entities);
    else viewer.highlight(lastRowHighlight);
  });

  // W10: clicking a node's mini-grid row highlights that row's element(s) in
  // 3D — same resolution as the data-grid pane (rowClicker: GlobalId column →
  // single entity; Level column fallback → all on level; via setRowHighlight,
  // so wire-hover-out restores it). NOTE: status.tablePreview is a ≤29-row
  // PAGE of the node's real table — the fired rowIndex indexes the preview,
  // which is exactly the row the user clicked.
  onGridRowClick((nodeId, rowIndex) => {
    const preview = lastResult?.status.get(nodeId)?.tablePreview;
    const row = preview?.rows[rowIndex];
    if (preview && row) rowClicker(preview)(row);
  });

  // W9: the reverse link (design §4.4) — picking an element in 3D highlights
  // every wire whose upstream value carries it; empty pick clears.
  const sortedHas = (a: Uint32Array, x: number): boolean => {
    let lo = 0, hi = a.length - 1;
    while (lo <= hi) {
      const mid = (lo + hi) >> 1;
      if (a[mid] === x) return true;
      if (a[mid] < x) lo = mid + 1; else hi = mid - 1;
    }
    return false;
  };
  viewer.onPick((entityIndex) => {
    if (entityIndex === null || !lastDoc || !lastResult) { setWireHighlight(null); return; }
    const carries = (v: Value | undefined): boolean =>
      (isScene(v) || isView(v)) && sortedHas(v.entities, entityIndex);
    const keys = lastDoc.edges
      .filter(e => carries(lastResult!.values.get(e.from.node)))
      .map(e => `${e.from.node}.${e.from.slot}->${e.to.node}.${e.to.slot}`);
    setWireHighlight(keys.length ? keys : null);
  });

  // MCP agent intent polling. W9: fast-forward past the queue's history at
  // boot — a fresh session must not replay every intent any agent ever sent
  // (wave-1 finding #5: stale connects clobber freshly loaded demos via the
  // one-wire-per-input rule). Only intents issued AFTER this session starts
  // apply; the real design's session model supersedes this.
  let since = 0;
  try {
    const r0 = await (await fetch("/api/intents?since=0")).json();
    if (typeof r0.now === "number") since = r0.now;
  } catch { /* host down; keep 0 */ }
  setInterval(async () => {
    try {
      const r = await (await fetch(`/api/intents?since=${since}`)).json();
      for (const { seq, intent } of r.intents ?? []) {
        since = Math.max(since, seq);
        editor.dispatch(intent as Intent);
        // W13: an agent-pushed graph arrives framed, same as the Examples menu.
        if ((intent as Intent)?.t === "load") editor.fitToFrame();
      }
      if (typeof r.now === "number") since = Math.max(since, r.now - (r.intents?.length ? 0 : 0));
    } catch { /* host down; keep quiet */ }
  }, 300);

  say(models.length ? "ready — pick a demo above or right-click to add nodes" : "ready (no host)");
  (window as any).poc = {
    editor, viewer, panes, runEffect,
    get doc() { return lastDoc; }, get result() { return lastResult; },
  };
}

boot();
