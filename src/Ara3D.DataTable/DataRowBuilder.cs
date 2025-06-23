namespace Ara3D.DataTable;

public class DataRowBuilder : IDataRow
{
    public int RowIndex { get; }
    public IDataTable DataTable => _dataTableBuilder;
    public IReadOnlyList<object> Values => _dataTableBuilder.GetRowValues(RowIndex);
    private readonly DataTableBuilder _dataTableBuilder;
    public DataRowBuilder(DataTableBuilder dataTableBuilder, int rowIndex)
    {
        _dataTableBuilder = dataTableBuilder;
        RowIndex = rowIndex;
    }
}