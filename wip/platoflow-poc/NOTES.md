# PoC notes — findings that must feed back into the real design

Agents: append findings here (design friction, contract changes, surprises, perf
numbers). This file is the PoC's real deliverable alongside the demo.

## Contract changes

(record any edits to CONTRACTS.md / contracts.ts / kinds.ts / reducer.ts here)

**2026-08-19 rebrand (cream theme):** `contracts.ts` — WIRE_COLORS *values* changed to
desaturated hues for the light canvas (keys/types untouched). Rest of the rebrand:
`web/src/theme.ts` (new; registers a "cream" gratify palette + snaps it at boot),
`--pf-*` CSS vars in the three html pages, DOM style blocks switched to the vars,
CATEGORY_COLOR/STATUS_COLOR desaturated, dark-theme-assuming canvas literals
(white-alpha hovers, black-mix wells, dark badges) flipped for white cards. The 3D
viewport intentionally stays near-black. Hex values are approximate targets from a
screenshot spec, not exact brand tokens.

**Track D — no edits to CONTRACTS.md / contracts.ts / kinds.ts / reducer.ts.** Two
clarifications of things the doc already implied:
- `/api/kinds` is referenced in the CONTRACTS.md "Node vocabulary" section but is missing
  from the Host API table. The host implements `GET` (serve `data/kinds.json`) and `POST`
  (replace it with the web app's own dump — Track C exposes `kindsJson()` for exactly this).
- `kinds.ts` declares **13** node kinds, not the 14 the task brief and CONTRACTS.md say.
  `host/kinds.default.json` transcribes the 13 that exist.

## Findings

### Track B — viewer bridge + panes

No edits to `contracts.ts`. Two things outside the fence, both flagged in the brief:
`web/public/*.bos` added to `.gitignore` (the sample model is copied in, not committed),
and `src/panes/index.ts` now *imports* `ramp` from Track C's `flow/nodes.ts` instead of
keeping its own copy — Track C asked for exactly that so the legend and the geometry
colours cannot disagree. No `package.json` changes, so no `npm install` needed.

**`ModelData` has no notion of which entities are drawable, and it needs one.** In
`rac_basic` only **500 of 2570 entities have geometry**; the other 2070 are materials,
views, levels, family types and documents. Worse, they come *first*: entity indices
0–1604 all have an empty category. The harness's "first 100 entities" table was therefore
100 rows that highlight nothing when clicked — a bug that looks exactly like a broken
highlight. The bridge now exposes a non-contract `geometryEntities()` and the demo table
lists drawable entities only. In the real design `ModelData` should carry
`hasGeometry: Uint8Array` (or `drawable: Uint32Array`); every viz node and every table
that feeds a viewer selection needs it, and each one will otherwise rediscover this.

**Recolouring costs a full geometry rebuild, and that is the dominant number here.**
`applyView` has to hand `BimData.rebuildGeometry` a complete instance list, which re-merges
and re-uploads every mesh. Measured on rac_basic (2570 entities / 19599 instances,
Chrome, SwiftShader):

| view | entities coloured | instances emitted | plan | rebuild | total |
|---|---|---|---|---|---|
| walls red + ghost rest | 216 | 19599 | 1.2–1.8 ms | 18–31 ms | ~20–33 ms |
| colour by level (all) | 2570 | 19599 | 1.1–1.6 ms | 20–24 ms | ~22–26 ms |
| walls only, others hidden | 216 | 1240 | 1.6 ms | 1.1 ms | 2.7 ms |
| colour by level, repeat | 2570 | 19599 | 2.7 ms | 26 ms | 29 ms |

Two readings. (1) The *planning* step — turning a `ViewValue` into per-instance materials —
is essentially free (1–3 ms); all the cost is the library's merge. (2) Cost scales with
instances *emitted*, not entities *selected*: hiding the remainder instead of ghosting it
is 10x cheaper. At PoC scale 30 ms is fine (interactive), but this is ~7 µs per instance,
so a 300k-instance model would be ~2 s per recolour — unusable for a slider. The real
design wants per-instance colour as a vertex/instance attribute updated in place
(`InstancedMesh.instanceColor` + a colour attribute on merged meshes) so a recolour is a
buffer upload, not a re-merge. Keep `rebuildGeometry` for structural changes only.
Mitigation already in: one shared `MeshStandardMaterial` per *quantized* colour, so
colouring by level allocates 7 materials, not 2570 — without that the merge would produce
one draw call per entity.

**The library gives no way to change an instance's colour without rebuilding, and no way
to dispose what a rebuild allocated.** `rebuildGeometry` merges geometries into fresh
`BufferGeometry` objects every call; nothing tracks them, so a naive `applyView` loop leaks
GPU memory. The bridge keeps a `Set` of loader-owned geometries and disposes only the
merged ones on swap. A viewer library should own that lifecycle.

**BOS geometry has no normals.** `computeMeshGeometries` writes only `position` and
`index`. The loader's own materials get away with it via `flatShading: true`. The first
highlight overlay used a `MeshStandardMaterial` and rendered nothing at all — silently, no
error. Overlays must be unlit (`MeshBasicMaterial`), or the loader should compute normals.
This cost real debugging time and will cost it again.

**Loader API friction, in order of annoyance.**
1. The npm package's `index.ts` exports the *loader class* but not the *types it returns* —
   `BimData`, `Instance`, `BimResolver` are all unexported. Consumers must recover them
   structurally (`Awaited<ReturnType<BimOpenSchemaLoader["load"]>>`). Export the types.
2. **The package bundles its own three.js (0.182) and re-exports it as `ARA3D.THREE`.**
   Any consumer that also depends on `three` gets a second, incompatible instance whose
   materials the library's renderer cannot use. Every three.js value in `src/viewer` comes
   from the package's namespace for this reason. The package should declare `three` a peer
   dependency and externalise it.
3. `loader.load(url)` swallows the fetch, so a stalled body is indistinguishable from a
   slow parse. The bridge fetches the `ArrayBuffer` itself (with retries and per-stage
   status) and calls `loadFromArrayBuffer`. A loader should either take bytes or report
   progress.
4. `BimData.Instances` is a *filtered* array — instances with missing geometry are skipped —
   so its array position is **not** the `InstanceIndex` that `userData.pick` returns.
   Indexing `Instances[instanceIndex]` looks right and is wrong. The bridge builds an
   explicit `InstanceIndex -> Instance` map.
5. Parameter values are reachable only through `Resolver.ParameterMap` (entity -> array of
   `{Name, Value}`), with no by-name index, so materialising one column costs a full scan
   of the map (0.6–3.2 ms here, cached afterwards). Descriptor metadata (pset/group, type)
   lives in a *different* structure (`Descriptors`) keyed by descriptor index, which the
   `ParameterMap` has already discarded. Joining "what is this parameter" to "what is its
   value" therefore has to go through the name string. A columnar parameter accessor
   (`column(descriptorIndex) -> typed array`) would be both faster and better typed.
6. `Viewer` finds its canvas with `document.getElementById`, so the canvas must exist in
   the DOM *before* the `Viewer` is constructed. It never takes a container element.
7. Framing is manual: `viewer.camera.do().frame(box)` after computing the `Box3` yourself.

**Entity "type" is the Revit category, not an IFC class.** `types[i]` is
`GetEntityCategoryName`, giving `Walls`, `Doors`, `Curtain Panels`, `Topography` — not
`IfcWall`. The `select.byType` vocabulary the graph exposes to users is therefore whatever
the authoring tool called it, and it differs per source model. Whatever the real design
calls this field, it must not be named `types` and documented as "IFC type".

**Verifying a WebGL page in this environment needed its own escape hatch.** The shared
Browser pane freezes page timers whenever another agent fronts its tab; with four agents
running, `JSZip.loadAsync` inside the BOS loader never completed (a 724 KB `fetch` took
64 s and only progressed while the pane was displayed). Neither a screenshot nor a console
read is trustworthy under that. The gate was instead run against a private headless Chrome
driven over CDP from a ~250-line Node script: launch `chrome --headless=new
--remote-debugging-port=N --enable-unsafe-swiftshader`, `PUT /json/new?<url>`,
`Runtime.enable` for console capture, then `Runtime.evaluate`. Recolouring is asserted on
real pixels — force `renderer.render()`, `gl.readPixels`, and average **only the pixels
that differ from the scene background** (the model covers 2.7% of the canvas, so a
whole-canvas mean moves by ~1 even on a total recolour and proves nothing). Results:
26/26 browser checks, plus 27/27 checks of the pure modules run directly in Node.
Recommend the real repo keep this as a committed script — it is the only way we have to
prove a 3D view actually changed.

### Track C — evaluator + node semantics

No contract edits were needed: `contracts.ts` / `kinds.ts` / `reducer.ts` are untouched.
45 vitest specs over `src/flow/tests`, `tsc --noEmit` clean, no DOM references in `src/flow`.
Public surface for other tracks: `flow/evaluate.ts → evaluateGraph`, `flow/nodes.ts →
NODES, ramp, buildPsetRows, asNumber`, `flow/summaries.ts → summarize`, `flow/index.ts`
re-exports all of it plus `kindsJson()` for Track D's `data/kinds.json`.

**`ModelData.param()` cannot say "no such parameter".** It returns a full-length
`Cell[]`, and the mock fills unknown names with nulls — so a typo'd parameter looks
exactly like a parameter that happens to be empty on this selection. The evaluator has to
use `paramNames()` as the existence oracle before calling `param()`. In the real design
`param()` should return `Cell[] | null` (or `paramNames()` should be the only entry point,
with `param(info)` taking the descriptor). This bit one test before the workaround landed.

**The channel model is the strongest idea here, and it wants sharpening.** A channel is a
full-length `Cell[]` keyed by name in `SceneValue.channels`, and downstream nodes resolve a
name as "channel first, model parameter second". That single rule made `select.byParameter`
and `viz.colorBy` work over joined CSV data and native IFC psets with no branching. Three
frictions worth fixing for real:
1. *Silent shadowing.* A channel named `Area` hides the model's `Area` with no warning.
   Channels probably deserve their own namespace (`ch:Area`) or an explicit shadow flag.
2. *No provenance or units.* A channel is a bare array; nothing records that it came from
   `carbon.csv` column `embodied_carbon`, whether it is numeric, or what unit it is in.
   `sink.writePset` has to guess the IFC property type from the JS value at write time.
   A channel should be `{ values, source, numeric, unit? }`.
3. *Full-length arrays are wasteful but were the right call.* Selection and data are
   deliberately separate: `attach.column` writes a full-length array and leaves
   `entities` alone, so narrowing the selection later never loses data. Keep that
   invariant in the real design; swap the representation (sparse map / typed array +
   validity mask) only if profiling demands it.

**Poisoning needs to name the culprit, not just the immediate parent.** Downstream nodes
report `upstream error in <id>`, which is only the direct predecessor — three hops down a
chain the user sees `upstream error in n3` and has to walk back manually. A real
implementation should carry the *root* failing node id plus its message.

**`table.aggregate`'s param schema is under-specified for `count`.** `count` does not need
a value column, but `kinds.ts` gives it the same `value` param as the other aggregations;
the evaluator special-cases it (column named `count` when `value` is blank). Aggregations
want per-agg param schemas, or `count` wants to be its own kind.

**Empty params on select nodes error rather than pass through.** A freshly dropped
`select.byType` with no type reports `no type selected` instead of silently forwarding
everything. That is the honest signal for an editor, but it means a new node is red until
configured — worth confirming that Track A's UI reads that as "needs input", not "broken".

**Ordered comparisons drop non-numeric rows silently.** `FireRating > 1` yields zero
entities because `"1HR"` is not a number. Correct, but invisible: a real design should
surface a "42 rows dropped as non-numeric" note on the node rather than an empty result.

**`null` in a filter means "absent", not "a value".** `==`, ordered ops, `contains` and
`exists` are all false for null; only `!=` is true. This is the pragmatic choice for
BIM data (most parameters are null on most entity types) and differs from SQL's
three-valued logic — the real design must document which one it implements.

**Cycles are reported per node, not per graph.** Nodes on a cycle get `cycle detected`;
everything hanging off the cycle gets the ordinary poison message. Self-loops count as
cycles. Whole-graph re-evaluation with no memoization was plenty fast at PoC scale
(45 specs incl. ~10 full graph evaluations run in well under 100 ms).

**Duplicate ramp maths.** `src/panes/index.ts` notes it reimplemented the color ramp
because `flow/` did not exist yet. `ramp(name, t)` is now exported from `flow/nodes.ts`;
the legend and the geometry colors must sample the *same* function or the legend lies.
Suggest Track B/E delete the local copy and import it.

**Quoted CSV cells stay strings; unquoted numeric-looking cells become numbers.** So
`"007"` survives as a GlobalId-ish key while `007` becomes `7`. Worth keeping — IFC data
is full of numeric-looking identifiers — but it means the CSV writer's quoting decides
the column type, which is fragile. Real design: infer column type over the whole column,
not cell by cell.

### Track D — host

Everything works against real data: duplex converted, both models queryable, psets written
byte-exactly. `host/smoke.ps1` is 15 steps, all PASS. No fallback was needed.

**BOS is not one schema, and nothing says which one a file is.** The two demo models
disagree. `duplex.bos`, produced now by `IfcToBosConverter`, has `Parameters` + `Numbers`
with a `Descriptors.Type` discriminator. `rac_basic_sample_project-2025.bos` from
`ara3d-webgl/examples/public` instead splits parameters by value type —
`SingleParameters`, `IntegerParameters`, `StringParameters`, `EntityParameters`,
`PointParameters` — and has no `Numbers` table at all. `IfcDuck.CreateViews` is written
against the first, so the second dies on `Catalog Error: Table with name Parameters does
not exist!` *while creating a view*, i.e. after the whole zip has already been unpacked
and loaded. The host now catches that and rebuilds equivalent views over whatever tables
are present (`host/BosCompat.cs`: `EntityText`, `RelationText`, and a `ParameterText`
that UNION ALLs the split tables, each branch probed with `LIMIT 0` and dropped if it
does not compile). Both models then answer the same SQL.

Real design consequences, in priority order:
1. **A .bos file needs a schema version in it.** Sniffing table names to guess the layout
   is what we just had to do, and it only worked because the difference happened to be
   visible in `information_schema`.
2. **The view layer belongs with the reader, not with the caller.** `IfcDuck.CreateViews`
   is the only thing that makes a BOS database answerable, and it lives in
   `wip/Ara3D.Ifc.Mcp` — a tool project. Every future consumer will re-hit this.
3. The split-table layout is the better one for a columnar store (no per-row type
   discriminator, no CAST-everything-to-VARCHAR). If the two are converging, converge
   toward it — but then `ParameterText` needs to be the *only* documented entry point.

**Conversion cost is a UX problem, not a perf problem.** duplex 2.3 MB IFC to a 95 KB BOS
takes **42.9 s** (it re-reads the file with geometry through the native web-ifc DLL), plus
6.0 s to build the DuckDB database. rac_basic's database builds in 0.3 s from an existing
BOS. The PoC hides this by caching everything in `data/` and skipping on a second run, but
43 s of silence at startup is not shippable: the real host needs the conversion cached by
content hash and a progress channel to the UI.

**`IfcDuck` reuse: took the two useful pieces, wrote around the third.** `ReadOnlyQuery`
(single-statement SELECT/WITH validation) and `CreateViews` source-linked cleanly and are
exactly right. `IfcDuck.Query` was not usable: it wraps every query in
`SELECT count(*) FROM (...)` *and* a paged `SELECT *`, so a browser table costs two full
executions per fetch, and it returns `IfcQueryResult` (Total/Skip/Count/Columns/Rows)
rather than the `TableValue` (`{columns, rows}`) the wire contract wants. Its row reader is
private, so `host/ModelCatalog.cs` has its own 30-line one. For the real host: separate
"validate + execute" from "page + count", and let the caller choose the result shape.

**`IfcBosArtifacts` assumes a session, not a server.** It builds into a GUID temp folder
and deletes it on dispose, which is right for an MCP session and wrong for a host that
wants `data/duplex.bos` to survive restarts. The three lines that matter (`SaveToBos`,
`BosToDuckDB`, `CreateViews`) were easier to call directly. Note the comment it carries is
load-bearing: `IfcToBosConverter.Convert` never disposes the `IfcFile` it opens, so a
long-running process must run the converter directly and dispose it itself.

**`wip/Ara3D.MCP` could not be reused, for one structural reason.** `McpJsonRpcHandler` —
the part that actually implements initialize/tools-list/tools-call — is `internal`, and
the only public way in is `McpHttpListener`, which owns its own `HttpListener` and
therefore its own port. The contract here is one port serving `/api/*` and `/mcp`, so the
host hand-rolled JSON-RPC instead (~200 lines, `host/McpEndpoint.cs`). **Fix for the real
product: make a public, transport-free handler — string in, string out — and let the host
own the socket.** That single change would have made the whole file unnecessary. The
protocol details worth copying were the origin check (absent Origin allowed for CLI
clients, browser Origins must be loopback) and lifting a failed handler onto `isError`.

**The pset write path is production-shaped code living in a test project.**
`IfcSourceFile` / `IfcPatcher` / `IfcPropertySetBuilder` / `IfcPropertyValue` / `IfcDiff`
source-linked out of `tests/Ara3D.Ifc.Tests` with zero friction and worked first try: two
rows became 8 entities (4 props + 1 pset + 1 rel per element), `IfcDiff` reports 8 added /
0 deleted / 0 changed, everything else byte-identical. It should be a real library
(`Ara3D.Ifc.Editing`); shipping a graph-to-IFC writeback on top of a test assembly is not
a thing we can do twice. Also confirmed: `IfcSourceFile.GlobalIdToEntityId()` keys match
`EntityText.GlobalId` from the BOS conversion exactly, so the graph can round-trip
entity identity through SQL and back into the file with no id mapping table.

**Errors as HTTP 200 `{error}` was the right call.** The browser gets one code path for
"the host answered", and a rejected SQL statement shows up in the node's status the same
way a bad parameter does. Only the file sandbox (403) and unknown routes (404) use status
codes, because those are bugs rather than user errors.

### Track A — node-graph editor (Gratify)

No contract edits: `contracts.ts` / `kinds.ts` / `reducer.ts` / `package.json` untouched.
Files are `web/src/editor/{index,doc,parts,panel,geom,harness}.ts`. Every mutation —
gesture, palette, HTML inspector, external `dispatch()` — funnels through `applyIntent`.

**The reducer's "one wire per input" rule bought reconnect-replaces for free.** The editor
has zero code for "this input is occupied"; the connect gesture just emits `connect` and
the old edge disappears. Contracts that make the illegal state unrepresentable pay for
themselves immediately — keep that rule.

**Type-checked wiring cost nothing, because the type rides on the anchor.** Each socket
publishes `meta: { node, dir, slot, type }`, and the magnetic snap takes a predicate:
`query.nearestAnchor(p, 30, c => c.type === from.type && c.dir !== from.dir)`. An
incompatible socket never highlights, so a wrong wire cannot be *drawn*, let alone
validated after the fact. The real design should keep validation in the snap predicate
rather than in a post-connect check.

**Intents that carry their own id force an id-minting layer into the editor.**
`addNode` takes `id`, but `freshNodeId(doc)` needs the doc, so the palette cannot build a
complete intent at the click site — it emits an internal `addKind` intent and the reducer
step mints the id before calling `applyIntent`. Track D's MCP `add_node` has the same
problem from the other side ("generates the id and returns it"). The real Intent set wants
a *create* intent with no id, and the reducer returning the id it assigned.

**Gratify has no "doc committed" subscription.** Canvas interactors dispatch into the
Runtime internally, so anything outside the canvas (here: the HTML inspector) never learns
that the doc changed. Worked around by wrapping `rt.dispatch` after `mount`
(`index.ts`), and by having the app reducer `queueMicrotask` the host's `onDocChange`.
Both are hacks around a missing seam: `AppSpec` should accept an `onCommit(doc)`, or
`Runtime` should expose `subscribe()`.

**Adornments are one flat overlay above *all* content, which breaks world-layer popups.**
The palette is a world-layer element, so every node's eye/Run chip drew *on top of* it.
Fixed by threading a `hideChips` prop from `doc.palette` into every card — i.e. the app
now knows about paint order, which is exactly what the layer model is supposed to prevent.
Two candidate fixes for the real design: give adornments a z-tier, or make popups
adornments themselves.

**`modal(el, dismiss)` only applies to adornments, so the palette can't use click-away.**
The palette closes via the surface's `Press` plus `Escape`; the visible cost is that
clicking the palette's own padding falls through to the surface and closes it. Making the
palette an adornment of the surface would have given click-away for free — worth doing if
this survives past the PoC.

**SKILL.md rule 9 is stale in one direction and correct in the other.** Local state and
modal popups *are* shipped (`.local()` / `.reduce()` / `modal()`, see `examples/dropdown`).
Text input still is not, so the parameter inspector is plain DOM — and that turned out to
be the right seam anyway: schema-driven HTML controls (`input`/`select`/`textarea` from
`ParamSchema`) took ~120 lines and gave native focus, IME, and clipboard for free. The
real product should keep text entry in the DOM rather than grow canvas text fields.

**Panel-rebuild policy is worth stealing.** The inspector rebuilds only when the *selected
node id* changes, and otherwise just repaints the status line. That is what keeps `input`
events (live SQL typing) from wiping the field under the cursor. External `dispatch()`
forces a rebuild, because MCP may have changed a param out from under the DOM.

**Verification recipe (and a trap that cost ~20 minutes).** The Claude Browser pane
collapses to 0×0 and suspends its renderer when it is not displayed: canvas apps then
render blank, screenshots fail outright, and `performance.now()` deltas are meaningless —
a frame with *every* draw call stubbed to a no-op measured 1.4 s, which briefly looked
like a Gratify perf bug (it is not; `painter.glow` is fine). What does work:
- drive `runtime.pointerDown/Move/Up`, `runtime.key`, and `runtime.tick(1/60)` directly
  and assert on `runtime.doc` — this exercises the real input pipeline, gestures included;
- for pixels, `POST canvas.toDataURL()` to a throwaway local HTTP sink that writes a PNG,
  then read the file. `preview_screenshot` never worked in this session.

**Gotcha for anyone writing `.ps1` here: keep it pure ASCII.** Windows PowerShell 5.1
reads a UTF-8-without-BOM script as ANSI, so an em dash decodes to `â€"` — and that last
character is a *smart double quote*, which the parser accepts as a string delimiter. One
em dash in a comment-adjacent string made the whole 230-line script fail to parse with a
misleading "missing terminator" 160 lines later.

### Track D — BOS layout and type codes are both incompatible between writer and reader

Integration hit the mirror image of the `BosCompat` problem: the host could read
`rac_basic.bos` but not with `IfcDuck`'s views, and the *browser* could read `rac_basic.bos`
but not the freshly converted `duplex.bos` (`Could not find "IntegerParameters.parquet" in
zip archive`). Chasing that turned up a second, worse mismatch underneath it.

**1. Two parameter layouts.** The old writer (rac_basic, and everything
`@ara3d/ara3d-webgl` 1.3.15 can read) splits parameters by value type into
`IntegerParameters` / `SingleParameters` / `StringParameters` / `EntityParameters` /
`PointParameters`, each `Entity:int, Descriptor:int, Value`. The current
`IfcToBosConverter` (duplex) writes a single `Parameters` table plus a `Numbers` side
table. Nothing in either file says which layout it is.

**2. The type codes are off by one, and nothing caught it.** `ParameterType` is
`Int=0, Bool=Int=0, Number=1, Entity=2, String=3, Point=4`. rac_basic matches it exactly —
its `SingleParameters` are all `Type=1`, `IntegerParameters` all `Type=0`, `String` 3,
`Entity` 2, `Point` 4. **duplex is uniformly one higher**: Number=2, Entity=3, String=4,
consistent with a writer whose enum gave `Bool` its own value instead of aliasing it to
`Int`. Both readers assume the canonical numbering, so on a current-converter file:
- `IfcDuck.ParameterText` labelled every parameter with the wrong type *and* dereferenced
  each value against the wrong side table. `Ifc:ApplicationIdentifier` came back as
  `Point 140`; `Area` came back as the entity name `IFCORGANIZATION`. It never threw — the
  indexes are all in range for the wrong table, so the output is plausible and wrong. The
  host was serving this for duplex until the fix below.
- the loader's `GetVal` (`descType==3` indexes Strings, `==2` is an entity, else raw
  number) would have mis-resolved the same way.

