# Prioritized TODO List

A repo-wide backlog ordered by impact. Use this to decide **what to work on next**.

- **Detail log:** [`TECHNICAL_DEBT.md`](TECHNICAL_DEBT.md) — what each shortcut is and why it matters.
- **Investigations:** [`investigations.md`](investigations.md) — evidence and hypotheses for unresolved problems.
- **Workflow:** [`../AGENTS.md`](../AGENTS.md) — build/test expectations before marking items done.
- **Scope:** `src/` and `ext/` only. `toolchain/` Plato experiments are excluded (unsupported).

When you complete an item: check it off here, remove the matching row from `TECHNICAL_DEBT.md`
(if any), and delete the inline `// TODO:` marker in code.

Use short IDs like `IFC-STR-001` when an item needs to be linked across TODOs,
technical debt, tests, and investigation notes.

**Sort rule:** priority tier first (P0 → P4), then effort as a tie-breaker within a tier when
time is limited.

---

## Priority key

| Tier | Meaning | When to tackle |
| --- | --- | --- |
| **P0** | Live bug or silent wrong output | Next — blocks trust in core APIs |
| **Now** | In-flight work — finish before starting new P2+ items | Immediately |
| **P1** | Correctness gap or incomplete feature in active paths | Soon — after P0 |
| **P2** | Performance / scale on real files | When P0/P1 are green |
| **P3** | Architecture / API shape — works but hard to evolve | Deliberate refactors |
| **P4** | Packaging, dead code, minor cleanup | Batch when convenient |

## Effort key

Rough sizing for a solo maintainer (includes test + `test.bat` scope for the area).

| Effort | Typical scope |
| --- | --- |
| **XS** | Minutes — one-line or trivial change, no design |
| **S** | ~1–2 hours — localized fix + test |
| **M** | Half day — multi-file change or investigation required |
| **L** | Several days — non-trivial design, many call sites, or hard debugging |
| **XL** | Multi-day / ongoing — large refactor or open-ended maintenance |

Estimates are planning hints, not commitments. Update when an item splits or scope becomes clear.

---

## Suggested next 5

| Order | Item | Effort | Why |
| --- | --- | --- | --- |
| 1 | P0 #1 `FlowObject.WithNewPresentation` | XS | One-line fix; unblocks Studio modifiers |
| 2 | P0 #3 `AxisAngle` test + fix | M | Confirm or kill a geometry correctness risk |
| 3 | P0 #4 `FlowObject.Transform` | L | Unblocks remaining Studio pipeline |
| 4 | P2 #17 `IfcEntityResolver` filtering | M | High payoff on real IFC files |
| 5 | P1 #6 glTF exporter per-instance materials | M | Fixes lossy glTF export |

---

## P0 — Fix first (live bugs)

- [ ] **1. [XS] `FlowObject.WithNewPresentation` ignores its argument** — `src/Ara3D.Studio.API/FlowObject.cs` lines 32–33 pass `Presentation` instead of `presentation`, so `WithNewMaterial` / `WithNewRenderSettings` do nothing.
- [ ] **3. [M] `AxisAngle` / `Rotate(AxisAngle)` may be wrong** — `src/Ara3D.Geometry/TransformableExtensions.cs` lines 23–37. Add a focused geometry test first, then fix or remove the path.
- [ ] **4. [L] `FlowObject.Transform` throws `NotImplementedException`** — `src/Ara3D.Studio.API/FlowObject.cs` lines 50–55. Blocks Studio modifier pipeline end-to-end.

---

## Now — In-flight work

None currently. (IFC-relations refactor landed — see Resolved.)

---

## P1 — Correctness gaps

- [ ] **5. [M] Presentation swap may leave stale attributes** — `src/Ara3D.Studio.API/FlowObject.cs` line 28.
- [ ] **6. [M] glTF exporter drops per-instance materials** — `src/Ara3D.IO.GltfExporter/GltfBuilder.cs` line 122.
- [ ] **7. [M] PLY import drops normals / colors / UV** — `src/Ara3D.IO.PLY/PlyImporter.cs` line 296.
- [ ] **8. [S] `GetDistinctLevels` elevation epsilon hack** — `src/Ara3D.BimOpenSchema/BimObjectModelExtensions.cs` line 21 (`0.001` hard-coded).
- [ ] **9. [M] `ZipUtil.CreateEntryFromText` sporadic failures** — `src/Ara3D.Utils/ZipUtil.cs` line 87; needs repro test before fix.
- [ ] **10. [S] `BFast.CheckAlignment` skips at stream end** — `src/Ara3D.IO.BFAST/BFast.cs` line 69; open question: bail vs skip.
- [ ] **11. [L] Isotropic remesher topology incomplete** — `src/Ara3D.Geometry/IsotropicRemesher.cs` lines 280–304.
- [ ] **12. [M] Revit geometry computation doubt** — `ext/Ara3D.Bowerbird.RevitSamples/ExtensionsRevit.cs` line 259.
- [ ] **13. [L] Known-issue triangulation bugs** — `tests/Ara3D.SDK.KnownIssues.Tests/PolygonTriangulatorKnownIssueTests.cs` (opt-in via `test.bat knownissues`).

---

## P2 — Performance and scale

- [ ] **17. [M] `IfcEntityResolver` creates `IfcEntity` for every STEP entity** — `ext/Ara3D.IfcLoader/IfcEntityResolver.cs` line 14.
- [ ] **18. [M] `ToBimGeometry` copies via `IDataSet` instead of Parquet columns** — `src/Ara3D.BimOpenSchema/BimGeometryExtensions.cs` line 252.
- [ ] **19. [M] `Model3DExtensions` buffer copies, no non-colored fast path** — `src/Ara3D.Models/Model3DExtensions.cs` lines 25, 69, 292.
- [ ] **20. [M] Revit AST geometry path optimization** — `ext/Ara3D.Bowerbird.RevitSamples/GeometryAbstractSyntaxTree.cs` line 99.

