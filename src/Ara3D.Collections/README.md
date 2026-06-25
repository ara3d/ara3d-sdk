# Ara3D.Collections

[![NuGet Version](https://img.shields.io/nuget/v/Ara3D.Collections)](https://www.nuget.org/packages/Ara3D.Collections)

Optimized collection types and LINQ helpers for `IReadOnlyList<T>`.

## Overview

Standard .NET collections and LINQ are oriented around mutable `IList<T>` and `IEnumerable<T>`.
This library provides immutable-friendly list views, multi-dimensional read-only lists, and
utilities that avoid unnecessary allocations when working with large read-only data sets.

Part of the [Ara3D.SDK](https://www.nuget.org/packages/Ara3D.SDK) meta-package. Also published
as a standalone NuGet package.

## Key types

- `ReadOnlyList<T>`, `ReadOnlyList2D<T>`, `ReadOnlyList3D<T>` — zero-copy views over arrays
- `IReadOnlyList2D<T>`, `IReadOnlyList3D<T>` — multi-dimensional list interfaces
- `CompressedSparseRow` — CSR sparse matrix storage
- `LinqArray` — array-backed LINQ operations
- `EmptyList`, `IntegerRange` — small utility collections

Experimental types live under `wip/` (trees, stacks, lookups).

## Dependencies

None — .NET 8 only.

## Related projects

- [Ara3D.Geometry](../Ara3D.Geometry) — uses read-only list views for mesh data
- [Ara3D.DataTable](../Ara3D.DataTable) — columnar data built on similar principles

## License

MIT — see [LICENSE](../../LICENSE).
