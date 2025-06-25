namespace Ara3D.DataTable;

public class DataSetBuilder : IDataSet
{
    public DataTableBuilder AddTable(string name)
    {
        var r = new DataTableBuilder(name);
        _tableBuilders.Add(r);
        return r;
    }

    private readonly List<DataTableBuilder> _tableBuilders = new();
    public IReadOnlyList<IDataTable> Tables => _tableBuilders.Cast<IDataTable>().ToList();
}