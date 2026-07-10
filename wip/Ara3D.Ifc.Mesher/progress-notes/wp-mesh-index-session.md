# WP — Mesh index crash fix (extrusion with voids)

**Date:** 2026-07-10  
**Verdict:** **Shipped** — `BuildExtrusionWithHoles` cap/top index bug

## Root cause

`MeshHelpers.BuildExtrusionWithHoles` snapshotted `bottomCount` after registering outer/hole ring vertices, then duplicated the top ring, then called `profile.Triangulate()` for end caps. Cap triangulation (offset-ring / ear-clip on `IFCARBITRARYPROFILEDEFWITHVOIDS`) can introduce bottom vertices not on the profile rings. `AddBottom` during cap emission appended those after the top ring, while `TopIndex(bottom)` still used the stale `topOffset` — producing face indices like `(76,77,228)` on 188-point meshes.

Affected **18 meshes** on `example.ifc` (SHS columns/beams #7663, #7949, …) and likely the other catalog export crashes sharing void-profile extrusions.

## Fix

`MeshHelpers.cs`: pre-register all cap-triangulation vertices via `AddBottom` before fixing `bottomCount` and duplicating the top ring; reuse cached `capTris` for face emission.

## Gate table

| Gate | Before | After | Pass |
|------|--------|-------|------|
| `ExampleIfc_ExtrudedVoidProfiles_HaveValidMeshIndices` | 18 bad meshes | **0** | ✓ |
| `ExampleIfc_ExportsBfastWithoutIndexErrors` | crash | **PASS** | ✓ |
| `ExampleIfc_MeshBoundsWithinOracleTolerance` | IndexOOR | **PASS** | ✓ |
| T0 `GoldenMeshTests` | 64/64 | **64/64** | ✓ |
| `Schependomlaan_PlacementGate` | 530/3493, mean 0.54m | unchanged | ✓ |
| `DigitalHub_MeshBbox_AtLeast055` | 0.440 | 0.440 | pending (WP-W11) |

## Tests

```
dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "FullyQualifiedName~MeshIndexDiagnostic"
dotnet test ... --filter "FullyQualifiedName~GoldenMeshTests"
```

## Files changed

- `Approach1/MeshHelpers.cs`
- `tests/.../MeshIndexDiagnosticTests.cs` (new)

## Deferred

- Mapping-origin inverse vs `target*origin` (schependomlaan max Δ ~10m subset)
- DigitalHub mesh-bbox ≥0.55 (dedup/pairing — WP-W11)
- WP-W12 cylindrical tessellation, PRIMARK stretch, WP-M4 scorecard refresh
