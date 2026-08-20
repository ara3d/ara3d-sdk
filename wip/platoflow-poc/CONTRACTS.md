# PoC contracts — the seams between parallel tracks

Every inter-track interface lives here or in `web/src/contracts.ts` (types),
`web/src/kinds.ts` (node vocabulary), `web/src/reducer.ts` (graph mutations).
Those three files plus this doc are **supervisor-owned**: agents read them freely; if one
blocks you, make the smallest change that unblocks, and record it under "Contract
changes" in `NOTES.md` so other tracks find out. Do not redesign them.

## Fences (who writes where)

| Track | Writes only |
|---|---|
| A editor | `web/src/editor/**`, `web/editor.html` |
| B viewer+panes | `web/src/viewer/**`, `web/src/panes/**`, `web/viewer.html` |
| C evaluator+nodes | `web/src/flow/**` (incl. its `tests/`) |
| D host | `host/**`, `data/**` |
| E integration (supervisor) | `web/src/main.ts`, `web/index.html`, `demo/**`, root docs |

`web/package.json`: additions allowed by any track (run `npm install` after; note it in
NOTES.md). Commit with pathspec limited to your fence: `git commit -- ara3d-sdk/wip/platoflow-poc/<paths>`.
Do not push; the supervisor pushes.

## Graph document + intents

See `GraphDoc` / `Intent` in `contracts.ts` and `applyIntent` in `reducer.ts`. The
reducer is the only way any code mutates a doc — editor gestures, MCP agents, demo
loading all dispatch intents.

## Node vocabulary

`kinds.ts` declares the 14 node kinds (id, category, inputs/outputs with value types,
param schemas). Track A renders palette/params from it; Track C implements one
`evaluate` per kind; Track D's `list_node_kinds` MCP tool returns it verbatim (C exports
a JSON dump; D serves the file `data/kinds.json` that the web app POSTs on startup —
see Host API `/api/kinds`).

Value types on wires: `scene`, `table`, `colormap`, `view`. Socket colors:
scene #4fc3f7, table #ffb74d, colormap #ba68c8, view #81c784.

## Evaluator (C provides, A/E consume)

```ts
evaluateGraph(doc: GraphDoc, ctx: EvalCtx): Promise<EvalResult>
```
Re-evaluates the whole graph every call (no memoization — PoC models are small). Node
errors poison downstream (`status: "error"`). `EvalCtx` supplies model loading, `sql()`,
and `fetchText()` — implementations injected by E (real) and by A's harness (mock).

## Viewer bridge + panes (B provides, E consumes)

`ViewerBridge` and `Panes` interfaces in `contracts.ts`. Entity-level selection
everywhere; instances are B's internal concern. `applyView(null)` restores the default
full-model view.

## Host API (D provides; port 5214; all JSON; CORS `*`)

| Endpoint | Contract |
|---|---|
| `GET /api/models` | `[{ id, name, bosUrl, ifcPath? }]` — from `data/models.json` |
| `GET /api/file?path=` | Serve a file from the configured data roots (sandbox: reject paths outside roots) |
| `POST /api/sql` | `{ model, sql }` → `TableValue` (`{columns, rows}`) or `{ error }`. DuckDB over the model's BOS data, read-only, single statement |
| `POST /api/append-psets` | `{ model, psetName, rows: [{ globalId, props: {name: number\|string} }] }` → `{ outPath, entitiesAdded, diffSummary }`. Patches the model's source IFC, writes `<name>-enriched.ifc` beside it in an output dir |
| `GET /api/intents?since=N` | `{ intents: [{ seq, intent }], now }` — queue of agent-issued intents; browser polls ~250ms and applies via reducer |
| `POST /api/state` | Browser pushes `{ doc, results: { nodeId: { state, summary, table? } } }` after each eval (debounced). Host caches it to answer MCP reads |
| `POST /mcp` | JSON-RPC 2.0: `initialize`, `tools/list`, `tools/call`. Protocol as in `wip/Ara3D.MCP` |

MCP tools (all thin): `list_node_kinds` (from cached state or kinds.json),
`get_graph` (cached doc), `add_node`, `connect`, `set_param`, `set_display`,
`load_graph` (each enqueues an intent; `add_node` generates the id and returns it),
`read_node` (cached result summary + table page for a node), `sql` (direct `/api/sql`
passthrough so an agent can explore the model).

