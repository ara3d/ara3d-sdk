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
