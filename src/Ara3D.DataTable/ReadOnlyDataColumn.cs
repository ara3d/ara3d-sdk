using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ara3D.DataTable
{
    public class ReadOnlyDataColumn<T>
        : IDataColumn
    {
        private IReadOnlyList<T> _values;
        public IDataDescriptor Descriptor { get; }
        public IReadOnlyList<object> Values { get; }
        public int ColumnIndex { get; }
        public ReadOnlyDataColumn(int index, IReadOnlyList<T> values)
        {
            ColumnIndex = index;
            _values = values;
            Descriptor = new DataDescriptor(typeof(T).Name, typeof(T), index);
            Values = _values.Select(v => (object)v).ToList();
        }
    }
}
