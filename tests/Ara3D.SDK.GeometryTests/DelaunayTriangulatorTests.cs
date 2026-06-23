using Ara3D.Geometry;

[TestFixture]
public class DelaunayTriangulatorTests
{
    const float Tol = 1e-3f;

    static float TriArea(Vector2 a, Vector2 b, Vector2 c)
        => MathF.Abs(((b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X)) * 0.5f);

    static bool InCircumcircle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        var ax = a.X - p.X;
        var ay = a.Y - p.Y;
        var bx = b.X - p.X;
        var by = b.Y - p.Y;
        var cx = c.X - p.X;
        var cy = c.Y - p.Y;

        var det =
            (ax * ax + ay * ay) * (bx * cy - by * cx)
            - (bx * bx + by * by) * (ax * cy - ay * cx)
            + (cx * cx + cy * cy) * (ax * by - ay * bx);

        return det > DelaunayTriangulator.Eps;
    }

    static void AssertDelaunay(IReadOnlyList<Vector2> points, IReadOnlyList<Integer3> faces)
    {
        foreach (var face in faces)
        {
            var a = points[face.A];
            var b = points[face.B];
            var c = points[face.C];
            Assert.That(TriArea(a, b, c), Is.GreaterThan(Tol));

            for (var i = 0; i < points.Count; ++i)
            {
                if (i == face.A || i == face.B || i == face.C)
                    continue;

                Assert.That(InCircumcircle(points[i], a, b, c), Is.False);
            }
        }
    }

    [Test]
    public void SquareFourPoints()
    {
        var points = new List<Vector2>
        {
            new(0, 0), new(2, 0), new(2, 2), new(0, 2)
        };

        var faces = DelaunayTriangulator.Triangulate(points);
        Assert.That(faces.Count, Is.EqualTo(2));
        AssertDelaunay(points, faces);
    }

    [Test]
    public void RandomPlanarPoints()
    {
        var random = new Random(123);
        var points = Enumerable.Range(0, 32)
            .Select(_ => new Vector2(random.NextSingle() * 4f - 2f, random.NextSingle() * 4f - 2f))
            .ToList();

        var faces = DelaunayTriangulator.Triangulate(points);
        Assert.That(faces.Count, Is.GreaterThan(0));
        AssertDelaunay(points, faces);
    }

    [Test]
    public void TriangleMesh3DFromPoints()
    {
        var points = new List<Point3D>
        {
            new(0, 0, 0),
            new(1, 0, 0),
            new(0.5f, 1, 0),
            new(0.5f, 0.5f, 0)
        };

        var mesh = DelaunayTriangulator.Triangulate(points, DelaunayTriangulator.ProjectionPlane.XY);
        Assert.That(mesh.Points.Count, Is.EqualTo(points.Count));
        Assert.That(mesh.FaceIndices.Count, Is.GreaterThan(0));
    }
}
