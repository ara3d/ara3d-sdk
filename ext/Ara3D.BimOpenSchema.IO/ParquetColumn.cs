using System;
using System.Collections;
using System.Collections.Generic;
using Ara3D.DataTable;
using DataColumn = Parquet.Data.DataColumn;

namespace Ara3D.BimOpenSchema.IO;

/// <summary>
/// Wraps a Parquet Data Column so that it is typesafe, and implements
/// both IDataColumn and IDataTable
/// </summary>
/// <typeparam name="T"></typeparam>
public class ParquetColumn<T> : IReadOnlyList<T>, IDataColumn, IDataTable
{
    public readonly T[] Values;
    private ReadOnlyListSingleColumnDataAdapter<T> _adapter;

    public ParquetColumn(DataColumn column)
    {
        Values = column.Data as T[];
        if (Values == null)
            throw new Exception($"Column has type {column.Data.GetType()} not {typeof(T[])}");
        _adapter = new ReadOnlyListSingleColumnDataAdapter<T>(column.Field.Name, Values);
    }

    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>)Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Values.GetEnumerator();
    public int ColumnIndex => _adapter.ColumnIndex;
    public IDataDescriptor Descriptor => _adapter.Descriptor;
    public int Count => Values.Length;
    object IDataColumn.this[int n] => Values[n];
    public Array AsArray() => Values;
    public T this[int index] => Values[index];
    public string Name => _adapter.Name;
    public IReadOnlyList<IDataRow> Rows => _adapter.Rows;
    public IReadOnlyList<IDataColumn> Columns => _adapter.Columns;
    public object this[int column, int row] => _adapter[column, row];
    public override string ToString() => $"{Name}:{Descriptor.Type}";
}