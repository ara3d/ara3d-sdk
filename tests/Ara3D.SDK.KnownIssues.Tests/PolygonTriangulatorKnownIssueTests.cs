using Ara3D.Geometry;
using System.Collections.Generic;
using System.Linq;

[TestFixture]
public class PolygonTriangulatorKnownIssueTests
{
    static float Area(IReadOnlyList<Vector2> poly)
    {
        double a = 0;
        for (var i = 0; i < poly.Count; ++i)
        {
            var p = poly[i];
            var q = poly[(i + 1) % poly.Count];
            a += (double)p.X * q.Y - (double)q.X * p.Y;
        }

        return (float)(0.5 * a);
    }

    static float TriArea(Vector2 a, Vector2 b, Vector2 c)
        => System.MathF.Abs(((b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X)) * 0.5f);

    static float SumTriangleAreas(IReadOnlyList<Triangle2D> tris)
        => tris.Aggregate(0f, (acc, t) => acc + TriArea(t.A, t.B, t.C));

    const float Tol = 1e-3f;

    [Test]
    [Category("KnownIssue")]
    public void RectangleWithRectHole()
    {
        var outer = new List<Vector2>
        {
            new(0, 0), new(6, 0), new(6, 4), new(0, 4)
        };
        var hole = new List<Vector2>
        {
            new(2, 1), new(4, 1), new(4, 3), new(2, 3)
        };

        var holes = new List<IReadOnlyList<Vector2>> { hole };
        var tris = PolygonTriangulator.GetTriangles(outer, holes);

        var targetArea = System.MathF.Abs(Area(outer)) - System.MathF.Abs(Area(hole));
        var gotArea = SumTriangleAreas(tris);
        Assert.That(System.MathF.Abs(targetArea - gotArea) <= Tol);

        var expected = outer.Count + hole.Count + 2 * 1 - 2;
        Assert.That(tris.Count, Is.EqualTo(expected));
    }

    [Test]
    [Category("KnownIssue")]
    public void DonutLike_OuterSquare_InnerTriangle()
    {
        var outer = new List<Vector2>
        {
            new(0, 0), new(5, 0), new(5, 5), new(0, 5)
        };
        var hole = new List<Vector2>
        {
            new(2, 2), new(3.5f, 2.5f), new(2.5f, 3.5f)
        };

        var tris = PolygonTriangulator.GetTriangles(outer, new List<IReadOnlyList<Vector2>> { hole });

        var targetArea = System.MathF.Abs(Area(outer)) - System.MathF.Abs(Area(hole));
        var gotArea = SumTriangleAreas(tris);
        Assert.That(System.MathF.Abs(targetArea - gotArea) <= Tol);

        var expected = outer.Count + hole.Count + 2 * 1 - 2;
        Assert.That(tris.Count, Is.EqualTo(expected));
    }

    [Test]
    [Category("KnownIssue")]
    public void MultipleHoles()
    {
        var outer = new List<Vector2>
        {
            new(0, 0), new(8, 0), new(8, 6), new(0, 6)
        };
        var holeA = new List<Vector2> { new(1, 1), new(3, 1), new(3, 3), new(1, 3) };
        var holeB = new List<Vector2> { new(5, 2), new(7, 2), new(7, 4), new(5, 4) };

        var tris = PolygonTriangulator.GetTriangles(outer, new List<IReadOnlyList<Vector2>> { holeA, holeB });

        var targetArea = System.MathF.Abs(Area(outer)) - System.MathF.Abs(Area(holeA)) - System.MathF.Abs(Area(holeB));
        var gotArea = SumTriangleAreas(tris);
        Assert.That(System.MathF.Abs(targetArea - gotArea) <= Tol);

        var expected = outer.Count + holeA.Count + holeB.Count + 2 * 2 - 2;
        Assert.That(tris.Count, Is.EqualTo(expected));
    }
}
