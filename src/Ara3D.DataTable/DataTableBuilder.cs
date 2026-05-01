using Ara3D.Collections;

namespace Ara3D.DataTable
{
    public class DataTableBuilder 
    {
        public DataTableBuilder(string tableName)
            => Name = tableName;

        public string Name { get; }
        public List<DataRow> Rows { get; }
        public List<DataColumn> Columns { get; }
        
        private List<IDataColumn> _columns { get; } = new();
        
        public IDataColumn AddColumn(IDataColumn col)
        {
            _columns.Add(col);
            return col;
        }

        public IDataColumn AddColumn<T>(T[] values, string name)
            => AddColumn(values, name, typeof(T));

        public IDataColumn AddColumn(Array values, string name, Type type)
        {
            var descriptor = new DataDescriptor(name, type);
            var column = new DataColumnWithValues(descriptor, Columns.Count, values);
            return AddColumn(column);
        }

        public object this[int column, int row] 
            => Columns[column][row];
    }
}
