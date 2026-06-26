# Technical Debt Log

A single, scannable list of known shortcuts and areas for improvement, so they can be
planned and tackled deliberately instead of all at once.

**How to use this file** (see [`../AGENTS.md`](../AGENTS.md) §6):

- When you take a shortcut or spot an improvement that is out of scope, add a `// TODO:`
  marker in the code **and** an entry here.
- Each entry: where (file/area), what, and why it matters. Keep it specific and actionable.
- Remove an entry when the debt is paid off.

This log focuses on **high-level, cross-cutting debt** in supported `src/` and `ext/`
libraries. `toolchain/` Plato experiments are not tracked here (unsupported per README).

---

## Open items

Grouped by kind so the heaviest items don't get lost among small cleanups. Within each
group, roughly highest-impact first.

### Correctness (wrong or missing behavior that can bite a caller)

| Area / file | What | Why it matters |
| --- | --- | --- |
| `src/Ara3D.Studio.API/FlowObject.cs` | `WithNewPresentation(presentation)` ignores its argument and re-passes the existing `Presentation` | `WithNewRenderSettings` and `WithNewMaterial` both delegate to it, so setting a material/render settings silently does nothing — this is a live bug, not just debt |
| `src/Ara3D.Studio.API/FlowObject.cs` | `Transform` throws `NotImplementedException` | Studio modifier / flow-graph pipeline cannot apply transforms to a `FlowObject` end-to-end |
| `src/Ara3D.Studio.API/FlowObject.cs` | Presentation updates may leave stale attributes when modifiers change | Flow-graph objects can carry invalid attribute state after a presentation swap |
| `src/Ara3D.Geometry/TransformableExtensions.cs` | `AxisAngle` / `Rotate(AxisAngle)` path flagged "might be broken" (the matrix-based `RotateX/Y/Z` look fine) | Any transform chain that goes through `AxisAngle` may be wrong; needs a test to confirm or fix |
| `src/Ara3D.IO.GltfExporter/GltfBuilder.cs` | Assumes all instances sharing a mesh share one material | Drops material variation on instanced meshes in glTF output (worked-around glTF limitation, but currently lossy) |
| `src/Ara3D.IO.PLY/PlyImporter.cs` | `ToMesh` reads positions only; normals / colors / UV are dropped | Colored or textured PLY files lose data silently |
| `src/Ara3D.BimOpenSchema/BimObjectModelExtensions.cs` | `GetDistinctLevels` dedups with a hard-coded `0.001` elevation epsilon | Can mis-classify nearly-coincident floors or split true duplicates |
| `src/Ara3D.Utils/ZipUtil.cs` | `CreateEntryFromText` reported to produce sporadic zip-creation failures (author note) | Unreliable archive writes; needs reproduction + a test before relying on it |
| `src/Ara3D.IO.BFAST/BFast.cs` | `CheckAlignment` returns early when `stream.Position == stream.Length` (open question: bail vs skip) | Alignment checks on actively-writing streams are silently skipped |
| `src/Ara3D.Geometry/IsotropicRemesher.cs` | Face-key dedup and half-edge helper incomplete (`GetUndirectedFaceKey` should move to topology) | Vertex-collapse remeshing may miss duplicate faces or mishandle shared topology |

### Architecture & API shape (works, but hard to evolve)

