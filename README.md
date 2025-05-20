# 📚 Ara3D-SDK

[![NuGet Version](https://img.shields.io/nuget/v/Ara3D.SDK)](https://www.nuget.org/packages/Ara3D.SDK)

**Ara3D-SDK** is a powerful collection of open-source C# libraries for processing, transforming, and visualizing large-scale 3D models—especially tailored for AEC (Architecture, Engineering, and Construction) workflows.

Use it standalone, or extend and customize the **Ara 3D Studio** desktop application.

Designed for high performance and scalability, the libraries handle massive 3D data sets in real-time. They're cross-platform and compatible with .NET 8.

---

## 🖥️ Ara 3D Studio

**Ara 3D Studio** is a Windows desktop application included in this repository as a pre-built binary (`Ara3D.Studio.exe`). It allows you to:

- View, import, and export large-scale 3D models
- Create and manipulate geometry interactively
- Extend functionality via plugins and scripting

Once registered, the software is free to use for any purpose, including commercial work.

➡️ To get started, download and run `Ara3D.Studio.exe` from the `dist/` folder.

---

## 📁 Repository Structure

- `data/` – Sample and test 3D models
- `examples/` – Sample applications and usage examples
- `src/` – Core cross-platform C# libraries
- `plato/` – [Plato](https://github.com/ara3d/plato): a scripting language and toolchain
- `tests/` – NUnit projects for unit, regression, and developer testing
- `dist/` – Pre-built binaries and tools, including `Ara3D.Studio.exe`

---

## 📜 License

**Ara3D-SDK** is licensed under the [MIT License](LICENSE).

---

## 🖇️ Dependencies 

The core **Ara3D.SDK** package is .NET 8 compatible and uses the following external libraries:

- Microsoft.CodeAnalysis.CSharp - 4.8.0
- Microsoft.DiaSymReader.Native - 1.7.0
- System.Memory - 4.6.0

The test projects and samples are windows-specific and use NUnit 3 and executables found 
in the `dist` folder. 

---

## 🗂️ Projects

- [Ara3D.Collections](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.Collections)  
  Optimized LINQ operations for IReadOnlyList. Additional generic collection types and utilities.

- [Ara3D.DataTable](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.DataTable)  
  In-memory, schema-driven table structures optimized for analytical workloads.

- [Ara3D.Domo](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.Domo)  
  Domain modeling helpers and patterns for building robust data-driven apps.

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
  Roslyn-specific utility extensions (syntax walkers, code fixes).

- [Plato.Geometry](https://github.com/ara3d/ara3d-sdk/tree/main/src/Plato.Geometry)  
  2D and 3D math and geometry library. Written in Plato, and auto-generated as C#. 

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

- [Ara3D.Plato](https://github.com/ara3d/plato) – Lightweight scripting language for geometry
- [Ara3D.IO](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.IO) – Import/export logic for various 3D formats

<!--

# Libraries

## Math and Geometry Libraries

### plato-src 

This project contains the Plato source code for our core geometry and mathematics libraries. 
Plato is a domain specific language, designed to make it easy to design numerical data structures and algorithms
that target different languages. 

For more information see [the Plato repository](https://github.com/cdiggins/plato). 

This code is being migrated from [the Plato.Geometry repository](https://github.com/ara3d/Plato.Geometry). 

###  Plato.Core

This contains an extensive C# library of mathematical and geometric data structures and routines. This code 
is auto- generated from the `plato-src` proejct.   

### Plato.Intrinsics

This is a shared project containing the primitive types and building block functions assumed by the Plato code generator. It is used by Plato.Core. 

### Ara3D.Scene

A simple generic 3D scene graph library for use by both IO libraries and rendering libraries. 

## Low-Level Libraries

### Ara3D.Memory

This is a collection of useful classes and interfaces for efficiently working with very large amounts of aligned low-level memory. 

Compared to the System libraries:

* Can go beyond the 2^31 limit imposed by `Span`
* `ByteSlice` is not subject to `ref struct` limitated (e.g., can be stored on the heap) 
* Uses aligned native allocators  so that it can be cast to SIMD vector types (e.g. `Vector256<float>`)

The primary classes and structs are:

* `AlignedMemory` - A block of fixed memory that is aligned to a specific byte boundary. This makes casting between SIMD type (like Vector256) safe and efficient. It can be larger than 2GB. 
* `FixedArray` - A pointer to an array that is fixed in memory, and provides access via ByteSlices and Spans.  
* `ByteSlice` - A pointer to a region of memory, with a length. Similar to a `Span<byte>` except that it can be stored on the heap and can be longer than 2GB. Provides helpers for safe casting to unmanaged types. 
* `UnmanagedList<T>` - A dynamic array of unmanaged types which uses, and makes public, an aligned memory block. Can grow but not shrink. 
* `Buffer<T>` - A types-safe wrapper around a `ByteSlice` and that exposes an array-like interface for reading and writing. 

The interfaces are:

* `IBuffer` - A generic block of memory accessible as a slice. 
* `ITypedBuffer` - A generic interface for a buffer of unmanaged types. 
* `INamedBuffer` - A buffer with an associated name.
* `ITypedNamedBuffer` - A buffer with both an associated name and a type 
* `IBuffer<T>` - An array of unmanaged types. Implements `IReadOnlyList<T>`
* `INamedBuffer<T>` - An array of unmanaged types associated with a name.
* `IMemoryOwner` - A disposable block of memory that provides a `IBuffer` interface.
* `IMemoryOwner<T>` - A disposable block of memory that provides an `IBuffer<T>` interface.

## Infrastructure Libraries 

### Ara3D.Logging 

A library of classes to help with logging.

### Ara3D.Utils

A collection of miscellaneous helper types and functions. 

### Ara3D.Domo

A library for defining "models" in the context of MVC or MVVM architecture. Domo stands for domain modeling, 
and is inspired by Domain Driven Design principles. 

In a nutshell, using Domo you can define data models as immutable objects that are stored in repositories 
which inform observers when the model has been updated. 

This makes it easier to separate the business logic from the application logic and the UI. This makes 
your software architecture easier to modify, extend, reuse, and maintain.    

### Ara3D.Services

Used for breaking software up into areas of responsibility called services, which are high-level classes 
that usually have one instance throughout the lifetime of an application. Services are stored within a Service 
Manager. 

Services are passed other services which they depend on in their constructor. This is a pattern known 
Dependency Injection. This is done in a straightforward and transparent manner without any kind of reflection,
code generation, or special framework support, while still providing the architectual benefits.  

## Collection Libraries

### Ara3D.Collections

*Undergoing significant refactoring* 

Primarily used today for `IArray<T>` and related functions which will be replaced throughout
by `IReadOnlyList<T>` for a better experience with existing libraries. 

## IO Libraries

### Ara3D.BFAST 

A library for efficiently reading and writing large named buffers from memory. A named buffer
is an array of bytes that is associated with a string.  

### Ara3D.G3D

A library for reading and writing geometry in the G3D format.

### Ara3D.IFCParser

A library for parsing [IFC](https://en.wikipedia.org/wiki/Industry_Foundation_Classes) entity stored within a STEP file.

### Ara3D.StepParser

A library for parsing [STEP](https://en.wikipedia.org/wiki/ISO_10303-21) files.

### Ara3D.MemoryMappedFile

A library for efficiently working with very large files as [memory mapped files](https://en.wikipedia.org/wiki/Memory-mapped_file). 

## Ara3D.Studio API

### Ara3D.Studio.Data

This library defines the classes that define the internal representation of rendering and scene data used by Ara3D.Studio. 
-->
