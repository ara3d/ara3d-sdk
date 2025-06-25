using System.Diagnostics;
using Ara3D.Collections;

namespace Ara3D.DataTable
{
    public class DataTableBuilder : IDataTable
    {
        public string Name { get; }
        private int _NumRows = 0;

        public IReadOnlyList<IDataRow> Rows => _NumRows.Select(this.GetRow);
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

        public DataColumnBuilder AddColumn(IReadOnlyList<object> values, string name, Type type)
        {
            if (Columns.Count != 0)
            {
                if (values.Count != _NumRows)
                    throw new Exception($"Number of values in column {values.Count} must match number of rows {_NumRows}");
            }

            var descriptor = new DataDescriptor(name, type, Columns.Count);
            var r = new DataColumnBuilder(values, descriptor);
            _columnBuilders.Add(r);
            if (Columns.Count == 1)
                _NumRows = values.Count;

            Debug.Assert(Columns.All(c => c.Values.Count == _NumRows));
            return r;
        }

        public DataTableBuilder(string tableName)
        {
            Name = tableName;
        }
    }
}
