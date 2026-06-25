namespace Ara3D.DataTable;

public class SparseColumn : IDataColumn, IDataDescriptor
{
    public SparseColumn(string name, Type type, int rows, int columnIndex, object defaultValue)
    {
        Name = name;
        Type = type;
        Count = rows;
        ColumnIndex = columnIndex;
        DefaultValue = defaultValue;
    }
    public Dictionary<int, object> Dictionary { get; } = [];
    public int ColumnIndex { get; }
    public IDataDescriptor Descriptor => this;
    public Type Type { get; }
    public string Name { get; }
    public int Count { get; }
    public object DefaultValue { get; set; }
    public object this[int n] => Dictionary.GetValueOrDefault(n, DefaultValue);
}