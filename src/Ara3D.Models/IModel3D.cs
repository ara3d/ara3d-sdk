using Ara3D.DataTable;
using Plato;

namespace Ara3D.Models;

public interface IModel3D
{
    IReadOnlyList<TriangleMesh3D> Meshes { get; }
    IReadOnlyList<Material> Materials { get; }
    IReadOnlyList<Node> Nodes { get; }
    IReadOnlyList<Matrix4x4> NodeTransforms { get; }
    IReadOnlyList<string> NodeNames { get; }
    IDataTable DataTable { get; }
}