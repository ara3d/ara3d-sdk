namespace Ara3D.DataTable;

public interface IDataRow
{
    IDataSchema Schema { get; }
    IReadOnlyList<object> Values { get; }
}