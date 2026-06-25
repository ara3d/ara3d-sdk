# Ara3D.IO.StepParser

Low-level parser for ISO STEP (`.stp`/`.step`) files.

## Overview

Parses STEP physical files into tokens, entity definitions, and a graph structure. Uses
unsafe pointer access over aligned memory buffers for performance on large CAD exports.

Part of the [Ara3D.SDK](https://www.nuget.org/packages/Ara3D.SDK) meta-package.

## Key types

- `StepDocument` — owns file buffer, header, tokens, and definitions
- `StepTokenizer`, `StepToken`, `StepTokenType` — lexical analysis
- `StepDefinition` — parsed STEP entity instance
- `StepGraph` — navigable graph over definitions
- `StepHeader` — FILE_DESCRIPTION / FILE_NAME header section

## Dependencies

- [Ara3D.Memory](../Ara3D.Memory)
- [Ara3D.Logging](../Ara3D.Logging)
- [Ara3D.Utils](../Ara3D.Utils)

## Related projects

- [Ara3D.Geometry](../Ara3D.Geometry) — downstream mesh/geometry processing

## License

MIT — see [LICENSE](../../LICENSE).
