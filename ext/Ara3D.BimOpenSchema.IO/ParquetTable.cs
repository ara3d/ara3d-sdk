using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ara3D.Collections;
using Ara3D.DataTable;
using Ara3D.Geometry;

namespace Ara3D.BimOpenSchema.IO;

public class ParquetTable<T> : IReadOnlyList<T>, IDataTable
{
    public string Name { get; }
    public IReadOnlyList<IDataRow> Rows { get;  }
    public IReadOnlyList<IDataColumn> Columns { get; }
    public object this[int column, int row] => _parquetColumns[column].Data.GetValue(row);
    private IReadOnlyList<Parquet.Data.DataColumn> _parquetColumns { get; }
    private Func<object[], T> _ctor;

    public ParquetTable(string name, IReadOnlyList<Parquet.Data.DataColumn> parquetColumns, Func<object[], T> ctor)
    {
        _parquetColumns = parquetColumns;
        Name = name;
        Count = _parquetColumns.Count > 0 ? _parquetColumns[0].NumValues : 0;
        _ctor = ctor;

        var fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (fields.Length != _parquetColumns.Count)
            throw new InvalidOperationException($"Field count ({fields.Length}) != column count ({_parquetColumns.Count}).");
        
        Rows = Count.MapRange(i => GetRow(i)).ToList();
        Columns = _parquetColumns.Select((c, i) => new ParquetColumnAdapter(c, i)).ToList();
    }

    public IDataRow GetRow(int n)
        => new DataRow(this, n);

    public T this[int n]
    {
        get
        {
            var vals = new object[_parquetColumns.Count];
            for (int i = 0; i < _parquetColumns.Count; i++)
                vals[i] = _parquetColumns[i].Data.GetValue(n);
            return _ctor(vals);
        }
    }

    public int Count { get; }

    public IEnumerator<T> GetEnumerator()
    {
        for (var i = 0; i < Count; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public override string ToString()
        => $"Table {Name}, {Columns.Count} Columns, {Count} Rows";
}