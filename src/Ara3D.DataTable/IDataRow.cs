namespace Ara3D.DataTable;

public interface IDataRow
{
    int RowIndex { get; }
    IDataTable DataTable { get; }
    IReadOnlyList<object> Values { get; }
}