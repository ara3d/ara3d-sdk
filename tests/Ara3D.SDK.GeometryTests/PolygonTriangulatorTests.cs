using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Ara3D.Geometry;


[TestFixture]
public class PolygonTriangulatorTests
{
    static float Area(IReadOnlyList<Vector2> poly)
    {
        double a = 0;
        for (int i = 0; i < poly.Count; ++i)
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
    public void ConvexSquare()
    {
        var square = new List<Vector2>
        {
            new(0,0), new(2,0), new(2,2), new(0,2)
        };
        var tris = PolygonTriangulator.GetTriangles(square, new List<IReadOnlyList<Vector2>>());
        Assert.AreEqual(2, tris.Count); // n-2

        var targetArea = System.MathF.Abs(Area(square));
        var gotArea = SumTriangleAreas(tris);
        Assert.That(System.MathF.Abs(targetArea - gotArea) <= Tol);
    }

    [Test]
    public void ConcaveArrow()
    {
        // Simple concave "arrow" / chevron shape
        var poly = new List<Vector2>
        {
            new(0,0), new(3,1), new(0,2), new(1,1)
        };
        var tris = PolygonTriangulator.GetTriangles(poly, new List<IReadOnlyList<Vector2>>());
        Assert.AreEqual(poly.Count - 2, tris.Count);

        var targetArea = System.MathF.Abs(Area(poly));
        var gotArea = SumTriangleAreas(tris);
        Assert.That(System.MathF.Abs(targetArea - gotArea) <= Tol);
    }

    [Test]
    public void LShapedConcave()
    {
        // L-shape (concave)
        var poly = new List<Vector2>
        {
            new(0,0), new(4,0), new(4,1), new(1,1), new(1,4), new(0,4)
        };
        var tris = PolygonTriangulator.GetTriangles(poly, new List<IReadOnlyList<Vector2>>());

        Assert.AreEqual(poly.Count - 2, tris.Count);

        var targetArea = System.MathF.Abs(Area(poly));
        var gotArea = SumTriangleAreas(tris);
        Assert.That(System.MathF.Abs(targetArea - gotArea) <= Tol);
    }

}
