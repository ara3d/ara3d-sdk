using Ara3D.Collections;

namespace Ara3D.DataTable;

public class ReadOnlyDataTable : IDataTable
{
    public string Name { get; }
    public IReadOnlyList<IDataRow> Rows { get; }
    public IReadOnlyList<IDataColumn> Columns { get; }

    public ReadOnlyDataTable(string name, IReadOnlyList<IDataColumn> dataColumns)
    {
        Name = name;
        Columns = dataColumns;
        Rows = dataColumns.Count > 0 
            ? dataColumns[0].Count.Select(i => new DataRow(this, i))
            : [];
    }

    public object this[int column, int row] 
        => Columns[column][row];
}