using Plato;

namespace Ara3D.Models;

public class ModelDefinition
{
    public class Node
    {
        public Guid Id { get; } 
        public string Name { get; }
        public int MeshIndex { get; }
        public int MaterialIndex { get; }
        public Matrix4x4 Transform { get; }
    }

    public IReadOnlyList<TriangleMesh3D> Meshes { get; }
    public IReadOnlyList<ModelMaterial> Materials { get; }
    public IReadOnlyList<Node> Nodes { get; }

    public ModelDefinition(
        IReadOnlyList<TriangleMesh3D> meshes,
        IReadOnlyList<ModelMaterial> materials,
        IReadOnlyList<Node> nodes)
    {
        Meshes = meshes;
        Materials = materials;
        Nodes = nodes;
    }
}