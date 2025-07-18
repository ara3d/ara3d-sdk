
namespace Ara3D.DataTable
{
    public class ReadOnlyDataColumn<T>
        : IDataColumn
    {
        private readonly IReadOnlyList<T> _values;
        public IDataDescriptor Descriptor { get; }
        public object this[int n] => _values[n];
        public int Count => _values.Count;
        public int ColumnIndex { get; }
        public ReadOnlyDataColumn(int index, IReadOnlyList<T> values)
        {
            ColumnIndex = index;
            _values = values;
            Descriptor = new DataDescriptor(typeof(T).Name, typeof(T), index);
        }

        public Array AsArray()
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (_values is Array array) return array;
            return _values.ToArray();
        }
    }
}
