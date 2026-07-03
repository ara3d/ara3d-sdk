# 📚 Ara3D-SDK

[![NuGet Version](https://img.shields.io/nuget/v/Ara3D.SDK)](https://www.nuget.org/packages/Ara3D.SDK)

**Ara3D-SDK** is a powerful collection of open-source C# libraries for processing, transforming, and visualizing large-scale 3D models tailored for 
AEC (Architecture, Engineering, and Construction) workflows. 

Use it standalone, or to extend and customize the **Ara 3D Studio** desktop application.

Designed for high performance and scalability, the libraries handle massive 3D data sets in real-time. They're cross-platform and compatible with .NET 8.

## 📁 Repository Structure

- `artifacts/` — built NuGet packages (gitignored; output of `pack.bat`)
- `apps/` — standalone desktop apps (e.g. BOS Browser)
- `data/` — testing data
- `deprecated/` — projects that are no longer being built or maintained
- `dist/` — pre-built binaries and tools, including `Ara3D.Studio.exe`
- `examples/` — sample applications and usage examples
- `ext/` — Windows-only SDK extensions (IFC loader, WPF helpers)
- `integrations/` — optional third-party adapters (e.g. Assimp)
- `plugins/` — host plug-ins (Bowerbird, Revit add-ins)
- `plato-src/` — [Plato](https://github.com/ara3d/plato) source for core numerical and geometry types
- `src/` — supported cross-platform libraries and NuGet meta-packages
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

Packages are written to [`artifacts/`](artifacts/) (gitignored). The pack list is
[`build/packages.txt`](build/packages.txt). Dependency diagrams:
[`docs/PACKAGES.md`](docs/PACKAGES.md). Release workflow:
[`docs/NUGET_RELEASE.md`](docs/NUGET_RELEASE.md).

### Meta-package hierarchy

Meta-packages are dependency-only bundles (no source of their own). Pick the smallest tier
that fits your app:

```
Ara3D.SDK  (net8.0-windows — full Windows stack)
├── Ara3D.SDK.Core
├── Ara3D.SDK.Geometry
├── Ara3D.SDK.IO
├── Ara3D.Studio.API
└── Ara3D.Utils.Wpf          (ext/)
```

| Meta-package | TFM | Use when |
| --- | --- | --- |
| [Ara3D.SDK.Core](src/Ara3D.SDK.Core) | `net8.0` | Minimal cross-platform foundation |
| [Ara3D.SDK.Geometry](src/Ara3D.SDK.Geometry) | `net8.0` | Meshes, models, SIMD math |
| [Ara3D.SDK.IO](src/Ara3D.SDK.IO) | `net8.0-windows` | File formats, BOS, and IFC conversion |
| [Ara3D.SDK](src/Ara3D.SDK) | `net8.0-windows` | Everything above plus Studio API and WPF |

### What each meta-package includes

**Ara3D.SDK.Core** — `Ara3D.Collections`, `Ara3D.DataTable`, `Ara3D.Events`, `Ara3D.F8`,
`Ara3D.Logging`, `Ara3D.Memory`, `Ara3D.PropKit`, `Ara3D.Utils`, `Ara3D.Utils.Roslyn`,
`Ara3D.WorkItems`

**Ara3D.SDK.Geometry** — `Ara3D.Collections`, `Ara3D.F8`, `Ara3D.Geometry`, `Ara3D.Memory`,
`Ara3D.Models`, `Ara3D.Utils`

**Ara3D.SDK.IO** — `Ara3D.IO.BFAST`, `Ara3D.IO.G3D`, `Ara3D.IO.GeoJson`,
`Ara3D.IO.GltfExporter`, `Ara3D.IO.PLY`, `Ara3D.IO.SharpGLTF`, `Ara3D.IO.StepParser`,
`Ara3D.IO.VIM`, `Ara3D.BimOpenSchema`, `Ara3D.BimOpenSchema.IO`, `Ara3D.IfcLoader`

**Ara3D.SDK** — all three meta-packages above, plus `Ara3D.Studio.API` and `Ara3D.Utils.Wpf`
from `ext/`.

### Individual library packages

Every library listed above is also published on its own (same version). Windows extensions
`Ara3D.IfcLoader` and `Ara3D.Utils.Wpf` from [`ext/`](ext/) are published separately as well.
You can reference individual packages instead of a meta-package when you want a smaller
dependency graph. Project descriptions and links: [`src/README.md`](src/README.md).

Note: `Ara3D.IO.SharpGLTF` is the NuGet package ID; the assembly name remains `SharpGLTF.Core`
for upstream API compatibility.

### Not published to NuGet

These repo folders are built locally but excluded from meta-packages and `build/packages.txt`:

- [`plugins/`](plugins/) — Bowerbird, Revit add-ins
- [`apps/`](apps/) — BOS Browser
- [`integrations/`](integrations/) — Assimp loader
- [`wip/`](wip/) — work in progress (e.g. Domo)
- [`toolchain/`](toolchain/) — dev tools and Plato experiments

---

## 📜 License

**Ara3D-SDK** is licensed under the [MIT License](LICENSE).

---

## 🖇️ Dependencies 

The core **Ara3D.SDK** package is .NET 8 compatible, cross-platform,  
and uses only the following external Nuget libraries:

- Microsoft.CodeAnalysis.CSharp - 4.8.0
- Microsoft.DiaSymReader.Native - 1.7.0
- System.Memory - 4.6.0

The test projects and samples are windows-specific and use NUnit 3 and executables found 
in the `dist` folder.

The auto-generated code from Plato uses projects found in the toolchain folder. 
They are not run automatically, and are not currently supported. 

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
test.bat sdk OpenVIM   :: run tests in an area matching a name substring
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
