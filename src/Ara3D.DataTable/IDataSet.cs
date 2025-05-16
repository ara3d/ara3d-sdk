namespace Ara3D.DataTable;

public interface IDataSet
{
    IReadOnlyList<IDataTable> Tables { get; }
}