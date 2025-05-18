using Ara3D.DataTable;

namespace Ara3D.Models;

public interface IModel
{
    IReadOnlyList<IModelNode> Nodes { get; }
    IDataTable DataTable { get; }
}