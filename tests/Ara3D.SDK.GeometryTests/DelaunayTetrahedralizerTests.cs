using Ara3D.Geometry;

[TestFixture]
public class DelaunayTetrahedralizerTests
{
    const float Tol = 1e-3f;

    static float TetVolume(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        => Vector3.Dot(Vector3.Cross(b - a, c - a), d - a) / 6f;

    static IReadOnlyList<Point3D> UnitCubeCorners()
        => new List<Point3D>
        {
            new(0, 0, 0), new(1, 0, 0), new(0, 1, 0), new(1, 1, 0),
            new(0, 0, 1), new(1, 0, 1), new(0, 1, 1), new(1, 1, 1)
        };

    static IReadOnlyList<Point3D> RandomPoints(int count, int seed)
    {
        var random = new Random(seed);
        var points = new List<Point3D>(count);
        for (var i = 0; i < count; ++i)
            points.Add(new Point3D(
                random.NextSingle() * 4f - 2f,
                random.NextSingle() * 4f - 2f,
                random.NextSingle() * 4f - 2f));
        return points;
    }

    static void AssertPositiveVolumes(IReadOnlyList<Point3D> points, IReadOnlyList<Integer4> tets)
    {
        foreach (var t in tets)
        {
            var volume = TetVolume(
                points[t.A].Vector3, points[t.B].Vector3,
                points[t.C].Vector3, points[t.D].Vector3);
            Assert.That(volume, Is.GreaterThan(0));
        }
    }

    static void AssertDelaunay(IReadOnlyList<Point3D> points, IReadOnlyList<Integer4> tets)
    {
        foreach (var t in tets)
        {
            var a = points[t.A].Vector3;
            var b = points[t.B].Vector3;
            var c = points[t.C].Vector3;
            var d = points[t.D].Vector3;
            var (center, radiusSq) = Circumsphere(a, b, c, d);

            for (var i = 0; i < points.Count; ++i)
            {
                if (i == t.A || i == t.B || i == t.C || i == t.D)
                    continue;

                var distSq = (float)(points[i].Vector3 - center).LengthSquared();
                Assert.That(distSq, Is.GreaterThan(radiusSq - Tol));
            }
        }
    }

    static (Vector3 Center, float RadiusSq) Circumsphere(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        var ba = b - a;
        var ca = c - a;
        var da = d - a;
        var det = 2f * Vector3.Dot(ba, Vector3.Cross(ca, da));
        var offset =
            (Vector3.Cross(ca, da) * ba.LengthSquared()
             + Vector3.Cross(da, ba) * ca.LengthSquared()
             + Vector3.Cross(ba, ca) * da.LengthSquared()) / det;
        var center = a + offset;
        return (center, (a - center).LengthSquared());
    }

    [Test]
    public void CubeCornersFillCubeVolume()
    {
        var points = UnitCubeCorners();
        var tets = DelaunayTetrahedralizer.Tetrahedralize(points);
        Assert.That(tets.Count, Is.GreaterThanOrEqualTo(5));
        AssertPositiveVolumes(points, tets);

        var totalVolume = 0f;
        foreach (var t in tets)
            totalVolume += TetVolume(
                points[t.A].Vector3, points[t.B].Vector3,
                points[t.C].Vector3, points[t.D].Vector3);
        Assert.That(totalVolume, Is.EqualTo(1f).Within(Tol));
    }

    [Test]
    public void CubeCornersBoundaryIsTwelveTriangles()
    {
        var points = UnitCubeCorners();
        var hull = points.DelaunayHull();
        Assert.That(hull.FaceIndices.Count, Is.EqualTo(12));
    }

    [Test]
    public void RandomPointsAreDelaunay()
    {
        var points = RandomPoints(24, 123);
        var tets = DelaunayTetrahedralizer.Tetrahedralize(points);
        Assert.That(tets.Count, Is.GreaterThan(0));
        AssertPositiveVolumes(points, tets);
        AssertDelaunay(points, tets);
    }

    [Test]
    public void FewerThanFourPointsYieldsNothing()
    {
        var points = new List<Point3D> { new(0, 0, 0), new(1, 0, 0), new(0, 1, 0) };
        Assert.That(DelaunayTetrahedralizer.Tetrahedralize(points), Is.Empty);
    }
}