The fix (`host/LegacyBosTables.cs`) does not hard-code the shift. It infers it by
validating each type code against the only invariant available: a code claiming to be
String must have values that are legal indices into `Strings`, Entity into `Entities`,
Number into `Numbers`, Point into `Points`; Int holds its value inline and constrains
nothing. The smallest shift under which every code is legal wins (detected: 1). It then
folds the shift out of `Descriptors.parquet` and derives the five legacy tables, so the
same edit fixes the browser *and* `ParameterText`. Confirmed after the fix: `Area` is
`Number 30.141645`, `Ifc:ApplicationIdentifier` is `String "Revit"`.

Value-splitting rules, all confirmed against resolved data rather than assumed:
| legacy table | canonical type | Value |
|---|---|---|
| `IntegerParameters` | 0 Int/Bool | `p.Value` inline, INTEGER |
| `SingleParameters` | 1 Number | **dereferenced**: `Numbers.Numbers[p.Value]`, FLOAT |
| `EntityParameters` | 2 Entity | `p.Value`, entity index, INTEGER |
| `StringParameters` | 3 String | `p.Value`, index into `Strings`, INTEGER |
| `PointParameters` | 4 Point | `p.Value`, index into `Points`, INTEGER |

Only Number changes representation: the new layout stores an index into `Numbers`, the old
one stores the float itself. duplex splits 3650 Single / 11946 String / 60 Entity /
0 Integer / 0 Point = 15656 = every row of `Parameters`. The new-layout tables are kept
alongside, so the host's own SQL is unaffected. Idempotent: a `.bos` already carrying the
five tables is left alone.

