namespace Ara3D.Studio.Samples;

public class ColoredTriangleMesh3D : IPointGeometry3D<ColoredTriangleMesh3D>
{
    public IReadOnlyList<Point3D> Points { get; }
    public IReadOnlyList<Integer3> Faces { get; }
    public IReadOnlyList<Vector3> VertexColors { get; }

    public ColoredTriangleMesh3D(IReadOnlyList<Point3D> points, IReadOnlyList<Integer3> faces, IReadOnlyList<Vector3> vertexColors)
        => (Points, Faces, VertexColors) = (points, faces, vertexColors);

    public ColoredTriangleMesh3D Transform(Transform3D t)
        => new(Points.Transform(t), Faces, VertexColors);

    public ColoredTriangleMesh3D Deform(Func<Point3D, Point3D> f)
        => new(Points.Map(f), Faces, VertexColors);
}