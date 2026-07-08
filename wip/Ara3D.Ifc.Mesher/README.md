# Ara3D.Ifc.Mesher

WIP library for pure C# IFC meshing (`Approach1`).

## Layout

| Folder | Description |
|---|---|
| `Approach1/` | Modular mesher: `GeometryDispatcher`, `ProfileBuilder`, `CurveEvaluator`, `ModelAssembler`, `GeometryCreationBacklog`, etc. |
| *(root)* | Common `IIfcMesher`, `IfcMeshingResult`, `IfcModelStats` |

## Entry point

```csharp
IIfcMesher mesher = new Approach1Mesher();
using var file = new IfcFile(path, includeGeometry: false);
IfcMeshingResult result = mesher.Build(file);
```

`IfcMeshingResult` carries a `Model3D`, triangle/bounds/volume stats, and diagnostic messages.

Tests and cross-backend comparison live in `tests/Ara3D.IfcMeshingComparison`.
