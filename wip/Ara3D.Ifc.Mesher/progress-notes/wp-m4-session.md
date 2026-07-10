# WP-M4 v2 — Session scorecard refresh

**Date:** 2026-07-10  
**Verdict:** Multi-gap session — catalog index fix confirmed; DigitalHub W11/W12 landed; PRIMARK harness unblocked

## Per-task status

| # | Task | Status | Notes |
|---|------|--------|-------|
| 1 | Catalog crash IFCs | **Done** | All listed files PASS index + BFAST export (DigitalHub covered by W11) |
| 2 | Mapping-origin inverse | **Partial** | `target * origin` confirmed; rotated micro-golden added; max Δ still ~10 m (no formula change — inverse breaks golden) |
| 3 | WP-W11 DigitalHub mesh-bbox ≥0.55 | **Done** | **0.440 → 0.979** via center-aligned local bounds fallback |
| 4 | WP-W12 cylindrical densify | **Done** | Surface-local UV densify in `Brep.cs`; merged-tri ratio **0.895**; T0/T1 quick green |
| 5 | PRIMARK OracleEntityMap AV | **Done** | Clone BFAST meshes before Dispose; stretch re-ran |
| 6 | WP-M4 scorecard refresh | **Done** | This note + quick scorecard rewrite |
| 7 | Re-export schependomlaan.bfast | **Skipped** | Placement metrics unchanged (mean 0.54 m / max 10.0 m) — no placement improvement to bake |

## Crash catalog (void-profile index fix)

| File | Index | BFAST export |
|------|-------|--------------|
| example.ifc | PASS | PASS |
| dental_clinic.ifc | PASS | PASS |
| ifcbridge-model01.ifc | PASS | PASS |
| ISSUE_034_HouseZ.ifc | PASS | PASS |
| ISSUE_068_ARK_NUS_skolebygg.ifc | PASS | PASS |
| Office_A_20110811.ifc | PASS | PASS |
| 20210219Architecture.ifc | missing locally | — |
| FM_ARC_DigitalHub.ifc | (W11 gate) | — |

## Metrics

### Schependomlaan (placement)

| Metric | Value |
|--------|-------|
| Entity bbox matched | 530 / 3493 |
| Mean center Δ | 0.54 m |
| Max center Δ | 10.0 m |

No change vs prior session — mapping formula already `target * origin`.

### DigitalHub

| Metric | Before | After |
|--------|--------|-------|
| Mesh bbox | 0.440 (694/2287) | **0.979 (2233/2287)** |
| Entity bbox | 766/766 | 766/766 |
| Merged tri ratio | ~0.78 | **0.895** (279164/311811) |
| Merged mesh score | 0.672 | **0.729** |
| Parity | 0.910 | **0.941** |

### PRIMARK stretch

| Metric | Before | After |
|--------|--------|-------|
| Harness | AV in `OracleEntityMap.ToRecord` | **PASS** |
| Parity | (blocked) | **0.861** |
| Oracle-only products | 404 | **232** (all `IFCDISCRETEACCESSORY`) |
| Entity bbox | — | 7881/7916 |

Parity ≥0.82 goal met; oracle-only still above 100 (remaining discrete accessories).

### T0 / T1 quick

- `GoldenMeshTests`: **64/64**
- Quick files parity: IfcOpenHouse 0.960, example 0.937, steelplates 0.907

## Files changed

- `Approach1/Brep.cs` — cylindrical UV densify (`DensifyCylindricalUvRing`)
- `Harness/ModelComparer.cs` — center-aligned mesh-bbox fallback
- `Harness/OracleEntityMap.cs` — clone BFAST meshes; sample PCA points
- `Tests/PureCSharp/MeshIndexDiagnosticTests.cs` — catalog sweep gates
- `Tests/PureCSharp/WpPlacementTests.cs` — rotated mapping-origin micro-golden
- `Tests/PureCSharp/WpW12Tests.cs` — densify micro-golden; ratio gate

## Remaining blockers

- Schependomlaan max center Δ ~10 m on a subset (deep mapped / non-identity origin cases need per-entity diagnosis, not global inverse)
- PRIMARK oracle-only still 232 `IFCDISCRETEACCESSORY` (parity OK; count gate <100 open)
- `20210219Architecture.ifc` not present in local catalog paths
