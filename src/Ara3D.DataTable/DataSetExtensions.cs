using Ara3D.Collections;

namespace Ara3D.DataTable;

public static class DataSetExtensions
{
    public static DataRow GetRow(this IDataTable self, int rowIndex)
        => new(self, rowIndex);

    public static IReadOnlyList<object> GetRowValues(this IDataTable table, int row)
        => table.Columns.Select(c => c.Values[row]).ToList();

    public static ReadOnlyDataSet AddColumns(this IDataSet self, IDataTable table, params IDataColumn[] columns)
        => self.AddTable(new ReadOnlyDataTable(table.Name, table.Columns.Concat(columns)));

    public static ReadOnlyDataSet AddTable(this IDataSet self, IDataTable table)
        => new(self.Tables.Append(table).ToList());

    public static IDataTable? GetTable(this IDataSet self, string name)
        => self.Tables.FirstOrDefault(t => t.Name == name);

    public static IDataColumn? GetColumn(this IDataTable self, string name)
        => self.Columns.FirstOrDefault(c => c.Descriptor.Name == name);

    public static ReadOnlyDataSet AddColumnsToTable(this IDataSet self, string tableName,
        IReadOnlyList<IDataColumn> columns)
    {
        var table = self.GetTable(tableName);
        if (table == null)
        { 
            table = new ReadOnlyDataTable(tableName, columns);
            return self.AddTable(table);
        }

        return self.AddColumns(table, columns.ToArray());
    }
}