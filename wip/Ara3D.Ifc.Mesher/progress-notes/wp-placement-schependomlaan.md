# WP — Placement transform chain (schependomlaan)

**Date:** 2026-07-09  
**Verdict:** **Shipped** — instance matrix multiply order fix

## Root cause

`ModelAssembler` composed instance transforms as `productMatrix * partTransform` (column-vector /
pre-multiply convention). The mesh pipeline uses `System.Numerics` row-vector transforms
(`Vector3.Transform`: `world = local * matrix`). With nested `IFCLOCALPLACEMENT` rotations and
`IFCMAPPEDITEM` mapping rotations, the reversed order rotated the product translation into wrong
world coordinates — translation-only deltas (Rot° ≈ 0) on flow segments, railings, stairs.

Diagnosis on `#44513` (IFCFLOWSEGMENT): product placement origin `(21.66, 6.98, -0.24)` matched
oracle, but instance matrix was `(-10.38, -20.25, -0.24)` because `product * mapping` rotated the
translation; `mapping * product` (correct row-vector order) matched placement.

BFAST I/O was ruled out earlier (round-trip deltas 0).

## Fix

- `ModelAssembler.cs`: instance matrix `part.Part.Transform * productMatrix`; same order for void
  carve inverse and aggregated-child world bake.
- No change to `TryGetMappedItemTransform` formula (inverse origin breaks existing golden).

## Before / after (schependomlaan)

| Metric | Before | After |
|--------|--------|-------|
| Entity bbox matched | 479 / 3493 (13.7%) | **530 / 3493 (15.2%)** |
| Mean bounds-center Δ | 1.70 m | **0.54 m** |
| Max bounds-center Δ | 77.9 m | **10.0 m** |
| Parity | 0.719 | **0.721** |

Duplex control mean center Δ: **0.53 m** (unchanged band).

## Tests

- `WpPlacementTests.cs` — local placement chain, mapped-item formula, explicit schependomlaan gate
- T0 `GoldenMeshTests` 64/64 green
- T1 `ScorecardTests` + `Duplex_Control_TransformsMostlyMatchOracle` green

## Remaining gaps

- Entity bbox match still low (15%) — residual per-entity shape/instancing gaps, not global placement drift
- Max center Δ ~10 m on a subset (likely deep mapped-item / non-identity mapping-origin cases)
- Mapping-origin inverse vs `target * origin` still open for non-identity origins without breaking golden
