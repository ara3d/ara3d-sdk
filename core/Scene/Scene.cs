using Plato;

namespace Ara3D.Scene
{
    public class Scene : IScene
    {
        public List<SceneNode> Nodes { get; } = new();

        IReadOnlyList<ISceneNode> IScene.Nodes => Nodes.Select(x => (ISceneNode)x);
        
        public SceneNode AddNode(string id, ITransform3D transform, Material material, TriangleMesh3D mesh)
        {
            var node = new SceneNode(id, Nodes.Count, transform, material, mesh);
            Nodes.Add(node);
            return node;
        }
    }
}