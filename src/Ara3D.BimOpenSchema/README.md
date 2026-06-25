# Ara3D.BimOpenSchema

Object model for **BIM Open Schema (BOS)** — a standardized, column-oriented representation of
Building Information Modeling data.

## Overview

BOS expresses federated BIM data as a set of tables (entities, parameters, relations, geometry,
and shared string/number pools). It is optimized for compact storage and fast loading into
analytical tools (Parquet files, DuckDB, or in-memory C# workflows), not for ad-hoc querying.

A typical workflow is to ingest BOS into DuckDB and build denormalized views with SQL for a
specific use case.

Serialization to disk is provided by [`Ara3D.BimOpenSchema.IO`](../../ext/Ara3D.BimOpenSchema.IO).
This project is **not** included in the [Ara3D.SDK](https://www.nuget.org/packages/Ara3D.SDK) meta-package.

Current schema version: **0.3** (`Manifest.CurrentVersion`).

## Key types

- `IBimData` — root data container (manifest, descriptors, entities, geometry, …)
- `Manifest` — version, generator application, export options
- `Entity`, `Parameter`, `ParameterDescriptor` — BIM elements and their properties
- `EntityRelation` — relationships between entities
- `BimGeometry`, `BimGeometryBuilder` — tessellated geometry linked to entities
- `BimDataBuilder` — construct `IBimData` in memory

## Dependencies

- [Ara3D.DataTable](../Ara3D.DataTable)
- [Ara3D.Geometry](../Ara3D.Geometry)
- [Ara3D.Models](../Ara3D.Models)

## Related projects

- [Ara3D.IO.VIM](../Ara3D.IO.VIM) — VIM binary BIM format
- [Ara3D.DataTable](../Ara3D.DataTable) — columnar data interfaces

## License

MIT — see [LICENSE](../../LICENSE).
