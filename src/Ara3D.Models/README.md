# Ara3D.Models

Scene and render model abstractions built on top of geometry and memory buffers.

## Overview

A **model** is a collection of triangular meshes plus **instances** (transform, mesh index,
material, and entity reference). This library defines the interfaces and concrete types used
to assemble scenes for rendering, serialization, and downstream processing.

Part of the [Ara3D.SDK](https://www.nuget.org/packages/Ara3D.SDK) meta-package.

## Key types

- `IModel3D` — meshes and instances with transform support
- `IBoundedModel3D` — adds per-mesh and per-instance bounds
- `IRenderableModel3D` — GPU-friendly vertex/index/mesh-slice/instance buffers
- `Model3D`, `Model3DBuilder` — concrete model construction
- `InstanceStruct`, `MeshSliceStruct`, `Material` — compact instance and draw data
- `RenderModelBfastSerializer` — serialize render models to [BFAST](../Ara3D.IO.BFAST)

## Dependencies

- [Ara3D.Geometry](../Ara3D.Geometry)
- [Ara3D.Memory](../Ara3D.Memory)
- [Ara3D.Collections](../Ara3D.Collections)
- [Ara3D.DataTable](../Ara3D.DataTable)
- [Ara3D.F8](../Ara3D.F8)
- [Ara3D.IO.BFAST](../Ara3D.IO.BFAST)
- [Ara3D.Logging](../Ara3D.Logging)
- [Ara3D.PropKit](../Ara3D.PropKit)

## Related projects

- [Ara3D.Studio.API](../Ara3D.Studio.API) — host application and asset interfaces
- [Ara3D.BimOpenSchema](../Ara3D.BimOpenSchema) — BIM data attached to models

## License

MIT — see [LICENSE](../../LICENSE).
