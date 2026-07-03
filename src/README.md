# Ara 3D SDK — `src/`

This folder contains the core libraries of the Ara 3D SDK.

These projects are, for the most part, dependency-free, cross-platform, and intended for
consumption from .NET 8. Additional projects which are executables, or have extra dependencies,
can be found in [`../ext`](../ext) and [`../wip`](../wip).

Install everything via the [Ara3D.SDK](https://www.nuget.org/packages/Ara3D.SDK) meta-package,
or reference individual projects from this folder.

---

## Meta-package

| Project | Description |
| --- | --- |
| [Ara3D.SDK](Ara3D.SDK) | Bundles most libraries below into a single NuGet package |
| [Ara3D.SDK.Geometry](Ara3D.SDK.Geometry) | Convenience bundle for geometry, models, memory, and SIMD |
| [Ara3D.SDK.IO](Ara3D.SDK.IO) | Convenience bundle for BFAST, G3D, VIM, PLY, STEP, GeoJSON, and glTF |
| [Ara3D.SDK.BIM](Ara3D.SDK.BIM) | Convenience bundle for BIM Open Schema model and IO packages |
| [Ara3D.SDK.Studio](Ara3D.SDK.Studio) | Convenience bundle for Studio API, scripting, services, PropKit, and Roslyn helpers |

---

## Core geometry and models

| Project | Description |
| --- | --- |
| [Ara3D.Geometry](Ara3D.Geometry) | Meshes, topology, SDFs, voxels, spatial queries, exporters |
| [Ara3D.Models](Ara3D.Models) | Scene models, instances, render buffers |
| [Ara3D.F8](Ara3D.F8) | SIMD (`AVX`) wrappers for 8-wide float math |
| [Ara3D.Memory](Ara3D.Memory) | Aligned buffers, slices, memory-mapped file views |
| [Ara3D.Collections](Ara3D.Collections) | Read-only list views, sparse matrices, LINQ helpers |
| [Ara3D.DataTable](Ara3D.DataTable) | Columnar in-memory data interfaces |

Shared math types used by Geometry are generated in [`Plato.Generated`](Plato.Generated) and
[`Plato.Intrinsics`](Plato.Intrinsics) (imported into `Ara3D.Geometry`, not standalone packages).

---

## I/O formats

| Project | Description |
| --- | --- |
| [Ara3D.IO.BFAST](Ara3D.IO.BFAST) | Binary Format for Array Serialization and Transmission |
| [Ara3D.IO.G3D](Ara3D.IO.G3D) | G3D geometry exchange format (BFAST container) |
| [Ara3D.IO.VIM](Ara3D.IO.VIM) | VIM BIM binary format |
| [Ara3D.IO.PLY](Ara3D.IO.PLY) | PLY mesh import/export |
| [Ara3D.IO.StepParser](Ara3D.IO.StepParser) | ISO STEP file tokenizer and parser |
| [Ara3D.IO.GeoJson](Ara3D.IO.GeoJson) | GeoJSON and IMDF indoor mapping |
| [Ara3D.IO.GltfExporter](Ara3D.IO.GltfExporter) | glTF/GLB export |
| [Ara3D.IO.SharpGLTF](Ara3D.IO.SharpGLTF) | glTF/GLB import and manipulation (fork of SharpGLTF) |

---

## BIM

| Project | Description |
| --- | --- |
| [Ara3D.BimOpenSchema](Ara3D.BimOpenSchema) | BIM Open Schema object model (not in meta-package) |

Serialization for BOS lives in [`../ext/Ara3D.BimOpenSchema.IO`](../ext/Ara3D.BimOpenSchema.IO).

---

## Application architecture

| Project | Description |
| --- | --- |
| [Ara3D.Events](Ara3D.Events) | Thread-safe event bus |
| [Ara3D.Services](Ara3D.Services) | Service registration and event bus host |
| [Ara3D.Logging](Ara3D.Logging) | Logging, progress, and job management |
| [Ara3D.WorkItems](Ara3D.WorkItems) | Background work-item queues |
| [Ara3D.PropKit](Ara3D.PropKit) | Runtime property descriptors for UI binding |

Domo-backed model/repository helpers live in [`../deprecated/wip/Ara3D.Domo`](../deprecated/wip/Ara3D.Domo) (WIP, not shipped).

---

## Studio and scripting

| Project | Description |
| --- | --- |
| [Ara3D.Studio.API](Ara3D.Studio.API) | Plug-in and flow-graph API for Ara 3D Studio |
| [Ara3D.ScriptService](Ara3D.ScriptService) | Legacy Roslyn scripting service (Bowerbird only) |
| [Ara3D.Utils.Roslyn](Ara3D.Utils.Roslyn) | Roslyn compilation helpers |

---

## Utilities

| Project | Description |
| --- | --- |
| [Ara3D.Utils](Ara3D.Utils) | General-purpose helpers (paths, JSON, threading, web, …) |

---

## License

MIT — see [LICENSE](../LICENSE).
