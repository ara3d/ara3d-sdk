namespace Ara3D.Geometry;

public class ColoredTriangleMesh3D : IGeometry
{
    public TriangleMesh3D Mesh { get; }
    public IReadOnlyList<Vector3> VertexColors { get; }

    public ColoredTriangleMesh3D(TriangleMesh3D mesh, IReadOnlyList<Vector3> vertexColors)
        => (Mesh, VertexColors) = (mesh, vertexColors);
}