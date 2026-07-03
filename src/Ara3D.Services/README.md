# Ara3D.Services

[![NuGet Version](https://img.shields.io/nuget/v/Ara3D.Services)](https://www.nuget.org/packages/Ara3D.Services)

Lightweight application infrastructure: service registration and an event bus.

* `IService` — marker for application services
* `IServiceManager` — holds services and an `IEventBus`

Domo-backed model/repository service helpers (`BaseService`, `LoggingService`, etc.)
live in [`deprecated/wip/Ara3D.Domo`](../../deprecated/wip/Ara3D.Domo) alongside
[`Ara3D.Domo`](../../deprecated/wip/Ara3D.Domo).

## Related projects

* [`Ara3D.Events`](../Ara3D.Events) — thread-safe event bus

## License

MIT — see [LICENSE](../../LICENSE).
