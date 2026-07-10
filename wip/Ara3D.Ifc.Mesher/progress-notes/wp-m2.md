# WP-M2 — Exclude oracle mis-tags from entityShape score

**Status:** done (Wave 0)

## Root cause / rationale

WP-W9 proved duplex/example low `entityShape` scores are driven by oracle BFAST per-entity mesh
mis-tags (slab cluster permutation), not candidate defects. `MisTagSuspectId` already flags these;
this WP excludes flagged entities from the scored average so parity reflects candidate quality.

## Changes

- `ModelComparer.CompareEntityShapes`: flag mis-tags on all entities; average score over
  non-excluded only; `EntityShapeMetricScore.ExcludedMisTagCount` added.
- `FormatResult`: reports excluded mis-tag count when > 0.
- `WpM2Tests.cs`: synthetic permutation (2 entities) → both excluded, score ~1; real mismatch not excluded.

## Before/after (expected on duplex)

- entityShape should rise (mis-tagged slabs excluded from average)
- parity may rise slightly via v2 weights

## Rerun

```
dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "FullyQualifiedName~WpM2Tests"
dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "FullyQualifiedName~ScoreDuplexStretch"
dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "Category=IfcMesherScore"
```
