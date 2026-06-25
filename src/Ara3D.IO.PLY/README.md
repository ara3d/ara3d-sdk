# Ara3D.IO.PLY

Import and export of [PLY](https://en.wikipedia.org/wiki/PLY_(file_format)) mesh files.

## Overview

Reads ASCII and binary PLY into `TriangleMesh3D` and writes meshes back to PLY. Typed buffer
helpers map PLY property types to strongly typed geometry attributes.

Part of the [Ara3D.SDK](https://www.nuget.org/packages/Ara3D.SDK) meta-package.

## Key types

- `PlyImporter` — load PLY text into geometry
- `PlyExporter` — write meshes to PLY
- `PlyBuffer`, `IPlyBuffer` — typed column buffers for PLY properties

## Dependencies

- [Ara3D.Geometry](../Ara3D.Geometry)
- [Ara3D.Memory](../Ara3D.Memory)

## Related projects

- [Ara3D.IO.G3D](../Ara3D.IO.G3D) — alternative binary geometry format
- [Ara3D.Models](../Ara3D.Models) — scene assembly from imported meshes

## License

MIT — see [LICENSE](../../LICENSE).
