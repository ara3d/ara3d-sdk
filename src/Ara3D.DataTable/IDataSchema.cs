namespace Ara3D.DataTable;

public interface IDataSchema
{
    IReadOnlyList<IDataDescriptor> Descriptors { get; }
}