## Demo data

- `data/models.json` lists available models. D populates it with: `duplex` (converted
  from `C:\Users\cdigg\git\nrc-ifc-llm\IFC-Test-Kit\duplex.ifc` — required for the CSV
  join + pset beats) and `rac_basic` (copy of
  `C:\Users\cdigg\git\ara3d-webgl\examples\public\rac_basic_sample_project-2025.bos`).
- Carbon CSV: `C:\Users\cdigg\git\nrc-ifc-llm\IFC-Test-Kit\analytics_dataset_with_levels.csv`
  (keyed by duplex GlobalIds) — D copies it into `data/` and serves via `/api/file`.
- Fallback if duplex IFC→BOS conversion fights back: generate a synthetic carbon CSV for
  `rac_basic` from its entity table (via SQL) and note it in NOTES.md; the join beat
  works the same.

## Ports

Host `5214`; vite `5215` (proxy `/api` + `/mcp` → 5214, configured in `vite.config.ts`).

## UX-plan fences (waves W1–W3; see docs/platoflow-ux-implementation-plan-2026-08-09.md)

After the W0 split, `web/src/editor/parts.ts` is a re-export shim. Frozen files
(supervisor-owned, tracks READ only): `contracts.ts`, `kinds.ts`, `reducer.ts`,
`editor/geom.ts` (after its W0 signature change), `editor/doc.ts` history section.

| Track | Writes only |
|---|---|
| T1 widgets | `editor/widgets.ts`, `editor/cards.ts` (param-row render + gesture dispatch) |
| T2 commit/focus | `editor/params.ts`, `editor/focus.ts` (new), `web/editor.html` |
| T3 link-drag-search | `editor/wires.ts`, `editor/palette.ts`, `editor/surface.ts` |
| T4 undo UI | `editor/doc.ts` (non-history), `editor/chrome.ts`, keyboard in `editor/index.ts` |
| T5 colormap body | `editor/colormap.ts` (new) or the `// gradient` region of `widgets.ts` |
| T6/T12 gratify ⚠ | `submodules/gratify/**` only — own repo, own push, never the PoC |
| T7 tests | `web/src/editor/tests/**`, `tools/edgate-smoke.mjs`, `tools/intgate-smoke.mjs` |

## Wave-9 fences (design-gap wave: honesty + set algebra/grouping + linking + persistence)

Supervisor-owned as always: `contracts.ts`, `kinds.ts`, `reducer.ts`, `editor/geom.ts`
(narrow delegations noted in briefs), this doc. Landed before spawn: `ChannelValue`
record (`channels: Record<string, ChannelValue>`), `SceneValue.groups`, `NodeStatus`
"needs-setup" + `warning`, `ViewValue.legend`, `Panes.showViewLegend?`,
`TableValue.source`, dynamicEnum source `"groupKeys"`, 7 new kinds (stubbed), the
`flow/nodes.ts` split (`registry.ts` + `lib.ts` + `defs-core.ts` + `defs-viz.ts` +
`defs-sets.ts` + `defs-export.ts`; `nodes.ts` = aggregator), `NeedsSetup`/`needsSetup`
+ `NodeOut.warning` in `flow/types.ts`.

Edge-key convention (wire highlight, W9-C ⇄ supervisor): `${from.node}.${from.slot}->${to.node}.${to.slot}`.

