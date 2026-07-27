namespace Ara3D.DataTable;

public class DataSetBuilder : IDataSet
{
    public DataTableBuilder AddTable(string name)
    {
        var r = new DataTableBuilder(name);
        _tableBuilders.Add(r);
        return r;
    }

    public DataTableBuilder AddTable(IDataTable table)
    {
        var r = AddTable(table.Name);
        foreach (var col in table.Columns)
            r.AddColumn(col);
        return r;
    }

    private readonly List<DataTableBuilder> _tableBuilders = new();
    public IReadOnlyList<IDataTable> Tables => _tableBuilders.Select(b => b.Build()).ToList();
}