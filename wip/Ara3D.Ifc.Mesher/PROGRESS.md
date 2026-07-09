# IfcMesher → IfcLoader IModel3D Parity Progress

Measurement-driven parity work against web-ifc `ToModel3D()` oracle. Triangle body geometry only.

**Scorecard test:** `dotnet test ara3d-sdk/tests/Ara3D.IfcMeshingComparison --filter "Category=IfcMesherScore"`

**Machine-readable output:** `ara3d-sdk/tests/Ara3D.IfcMeshingComparison/data/reports/scorecard.json`

---

## Scorecard (last run: 2026-07-08, WP-V duplex surface-model instancing)

| File | Score | Inst | Mesh | EntityInst | EntityBBox | MeshBBox | Shape | Merged |
|------|------:|-----:|-----:|-----------:|-----------:|---------:|------:|-------:|
| IfcOpenHouse_IFC4.ifc | 0.906 | 1.000 | 0.684 | 1.000 | 1.000 | 0.937 | 0.916 | 0.713 |
| example.ifc | 0.910 | 1.000 | 0.889 | 1.000 | 0.994 | 0.969 | 0.819 | 0.683 |
| steelplates.ifc | 0.862 | 1.000 | 0.857 | 1.000 | 1.000 | 0.971 | 0.696 | 0.496 |
| AC20-FZK-Haus.ifc | 0.898 | 1.000 | 0.968 | 1.000 | 0.938 | 0.732 | 0.940 | 0.664 |
| 171210AISC_Sculpture_brep.ifc | 0.920 | 1.000 | 0.828 | 1.000 | 1.000 | 0.966 | 0.934 | 0.673 |
| FM_ARC_DigitalHub.ifc | 0.760 | 0.909 | 0.735 | 0.960 | 0.948 | 0.349 | 0.837 | 0.378 |
| duplex.ifc | 0.843 | 0.957 | 0.929 | 0.984 | 0.866 | 0.591 | 0.828 | 0.681 |

_Metric columns are per-metric scores in [0,1]. Quick-file counts from `Category=IfcMesherScore`; **AC20**, **sculpture**, **DigitalHub** from stretch tests; **duplex** from `ScoreDuplexStretch` (**713/682 inst**, parity **0.843**)._

