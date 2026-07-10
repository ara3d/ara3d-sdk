# WP-W6 — Sculpture mesh dedup polish

**Status:** done

## Diagnosis

Sculpture was **120/145 meshes** with perfect instancing (546/546). Fingerprint-only dedup was not
mis-merging different topologies — diagnosis showed **120 unique oracle topologies** vs **145 oracle
mesh indices**. The 25-gap is **granularity**: web-ifc keeps separate mesh slots per
`IFCREPRESENTATIONMAP` / `IFCSHAPEREPRESENTATION` even when local geometry is identical (bolt-cap
type maps with coarse shared hex heads). Global `(fingerprint → mesh)` collapsed those oracle slots.

First-16-vertex sampling was also too coarse for bolt assemblies (cap verts dominate the buffer).

## Fix (`ModelAssembler.cs` only)

1. **`CollectScopedParts`** — mirrors collector walk but tags each part with dedup scope =
   representation-map id (mapped items) or shape-representation id (direct breps).
2. **`GetOrAddMesh(scope, mesh)`** — bucket key `(scope << 32 | fingerprint)`; topology equality
   confirms reuse within a bucket.
3. **Fingerprint** — spread-samples vertices + triangle indices across the mesh (not just index 0..15).

## Gate table

| Gate | Before | After | Pass |
|------|--------|-------|------|
| Sculpture meshes | 120/145 | **145/145** | yes (≥130) |
| Sculpture parity | 0.920 | **0.925** | yes (≥0.92) |
| Sculpture inst Jaccard | 1.000 | 1.000 | yes |
| Sculpture mesh bbox | 0.966 | **1.000** | yes |
| OpenHouse parity | 0.942 | 0.961 | no regression |
| example parity | 0.896 | 0.904 | no regression |
| steelplates parity | 0.895 | 0.907 | no regression |
| Quick inst Jaccard | 1.000 | 1.000 | yes |
| T0 GoldenMeshTests | — | 64/64 | yes |
| ScoreAiscSculptureStretch | — | 0.925 | yes |

## Tests

- `Tests/PureCSharp/WpW6Tests.cs` — micro dedup guards + sculpture gates + quick Jaccard check

## Rerun

```
dotnet build ara3d-sdk/tests/Ara3D.IfcMeshingComparison/Ara3D.IfcMeshingComparison.csproj
dotnet test  ... --filter "FullyQualifiedName~WpW6Tests"
dotnet test  ... --filter "FullyQualifiedName~GoldenMeshTests"
dotnet test  ... --filter "Category=IfcMesherScore"
dotnet test  ... --filter "FullyQualifiedName~ScoreAiscSculptureStretch"
```

## Blockers / notes

- Test harness build initially blocked by parallel-agent `.cs.off` renames + stale `obj/` cache;
  `dotnet clean` on the test project resolved it. No source blockers remain.
- `mergedMesh` tri ratio (0.673) unchanged — tessellation density is out of scope (WP-W12).
