# Ara3D.Domo

Model and repository pattern for observable application state.

## Overview

Domo provides identity-stable wrappers around data objects (`IModel`), keyed collections
(`IRepository`), and change notification when models are added, removed, or updated. It is
the state-management foundation used by [Ara3D.Services](../Ara3D.Services).

Part of the [Ara3D.SDK](https://www.nuget.org/packages/Ara3D.SDK) meta-package.

## Key types

- `IModel` / `Model` — wrapper with stable identity and change events
- `IRepository` / `Repository` — keyed model store
- `RepositoryManager` — manages multiple repositories
- `RepositoryChangeType`, `RepositoryChangeArgs` — change notification payloads

## Dependencies

None — targets .NET Standard 2.0 for broad compatibility.

## Related projects

- [Ara3D.Events](../Ara3D.Events) — event bus used alongside repositories
- [Ara3D.Services](../Ara3D.Services) — service layer built on Domo

## License

MIT — see [LICENSE](../../LICENSE).
