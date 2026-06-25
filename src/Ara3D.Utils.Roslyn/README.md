# Ara3D.Utils.Roslyn

Roslyn-based C# compilation utilities.

## Overview

Wraps `Microsoft.CodeAnalysis` to compile C# source from files or directories, with optional
directory watching for live recompilation. Used by the legacy
[Ara3D.ScriptService](../Ara3D.ScriptService).

Part of the [Ara3D.SDK](https://www.nuget.org/packages/Ara3D.SDK) meta-package.

## Key types

- `Compiler`, `CompilerOptions`, `CompilerInput`, `CompilerOutput` — compile pipeline
- `DirectoryWatchingCompiler` — watches a scripts folder and recompiles on change
- `ParsedSourceFile`, `ParsedCompilerInput` — parsed input models
- `RoslynUtils`, `RoslynAnalysisUtils` — helper methods

## Dependencies

- Microsoft.CodeAnalysis.CSharp
- [Ara3D.Logging](../Ara3D.Logging)

## Related projects

- [Ara3D.ScriptService](../Ara3D.ScriptService) — scripting service consumer
- [Ara3D.Studio.API](../Ara3D.Studio.API) — plug-in and scripted component interfaces

## License

MIT — see [LICENSE](../../LICENSE).
