# Ara3D.Utils

General-purpose utility library used across the Ara 3D SDK.

## Overview

A collection of small, focused helpers for paths, serialization, reflection, threading,
statistics, HTML/JSON/XML, and process management. Prefer adding domain-specific code to the
relevant library rather than here.

Part of the [Ara3D.SDK](https://www.nuget.org/packages/Ara3D.SDK) meta-package.

## Key areas

- **Paths and files** — `FilePath`, `DirectoryPath`, `PathUtil`, `StreamUtil`, `ZipUtil`
- **Serialization** — `JsonUtil`, `XmlUtil`, `JsonStringBuilder`
- **Reflection** — `ReflectionUtils`, `ReflectionGetterSetterUtil`, `AssemblyUtil`
- **Collections** — `BiDictionary`, `MultiDictionary`, `CountedSet`, `TreeUtil`
- **Concurrency** — `Parallelizer`, `Synchronizer`, `ThreadUtil`, `TaskUtil`
- **Statistics** — `ScalarStatistics`, `ScalarWeightedStatistics`
- **Web** — `WebUtil`, `WebServer`, `HttpRequest`, `HtmlBuilder`
- **Development** — `ApplicationFolders`, `DevelopmentFolders`, `ProfilingUtil`

## Dependencies

None — .NET 8 only.

## Related projects

- [Ara3D.Utils.Roslyn](Ara3D.Utils.Roslyn) — Roslyn-specific compilation utilities

## License

MIT — see [LICENSE](../../LICENSE).
