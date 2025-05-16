using Ara3D.PropKit;

namespace Ara3D.DataTable;

public interface IDataDescriptor
{
    PropDescriptor PropDescriptor { get; }
    bool IsIndex { get; }
    IDataTable LinkedTable { get; }
}