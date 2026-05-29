namespace Ara3D.Geometry;

public class ColoredTriangleMesh3D : IGeometry
{
    public TriangleMesh3D Mesh { get; }
    public IReadOnlyList<Vector3>? VertexColors { get; }

    public ColoredTriangleMesh3D(TriangleMesh3D mesh, IReadOnlyList<Vector3>? vertexColors)
        => (Mesh, VertexColors) = (mesh, vertexColors);
}

public static class ColoredTriangleMesh3DExtensions
{
    public static ColoredTriangleMesh3D ToColored(this TriangleMesh3D mesh, IReadOnlyList<Vector3>? vertexColors)
    {
        if (vertexColors != null)
            if (vertexColors.Count != mesh.Points.Count)
                throw new Exception(
                    $"# Vertex colors {vertexColors.Count} must match number of points {mesh.Points.Count}");
        return new(mesh, vertexColors);
    }
}