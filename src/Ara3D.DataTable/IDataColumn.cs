namespace Ara3D.DataTable;

public interface IDataColumn
{
    int ColumnIndex { get; }
    IDataDescriptor Descriptor { get; }
    IReadOnlyList<object> Values { get; }
}