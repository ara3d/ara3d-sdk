# Wave 0 baseline (post WP-M2, 2026-07-09)

Frozen numbers for Wave 1 agents to measure against. v2 weights + mis-tag exclusion active.

## T1 quick (`Category=IfcMesherScore`)

| File | Parity | entityShape | Notes |
|------|-------:|------------:|-------|
| IfcOpenHouse_IFC4.ifc | 0.942 | 0.969 (32/35) | |
| example.ifc | 0.896 | 0.813 (84/116) | |
| steelplates.ifc | 0.895 | 0.914 (10/14) | |

## Duplex stretch (`ScoreDuplexStretch`)

| Metric | Score |
|--------|------:|
| Parity | 0.873 |
| entityShape | 0.881 (135/227, 8 mis-tags excluded) |
| mergedMesh | 0.712 (24404/27686 tris) |
| inst | 714/682 |

## Rerun commands

```
dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "Category=IfcMesherScore"
dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "FullyQualifiedName~ScoreDuplexStretch"
dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "FullyQualifiedName~ScoreDigitalHubStretch"
```
