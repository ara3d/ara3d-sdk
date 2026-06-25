# Ara3D.Geometry

Core geometry, mesh, and spatial algorithms for the Ara 3D SDK.

## Overview

This library provides triangle and quad meshes, half-edge topology, signed distance fields,
voxelization, spatial acceleration structures, and mesh processing utilities. It is used
throughout the SDK for modeling, analysis, and export.

Math types (`Vector3`, `Matrix4x4`, `TriangleMesh3D`, etc.) come from shared
[`Plato.Generated`](../Plato.Generated) and [`Plato.Intrinsics`](../Plato.Intrinsics) code
imported into this project.

Part of the [Ara3D.SDK](https://www.nuget.org/packages/Ara3D.SDK) meta-package.

## Key types

- `TriangleMesh3D`, `QuadMesh3D` — mesh data structures and builders
- `Topology`, `TopoFace`, `TopoHalfEdge` — half-edge mesh topology
- `AABBTree` — axis-aligned bounding box tree for spatial queries
- `MarchingCubes`, `Sdf3D`, `VoxelizedField` — implicit surfaces and voxel grids
- `PolygonTriangulator`, `DelaunayTriangulator` — 2D/3D tessellation
- `StlExporter`, `ObjExporter` — common mesh export formats
- `IsotropicRemesher`, `VertexWelder`, `MeshModifiers` — mesh processing

## Dependencies

- [Ara3D.Collections](../Ara3D.Collections)
- [Ara3D.Memory](../Ara3D.Memory)
- [Ara3D.Utils](../Ara3D.Utils)

## Related projects

- [Ara3D.Models](../Ara3D.Models) — scene graph and render-ready model buffers
- [Ara3D.IO.G3D](../Ara3D.IO.G3D) — G3D geometry file format
- [Ara3D.IO.PLY](../Ara3D.IO.PLY) — PLY import/export

## License

MIT — see [LICENSE](../../LICENSE).
