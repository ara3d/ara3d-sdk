using Ara3D.PropKit;

namespace Ara3D.DataTable;

public class DataColumnFromAccessorAndList<T>
    : IDataColumn
{
    private readonly IReadOnlyList<T> _values;
    public int ColumnIndex { get; }
    public IDataDescriptor Descriptor { get; }
    public int Count => _values.Count;
    public object this[int n] => Accessor.GetValue(_values[n]);
    public IPropAccessor Accessor { get; }

    public DataColumnFromAccessorAndList(int index, IPropAccessor acc, IReadOnlyList<T> values)
    {
        ColumnIndex = index;
        Descriptor = new DataDescriptor(acc.Descriptor.Name, acc.Descriptor.Type);
        Accessor = acc;
        _values = values;
    }
}
