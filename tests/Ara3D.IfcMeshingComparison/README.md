# Ara3D.IfcMeshingComparison

Single home for IFC meshing tests, backend comparison, and reporting.

## Backends

| Name | Source | Description |
|------|--------|-------------|
| **WebIfcDll** | `IfcFile(includeGeometry:true).ToModel3D()` | Native web-ifc DLL tessellation (oracle) |
| **Approach1** | `Ara3D.Ifc.Mesher.Approach1` | Modular pure C# mesher (`GeometryDispatcher` + `ModelAssembler`) |

`Tests/Native/IfcMeshingTests` validates the native web-ifc path (geometry enumeration, buffer checks, `ToModel3D`).

## Test layout

```
Tests/
  Native/           WebIfc validation tests
  PureCSharp/       Unit, integration, and golden micro-IFC tests for Approach1
  Comparison/       Backend smoke tests and cross-backend comparison runners
  Support/          Shared helpers (MicroIfc, OracleComparison, MeshTestAssert)
Harness/            Test file catalog, BFAST oracles, geometry comparison utilities
Meshers/            IMeshingBackend implementations
Reporting/          Markdown report generation
```

Meshing implementation lives in `wip/Ara3D.Ifc.Mesher/Approach1`.

## Data layout (gitignored)

```
data/ifc/              — local IFC copies (from external corpora)
data/bfast/webifc/     — WebIfc BFAST oracle files
data/reports/          — generated markdown reports
```

## Populate data

```bat
dotnet test ara3d-sdk\tests\Ara3D.IfcMeshingComparison --filter "GenerateWebIfcBfastOracles"
```

## Run comparisons

Quick comparison (IfcOpenHouse, example, steelplates):

```bat
dotnet test ara3d-sdk\tests\Ara3D.IfcMeshingComparison --filter "RunQuickComparison"
```

Full catalog comparison:

```bat
dotnet test ara3d-sdk\tests\Ara3D.IfcMeshingComparison --filter "RunFullComparison"
```

Reports write to `data/reports/comparison_{timestamp}.md` and `data/reports/capabilities.md`.

## Fast tests (no data required)

```bat
dotnet test ara3d-sdk\tests\Ara3D.IfcMeshingComparison --filter "Category!=Slow&Category!=Explicit"
```

Golden micro-IFC tests:

```bat
dotnet test ara3d-sdk\tests\Ara3D.IfcMeshingComparison --filter "FullyQualifiedName~GoldenMeshTests"
```