| Track | Writes only |
|---|---|
| W9-A flow honesty | `web/src/flow/{defs-core,evaluate,types,csv,summaries,lib}.ts`, `web/src/fixtures/mockModel.ts`, `web/src/flow/tests/{beat1,beat3,byParameter,errors,wave2,demos,askChart,parts,subgraph}.test.ts`, `web/src/flow/tests/harness.ts`, NEW `web/src/flow/tests/honesty.test.ts` |
| W9-B new nodes | `web/src/flow/{defs-sets,defs-viz}.ts`, NEW `web/src/flow/tests/{setops,groupby,categorical}.test.ts`, `demo/{set-algebra,group-color}.json`, `web/public/demo/{set-algebra,group-color}.json` |
| W9-C editor status+wires | `web/src/editor/{cards,wires,help}.ts`, `web/src/editor/tests/badges.spec.ts`, NEW `web/src/editor/tests/{status-render,wirehl}.spec.ts` (+ pre-authorized NARROW `editor/geom.ts` helpLines edit — see brief) |
| W9-D panes legend | `web/src/panes/**`, `web/src/viewer/**` |
| W9-E host persistence | `host/**`, `data/**`, `web/src/editor/chrome.ts`, `web/src/flow/defs-export.ts`, NEW `web/src/flow/tests/export.test.ts`, `web/src/editor/tests/{chrome.spec.ts,chrome-edit.spec.ts}` |
| S integration (supervisor) | `web/src/main.ts`, `web/index.html`, `tools/**`, root docs |

W9-E MAY restart the shared host on :5214; nobody else touches it. All other tracks
verify with `npx tsc --noEmit` + vitest (mock model) only. Commit with pathspec
limited to your fence; never push.

## Browser-check retirement list (agreed W0; T7 executes)

Retired into pure vitest (do NOT fix these when behavior changes — delete them):
edgate: param-click/popover/commit/Esc/click-away/Backspace-guard, help-chip +
socket-shift, eye/✕/Del, ask-detail + chart-shape checks (19 of 24).
intgate: all seven `editor:` checks; wave-2/wave-4 evaluator-output assertions
(auto-domain summary, view-node reports, filter-sort columns, chart payloads)
(~12 of 30).
Survivors: `edgate-smoke.mjs` (5: boot, one popover over canvas, chart pixels,
one trusted-input drag, console clean) and `intgate-smoke.mjs` (~13: boot,
beat-1 pixels, scrub→pixel timing, one demo per host feature, legend, console).
Rules for every new check: wait only on data the new state can uniquely produce;
no magic coordinates — compute via `nodeLayout`; one suite owns a behavior.

## W13 UX wave (2026-08-20) — fences

Supervisor-owned (tracks READ only; request smallest unblocking change via report):
`web/src/contracts.ts`, `web/src/kinds.ts`, `web/src/reducer.ts`, `web/src/editor/index.ts`,
`web/src/editor/geom.ts`, `web/src/main.ts`, `web/src/panes/**`, `web/src/tour.ts`, this doc, `NOTES.md`.

Contract changes landed pre-wave: `ChromeHooks` gained `fitGraph()` and `tidyGraph()`
(chrome.ts interface; index.ts implements — fit is live, tidy is a stub until the
layout module is wired at integration).

| Track | Writes only | Resources |
|---|---|---|
| A layout | `web/src/editor/layout.ts` (new), `web/src/editor/tests/layout.spec.ts` (new) | vitest only |
| B calm+eye | `web/src/editor/cards.ts`, `web/src/editor/wires.ts`, `web/src/editor/surface.ts`, any EXISTING spec under `web/src/editor/tests/**` EXCEPT `chrome.spec.ts` / `chrome-edit.spec.ts`, plus `web/src/editor/tests/calm.spec.ts` (new) | vitest only |
| C chrome | `web/src/editor/chrome.ts`, `web/src/editor/toast.ts` (new), `web/src/editor/tests/chrome.spec.ts`, `web/src/editor/tests/chrome-edit.spec.ts`, `web/src/editor/tests/toast.spec.ts` (new) | vitest only |
| D context menu | `web/src/editor/contextmenu.ts` (new), `web/src/editor/tests/contextmenu.spec.ts` (new) | vitest only |
| S integration (supervisor) | everything supervisor-owned; wires A into tidy/drop, B's display model into main.ts, D into index.ts; runs full gate | host :5214, vite :5215, headless shots |

No track starts vite or the host, restarts servers, or uses the shared Browser pane.
Commit with pathspec limited to your fence (`git commit -- <paths>`); never push.

### W13 display-model decision (context for all tracks)
The eye chip is REMOVED from the UI. Selection drives what the 3D viewer and grid
show; `GraphDoc.display` stays in the schema as the "pinned" fallback (MCP compat,
shown when nothing is selected). Auto-display of the last viz/sink on load and all
main.ts wiring is supervisor work.
