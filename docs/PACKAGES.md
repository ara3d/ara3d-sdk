# NuGet package dependencies

Published packages are listed in [`build/packages.txt`](../build/packages.txt). That manifest
includes **`src/` and `ext/` library projects only** — never `toolchain/`, `plugins/`, `apps/`,
or other repo folders. All listed packages share the version in
[`Directory.Build.props`](../Directory.Build.props). Pack with `pack.bat`; see
[`NUGET_RELEASE.md`](NUGET_RELEASE.md).

Arrows in the diagrams below point from a package **to what it depends on**.

## Meta-package hierarchy

Meta-packages are dependency-only bundles (no source of their own). Pick the smallest tier
that fits your app.

```mermaid
flowchart TB
  subgraph full ["Ara3D.SDK (net8.0-windows — full stack)"]
    SDK["Ara3D.SDK"]
  end

  subgraph tiers ["Meta-packages"]
    CORE["Ara3D.SDK.Core<br/>net8.0"]
    GEO["Ara3D.SDK.Geometry<br/>net8.0"]
    IO["Ara3D.SDK.IO<br/>net8.0-windows"]
    STUDIO["Ara3D.Studio.API"]
    WPF["Ara3D.Utils.Wpf<br/>ext/"]
  end

  SDK --> CORE
  SDK --> GEO
  SDK --> IO
  SDK --> STUDIO
  SDK --> WPF
```

| Meta-package | Directly includes |
| --- | --- |
| **Ara3D.SDK.Core** | Collections, DataTable, Events, F8, Logging, Memory, PropKit, Utils, Utils.Roslyn, WorkItems |
| **Ara3D.SDK.Geometry** | Collections, F8, Geometry, Memory, Models, Utils |
| **Ara3D.SDK.IO** | All I/O + BOS libraries, plus IfcLoader |
| **Ara3D.SDK** | Core + Geometry + IO + Studio.API + Utils.Wpf |

| Meta-package | TFM | Use when |
| --- | --- | --- |
| [Ara3D.SDK.Core](../src/Ara3D.SDK.Core) | `net8.0` | Minimal cross-platform foundation |
| [Ara3D.SDK.Geometry](../src/Ara3D.SDK.Geometry) | `net8.0` | Meshes, models, SIMD math |
| [Ara3D.SDK.IO](../src/Ara3D.SDK.IO) | `net8.0-windows` | File formats, BOS, and IFC conversion |
| [Ara3D.SDK](../src/Ara3D.SDK) | `net8.0-windows` | Everything above plus Studio API and WPF |

## Library dependency graph

Grouped by layer. **Leaf nodes** (no internal Ara3D dependencies): `Collections`, `Events`,
`F8`, `Memory`, `Utils`, `WorkItems`, `IO.SharpGLTF`.

```mermaid
flowchart BT

  subgraph leaves ["Foundation (no internal deps)"]
    Collections
    Events
    F8
    Memory
    Utils
    WorkItems
    SharpGLTF["IO.SharpGLTF"]
  end

  subgraph core ["Core utilities"]
    Logging --> Utils
    UtilsRoslyn["Utils.Roslyn"] --> Logging
    Geometry --> Collections
    Geometry --> Memory
    Geometry --> Utils
    PropKit --> Geometry
    PropKit --> Utils
  end

  subgraph data ["Data & models"]
    DataTable --> Collections
    DataTable --> PropKit
    Models --> Collections
    Models --> F8
    Models --> BFAST["IO.BFAST"]
    Models --> Memory
    Models --> Geometry
    BFAST --> Memory
    BFAST --> Utils
  end

  subgraph io ["File I/O"]
    G3D["IO.G3D"] --> Collections
    G3D --> BFAST
    VIM["IO.VIM"] --> Memory
    VIM --> BFAST
    VIM --> G3D
    StepParser["IO.StepParser"] --> Memory
    StepParser --> Logging
    StepParser --> Utils
    PLY["IO.PLY"] --> Geometry
    PLY --> Memory
    GltfExporter["IO.GltfExporter"] --> Models
    GeoJson["IO.GeoJson"] --> Collections
    GeoJson --> Models
    GeoJson --> Utils
  end

  subgraph bim ["BIM Open Schema"]
    BOS["BimOpenSchema"] --> DataTable
    BOS --> Geometry
    BOS --> Models
    IfcLoader["IfcLoader (ext/)"] --> BOS
    IfcLoader --> StepParser
    IfcLoader --> Models
    BOSIO["BimOpenSchema.IO"] --> BOS
    BOSIO --> IfcLoader
  end

  subgraph studio ["Studio & WPF"]
    StudioAPI["Studio.API"] --> DataTable
    StudioAPI --> Logging
    StudioAPI --> Models
    UtilsWpf["Utils.Wpf (ext/)"] --> Geometry
    UtilsWpf --> Utils
  end
```

## External NuGet dependencies

Most libraries have **zero** external NuGet deps. The exceptions:

```mermaid
flowchart LR
  UtilsRoslyn["Utils.Roslyn"] --> CodeAnalysis["Microsoft.CodeAnalysis.CSharp"]
  UtilsRoslyn --> DiaSym["Microsoft.DiaSymReader.Native"]
  GltfExporter["IO.GltfExporter"] --> Newtonsoft["Newtonsoft.Json"]
  BOSIO["BimOpenSchema.IO"] --> ClosedXML
  BOSIO --> DuckDB["DuckDB.NET.Data.Full"]
  BOSIO --> Parquet["Parquet.Net"]
```

`IfcLoader` also ships the native **`web-ifc-library.dll`** from [`vendor/`](../vendor/) (not a
NuGet package).

## Quick reference

| If you need… | Reference |
| --- | --- |
| Minimal cross-platform foundation | `Ara3D.SDK.Core` |
| Meshes, SIMD math, models | `Ara3D.SDK.Geometry` |
| VIM, PLY, glTF, BOS, IFC | `Ara3D.SDK.IO` |
| Studio plug-in API | `Ara3D.Studio.API` (or full `Ara3D.SDK`) |
| WPF helpers | `Ara3D.Utils.Wpf` |
| Just one format | Individual package, e.g. `Ara3D.IO.VIM` |

Per-project descriptions: [`src/README.md`](../src/README.md).

## Not published to NuGet

Built locally but excluded from meta-packages and [`build/packages.txt`](../build/packages.txt).
Projects under these folders set `IsPackable=false` (see `toolchain/Directory.Build.props`).

| Folder | Examples | Notes |
| --- | --- | --- |
| [`toolchain/`](../toolchain/) | Parakeet, Plato, IfcTypeGen | Dev/codegen tools; Parakeet is **not** packed from this repo |
| [`plugins/`](../plugins/) | Bowerbird, Revit add-ins | Host plug-ins |
| [`apps/`](../apps/) | BOS Browser | Standalone apps |
| [`integrations/`](../integrations/) | Assimp loader | Optional adapters |
| [`wip/`](../wip/) | Domo | Work in progress |
