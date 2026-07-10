# WP-W5 — PRIMARK composite-curve closure (Wave 2 session)

**Date:** 2026-07-09  
**Verdict:** **Shipped** — composite trimmed-arc profile closure for PRIMARK plates

## Choice / ROI

Prioritized **WP-W5 PRIMARK** over placement (schependomlaan) this session: 403 `IFCDISCRETEACCESSORY` oracle-only products share one root cause (composite `IFCCOMPOSITECURVE` + `SenseAgreement=.F.` fillet arcs + `DISCONTINUOUS` close). Single `CurveEvaluator.cs` fix unlocks the weakest large-file gap.

## Diagnosis

1. Wave 2 `ConvertTrimValue` radian heuristic alone was insufficient.
2. Root cause: `NormalizeAngleSpan` collapsed long PARAMETER trims (0→3π/2) to 90° when `SenseAgreement=.F.` — fillet arcs disconnected from polylines → non-simple / unclosed rings → ear-clip failure.
3. Secondary: long-arc orientation ambiguous (u1 vs u2 start); fixed via continuous-segment join hint + alternate sweep.
4. Tertiary: `DISCONTINUOUS` closing segments re-listed earlier vertices → degenerate spike; skip first point when it duplicates any prior vertex.

Micro gate `PrimarkStyle_CompositePlateProfile_Micro_Builds` and catalog gate `#92859` both failed with `Ear clipping failed`; now pass.

## Fix (`CurveEvaluator.cs`)

- `ResolveConicTrimSweep` / `SampleTrimmedConicArc2D`: sense-F long arcs (|u2−u1|>π) sweep from u2 by raw span; short arcs sweep −raw then reverse.
- `OrientTrimmedArcForContinuousJoin` + `TrySampleAlternateLongTrimmedArc2D`: pick arc orientation from previous segment endpoint.
- `EvaluateCompositeCurve2D`: skip first point of `DISCONTINUOUS` segment when it duplicates an earlier vertex.

## Gate table

| Gate | Before | After | Pass |
|------|--------|-------|------|
| `Primark_DiscreteAccessory_CompositePlateProfile_Builds` (#92859) | FAIL (null mesh) | **PASS** | ✓ |
| `PrimarkStyle_CompositePlateProfile_Micro_Builds` | FAIL | **PASS** | ✓ |
| `Primark_DiscreteAccessory_ClippedPlate_Builds` | PASS | PASS | ✓ |
| T0 `GoldenMeshTests` | 64/64 | **64/64** | ✓ |
| T1 quick parity (ScorecardTests) | OpenHouse 0.942 / example 0.896 / steel 0.895 | **0.960 / 0.904 / 0.907** | ✓ (improved) |
| PRIMARK oracle-only < 100 | 404 | *not re-measured* (OracleEntityMap AV on full PRIMARK) | pending |
| PRIMARK parity ≥ 0.82 | 0.791 | pending stretch | pending |

## Tests

```
dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "FullyQualifiedName~WpW5Tests&FullyQualifiedName!~Diagnosis&FullyQualifiedName!~ScorePrimark"
dotnet test ... --filter "FullyQualifiedName~GoldenMeshTests"
dotnet test ... --filter "FullyQualifiedName~ScorecardTests"
```

## Files changed

- `Approach1/CurveEvaluator.cs`
- `tests/.../WpW5Tests.cs` (diagnostic Explicit test)

## Blockers / next

- **Placement (schependomlaan):** still open — deep `IFCLOCALPLACEMENT` / `IFCMAPPEDITEM` chains; use `TransformComparisonTests`.
- **PRIMARK stretch metrics:** `ScorePrimarkStretch_Diagnosis` crashes in `OracleEntityMap` (AccessViolation on large model) — harness issue, not mesher.
- **WP-W11:** DigitalHub mesh-bbox gate still pending re-verify.
- **Mesher index crashes** (9 IFCs): not addressed this session.
