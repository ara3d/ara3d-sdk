using Plato;

namespace Ara3D.Scene
{
    public record SceneNode(string Id, int Index, ITransform3D Transform, Material Material, TriangleMesh3D Mesh) : ISceneNode;
}