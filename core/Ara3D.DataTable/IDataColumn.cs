using Ara3D.Memory;

namespace Ara3D.DataTable;

public interface IDataColumn
{
    IDataDescriptor Descriptor { get; }
    IBuffer Data { get; }
}