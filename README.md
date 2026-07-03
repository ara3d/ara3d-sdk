# 📚 Ara3D-SDK

[![NuGet Version](https://img.shields.io/nuget/v/Ara3D.SDK)](https://www.nuget.org/packages/Ara3D.SDK)

**Ara3D-SDK** is a collection of open-source C# libraries for AEC (Architecture, Engineering,
and Construction) workflows. Use it standalone, or to extend and customize the **Ara 3D Studio**
desktop application.

Designed for high performance and scalability, the libraries handle massive 3D data sets in real-time. Core and geometry packages target **`net8.0`** and run cross-platform; file I/O and the full SDK target **`net8.0-windows`**.

## 📁 Repository Structure

- `artifacts/` — built NuGet packages (gitignored; output of `pack.bat`)
- `apps/` — standalone desktop apps (e.g. BOS Browser)
- `data/` — testing data
- `deprecated/` — projects that are no longer being built or maintained
- `examples/` — sample applications and usage examples
- `ext/` — Windows-only SDK extensions (IFC loader, WPF helpers)
- `integrations/` — optional third-party adapters (e.g. Assimp)
- `plugins/` — host plug-ins (Bowerbird, Revit add-ins)
- `plato-src/` — [Plato](https://github.com/ara3d/plato) source for core numerical and geometry types
- `src/` — supported SDK libraries and NuGet meta-packages
- `tests/` — NUnit projects for unit, regression, and developer testing
- `vendor/` — required third-party libraries
- `toolchain/` — Plato parsing and code-generation tools (unsupported)

---

## 📦 NuGet packages

**Current version:** `1.6.1` (set by `Ara3DVersion` in [`Directory.Build.props`](Directory.Build.props)).

All published packages share that version. Bump with `bump-version.bat patch|minor|major|X.Y.Z`, then
build and pack:

```bat
build.bat Release
pack.bat
```

Packages are written to [`artifacts/`](artifacts/) (gitignored). Only `src/` and `ext/`
libraries are packed — see [`build/packages.txt`](build/packages.txt) (nothing under
`toolchain/`, including Parakeet). Dependency diagrams:
[`docs/PACKAGES.md`](docs/PACKAGES.md). Release workflow:
[`docs/NUGET_RELEASE.md`](docs/NUGET_RELEASE.md).

### Meta-packages

Meta-packages are dependency-only bundles (no source of their own). Pick the smallest tier
that fits your app:

```
Ara3D.SDK  (net8.0-windows — full Windows stack)
├── Ara3D.SDK.Core            net8.0 — cross-platform foundation
├── Ara3D.SDK.Geometry        net8.0 — meshes, models, SIMD math
├── Ara3D.SDK.IO              net8.0-windows — file formats, BOS, IFC
├── Ara3D.Studio.API          Studio plug-in API
└── Ara3D.Utils.Wpf           WPF helpers (ext/)
```

Each library below is also published on its own at the same version. Per-project READMEs:
[`src/README.md`](src/README.md).

**Ara3D.SDK.Core** (`net8.0`)

| Package | Description |
| --- | --- |
| `Ara3D.Collections` | Read-only list views, sparse matrices, LINQ helpers |
| `Ara3D.DataTable` | Columnar in-memory data interfaces |
| `Ara3D.Events` | Thread-safe event bus |
| `Ara3D.F8` | SIMD (`AVX`) 8-wide float math |
| `Ara3D.Logging` | Logging, progress, and job management |
| `Ara3D.Memory` | Aligned buffers, slices, memory-mapped file views |
| `Ara3D.PropKit` | Runtime property descriptors for UI binding |
| `Ara3D.Utils` | Paths, zip, profiling, and general utilities |
| `Ara3D.Utils.Roslyn` | Roslyn compilation helpers |
| `Ara3D.WorkItems` | Background work-item queues |

**Ara3D.SDK.Geometry** (`net8.0`)

| Package | Description |
| --- | --- |
| `Ara3D.Collections` | Read-only list views, sparse matrices, LINQ helpers |
| `Ara3D.F8` | SIMD (`AVX`) 8-wide float math |
| `Ara3D.Geometry` | Meshes, topology, SDFs, voxels, spatial queries |
| `Ara3D.Memory` | Aligned buffers, slices, memory-mapped file views |
| `Ara3D.Models` | Scene models, instances, render buffers |
| `Ara3D.Utils` | Paths, zip, profiling, and general utilities |

**Ara3D.SDK.IO** (`net8.0-windows`)

| Package | Description |
| --- | --- |
| `Ara3D.IO.BFAST` | Binary Format for Array Serialization and Transmission |
| `Ara3D.IO.G3D` | G3D geometry exchange format (BFAST container) |
| `Ara3D.IO.GeoJson` | GeoJSON and IMDF indoor mapping |
| `Ara3D.IO.GltfExporter` | glTF/GLB export |
| `Ara3D.IO.PLY` | PLY mesh import/export |
| `Ara3D.IO.SharpGLTF` | glTF/GLB import and manipulation (assembly: `SharpGLTF.Core`) |
| `Ara3D.IO.StepParser` | ISO STEP file tokenizer and parser |
| `Ara3D.IO.VIM` | VIM BIM binary format |
| `Ara3D.BimOpenSchema` | BIM Open Schema object model |
| `Ara3D.BimOpenSchema.IO` | Parquet/DuckDB/Excel serialization and IFC import |
| `Ara3D.IfcLoader` | IFC → BOS conversion (`ext/`) |

**Ara3D.SDK** (`net8.0-windows`) — all three meta-packages above, plus:

| Package | Description |
| --- | --- |
| `Ara3D.Studio.API` | Flow graph, assets, and modifier pipeline types |
| `Ara3D.Utils.Wpf` | WPF helpers (`ext/`) |

### Not published to NuGet

These repo folders are built locally but excluded from meta-packages and `build/packages.txt`:

- [`plugins/`](plugins/) — Bowerbird, Revit add-ins
- [`apps/`](apps/) — BOS Browser
- [`integrations/`](integrations/) — Assimp loader
- [`wip/`](wip/) — work in progress (e.g. Domo)
- [`toolchain/`](toolchain/) — dev tools (Parakeet, Plato, IfcTypeGen); `IsPackable=false`, not in `packages.txt`

---

## 📜 License

**Ara3D-SDK** is licensed under the [MIT License](LICENSE).

---

## 🖇️ Dependencies

Most library packages have **zero** external NuGet dependencies. The exceptions:

| Package | External dependencies |
| --- | --- |
| `Ara3D.Utils.Roslyn` | Microsoft.CodeAnalysis.CSharp, Microsoft.DiaSymReader.Native |
| `Ara3D.IO.GltfExporter` | Newtonsoft.Json |
| `Ara3D.BimOpenSchema.IO` | ClosedXML, DuckDB.NET.Data.Full, Parquet.Net |

`Ara3D.IfcLoader` also ships the native **`web-ifc-library.dll`** from [`vendor/`](vendor/)
(not a NuGet package). See [`docs/PACKAGES.md`](docs/PACKAGES.md) for the full dependency graph.

Test projects and samples are Windows-specific and use NUnit 3. Auto-generated code from Plato
uses projects in [`toolchain/`](toolchain/); they are not run automatically and are not
currently supported. 

---

## 🗂️ Projects

All supported libraries live under [`src/`](src/). See the [src/ project index](src/README.md) for
per-project READMEs grouped by category (geometry, I/O, BIM, Studio, utilities).

Install via a meta-package (see [NuGet packages](#-nuget-packages) above) or reference individual
library projects from `src/`.

---

## 🤝 Contributing

We welcome contributions of all kinds—bug fixes, features, documentation, and more!

Before submitting a pull request:
- Familiarize yourself with the code base
- Follow the existing style and architecture
- For significant changes, please open an [issue](https://github.com/ara3d/ara3d-sdk/issues) first

### Coding guidelines and AI agents

Coding conventions and the preferred development workflow are documented in
[`AGENTS.md`](AGENTS.md), with tracked improvements logged in
[`docs/TECHNICAL_DEBT.md`](docs/TECHNICAL_DEBT.md). Read these before making changes
(they apply to AI agents and humans alike).

Use the helper scripts to build and test:

```bat
build.bat              :: build the solution (Debug)
test.bat               :: run the full test suite (includes Slow tests)
test.bat fast          :: run all areas, skip Slow file-I/O tests
test.bat geometry      :: run only one area's tests (all | sdk | geometry | bim | devtools | knownissues)
test.bat geometry fast :: run one area, skip Slow tests
test.bat geometry Delaunay   :: run tests in an area matching a name substring
test.bat knownissues   :: run documented known-broken behavior tests (opt-in only)
pack.bat               :: pack all NuGet packages from build/packages.txt (Release)
release.bat            :: build supported SDK surface, run scoped tests, then pack
bump-version.bat patch :: bump Ara3DVersion in Directory.Build.props
publish-nuget.bat smoke :: build, test, pack, and run NuGet integration tests (no push)
release-nuget.bat patch :: full NuGet release (bump, smoke, commit, tag, publish)
save.bat "message"      :: commit without pushing
```

Script cheat sheet: [`docs/WORKFLOWS.md`](docs/WORKFLOWS.md).  
NuGet release: [`docs/NUGET_RELEASE.md`](docs/NUGET_RELEASE.md).

Known-issues tests are intentionally excluded from default and release test runs. They
document bugs or incomplete behavior that should fail until the underlying issue is fixed.

Let's build something amazing together 🚀

---

## 🐞 Issues and Feature Requests

Found a bug? Have a question? Want to suggest a feature for either the SDK or **Ara 3D Studio**?

👉 [Open an issue](https://github.com/ara3d/ara3d-sdk/issues) and let us know!

---

## 🔗 Related Projects

- [Ara3D.Plato](https://github.com/cdiggins/plato) – A domain-specific programming language for math and geometry
