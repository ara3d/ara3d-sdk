# WP — Approach1 backlog session (placement triage + void seams + PRIMARK residual)

**Date:** 2026-07-10  
**Verdict:** Mixed — nested mapped-item fix + void-extrusion seam fix shipped; schependomlaan max-Δ diagnose-only; PRIMARK residual still null-mesh profiles

## Per-item status

| # | Task | Status | Notes |
|---|------|--------|-------|
| 1 | Schependomlaan max-Δ triage | **Diagnose + partial fix** | Nested `IFCMAPPEDITEM` multiply order fixed; worst Δ still windows/doors (SurfaceModel FacetedBrep in world-ish map coords). Mean/max center Δ unchanged (0.54 / 10.0 m) |
| 2 | PRIMARK 232 accessories | **Diagnose-only** | All 232 still `BuildEntityMesh` null: **124** bare `IFCEXTRUDEDAREASOLID`, **108** `IFCBOOLEANCLIPPINGRESULT`. Parity **0.861** held. Next fix still profile/`CurveEvaluator` (not safe to reopen deep composite this session) |
| 3 | Quick-file volumeRatio≈0 | **Shipped** | Offset-ring hole resample left wall loops on original hole verts → open seams. Aligned hole rings before extrusion. example entityShape **0.914→0.928**; `#12799` openEdges **152→0**, shape **0.643→0.741** |
| 4 | Split schependomlaan bbox % | **Shipped** | Harness classifies placement-matrix vs shape-inflated vs matched (baking-aware). Paired: **21%** matrix / **19%** shape-inflated / **60%** matched — 15% bbox is not pure transform |
| 5 | W10 duplex windows | **Skipped** | Time used on 1–4 |

## Schependomlaan max-Δ (item 1)

### Worst instances (after nested fix)

Top center-Δ are `IFCWINDOW` / `IFCDOOR` (e.g. `#948940` velux), Rot°≈0, Trans/Frobenius large.

### `#948940` root cause (diagnose-only)

- Product placement chain resolves correctly to `(26.6, 0, 0)`.
- Mapped Body/`SurfaceModel` → `IFCFACETEDBREP` vertices already at building-scale coords (e.g. `(-20.4, 7.2, 11.2)` m) with **identity** MappingOrigin/Target.
- Applying correct product placement yields world centers ~`(6.2, 7.5, 11.2)` vs oracle ~`(16.4, 7.5, 11.2)` (~10 m X).
- Not fixed by mapping-origin inverse (origin is identity; global inverse still forbidden).
- Instance pairing by equal tri-count also mixes dissimilar 12-tri stubs.

### Nested mapped-item fix

`CollectMappedItem` / `CollectScopedParts` now compose `mapping * parent` (row-vector: inner then outer). Micro-golden proves wrong `parent * mapping` rotates translations.

| Metric | Before | After |
|--------|--------|-------|
| Mean center Δ | 0.54 m | **0.54 m** (unchanged) |
| Max center Δ | 10.0 m | **10.0 m** (unchanged) |
| Entity bbox | 530/3493 (15.2%) | **530/3493 (15.2%)** |

## PRIMARK (item 2)

| Metric | Value |
|--------|------:|
| Parity | **0.861** (≥0.861 gate) |
| Oracle-only | **232** (all `IFCDISCRETEACCESSORY`) |
| Null mesh among oracle-only | **232 / 232** |
| Extrusion-only | 124 |
| Boolean clipping | 108 |

Clipping residuals likely fail because the first-operand extrusion already returns null (same profile path).

## Void extrusion seams (item 3)

**Root cause:** `TryTriangulateOffsetRing` resamples the hole to the outer vertex count for annular caps, but `BuildExtrusionWithHoles` built hole walls from the **original** hole ring → unmatched vertices → open edges → unreliable signed volume vs oracle.

**Fix:** `AlignHoleRingsForExtrusion` + public `ResampleClosedRing` so walls share cap vertices.

| Gate | Before | After |
|------|--------|-------|
| `#12799` openEdges | 152 | **0** |
| `#12799` entityShape | 0.643 | **0.741** |
| example entityShape | 0.914 | **0.928** |
| example parity | 0.937 | **0.939** |
| `#12799` vol similarity | 0 (vs open oracle) | still 0 (oracle openEdges=770, vol≪candidate) |

Steelplates `#1193/#633/#1385` unchanged (boolean-clip / open-boundary vs oracle) — no clear safe Booleans.cs fix this session.

## Bbox split (item 4)

Baking-aware `PlacementDeltaClass`: prefer world center + rotation over raw Frobenius.

Schependomlaan paired instances: **578** placement-matrix (21%), **509** shape-inflated (19%), **1634** matched (60%).

## T0 / T1

- `GoldenMeshTests`: **64/64**
- Quick scorecard: IfcOpenHouse **0.960**, example **0.939**, steelplates **0.907**

## Files changed

- `Approach1/GeometryPartCollector.cs` — nested map `mapping * parent`
- `Approach1/ModelAssembler.cs` — same for scoped parts
- `Approach1/PolygonWithHoles.cs` — public `ResampleClosedRing`
- `Approach1/MeshHelpers.cs` — `AlignHoleRingsForExtrusion`
- `Harness/TransformComparison.cs` — placement vs shape classification + summary split
- Tests: `WpNestedMappedTests.cs`, `WpSchependomlaanTriageTests.cs`, `WpSchependomlaanWindowTests.cs`, `WpEntityShapeVolumeTests.cs`, `WpPrimarkResidualTests.cs`

## Remaining blockers

1. Schependomlaan window/door max-Δ: world-baked FacetedBrep inside identity-mapped RepresentationMap — needs a careful per-map heuristic, not global origin inverse.
2. PRIMARK 232: still null meshes on composite/trimmed profiles (CurveEvaluator / profile sanitize).
3. example SHS vol similarity vs oracle remains 0 while candidate is watertight (oracle open shell).
4. steelplates clipped beams: open-boundary / volume disagreement — boolean path TBD.
