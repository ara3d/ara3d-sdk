using Ara3D.DataTable;

namespace Ara3D.Scenes;

public interface IModel
{
    IReadOnlyList<IModelNode> Nodes { get; }
    IDataTable DataTable { get; }
}