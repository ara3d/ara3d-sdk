namespace Ara3D.DataTable;

public class DataDescriptor : IDataDescriptor
{
    public string Name { get; }
    public Type Type { get; }
    public int Index { get; }

    public DataDescriptor(string name, Type type, int index)
    {
        Name = name;
        Type = type;
        Index = index;
    }
}