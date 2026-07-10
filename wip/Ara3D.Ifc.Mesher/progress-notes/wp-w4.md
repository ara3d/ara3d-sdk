# WP-W4 — Institute tri ratio (C20 + AC11)

**Date:** 2026-07-09  
**Owns:** `Approach1/Brep.cs`, `Approach1/CurveEvaluator.cs`  
**Tests:** `Tests/PureCSharp/WpW4Tests.cs`

## Diagnosis

Institute variants have **matching instance counts** but very low merged-triangle ratio vs oracle (C20 ~0.27, AC11 ~0.32 at baseline).

Catalog/inventory review:
- **C20** is dominated by **122 `IFCSHELLBASEDSURFACEMODEL` / `IFCOPENSHELL`** helix/tread geometry (mapped furnishing type `Kubus-Einrichtung`), not `IFCADVANCEDBREP`.
- **`IFCCURVEBOUNDEDPLANE` / `IFCCOMPOSITECURVE`** counts are high but almost entirely **`IFCRELSPACEBOUNDARY`** (space boundaries) — not in the meshed product body path.
- **AC11** is mostly `IFCFACETEDBREP` + extrusions; no shell surface models.

Root cause on C20: helix shells tessellate many **3-vertex facets** with ~0.1 mm edges (e.g. face `#14947`: points at x=1.5075 vs 1.5074). `PolygonTriangulator.EarClipTriangulate` treats corners with cross ≤ `Eps` as non-convex → throws → `BuildFaceSet` **catch/continue** drops the face. Hundreds of mapped tread instances × skipped facets → ~10k missing merged tris.

## Fix (minimal)

**`Brep.cs`**
- `TryTriangulateFaceRing`: direct emit for 3- and 4-vertex rings (bypass ear clip on thin facets); convex-quad split fallback.
- `ComputeTrianglePlane` + use for 3-point faces in `ResolveSurfaceMap`.
- `TryGetFacePlane(face, outer)`: reject declared `IFCPLANE` when boundary points are off-surface (shared-plane refs) → Newell plane per face (fixes golden box regression).

**`CurveEvaluator.cs`**
- `EvaluateCompositeCurve3D`: scale-aware join tolerance (`CompositeJoinToleranceSquared`), `EvaluateCompositeSegment3D` for trimmed segments, open-path dedupe (parity with 2D path).

## Gate table

| File | Metric | Before | After | Gate |
|------|--------|-------:|------:|------|
| C20-Institute-Var-2.ifc | Parity | 0.817 | **0.879** | ≥ 0.85 ✓ |
| | entityShape | — | **0.890** | ≥ 0.85 ✓ |
| | merged tris (cand/oracle) | 3618/13529 (0.27) | **56858/44868 (1.27)** | ≥ 0.35 ✓ |
| | inst | 2094/2098 | 2094/2098 | — |
| AC11 Institute | Parity | 0.859 | **0.896** | ≥ 0.85 ✓ |
| | entityShape | — | **0.891** | ≥ 0.85 ✓ |
| | merged tris | 3538/11194 (0.32) | **52351/42572 (1.23)** | — |
| | inst | 1612/1612 | 1612/1612 | — |
| Quick files | OpenHouse / example / steel | green | **green** (0.942 / 0.896 / 0.895) | no regression ✓ |
| T0 golden | GoldenMeshTests | — | **66/66** | ✓ |

## Tests

```
dotnet build ara3d-sdk/tests/Ara3D.IfcMeshingComparison/Ara3D.IfcMeshingComparison.csproj
dotnet test  ... --filter "FullyQualifiedName~GoldenMeshTests"
dotnet test  ... --filter "FullyQualifiedName~WpW4Tests"
dotnet test  ... --filter "FullyQualifiedName~ScoreC20InstituteStretch"
dotnet test  ... --filter "FullyQualifiedName~ScoreAc11InstituteStretch"
dotnet test  ... --filter "Category=IfcMesherScore"
```

## Notes

- Candidate now **overshoots** oracle merged tri count on both Institute files (finer helix facets retained). Parity and entityShape improved; mesh-count / mesh-bbox remain lower (dedup granularity — out of scope).
- Parallel Wave-1 WPs (`WpM3Tests`, `WpO1Tests`) may block clean builds until coordinator merges; local build succeeded after disabling incomplete `WpM3Tests.cs.off` duplicate.
