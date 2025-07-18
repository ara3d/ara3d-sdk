using Ara3D.Collections;
using Ara3D.DataTable;
using Ara3D.PropKit;

namespace Ara3D.DataTable;

public class DataColumnFromAccessorAndList<T>
    : IDataColumn
{
    private readonly IReadOnlyList<T> _values;
    public int ColumnIndex { get; }
    public IDataDescriptor Descriptor { get; }
    public int Count => _values.Count;
    public object this[int n] => Accessor.Getter(_values[n]);
    public PropAccessor Accessor { get; }

    public DataColumnFromAccessorAndList(int index, PropAccessor acc, IReadOnlyList<T> values)
    {
        ColumnIndex = index;
        Descriptor = new DataDescriptor(acc.Descriptor.Name, acc.Descriptor.Type, index);
        Accessor = acc;
        _values = values;
    }

    public Array AsArray() => 
        // ReSharper disable once SuspiciousTypeConversion.Global
        _values as Array ?? _values.ToArray();
}
