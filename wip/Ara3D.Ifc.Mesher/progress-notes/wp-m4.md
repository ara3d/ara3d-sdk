# WP-M4 — v2 scorecard refresh (coordinator)

**Date:** 2026-07-09  
**Scope:** reporting only — stretch + catalog remeasure under v2 weights + mis-tag exclusion.

## Measured (Wave 2 session, partial — catalog T2 not re-run)

| File | Parity | entityShape | mergedMesh | Notes |
|------|-------:|------------:|-----------:|-------|
| IfcOpenHouse_IFC4.ifc | 0.920 | 0.971 | 0.732 | T1 quick |
| example.ifc | 0.864 | 0.813 | 0.683 | T1 quick |
| steelplates.ifc | 0.867 | 0.914 | 0.553 | T1 quick |
| duplex.ifc | 0.874 | 0.882 (8 mis-tags excl.) | 0.711 | stretch |
| FM_ARC_DigitalHub.ifc | 0.910 | 0.919 | 0.672 | stretch; mesh-bbox 0.440 pre-W11 fix |

*v1 catalog rows (AC20, sculpture, C20, AC11, PRIMARK) still need `Category=IfcMesherCatalog` after Wave 2 merges.*

## Wave 1 deltas (from progress notes, for coordinator fold-in)

| File | Parity (Wave 1 notes) |
|------|------------------------|
| C20 Institute | 0.879 |
| AC11 Institute | 0.896 |
| AISC Sculpture | 0.925 |
| C20/AC11/PRIMARK | see wp-w4/w6/w5 notes |

## Rerun (full M4)

```
dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "Category=IfcMesherScore"
dotnet test ... --filter "FullyQualifiedName~ScoreDuplexStretch"
dotnet test ... --filter "FullyQualifiedName~ScoreDigitalHubStretch"
dotnet test ... --filter "Category=IfcMesherCatalog"
```

## Blockers

- Full T2 catalog blocked on clean CI-style run (file locks during parallel Wave 2).
- Coordinator should fold all `progress-notes/*.md` into PROGRESS.md and remove v1-weights banner after merge.
