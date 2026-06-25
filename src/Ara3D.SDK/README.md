# Ara3D.SDK

[![NuGet Version](https://img.shields.io/nuget/v/Ara3D.SDK)](https://www.nuget.org/packages/Ara3D.SDK)

Meta-package that bundles the core Ara 3D SDK libraries into a single NuGet reference.

## Overview

This project contains no source code. It references the individual library projects under
`src/` and packs their assemblies into one `Ara3D.SDK` NuGet package via
`IncludeReferencedProjects`.

For documentation of each bundled library, see the [src/ project index](../README.md).

## Bundled libraries

Ara3D.Collections, Ara3D.DataTable, Ara3D.Domo, Ara3D.Events, Ara3D.Geometry,
Ara3D.IO.BFAST, Ara3D.IO.G3D, Ara3D.IO.GltfExporter, Ara3D.IO.PLY, Ara3D.IO.StepParser,
Ara3D.IO.VIM, Ara3D.Logging, Ara3D.Memory, Ara3D.Models, Ara3D.PropKit, Ara3D.ScriptService,
Ara3D.Services, Ara3D.Studio.API, Ara3D.Utils, Ara3D.Utils.Roslyn, Ara3D.WorkItems.

## Not included

These live in `src/` but are referenced separately:

- [Ara3D.BimOpenSchema](../Ara3D.BimOpenSchema) — use `Ara3D.BimOpenSchema.IO` from `ext/`
- [Ara3D.IO.SharpGLTF](../Ara3D.IO.SharpGLTF) — glTF import/manipulation
- [Ara3D.IO.GeoJson](../Ara3D.IO.GeoJson) — GeoJSON / IMDF

## External dependencies

- Microsoft.CodeAnalysis.CSharp 4.8.0
- Microsoft.DiaSymReader.Native 1.7.0
- System.Memory 4.6.0

## License

MIT — see [LICENSE](../../LICENSE).
