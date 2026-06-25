# Ara3D.F8

SIMD math wrappers for 8-wide float operations (AVX).

## Overview

Modern x64 CPUs expose AVX instructions that operate on eight `float` values at once via
`Vector256<float>`. This library wraps those intrinsics in easier-to-use types for geometry
hot paths.

Part of the [Ara3D.SDK](https://www.nuget.org/packages/Ara3D.SDK) meta-package.

## Key types

- `f8` — eight-wide float vector with basic math operations
- `Vector3x8` — eight `Vector3` values processed in parallel
- `d4` — four-wide double vector
- `BoundsUtil` — bounds helpers using SIMD

## Dependencies

None — .NET 8 only (uses `System.Runtime.Intrinsics`).

## Related projects

- [Ara3D.Models](../Ara3D.Models) — uses F8 for render buffer processing
- [Ara3D.Geometry](../Ara3D.Geometry) — geometry algorithms that benefit from SIMD

## License

MIT — see [LICENSE](../../LICENSE).
