# Ara3D.Studio.API

Public API for extending and scripting **Ara 3D Studio**.

## Overview

This library defines the contracts between the Studio host application and plug-ins, loaders,
generators, exporters, and flow-graph nodes. Implement these interfaces to load custom file
formats, export geometry, or participate in the visual programming graph.

Part of the [Ara3D.SDK](https://www.nuget.org/packages/Ara3D.SDK) meta-package.

## Key types

### Host

- `IHostApplication` — logger, UI refresh, camera animation, asset/scene I/O

### Assets and loading

- `IAsset`, `IAssetSource` — loaded data with optional attachments (e.g. BIM metadata)
- `ILoader` — load a file path into an asset
- `RenderableAsset`, `RenderSettings`, `CameraState` — rendering context

### Flow graph

- `FlowObject`, `FlowAttribute`, `FlowTypes` — typed ports and node metadata
- `EvalContext` — evaluation context passed through the graph
- `IGenerator`, `IModifier`, `IExporter` — scripted graph node roles

### Commands

- `SimpleCommand` — lightweight command wrapper
- Attributes in `Attributes.cs` — metadata for discovery and UI

## Dependencies

- [Ara3D.Logging](../Ara3D.Logging)
- [Ara3D.Models](../Ara3D.Models)

## Related projects

- [Ara3D.ScriptService](../Ara3D.ScriptService) — legacy Roslyn scripting (Bowerbird)
- [Ara3D.PropKit](../Ara3D.PropKit) — property descriptors for UI binding

## License

MIT — see [LICENSE](../../LICENSE).
