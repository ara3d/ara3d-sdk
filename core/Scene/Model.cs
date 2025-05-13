using Plato;

namespace Ara3D.Scenes
{
    public class Model 
    {
        public Model(ReadOnlySpan<ModelNode> nodes)
        {
            foreach (var node in nodes)
                Nodes.Add(node);
        }

        public Model(IEnumerable<ModelNode> nodes)
            => Nodes.AddRange(nodes);

        public List<ModelNode> Nodes { get; } 
            = new();

        public static implicit operator Model(List<ModelNode> nodes)
            => new(nodes);

        public static implicit operator Model(ModelNode[] nodes)
            => new(nodes.AsSpan());

        public static implicit operator Model(ReadOnlySpan<ModelNode> nodes)
            => new(nodes);
    }
}