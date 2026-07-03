# Ara3D.Events

Thread-safe publish/subscribe event bus.

## Overview

Provides a decoupled messaging layer where publishers and subscribers communicate through
`IEventBus` without the resource-leak risks of C# multicast events. Used by
[Ara3D.Services](../Ara3D.Services) and repository change propagation.

Part of the [Ara3D.SDK](https://www.nuget.org/packages/Ara3D.SDK) meta-package.

## Key types

- `IEvent`, `IEventBus`, `EventBus` — event types and bus implementation
- `ISubscriber`, `Subscriber` — typed subscription handling
- `IEventErrorHandler` — optional error handling for event dispatch

## Dependencies

None — .NET 8 only.

## Related projects

- [Ara3D.Services](../Ara3D.Services) — wires event bus into application services

## License

MIT — see [LICENSE](../../LICENSE).
