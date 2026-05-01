using System;
using Ara3D.DataTable;
using DataColumn = Parquet.Data.DataColumn;

namespace Ara3D.BimOpenSchema.IO;

public class ParquetColumnAdapter : IDataColumnWithValues
{
    public DataColumn Column;

    public ParquetColumnAdapter(DataColumn dc, int index)
    {
        Column = dc;
        ColumnIndex = index;
        Descriptor = new DataDescriptor(dc.Field.Name, dc.Field.ClrType);
        Count = Column.NumValues;
    }

    public int ColumnIndex { get; }
    public IDataDescriptor Descriptor { get; }
    public int Count { get; }
    public object this[int n] => Column.Data.GetValue(n);
    public Array Values => Column.Data;
}