namespace Ara3D.Studio.Samples.Demos;

/// <summary>
/// Teaching demo: blue (Poisson-disk) noise versus white (uniform-random) noise, side by side at
/// the SAME point count. Blue noise spreads evenly with no clumps or gaps; white noise clusters.
/// Turn on HighlightDefects to paint every point that has a neighbour closer than Radius red —
/// the white patch breaks out in a rash, the blue patch stays clean. That is the whole lesson.
/// </summary>
[Category(Cat.ExperimentalDemos)]
[Description("Teaching demo contrasting blue (Poisson-disk) noise with white noise at equal point counts, optionally flagging points that violate the minimum spacing.")]
public class BlueNoiseField : IGenerator
{
    [Range(1f, 40f)] public float Width = 12f;
    [Range(1f, 40f)] public float Height = 12f;
    [Range(0.2f, 5f)] public float Radius = 0.7f;
    [Range(1, 60)] public int Attempts = 30;
    [Range(0, 10000)] public int Seed = 1234;
    [Range(0.02f, 1f)] public float MarkerSize = 0.18f;

    public bool ShowWhiteNoise = true;
    public bool HighlightDefects = true;

    public IModel3D Eval()
    {
        var marker = PlatonicSolids.TriangulatedCube.Scale(MarkerSize);
        var gap = Width * 0.35f;
        var models = new List<IModel3D>();

        var blue = PoissonDiskSampling.Sample(Width, Height, Radius, Seed, Attempts);
        AddPatch(models, marker, blue, -Width - gap);

        if (ShowWhiteNoise)
        {
            var white = PoissonDiskSampling.UniformRandom(Width, Height, blue.Count, Seed);
            AddPatch(models, marker, white, gap);
        }

        return models.Merge();
    }

    private void AddPatch(List<IModel3D> models, TriangleMesh3D marker, IReadOnlyList<Vector2> pts, float xOffset)
    {
        var defect = HighlightDefects ? Defects(pts, Radius) : null;
        var good = new List<Point3D>();
        var bad = new List<Point3D>();
        for (var i = 0; i < pts.Count; i++)
        {
            var p = new Point3D(pts[i].X + xOffset, pts[i].Y - Height / 2, 0);
            if (defect != null && defect[i])
                bad.Add(p);
            else
                good.Add(p);
        }

        if (good.Count > 0)
            models.Add(marker.Clone(Material.Default.WithColor((0.2f, 0.5f, 1f, 1f)), good));
        if (bad.Count > 0)
            models.Add(marker.Clone(Material.Default.WithColor((1f, 0.15f, 0.15f, 1f)), bad));
    }

    // Brute-force minimum-distance violations; teaching sizes are small (hundreds of points).
    private static bool[] Defects(IReadOnlyList<Vector2> pts, float radius)
    {
        var n = pts.Count;
        var flags = new bool[n];
        var r2 = radius * radius;
        for (var i = 0; i < n; i++)
        for (var j = i + 1; j < n; j++)
        {
            double dx = pts[i].X - pts[j].X;
            double dy = pts[i].Y - pts[j].Y;
            if (dx * dx + dy * dy < r2)
            {
                flags[i] = true;
                flags[j] = true;
            }
        }
        return flags;
    }
}

/// <summary>
/// Blue noise as a MODIFIER: scatter instances across an input surface with a guaranteed minimum
/// spacing. Greedily thins the input mesh's own vertices to blue-noise density (so the scatter
/// conforms exactly to the surface), then clones a marker at each kept point. This is the production
/// payoff — distributing rocks, rivets, trees, foliage without overlaps and with controllable density.
/// </summary>
[Category(Cat.ExperimentalDemos)]
[Description("Scatters instances across an input surface at a guaranteed minimum spacing using blue-noise thinning.")]
public class PoissonScatterOnSurface : IModifier
{
    [Range(0.05f, 5f)] public float Radius = 0.5f;
    [Range(0, 10000)] public int Seed = 1234;
    [Range(0.02f, 2f)] public float InstanceScale = 0.15f;

    public bool KeepInput = true;

    public IModel3D Eval(IModel3D model)
    {
        var mesh = model.ToColoredMesh();
        var points = mesh.Mesh.Points.Map(p => p.Vector3);
        var kept = PoissonDiskSampling.SelectSubset(points, Radius, Seed);

        var marker = PlatonicSolids.TriangulatedCube.Scale(InstanceScale);
        var positions = kept.Map(v => new Point3D(v.X, v.Y, v.Z));
        var scattered = marker.Clone(Material.Default.WithColor((0.2f, 0.9f, 0.4f, 1f)), positions);

        return KeepInput ? model.Combine(scattered) : scattered;
    }
}
