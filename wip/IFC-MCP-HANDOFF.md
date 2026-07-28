# IFC MCP server — handoff

Status as of 2026-07-27. Steps 1–4 of "Pick up here" are **done and pushed**: stdio transport
(`6f7dc35`) and `wip/Ara3D.Ifc.Mcp` with the whole data tool surface (`fc635ca`). Analytics,
geometry, and write tools are still unbuilt. Two more upstream defects were found while building
the data tools — see "Defects found building the data tools" below.

## Goal

Two goals, in priority order (the user's framing):

1. **Primary — prove `dotnet-greenhouse` works on a real project and actually reduces token
   use.** When greenhouse cannot answer something, stop the feature work, fix greenhouse in
   `C:\Users\cdigg\git\dotnet-greenhouse`, repack, resume. This has already happened twice.
2. **Secondary — an MCP server that does honestly useful things with IFC files.** Be generous
   with the tool surface early; trimming and merging comes later.

## Tool discipline (non-negotiable)

Use greenhouse for **all** code questions — `mcp__greenhouse__greenhouse_*` when the MCP
server is loaded, otherwise `dotnet tool run greenhouse -- <verb>` from the repo root. Do not
open a discovery session with grep/read sweeps. `explore` (declaration + source + callers +
callees in one call) is the highest-value verb; `project` answers .csproj questions.

When delegating to subagents, say this explicitly and require a **Fallbacks list**: every
greenhouse command that failed, with its error. Repeated fallbacks are a greenhouse feature
gap to fix, not an inconvenience to work around.

## Setup already done (committed, both repos pushed)

- Greenhouse packed and installed as repo-local dotnet tools; manifest is `dotnet-tools.json`
  at the repo root (SDK 10 puts it there, not under `.config/`). Current pin: `0.1.1785195532`.
- `.mcp.json` at the repo root registers `dotnet tool run greenhouse-mcp`.
- `.greenhouse/` cache excluded via `../.git/modules/ara3d-sdk/info/exclude` — **not**
  `.gitignore`, which is on the no-touch list. Note `.git` here is a gitfile, not a directory.

**Repacking gotcha:** `dotnet tool` caches by version, so reusing a version silently serves a
stale build. `scripts/pack-tools.sh` stamps a unique version; after packing, run
`dotnet tool update --local Greenhouse.Cli|Greenhouse.Mcp --add-source <nupkg dir>` here.

**A running MCP server is pinned to the build it launched with.** After repacking greenhouse,
the session must be restarted before the new server code is in effect.

## Greenhouse defects found and fixed

1. **`greenhouse_project` added** (greenhouse `3a52836`) — greenhouse could not answer any
   .csproj question (target frameworks, package versions, Windows-only). Two independent
   mapping agents both fell back to grep for exactly this. Reads project XML directly; no
   MSBuild evaluation, so inherited `Directory.Build.props` values are deliberately not
   resolved and every response says so.
2. **stdio hang fixed** (greenhouse `5a40c81`) — every MCP tool call hung forever. None of the
   five `ProcessStartInfo` sites redirected stdin, so each spawned `git` inherited the server's
   stdin, which under the stdio transport is the JSON-RPC protocol stream; `ResponseGovernance`
   runs git on every response. Measured: `greenhouse_project` 24,566ms → 128ms,
   `greenhouse_outline` 44,585ms → 725ms, replies previously never delivered at all. Fixed via
   `Greenhouse.Core.ChildProcess`. Full fast tier green at 546 tests.

Two other reports turned out **not** to be bugs, do not re-chase them: `outline` "dropping" a
class was truncation that greenhouse *did* disclose in a `... N more types (raise top).` footer
an agent's grep skipped; and the CLI docs are accurate (the kit table lists MCP tool names, not
CLI verbs).

**Known greenhouse gaps, not yet fixed:**

1. **No live-subprocess MCP test**, which is why a total stdio outage shipped. Now solved *here*
   first — `tests/Ara3D.Ifc.Mcp.Tests/StdioEndToEndTests.cs` + `StdioServerProcess.cs`, with the
   port note at `wip/stdio-e2e-test-pattern.md`. Verified against an injected defect. Port it.
