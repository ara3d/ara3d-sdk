# Agent orientation — PlatoFlow PoC

Throwaway-but-working prototype of the PlatoFlow × IFC design. You are probably here to
extend it. Read in this order: this file, `CONTRACTS.md` (interfaces), `NOTES.md`
(accumulated findings — append yours), and for design intent
`../../../../docs/platoflow-ifc-design.md` (+ `platoflow-design-principles.md`, which wins
conflicts).

## Run + verify (do this first, before changing anything)

```
dotnet run --project host          # C# host :5214 (first run converts duplex.ifc, ~45s)
cd web && npm install && npm run dev   # vite :5215
```

Baseline gates — all must be green before AND after your change:
- `cd web && npm run check` (tsc) and `npx vitest run` (~500 specs, <10s — this is
  the inner loop: layout goldens, headless-runtime interaction specs, jsdom DOM
  organs, undo, flow, demos)
- `node tools/edgate-smoke.mjs` (5 editor browser checks, ~3s; boots its own vite
  on :5216 if none is up)
- `node tools/intgate-smoke.mjs http://localhost:5215/` (13 integration checks,
  ~8s, both servers up)
- `host/smoke.ps1` if you touched the host (kill the host before `dotnet build` — a
  running exe locks `bin/`)

The old fat gates (edgate.mjs 24 checks, intgate.mjs 30 checks, minutes each) were
retired at the UX-plan W1 join — their behaviors live in vitest now (see the
retirement list in CONTRACTS.md). Rules for any new browser check: wait only on
data the new state uniquely produces; compute coordinates via nodeLayout, never
magic numbers; one suite owns a behavior.

**Never use the shared/embedded Browser pane for verification** — it freezes hidden
tabs and stalls the BOS loader. Use the headless-CDP pattern in `tools/intgate.mjs` /
`tools/shot.mjs` (private Chrome, `Runtime.evaluate`, pixel sampling on non-background
pixels). `window.poc` exposes `{editor, viewer, panes, runEffect, doc, result}`.

## Map

| Where | What |
|---|---|
| `web/src/contracts.ts` | ALL shared types (GraphDoc, Intent, values, ModelData, bridges). Change carefully; everything depends on it |
| `web/src/kinds.ts` | The 20 node kinds: ports (with docs), param schemas, descriptions. Drives palette, in-node params/help, MCP `list_node_kinds` |
| `web/src/reducer.ts` | `applyIntent` — the ONLY way a graph changes |
| `web/src/flow/` | Evaluator + node implementations + vitest suite. Pure TS, no DOM |
| `web/src/editor/` | Gratify canvas editor + HTML chrome (menu bar, palette, help, Edit menu). NO side panel — params/help/status render in the node. In-row widgets: numbers scrub, booleans toggle, small enums chip; text params open the docked focus editor (explicit commit); other rows popover. Link-drag-search (drop a wire on empty canvas), splice-on-wire, undo (Ctrl-Z, coalesced per gesture). Files: geom (FROZEN layout), cards/wires/surface/palette/widgets/colormap/params/focus/chrome/help/doc. `createEditor(canvas, kinds, opts)` |
| `web/src/viewer/` | three.js BOS viewer bridge (`load/applyView/highlight/onPick`) |
| `web/src/panes/` | data grid + legend (plain DOM) |
| `web/src/main.ts` | integration: eval loop, display routing, selection preview, MCP intent polling |
| `host/` | C# SimpleHttpServer: `/api/models,file,sql,ask,append-psets,intents,state,kinds` + `/mcp` (10 tools). `BosCompat.cs`/`LegacyBosTables.cs` = BOS schema-drift shims. `/api/ask` needs `ANTHROPIC_API_KEY` |
| `demo/` + `web/public/demo/` | 10 demo graphs — KEEP BOTH COPIES IN SYNC (app fetches `/demo/*.json`) |
| `tools/` | intgate-smoke.mjs (integration gate), edgate-smoke.mjs (editor gate), shot.mjs (screenshot) |

## Extension recipes

- **New node kind**: add entry to `kinds.ts` (ports/params/description) + one `def()` in
  `web/src/flow/nodes.ts` + tests. Palette/inspector/help/MCP pick it up automatically.
  Follow existing semantics: null = absent; channel shadows model parameter; errors
  poison downstream.
- **New demo**: JSON in `demo/` AND `web/public/demo/`, add name to the `demos` array in
  `web/src/main.ts`; `demos.test.ts` validates structure automatically. Add a gate check
  in `tools/intgate.mjs`.
- **New pane/panel**: plain DOM beside the canvas (house rule: text/forms in DOM,
  graph/3D on canvas).
- **New host endpoint**: `host/HostApi.cs` pattern; user/data errors = 200 `{error}`,
  protocol errors = 4xx; keep the path sandbox.
- **New MCP tool**: `host/McpEndpoint.cs` — thin wrapper over existing operations only.

## Known warts (documented, don't rediscover)

- MCP intent queue replays from seq 0 into every fresh page (stray nodes after gate
  runs) — restart the host to clear, or fix it properly (session offsets).
- `EntityText.Type` = family/type string; the IFC class is `Category`.
- Browser BOS loader truncates numeric parameters to ints (upstream ara3d-webgl bug) —
  analytics numbers must come from CSV or `/api/sql`, never `ModelData.param()` floats.
- Recolor rebuilds geometry (~25ms at 20k instances) — fine here, don't scale it.
- duplex demo data: wall `operational_carbon` runs 221–412; the empty-Level aggregate
  group is real (unplaced beams).
- `data/models.json` is REGENERATED by the host at startup (`DataSetup.WriteModelsJson`)
  — add models in `DataSetup.Prepare`, never by editing the JSON. Default model =
  snowdon (95MB IFC converts in ~12s; conversion time is not size-proportional).
- Gratify window-level keydown lacks an editable-focus guard; the editor's dispatch
  wrapper drops delete/escape intents while a DOM field has focus. Fix belongs upstream.

## Conventions

Commit to main with pathspec (`git commit -- ara3d-sdk/wip/platoflow-poc/...`), push after a
verified milestone (`git pull --rebase` first — parallel agents have collided twice).
This is PoC code: favor clarity and speed over polish, but keep gates green and record
findings in NOTES.md — the findings are the deliverable that feeds the real design.
