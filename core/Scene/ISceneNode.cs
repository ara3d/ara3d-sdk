using Plato;

namespace Ara3D.Scene
{
    public interface ISceneNode
    {
        string Id { get; }
        int Index { get; }
        ITransform3D Transform { get; }
        Material Material { get; }
        TriangleMesh3D Mesh { get; }
    }
}