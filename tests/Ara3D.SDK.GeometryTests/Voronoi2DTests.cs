using Ara3D.Geometry;

[TestFixture]
public class Voronoi2DTests
{
    const float Tol = 1e-3f;

    static float PolyArea(IReadOnlyList<Vector2> poly)
    {
        var a = 0f;
        for (var i = 0; i < poly.Count; ++i)
        {
            var p = poly[i];
            var q = poly[(i + 1) % poly.Count];
            a += (float)p.X * (float)q.Y - (float)q.X * (float)p.Y;
        }

        return MathF.Abs(a) * 0.5f;
    }

    static int NearestSite(IReadOnlyList<Vector2> sites, Vector2 p)
    {
        var best = -1;
        var bestD = float.MaxValue;
        for (var i = 0; i < sites.Count; ++i)
        {
            float dx = (float)sites[i].X - (float)p.X, dy = (float)sites[i].Y - (float)p.Y;
            var d = dx * dx + dy * dy;
            if (d < bestD) { bestD = d; best = i; }
        }

        return best;
    }

    static List<Vector2> RandomSites(int count, int seed, float radius)
    {
        var r = new Random(seed);
        var sites = new List<Vector2>(count);
        for (var i = 0; i < count; ++i)
            sites.Add(new Vector2((r.NextSingle() * 2 - 1) * radius, (r.NextSingle() * 2 - 1) * radius));

        return sites;
    }

    [Test]
    public void FourSitesTileTheBoxInQuarters()
    {
        var sites = new List<Vector2> { new(-1, -1), new(1, -1), new(1, 1), new(-1, 1) };
        var (min, max) = Voronoi2D.BoundsOf(sites, 1f);
        var boxArea = ((float)max.X - (float)min.X) * ((float)max.Y - (float)min.Y);

        var cells = Voronoi2D.Compute(sites, min, max);
        Assert.That(cells.Count, Is.EqualTo(4));

        var total = 0f;
        foreach (var cell in cells)
        {
            total += PolyArea(cell.Polygon);
            Assert.That(PolygonTriangulator.PointInPolygon(cell.Polygon, cell.Center), Is.True,
                "a site must lie inside its own cell");
        }

        // By symmetry each of the four cells is a quarter of the box.
        Assert.That(total, Is.EqualTo(boxArea).Within(Tol));
        foreach (var cell in cells)
            Assert.That(PolyArea(cell.Polygon), Is.EqualTo(boxArea / 4).Within(Tol));
    }

    [Test]
    public void CellsPartitionTheBoundingBox()
    {
        var sites = RandomSites(40, 7, 3f);
        var (min, max) = Voronoi2D.BoundsOf(sites, 0.5f);
        var boxArea = ((float)max.X - (float)min.X) * ((float)max.Y - (float)min.Y);

        var cells = Voronoi2D.Compute(sites, min, max);
        var total = 0f;
        foreach (var cell in cells)
            total += PolyArea(cell.Polygon);

        // Convex cells tile the box with no gaps or overlaps: areas sum to the box area.
        Assert.That(total, Is.EqualTo(boxArea).Within(1e-2f));
    }

    [Test]
    public void EachPointFallsInNearestSiteCell()
    {
        var sites = RandomSites(30, 11, 3f);
        var (min, max) = Voronoi2D.BoundsOf(sites, 0.5f);
        var cells = Voronoi2D.Compute(sites, min, max);

        var r = new Random(99);
        for (var t = 0; t < 200; ++t)
        {
            var p = new Vector2(
                (float)min.X + r.NextSingle() * ((float)max.X - (float)min.X),
                (float)min.Y + r.NextSingle() * ((float)max.Y - (float)min.Y));

            var nearest = NearestSite(sites, p);
            Assert.That(PolygonTriangulator.PointInPolygon(cells[nearest].Polygon, p), Is.True,
                "a point must lie in the cell of its nearest site (the defining Voronoi property)");
        }
    }

    [Test]
    public void RelaxationMovesSitesTowardCellCentroids()
    {
        var sites = RandomSites(50, 3, 3f);
        var (min, max) = Voronoi2D.BoundsOf(sites, 0.5f);

        var relaxed = Voronoi2D.Relax(sites, min, max, 12);
        var cells = Voronoi2D.Compute(relaxed, min, max);

        // After Lloyd relaxation each site sits (nearly) at its cell centroid — CVT convergence.
        var maxDrift = 0f;
        foreach (var cell in cells)
        {
            if (cell.Polygon.Count < 3)
                continue;

            var c = PolygonTriangulator.Centroid(cell.Polygon);
            float dx = (float)c.X - (float)cell.Center.X, dy = (float)c.Y - (float)cell.Center.Y;
            maxDrift = MathF.Max(maxDrift, MathF.Sqrt(dx * dx + dy * dy));
        }

        Assert.That(maxDrift, Is.LessThan(0.25f));
    }
}
