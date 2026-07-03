# Ara3D.SDK

[![NuGet Version](https://img.shields.io/nuget/v/Ara3D.SDK)](https://www.nuget.org/packages/Ara3D.SDK)

Windows convenience meta-package that pulls in the full supported SDK stack.

## Overview

This project contains no source code. It references the tier meta-packages under `src/` plus
Studio API and WPF helpers, and packs their assemblies into one `Ara3D.SDK` NuGet package.

## Included meta-packages

- [Ara3D.SDK.Core](../Ara3D.SDK.Core) — cross-platform foundation
- [Ara3D.SDK.Geometry](../Ara3D.SDK.Geometry) — geometry and modeling
- [Ara3D.SDK.IO](../Ara3D.SDK.IO) — file formats, BOS, and IFC conversion

## Also included directly

- [Ara3D.Studio.API](../Ara3D.Studio.API) — Studio flow graph and modifier pipeline
- [Ara3D.Utils.Wpf](../../ext/Ara3D.Utils.Wpf) — WPF helpers

## Not included

Plug-ins, apps, and optional integrations are repo-only:

- [`plugins/`](../../plugins/) — Bowerbird, Revit add-ins
- [`apps/`](../../apps/) — BOS Browser
- [`integrations/`](../../integrations/) — Assimp loader

For cross-platform consumers, use `Ara3D.SDK.Core` or `Ara3D.SDK.Geometry` instead of this package.

## License

MIT — see [LICENSE](../../LICENSE).
