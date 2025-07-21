using System.Diagnostics;
using Ara3D.Collections;

namespace Ara3D.DataTable
{
    public class DataTableBuilder : IDataTable
    {
        public DataTableBuilder(string tableName)
            => Name = tableName;

        public string Name { get; }
        private int _NumRows = 0;

        public IReadOnlyList<IDataRow> Rows => _NumRows.Select(this.GetRow);
        public IReadOnlyList<IDataColumn> Columns => ColumnBuilders.Cast<IDataColumn>().ToList();
        public List<DataColumnBuilder> ColumnBuilders { get; } = new();

        public void AddRow(IReadOnlyList<object> values)
        {
            if (values.Count != Columns.Count)
                throw new Exception($"Number of values in row ({values.Count}) must match number of columns {Columns.Count}");
            for (var i = 0; i < ColumnBuilders.Count; i++)
                ColumnBuilders[i].Values.Add(values[i]);
            _NumRows++;
        }

        public DataColumnBuilder AddColumn(string name, Type type)
            => AddColumn([], name, type);

        public DataColumnBuilder AddColumn(IReadOnlyList<object> values, string name, Type type)
        {
            if (Columns.Count != 0)
            {
                if (values.Count != _NumRows)
                    throw new Exception($"Number of values in column {values.Count} must match number of rows {_NumRows}");
            }

            var descriptor = new DataDescriptor(name, type, Columns.Count);
            var r = new DataColumnBuilder(descriptor, descriptor.Index);
            r.Values.AddRange(values);
            ColumnBuilders.Add(r);
            if (Columns.Count == 1)
                _NumRows = values.Count;

            Debug.Assert(Columns.All(c => c.Count == _NumRows));
            return r;
        }

        public object this[int column, int row] 
            => Columns[column][row];
    }
}
