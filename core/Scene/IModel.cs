using Ara3D.DataTable;
using Ara3D.NarwhalDB;

namespace Ara3D.Scenes;

public interface IModel
{
    IReadOnlyList<IModelNode> Nodes { get; }
    IDataTable DataTable { get; }
}