**WP-V-duplex-assembly (round 1):** Diagnosis: duplex **246 missing inst** were count deltas on shared entities — **IFCRAILING** 126 (2 products, 64 oracle vs 1 cand each), **IFCFURNISHINGELEMENT** 115 (37 products, 3–5 vs 1), **IFCWINDOW** 4 (2 products, 3 vs 1). Oracle-only product count 0. Root cause: `GeometryPartCollector` called `TryBuild` on `IFCFACEBASEDSURFACEMODEL`, which merged all `FbsmFaces` (`IFCCONNECTEDFACESET` children) into one mesh → one instance per product; web-ifc emits one instance per connected face set (e.g. railing #12181 has 64 face sets). Fix: `CollectFaceBasedSurfaceModel` iterates `FbsmFaces` and emits one `CollectedPart` per element via `Brep.BuildFaceBasedSurfaceElement`; golden `FaceBasedSurfaceModel_EmitsInstancePerConnectedFaceSet`. Harness: `ScoreDuplexStretch` oracle-map diagnosis; `OracleEntityMap` skips invalid oracle `MeshIndex`. **Duplex 713/682 inst**, **404/435 meshes**, parity **0.843** (was 436/682 inst, 240/435 meshes, 0.730); entity inst Jaccard **0.996**, entity bbox **191/235**. Remaining gap: **#22475 IFCROOF** (1 inst, `IFCPOLYGONALBOUNDEDHALFSPACE` boolean); **33 extra** candidate instances (likely duplicate face-set emission on non-duplex FBSM layouts). Quick files unchanged.

**WP-P-advanced-brep (round 1):** Diagnosis: DigitalHub **2309 missing inst** concentrated in `IFCADVANCEDBREP` type maps on `IFCBUILDINGELEMENTPROXY` (e.g. bicycle #135409 → rep #135372 with 33 `IFCADVANCEDBREP` solids). Root cause: `Brep.ReadLoop` only accepted `IFCPOLYLOOP` — DigitalHub advanced faces use `IFCEDGELOOP` + `IFCORIENTEDEDGE` + `IFCEDGECURVE` (BSpline/polyline geometry) on `IFCPLANE` / `IFCCURVEBOUNDEDPLANE` surfaces → bound read threw → face skipped → 0 tris per brep. Fix in `Brep.cs`: tessellate `IFCEDGELOOP` via oriented-edge curve evaluation (`CurveEvaluator.Evaluate3D`); `IFCCURVEBOUNDEDPLANE` basis-plane projection + outer/inner boundary fallback; Newell plane retained for `IFCPLANE` faces (shared-plane box test). Golden: `MeshesAdvancedBrepWithEdgeLoopBounds`, `MeshesCurveBoundedPlaneAdvancedFace`, `DigitalHub_BicycleProxyAdvancedBrep_Builds` (#135371 → 144 tris). Stretch: `ScoreDigitalHubStretch` **3272/3599 inst**, **50801/311811 merged tris**, parity **0.760** (was 1290/3599 inst, 9726/90010 tris, 0.616); entity inst Jaccard **0.958**, entity bbox **695/737**. Remaining gap: non-planar advanced-face surfaces (`IFCSURFACEOFREVOLUTION`, `IFCCYLINDRICALSURFACE`, NURBS) still bounds-only/ignored; merged tri ratio 0.378.

**WP-S-bolt-shank-circle (round 1):** Diagnosis: sculpture **196 missing inst** were all `IFCMECHANICALFASTENER` bolt maps — shank `IFCCIRCLEPROFILEDEF` (r=0.375 in) triangulation failed with `Ear clipping failed: n=32, remaining=32`; hex head extruded OK, collector emitted 1 part. Root cause: `PolygonTriangulator.EarClipTriangulate` uses absolute convex threshold `Eps=1e-6` while inch-scaled shank vertex cross products are ~1e-7 (r≈0.0095 m, 32 segments) → no ear found on any vertex. Fix: `PolygonWithHoles.TryTriangulateConvexFan` — scale-relative convexity test + fan triangulation for hole-free convex rings before ear-clip fallback. Golden: `SmallInchBoltShankCircleProfile_Triangulates`, `MeshesSmallInchBoltShankCircleExtrusion`. **Sculpture 546/546 inst, 120/145 meshes, 5360/5012 part tris, parity 0.920** (was 350/546 inst, 111/145 meshes, 4244 tris, 0.751); entity inst Jaccard **1.0**, entity bbox **350/350**. Remaining gap: mesh dedup (120 vs 145 oracle meshes), merged tri ratio (coarser bolt caps).

**WP-R-mapped-multibrep (round 1):** Diagnosis: sculpture **146 oracle-only** was **not** mapped-item instancing — `CollectMappedItem` + per-item `CollectParts` already emitted parts correctly. Root cause: `Units.ResolveLengthScaleToMeters` returned **0** on `IFCCONVERSIONBASEDUNIT` inch factors (`IFCMEASUREWITHUNIT(IFCLENGTHMEASURE(25.4),…)`); `ReadNumber` ignored typed measures → all inch brep coords scaled to 0 → `IFCFACETEDBREP` built **0 tris** for every mapped brep type map. Fix: `Units.ReadMeasureWithUnitScale` unwraps `IFCLENGTHMEASURE` via `AsSimpleEntity` and applies component-unit scale (mm→m). Golden: `InchConversionUnit_ResolvesLengthScaleToMeters`, `MappedFacetedBrep_InchUnits_EmitsInstance`, `AiscSculpture_MappedBrepCoverage`. **Sculpture 350/546 inst, 111/145 meshes, 4244/5012 tris, parity 0.751** (was 3 meshes / 26 tris); **oracle-only products 0** (was 146). Remaining **196 inst**: `IFCMECHANICALFASTENER` bolt maps (shank `IFCCIRCLEPROFILEDEF` ear-clip failure, hex head OK) — not collector merge. **AC20 stretch unchanged: 0.898, 252/252 inst.**

**WP-Q-open-profile (round 1):** Entity-level ribbon extrusion verified at scale: **duplex 184/184**, **Office_A 1243/1243**, **dental_clinic 3156/3156** `IFCSURFACEOFLINEAREXTRUSION` build via `GeometryDispatcher.TryBuild`. Duplex/Office_A open profiles are all `IFCPOLYLINE`; trimmed-arc open profiles mesh in dental_clinic (#161277, 14 tris). Fixes: `CurveEvaluator.SanitizeOpenPathPoints` (no false closure on open paths); composite open curves use open-path sanitize; `BuildSurfaceOfLinearExtrusion` prefers 2D swept-curve evaluation before 3D. Backlog: `IFCARBITRARYOPENPROFILEDEF` + `IFCSURFACEOFLINEAREXTRUSION` → **Supported**. Golden: `MeshesDuplexStyleOpenProfileRibbon`, `Duplex_OpenProfileRibbon_WithAxisPlacement`, `OpenProfile_CompositeCurveRibbon_Builds`, scale tests `Duplex_AllSurfaceOfLinearExtrusions_BuildRibbons` / `OfficeA_AllSurfaceOfLinearExtrusions_BuildRibbons`. **duplex parity 0.730 unchanged** (436/682 inst): ribbons live in `IFCCONNECTIONSURFACEGEOMETRY` only — not in any `Body` `ShapeRepresentation`; instance gap is mapped-item / brep / surface-model assembly, not ribbon builder failure.

**WP-O-parity-gaps (round 1):** Four remaining parity gaps closed. **OpenHouse #268/#281:** oblique nested gable half-spaces used planar keep-side (`!agreement`) → kept roof wedge only; fix uses agreement directly when `|normal.x| >= |normal.z|`. **steelplates #633/#1193/#1385:** half-space plane coords encode extrusion-end trim distance (778 ≈ depth−1500) but plane normal was profile-axis — ineffective clip; fix resolves extrusion-aligned clip plane when normal ⊥ extrusion axis. **AC20 +33 inst:** 17 `IFCOPENINGELEMENT` products (2 inst each) emitted geometry oracle ignores; excluded from `IsProduct`. **duplex compare crash:** oracle BFAST had sparse mesh indices — `CompareMergedMesh` now skips invalid `MeshIndex` via `ToMergedMesh`. Golden: `MeshesExtrusionEndBooleanClipAlongDepth`, `MeshesNestedObliqueGableBooleanClips`. **OpenHouse entity bbox 35/35**, **steelplates 14/14**, **AC20 252/252 inst**, parity **0.906/0.862/0.898**.

**WP-N-example-shs (round 1):** Oracle map: remaining **18 oracle-only** on example.ifc were all **100×6.0 SHS** (`IFCARBITRARYPROFILEDEFWITHVOIDS` with rounded composite outer/inner curves). Root cause: outer **40** vs inner **36** vertices → congruent-ring triangulation skipped, hole-stitch ear-clip failed (`n=77, remaining=43`). Fix: `TryTriangulateOffsetRing` resamples inner ring to outer count + arc-length correspondence; `CleanRing` on void inner curves in `BuildArbitraryWithVoids`. Golden: `ExampleIfc_ShsProfileWithVoids_Builds` (#7649/#7935). **example.ifc 120/120 inst**, **116/116 entity Jaccard**, parity **0.910** (was 0.839), merged tris **12392/15046** (was 6846).

**WP-J-boolean-openhouse:** Oracle map: steelplates **#633, #1193, #1385** (all `IFCBOOLEANCLIPPINGRESULT` W-shape/HSS braces) and OpenHouse **#268, #281** (mapped `IFCWALLSTANDARDCASE` with nested half-space clips) had entity bbox misses. Root cause in `Booleans.ClipByHalfSpace`: (1) DIFFERENCE kept the half-space solid instead of its complement — fixed via `keepPositiveSide = !agreement`; (2) clipped meshes seeded output `Points` from the full pre-clip list, so removed vertices still inflated bounds/volume metrics. Fix: empty output point list + agreement-aware keep-side in `ClipTriangle`. Golden: `MeshesBooleanClippingResultWithPlanarHalfSpace`, `MeshesBooleanClippingResultKeepsOppositeHalfSpaceWhenAgreementFalse`, `BooleanTests.BooleanClippingResult_HalfSpace_KeepsComplementForAgreementTrue`. **OpenHouse entity bbox 33/35** (unchanged; #268/#281 oblique nested clips in map #257); **steelplates entity bbox 11/14** (was 10/11 on 11 shared ids — shared set now 14 with 3 boolean-beam clip placement mismatches remaining). OpenHouse meshBBox **0.907** (was 0.168); steelplates parity **0.787** (was 0.633).

**WP-I-example-steel (round 1):** L-angle composite profiles (#3405/#3396, #3576/#3567, …) with `SameSense=.F.` trimmed fillet arcs (270°→0° PARAMETER). Root cause in `CurveEvaluator.EvaluateCompositeSegment2D`: inverting trim *sense* on `SameSense=.F.` segments left arc sampled in wrong direction (chord through fillet hub → 31-pt self-intersecting ring). Fix: evaluate trimmed arcs normally, **reverse point order** when `SameSense=.F.`; tolerance-based continuous joins; pop polyline vertex when it equals next arc circle center; scale-aware `CompositeJoinToleranceSquared`; `ConicArcSampleCount` `perQuadrant = Max(2, segments/4)`. Golden: `ExampleLAngleColumnProfile_IsSimplePolygon`, `MeshesSteelSectionProfileWithTrimmedFilletArc`, `ExampleIfc_SteelSectionExtrusions_Build` (5/5). **example.ifc 102/120 inst**, **97/98 entity bbox**, **72/73 mesh bbox**, **shape 0.921**, parity **0.839**, oracle-only **18** (was 48).

**WP-L-mesh-shape (round 3):** Oracle map: entity bbox strong (OpenHouse 33/35, steelplates 10/11) but mesh bbox **0/16** and **0/11** — meshes were paired by sorted local `bounds.Min` while candidate/oracle store geometry in different part-local vs representation-local frames (transform on instance, not mesh). Root cause: **comparer pairing/metric**, not `ModelAssembler` dedup. Fixes in `ModelComparer`: entity co-occurrence mesh pairing; world-space bounds via shared-entity instance transform; centroid-canonicalized shape fingerprint; `CompareOrientedExtents` uses `1.0 + relTolerance` as max ratio. **OpenHouse meshBBox 0.907 (12/13), shape 0.926** (was 0.168/0.502); **steelplates meshBBox 0.868 (8/9), shape 0.696** (was 0.157/0.360). Parity **0.886** / **0.755** (was 0.752 / 0.648).

**WP-K-example-push (round 2):** Oracle map diagnosis: **48 oracle-only** products on example.ifc — **IFCBEAM 33**, **IFCCOLUMN 14**, **IFCSPACE 1** (#9989, hollow SHS profile). Walls/slabs/faceted breps already in shared 68 entities (not oracle-only). Non-steel instancing gap: **#12881** proxy mapped rep has 3 extruded solids but collector merged to 1 instance. Fixes: `GeometryPartCollector` walks mapped reps per-part (not merged cache); `IsBodyRepresentation` accepts identifier **or** type (`Clipping`, `MappedRepresentation`, …); `GeometryDispatcher.TryGetMappedItemTransform` shared helper. Golden: `MappedItem_MultiSolidRepresentation_EmitsMultipleInstances`; harness `Example_PrintOracleOnlyGaps`. **Expected ~72/120 inst** (+2 from #12881); scorecard re-run blocked by parallel build locks — re-run `Category=IfcMesherScore` locally. **90+ inst** still needs WP-I steel profiles (remaining 47 oracle-only).

**Round 3 (WP-M-milestones):** Refreshed T1 scorecard; AC20 stretch re-run **229/252 inst**, **17996/21770 merged tris**, parity **0.649** (was 183/252, 0.517). Duplex baseline attempted via `ScoreDuplexStretch` — model builds meshes but `ModelComparer` aborts on invalid `InstanceStruct.MeshIndex` (not a quick fix). M3/M4 met on quick files; M2 blocked on example entity coverage; M5 strong on AC20.

**WP-G-example (round 2):** `StepTokenizer` signed-number lexing + `CurveEvaluator` composite sanitize. **70/120 inst**, **3630/15046 merged tris** (4.1× oracle), entity bbox **68/68** on shared ids. Remaining: ~48 oracle-only entities (UC/PFC composite steel).

**WP-F-openhouse (round 1):** Polyline ring cleanup, `Facetation`/`SurfaceModel` rep traversal, `IFCOPENSHELL` in FBSM. **42/38 inst**, **1060/1098 merged tris**, parity **0.752**.

**WP-E-placement (round 1):** L-profile centering (`BuildLShape` on `Position`); transforms were already correct. steelplates entity bbox **10/11**; OpenHouse **33/35**.

**WP-H-stretch (round 2 continuation):** Diagnosis on `ScoreAc20FzkHausStretch`: **IFCMEMBER 42/42 complete** (Sparren: `IFCREPRESENTATIONMAP` → single `IFCFACETEDBREP`, 12 tris each). Historical ~69 missing instances were mostly brep failures (round 1 `Brep.cs` dedupe/resilience) plus mapped-rep sub-solid merging. `GeometryPartCollector.CollectMappedItem` now recurses into mapped `Body` items (one instance per brep/solid) via `TryGetMappedItemTransform`; golden `MappedItemWithMultipleBodyItems_InstancesPerPart`. Remaining **23 inst gap**: **IFCWINDOW** (9 products, oracle 7 vs cand 1 — 7 breps in window type map `#22996`) and **IFCDOOR** (2 products, oracle 2 vs cand 1). Not profile/placement — assembly instancing + multi-brep window solids.

**WP-H-stretch (round 2):** AC20 brep dedupe + `IsProduct` relation filter + IFCMEMBER recovery. **229/252 inst** (was 55), **17996/21770 tris** (was 5428), entity bbox **82/90** shared ids.

**duplex.ifc (M5 probe):** Oracle **682 inst / 27342 tris**. Candidate partial build; compare crashes on out-of-range mesh index. Prior full-catalog run had **0 inst** (same root cause). Top gap: assembly integrity / open-profile extrusions (`IFCARBITRARYOPENPROFILEDEF` ×184).

### Milestones

| Milestone | Target | Status |
|-----------|--------|--------|
| M1 Measurement | scorecard + oracle maps + PROGRESS.md | **done** |
| M2 Structure | instance + entity histogram within 2× on quick files | **done** — inst ratio within 2× on all 3 quick files; example **120/120 inst**, entity Jaccard **1.0** |
| M3 Coverage | merged tri count within 5× on IfcOpenHouse + steelplates | **done** — OpenHouse 1060/1098 (1.0×); steelplates 356/1428 (4.0×) |
| M4 Shape | per-entity bbox match > 70% shared ids on quick files | **done** — OpenHouse 33/35 (94%), example 97/98 (99%), steelplates 11/14 (79%) |
| M5 Stretch | progress on one large file (AC20-FZK-Haus or duplex) | **in progress** — AC20 **252/252 inst**, parity **0.898**; duplex **713/682 inst**, parity **0.843** (was 436/682, 0.730) |

---

## Work packages

| ID | Status | Owner | Blocked by | ROI | Scope |
|----|--------|-------|------------|-----|-------|
| WP-A-comparer | done | — | — | high | `ModelComparer`, `ScorecardTests`, `PROGRESS.md` |
| WP-B-oracle-map | done | — | — | high | `OracleEntityMap`, BFAST regen for quick files |
| WP-C-assembly | done | — | — | high | `ModelAssembler` per-part instancing, mapped-item transform on instance |
| WP-D-profiles | done | — | WP-C | med | `ProfileBuilder.cs` composite/derived/trapezium golden tests |
| WP-D-brep | done | — | WP-C | med | `Brep.cs` |
| WP-D-swept | done | — | WP-C | med | `SweptSolids.cs` |
| WP-D-boolean | done | — | WP-C | med | `Booleans.cs` |
| WP-D-tess | done | — | WP-C | med | `Tessellated.cs` |
| WP-D-mapped | done | — | WP-C | high | `GeometryDispatcher.TryBuildMappedItemLocal` instancing |
| WP-G-example | done | — | — | high | Round 2: signed STEP coords + composite sanitize; example 70/120 inst |
| WP-J-boolean-openhouse | done | — | WP-D-boolean | high | Half-space DIFFERENCE keep-side + orphan-vertex bbox fix; OpenHouse 33/35, steelplates 11/14 entity bbox |
| WP-E-placement | done | — | WP-C | high | Round 1: L-profile centering; steelplates entity bbox 10/11 |
| WP-F-openhouse | done | — | WP-C | high | Round 1: polyline rings, Facetation rep, FBSM open shells; OpenHouse 42/38 inst |
| WP-H-stretch | in progress | — | — | high | Round 2: AC20 parity 0.898; 252/252 inst; IFCMEMBER 42/42 |
| WP-O-parity-gaps | done | — | WP-D-boolean, WP-C | high | Oblique gable + extrusion-end clips; opening filter; comparer mesh-index guard |
| WP-N-example-shs | done | — | WP-I | high | Round 1: SHS void offset-ring triangulation; example 120/120 inst |
| WP-I-example-steel | done | — | WP-G | high | Round 1: SameSense.F fillet arc reversal; example 102/120 inst, 97/98 entity bbox |
| WP-K-example-push | done | — | WP-G | high | Round 2: mapped multi-solid instancing; 102/120 inst with WP-I |
| WP-L-mesh-shape | done | — | WP-C | med | Entity-guided mesh pairing + world-space mesh metrics; OpenHouse meshBBox 0.907, shape 0.926 |
| WP-M-milestones | done | — | — | high | Round 3: scorecard refresh, milestone assessment, duplex probe, `ScoreDuplexStretch` |
| WP-Q-open-profile | done | — | — | high | Round 1: entity ribbon 184/184 duplex, 1243/1243 Office_A; open-path sanitize; duplex parity unchanged (connection-geometry only) |
| WP-R-mapped-multibrep | done | — | WP-Q | high | Round 1: inch `IFCLENGTHMEASURE` unit scale; sculpture 350/546 inst, parity 0.751 (was 3/26 tris) |
| WP-S-bolt-shank-circle | done | — | WP-R | high | Round 1: scale-relative convex fan for small `IFCCIRCLEPROFILEDEF`; sculpture 546/546 inst, parity 0.920 |
| WP-P-advanced-brep | done | — | WP-S | high | Round 1: IFCEDGELOOP curve bounds + IFCCURVEBOUNDEDPLANE; DigitalHub 3272/3599 inst, parity 0.760 (was 0.616) |
| WP-V-duplex-assembly | done | — | WP-Q | high | Round 1: FBSM per connected-face-set instancing; duplex 713/682 inst, parity 0.843 (was 436/682, 0.730) |

---

## Agent work-package rules

1. **One WP per session** — touch one geometry module or one harness file; no cross-WP edits in the same session.
2. **End every WP with:**
   - (1) micro/golden test if geometry work (`GoldenMeshTests` / `ModelBuildTests`)
   - (2) scorecard delta on **one** target quick file (`Category=IfcMesherScore`)
   - (3) update the scorecard table row in this file
3. **Pick work** marked `planned` with no blockers.
4. **Do not** chase full-catalog green after every change; use tiered tests (see plan).
5. **Do not** refactor unrelated code; keep diffs focused.

### Test tiers

| Tier | Filter / command | When |
|------|------------------|------|
| T0 Micro | `GoldenMeshTests`, `ModelBuildTests` | Every geometry change |
| T1 Quick score | `Category=IfcMesherScore` | End of each WP session |
| T2 Deep | `ModelComparer` + `oracle_maps` | Assembly changes, regressions |
| T3 Full | `RunFullComparison` (Explicit) | Milestone only |
