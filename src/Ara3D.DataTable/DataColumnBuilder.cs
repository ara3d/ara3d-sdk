namespace Ara3D.DataTable;

public class DataColumnBuilder : IDataColumn
{
    public int ColumnIndex => Descriptor.Index;
    IDataDescriptor IDataColumn.Descriptor => Descriptor;
    IReadOnlyList<object> IDataColumn.Values => Values;
    public DataDescriptor Descriptor { get; }
    public List<object> Values { get; } 

    public DataColumnBuilder(IReadOnlyList<object> values, DataDescriptor descriptor)
    {
        Values = new List<object>(values);
        Descriptor = descriptor;
    }
}