using Ara3D.Collections;
using Ara3D.DataTable;
using Ara3D.PropKit;

namespace Ara3D.DataSetBrowser.WPF;

public class DataColumnFromAccessorAndList<T>
    : IDataColumn
{
        private IReadOnlyList<T> _values;
    public int ColumnIndex { get; }
    public IDataDescriptor Descriptor { get; }

    IReadOnlyList<object> IDataColumn.Values 
        => _values.Select(v => Accessor.Getter(v));
        
    public IReadOnlyList<T> Values { get; }
    public PropAccessor Accessor { get; }

    public DataColumnFromAccessorAndList(int index, PropAccessor acc, IReadOnlyList<T> values)
    {
        ColumnIndex = index;
        Descriptor = new DataDescriptor(acc.Descriptor.Name, acc.Descriptor.Type, index);
        Accessor = acc;
        _values = values;
    }
}
