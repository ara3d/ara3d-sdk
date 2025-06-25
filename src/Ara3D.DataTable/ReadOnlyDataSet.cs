namespace Ara3D.DataTable;

public class ReadOnlyDataSet : IDataSet
{
    public IReadOnlyList<IDataTable> Tables { get; }

    public ReadOnlyDataSet(IReadOnlyList<IDataTable> dataTables)
        => Tables = dataTables;
}