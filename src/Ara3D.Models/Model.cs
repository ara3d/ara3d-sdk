using Ara3D.DataTable;
using Ara3D.Collections;
using Plato;

namespace Ara3D.Models
{
    public class Model : ITransformable3D<Model>
    {
        public Model(IReadOnlyList<ModelNode> nodes, IDataTable dataTable = null)
        {
            Nodes = nodes;
            DataTable = dataTable;
        }

        public IReadOnlyList<ModelNode> Nodes { get; }

        public IDataTable DataTable { get; }

        public static implicit operator Model(List<ModelNode> nodes)
            => new(nodes);

        public static implicit operator Model(ModelNode[] nodes)
            => new(nodes);

        public Model Transform(Matrix4x4 matrix)
            => new(Nodes.Select(n => n.Transform(matrix)), DataTable);
    }
}