using Ara3D.Geometry;

[TestFixture]
public class EarcutTests
{
    static List<Vector2> Ring(params (float X, float Y)[] points)
        => points.Select(p => new Vector2(p.X, p.Y)).ToList();

    static double SignedArea(IReadOnlyList<Vector2> ring)
    {
        var a = 0.0;
        for (var i = 0; i < ring.Count; ++i)
        {
            var p = ring[i];
            var q = ring[(i + 1) % ring.Count];
            a += (double)p.X.Value * q.Y.Value - (double)q.X.Value * p.Y.Value;
        }
        return 0.5 * a;
    }

    static double SignedArea(Vector2 a, Vector2 b, Vector2 c)
        => 0.5 * (((double)b.X.Value - a.X.Value) * ((double)c.Y.Value - a.Y.Value)
            - ((double)b.Y.Value - a.Y.Value) * ((double)c.X.Value - a.X.Value));

    static readonly List<Vector2> Square6x4 = Ring((0, 0), (6, 0), (6, 4), (0, 4));

    /// <summary>Outer ring, holes, and the name of the case.</summary>
    static IEnumerable<TestCaseData> Polygons()
    {
        TestCaseData Case(string name, List<Vector2> outer, params List<Vector2>[] holes)
            => new TestCaseData(outer, holes.ToList()).SetName(name);

        yield return Case("ConvexSquare", Ring((0, 0), (2, 0), (2, 2), (0, 2)));
        yield return Case("ConcaveArrow", Ring((0, 0), (3, 1), (0, 2), (1, 1)));
        yield return Case("LShape", Ring((0, 0), (4, 0), (4, 1), (1, 1), (1, 4), (0, 4)));
        yield return Case("ClockwiseLShape", Ring((0, 4), (1, 4), (1, 1), (4, 1), (4, 0), (0, 0)));
        // The three cases below were PolygonTriangulator's known issues.
        yield return Case("RectangleWithRectHole", Square6x4, Ring((2, 1), (4, 1), (4, 3), (2, 3)));
        yield return Case("SquareWithTriangleHole", Ring((0, 0), (5, 0), (5, 5), (0, 5)), Ring((2, 2), (3.5f, 2.5f), (2.5f, 3.5f)));
        yield return Case("TwoHoles", Ring((0, 0), (8, 0), (8, 6), (0, 6)),
            Ring((1, 1), (3, 1), (3, 3), (1, 3)), Ring((5, 2), (7, 2), (7, 4), (5, 4)));
        // A 32-point circle of radius 9.5 mm, an inch bolt shank in metres: its corners' cross
        // products are about 1e-7, below the absolute tolerance PolygonTriangulator used.
        yield return Case("SmallCircle", Enumerable.Range(0, 32)
            .Select(i => new Vector2(0.0095f * MathF.Cos(i * MathF.PI / 16), 0.0095f * MathF.Sin(i * MathF.PI / 16))).ToList());
    }

    [TestCaseSource(nameof(Polygons))]
    public void CoversTheOuterRingLessItsHoles_WithCounterClockwiseTriangles(List<Vector2> outer, List<List<Vector2>> holes)
    {
        var points = holes.Prepend(outer).SelectMany(r => r).ToList();
        var triangles = Earcut.Triangulate(outer, holes);
        var areas = triangles.Select(t => SignedArea(points[t.A], points[t.B], points[t.C])).ToList();
        var expected = Math.Abs(SignedArea(outer)) - holes.Sum(h => Math.Abs(SignedArea(h)));

        Assert.Multiple(() =>
        {
            Assert.That(triangles, Has.Count.EqualTo(points.Count - 2 + 2 * holes.Count), "n - 2, plus 2 per hole for its bridge");
            Assert.That(areas.Sum(), Is.EqualTo(expected).Within(1e-6).Percent);
            Assert.That(areas, Is.All.GreaterThan(0), "every triangle winds counter-clockwise");
            Assert.That(Earcut.Deviation(outer, holes, triangles), Is.LessThan(1e-9));
        });
    }

    [Test]
    public void RingCrossingItself_DeviatesFromItsArea()
    {
        // A bow tie: two lobes of opposite winding whose signed areas cancel.
        var bowTie = Ring((0, 0), (2, 2), (2, 0), (0, 2));
        Assert.That(Earcut.Deviation(bowTie, [], Earcut.Triangulate(bowTie, [])), Is.GreaterThan(0.5));
    }

    [Test]
    public void HoleVertices_AreIndexedAfterTheOuterRing()
    {
        var hole = Ring((2, 1), (4, 1), (4, 3), (2, 3));
        var used = Earcut.Triangulate(Square6x4, [hole])
            .SelectMany(t => new[] { t.A.Value, t.B.Value, t.C.Value }).Distinct().Order();
        Assert.That(used, Is.EqualTo(Enumerable.Range(0, 8)));
    }

    [Test]
    public void ScalingThePolygon_GivesTheSameTriangles()
    {
        var hole = Ring((2, 1), (4, 1), (4, 3), (2, 3));
        List<Integer3> Scaled(float s)
            => Earcut.Triangulate(Square6x4.Select(p => p * s).ToList(), [hole.Select(p => p * s).ToList()]);
        Assert.That(Scaled(1e-4f), Is.EqualTo(Scaled(1)).And.EqualTo(Scaled(1e4f)));
    }

    [Test]
    public void CollinearPoints_GiveNoTriangles()
        => Assert.That(Earcut.Triangulate(Ring((0, 0), (1, 1), (2, 2), (3, 3)), []), Is.Empty);

    [Test]
    public void FlatInput_GivesIndexTriplets()
        => Assert.That(Earcut.Triangulate([0, 0, 1, 0, 1, 1, 0, 1], []), Has.Count.EqualTo(6));
}
