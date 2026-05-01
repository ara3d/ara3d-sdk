namespace Ara3D.DataTable;

public class DataColumn : IDataColumn
{
    public int ColumnIndex { get; }
    public IDataTable Table { get; }
    public IDataDescriptor Descriptor { get; }
    public int Count => Table.Rows.Count;
    public object this[int index] => Table[ColumnIndex, index];

    public DataColumn(IDataTable table, IDataDescriptor descriptor, int index)
    {
        Table = table;
        Descriptor = descriptor;
        ColumnIndex = index;
    }

    public override string ToString()
        => $"{Descriptor.Name}:{Descriptor.Type}[{Count}]";
}