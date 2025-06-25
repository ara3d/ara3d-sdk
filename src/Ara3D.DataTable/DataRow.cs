namespace Ara3D.DataTable;

public class DataRow : IDataRow
{
    public int RowIndex { get; }
    public IDataTable DataTable { get; }
    public IReadOnlyList<object> Values => DataTable.GetRowValues(RowIndex);
    private readonly DataTableBuilder _dataTableBuilder;
    public DataRow(IDataTable table, int rowIndex)
    {
        DataTable = table;
        RowIndex = rowIndex;
    }
}