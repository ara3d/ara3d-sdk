# WP-M3 — Tier 2 metrics: Chamfer/Hausdorff + convex-hull IoU

**Status:** done (Wave 1)

## Scope

On-demand Tier 2 per-entity shape diagnostics (harness only, **not scored**):
directed + symmetric Chamfer (mean) and Hausdorff (max) surface distance via `AabbTree`
nearest-point queries on triangle meshes; optional convex-hull IoU via brute-force hull faces +
Tier 1 voxel overlap.

## Changes

- `ShapeDiagnostics.cs`: `EntityTier2Diagnostic`, `MeshSurfaceDistance`,
  `CompareEntitiesTier2`, `CompareMeshSurfaces`; `MeshSurfaceIndex` + `PointClosestTriangleQuery`
  over `Ara3D.Geometry.AabbTree`.
- `WpM3Tests.cs`: unit tests (identical → ~0; displaced rod → distance ≈ displacement);
  explicit `Tier2DiagnosticsDuplex` + `Tier2DiagnosticsDigitalHub` (`[Category("Slow")]` on DigitalHub).

## API

| Member | Description |
|--------|-------------|
| `CompareEntitiesTier2(candidate, oracle)` | Per shared entity; sorted worst Hausdorff first |
| `CompareMeshSurfaces(a, b)` | Mesh-level Chamfer/Hausdorff + optional hull IoU |
| `EntityTier2Diagnostic` | Directed + symmetric Chamfer/Hausdorff, `ConvexHullIoU?` |

## Blockers / notes

- No dedicated mesh `AABBTree` type in SDK; built per mesh from triangle `Bounds3D` lists via `Ara3D.Geometry.AabbTree`.
- Convex hull is O(n³) on subsampled vertices (default max 100) — fine for diagnostics, not scoring.
- Vertex-only sampling: symmetric Chamfer for half-length X translation can read low when faces coincide; unit test uses Y translation + Hausdorff for max distance.
- **Integration:** full `Ara3D.IfcMeshingComparison` build currently blocked by in-flight parallel WPs (`WpO1Tests` / `WpW4Tests` compile errors against incomplete `WebIfcBfastOracle` / `MergedMeshMetricScore` APIs). WP-M3 code and `WpM3Tests` compile and pass when those files are absent.
- Duplex/DigitalHub dumps are `[Explicit]`; run manually for frontier triage.

## Test results

```
dotnet test ... --filter "FullyQualifiedName~WpM3Tests&TestCategory!=Explicit"
Passed: 3 (Tier2_IdenticalMeshes, Tier2_DisplacedRod, Tier2_EntityComparison_OrdersByWorstHausdorff)
```

## Rerun

```
dotnet build ara3d-sdk/tests/Ara3D.IfcMeshingComparison/Ara3D.IfcMeshingComparison.csproj
dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "FullyQualifiedName~WpM3Tests&TestCategory!=Explicit"
dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "FullyQualifiedName~Tier2DiagnosticsDuplex"
dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "FullyQualifiedName~Tier2DiagnosticsDigitalHub"
```
