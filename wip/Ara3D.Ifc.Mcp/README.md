# Ara3D.Ifc.Mcp

An MCP server that answers questions about IFC files. Built on `wip/Ara3D.MCP` for the protocol
and `ext/Ara3D.IfcLoader` for the entity layer.

The data tools never load geometry (`includeGeometry: false`). The analytics tools do, but not by
choice: `IfcToBosConverter` hardcodes it, so `ifc_to_bos` and anything built on it needs the native
`web-ifc-library.dll`. The project targets `net8.0-windows` because `Ara3D.IfcLoader` does.

## Running

```bash
dotnet run --project wip/Ara3D.Ifc.Mcp
```

Stdio is the default, which is how MCP clients launch a server. `--http [port]` listens on
`http://127.0.0.1:8766/mcp` instead, which is easier to poke at by hand. Under stdio, stdout is
the protocol stream and all diagnostics go to stderr.

Client config:

```json
{
  "mcpServers": {
    "ara3d-ifc": {
      "command": "dotnet",
      "args": ["run", "--project", "wip/Ara3D.Ifc.Mcp"]
    }
  }
}
```

## Tools

| Tool | Answers |
|---|---|
| `ifc_open` | Schema and entity count. Optional — any tool opens the file it is given. |
| `ifc_close` / `ifc_models` | Free a model; list what is held open. |
| `ifc_header` | STEP header: description, originating file name, schema. |
| `ifc_type_counts` | Entity counts by IFC type, most common first. |
| `ifc_search` | Entities whose name, GlobalId, or type contains some text. |
| `ifc_entity` | One entity by STEP id, with its raw attributes. |
| `ifc_entities_of_type` | Every entity of one type. |
| `ifc_attributes` | Raw STEP attributes by position. |
| `ifc_properties` | Properties grouped by property set. |
| `ifc_quantities` | Lengths, areas, volumes, counts, weights, times. |
| `ifc_property_sets` | Set names and sizes, without values. |
| `ifc_parameters` | Every parameter in the model with element counts, ranges, and sample values. |
| `ifc_parameter_values` | The distinct values of one parameter and how many elements hold each. |
| `ifc_find_by_parameter` | Elements whose parameter passes a test (`eq`, `contains`, `gt`, …). |
| `ifc_parameter_table` | A row per element, a column per parameter. |
| `ifc_relations` | Relationship edges touching an entity. |
| `ifc_spatial_tree` | Project → site → building → storey → space. |
| `ifc_spatial_contents` | Elements directly inside one container. |
| `ifc_element_containment` | The container chain above an element. |
| `ifc_mesh` | Mesh statistics for an element or the whole model. |
| `ifc_bounds` | Bounding boxes, per element and whole-model. |
| `ifc_volume` | Volume and surface area from geometry. |
| `ifc_export_glb` | Writes a GLB. |
| `ifc_meshing_diagnostics` | What failed to mesh, and why. |
| `ifc_to_bos` | Converts a model to BIM Open Schema, optionally saving the `.bos` file. |
| `ifc_table` | The tables and views a query can use, with row counts and column types. |
| `ifc_sql` | A read-only DuckDB query over the converted model, paged. |
| `ifc_sql_export` | The full result of a query, written to `.csv`, `.parquet`, or `.json`. |

Anything returning a list takes `skip` and `take` and reports the unpaged `total`, so a caller can
tell a complete answer from a truncated one.

## Design notes

**Sessions.** Loading an IFC file is a whole-file parse, and an agent asks many small questions of
one model, so `IfcSessionCache` keeps recent models open — three by default, evicting the least
recently used. Relation and property indexes are each another full scan, so they are built on first
use and kept for the life of the session.

**Lifetime.** Every `IfcEntity` points into the file's pinned buffer. Nothing derived from a session
may outlive it, which is why tools serialize their answers before returning.

**Parameters run the other way.** `ifc_properties` and friends answer "what does element N carry"
and need an id in hand. Every question worth asking first runs the other way — which elements are
load bearing, what fire ratings exist, `Height` for all the windows — and answering those from the
per-element tools costs a call per element, each re-unwrapping value tokens. `IfcParameterIndex`
inverts the property data once per session: value text and its numeric reading are resolved a single
time and keyed by `(property set, name)`, so a query is a dictionary hit plus a walk of the matches.
It reads no extra bytes — it is a second pass over `IfcSession.Properties`, which is already in
memory — and in particular it does not touch the BOS conversion, so parameter questions stay cheap.
`ifc_sql` remains the tool for arbitrary joins; these four cover the common shapes without SQL.

**Analytics.** `IfcBosArtifacts` converts a model to a `.bos` (a zip of Brotli-compressed Parquet
tables) and loads it into a DuckDB database. Both are temp files owned by the session, so closing a
model or evicting it from the cache deletes them. The conversion is a second whole-file parse, so it
happens once per session rather than once per query.

**Why the views matter.** BIM Open Schema interns every string and every enum: `Entities.Name` is an
index into `Strings`, `Entities.Category` is an index into `Entities`, and `Parameters.Value` is a
tagged index whose meaning depends on its descriptor's type. A query against the raw tables can see
no text at all, so `IfcDuck.CreateViews` adds `EntityText`, `ParameterText`, and `RelationText`,
which resolve those indexes by joining on `rowid`.

**Spatial containers survive the conversion.** `IfcToBosConverter.HiddenIfcNames` reads like an
entity filter but is only a geometry-instance visibility flag, so site, building, and storey are all
present in `EntityText`. The converted relations are a flat edge list, though, so `ifc_spatial_tree`
is still the way to read the hierarchy.

## One upstream defect found and not fixed

**`IfcPropData` decodes property values but not property names.** `GetValueText` runs `DecodeIfc`,
while `ParseProperty` and `ParseElementQuantity` take names through `StripQuotes` alone. In any
model that names things outside ASCII, every parameter and property-set name reads back in its STEP
encoding — FZK-Haus indexes `H\X2\00F6\X0\he`, not `Höhe`, so a search for the real name finds
nothing. `IfcParameterIndex` decodes names as it builds, which is why the parameter tools show
`Höhe` while `ifc_properties` still shows the raw form. The fix belongs in `ext/Ara3D.IfcLoader`,
which this project does not edit; until it lands the two tool groups disagree on names.

## Two upstream defects found (and since fixed) here

Both were found by driving these tools against the FZK-Haus sample, and both are now fixed
upstream in `ext/Ara3D.IfcLoader`; the workarounds this project briefly carried are gone.

1. **`IfcPropData.ParseElementQuantity` read the wrong attributes** — name from attribute 0 and
   members from attribute 3, but `IFCELEMENTQUANTITY` is
   `(GlobalId, OwnerHistory, Name, Description, MethodOfMeasurement, Quantities)`. Every quantity
   set in every model read back named after its GlobalId GUID and containing nothing — 64 missing
   quantities on one FZK-Haus wall. Fixed to read name at 2, members at 5.

2. **A typed property value is not reachable through the attribute list.** For
   `IFCPROPERTYSINGLEVALUE('ConstructionMode',$,IFCLABEL('Massivhaus'),$)` the attribute at index 2
   is the bare token `IFCLABEL`; `StepTokenExtensions.AsList` steps over the `('Massivhaus')`
   payload to keep the arity right, so the value is absent from the list. That skip is load-bearing
   (changing it would shift attribute indices for every positional consumer), so the fix is
   `IfcPropValueExtensions.GetMeasureType/GetValueText`, which unwrap the payload from the token
   stream. Without them, every property reads back as the name of its own measure type.