2. ~~**`greenhouse_diagnostics` directory mode is unusable.**~~ **FIXED 2026-07-28**, greenhouse
   `9dde612`. Sighted by all three Wave 1 agents: directory targets compiled without NuGet,
   framework, or ProjectReference resolution — 502 phantom errors on `tests/Ara3D.MCP.Tests`,
   330 on `tests/Ara3D.Ifc.Mcp.Tests`, 87 on `wip/Ara3D.Ifc.Mcp`. Directory targets now resolve
   to the owning project(s) in `WorkspaceCache` (so **every** verb benefits, not just
   `diagnostics`); the staleness stamp names which case answered — `dir: compiled via X.csproj`,
   `dir: compiled via enclosing X.csproj (scope widened...)`, `dir: aggregated N projects`, or
   `dir: no .csproj found` for genuine loose files. Verified here: `wip/Ara3D.Ifc.Mcp` now
   reports `dir: compiled via Ara3D.Ifc.Mcp.csproj`. Greenhouse suite 582/582.
   **Cost:** directory targets now pay an MSBuild load — greenhouse's own MCP test project went
   24s → ~3m. Correctness over speed; the cache pays it once per session.
   **Local pin bumped to `0.1.1785214825`** in `dotnet-tools.json`. A running MCP server stays on
   the build it launched with, so **restart the session** before the fix reaches the MCP tools.
3. **`greenhouse_explore` truncates source at a fixed cap** (`... 139 more lines.`) with no
   parameter to raise it — `top` only affects callers/callees. Forces a raw `Read` on big files.
4. **`greenhouse_validate_changes` fails open on unmapped dirty files** — the three pre-existing
   modified files (`FlowObject.cs` et al.) defeat impact selection, so its test section skips to
   ALL. Honest and disclosed, not a defect, but it means the verb adds nothing while the tree is
   dirty.

## SDK map (verified, file:line)

**Use `ext/Ara3D.IfcLoader` as the entity layer.** The user has confirmed referencing it is
fine; it remains on the never-edit, never-stage list.

| Capability | API | Location |
|---|---|---|
| Open a file | `IfcFile.Load(path, includeGeometry, logger)` | `ext/Ara3D.IfcLoader/IfcFile.cs:71` |
| All entities | `IfcEntityResolver.GetEntities()` | `ext/Ara3D.IfcLoader/IfcEntityResolver.cs:29` |
| Entity by id | `IfcEntityResolver.GetEntity(int)` | `IfcEntityResolver.cs:23` |
| Type name/code | `IfcEntity.GetEntityName/GetEntityCode` | `ext/Ara3D.IfcLoader/IfcEntity.cs` |
| Attributes | `IfcEntity.GetString/GetNumber/GetId/GetIdList` | `ext/Ara3D.IfcLoader/IfcEntity.cs` |
| Property sets | `IfcPropData.GetProperties(entityId)` | `ext/Ara3D.IfcLoader/IfcPropData.cs:42` |
| Relations | `new IfcRelations(file)` | `ext/Ara3D.IfcLoader/IfcRelations.cs:28` |
| Containment | `IfcRelations.ParseContainedIn` | `IfcRelations.cs:80` |
| Spatial aggregation | `IfcRelations.ParseDecomposition` | `IfcRelations.cs:90` |
| Whole-model analytics | `IfcToBosConverter.Convert(in, out)` | `src/Ara3D.BimOpenSchema.IO/IfcToBosConverter.cs:107` |
| SQL over a model | `DuckUtils.BosToDuckDB` | `src/Ara3D.BimOpenSchema.IO/DuckUtils.cs:11` |
| IFC → meshes | `Approach1Mesher.Build(IfcFile|FilePath)` | `wip/Ara3D.Ifc.Mesher/Approach1/Approach1Mesher.cs:15,30` |
| Mesh container | `Model3D` | `src/Ara3D.Models/Model3D.cs:11` |
| Export GLB | `GltfWriter.WriteGlb(IModel3D, FilePath)` | `src/Ara3D.IO.GltfExporter/GltfWriter.cs:60` |

`src/Ara3D.IO.StepParser` is raw tokenizing — too low-level to build tools on; it is what
`IfcFile` wraps. A `.bos` file is a zip of Brotli-compressed Parquet tables.

**Gotchas that will bite:**
- `IfcFile` and `StepDocument` are `IDisposable` and hold raw pointers into a pinned buffer.
  Every `IfcEntity`/`StepToken` is valid only while the document is alive.
- Conversion is whole-file, in memory. `IfcEntityResolver` allocates an entity for every
  definition (there is an explicit `// TODO: this should be filtered` at `IfcEntityResolver.cs:14`).
- ~~`IfcToBosConverter` filters out spatial containers.~~ **WRONG — corrected 2026-07-28.** It does
  not. `HiddenIfcNames` (`IfcToBosConverter.cs:359`) is only a geometry-instance visibility flag;
  site/building/storey are all present in BOS. Pinned by `Bos_KeepsSpatialContainers`.
  Relation direction is still child → parent.
- `IfcToBosConverter`'s constructor hardcodes `includeGeometry: true`, so the **analytics tools do
  carry the native DLL dependency** — the "all data tools use `includeGeometry: false`" rule below
  does not extend to them.
