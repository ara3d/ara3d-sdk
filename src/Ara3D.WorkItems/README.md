# Ara3D.WorkItems

Simple background work-item queue.

## Overview

Schedules named actions on a background thread with cancellation support. Provides synchronized
and factory-based queue variants for serial or parallel execution of short tasks.

Part of the [Ara3D.SDK](https://www.nuget.org/packages/Ara3D.SDK) meta-package.

## Key types

- `WorkItem` — named action with auto-incrementing ID
- `IWorkItemQueue`, `WorkItemQueue` — queue interface and implementation
- `SynchronizedWorkItemQueue` — thread-safe wrapper
- `WorkItemQueueFactory`, `WorkItemQueueExtensions` — creation helpers
- `IWorkItemErrorHandler` — optional error callback

## Dependencies

None — .NET 8 only.

## Related projects

- [Ara3D.Logging](../Ara3D.Logging) — jobs with progress and cancellation
- [Ara3D.Utils](../Ara3D.Utils) — `SimpleWorkItemQueue` utility variant

## License

MIT — see [LICENSE](../../LICENSE).
