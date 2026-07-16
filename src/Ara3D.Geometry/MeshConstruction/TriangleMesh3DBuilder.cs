using Ara3D.Collections;

namespace Ara3D.Geometry;

public class TriangleMesh3DBuilder
{
    public List<Point3D> Points { get; } = new();
    public List<Integer3> Faces { get; } = new();
    public TriangleMesh3D ToTriangleMesh3D() => (Points, Faces);

    public TriangleMesh3DBuilder AddFan(List<int> indices)
    {
        var points = indices.Map(i => Points[i]);
        var mid = points.Middle();
        var midIndex = Points.Count;
        Points.Add(mid);
        for (var i = 0; i < indices.Count; i++)
        {
            var face = (midIndex, i, (i + 1) % indices.Count);
            Faces.Add(face);
        }

        return this;
    }
}