- **Upstream leak, reported not fixed:** `IfcToBosConverter.Convert` never disposes its `IfcFile`,
  leaking a pinned buffer and a native web-ifc model per call. `wip/Ara3D.Ifc.Mcp` sidesteps it by
  calling the constructor + `SaveToBos` and disposing itself. `ext/` stays no-touch.
- BOS is **fully interned**: `Entities.Name` indexes `Strings`, `Category` indexes `Entities`,
  `Parameters.Value` is a tagged index. Raw SQL sees zero text — use the `EntityText` /
  `ParameterText` / `RelationText` views (`wip/Ara3D.Ifc.Mcp/IfcDuck.cs`).
- `IfcFile.GetSitePlacement()` is a stub returning `(0,0,0)` (`IfcFile.cs:51`) — `OriginOffset`
  is always zero; do not use it for georeferencing.
- Geometry needs the native `web-ifc-library.dll` (`ext/Ara3D.IfcLoader/../../vendor/`), forcing
  Windows/x64. `includeGeometry: false` avoids it entirely, and all data tools should.
- `IfcRelationMapping.ToBos` throws on any unmapped kind (`IfcRelationMapping.cs:21`).
- `tests/Ara3D.Ifc.Tests` needs a test kit via `TestData.RequireTestKit()` (Duplex IFC data).

**Windows-pinned projects** (`greenhouse project src --windows-only`): `Ara3D.BimOpenSchema.IO`,
`Ara3D.SDK`, `Ara3D.SDK.IO`. Also `ext/Ara3D.IfcLoader` and `wip/Ara3D.Ifc.Mesher`. Everything
else in the data path is plain net8.0.

## The MCP host

`wip/Ara3D.MCP` is a working MCP server, net8.0, clean, with tests in `tests/Ara3D.MCP.Tests`
(`McpServerTests.HttpPost_ToolsListAndCall_RoundTrip` is the end-to-end example).

- Register tools fluently: `new McpServer(...).Tool(name, description, schema, handler).Start()`.
  Overloads at `wip/Ara3D.MCP/McpServer.cs:34,43,46,49`. Handlers return a `string`.
- Schemas: `McpSchema.Object().String(name, desc, required).Build()` — **no arrays, enums,
  nested objects, or defaults** (`McpSchema.cs:6,15,33`). Extend it if a tool needs them.
- Transport is HTTP-only, but coupling is thin: `McpHttpListener` calls exactly one method,
  and `McpServer.HandlePost(string)` (`McpServer.cs:62`) is already transport-free.

**Protocol gaps** in `McpJsonRpcHandler`: only `initialize`, `tools/list`, `tools/call`, `ping`;
the hardcoded protocol version is never negotiated; no `notifications/initialized` handling; no
`tools/list_changed`; parse errors return HTTP 400 rather than JSON-RPC `-32700`; `tools/call`
blocks the listener thread via `.GetAwaiter().GetResult()`.

## Defects found building the data tools — both FIXED upstream (user authorized the edits)

1. **`IfcPropData.ParseElementQuantity` read the wrong attributes**
   (`ext/Ara3D.IfcLoader/IfcPropData.cs:125`) — name from attribute 0, members from attribute 3,
   but `IFCELEMENTQUANTITY` is `(GlobalId, OwnerHistory, Name, Description, MethodOfMeasurement,
   Quantities)`. Every quantity set in every model read back named after its GlobalId GUID and
   empty: 64 missing quantities on one FZK-Haus wall. Fixed: name at 2, members at 5.
2. **A typed property value is unreachable through the attribute list.** For
   `IFCPROPERTYSINGLEVALUE('ConstructionMode',$,IFCLABEL('Massivhaus'),$)` attribute 2 is the bare
   token `IFCLABEL`; `StepTokenExtensions.AsList` steps over the `('Massivhaus')` payload to keep
   arity right. That skip is load-bearing — changing `AsList` would shift attribute indices for
   every positional consumer (`IfcRelations`, `IfcPropData`) — so the fix is
   `ext/Ara3D.IfcLoader/IfcPropValueExtensions.cs` (`GetMeasureType` / `GetValueText`), which
   unwraps the payload from the token stream. The wip workarounds (`IfcPropertySets`,
   `IfcPropertyText`) are deleted.

Note `ext/Ara3D.IfcLoader` stays on the never-edit list by default; these two edits were
explicitly authorized by the user on 2026-07-27. Do not treat that as a standing permission.

Greenhouse answered every code question in this round; no fallbacks to grep for code structure.
The one thing it could not settle was IFC *attribute order* inside a specific model, which is a
data question, not a code question — reading the raw `.ifc` line was correct there.

## Done

