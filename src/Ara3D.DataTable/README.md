# Ara3D.DataTable

This is a high-performance alternative to using `System.DataTable`. 
It is a set of interfaces for use with in-memory columnar databases or ECS (Entity Component System) architecture. 

Normally, you don't have to implement these interfaces yourself instead you work directly with an implementation 
like [Ara3D.NarwhalDB](https://github.com/ara3d/ara3d-sdk/tree/main/src/Ara3D.NarwhalDB).

## What is a Columnar Database

In traditional object-oriented approches large amounts of data are stored as collections of objects or structs. 
This is also referred to as the array-of-structs approach. This can be thought of as a collection of rows in a table. 

The alternative to lists of objects is a column-oriented layout, also known as struct-of-arrays. The column-oriented approach 
is more efficient for in-memory processing, and allows for extremely efficient packing of the columns.

For more information see:
* [Apache Parquet](https://parquet.apache.org/) - column oriented file format
* [Apache Arrow](https://arrow.apache.org/) - column oriented in memory database

## Design 

Ara3D.DataTable is primarily a simple set of interfaces, that allows a unified method of working with columnar data in memory 
or serialized from disk. 

It is the basis of other libraries that might do compression, (de-)serialization, queries, conversion to-from different formats,
and transporting data to/from relational databases or object-relational mapping (ORM). 

## Example Use Case

At **Ara 3D** we use this library to create efficient code for working with BIM data associated with large AEC models. 

## Interfaces

- `IDataSet` - A collection of `IDataTable` objects.
- `IDataTable` - A collection of `IDataColumn` objects.
- `IDataRow` - A collection of values for a single row (usually created on demand)
- `IDataColumn` - A buffer of unmanaged values for a single column.
- `IDataDescriptor` - A set of column descriptors (type, name) 

## Builder Helper Classes

DataTable comes with a set of builder classes to make it easy to construct simple object tables in memory incrementally that satisfy the interfaces. 
These classes are not optimized for performance, but instead designed for simple use cases. 

- `DataSetBuilder`
- `DataTableBuilder`
- `DataRowBuilder`
- `DataColumnBuilder`

No type checking is done on objects passed into table builder. 

## Motivation

This library has many parallels to `System.Data`, but there are several issues with using `System.Data` classes like `DataSet` and `DataTable` directly. 

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

## Dependencies

This project is built using .NET 8 and has no dependencies. 
