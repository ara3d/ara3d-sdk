# Ara3D.Ifc.Mcp

An MCP server that answers questions about IFC files. Built on `wip/Ara3D.MCP` for the protocol
and `ext/Ara3D.IfcLoader` for the entity layer.

Geometry is never loaded (`includeGeometry: false`), so the native `web-ifc-library.dll` is not
touched by anything here. The project still targets `net8.0-windows` because `Ara3D.IfcLoader`
does.

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
| `ifc_relations` | Relationship edges touching an entity. |
| `ifc_spatial_tree` | Project → site → building → storey → space. |
| `ifc_spatial_contents` | Elements directly inside one container. |
| `ifc_element_containment` | The container chain above an element. |

Anything returning a list takes `skip` and `take` and reports the unpaged `total`, so a caller can
tell a complete answer from a truncated one.

## Design notes

**Sessions.** Loading an IFC file is a whole-file parse, and an agent asks many small questions of
one model, so `IfcSessionCache` keeps recent models open — three by default, evicting the least
recently used. Relation and property indexes are each another full scan, so they are built on first
use and kept for the life of the session.

**Lifetime.** Every `IfcEntity` points into the file's pinned buffer. Nothing derived from a session
may outlive it, which is why tools serialize their answers before returning.

## Two upstream defects worked around here

Both live in code this project may not edit. Fixing them upstream would let the workarounds go.

1. **`IfcPropData.ParseElementQuantity` reads the wrong attributes**
   (`ext/Ara3D.IfcLoader/IfcPropData.cs:125`). It takes the name from attribute 0 and the members
   from attribute 3, but `IFCELEMENTQUANTITY` is
   `(GlobalId, OwnerHistory, Name, Description, MethodOfMeasurement, Quantities)` — name at 2,
   members at 5. Every quantity set in every model therefore reads back named after its GlobalId
   GUID and containing nothing. On one FZK-Haus wall that is 64 missing quantities.
   `IfcPropertySets` reads the entity directly instead.

2. **A typed property value is not reachable through the attribute list.** For
   `IFCPROPERTYSINGLEVALUE('ConstructionMode',$,IFCLABEL('Massivhaus'),$)` the attribute at index 2
   is the bare token `IFCLABEL`; `StepTokenExtensions.AsList` steps over the `('Massivhaus')`
   payload to keep the arity right, so the value is absent from the list. It is still in the token
   stream, one position along. `IfcPropertyText` unwraps it. Without that, every property reads
   back as the name of its own measure type.