1. **stdio transport in `wip/Ara3D.MCP`** (`6f7dc35`). `McpStdioTransport` pumps line-delimited
   JSON through the existing `McpServer.HandlePost`; `McpJsonRpcHandler` untouched. Transport is
   chosen at construction (`McpTransport.Http|Stdio`); `Url` is null under stdio;
   `WaitForShutdown()` blocks a console host until its client closes stdin. `StartStdio(reader,
   writer)` makes the pump testable without console handles. 4 tests; suite 24/24.
2. **`wip/Ara3D.Ifc.Mcp`** (`fc635ca`), net8.0-windows — forced by `Ara3D.IfcLoader`, not by
   choice. 16 tools: `ifc_open`, `ifc_close`, `ifc_models`, `ifc_header`, `ifc_type_counts`,
   `ifc_search`, `ifc_entity`, `ifc_entities_of_type`, `ifc_attributes`, `ifc_properties`,
   `ifc_quantities`, `ifc_property_sets`, `ifc_relations`, `ifc_spatial_tree`,
   `ifc_spatial_contents`, `ifc_element_containment`. `IfcSessionCache` keeps 3 models open (LRU);
   relation and property indexes are lazy per session. Every list tool pages and reports the
   unpaged total. `tests/Ara3D.Ifc.Mcp.Tests` 17/17 against the checked-in FZK-Haus sample.
   Neither project is in `Ara3D.SDK.sln`.

Run it: `dotnet run --project wip/Ara3D.Ifc.Mcp` (stdio; `--http [port]` to listen instead).

## Wave 1 — done 2026-07-28 (`f25cf1a`, `393a9a2`, `91f12d6`)

Three parallel agents, disjoint file scopes, no conflicts. Suites verified by the main thread,
not just self-reported: `tests/Ara3D.MCP.Tests` **49/49** (was 24), `tests/Ara3D.Ifc.Mcp.Tests`
**30/30** (was 17).

1. **MCP host hardened.** `McpSchema` gained enums, arrays, nested objects, defaults — old fluent
   calls compile unchanged. `McpJsonRpcHandler` is async end-to-end (`tools/call` no longer blocks
   the listener), handles `notifications/initialized`, negotiates the protocol version against
   `2025-06-18 / 2025-03-26 / 2024-11-05`, and returns `-32700` bodies instead of empty HTTP 400.
   **Behavior change to know:** unparseable stdio input now emits a `-32700` line where it
   previously wrote nothing; `Stdio_UnparseableAndBlankLines_AreSkipped` was renamed accordingly.
2. **Analytics group.** `ifc_to_bos`, `ifc_table`, `ifc_sql`, `ifc_sql_export`. See the corrected
   gotchas above — the two facts this doc previously got wrong were both in this area.
3. **Live stdio E2E test.** Launches the built `ara3d-ifc-mcp.exe` directly rather than
   `dotnet run --project`: a build inside the timed window is indistinguishable from a hung
   server, and did in fact hang >10 min under concurrent MSBuild contention.

**Operational note:** a live `ara3d-ifc-mcp.exe` locks the app's `bin` and hard-fails builds with
`MSB3027`. Kill stray servers before running the suites; CI running tests beside a live server
hits the same.

## Pick up here

1. **Geometry** (`ifc_mesh`, `ifc_bounds`, `ifc_volume`, `ifc_export_glb`,
   `ifc_meshing_diagnostics`), **then write** (`ifc_append_pset`, `ifc_remove_patch`, `ifc_diff`).
   Geometry needs sessions opened with `includeGeometry: true` — decide whether that is a second
   cache or a flag on `IfcSession`, given the 3-model LRU.
2. **Now that the schema builder has enums**, retrofit the tools that take unvalidatable strings —
   `ifc_relations`' `kind` is the known one.
3. Greenhouse `diagnostics` directory mode is **fixed** (gap 2). Remaining known gap worth doing:
   `explore`'s fixed source-truncation cap with no way to raise it (gap 3).

The write path currently exists **only in tests** — `IfcPatcher.Append/Remove` splice lines
before `ENDSEC` and have a proven byte-identical round trip
(`tests/Ara3D.Ifc.Tests/AnalyticsPropertyTests.cs:31,37,50`). Promoting it into a library is a
deliberate step, not a copy-paste.

## Repo rules that apply

- Never touch or stage: `Ara3D.SDK.sln`, `ext/Ara3D.IfcLoader`, `wip/`'s pre-existing projects,
  `tests/Ara3D.IfcMeshingComparison`, `.gitignore`. Referencing `ext/Ara3D.IfcLoader` is
  explicitly allowed; editing it is not.
- New test/tool projects are not added to the `.sln`.
- Follow the `csharp-style` skill for handwritten C#.
- Commit after each verified milestone and push; commit to the current branch, never branch.
