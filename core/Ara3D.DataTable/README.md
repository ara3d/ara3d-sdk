# Ara3D.DataTable

This is a high-performance alternative to using System.DataTable. 
It is designed to be used with in-memory columnar databases or ECS (Entity Component System) architecture. 

Normally, you don't implement these interfaces yourself instead you work directly with an implementation like Ara3D.NarwhalDB. 

This project contains the core interfaces. 

## Motivation

There are several issues with using `System.Data` classes like `DataSet` and `DataTable` directly. 


| Issue                            | Impact in a real‑time viewer | 
| -------------------------------- | ---------------------------- |
| **Memory overhead**              | Each `DataRow` is a full object with boxing for value types, versioning arrays, and two hash tables |
| **GC pressure**                  | Per‑row allocations plus frequent `ColumnChanged` events drive Gen‑0/1 churn; stalls become visible at frame‑times < 16 ms. |
| **Weak typing**                  | *stringly‑typed* columns (`object` cells) kill inlining, require casts, and move many errors to runtime. |
| **No columnar access**           | Many analytics scan rows sequentially; CPUs love structure‑of‑arrays, not array‑of‑structures. |
| **Thread‑safety locks**          | `DataTable` internally synchronises; fine for editor‑thread, harmful if your physics or analysis jobs touch the same table. |
| **Limited relational modelling** | Cross‑table constraints exist, but traversing n‑level object graphs is verbose and slow compared with a proper ORM or ECS. |
| **Versioning you don’t need**    | Proposed/Current values cost memory even if you never call `BeginEdit`. Can’t switch them off. |
| **Complexity**				   | `DataTable` is a complex object with many properties and events. It’s hard to understand and use correctly. |

We created Ara3D.DataTable to handle large amounts of relational data in memory efficiently. 

## Interfaces

- `IDataSet` - A collection of `IDataTable` objects.
- `IDataTable` - A collection of `IDataColumn` objects.
- `IDataRow` - A collection of values for a single row (usually created )
- `IDataColumn` - A buffer of unmanaged values for a single column.
- `IDataSchema` - A set of column descriptors (type, name, validation) using `PropDescriptor` for run-time data descriptions.

## Dependencies

This project is built using .NET 8. 

It has dependencies on 

- **Ara3D.PropKit** - It uses the `PropDescriptor` class for run-time data descriptions. 
- **Ara3D.Memory** - It uses the `Buffer` class for memory management (column data)