**3. The loader truncates every numeric parameter to an integer.** `bimOpenSchemaLoader`
reads all five tables with `Int32Array` as the coercion constructor, and
`SingleParameters.Value` is FLOAT, so `new Int32Array(float32Array)` drops the decimals.
Measured in headless Chrome against the patched duplex: `Area` reads `[17, 7, 3, 17, 7]`
where the parquet holds `[17.94, 7.80, 3.99, ...]`; `Width` is all zeroes. This is
pre-existing — rac_basic stores `Value` as FLOAT too and loses the same precision — and no
host-side encoding avoids it, so the PoC matches the old writer and leaves it. It does mean
**numeric parameters read through the browser BOS loader are useless for analytics**. The
PoC is unaffected only because its numbers come from the CSV and `/api/sql` (DuckDB, full
precision), never from the loader's parameter map. Real fix belongs in the loader: pass
`Float32Array` for `SingleParameters`, or drop the ctor and let hyparquet's own types stand.

**What this says about the real design.** A `.bos` needs a schema version *and* the
parameter type enum needs a single owner. Three components each hold their own copy of the
numbering — the converter, `IfcDuck`, and the TypeScript loader — and two of the three
disagreed with no error anywhere: every symptom was plausible-looking wrong data. A shared
generated constant, plus a conversion test that round-trips one known string and one known
number end to end, would have caught it at the source.

### Track E — integration + verification (supervisor)

**All five demo beats verified end-to-end: 14/14 checks in headless Chrome**
(`tools/intgate.mjs`; screenshot `poc-carbon-walls.png`). Beat highlights: join
matched 56/56 walls; scrub → recolor measured **123 ms param-to-pixels** (includes the
60 ms eval debounce and 50 ms sampling granularity — the true redraw cost is B's
~2-4 ms rebuild); aggregate-by-level table with row-click 3D highlight (221 overlay
meshes); DuckDB SQL node returning a 25-row Category census; pset write producing
`duplex-enriched.ifc` (168 entities added, diff clean); MCP `add_node` appearing in the
live editor within one poll cycle; zero console errors.

Findings for the real design:

1. **The contracts held.** Four agents built against `contracts.ts`/`kinds.ts`/
   `reducer.ts` without meeting, and the whole project typechecked on first assembly.
   Zero interface mismatches; every integration failure was data or environment, never
   the seams. Strong evidence for the registry+reducer+JSON P2 architecture.
2. **BOS `Type` vs `Category` semantics bit twice.** `EntityText.Type` is the type
   object/family string ("1000mm"); the IFC class is `Category`. Both the demo SQL and
   my range probes assumed the opposite. The real design's fact vocabulary must name
   these unambiguously (e.g. `ifcClass` vs `familyType`).
3. **Colormap domains need auto-range.** Wall carbon runs 221–412; the demo colormap
   said 0–150, so every wall clamped to one color and scrubbing changed nothing —
   *silently*. A `viz.colormap` auto mode (or a domain readout on the node) is a v1
   requirement, not a nicety: this is the #1 way a novice will conclude the tool is
   broken.
4. **The empty group is real data.** The largest carbon sum in carbon-by-level is the
   ""-level group (unplaced elements — beams). Aggregates must render the null group
   honestly, and level highlight must treat "" as "entities with no level" (fixed in
   main.ts).
5. **The MCP intent queue needs consumer offsets.** A fresh browser session polls
   `since=0` and replays every intent any agent ever sent (stray Colormap nodes in the
   screenshot). PoC-acceptable; the host-API design's session model already fixes this.
6. **Shared Browser pane is unusable under multi-agent load** — background tabs freeze
   timers, hidden panes stall renderers. Track B's headless-Chrome-over-CDP recipe
   (`tools/intgate.mjs` pattern) was the reliable gate for every track and belongs in
   the real repo's verification toolkit.
