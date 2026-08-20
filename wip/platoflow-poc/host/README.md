# platoflow-poc host

One C# console app that is the whole back end of the PoC: it prepares the demo data, serves the
models and files the browser needs, answers SQL over BIM data, writes property sets back into a
real IFC file, and exposes the graph editor to an LLM agent over MCP — all on
`http://127.0.0.1:5214`.

It is a throwaway. It keeps everything in memory, trusts its only caller, and answers errors with
HTTP 200 because that makes the browser side smaller.

## Running it

```powershell
dotnet run --project ara3d-sdk/wip/platoflow-poc/host/PlatoFlowHost.csproj
# or a different port
dotnet run --project ara3d-sdk/wip/platoflow-poc/host/PlatoFlowHost.csproj -- 5300
```

The first run converts `duplex.ifc` to BOS and builds two DuckDB databases; expect roughly a
minute. Later runs reuse both and start in a second or two. Everything derived lands in
`ara3d-sdk/wip/platoflow-poc/data/`, which is gitignored apart from `models.json`.

To rebuild the demo data from scratch, delete `data/` (keep nothing) and run again.

The app finds its own tree by walking up from the exe looking for `CONTRACTS.md`. Set
`PLATOFLOW_POC_ROOT` to override.

## What startup produces

| File in `data/` | Where it comes from |
|---|---|
| `duplex.ifc` | copy of `nrc-ifc-llm/IFC-Test-Kit/duplex.ifc` — pset writes patch this copy, never the source repo |
| `duplex.bos` | `IfcToBosConverter` over `duplex.ifc` (loads geometry via the native web-ifc DLL), then repacked by `LegacyBosTables` so the browser can read it — see below |
| `rac_basic.bos` | copy of `ara3d-webgl/examples/public/rac_basic_sample_project-2025.bos` |
| `carbon.csv` | copy of `IFC-Test-Kit/analytics_dataset_with_levels.csv`, keyed by duplex GlobalIds |
| `*.duckdb` | `BosToDuckDB` + `IfcDuck.CreateViews` per model |
| `models.json` | the model list `GET /api/models` serves (the one committed file) |
| `kinds.json` | seeded from `host/kinds.default.json`; the web app may `POST /api/kinds` its own dump over it |
| `out/<model>-enriched.ifc` | written by `POST /api/append-psets` |
| `rac-carbon.csv` | **fallback only** — synthetic carbon keyed by rac_basic GlobalIds, written when duplex is unavailable |

## Endpoints

All responses are JSON with `Access-Control-Allow-Origin: *`; `OPTIONS` answers 204.

| Endpoint | Body / query | Answer |
|---|---|---|
| `GET /api/health` | — | `{ok, service, root, models}` |
| `GET /api/models` | — | `[{id, name, bosUrl, ifcPath}]` |
| `GET /api/file` | `?path=<name>` | the file, streamed with a correct `Content-Length`. Only files inside `data/`; anything resolving outside answers 403 |
| `GET /api/kinds` | — | the node vocabulary array |
| `POST /api/kinds` | a JSON array | replaces `data/kinds.json` |
| `POST /api/sql` | `{model, sql}` | `{columns, rows}` or `{error}`. One `SELECT`/`WITH` statement, capped at 20 000 rows |
| `POST /api/append-psets` | `{model, psetName, rows:[{globalId, props}]}` | `{outPath, entitiesAdded, diffSummary, diff, elementsWritten, skipped}` |
| `GET /api/intents` | `?since=N` | `{intents:[{seq, intent}], now}` |
| `POST /api/state` | `{doc, results}` | caches the browser's latest graph + eval results |
| `POST /mcp` | JSON-RPC 2.0 | `initialize`, `tools/list`, `tools/call`, `ping` |

### SQL

Queries run against the model's DuckDB database built from its BOS conversion. The useful views —
`EntityText`, `ParameterText`, `RelationText` — are the ones from `IfcDuck`, which resolve BIM Open
Schema's interned integer indexes into names. The raw tables (`Entities`, `Strings`, `Parameters`,
…) are almost entirely integers and rarely what you want.

```powershell
curl.exe -s -X POST http://127.0.0.1:5214/api/sql -H "Content-Type: application/json" `
  -d '{\"model\":\"duplex\",\"sql\":\"SELECT Category, count(*) n FROM EntityText GROUP BY 1 ORDER BY n DESC LIMIT 5\"}'