| Area / file | What | Why it matters |
| --- | --- | --- |
| `src/Ara3D.Geometry/GeometryUtil.cs` | ~1,360-line catch-all for vectors, transforms, mesh helpers, tolerances | Hard to navigate/refactor; many functions belong in dedicated math/topology modules (inline `// TODO:` at line 19) |
| `src/Ara3D.Domo/Repository.cs` | `SetModelValues` updates one-by-one — no bulk notification, no rollback, no functional update | Slow and non-atomic bulk edits; partial failure leaves the repository inconsistent |
| `src/Ara3D.Domo/Model.cs` | `SetPropertyValue` writes read-only auto-props via the `<name>k__BackingField` reflection trick | Relies on an undocumented compiler naming convention; brittle and slow |
| `src/Ara3D.BimOpenSchema/BimGeometryExtensions.cs` | `ToBimGeometry` copies columns through `IDataSet` helpers instead of reading Parquet columns directly | Extra allocations and indirection on large BIM geometry loads |
| `src/Ara3D.BimOpenSchema/BimDataBuilder.cs` | `Geometry` is a mutable property on the general builder, not a dedicated `BimGeometryBuilder` | Awkward API; easy to misuse when building BOS documents |
| `src/Ara3D.Models/Model3DExtensions.cs` | Only a colored-mesh path; no separate non-colored fast path; copies buffers instead of reusing them; some helpers may belong in geometry extensions | Unnecessary work and allocations when building render models from large meshes |
| `src/Ara3D.Logging/ILogger.cs` | `Create(category)` defaults to `DebugWriter` instead of inheriting the parent logger's writer | Child loggers lose custom writers; hard to route logs consistently in pipelines |
| `src/Ara3D.Utils/PathUtil.cs` | Mixed string and `FilePath`/`DirectoryPath` API (class-level note to finish the migration) | Easy to misuse paths at call sites; inconsistent conventions across the SDK |
| `src/Ara3D.Utils/ProfilingUtil.cs` | Direct `Console` references in profiling helpers | Profiling output cannot be redirected in headless or CI runs |
| `src/Ara3D.Logging/Job.cs` | Job / chained-progress API is commented out and unfinished | Progress reporting across multi-step pipelines stays ad hoc |
| `src/Ara3D.IO.SharpGLTF/` | Vendored SharpGLTF fork with scattered validation / extension TODOs | Drift from upstream; glTF edge cases (extensions, external images, animation pointer) may be under-validated |

### Packaging, layering & cleanup (low individual cost, but they add up)

| Area / file | What | Why it matters |
| --- | --- | --- |
| `src/Ara3D.ScriptService/` | Legacy Roslyn scripting service, marked to move to Bowerbird, **yet referenced by the `Ara3D.SDK` meta-package** | Ships to consumers while documented as "do not depend on" — contradictory public surface |
| `src/Ara3D.BimOpenSchema` packaging | Core BOS not in the meta-package; its IO lives in `ext/` | Consumers must discover and wire up projects manually; split increases release friction |
| `ext/Ara3D.BimOpenSchema.IO/BosBfastSerializer.cs` | Serializer helpers sit in the IO project with a "move this somewhere" note | Blurs the line between the core BOS model and serialization utilities |
| `src/Ara3D.Memory/` (`Ara3D.MemoryMappedFiles` namespace) | MMF helpers merged in from a deleted project but keep the old `Ara3D.MemoryMappedFiles` namespace | Callers `using Ara3D.MemoryMappedFiles;` to get types from `Ara3D.Memory` — confusing after consolidation |
| `src/Ara3D.IO.StepParser/StepGraph.cs`, `src/Ara3D.PropKit/PropAccessor.cs` | Whole types commented out with "delete" TODOs | Dead-code noise; decide to delete or restore |
| `src/Ara3D.Geometry/MeshFeatures_Helpers.cs` | ~12-point future-features roadmap (caching, units, BIM heuristics, …) embedded in a comment block | Not a shortcut — a plan living in source; move to an issue/this log or trim, so it's tracked rather than buried |
| `deprecated/` | Former `Ara3D.Geometry`, PropKit WIP, graphics experiments — not built | Repo clutter; risk of copying stale patterns back into active code |
| `ext/Ara3D.IfcLoader/IfcEntityResolver.cs` | Creates an `IfcEntity` for every STEP entity without filtering | Memory and parse cost on large IFC files |
| `ext/Ara3D.Bowerbird.Revit2025/` | Hard-coded to Revit 2025; upgrade path noted in TODO | Each Revit year needs a sibling project or a version-abstraction layer |
| `ext/Ara3D.Utils.Wpf/ObservablePair.cs` | WPF-specific helper that "should move to shared utilities" | Minor; duplication risk if similar binding helpers are needed elsewhere |
| `tests/Ara3D.BimOpenSchema.Tests/GltfMaterialFactory.cs` | glTF → `Models.Material` conversion lives in test code with a "move to models" note | Duplicated or lost if tests change; belongs in production code if the mapping is real |

---

## Resolved

Move items here (or delete them) once addressed, with a short note on the resolution.
