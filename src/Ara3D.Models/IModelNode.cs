using Plato;

namespace Ara3D.Models;

public interface IModelNode
{
    Guid Id { get; }
    string Name { get; }
    ModelMaterial Material { get; }
    TriangleMesh3D Mesh { get; }
    Matrix4x4 Transform { get; }
}