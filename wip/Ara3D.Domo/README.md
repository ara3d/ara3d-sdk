# Ara3D.Domo (WIP)

Model and repository pattern for observable application state.

**Status:** Work in progress — moved out of `src/`; not shipped in the
[Ara3D.SDK](https://www.nuget.org/packages/Ara3D.SDK) meta-package.

## Overview

Domo provides identity-stable wrappers around data objects (`IModel`), keyed collections
(`IRepository`), and change notification when models are added, removed, or updated.
Domo-backed service helpers live under `Services/` in this project.

## Key types

- `IModel` / `Model` — wrapper with stable identity and change events
- `IRepository` / `Repository` — keyed model store
- `RepositoryManager` — manages multiple repositories
- `RepositoryChangeType`, `RepositoryChangeArgs` — change notification payloads

## Dependencies

- [Ara3D.Events](../../../src/Ara3D.Events)
- [Ara3D.Logging](../../../src/Ara3D.Logging)
- [Ara3D.Services](../../../src/Ara3D.Services)
- [Ara3D.Utils](../../../src/Ara3D.Utils)

Targets .NET 8.

## Related projects

- [Ara3D.Services](../../../src/Ara3D.Services) — slim service host (in `src/`)
- [Ara3D.Events](../../../src/Ara3D.Events) — event bus used alongside repositories

## License

MIT — see [LICENSE](../../../LICENSE).