---

## P3 — Architecture and API shape

- [ ] **21. [XL] Split `GeometryUtil.cs` (~1,360 lines)** — `src/Ara3D.Geometry/GeometryUtil.cs` line 19.
- [ ] **22. [M] `BimDataBuilder.Geometry` should be `BimGeometryBuilder`** — `src/Ara3D.BimOpenSchema/BimDataBuilder.cs` line 66.
- [ ] **23. [L] Domo bulk update API** — `src/Ara3D.Domo/Repository.cs` lines 166–168.
- [ ] **24. [M] Domo `SetPropertyValue` reflection on backing fields** — `src/Ara3D.Domo/Model.cs`.
- [ ] **25. [S] `ILogger.Create` should inherit parent writer** — `src/Ara3D.Logging/ILogger.cs` line 56.
- [ ] **26. [L] Finish `PathUtil` `FilePath`/`DirectoryPath` migration** — `src/Ara3D.Utils/PathUtil.cs`.
- [ ] **27. [L] Complete `Job` / chained-progress API** — `src/Ara3D.Logging/Job.cs` line 3.
- [ ] **28. [XL] Vendored SharpGLTF fork maintenance** — scattered TODOs under `src/Ara3D.IO.SharpGLTF/`.
- [ ] **29. [S] `ProfilingUtil` direct `Console` references** — `src/Ara3D.Utils/ProfilingUtil.cs` line 81.
- [ ] **30. [M] PropKit vector descriptor generalization** — `src/Ara3D.PropKit/PropDescriptorVector3.cs` line 30.
- [ ] **31. [S] BOS Browser Family vs Type naming confusion** — `ext/Ara3D.BimOpenSchema.Browser/MainWindow.xaml.cs` line 161.

---

## P4 — Packaging, cleanup, hygiene

- [ ] **32. [M] `ScriptService` in meta-package but marked legacy** — `src/Ara3D.ScriptService/ScriptingService.cs` line 13.
- [ ] **33. [M] BOS core not in meta-package; IO in `ext/`** — see `TECHNICAL_DEBT.md` packaging row.
- [ ] **34. [S] Move `BosBfastSerializer` helpers** — `ext/Ara3D.BimOpenSchema.IO/BosBfastSerializer.cs` line 14.
- [ ] **35. [S] `Ara3D.MemoryMappedFiles` namespace after merge** — `src/Ara3D.Memory/`.
- [ ] **36. [XS] Delete or restore dead types** — `src/Ara3D.IO.StepParser/StepGraph.cs`, `src/Ara3D.PropKit/PropAccessor.cs`.
- [ ] **37. [XS] Move `MeshFeatures_Helpers` roadmap comment into debt log** — `src/Ara3D.Geometry/MeshFeatures_Helpers.cs`.
- [ ] **38. [M] Prune or archive `deprecated/`**.
- [ ] **39. [XL] Revit 2025 hard-coding / multi-version path** — `ext/Ara3D.Bowerbird.Revit2025/BowerbirdRevitApp.cs` line 59.
- [ ] **40. [S] Promote `GltfMaterialFactory` from tests** — `tests/Ara3D.BimOpenSchema.Tests/GltfMaterialFactory.cs` line 80.
- [ ] **41. [S] Minor WPF utility moves** — `ext/Ara3D.Utils.Wpf/ObservablePair.cs`, `WpfHelpers.cs`.
- [ ] **42. [M] Layout importer two-door path limit** — `ext/Ara3D.Bowerbird.RevitSamples/BowerbirdLayoutImporter.cs` line 335.

---

## Resolved

Move completed items here with a one-line note on the resolution, then delete from the
sections above.

- [x] **14. [M] Land IFC relations consolidation** — committed and pushed; `IfcRelations.cs` / `IfcRelationMapping.cs` / `IfcMaterialSelectResolver.cs` wired into `IfcToBosConverter`, old structural-relation files removed.
- [x] **2 / 16. [S] IFC document index hard-coded to `-1`** — accepted as-is: single-document IFC→BOS conversion does not need a per-document index, so `_docIndex = -1` is intentional; no `AddDocument` call required. `// TODO:` marker removed.
- [x] **15. [S] Converter-level relation tests (openings + groups)** — added `ConverterEmitsOpeningRelations` and `ConverterEmitsGroupAndProjectRelations` to `IfcRelationsTests.cs`.
- [x] **43. [S] Consistent IFC string decoding** — `DecodeIfc` now applied to entity display names (`GetEntityLabel`) and string parameter values in `IfcToBosConverter`; covered by `ConverterDecodesEscapedEntityName` / `ConverterDecodesEscapedStringPropertyValue`.
- [x] **44. [S] IfcSpace naming** — `GetEntityLabel` prefers `LongName` for `IfcSpace`, stops surfacing the GlobalId GUID as a name (unset `$`/`*` now treated as empty via `GetStringOrEmpty`); the room number (`Name`) is preserved as an `Ifc:Room:Number` parameter. Covered by `ConverterUsesSpaceLongNameAndKeepsRoomNumber`.
- [x] **45. [M] `IFC-STR-001` schependomlaan string decoding** — closed as false alarm: `\uXXXX` in room JSON output is default `System.Text.Json` ASCII escaping of decoded Unicode (e.g. Dutch `ë`), not missing IFC decode. BOS string values round-trip correctly via `GetValue<string>()`.

<!-- Example:
- [x] **1. [XS] FlowObject.WithNewPresentation** — fixed in commit abc123; passes Studio modifier tests.
-->
