namespace Ara3D.DataTable;

public class DataColumnWithValues : IDataColumnWithValues
{
    public Array Values { get; }
    public int ColumnIndex { get; }
    public IDataDescriptor Descriptor { get; }
    public int Count => Values.Length;
    public object this[int index] => Values.GetValue(index);

    public DataColumnWithValues(IDataDescriptor descriptor, int index, Array values)
    {
        Descriptor = descriptor;
        ColumnIndex = index;
        Values = values;
    }

    public static DataColumnWithValues Create<T>(string name, T[] values, int index)
        => new(new DataDescriptor(name, typeof(T)), index, values);
}