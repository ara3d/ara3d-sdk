using Plato;

namespace Ara3D.Scenes;

public interface IModelNode
{
    Guid Id { get; }
    string Name { get; }
    Material Material { get; }
    TriangleMesh3D Mesh { get; }
    Matrix4x4 Transform { get; }
}