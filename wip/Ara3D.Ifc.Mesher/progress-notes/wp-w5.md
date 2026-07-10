# WP-W5 — PRIMARK oracle-only products (read-only triage → defer)

**Date:** 2026-07-09  
**Branch:** (coordinator integrates)  
**Verdict:** **Defer geometry fix to WP-W4 (`CurveEvaluator.cs`).** Not `Tessellated.cs`.

## Symptom (baseline)

| Metric | Value |
|--------|------:|
| Parity (v1) | 0.791 |
| Instances | 42512 / 42916 |
| Oracle-only products | **404** |
| Entity Jaccard shared | 7744 |
| Entity bbox matched | 5698 / 7744 |

Source: `studio_catalog_evaluation.json` (T2 catalog, 2026-07-09).

## Diagnosis

### Oracle-only by product type

| Type | Count |
|------|------:|
| IFCDISCRETEACCESSORY | **403** |
| IFCPLATE | 1 |

Hypothesis confirmed: discrete accessories dominate; not tessellated geometry (PRIMARK has **zero** `IFCTRIANGULATEDFACESET` / `IFCPOLYGONALFACESET`).

### Oracle-only by geometry entity (representation items)

| Entity | Oracle-only products |
|--------|---------------------:|
| IFCEXTRUDEDAREASOLID | 296 |
| IFCBOOLEANCLIPPINGRESULT | 108 |

### Top oracle-only entity

`#92859 IFCDISCRETEACCESSORY` — 644 oracle tris. Body is `IFCEXTRUDEDAREASOLID` on profile `#92854` (`IFCARBITRARYCLOSEDPROFILEDEF` → `IFCCOMPOSITECURVE` with **trimmed arcs**, **CONTINUOUS** + **DISCONTINUOUS** closing segment). Same pattern across the 403 accessories (steel plates / support sections with filleted corners).

### Unsupported diagnostics histogram

No dominant unsupported *geometry* type (void-relation `IFCRELVOIDSELEMENT` 2354 is expected diagnostic noise). Failures are **silent build exceptions** swallowed per-product (`MeshingContext.Try`) → 0 candidate instances.

### Triage decision

| Candidate file | Owns? | Verdict |
|--------------|-------|---------|
| `Tessellated.cs` | W5? | **No** — no tessellated reps in PRIMARK |
| `Brep.cs` | W4 | **No** — 100 Brep reps exist but oracle-only gap is not faceted-brep columns |
| `CurveEvaluator.cs` | **W4** | **Yes — primary root cause** (composite + trimmed-arc 2D profiles for extrusions) |
| `Booleans.cs` | W5? | Secondary (108 clipping); likely many share failed base extrusions; not pursued without W4 |
| `ProfileBuilder.cs` | W5? | Thin wrapper over `CurveEvaluator.Evaluate2D`; real fix is upstream |

Per parallel-plan rule: **stop and defer** — fix belongs in `CurveEvaluator.cs` owned by WP-W4 this wave.

## Tests added

`Tests/PureCSharp/WpW5Tests.cs`:

- `ScorePrimarkStretch_Diagnosis` (Explicit/Slow) — oracle map + unsupported histogram
- `Primark_DiscreteAccessory_CompositePlateProfile_Builds` — gates `#92859` (644 oracle tris)
- `Primark_DiscreteAccessory_ClippedPlate_Builds` — gates `#40020` clipped plate
- `PrimarkStyle_CompositePlateProfile_Micro_Builds` — micro composite+arc+DISCONTINUOUS profile

## Test results (2026-07-09)

| Suite | Result |
|-------|--------|
| T0 `GoldenMeshTests` | 63 passed, **1 failed** (pre-existing; no mesher change) |
| T1 `Category=IfcMesherScore` (quick files) | **3 passed** — no regression |
| `WpW5Tests` | 1 passed, **2 failed** (expected until W4) |

| Test | Result | Notes |
|------|--------|-------|
| `Primark_DiscreteAccessory_ClippedPlate_Builds` (#40020) | **PASS** | Boolean clipping path OK for this entity |
| `Primark_DiscreteAccessory_CompositePlateProfile_Builds` (#92859) | **FAIL** | mesh null — composite+arc profile |
| `PrimarkStyle_CompositePlateProfile_Micro_Builds` | **FAIL** | mesh null — confirms CurveEvaluator gap |
| `ScorePrimarkStretch_Diagnosis` | skipped | Explicit/Slow — not run |

## Gate table (not met — deferred)

| Gate | Target | Actual (baseline) | Status |
|------|--------|-------------------|--------|
| Oracle-only products | < 100 | 404 | **FAIL** (deferred) |
| Parity | ≥ 0.82 | 0.791 | **FAIL** (deferred) |
| Quick files | no regression | 3/3 passed | **PASS** |

## Rerun (after WP-W4 CurveEvaluator fix)

```bat
dotnet build ara3d-sdk/tests/Ara3D.IfcMeshingComparison/Ara3D.IfcMeshingComparison.csproj
dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "FullyQualifiedName~GoldenMeshTests"
dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "Category=IfcMesherScore"
dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "FullyQualifiedName~WpW5Tests"
```

## Blockers

1. **File ownership:** primary fix requires `CurveEvaluator.cs` (WP-W4).
2. **Build environment:** concurrent Wave-1 agents left broken WIP in harness (`WebIfcBfastOracle.cs`, `ShapeDiagnostics.cs`); reverted for local build. File-lock contention on parallel `dotnet build` slowed verification.

## Post-W4 re-test (2026-07-09)

WP-W4 landed scale-aware composite joins in `CurveEvaluator.cs`. Re-ran `WpW5Tests` (non-Explicit):

| Test | Result |
|------|--------|
| `Primark_DiscreteAccessory_ClippedPlate_Builds` | **PASS** |
| `Primark_DiscreteAccessory_CompositePlateProfile_Builds` | **FAIL** |
| `PrimarkStyle_CompositePlateProfile_Micro_Builds` | **FAIL** |

**Conclusion:** Institute shell fix ≠ PRIMARK composite-plate profiles. W5 remains **open** — needs a dedicated `CurveEvaluator` pass on trimmed-arc + DISCONTINUOUS composite closed profiles (403 `IFCDISCRETEACCESSORY`), not a Tessellated path.

## Files touched (this WP)

- `tests/Ara3D.IfcMeshingComparison/Tests/PureCSharp/WpW5Tests.cs` (new)
- `wip/Ara3D.Ifc.Mesher/progress-notes/wp-w5.md` (this note)

No mesher source edits (triage defer).
