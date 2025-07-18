namespace Ara3D.DataTable;

public interface IDataColumn
{
    int ColumnIndex { get; }
    IDataDescriptor Descriptor { get; }
    int Count { get; }
    object this[int n] { get; }
    Array AsArray();
}