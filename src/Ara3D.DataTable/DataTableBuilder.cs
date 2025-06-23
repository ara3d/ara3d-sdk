using System.Diagnostics;

namespace Ara3D.DataTable
{
    public class DataTableBuilder : IDataTable
    {
        public int TableIndex { get; }
        public string Name { get; }
        IDataSet IDataTable.DataSet => DataSet;
        public DataSetBuilder DataSet { get; }
        private int _NumRows = 0;

        public IReadOnlyList<IDataRow> Rows => Enumerable.Range(0, _NumRows).Select(GetRow).ToList();
        public IReadOnlyList<IDataColumn> Columns => _columnBuilders.Cast<IDataColumn>().ToList();
        private List<DataColumnBuilder> _columnBuilders { get; } = new();

        public void AddRow(IReadOnlyList<object> values)
        {
            if (values.Count != Columns.Count)
                throw new Exception($"Number of values in row ({values.Count}) must match number of columns {Columns.Count}");
            for (var i = 0; i < _columnBuilders.Count; i++)
                _columnBuilders[i].Values.Add(values[i]);
            _NumRows++;
        }

        public void AddColumn(IReadOnlyList<object> values, string name, Type type)
        {
            if (Columns.Count != 0)
            {
                if (values.Count != _NumRows)
                    throw new Exception($"Number of values in column {values.Count} must match number of rows {_NumRows}");
            }

            var descriptor = new DataDescriptor(name, type, Columns.Count);
            _columnBuilders.Add(new DataColumnBuilder(values, descriptor));
            if (Columns.Count == 1)
                _NumRows = values.Count;

            Debug.Assert(Columns.All(c => c.Values.Count == _NumRows));
        }

        public IDataRow GetRow(int rowIndex)
            => new DataRowBuilder(this, rowIndex);

        public List<object> GetRowValues(int row)
            => _columnBuilders.Select(c => c.Values[row]).ToList();

        public DataTableBuilder(DataSetBuilder dataSet, int tableIndex, string tableName)
        {
            DataSet = dataSet;
            TableIndex = tableIndex;
            Name = tableName;
        }
    }
}
