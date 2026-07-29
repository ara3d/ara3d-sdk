# Ara 3D SDK — `src/`

This folder contains the supported SDK libraries: cross-platform foundations, geometry, I/O,
BIM, Studio APIs, and NuGet meta-packages.

Windows-only extensions (WPF, native IFC) live in [`../ext`](../ext). Apps, plug-ins, and
optional integrations live in [`../apps`](../apps), [`../plugins`](../plugins), and
[`../integrations`](../integrations).

---

## Meta-packages

| Package | TFM | Description |
| --- | --- | --- |
| [Ara3D.SDK.Core](Ara3D.SDK.Core) | `net8.0` | Low-level cross-platform foundation (Utils, Logging, Memory, Collections, …) |
| [Ara3D.SDK.Geometry](Ara3D.SDK.Geometry) | `net8.0` | Geometry and modeling stack (Geometry, Models, F8, Memory, Utils) |
| [Ara3D.SDK.IO](Ara3D.SDK.IO) | `net8.0-windows` | File formats, BOS, and IFC conversion |
| [Ara3D.SDK](Ara3D.SDK) | `net8.0-windows` | **All of the above** plus `Ara3D.Studio.API` and `Ara3D.Utils.Wpf` |

Use `Ara3D.SDK.Core` or `Ara3D.SDK.Geometry` when you need a minimal, cross-platform reference.
Use `Ara3D.SDK` on Windows when you want one package for almost any task.

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
| [Ara3D.BimOpenSchema](Ara3D.BimOpenSchema) | BIM Open Schema object model |
| [Ara3D.BimOpenSchema.IO](Ara3D.BimOpenSchema.IO) | Parquet/DuckDB/Excel serialization and IFC import (Windows TFM for IFC) |

---

## Application architecture

| Project | Description |
| --- | --- |
| [Ara3D.Events](Ara3D.Events) | Thread-safe event bus |
| [Ara3D.Logging](Ara3D.Logging) | Logging, progress, and job management |
| [Ara3D.WorkItems](Ara3D.WorkItems) | Background work-item queues |
| [Ara3D.PropKit](Ara3D.PropKit) | Runtime property descriptors for UI binding |

Domo-backed model/repository helpers live in [`../wip/Ara3D.Domo`](../wip/Ara3D.Domo) (WIP, not shipped).

---

## Studio

| Project | Description |
| --- | --- |
| [Ara3D.Flow](Ara3D.Flow) | Host-free evaluation pipeline (`FlowObject`, generators/modifiers, eval). Namespace stays `Ara3D.Studio.API`. Built with Studio; not yet listed in `build/packages.txt` / not a separate NuGet package. |
| [Ara3D.Studio.API](Ara3D.Studio.API) | Host-facing Studio contracts (tools, exporters, host interfaces) layered on Flow |
| [Ara3D.Utils.Roslyn](Ara3D.Utils.Roslyn) | Roslyn compilation helpers (used by Bowerbird) |
| [Ara3D.Utils](Ara3D.Utils) | Paths, zip, profiling, and general utilities |

---

## License

MIT — see [LICENSE](../LICENSE).
