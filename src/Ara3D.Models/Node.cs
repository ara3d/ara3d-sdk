using Plato;

namespace Ara3D.Models;

public class Node
{
    public Node(IModel3D model, int nodeIndex, int materialIndex, int meshIndex)
    {
        Model = model;
        NodeIndex = nodeIndex;
        MaterialIndex = materialIndex;
        MeshIndex = meshIndex;
    }

    public IModel3D Model { get; }
    public int NodeIndex { get; }
    public int MaterialIndex { get; }
    public int MeshIndex { get; }

    public string Name => Model.NodeNames[NodeIndex];
    public Matrix4x4 Transform => Model.NodeTransforms[NodeIndex];

    public TriangleMesh3D Mesh => Model.Meshes[MeshIndex];
    public Material Material => Model.Materials[MaterialIndex];
}