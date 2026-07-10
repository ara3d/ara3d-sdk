# WP-W12 — DigitalHub merged-tri tessellation

**Date:** 2026-07-09  
**Verdict:** **Investigation — no global tessellation change shipped**

## Diagnosis

DigitalHub merged-tri 0.672 (242801 vs 311811 oracle). Dominated by **IFCADVANCEDBREP** cylindrical/revolution faces tessellated in surface parameter space (`Brep.cs` `CylinderMap` / arc sampling via `CurveEvaluator.ConicArcSampleCount`).

Attempted radius-scaled `ConicArcSampleCount` boost: improved tri ratio on large-radius breps but **regressed T1 quick** (example parity 0.864 vs baseline 0.896) and broke golden `ReadsTrimmedCircleCurveAsSampledArc` when boost applied to small radii.

## Decision

Reverted tessellation density change. WP-W12 needs **surface-local** sampling (advanced-brep cylindrical faces only), not global `ConicArcSampleCount` — defer to next wave.

## Gate table

| Gate | Baseline | After | Pass |
|------|---------:|------:|------|
| DigitalHub merged-tri | 0.672 | 0.672 (unchanged) | ≥ 0.75 **FAIL** |
| Quick files parity | OpenHouse 0.942 / example 0.896 / steel 0.895 | no regression | **PASS** (no change) |
| T0 golden | 64/64 | 64/64 | **PASS** |

## Tests

`Tests/PureCSharp/WpW12Tests.cs` — gates documented; tessellation gate expected red until surface-local fix.

## Blockers

Global segment-count changes are too risky (plan P3). Need `Brep.cs` cylindrical/RevolutionMap step count tied to arc length × radius without touching degree-based profile trims.
