using Ara3D.Geometry;
using Assimp;

namespace Ara3D.AssimpLoader;

public class AssimpMesh
{
    public int MaterialIndex { get; }
    public TriangleMesh3D Mesh { get; }

    public AssimpMesh(Mesh mesh)
    {
        
    }

    public static AssimpMesh Create(Mesh mesh)
        => new(mesh);
}