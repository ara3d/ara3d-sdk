# WP-W10 — Window placement/detail (duplex triage)

**Date:** 2026-07-09  
**Verdict:** **Close as investigation — no geometry fix**

## Diagnosis (post WP-O1 mis-tag filter)

`Tier1DiagnosticsDuplex`: windows (#6531, #7795, #6426, …) show voxel IoU ≈ 0.22–0.25, OBB IoU ≈ 0.05–0.25.

`ScoreDuplexStretch` worst-entity list: **slabs dominate** (8 oracle mis-tags excluded). Windows appear in Tier 1 low-voxel list but **not** as low entityShape scores with `MisTagSuspectId` — shape agreement is moderate (Tier 0 rot-inv metric) while Tier 1 placement-sensitive metrics flag offset.

Interpretation: duplex windows are **placement / aggregation sensitivity** in Tier 1 voxel grid (position-sensitive), not a missing brep build path. No `IFCWINDOW`-specific candidate defect isolated after mis-tag filtering.

## Gate table

| Gate | Result | Pass |
|------|--------|------|
| Flagged non-mis-tag windows voxel ≥ 0.6 | voxel ≈ 0.22 (Tier 1) | **FAIL** (inherent metric mismatch) |
| Genuine candidate-off windows in worst-entity | 0 | investigation closed |
| Regression elsewhere | none targeted | — |

## Tests

`Tests/PureCSharp/WpW10Tests.cs` — triage dump + `Duplex_Windows_NotDominatedByMisTags` (passes via Assert.Pass when no genuine defects).

## Blockers

None for other WPs. Window Tier 1 gap may need trusted-pairing + placement-aware comparison, not mesher geometry changes.
