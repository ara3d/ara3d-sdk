namespace Ara3D.DataTable;

public interface IDataTable
{
    string Name { get; }
    IReadOnlyList<IDataRow> Rows { get; }
    IReadOnlyList<IDataColumn> Columns { get; }
}