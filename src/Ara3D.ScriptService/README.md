# Ara3D.ScriptService

> **Legacy** — used by Bowerbird only. Planned to move out of the SDK (`TODO: move to bowerbird`).

Roslyn-based scripting service that watches a scripts folder, recompiles on change, and
exposes discovered script types to the application.

## Overview

`ScriptingService` integrates [Ara3D.Utils.Roslyn](../Ara3D.Utils.Roslyn),
[Ara3D.Services](../Ara3D.Services), and [Ara3D.Studio.API](../Ara3D.Studio.API) for live C#
script editing in Bowerbird workflows.

Part of the [Ara3D.SDK](https://www.nuget.org/packages/Ara3D.SDK) meta-package, but new code
should not depend on this library.

## Key types

- `ScriptingService`, `IScriptingService` — main service
- `ScriptingOptions` — scripts and libraries folder paths
- `Script`, `ScriptingDataModel` — discovered script metadata

## Dependencies

- [Ara3D.PropKit](../Ara3D.PropKit)
- [Ara3D.Services](../Ara3D.Services)
- [Ara3D.Studio.API](../Ara3D.Studio.API)
- [Ara3D.Utils.Roslyn](../Ara3D.Utils.Roslyn)

## License

MIT — see [LICENSE](../../LICENSE).
