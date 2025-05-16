using Ara3D.DataTable;

namespace Ara3D.Scenes
{
    public class Model : IModel
    {
        public Model(IEnumerable<ModelNode> nodes, IDataTable dataTable = null)
        {
            Nodes = nodes.Cast<IModelNode>().ToList();
            DataTable = dataTable;
        }

        public IReadOnlyList<IModelNode> Nodes { get; }

        public IDataTable DataTable { get; }

        public static implicit operator Model(List<ModelNode> nodes)
            => new(nodes);

        public static implicit operator Model(ModelNode[] nodes)
            => new(nodes);
    }
}