7. **Two genuine data-plane bugs found by the drift between components** (details in
   Track D's section): the BOS writer's Descriptors.Type off-by-one (corrupting
   ParameterText silently) and the loader's Int32Array truncation of Number parameters.
   Neither would have surfaced without two independent consumers of the same tables —
   an argument for the parity-golden discipline in the real design.

---

## Track G — wave 2 (five new kinds, auto colormap domains, three demos)

Fence: `web/src/flow/**`, `demo/**`, `web/public/demo/**`. Gate: `npm run check`
(tsc --noEmit) clean, `npx vitest run` 97 specs green (45 wave-1 + 52 new).

### What shipped

`view.scene`, `view.table`, `table.filter`, `table.sort`, `compute.expr` — all in
`web/src/flow/nodes.ts`, same `def(kind, fn)` shape as wave 1. `KINDS.length === 18`
and `NODES.size === 18` are now asserted against each other, so a declared kind with
no evaluator (or an orphan evaluator) fails the suite. `kindsJson()` reads `KINDS`
directly, so the 18-kind export needed no regeneration.

### Semantic decisions (extending the wave-1 rules)

1. **`table.filter` shares `select.byParameter`'s predicate verbatim** — the same
   module-level `compare()`. Null stays "absent": it fails every test except `!=`, and
   ordered ops drop non-numeric cells. One comparison table, two node kinds; a change
   to the null rule can never desync them.
2. **`table.sort` puts nulls last in *both* directions.** Descending is not "ascending
   reversed" — absence is not an extreme value, and a user flipping the arrow to find
   the smallest number should not be handed a screen of blanks. Sorting is stable
   (index-decorated), so successive sorts refine rather than scramble.
3. **`compute.expr` fails loudly at compile time and quietly per entity.** A syntax
   error is a node error (the whole graph node goes red with the JS message); a throw
   at one element, or a result that is not a finite number or a string, leaves that
   entity null. One bad element must not take a 10k-element model down. `NaN`/`Infinity`
   are dropped to null too — they would poison every downstream min/max.
4. **`ch()` vs `param()` keep the channel-shadows-parameter rule split.** `ch(n)`
   resolves channel-first-then-parameter (the rule everywhere else in the graph);
   `param(n)` is always the raw model value. That gives the one thing the shadow rule
   otherwise makes impossible: comparing an overlay against what it overrode
   (`ch('Area') - param('Area')`).
5. **Auto domain is resolved by `colorBy`, not `colormap`.** The colormap node has no
   scene, so it cannot know the data range; it publishes `{ramp, min, max, auto}` and
   `colorBy` — which has both — computes the effective domain and reports it on
   `ViewValue.domain`/`.ramp`. The `auto` flag rides an evaluator-internal
   `ColormapCfg extends ColormapValue` (in `flow/types.ts`), so `contracts.ts` stayed
   untouched. Degenerate `lo === hi` widens to `[lo, lo+1]`; a channel with no numeric
   values at all falls back to the configured min/max rather than erroring.
   The colorBy summary now carries the domain ("56 colored · 221–412"), which is the
   direct fix for wave-1 finding #3: the clamp is no longer silent.

### Two wave-1 assertions were pinned, not changed

`auto` defaults to **true** (per `kinds.ts`), so two beat-1 tests that were written
against a fixed 0–100 / 0–10 domain now say `auto: false` explicitly, and the one
summary assertion gained the domain suffix. No behavioural expectation was weakened —
the graphs now state the mode they were always testing. Wave-1 *demo* JSONs were left
alone: they get auto-ranging for free, which is the desired upgrade.

### New demos (kept byte-identical in `demo/` and `web/public/demo/`)

- `whatif-expr` — csv join → `compute.expr` carbon per m² → auto greenred colorBy,
  with a `view.table` → `sink.table` branch off the same scene.
- `inspect-flow` — two `view.scene` probes straddling the join, so the summaries read
  "N entities · no channels" then "N entities · channels: operational_carbon".
- `filter-sort` — scene → table → filter > 100 → sort desc → view → grid.

`demos.test.ts` walks *every* demo file (wave 1 and 2) and asserts: the two copies are
identical, node ids are unique, every param name exists in that kind's schema, every
wire connects matching port types, every declared input is wired, `display` names a
real node, and the whole graph evaluates with zero non-ok statuses against the mock
model. That last check is what makes a demo JSON a test artifact rather than a
liability — a renamed param or kind now breaks the suite instead of the UI.

### Handoff / not mine to fix

- The demo buttons are hardcoded lists in `web/src/main.ts:197` and
  `web/src/editor/harness.ts:78` (`["carbon-walls", "carbon-by-level", "sql-explore",
  "write-pset"]`), both outside this fence. **The three new demos ship on disk but do
  not appear in the UI until those two arrays gain `"whatif-expr"`, `"inspect-flow"`,
  `"filter-sort"`.** Worth making that list derive from a directory listing.
- The legend pane takes a `ColormapValue`, so it still draws the node's configured
  min/max. With auto on, the truthful numbers are now on `ViewValue.domain`/`.ramp` —
  the legend should prefer those when a view is displayed.
- `compute.expr` uses `new Function`. Fine for a local PoC; a shipped product wants
  either a sandboxed expression language or an explicit "this runs code" affordance.

### Track F — editor chrome + node help (wave 2)

No contract edits. New files `web/src/editor/{chrome,help}.ts`; touched
`{index,parts,panel,geom,harness}.ts`. Gate: 23/23 over private headless Chrome
(vite :5216, CDP recipe copied from `tools/intgate.mjs`), `npm run check` clean.

Shipped: top menu bar (title · Examples dropdown · Add Node · host status),
collapsible left node palette (18 kinds, 7 category groups, one-line description
per row, full description on hover, click adds at the viewport centre and keeps
the panel open, `P` toggles), node-header hover tooltip after 500 ms, a "?"
descriptor card in the inspector (description, ports as `WIRE_COLORS` chips,
every param with its `doc`), and the `view` category accent (cyan `#26d6dc`).
Also `EditorOpts.onSelect(nodeId | null)` for Track E's selection-driven preview.

**Chrome belongs to the app shell, not the editor — but only after the editor
gets a viewport seam.** `createEditor(canvas, kinds, opts)` had to grow
`chrome?: ChromeSpec` and re-export `setStatus`, which is the wrong shape: an
Examples menu is an *app* concern (it loads documents), not a graph-editor one.
The reason it could not simply live in the shell is that two things the shell
cannot compute belong to the editor: (1) "add a node at the centre of what the
user is looking at" needs `Runtime.toWorld` plus the canvas size, and (2) the
on-canvas HUD has to be inset below whatever the shell drew on top of it — wave 1's
floating demo buttons covered the "+" button precisely because neither side knew
about the other. The real design should expose the viewport (`worldCenter()`,
`toWorld/toScreen`, and a `setInsets({top,left,right})`) on the editor handle;
then chrome is ordinary shell HTML and the editor stops knowing what a menu is.

**Documentation on `NodeKindInfo` paid for itself three times over.** One
`description` + one `doc` per param drives the palette subtitle, the `title=`
hover, the 500 ms tooltip and the descriptor card, with zero UI-side copy. Two
notes for the real vocabulary: the "first sentence is the short form" convention
in the contract comment does not survive contact — several descriptions have a
first sentence 100+ chars long (`load.model`, `attach.column`), so the palette
truncates mid-sentence. Give kinds an explicit `short` (≤ 60 chars) rather than
deriving one. And `ParamSchema.doc` should be *required*: the card renders
"(undocumented)" and there is nothing that forces an author to fill it in.

**The panel's rebuild-only-on-selection-change policy needed one adjustment, not
an exception.** The "?" state has to live *outside* the rebuild (a closure
`helpOpen`), because the card is recreated on every selection change. That is the
general rule for this seam: anything the user toggled is panel state, not node
state, and must survive the rebuild. Nothing else about the policy changed, and
live SQL typing still keeps focus.

**Hit-testing a node from outside Gratify is trivial and that is a good sign.**
The hover tooltip does not touch the runtime's scene at all: it converts the
pointer with `rt.toWorld` and tests `node.x/y` against `NODE_W`/`HEADER_H` from
`geom.ts`. Because geometry is a pure function of the doc, external chrome can
answer "what is under the cursor" without a picking API. Keep node geometry
derivable from the document.

**`CATEGORY_COLOR` is a `Record<category, Color>`, so adding the `view` category
to `kinds.ts` broke the build — correctly.** Exhaustive records over a union are
the cheapest possible "you added a case" alarm; the same edit needed a CSS twin
(`CATEGORY_CSS`) because the HTML chrome cannot use painter `Color`s. In the real
design the categorical palette should be declared once as hex and converted for
the painter, not maintained twice.

**Two known rough edges, deliberately left.** The open palette panel covers the
canvas "+" button (the HUD is inset vertically but not horizontally — the editor
would need the `setInsets` seam above to do it properly), and the palette adds
every node at the same viewport-centre point, so repeat adds stack on top of each
other until you drag them apart. A real palette should cascade or drop at the
last click.

## Wave 3 — node-as-inspector, Snowdon default, Ask AI + Bar Chart (2026-08-09)

Three parallel tracks (editor / flow / host) off supervisor contract edits. Gates after
integration: tsc clean, vitest 110/110, intgate 27/27, host smoke 16/16.

### Contract changes

- `PortSpec.doc?` — every port in kinds.ts now documented; socket hover shows
  `name · type — doc`.
- `NodeStatus.chart?: {labels, values, title?}` + `NodeStatus.detail?: string` —
  eval results can carry per-node visualization data (chart.bar) and a detail line
  (table.ask's generated SQL). `NodeOut` mirrors both.
- `NodeKindInfo.bodyHeight?` — kinds that render content in the node body (chart.bar: 96).
- `ParamSchema` modelPick gained `default?`; `load.model` defaults to `"snowdon"`.
- `EvalCtx.ask?(modelId, question) → {sql}` — optional LLM sidecar seam; evaluator
  errors the node with "host AI sidecar unavailable" when absent.
- New kinds: `table.ask` (Ask AI), `chart.bar` (Bar Chart) — 20 total.

### The side panel is gone; the node is the inspector

`panel.ts` deleted. `geom.ts` rebuilt around one pure `nodeLayout(info, helpOpen,
status)` shared by rendering AND hit-testing — header / help expando / socket rows /
param rows / chart body / footer. Params render as `name: value` rows (schema default
dim when unset); clicking a row opens a floating DOM popover (ported field builder;
text params get a 360px monospace textarea) that live-commits and closes on
Esc/click-away/pan/zoom/selection. Help expando under the title shows description +
status.detail + error; per-node `?` chip plus menu-bar Help ▸ Show all / Hide all.
Footer carries status accent + summary, Run (effect:write), eye, ✕. Socket and bar
hovers reuse the single 500ms tooltip. createEditor signature UNCHANGED — main.ts
needed zero edits for the editor swap.

Findings worth keeping:
- Help toggling shifts socket Y; nothing broke because every consumer uses the same
  layout function. Geometry as a pure function of (doc, ui state) remains the
  load-bearing invariant.
- Deterministic char-budget text wrap (not `measure.text`) is what keeps layout pure
  and shared with hit-testing.
- **Gratify wart**: window-level keydown has no editable-focus guard — Backspace in any
  DOM field deleted the selected node (latent since wave 1). Worked around in the
  editor's dispatch wrapper; real fix belongs in Gratify.
- CDP `Input.dispatchMouseEvent` is required for gate input: untrusted synthetic
  PointerEvents make Gratify's `setPointerCapture` throw.
- Popover-over-canvas beats a side panel structurally: close-on-pan/zoom falls out of
  canvas pointerdown/wheel for free.
- `tools/edgate.mjs` (from Track A) = standalone editor gate vs harness :5216, 24 checks.

### Evaluator

- `withDefaults` now applies `ParamSchema.default` before defs run — previously NO code
  applied defaults at eval time (bare `load.model` errored instead of using the
  default). One errors.test expectation updated accordingly.
- chart.bar column heuristic: first mostly-string column = labels, first mostly-numeric
  (numeric-looking strings count) = values; top 24 rows in input order, truncation noted
  in summary. An "id" column of "1","2","3" counts numeric — set columns explicitly then.

### Host

- Snowdon Towers Architectural (95MB IFC) converts in ~12s — duplex's 45s is not
  size-proportional. 19,382 entities; BOS 5.4MB. Same legacy-parameter repack path as
  duplex, handled unchanged.
- **`data/models.json` is regenerated by the host at startup** (`DataSetup.WriteModelsJson`)
  — add models in `DataSetup.Prepare`, never by editing the JSON. Snowdon is first,
  and load.model's schema default makes it the default model.
- `/api/ask` (+ 10th MCP tool `ask`): Anthropic Messages API via raw HttpClient, key
  `ANTHROPIC_API_KEY`, model `ANTHROPIC_MODEL` fallback claude-sonnet-5. Prompt embeds
  live view schemas introspected via `information_schema.columns` (DESCRIBE fails the
  SELECT-only guard), cached per model. Fence-strip + refusal check + reuse of the
  /api/sql single-statement guard. **Happy path untested — no key on this machine**;
  no-key and bad-model error paths verified; smoke step 14 auto-exercises the full path
  once a key exists.

### Ask AI happy-path verified + category-case fix (2026-08-09, key installed)

First live run: LLM wrote `Category = 'IfcDoor'` — but Category values are stored
UPPERCASE (`IFCDOOR`), so the query ran clean and returned 0 rows. Silent-empty, not
error: the worst failure mode for an NL→SQL node. Fix: the system prompt now embeds the
model's actual Category values with counts (top 60 by frequency, cached per model) plus
an explicit uppercase gotcha. Re-test "How many doors are on each level?" → correct
9-storey door counts via a RelationText self-join. Lesson for the real design: schema
introspection is not enough — enumerable low-cardinality VALUES (categories, levels)
belong in the prompt, because that's what the model would otherwise guess at.

## Wave 4 — analytics demos: quality-audit, cost-estimate, disciplines (2026-08-09)

Three demos built agent-side against live SQL, wired + gated by supervisor. Gates:
tsc clean, vitest 119/119 (+9 demo specs), intgate 30/30, host smoke green.

- `quality-audit` (8 nodes): snowdon has NO unnamed entities and EntityText has no
  Level column — the audit pivots on placement coverage (ContainedIn relation; IFCPLATE
  and IFCMEMBER are 0% placed — 2,087 unplaced elements), missing Volume/Area params,
  and untyped elements (all 142 railings). Real findings, zero external data.
- `cost-estimate` (9 nodes): `data/unit-costs.csv` is DERIVED AT STARTUP by
  `DataSetup.WriteUnitCosts` (deterministic: per-category rate × real Volume/Area from
  ParameterText, flat rate fallback; 5,816 rows, ≈2.77M total) — the codebase's
  self-provisioning pattern, since data/* is gitignored. Join by GlobalId, auto-heat
  colorBy, per-level aggregate runs client-side (EntityText has no Level).
- `disciplines` (10 nodes): snowdon-hvac + snowdon-struct added via PrepareIfcModel
  (3.9s/2.4s converts). Three side-by-side in-node bar charts show visibly distinct
  discipline profiles (arch: IFCMEMBER/IFCWALL; hvac: IFCDISTRIBUTIONPORT 5,579; struct:
  IFCBEAM 942). Cross-model comparison without any geometry math.
- Gate lesson: node ids repeat across demos (n1..nN), so "sink ok" waits can pass
  against the PREVIOUS demo's stale result map. Wave-4 checks wait on the chart payload
  (only the new demo produces it). General rule: demo-switch checks must key on data
  only the NEW doc can produce.
- demos.test.ts discovers demos by directory scan (no hardcoded list) — but its stub
  CSV must carry any column a demo joins (gained `cost`).
- chart.bar in two demos deliberately leaves label/value columns unset so the
  column-guess heuristic keeps them green against the mock's canned SQL too.

## Wave 5 (UX plan W0) — contract seams, geom freeze, headless harness (2026-08-09)

Executing docs/platoflow-ux-implementation-plan-2026-08-09.md. W0 = supervisor seams
+ mechanical agent. Gates: tsc clean, vitest 306/306 (was 130 after undo specs; +166
geom goldens, +10 drive specs; full run 2.6s), edgate 24/24, intgate 30/30.

- Contracts: `ParamSchema.commit` ("live"/"explicit"; the three text params are
  explicit), `NodeKindInfo.width` (240 on sql/ask/chart), `batch` intent (atomic,
  one undo entry), viz.colormap bodyHeight 40 reserved for the gradient body.
- Reducer: no-op guards on removeNode/disconnect — they used to return new docs for
  misses, which would have polluted undo history (caught by the undo specs).
- Undo: snapshot stacks in editor doc.ts (past/future, structurally shared), coalescing
  keys move:<id>/setParam:<id>:<name> so a 60-event drag is one Ctrl-Z, load clears,
  cap 200. 11 pure specs.
- geom.ts: ONE signature change (`nodeLayout(info, {params, wiredInputs},
  {helpOpen, zoom}, status?)`, paramRows row table, widgetFor seam, width honored,
  zoom accepted-and-ignored — spec proves it) then FROZEN. parts.ts split into
  cards/wires/surface/palette/widgets + re-export shim.
- Headless harness (tests/drive.ts): real editor app on Gratify headless Runtime,
  all coordinates via nodeLayout. Findings that shape W1 briefs: (1) DOM organs
  (popover/tooltip/chrome) don't mount headless — jsdom owns that half; (2) a drag
  across a param row starts the card-move gesture, so T1's scrub must CLAIM the
  gesture first or scrubbing drags the node; (3) position springs glide — specs
  needing exact rects call settle(); (4) keyboard routing is hover-chain — position
  the pointer before key specs; (5) palette rows clickable by instance key.
- edgate lesson: only 1 of 24 checks broke on the width change — hardcoded
  `x + 188 - 40`; fixed via `window.gratify.query.anchor(...)` right-edge lookup.
  Generalizes: width-dependent browser-gate coords can come from live anchors.
- Gratify upstream A (T6) pulled forward — blocked nothing: island() facet +
  AppSpec.onCommit landed upstream (4b61139, 54/54), keyboard guard was e543ef0.
  T14 notes: island is a facet (identity-stable el, null hides), islands mount on
  first tick not at mount(), wheel over an island doesn't zoom the canvas.

### W1 T4 (undo UI) — package note (2026-08-09)

- `web/package.json`: added devDependency `jsdom` (chrome-edit.spec.ts runs the
  Edit menu under `// @vitest-environment jsdom`; vitest needs jsdom as a real
  dep, it was only transitive before). Run `npm install` after pulling.

## Wave 6 (UX plan W1) — Phase 1 complete, fat gates retired (2026-08-09)

Six parallel tracks (T1-T5, T7) + T6 upstream, joined by the supervisor. Gates after
join: tsc clean, vitest 433/433 (~7s), edgate-smoke 5/5 in 2.9s, intgate-smoke 13/13
in 8.0s (Ask AI answered live). Fat gates DELETED (edgate 24 checks ~1-2min,
intgate 30 checks ~1-3min) — the plan's measurable claim held: inner loop is
Chrome-free and the browser surface shrank to ~11s total at joins.

Shipped: in-row widgets (numbers scrub — 2px/step, Shift fine, one undo entry per
gesture; booleans toggle; ≤5-option enums chip), explicit commit for SQL/expr/question
(buffer, Ctrl-Enter/blur commit, Esc revert, close-commits — an edit is never lost),
docked focus editor with schema chips + result preview + reserved AI-assist slot,
link-drag-search both directions (type-filtered palette, batch = one undo), splice-on-
wire with hover highlight, palette cascade (stacking wart dead), Ctrl-Z/Y + Edit menu,
colormap gradient body (click cycles ramp).

Join lessons:
- The widgetFor fold (T1's mapping into frozen geom.ts) broke exactly the specs that
  SHOULD break: 16 goldens (widget names only, offsets identical) + 2 W0 pins. The
  goldens did their job — they proved the fold changed classification, not geometry.
- Gesture-claim order (wire → widget → move, decline-by-null) is the load-bearing
  mechanism for in-row widgets; T3's relocation of card gestures into wires.ts and
  T1's widget gesture composed without conflict in the same file wave.
- Coalescing quirk pinned by spec: consecutive same-param toggle clicks merge into
  one undo entry (histKey survives across gestures). Fix candidate: histKey break on
  gesture end. Deliberately deferred — T5's ramp clicks sidestep via batch.
- Vec is structural, so the palette's pending-link payload rides the existing
  {k:"palette"} intent with zero contract change.
- T2/T7 spec-file collision (both wrote params.spec.ts) resolved by merge mid-wave;
  fences should name spec files, not just directories, in W2.
- capture-listener registration order is load-bearing for Esc-revert (params.ts
  registers before index.ts); T14's island()/focus work should subsume it.
- jsdom pinned as devDep (npm install needed after pull).
- Parity gaps accepted at retirement (T7): real pset write is host/smoke.ps1's job;
  beat3 row-click→viewer highlight has no check until T11; tooltip/menu pixel truth
  is jsdom-only now.

### W2 T13 — multi-select / copy / paste / duplicate / align (2026-08-09)

Shipped: Selection extended with `{kind:"nodes", ids}` (invariant ids.length>=2 —
`selFromIds` collapses 1→node, 0→null so every pre-T13 single-selection path keeps
its shape; `selectedNode` returns undefined for multi, so the popover closes and
onSelect reports null by design). Shift-click toggles membership; SHIFT-drag on
empty canvas is the marquee (binding choice: plain drag stays the Pan() interactor —
a declined marquee begin falls through to pan, zero conflict). Dragging any member
moves the whole set (one batch per event, coalesced per-drag by a `moveSet:<id>:<seq>`
key → exactly one undo entry per drag); drags snap to other nodes' left/top edges
within 6px with guide-line overlays (`snapMove` pure in surface.ts). Ctrl-C/V/D =
in-memory NodeClip (params deep-copied, internal edges as index pairs); paste is ONE
`batch` (fresh reducer ids, +24/+24 from originals, edges remapped, copies become
the selection); alt-drag duplicates likewise (ghost boxes while dragging, originals
never move — the copy materializes on release). Delete removes the set as one batch.
Gates: tsc clean, vitest 503/503 (multisel.spec.ts +14).

- **wires.ts touch (T11's fence, coordinated-minimal):** nodeMoveGesture now
  delegates through a `registerMoveExt` seam (~35 lines, same module-state pattern
  as registerScene); ALL set-move/snap/alt-dup logic lives in surface.ts. A null
  extension reproduces pre-T13 behavior exactly. Landed after T11's badge/hover
  code; no region overlap.
- Card `.press` can't see modifiers, so shift-select rides a one-shot
  `setPressShift` channel filled by the move gesture's begin (which gets Query.mods)
  and consumed by the "select" step. Consequence: shift-click toggles membership on
  headers/plain rows only — widget rows (scrub/toggle/chips) claim their own presses
  and shift already means "fine scrub" there.
- Set-move offsets are captured at begin (relative to the grabbed node), not
  recomputed per event — sidesteps the one-frame staleness of the view-registered
  doc during live drags.
- Runtime mods are sticky (`Object.assign` per event): headless specs MUST reset
  shift/alt with a trailing pointerMove or the next plain click still sees them.
- HUD (+ button at 12,12) is interactive and sits above the surface: marquee specs
  must start drags off the HUD or the press lands on the button.
- Set release never splices; single-node drags keep splice + the frozen cross-drag
  move coalescing byte-identical (drive.spec pins both).

## W3 T16 — subgraphs with promoted ports (2026-08-09)

Fence: NEW `editor/subgraph.ts`, `editor/surface.ts` + small index.ts region,
`flow/nodes.ts` + `flow/evaluate.ts` (+ `flow/types.ts` NodeOut.outputs),
`editor/geom.ts` narrow delegated edit, spec files. Kea G/U/enter interaction
ADOPTED from labs/kea (input.ts/document.ts), not redesigned. Gates: tsc clean,
subgraph.spec 21 + flow subgraph.test 8, all pre-existing suites green.

What shipped: `G` collapses a multi-selection into a graph.sub node at the
selection centroid (ONE batch = one Ctrl-Z), boundary crossings become promoted
ports named `<innerNode>.<slot>` (fan-out from one inner output dedupes onto one
port); `U` expands in place (inverse batch, exact positions back — see the
position convention below); double-click a subgraph header ENTERS (scratch-doc,
commit-on-exit; breadcrumb "root ▸ group" as a DOM organ), Esc / double-click
empty canvas exits a level. Evaluator: promoted inputs seed the inner
evaluation (`"<node>|<slot>" → Value`), promoted outputs surface per slot via
`NodeOut.outputs` (the evaluator's first multi-output node), inner errors fail
the node naming the ROOT culprit (`in subgraph: s1: …`), summary
"N nodes · M in / K out". Round-trip pinned: collapsed === uncollapsed
evaluation (same ViewValue colors), collapse→expand = identity including
positions, one-Ctrl-Z undo.

Findings for the real design:

1. **Dynamic arity fits the frozen layout with a 10-line shim.** `subInfo(info,
   sub)` merges the SubgraphSpec's ports over the static (empty) declaration;
   the VIEW hands every consumer the effective info, so cards/anchors/wire
   gestures/hit-tests all worked with ZERO edits to cards.ts or wires.ts.
   Geometry as a pure function of (info, state) pays off again — but note the
   consumers that build info themselves (help.ts tooltip, drive.ts layoutOf)
   silently use the static declaration and mis-size graph.sub cards. An
   `effectiveInfo(node)` seam should be THE lookup, not a convention.
2. **Position convention instead of an anchor field.** Kea stores `anchor` (group
   pos at collapse); SubgraphSpec has no such field, so inner x/y are stored
   RELATIVE to the group node — same information, zero extra contract. Expand
   after moving the group moves the set rigidly; collapse→expand is exact.
3. **The reducer paid for two subgraph behaviors for free**: removeNode clears
   `display` (display-flag-inside-selection just works) and clears crossing
   edges (the collapse batch needs no explicit disconnects). One-wire-per-input
   means promoted INPUT ports can never collide; only outputs need dedupe.
4. **History-per-level is the right undo policy** (Kea's rule adopted): enter/
   exit dispatch `load`, which already clears history. Undoable-enter would put
   history entries on a graph the user is no longer looking at. Collapse/expand
   are single batches, so they undo in one step WITHIN a level.
5. **The single-value evaluator needed per-slot outputs.** `EvalResult.values`
   is one Value per node; graph.sub is the first multi-output kind. Added
   `NodeOut.outputs?: Record<slot, Value>` + a per-slot lookup at edge
   resolution. The real design should key values by (node, slot) from day one.
6. **evaluate↔nodes is now a call-time import cycle** (nodes.ts calls
   `evaluateGraphSeeded`, evaluate.ts reads `NODES`). ESM live bindings handle
   it, but the real design wants the evaluator injected into node defs (or
   subgraph evaluation hosted in the evaluator itself).
7. **Known limitations, deliberate:** while ENTERED, main.ts evaluates the inner
   graph standalone, so nodes fed by promoted inputs show "missing input" (red)
   until exit — honest but ugly; portal pseudo-nodes (Kea's fix, which also
   makes boundary rewiring editable inside) are the upgrade path. MCP intents
   arriving while entered edit the inner doc. `copySelection` (doc.ts, T13)
   drops `sub` — pasting a subgraph node produces a broken empty group; doc.ts
   was out of fence, fix is one line (`sub: structuredClone(n.sub)` in clip
   nodes + pasteIntent). Chip-mode (zoom < 0.5) collapses promoted sockets to
   the header line like any node — fine.
8. **cards.ts wishes (T15/T17):** an enter affordance on graph.sub cards (corner
   glyph or "dbl-click to enter" hint — port names already render, they come
   from effective info) and a body line "N nodes" mirroring the summary.

## Wave 7 (UX plan W2) — Phase 2 + table stakes + upstream B (2026-08-09)

Six tracks (T8-T13), joined. Gates: tsc clean, vitest 503/503, edgate-smoke 5/5
(2.4s), intgate-smoke 13/13 (~11s).

Shipped: searchable picker with counts + auto-open on drop ("required unset" = empty
AND no schema default, so load.model never nags); mini-grid bodies on the six table
kinds (bodyHeight 88; tablePreview derived at eval in main.ts); semantic zoom (chip
mode below 0.5 — sockets collapse to the header line, width preserved); wire badges
(summary at midpoint, zoom-gated, error-tinted) + hover-a-scene-wire ghosts its
entities in 3D; multi-select/marquee(Shift-drag)/copy/paste/duplicate/align-snap,
all batch = one undo step; gratify upstream B (focus model + adorn z-tiers +
semantics slot, e3e603a).

User-reported polish, fixed at the join:
- **Adorn double-spring lag** (gradient/chips trailing a dragged card): overlay
  trees position from the host's already-sprung rect, then sprang again toward that
  moving target. Upstream fix 9e794ae: layoutScene(snapPos) — adorn/gesture roots
  pin to targets; enter/exit channels unchanged. Kernel test added.
- **Adornments missed the selection lift**: render raises the card by `3*ch.drag`
  but adorn positioned from the unraised rect — adorn now mirrors the lift formula.
  Lesson: any render-side offset must be shared with adorn/hit paths or it drifts.
- **✕ moved to the top-right corner** (was footer-right; standard corner wins),
  ? sits left of it, eye stays in the footer.
- **Adorn z-order**: the adorn layer paints above ALL cards, so a neighbor's chips
  drew over a dragged card. Interacted card's adornments now ride z-tier 2 (hover 1)
  via T12's tier(). Full per-host interleave = upstream follow-up.

Join lessons: picker rerouting of modelPick/dynamicEnum broke exactly the specs that
pinned the <select> popover (8 editor-dom + 1 edgate-smoke check) — retargeted to a
string param / .pf-picker; bodies2's six degrade pins flipped to positive 88px
assertions; 24 geom goldens grew bodyH (offsets otherwise identical). Thumbnails
DROPPED (T9): Painter has no image primitive (upstream) and the webgl buffer isn't
capturable outside its render tick (Track B seam) — degrades to count summary.

### W3 T15 — widget registry + per-instance node resize (2026-08-09)

Shipped: `WIDGETS` registry in widgets.ts (one entry per WidgetKind: row `draw`
+ optional gesture hooks; `widgetImpl(kind)` falls back to "row" for unknown
strings) — cards.ts's param-row switch and widgetGesture's begin/move/during/up
switch are GONE, both route through the registry keyed by geom's frozen
`widgetFor` output. Behavior pinned byte-identical (widgets/drive/chips suites
untouched and green). `paramDisplay` moved cards→widgets (cards re-exports) so
the "row" entry draws without a cycle.

Per-instance resize: `resizeGesture` (widgets.ts) on the right-edge 6px band +
bottom-right 12px grip (`resizeHandleAt`/`resizeGripRect` in geom.ts — render
and gesture share the zone). Declared wire → resize → widget → move; dispatches
`resize` per move (doc.ts `resize:<id>` key = one undo per drag), clamps to
[NODE_W, NODE_W_MAX=480], never moves the node. Double-click the grip resets to
`info.width ?? NODE_W` — WART: the resize intent cannot express "unset", so a
reset stores the default as an explicit w. Chip mode (zoom) has no handle. Eye
chip moved 14px left (r.right−40) — the corner is the grip now. geom.ts
delegated edits: `LayoutNode.w?` + `w = node.w ?? info.width ?? NODE_W` (both
branches) + the resize helpers. Zero geom golden churn.

CALLERS STILL TO WIRE (backwards-compatible optional field, their files were
in-flight): surface.ts makeView must pass `w: n.w` BOTH into its nodeLayout
call (nodeBoxes) and into NodeCard props, and index.ts `layoutOf` must add
`w: n.w` — until then a resized card renders at its kind default even though
doc/undo/anchor math is correct (help.ts + tests/drive.ts already pass it).
resize.spec.ts computes press points from the LIVE instance rect, so its 11
specs stay green before and after that wiring. Gates: tsc clean, vitest
595/595, edgate-smoke 5/5.

## Wave 8 (UX plan W3) — upstream integration, subgraphs, resize, range slider, splitter (2026-08-09)

Six tracks (T14/T15/T16/T18/T19 + the interrupted-and-resumed set after a network
outage killed three agents mid-flight; SendMessage-resume from transcripts worked
cleanly — partial edits stayed coherent). Gates at join: tsc clean, vitest 595/595,
edgate-smoke 5/5, intgate-smoke 13/13.

Shipped: island-backed popover/picker (survive pan/zoom, track dragged nodes;
close-on-pan deliberately deleted); dispatch wrap shrunk to one argued guard;
Esc = explicit cancel() pathway; subgraphs (G collapse / U expand / dbl-click enter,
promoted ports via ONE subInfo effective-info seam, seeded nested eval, per-level
history); per-instance node resize (right-edge grip, resize intent, one undo per
drag) + widget registry; colormap range slider (auto off: thumbs + window drag, one
undo each; gratify Range part upstream bef0de6); pane splitters (col/row, persisted,
min clamps) + viewer resizeDelay 200→40ms.

Join fixes:
- makeView/layoutOf now pass `w: n.w` (T15's flagged wiring); clipboard preserves
  `sub` + `w` (paste of a subgraph or resized node round-trips; w re-applies via a
  trailing resize intent — addNode can't carry it).
- Focusable() + focus ring on NodeCard (T14's diff), Enter opens the focused card's
  editor.
- **Upstream Escape-routing bug found by a poc spec**: a focused part consumed
  Escape to clear focus, so app-level Escape maps (palette dismiss) needed a second
  press. Fix: clear focus but KEEP ROUTING to the root key maps (still consumed for
  preventDefault). The linkdrag Esc-cancel spec caught it the moment cards became
  focusable — exactly the cross-layer regression the headless suite exists for.
- T18 pushed its poc commit mid-wave (c41c943) against the join protocol — audit
  showed pathspec-clean (its 2 fence files only), no harm; briefs should restate
  "no push" on resumes.

T16 findings worth design attention: evaluator values should be keyed (node, slot)
from day one (graph.sub is the first multi-output node and needed a bolt-on slot
map); an `effectiveInfo(node)` seam should be THE kind lookup (help.ts tooltip and
drive.ts still mis-size graph.sub cards); while entered, the inner graph evaluates
standalone so promoted inputs show "missing input" until exit (portals fix it).

### Wave 8 addendum — T17 visual QA (screenshot-driven)

All four user complaints reproduced against real pixels, fixed draw-only (zero
layout/golden churn, 595/595):
- "Messy top border at zoom" = the full-width radius-3 accent strip poking ~3px past
  the card's radius-10 corner curve AND covering the top border stroke. Fix: inset
  rounded accent bar (both modes). True full-bleed cap needs painter per-corner
  radii/clip — upstream feature, listed.
- "Strange dot under the ✕" = the header status dot colliding with the W2-relocated
  delete chip (error state read as a red badge ON the delete button). DELETED — status
  has one home now, the footer accent line + text; the pending pulse moved there.
- Cheap wins: resize grip hover-brightens, graph.sub gets a "dbl-click to enter" hint,
  colormap auto-tag contrast.
- Adorn-vs-control analysis (for the real design): every wave-7/8 adorn bug was the
  same root — adorn is a parallel scene mirroring the host BY CONVENTION (lag, lift,
  z-order = three manual mirror rules). Principle: adorn = transient non-interactive
  overlays only; interactive node content belongs in the node's own part tree (the
  widget registry is the right shape; needs gratify parts-as-render-children).
  Litmus: if it has a gesture, it's a control, not an adornment. Z-tiers are a patch;
  host-relative interleave is the fix. Islands correctly separate (DOM must be).
- Known wart left: wire badges overlap sockets on short wires (~1h, wires.ts).

### Wave 8 addendum — vertical resize + resize cursors (2026-08-09)

Body-carrying nodes are now vertically resizable: `GraphNode.bh` override, bottom
band on kinds with a body only, corner grip drags both axes, clamps [40, 400],
double-click resets both, one undo per drag (the resize:<id> key covers both axes),
clipboard round-trips bh. Cursors: hover hit-tests resizeHandleAt → ew/ns/nwse-resize,
pinned during drag via the splitter CSS-class pattern; a socketAt pre-check keeps
the cursor honest where out-sockets overlap the right band. GRID_MAX_ROWS 5→29
(derived from BODY_H_MAX) so a taller mini-grid actually shows more rows — note:
/api/state now pushes up to 29 preview rows per table node. Known gap (pre-existing
effectiveInfo class): help.ts tooltips don't pass bh, so tooltips on a stretched
node mis-size. 612/612, edgate-smoke 5/5.

## Wave 9 — design-gap wave (2026-08-09): honesty, set algebra + grouping, linking, persistence

Closing gaps against docs/platoflow-ifc-design.md (§1.4 channel record, §3 node rules,
§3 set algebra + GroupBy, §4.4 reverse viewport link, §7 save/load, §6 export).
Supervisor pre-landed: ChannelValue record migration, SceneValue.groups, needs-setup
state, NodeStatus/NodeOut warning, ViewValue.legend, TableValue.source, groupKeys
enum source, "group" category, 7 new kinds (stubbed), flow/nodes.ts split into
registry/lib/defs-core/defs-viz/defs-sets/defs-export, NeedsSetup error type.
Baseline after pre-work: tsc clean, vitest 668/668 (4 colorBy geom goldens updated
for the new mode param; 28 new-kind goldens written).

(track findings below)

### W9-A — flow semantic honesty

Fence: `flow/{defs-core,evaluate,lib,csv}.ts` + nine test files + NEW
`tests/honesty.test.ts` (17 specs). Gates at hand-off: tsc clean, vitest **750/750**
(668 baseline + Track B's 61 mid-wave + 17 honesty + 4 net from split/updated pins).
Commit 8c5c173.

**lib.ts edits other tracks should know about (both additive):**
- `compare(cell, op, raw, onDrop?)` — 4th optional callback fires when an ordered op
  drops a NON-null cell because it is not numeric (null never counts). Existing
  3-arg callers unaffected.
- `columnIndex(table, name, what)` — the unset-name branch now throws
  `NeedsSetup("choose a <what> column")` instead of `Error("no <what> column
  selected")`; the missing-column branch is unchanged. Any def calling it with a
  possibly-empty name now reports needs-setup, which is the intended semantics.

**Message formats settled (pin these in the real design):**
- direct needs-setup: setup-flavored imperatives — "choose a type", "choose a level",
  "choose a parameter", "choose a model", "choose a value column", "choose a
  group-by column", "choose a filter/sort column", "enter a csv url", "enter a
  query", "enter an expression", "name the output channel", "ask a question".
- unwired input: state "needs-setup", message `missing input "x"` (text unchanged
  from wave 1 — only the state moved).
- poison, upstream needs-setup: state "needs-setup", message `waiting on <rootId>`.
- poison, upstream error: state "error", message `upstream error in <rootId>:
  <rootMessage>` — rootId/rootMessage are the ORIGINATING node, carried through a
  per-run root-cause map (a node failing directly is its own root; a poisoned node
  copies its upstream's root). Cycle members stay "cycle detected" (own root);
  hangers-off get `upstream error in <cycleId>: cycle detected`.
- precedence when one node has both kinds of bad upstream: error wins (red beats
  gray — a real failure should never be softened to "waiting").
- graph.sub propagates inner state: inner root error → node error `in subgraph:
  <id>: <msg>`; inner root needs-setup → node needs-setup, same text. Root found by
  "message does not start with 'upstream error in '/'waiting on '".

**Dropped-row honesty:** `select.byParameter` → `N entities dropped as non-numeric`,
`table.filter` → `N rows dropped as non-numeric`, only when N > 0, via the compare
callback — one comparison table still serves both nodes. Deliberate scope: a
non-numeric cell counts; a non-numeric *comparison value* (`Area > "abc"`) does not
(that is a config problem visible as 0 matches, not silent data loss).

**Shadow + provenance:** `attach.column`/`compute.expr` warn `channel "X" shadows
model parameter` (joinWarnings joins multiple with "; " — currently each node has
at most one). Both now fill `ChannelValue.numeric`: true iff ≥1 non-null cell and
every non-null cell passes `asNumber`. `unit` remains unfilled — nothing in the PoC
knows units; the real design needs a source for it (IFC property type? CSV sidecar?).

**Per-column CSV typing (replaces quote-driven):** a column coerces to numbers iff
every non-empty cell is numeric-looking AND the header is not id-like
(`/(^|_|\s)(id|gid|guid|globalid)$/i`); otherwise all cells stay strings, quoted or
not. Empty cells are null either way (previously an unquoted empty cell was `""`).
Consequence worth stating: quoted `"007"` under a non-id header now becomes 7 —
quoting no longer protects identifiers, the header name does. The id-like regex is
a heuristic and the design should let a user override column types explicitly.

**Surprises / friction:**
- `table.sql` (and `table.ask`, `compute.expr`, `load.model`) have schema DEFAULTS,
  so through the evaluator their required params are never empty — the needs-setup
  branch in those defs is reachable only by explicitly setting "" (tested via direct
  def call). Needs-setup and param defaults pull against each other: a kind with a
  default never shows "needs setup", which is arguably the design intent (SQL node
  arrives runnable) but means the state mostly signals *unwired inputs* in practice.
- Track B's colorBy rewrite landed mid-wave: auto mode now renders text channels
  categorically, which retired wave2's "falls back to configured domain when no
  value is numeric" pin as written — the numeric fallback still exists but must be
  forced with `mode: "numeric"` (test updated accordingly). Cross-track test
  ownership (my fence pins their behavior) is the friction; fences should name
  behavior owners, not just files.
- demos stayed green with zero edits: every demo sets its params, so needs-setup
  never appears — confirming the state split cost nothing on working graphs.
- `inferNumeric` and chart.bar's `guessColumn` and csv's column inference are now
  three slightly different "is this column numeric" rules (all-non-null vs majority
  vs all-non-empty). The real design wants ONE column-type oracle on TableValue /
  ChannelValue, computed once at the boundary.

### W9-D — legend rework: effective domain + categorical swatches

Fence: `web/src/panes/**` only (viewer untouched — no polish needed, ViewerBridge
unchanged). `panes/index.ts` + NEW `panes/tests/legend.spec.ts` (15 jsdom specs).
Gates: tsc clean in-fence, panes suite 15/15; full-run numbers unstable during the
wave (flow/** failures + one defs-viz tsc error observed = W9-A/B mid-edit, out of
fence). Commit dc57ed2.

Shipped: `showViewLegend(v)` on the panes object, routed through a pure exported
`legendModel(v): LegendPlan` — `{kind:"none"}` | `{kind:"categorical", title?,
entries:[{label, css}]}` | `{kind:"numeric", title, ramp, ticks:[max,mid,min]
pre-formatted}` — and ONE `renderLegend(plan)` shared with the legacy
`showLegend(c, label)` path (which now just builds a numeric plan; behavior pinned
byte-identical by spec). Numeric plans draw the gradient from the EFFECTIVE
`ViewValue.domain`/`.ramp`, closing the wave-2 handoff wart (legend showed the
colormap node's configured min/max while auto mode used another domain).
Categorical: one chip+label row per `legend[]` entry in given order, scrollable
(`max-height` + overflow) for Track B's 24+"(none)" cap, title from `v.label`.

Decisions the contract does not state (pinned by spec, supervisor may want them
in contracts.ts):
1. **`legend[]` wins over `domain`+`ramp` when both present** — a categorical
   view's numeric domain is meaningless. Comment on `ViewValue` says nothing.
2. **Empty `legend: []` renders categorical-with-no-rows, NOT the gradient
   fallback** — "present but empty" is truthful (zero groups), falling through
   to a numeric domain would lie about the mode.
3. **`legend[].color` range is undocumented** — assumed 0..1 floats (matches
   `ViewValue.colors` and `ramp()` output). The contract comment should say so;
   a 0..255 producer would render near-black chips with no error.
4. `domain` without `ramp` (or vice versa) clears — numeric needs both.

jsdom gotcha for future pane specs: cssstyle normalizes color serialization
(`rgb(255,0,0)` → `rgb(255, 0, 0)`) and strips the default `to bottom` from
linear-gradients — expectations must round-trip through an element style
(`cssNorm` helper in legend.spec.ts) or they fail on spacing, not substance.

### W9-B — set algebra, group.by, table.count, categorical colorBy

Shipped: the six stubs in defs-sets.ts + the colorBy mode split in defs-viz.ts;
new setops/groupby/categorical test files (39 tests); demos set-algebra.json +
group-color.json (+ public copies). Gates at handoff: tsc clean, vitest 768/768
(41 files) including demos.test over both new demos.

Design data points (§14 open question, decided here):
1. **Channel merge on union/intersect = LAST-WINS + warning.** `{...a.channels,
   ...b.channels}`; a name bound to two DIFFERENT ChannelValue objects warns
   `channel "X": second input wins` (joined with "; "). The SAME object arriving
   down both branches of a diamond is NOT a clash — reference identity is the
   test, so re-converging a split pipeline stays silent.
2. **subtract does NOT merge** — a's channels/groups pass through untouched,
   b is only a removal mask. This follows the kinds.ts description ("The first
   input's channels pass through"), which contradicts the track brief's blanket
   merge-all-three wording; kinds.ts read as deliberate. Supervisor: confirm.
3. **groups on union/intersect: first input wins** (`a.groups ?? b.groups`), no
   warning on a groups clash — one partition per scene, silent preference.
4. **Categorical legend order**: distinct values by first appearance over the
   ascending selection; "(none)" appended LAST when any selected entity is
   null/empty. Values beyond the 24 cap render gray but are NOT in the legend
   (the warning `N values beyond 24 render gray` carries the fact); summary
   group count (`N colored · M groups`) counts ALL distinct non-null values,
   uncapped, and does not count "(none)".
5. **Palette**: golden-angle hue walk implemented locally in defs-viz.ts —
   `hsl((k*137.508)%360, 0.65, 0.55)` → RGB 0..1 floats (matches W9-D's
   assumption 3). Gray = [0.35,0.35,0.35], same constant as the numeric path's
   null gray.
6. **auto mode tiebreak**: majority-numeric over the selected entities'
   NON-EMPTY cells (`nums*2 > filled`, guessColumn's rule), so a sparse numeric
   channel (6 numeric + 18 null) still ramps numerically; an all-null source
   goes categorical (legend = just "(none)"). Group labels NEVER ramp — mode
   auto + groups source is always categorical, even for numeric-looking labels.
7. group.by needsSetup("choose a grouping key"); viz.colorBy with no channel and
   no groups now needsSetup("choose a channel") — aligned with W9-A's defs-core
   conversion mid-wave (was fail "no channel selected"; no other test pinned it).

Mid-wave friction: none beyond the expected — W9-A's pre-adapted wave2.test
(FireRating fallback pinned to mode:"numeric") landed before my defs-viz change,
so the auto→categorical flip broke nothing. lib.ts needed no additions; the one
gotcha re-learned: `fail()` in statement position does not narrow (const lacks
an explicit never annotation) — use `x ?? fail(...)` expression form.

### W9-E — host persistence, CSV export, dynamic Examples menu

Shipped: `host/GraphStore.cs` + three HostApi routes; smoke.ps1 16→22 steps (all
PASS); `chrome.ts` dynamic Examples + "Save graph…"; `defs-export.ts` real (pure)
evaluate + export.test.ts (4 specs) + 3 chrome.spec additions. Gates: tsc clean,
vitest 780/780, smoke 22/22. Commits 0e77be8, cdebe8d, 2ab2970.

Endpoint shapes as implemented:
- `GET /api/graphs` → `{demos: string[], saved: string[]}` — basenames, no
  extension; demos from `<root>/demo/*.json`, saved from `data/graphs/*.json`,
  both sorted case-insensitively.
- `GET /api/graph?name=X` → the graph JSON verbatim; `data/graphs/` searched
  before `demo/`, so a user save SHADOWS a demo of the same name ("save over the
  example I started from"); unknown → `{error: 'no graph "X"'}`.
- `POST /api/graphs {name, doc}` → `{ok: true, name: <sanitized>}`; doc written
  pretty-printed (diffs like the checked-in demos); overwrite deliberate.
- `POST /api/export-csv {name, table: {columns, rows}}` →
  `{outPath, rows: N}`; lands in `data/out/<sanitized>.csv`, extension forced.
  Quoting: strings quoted (doubled quotes) only when they contain comma/quote/
  newline; numbers and booleans bare via `ToJsonString()` (culture-safe); null
  empty. Round-trips through `Import-Csv` including an embedded newline.
- Sanitizer: keep `[A-Za-z0-9 _-]`, trim; empty result = `{error}`. A trailing
  `.json`/`.csv` is stripped BEFORE sanitizing (dots are eaten, so "export.csv"
  must become `export.csv`, not `exportcsv.csv`). `"../evil"` collapses to
  `evil` — no rejection needed, the name simply cannot spell a path.

Findings:
1. **Directory-as-registry proved itself mid-wave, zero coordination.** W9-B's
   set-algebra + group-color demo JSONs appeared in `/api/graphs` (10→12 demos)
   the moment they hit disk — design §7's "graphs in a folder ARE the workflow
   library" confirmed with no registry code. The hardcoded demo arrays wave 2
   complained about (main.ts:197, harness.ts:78) can now die at integration by
   pointing `ChromeSpec.getExamples` at `/api/graphs`.
2. **HostApi extension friction: none.** Static-class-per-feature + one switch
   case per route took the three endpoints in 15 lines of HostApi diff. The
   HTTP-200-`{error}` convention paid again: sanitize failures and unknown
   names ride the same channel the browser already renders.
3. **JSON round-trip identity holds through JsonNode**: System.Text.Json
   preserves member order parse→write, so the smoke's identical-JSON check
   (PS `[ordered]` on the send side, compress-compare after reload) passes
   byte-for-byte on values AND order. The real design should not rely on this
   silently — state it or canonicalize.
4. **Pathspec-commit gotcha (for wave briefs): `git commit -- <paths>` silently
   skips UNTRACKED files.** The host commit landed "2 files" without the new
   GraphStore.cs; needed explicit `git add <file>` first. Cost one follow-up
   commit; a brief line "git add new files before pathspec commit" would
   prevent it.
5. **Chrome rebuild-on-open is a second same-element listener** registered
   after `trigger()` — stopPropagation does not stop same-element listeners,
   so the rebuild sees the just-toggled open state. Stale async fills are
   dropped via a sequence counter; the Save item is rendered into the pending
   state too, so save never waits on the host list.
6. **Live host + `dotnet build` fight over the exe** (MSB3027 lock). The
   brief's explicit restart rights were necessary, not decorative;
   kill→build→relaunch is ~15 s with a warm `data/` (5 models, no conversion).
7. Saved-vs-demo shadowing means one name can appear in BOTH menu sections;
   honest but slightly odd. The real design wants either distinct namespaces
   or a "(modified)" marker on a shadowing save.

### W9-C — editor status rendering + wire highlight + badge wart

Shipped (commit 313502b): needs-setup renders neutral, warnings render amber,
setWireHighlight lands the §4.4 reverse link, short wires drop their badge.

- **needs-setup**: footer/chip tone comes from the existing gray STATUS_COLOR
  entry unchanged; the real fix was the footer TEXT switch — it now shows
  status.message ("choose a type") for needs-setup like it always did for
  error, instead of falling through to summary/state. No red anywhere; footer
  text stays textDim.
- **warning**: new pure helpers `footerText`/`footerTone` (exported from
  cards.ts, the badgeText/badgeTone pattern) — "⚠ " prefix on the summary,
  accent tone = cmix(stateColor, amber 255/196/84, 0.65). Errors are immune
  (warning never softens red). Chip mode reuses footerTone so zoomed-out chips
  tint amber too. Full warning text: help expando (geom delegation) + header
  hover tooltip (.pf-tip-warn line).
- **geom.ts delta (pre-authorized narrow)**: HelpLine tone union gained
  "warn"; helpLines() folds `⚠ ${status.warning}` in after detail, before
  error — statuses without warnings byte-identical, proven by the golden
  suite passing with zero churn (no -u anywhere).
- **wire highlight**: `setWireHighlight(keys | null)` + `wireHighlighted(ekey)`
  + `normalizeWireKey` in wires.ts, module-state like onWireHover. Accepts
  CONTRACTS.md keys (`n1.out->n2.in`) AND internal ekeys; unknown keys match
  nothing. Draw: soft halo (width 9, 0.28 alpha) under the casing + core
  brightened (mix 0.45 toward textBright) and +1.4px. Repaint gotcha: the
  runtime sleeps 0.4s after input and a viewport pick never touches the editor
  canvas, so setWireHighlight pokes `window.gratifyResume` — the waker
  attach() installs (same wake() a pointer move calls). Works today because
  the editor is the page's one mounted runtime; if a second canvas runtime
  ever mounts, last-attach wins and this needs a first-class waker seam
  (supervisor: trivial to swap for an `app.wake()` if E ever exposes one).
- **badge wart (T17)**: badges hide when endpoint distance < BADGE_MIN_WIRE_LEN
  = 90px (euclidean, `badgeFits`). 90 ≈ full badge (18 chars ≈ 107px worst
  case, typical ~70) + two socket dots + breathing room; adjacent-card wires
  (~70px) drop it, the demo layouts (300px+) all keep theirs.
- Specs: badges +3 (threshold boundary, fixture pins, adjacent-card case),
  status-render.spec 15, wirehl.spec 9. Editor suite 541 → 571, full run
  780/780 at commit time; tsc clean in-fence (the one repo error mid-wave was
  W9-B's defs-viz WIP, landed fixed before my final run).

### W9-S — supervisor integration (2026-08-09)

Join gates: tsc clean, vitest 780/780 (34→42 files), edgate-smoke 5/5 (2.0s),
intgate-smoke 13/13 (4.6s). Live-verified on the real stack: group-color demo
(group.by "3 groups + unnamed", categorical colorBy, legend swatches incl.
"(none)"), set-algebra demo (1 ∪ 14 → 15, invert 4706 of 4721, count).

- main.ts wiring: groupKeys enum options (Type/Level/channels/params), exportCsv
  Run branch → POST /api/export-csv, chrome getExamples → /api/graphs +
  onExample via /api/graph (static /demo fallback), onSaveGraph via
  window.prompt, viewer.onPick → setWireHighlight (binary search over sorted
  entities; edge keys per CONTRACTS convention), showViewLegend preferred over
  showLegend.
- **Intent-queue replay finally bit a gate** (wave-1 finding #5 made concrete):
  a fresh session replaying since=0 applied a stale `connect a1→n2.in` from an
  earlier smoke run, which REPLACED carbon-walls' load→byType wire via the
  one-wire-per-input rule — 4 intgate checks red, cause invisible until the
  queue was dumped. Fixed in main.ts: boot fast-forwards `since` to the queue's
  `now` and only applies intents issued after session start. The real design's
  session model (host-api spec) supersedes this; the lesson stands — replay +
  reconnect-replaces = silent graph corruption.
- intgate-smoke loadDemo now polls for the menu item: the Examples menu rebuilds
  async from /api/graphs on open, so a one-shot click finds nothing (§4.4 rule:
  wait on what the new state uniquely produces — including menu items).
- Track B's subtract-does-not-merge reading (b = removal mask, a's channels pass)
  is CONFIRMED as intended — matches the kinds.ts contract text.
- Screenshot note: hidden Browser-pane canvases still yield no pixels
  (toDataURL "data:," on webgl, blank 2D) — the gate's readPixels checks remain
  the only pixel truth, exactly as wave-1 concluded.

## Wave 10 — grid-click highlight, embedded colorBy ramp, column tools, six demos (2026-08-09)

Four fenced tracks + supervisor join. Gates: tsc clean, vitest 870/870,
edgate-smoke 5/5, intgate-smoke 13/13.

Shipped: mini-grid rows are clickable (and hover-tinted) — a click highlights that
row's element(s) in 3D via the same rowClicker path as the data-grid pane;
viz.colorBy carries its OWN ramp/auto/min/max + gradient/range-slider body (the
Colormap node is now only for sharing one ramp across views — its wired input
overrides, body dims with a "wired ▸ colormap" tag); new kinds table.columns
(keep/drop projection), table.stats (per-numeric-column summary), check.rule
(row assertion → violations out, PASS/FAIL verdict + warning); six new demos
built against live data (door-egress-check found 4 real sub-3ft doors in Snowdon;
wall-roles-overlap proved every load-bearing duplex wall is exterior; plus
wall-stats-explore, simple-color 4-node coloring, level-takeoff-export,
hvac-composition categorical).

Findings worth keeping:
- **Drag-delegation beats drag-eating for large claim zones** (Track A): the
  grid-click gesture wraps nodeMoveGesture (stores its begin state, routes
  move/during/up/view once slop is exceeded) so card-drag from a 400px body stays
  byte-identical — splice/multi-select/one-undo intact. Bless this pattern for any
  future click-on-big-surface gesture.
- **wiredInputsOf is param-only** (Track B): a wired PORT (colorBy's colormap) is
  invisible to the layout seam; detection must come from edges (cmapWired prop fed
  by makeView). Real design: one effectiveInfo/wiredPorts seam.
- **Optional inputs are a registry set, not a PortSpec flag** (Track C):
  isOptionalInput + a 2-line evaluate.ts seam; kinds.ts says "OPTIONAL" only in
  prose. Real design: `optional?: boolean` on PortSpec. demos.test's structural
  wired-check needed the same seam (join fix).
- **Mock-compatibility rule hardened** (Track D): column-explicit table nodes
  must sit downstream of fromScene/CSV, never raw table.sql (canned mock SQL).
  The compute.expr param('X')-to-channel trick is the mock-safe param filter.
- Snowdon dimensional params are imperial FEET; integer-feet rule thresholds are
  BOS-int-truncation-proof (floor(x)>=k ⟺ x>=k) — anything else needs the SQL path.
- check.rule keeps ALL input columns, so attach.column joins violations straight
  back onto the scene for offender-coloring with zero new vocabulary.

## Wave 11 — checklist widget, bounding boxes, level explode (2026-08-09)

Four fenced tracks + join. Gates: tsc clean, vitest 943/943, edgate-smoke 5/5,
intgate-smoke 13/13.

Shipped: select.checklist (live check-boxes on the node — every Type/Level actually
present, with counts; inverted `excluded` storage so new upstream values default
ticked; toggle = one undo each; drag-delegation keeps card moves intact);
viz.boxes (view→view AABB massing — synthetic unit-cube instances off cached
geometry.boundingBox, CHEAPER than mesh views at 2.4ms, picking survives via a
representative InstanceIndex); viz.explode (per-level vertical offsets, hide list
defaults to the verified-real ceiling category IFCCOVERING; ViewValue.offsets are
MODEL-SPACE Z-UP — the library flips the group afterward); demos checklist-live
(rac_basic — its human Revit type names make the best checkbox labels),
massing-boxes (eye-flip mesh-vs-box compare), explode-levels (snowdon, spacing 20
≈ 2× the real 10.75–13.3ft storey deltas). Link-drag from a view wire now offers
boxes/explode where view wires used to dead-end (spec flipped).

Findings worth keeping:
- **NodeOut→NodeStatus transport gap, third occurrence** (chart, detail, now
  checklist): every new status field needs a hand-added copy line in evaluate.ts.
  Real design: generic passthrough.
- **Instance.isIdentity gotcha** (viewer): any retransformed instance must set it
  false or the merge path silently drops the transform. Offsets/boxes ride the
  existing rebuild (~25ms ceiling); highlight is now view-aware (exploded/box
  positions — previously would have drawn at the un-offset location).
- **Render-tree sibling trap** (editor): a decorative part mounted ON TOP of an
  interactive sibling eats presses in the main tree; the ADORN layer fails over
  to the root tree instead — which is why decorative bodies pass clicks through.
  Spec rigs must order gesture hosts above decorative bodies.
- Checklist matching is EXACT against model.types (verified identical to host
  EntityText.Category); comma-in-value can't round-trip the excluded param
  (accepted, documented). Raw duplex/snowdon checklists drown in STEP-geometry
  cruft; rac_basic collapses cruft into "(none)".
- Unnamed sentinel "(none)" shared between checklist and categorical legend.

## W13 UX wave (2026-08-20) — four parallel tracks + supervisor

Driven by user feedback: overlapping nodes, graphs loading under the left panel,
"never understood the eye", overwhelming UI. All four tracks ran concurrently on
this tree under the CONTRACTS.md W13 fence table; gate after integration:
tsc clean, 997/997 specs (50 files), live-verified against the running host.

### Track A — layout (layout.ts)
- Layered longest-path layout: iterative DFS classifies edges (back edge on stack =
  cycle break), depth relaxed in topo order; all iteration follows graph order →
  deterministic, no recursion. Column membership is a pure function of depth, so
  widths/x/midline compute in one pass — no fixpoint.
- Row order key: avg y-centre of placed upstream nodes (sources fall back to current
  y) — keeps wires roughly horizontal without a crossing-minimization pass.
- placeFree: 12-step down-right cascade first (repeated drops read as a stack), then
  widening square rings; termination proved by ring cap past the right-most box.
- Stale edges (endpoint not in nodes) must be filtered before the depth walk.
- Real-design upgrade: slot-aware row ordering (align actual socket rows) — SlotRef
  already carries what's needed.

### Track B — calm pass + eye removal (cards/wires/surface)
- EyeChip deleted (part, press, glow channel, glow ring). `display` prop dead in the
  view; selection drives the 3D view (supervisor wiring in main.ts).
- THE gratify finding: `ch.hover` is strict-instance — interactive adorns capture
  hover from their host card, so gating chip EMISSION on card hover makes a chip
  evict itself when the pointer reaches it (emission/hover flicker loop). Fix: a
  card-level `over` channel with geometric target (`n.pointer && n.rect.contains`).
  Real design wants hover-chain / host-hover propagation for adornments.
- Calm values: grid dots 0.2; wire core 1.8px, casing 0.05@4px, halo 0.22@8px; chip
  fade floor 0.02, press floor 0.5 (a fading chip must not eat clicks — guard in the
  chip's own press); footer ok-line 0.32, ok-text 0.65 at rest; error/warn always full.
- Wire badges: hidden at rest, follow wire hover; error badges always show.
- badges.spec.ts needed no edits (pins pure helpers only) — layering specs by purity
  paid off.

### Track C — chrome search + View items + toasts
- Panel search matches label + FULL description (keyword in sentence two still hits);
  Enter adds top visible match; hint hides while filtering.
- F/T keys guard !ctrl && !meta && !alt so browser Ctrl+F/T survive; Enter/Escape in
  the search input stopPropagation (picker pattern) to avoid double-firing chrome's
  own Esc.
- Toast stack: bottom-left, max 3, evicted toasts must cancel their timers or they
  resurrect under fake timers. CSS vars carry hex fallbacks so a toast can fire
  before the theme block exists.

### Track D — context menu organ (contextmenu.ts)
- Clamp = shift (min against viewport), measured AFTER display:block (hidden rect is
  0×0). Menus taller than viewport pin, not flip — fine at PoC sizes.
- Opening-gesture race: click-away listens on window pointerdown; contextmenu fires
  after pointerdown completes, so opening from a right-click handler never
  self-closes — but opening from a pointerdown-phase handler needs stopPropagation.
- Disabled items get NO listener at all (jsdom dispatchEvent would bypass `disabled`).
- jsdom: no PointerEvent constructor (dispatch MouseEvent typed "pointerdown"), no
  layout (stub getBoundingClientRect for clamp specs).

### Supervisor — selection-as-display + integration
- shownId = selected > pinned doc.display > LAST drawable node in doc order. The
  last-drawable fallback replaces the old "wire a ColorBy, see nothing" trap AND
  made auto-display-on-load free (no dispatch, no doc mutation). doc.display stays
  in the schema for MCP; no UI produces setDisplay anymore.
- Tidy = ONE batch of move intents (one Ctrl-Z), then the fit tween; Add Node drops
  through placeFree over live node boxes; node right-click = context menu (Show in
  3D / Run [effect==="write" only] / Toggle help / Duplicate / Delete), empty canvas
  keeps the add palette.
- Wave mechanics: zero fence violations, zero merge conflicts, one baseline red
  caught pre-spawn (ChromeHooks growth broke spec mocks — landing contracts first
  surfaced it immediately). Two tracks independently hit "untracked files need
  explicit git add before a pathspec commit".

### W13 follow-up — multi-model viewer (disciplines was inert)
User: "in disciplines, clicking different things changes nothing." Two causes:
(1) the demo is table-shaped — three IDENTICAL SQL queries over three models, so
selection changed only near-identical grids; (2) the viewer was SINGLE-model —
`bim = data` on every load, applyView ignored `view.model`, so all three Load
Model scenes rendered against whichever BOS parsed last. Fix: every parsed model
is cached (bim/ModelData/indices/baseGroup per id, loaderGeometries + baseGroups
cumulative so dispose never eats a cached model), `activate(id)` swaps the active
one, and applyView switches to `view.model.id` first (warn + keep current when
unloaded). `viewer.load` of a cached id re-activates without re-parse. Verified:
n1 → architecture, n2 → duct network, live. Remaining wart: grid row-click
highlight still resolves entity indexes via `firstModel` — wrong model when a
table came from another discipline (real-design: values should carry their model
to the highlight path too).
