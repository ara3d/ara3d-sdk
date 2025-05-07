using Plato;

namespace Ara3D.Scenes
{
    public class Scene 
    {
        public Scene(ReadOnlySpan<SceneNode> nodes)
        {
            foreach (var node in nodes)
                Nodes.Add(node);
        }

        public Scene(IEnumerable<SceneNode> nodes)
            => Nodes.AddRange(nodes);

        public List<SceneNode> Nodes { get; } 
            = new();

        public static implicit operator Scene(List<SceneNode> nodes)
            => new(nodes);

        public static implicit operator Scene(SceneNode[] nodes)
            => new(nodes.AsSpan());

        public static implicit operator Scene(ReadOnlySpan<SceneNode> nodes)
            => new(nodes);
    }
}