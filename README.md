# 📚 Ara3D-SDK

[![NuGet Version](https://img.shields.io/nuget/v/Ara3D.SDK)](https://www.nuget.org/packages/Ara3D.SDK)

**Ara3D-SDK** is a powerful collection of open-source C# libraries for processing, transforming, and visualizing large-scale 3D models—especially tailored for AEC (Architecture, Engineering, and Construction) workflows.

Use it standalone, or extend and customize the **Ara 3D Studio** desktop application.

Designed for high performance and scalability, the libraries handle massive 3D data sets in real-time. They're cross-platform and compatible with .NET 8.

<!--
---

## 🖥️ Ara 3D Studio

**Ara 3D Studio** is a Windows desktop application included in this repository as a pre-built binary (`Ara3D.Studio.exe`). It allows you to:

- View, import, and export large-scale 3D models
- Create and manipulate geometry interactively
- Extend functionality via plugins and scripting

Once registered, the software is free to use for any purpose, including commercial work.

➡️ To get started, download and run `Ara3D.Studio.exe` from the `dist/` folder.
-->

---

## 📁 Repository Structure

- `data/` – Sample and test 3D models
- `dist/` – Pre-built binaries and tools, including `Ara3D.Studio.exe`
- `examples/` – Sample applications and usage examples
- `plato-src/` – [Plato](https://github.com/ara3d/plato) source code for core numerical and geometry types and functions 
- `src/` – Core cross-platform C# libraries
- `tests/` – NUnit projects for unit, regression, and developer testing
- `toolchain/` – Projects for parsing Plato and generating C# sourc code. 

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

- [Ara3D.Collections](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.Collections)  
  Optimized LINQ operations for IReadOnlyList. Additional generic collection types and utilities.

- [Ara3D.DataTable](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.DataTable)  
  In-memory, schema-driven table structures optimized for analytical workloads.

- [Ara3D.Domo](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.Domo)  
  Domain modeling helpers and patterns for building robust data-driven apps.

- [Ara3D.Geometry](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.Geometry)  
  2D and 3D math and geometry library. Compatible with System.Numerics.

- [Ara3D.IO.A3D](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.IO.A3D)  
  [*Under Development*] Reader/writer for the `.a3d` geometry format.

- [Ara3D.IO.BFAST](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.IO.BFAST)  
  High-speed I/O support for the BFAST binary columnar / array data format.

- [Ara3D.IO.G3D](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.IO.G3D)  
  Import/export of G3D mesh and scene files.

- [Ara3D.IO.IfcParser](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.IO.IfcParser)  
  IFC STEP-based BIM file parser with object graph mapping.

- [Ara3D.IO.StepParser](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.IO.StepParser)  
  Generic STEP file parser for CAD and BIM interoperability.

- [Ara3D.IO.VIM](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.IO.VIM)  
  Reader/writer for the VIM format.

- [Ara3D.Logging](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.Logging)  
  Lightweight logging framework 

- [Ara3D.Memory](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.Memory)  
  Efficient low-level routines for working with aligned memory and named buffers.

- [Ara3D.MemoryMappedFiles](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.MemoryMappedFiles)  
  Utilities to work with large files via memory-mapping.

- [Ara3D.Models](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.Models)  
  Core 3D model interfaces and data structures. Models are embedded scene-graphs with support for instancing.

- [Ara3D.NarwhalDB](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.NarwhalDB)  
  High-performance in-memory read-only relational database.

- [Ara3D.PropKit](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.PropKit)  
  Property descriptors and change-notification toolkit for auto-generated serialization and UI.

- [Ara3D.SceneEval](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.SceneEval)  
  Data structures for creating a scene evaluation dependency graph with caching. 

- [Ara3D.ScriptService](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.ScriptService)  
  Integration layer for embedding C# scripts at runtime.

- [Ara3D.SDK](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.SDK)  
  Meta-package that bundles all core Ara3D libraries.

- [Ara3D.Services](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.Services)  
  Library for creating a simple service-oriented architecture, with dependency injection.

- [Ara3D.Studio.API](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.Studio.API)  
  Public API surface for extending Ara 3D Studio.

- [Ara3D.Studio.Data](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.Studio.Data)  
  Low-level data layer for Ara 3D Studio.

- [Ara3D.Utils](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.Utils)  
  Miscellaneous helper methods and extension functions.

- [Ara3D.Utils.Roslyn](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.Utils.Roslyn)  
  Utility extensions for the C# Roslyn compiler.

- [Plato.Generated](https://github.com/ara3d/ara3d-sdk/tree/main/src/Plato.Generated)  
  C# shared project containing C# source generated from Plato source code. Contains core numerical and geometry types and functions.

- [Plato.Intrinsics](https://github.com/ara3d/ara3d-sdk/tree/main/src/Plato.Intrinsics)  
  C# shared project containing predefined types and built-in functions required by Plato generated code.

---

## 🚀 Getting Started

To get started with **Ara3D-SDK** you can run the Ara3D.Studio exe found in the `dist` folder. Take a look at the `examples` file.
If all goes well, you be able to modify scripts in that file and have the script auto-load.

---

## 🤝 Contributing

We welcome contributions of all kinds—bug fixes, features, documentation, and more!

Before submitting a pull request:
- Familiarize yourself with the code base
- Follow the existing style and architecture
- For significant changes, please open an [issue](https://github.com/ara3d/ara3d-sdk/issues) first

Let's build something amazing together 🚀

---

## 🐞 Issues and Feature Requests

Found a bug? Have a question? Want to suggest a feature for either the SDK or **Ara 3D Studio**?

👉 [Open an issue](https://github.com/ara3d/ara3d-sdk/issues) and let us know!

---

## 🔗 Related Projects

- [Ara3D.Plato](https://github.com/cdiggins/plato) – A Domain specific programming language for math and geometry/
