namespace Ara3D.DataTable;

public class DataTable : IDataTable
{
    public string Name { get; }
    public IReadOnlyList<IDataRow> Rows { get; } = [];
    public IReadOnlyList<IDataColumn> Columns { get; } = [];
    public Func<int, int, object> Func { get; } 

    public DataTable(string name, IReadOnlyList<IDataColumn> columns, Func<int, int, object>? func)
    {
        Name = name;
        Func = func;
        if (Func == null)
            Func = (int colIndex, int rowIndex) => columns[colIndex][rowIndex];
        Columns = columns;
        if (Columns.Count == 0) return;
        var n = Columns[0].Count;
        if (Columns.Any(c => c.Count != n))
            throw new Exception("All columns must have the same number of values");
        Rows = Enumerable.Range(0, n).Select(i => new DataRow(this, i)).ToList();
    }

    public object this[int column, int row] 
        => Func(column, row);
}
