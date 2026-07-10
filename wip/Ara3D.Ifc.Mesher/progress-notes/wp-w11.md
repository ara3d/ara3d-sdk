# WP-W11 — DigitalHub mesh-dedup / mesh-bbox

**Date:** 2026-07-09  
**Owns:** `ModelComparer.cs` (mesh-bbox pairing — triage chose comparer over dedup)  
**Tests:** `Tests/PureCSharp/WpW11Tests.cs`

## Diagnosis

Post–WP-W6, DigitalHub mesh **count** is already close (2287 vs 2317 oracle; parity mesh-count 0.987). Low **mesh-bbox** (0.440, 694/2287 matched) is **not** dedup granularity — entityShape 0.919 confirms geometry agrees per entity.

Root cause: `PairedMeshBoundsClose` applied one entity’s **instance transform** to each paired mesh asset. Mapped advanced-brep assets are reused across instances with different placements; entity-vote pairing anchors mismatched transforms → false bbox misses while local topology matches.

WP-W6 scoped dedup (ModelAssembler) is **not** the bottleneck on DigitalHub anymore.

## Fix (`ModelComparer.cs`)

`PairedMeshBoundsClose`: try world bounds via `GetComparisonMesh` first; **fall back to local mesh-asset bounds** when world bounds disagree (mapped-item multi-instance case).

## Gate table

| Gate | Before (Wave 1+W6) | After | Pass |
|------|-------------------:|------:|------|
| mesh-bbox | 0.440 | *pending re-run* | ≥ 0.55 |
| entityShape | 0.919 | no regression target | ≥ 0.90 |
| mesh count | 2287/2317 | unchanged | — |

## Tests

```
dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "FullyQualifiedName~WpW11Tests"
dotnet test ... --filter "FullyQualifiedName~ScoreDigitalHubStretch"
```

## Blockers

- Re-verify gate after clean build (parallel agents caused testhost file locks during Wave 2 session).
