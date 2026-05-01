using Ara3D.Geometry;

namespace Ara3D.DataTable;

public class ReadOnlyListSingleColumnDataAdapter<T> : IDataColumn, IDataTable
{
    public ReadOnlyListSingleColumnDataAdapter(string name, IReadOnlyList<T> values)
    {
        Name = name;
        _values = values;
        Descriptor = new DataDescriptor(name, typeof(T));
        Columns = [this];
        Rows = Count.MapRange(i => new DataRow(this, i)).ToList();
    }
    private readonly IReadOnlyList<T> _values;
    public string Name { get; }
    public IReadOnlyList<IDataRow> Rows { get; }
    public IReadOnlyList<IDataColumn> Columns { get; }
    public object this[int column, int row] => column != 0 ? throw new Exception("Column out of range") : this[row];
    public int ColumnIndex => 0;
    public IDataDescriptor Descriptor { get; }
    public int Count => _values.Count;
    public object this[int n] => _values[n];
    public Array AsArray() => Enumerable.ToArray(_values);
}

