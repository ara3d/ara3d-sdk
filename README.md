# 📚 Ara3D-SDK

[![NuGet Version](https://img.shields.io/nuget/v/Ara3D.SDK)](https://www.nuget.org/packages/Ara3D.SDK)

**Ara3D-SDK** is a powerful collection of open-source C# libraries for processing, transforming, and visualizing large-scale 3D models tailored for 
AEC (Architecture, Engineering, and Construction) workflows. 

Use it standalone, or to extend and customize the **Ara 3D Studio** desktop application.

Designed for high performance and scalability, the libraries handle massive 3D data sets in real-time. They're cross-platform and compatible with .NET 8.

## 📁 Repository Structure

- `artifacts/` - Built nuget packages 
- `data/` – Testing data
- `deprecated/` - Projects that are no longer being built or maintained
- `dist/` – Pre-built binaries and tools, including `Ara3D.Studio.exe`
- `examples/` – Sample applications and usage examples
- `plato-src/` – [Plato](https://github.com/ara3d/plato) source code for core numerical and geometry types and functions 
- `src/` – Core cross-platform C# libraries
- `ext/` - Libraries with additional dependencies, or for Windows only 
- `tests/` – NUnit projects for unit, regression, and developer testing
- `vendor/` - Required 3rd party libraries 
- `toolchain/` – Projects for parsing Plato and generating C# source code. 

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

All core libraries live under [`src/`](src/). See the [src/ project index](src/README.md) for a
full list grouped by category (geometry, I/O, BIM, architecture, Studio, utilities).

Install via the [Ara3D.SDK](https://www.nuget.org/packages/Ara3D.SDK) meta-package, or reference
individual projects from `src/`.

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
test.bat geometry      :: run only one area's tests (all | sdk | geometry | bim | devtools)
test.bat geometry fast :: run one area, skip Slow tests
test.bat sdk OpenVIM   :: run tests in an area matching a name substring
```

Let's build something amazing together 🚀

---

## 🐞 Issues and Feature Requests

Found a bug? Have a question? Want to suggest a feature for either the SDK or **Ara 3D Studio**?

👉 [Open an issue](https://github.com/ara3d/ara3d-sdk/issues) and let us know!

---

## 🔗 Related Projects

- [Ara3D.Plato](https://github.com/cdiggins/plato) – A domain-specific programming language for math and geometry
