# Ara 3D SDK

[![NuGet Version](https://img.shields.io/nuget/v/Ara3D.SDK)](https://www.nuget.org/packages/Ara3D.SDK)

**Ara 3D SDK** is a set of open-source C# libraries for developers who need to build,
transform, and exchange 3D geometry and BIM data in .NET — either inside **Ara 3D Studio**
or in a standalone tool or service.

The libraries target **.NET 8**. Core and geometry packages use `net8.0` and run
cross-platform. The full `Ara3D.SDK` meta-package and IFC conversion path use
`net8.0-windows`.

Published package version: **1.6.1** (from `Ara3DVersion` in
[`Directory.Build.props`](Directory.Build.props); also the latest release on
[nuget.org](https://www.nuget.org/packages/Ara3D.SDK) as of 2026-07-29).

---

## What problem it solves

Without a shared geometry and BIM stack, each tool reinvents meshes, scene instances,
format readers, and property tables — and then cannot share data with Ara 3D Studio or
with other tools in this family.

This SDK gives you:

- **Geometry** — triangle/quad meshes, topology, spatial queries, signed distance fields,
  voxels, and common mesh algorithms (`Ara3D.Geometry`, `Ara3D.Models`).
- **BIM tabular data** — the BIM Open Schema object model and Parquet/DuckDB/Excel I/O
  (`Ara3D.BimOpenSchema`, `Ara3D.BimOpenSchema.IO`).
- **AEC file formats** — VIM, G3D/BFAST, PLY, STEP tokenization, GeoJSON/IMDF, glTF export,
  and IFC → BOS conversion on Windows (`Ara3D.IO.*`, `Ara3D.IfcLoader`).
- **Studio scripting contracts** — generators, modifiers, tools, and the evaluation pipeline
  types used by Ara 3D Studio plug-ins (`Ara3D.Studio.API`, with host-free evaluation types
  in `Ara3D.Flow` in source).
- **Small foundations** — memory views, read-only list helpers, logging, property
  descriptors, and work queues with few or no external NuGet dependencies.

You can install a meta-package from NuGet, or reference individual projects under
[`src/`](src/) and [`ext/`](ext/).

---

## What it does not solve

- It is **not a CAD or BIM authoring application**. Ara 3D Studio is the desktop product;
  this repository is the library layer underneath it (and usable without it).
- It is **not a real-time renderer or game engine**. Models and buffers exist so hosts can
  draw them; there is no OpenGL/Vulkan/Direct3D engine here.
- It is **not a complete IFC platform**. Windows IFC import goes through a native
  `web-ifc` DLL (`Ara3D.IfcLoader`). A pure-C# IFC mesher lives under [`wip/`](wip/) and is
  experimental. Round-trip IFC write, full schema validation, and multi-platform IFC
  conversion are out of scope for the published packages.
- It is **not a general-purpose math library**. Vector/matrix/mesh math types come from
  Plato-generated code compiled into `Ara3D.Geometry`; the focus is geometry and AEC data,
  not arbitrary numerical computing.
- **Plug-ins, apps, and WIP projects are not on NuGet.** Revit/Bowerbird add-ins,
  the BOS Browser app, Assimp integration, and `wip/` experiments stay in this repo only.

---

## Example

Create a cube mesh and write an STL file (requires `Ara3D.SDK.Geometry` or `Ara3D.Geometry`):

```csharp
using Ara3D.Geometry;

var mesh = PlatonicSolids.TriangulatedCube;
Console.WriteLine($"{mesh.Points.Count} vertices, {mesh.FaceIndices.Count} triangles");
mesh.WriteStl("cube.stl");
```

Studio scripts use the same geometry types behind `IGenerator` / `IModifier` contracts.
See [`examples/Ara3D.Studio.Examples`](examples/Ara3D.Studio.Examples) and the workshop
lessons under [`examples/Workshop`](examples/Workshop).

---

## How to use it

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Windows, if you need `Ara3D.SDK`, `Ara3D.SDK.IO`, IFC loading, or WPF helpers

### Install from NuGet

Pick the smallest meta-package that fits:

```bat
dotnet add package Ara3D.SDK.Geometry
```

| Meta-package | TFM | Use when |
| --- | --- | --- |
| [`Ara3D.SDK.Core`](https://www.nuget.org/packages/Ara3D.SDK.Core) | `net8.0` | Utilities, memory, logging, collections — no geometry |
| [`Ara3D.SDK.Geometry`](https://www.nuget.org/packages/Ara3D.SDK.Geometry) | `net8.0` | Meshes, models, SIMD helpers |
| [`Ara3D.SDK.IO`](https://www.nuget.org/packages/Ara3D.SDK.IO) | `net8.0-windows` | File formats + BOS + IFC conversion |
| [`Ara3D.SDK`](https://www.nuget.org/packages/Ara3D.SDK) | `net8.0-windows` | Full stack: Core + Geometry + IO + Studio API + WPF |

Most individual libraries under `src/` and `ext/` are also published at the same version.
Most format readers (`Ara3D.IO.PLY`, `Ara3D.IO.VIM`, `Ara3D.IO.BFAST`, …) target `net8.0`
on their own; the **IO meta-package** is Windows-only because it includes `Ara3D.IfcLoader`.

### Build from source

```bat
build.bat
test.bat fast
```

`test.bat` runs the full supported suite (including Slow file tests).
`test.bat fast` skips Slow tests. Area filters: `sdk`, `geometry`, `bim`, `devtools`.
Script details: [`docs/WORKFLOWS.md`](docs/WORKFLOWS.md).

### Verify the install

After adding `Ara3D.SDK.Geometry`, the example above should compile and write `cube.stl`.
From a clone, `build.bat` followed by `test.bat geometry fast` should pass.

---

## Trade-offs

- **Few external dependencies in core libraries.** You get small, relocatable packages, but
  you will not find a large third-party ecosystem already wired in. Heavy dependencies
  (DuckDB, Parquet, ClosedXML, Roslyn, native `web-ifc`) are confined to specific packages.
- **Immutable, extension-oriented C# style.** Geometry APIs favor pure functions and
  read-only views. That helps composition and testing; hot mutable in-place APIs are not
  the default shape.
- **Plato-generated math surface.** Shared math/mesh types are generated and checked in
  under [`src/Plato.Generated/`](src/Plato.Generated/). You consume them as normal C#; you
  do not need the Plato compiler to use the SDK. Regenerating that surface is a separate
  toolchain concern (see [`CLAUDE.md`](CLAUDE.md)).
- **Windows for the full AEC stack.** Cross-platform work should reference `Ara3D.SDK.Core`
  or `Ara3D.SDK.Geometry`, not `Ara3D.SDK`.

---

## Known and tested vs. untested

**Demonstrated (automated):**

- Supported unit/regression areas via `test.bat` / `test.bat fast` —
  `Ara3D.SDK.Tests`, `Ara3D.SDK.GeometryTests`, `Ara3D.BimOpenSchema.Tests`, and related
  projects under [`tests/`](tests/) (exact counts change; GeometryTests alone had on the
  order of 50+ `[Test]` methods when sampled 2026-07-29).
- NuGet restore smoke tests (`test.bat nuget` after `pack.bat`) — see
  [`docs/NUGET_RELEASE.md`](docs/NUGET_RELEASE.md).
- Source metrics snapshot for `src/` (2026-07-03): about 59k code lines across the
  supported libraries — [`docs/SRC_METRICS.md`](docs/SRC_METRICS.md).

**Known gaps / incomplete:**

- Documented broken or unfinished behavior lives in `tests/Ara3D.SDK.KnownIssues.Tests`
  (`test.bat knownissues`) and in [`docs/TECHNICAL_DEBT.md`](docs/TECHNICAL_DEBT.md)
  (for example unfinished `FlowObject.Transform`, lossy PLY attribute import).
- IFC meshing parity and pure-C# meshing work under [`wip/Ara3D.Ifc.Mesher`](wip/) and
  `tests/Ara3D.IfcMeshingComparison` are **not** part of the default `test.bat` gate.
- [`wip/`](wip/), [`plugins/`](plugins/), and [`apps/`](apps/) are not release-quality
  SDK surface; treat them as product-specific or experimental.
- `Ara3D.Flow` (host-free evaluation pipeline) exists in source and is referenced by
  `Ara3D.Studio.API`, but it is **not** listed in [`build/packages.txt`](build/packages.txt)
  and has no separate NuGet package as of 2026-07-29. Prefer `Ara3D.Studio.API` as the
  published Studio entry point until Flow is packaged.

---

## Similar and related work

Comparisons below describe intent; package versions and feature sets change over time.

| Project | Relationship |
| --- | --- |
| [xBIM](https://docs.xbim.net/) | Broader IFC/BIM toolkit for .NET. This SDK’s IFC path is narrower (import → BOS via `web-ifc` on Windows). |
| [SharpGLTF](https://github.com/vpenades/SharpGLTF) | glTF library; `Ara3D.IO.SharpGLTF` is a maintained fork/vendored copy used here. |
| [Assimp](https://github.com/assimp/assimp) / AssimpNet | Broad mesh import. Optional wrapper lives in [`integrations/`](integrations/), not in the default meta-packages. |
| [geometry3Sharp](https://github.com/gradientspace/geometry3Sharp) | General mesh algorithms in C#. Overlaps some mesh ops; this SDK also covers BIM schema, Studio contracts, and AEC formats. |
| [Plato](https://github.com/cdiggins/plato) | Language/toolchain that generates math types consumed by `Ara3D.Geometry`. |

---

## Repository organization

| Path | Purpose |
| --- | --- |
| [`src/`](src/) | Supported libraries and NuGet meta-packages — start here |
| [`ext/`](ext/) | Windows-only extensions (IFC loader, WPF helpers) |
| [`tests/`](tests/) | NUnit projects (`test.bat` areas) |
| [`examples/`](examples/) | Studio scripts and workshop samples |
| [`apps/`](apps/) | Standalone apps (e.g. BOS Browser) — not on NuGet |
| [`plugins/`](plugins/) | Bowerbird / Revit hosts — not on NuGet |
| [`integrations/`](integrations/) | Optional third-party adapters (Assimp) |
| [`wip/`](wip/) | Experiments (IFC mesher, Domo, MCP, …) |
| [`toolchain/`](toolchain/) | Dev tools — never NuGet-packed |
| [`vendor/`](vendor/) | Native binaries (e.g. `web-ifc-library.dll`) |
| [`docs/`](docs/) | Package graphs, workflows, debt log |
| [`artifacts/`](artifacts/) | Packed `.nupkg` output (gitignored) |
| [`deprecated/`](deprecated/) | Unmaintained projects |

Per-library descriptions: [`src/README.md`](src/README.md).  
Dependency diagrams: [`docs/PACKAGES.md`](docs/PACKAGES.md).  
What gets packed: [`build/packages.txt`](build/packages.txt).

### Published packages (summary)

```
Ara3D.SDK  (net8.0-windows)
├── Ara3D.SDK.Core            net8.0
├── Ara3D.SDK.Geometry        net8.0
├── Ara3D.SDK.IO              net8.0-windows
├── Ara3D.Studio.API          net8.0
└── Ara3D.Utils.Wpf           ext/
```

**Ara3D.SDK.Core** — `Ara3D.Collections`, `Ara3D.DataTable`, `Ara3D.Events`, `Ara3D.F8`,
`Ara3D.Logging`, `Ara3D.Memory`, `Ara3D.PropKit`, `Ara3D.Utils`, `Ara3D.Utils.Roslyn`,
`Ara3D.WorkItems`.

**Ara3D.SDK.Geometry** — `Ara3D.Collections`, `Ara3D.F8`, `Ara3D.Geometry`, `Ara3D.Memory`,
`Ara3D.Models`, `Ara3D.Utils`.

**Ara3D.SDK.IO** — `Ara3D.IO.BFAST`, `Ara3D.IO.G3D`, `Ara3D.IO.GeoJson`,
`Ara3D.IO.GltfExporter`, `Ara3D.IO.PLY`, `Ara3D.IO.SharpGLTF`, `Ara3D.IO.StepParser`,
`Ara3D.IO.VIM`, `Ara3D.BimOpenSchema`, `Ara3D.BimOpenSchema.IO`, `Ara3D.IfcLoader`.

External NuGet dependencies are rare outside Roslyn helpers, glTF JSON, and BOS I/O
(ClosedXML, DuckDB, Parquet). Details: [`docs/PACKAGES.md`](docs/PACKAGES.md).

---

## Who it is for

- .NET developers building geometry tools, importers/exporters, or BIM data pipelines
- Authors of Ara 3D Studio scripts and plug-ins who need the public API types
- Teams that want a dependency-light mesh/BIM core they can relocate into another product

## Who it is not for (yet)

- Projects that need a polished multi-platform IFC authoring or validation suite
- Teams looking for a turnkey viewport/renderer without hosting their own UI
- Contributors expecting a large multi-maintainer process — this tree is maintained
  primarily by one person with little outside contribution traffic

---

## Contributing

Small, focused fixes and documentation improvements are welcome. For larger changes,
open an [issue](https://github.com/ara3d/ara3d-sdk/issues) first.

Coding conventions and the preferred workflow are in [`AGENTS.md`](AGENTS.md).
Tracked debt: [`docs/TECHNICAL_DEBT.md`](docs/TECHNICAL_DEBT.md).

```bat
build.bat
test.bat fast
test.bat                 :: full suite before you consider work done
pack.bat                 :: packages from build/packages.txt → artifacts/
```

NuGet release process: [`docs/NUGET_RELEASE.md`](docs/NUGET_RELEASE.md).

---

## License

MIT — see [`LICENSE`](LICENSE).

## Related projects

- [Ara 3D Studio](https://github.com/ara3d/studio) — desktop application that consumes this SDK
- [Plato](https://github.com/cdiggins/plato) — language/toolchain for the generated math surface