```

Anything that is not a single `SELECT`/`WITH` is rejected with `{error}` — the check is
`IfcDuck.ReadOnlyQuery`, source-linked rather than reimplemented.

### Two BOS layouts, and a type-code shift

The two demo models are written by different converters and disagree about how parameters
are stored, in two ways that the host has to bridge in opposite directions.

`rac_basic.bos` uses the older split layout (`IntegerParameters`, `SingleParameters`,
`StringParameters`, `EntityParameters`, `PointParameters`) which `IfcDuck.CreateViews`
cannot read — `BosCompat` rebuilds the three text views over whatever tables are present.
`duplex.bos` uses the current layout (`Parameters` + `Numbers`) which the *browser* loader
cannot read — `LegacyBosTables` derives the five legacy tables from it after conversion and
repacks the zip, keeping the new tables for the host's own SQL.

While doing that it also corrects a silent off-by-one: duplex's `Descriptors.Type` is
uniformly one higher than the `ParameterType` enum both readers assume, which made every
parameter resolve against the wrong lookup table without raising anything. The shift is
inferred (not hard-coded) by checking each type code's values against the row count of the
table it would have to index, then folded out of `Descriptors.parquet`.

Both steps are idempotent and run only when needed. Full account in `../NOTES.md`.

### Pset writes

`POST /api/append-psets` appends `IfcPropertySingleValue` / `IfcPropertySet` /
`IfcRelDefinesByProperties` lines to the model's source IFC using the byte-exact patcher from
`tests/Ara3D.Ifc.Tests`. Numbers become `IFCREAL`, booleans `IFCBOOLEAN`, strings `IFCLABEL` (or
`IFCTEXT` past 255 characters). Untouched bytes stay untouched, so `diffSummary` — produced by
`IfcDiff.Compare` — is the complete account of what changed. Rows whose `globalId` matches no
element come back in `skipped` instead of failing the call.

### MCP

Nine tools: `list_node_kinds`, `get_graph`, `add_node`, `connect`, `set_param`, `set_display`,
`load_graph`, `read_node`, `sql`. The mutating ones enqueue an intent and return immediately; the
browser polls `/api/intents` about four times a second and applies each one through its reducer, so
an agent edit and a mouse edit take exactly the same path. `add_node` allocates the id (`a1`, `a2`,
…) host-side and returns it, so an agent can wire up a node it has just created without waiting for
a round trip through the browser.

`get_graph` and `read_node` answer from whatever the browser last pushed to `/api/state`, so they
are only as fresh as the last evaluation.

A request with no `Origin` header is allowed (CLI clients); a browser-supplied `Origin` must be
loopback.

## Smoke test

With the server running:

```powershell
powershell -ExecutionPolicy Bypass -File ara3d-sdk/wip/platoflow-poc/host/smoke.ps1
# non-default port
powershell -ExecutionPolicy Bypass -File ara3d-sdk/wip/platoflow-poc/host/smoke.ps1 -BaseUrl http://127.0.0.1:5300
```

It exercises every endpoint end to end — model list, a multi-MB `.bos` download with a matching
`Content-Length`, path-traversal rejection, a SQL count, a rejected `DROP`, the MCP handshake and
tool list, `add_node` followed by the intent actually appearing in the queue, a `/api/state` round
trip read back through `get_graph`/`read_node`, and a two-row pset write verified against the file
on disk. It prints `PASS`/`FAIL` per step and exits with the failure count.

## Layout

| File | What it does |
|---|---|
| `Program.cs` | startup, port, shutdown |
| `PocPaths.cs` | finds the PoC root; the three external source-data paths |
| `DataSetup.cs` | conversion, copying, `models.json`, `kinds.json`, the duplex fallback |
| `LegacyBosTables.cs` | rewrites a converted `.bos` into something the browser loader accepts |
| `BosCompat.cs` | SQL views for a `.bos` whose layout `IfcDuck` cannot read |
| `ModelCatalog.cs` | the model list, cached DuckDB connections, query → `{columns, rows}` |
| `HostApi.cs` | routing, CORS, the file sandbox |
| `PsetWriter.cs` | `/api/append-psets` |
| `AgentBridge.cs` | intent queue, cached graph + results, agent node ids |
| `McpEndpoint.cs` | JSON-RPC 2.0 and the nine tools |
| `kinds.default.json` | transcription of `web/src/kinds.ts` |

Nothing under `ara3d-sdk/` is copied or edited: `IfcDuck.cs` and the eight IFC patch/diff files are
`<Compile Include>`-linked straight out of the SDK tree, the same way
`tests/Ara3D.DoorClearance.Tests` does it.
