# WP-W5 — PRIMARK retry (Wave 2)

**Date:** 2026-07-09  
**Partial fix:** `CurveEvaluator.ConvertTrimValue` — PARAMETER trims on circles/ellipses now accept **radians** when |value| ≤ π (PRIMARK fillet arcs use radian PARAMETERVALUE; degree trims still work for values > π).

## Diagnosis re-run

Wave 1 defer pointed at composite+trimmed-arc profiles. Trim fix alone **insufficient**:

| Test | Result |
|------|--------|
| `Primark_DiscreteAccessory_ClippedPlate_Builds` | **PASS** |
| `Primark_DiscreteAccessory_CompositePlateProfile_Builds` (#92859) | **FAIL** (mesh null) |
| `PrimarkStyle_CompositePlateProfile_Micro_Builds` | **FAIL** (mesh null) |

Remaining root cause: **IFCCOMPOSITECURVE** closure (CONTINUOUS + DISCONTINUOUS segments) → invalid/non-closed profile polygon for extrusion. Still in `CurveEvaluator.cs` / profile sanitize path.

## Gate table

| Gate | Target | Actual | Pass |
|------|--------|--------|------|
| Oracle-only products | < 100 | 404 (unchanged) | **FAIL** |
| Parity | ≥ 0.82 | 0.791 (v1 catalog) | **FAIL** |
| Quick files | no regression | green when run | **PASS** |

## Files changed (Wave 2)

- `Approach1/CurveEvaluator.cs` — `ConvertTrimValue` radian/degree heuristic

## Blockers

Composite-curve DISCONTINUOUS closure for PRIMARK plate profiles — needs dedicated WP (still CurveEvaluator-owned).
