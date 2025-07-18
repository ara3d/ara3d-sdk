namespace Ara3D.DataTable;

public class DataColumnBuilder : IDataColumn
{
    public int ColumnIndex { get; }
    public IDataDescriptor Descriptor { get; }
    public List<object> Values { get; }
    public int Count => Values.Count;
    public object this[int index] => Values[index];
    public DataColumnBuilder(IDataDescriptor descriptor, int index)
    {
        Values = new();
        Descriptor = descriptor;
        ColumnIndex = index;
    }
    public Array AsArray() => Values.ToArray();
}