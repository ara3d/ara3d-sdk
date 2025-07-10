namespace Ara3D.DataTable;

public class DataRow : IDataRow
{
    public int RowIndex { get; }
    public IDataTable DataTable { get; }
    public IReadOnlyList<object> Values => DataTable.GetRowValues(RowIndex);
    public DataRow(IDataTable table, int rowIndex)
    {
        DataTable = table;
        RowIndex = rowIndex;
    }

    public object this[int n] => Values[n];
}