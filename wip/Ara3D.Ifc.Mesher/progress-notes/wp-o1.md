# WP-O1 — Duplex oracle mis-tag: fixable vs inherent

**Status:** done — **fixable** (stale BFAST cache); trusted-pairing filter shipped

## Conclusion

Duplex per-entity mesh mis-tagging is **not inherent to web-ifc**. A fresh `ToModel3D()` correctly
assigns slab meshes (`#22492` roof, `#21658` floor finish match candidate/IFC ground truth). The
**on-disk BFAST oracle was stale** — it permuted those meshes across express ids. Regenerating BFAST
from live web-ifc fixes pairing.

Evidence: live bounds `#22492` = (7.966×16.966×0.457), stale BFAST = (5.8×4.38×0.15); live `#21658`
= (5.809×2.23×0.013), stale BFAST = roof slab dims.

## Fix shipped

| File | Change |
|------|--------|
| `WebIfcBfastOracle.cs` | `IsStaleRelativeToLive` + extended `NeedsRegeneration` auto-detects BFAST drift vs live web-ifc; `CompareOnDiskWithLive`, `CompareRoundTrip` |
| `OracleEntityMap.cs` | `OracleTrustedPairing` + `OracleTrustedPairingBuilder` — shape-based remap for stale-cache detection |
| `Tests/Comparison/WpO1Tests.cs` | live-vs-candidate, regen fix, round-trip, stale detection |

**Coordinator action:** run `WebIfcBfastOracle.Generate(duplex)` (or `ScoreQuickComparisonFiles` which
now auto-regens stale oracles) to refresh `data/bfast/webifc/duplex.bfast`. Do not commit regenerated
artifact unless intentional.

## Usage

```csharp
if (WebIfcBfastOracle.IsStaleRelativeToLive(ifcPath))
    WebIfcBfastOracle.Generate(ifcPath);

var pairing = OracleTrustedPairingBuilder.Build(candidate, oracle);
// pairing.IsMisTagged(id) flags stale-cache permutations for scoring exclusion
```

## Tests

```
dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "FullyQualifiedName~WpO1Tests"
```

## Blockers

None. WP-W10 should use fresh oracles (`NeedsRegeneration`) before triaging window gaps.
