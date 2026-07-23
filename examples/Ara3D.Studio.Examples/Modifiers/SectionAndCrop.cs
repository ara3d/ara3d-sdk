namespace Ara3D.Studio.Samples.Modifiers;

/// <summary>
/// Extracts the contour where a horizontal plane (constant Z) slices a triangle mesh, returned as
/// a <see cref="LineMesh3D"/>. Every triangle straddling the plane contributes exactly one segment,
/// so as the height slides the outline updates — a live section cut. The plane height is a fraction
/// between the mesh bounds' Min.Z and Max.Z. A vertex lying exactly on the plane is treated as being
/// on the positive side, avoiding zero-length or doubled segments at that degenerate case.
/// </summary>
[Category(Cat.Cut)]
[Description("Extracts the contour where a horizontal plane slices a mesh, as a live section outline (line mesh).")]
public class PlaneSectionContours : IModifier
{
    [Range(0f, 1f)] public float Height = 0.5f;

    public int SegmentCount { get; private set; }

    public LineMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var bounds = mesh.DerivedBounds();
        var z = (float)bounds.Min.Z.Lerp(bounds.Max.Z, Height);

        var points = new List<Point3D>();
        var indices = new List<Integer2>();
        for (var i = 0; i < mesh.FaceIndices.Count; i++)
        {
            var f = mesh.FaceIndices[i];
            var a = mesh.Points[f.A].Vector3;
            var b = mesh.Points[f.B].Vector3;
            var c = mesh.Points[f.C].Vector3;
            if (!TryContourSegment(a, b, c, z, out var p0, out var p1))
                continue;
            var i0 = points.Count;
            points.Add(new Point3D(p0.X, p0.Y, p0.Z));
            points.Add(new Point3D(p1.X, p1.Y, p1.Z));
            indices.Add((i0, i0 + 1));
        }

        SegmentCount = indices.Count;
        ctx.Services.RefreshUI(this);
        return points.Count == 0 ? LineMesh3D.Default : new LineMesh3D(points, indices);
    }

    static bool TryContourSegment(Vector3 a, Vector3 b, Vector3 c, float z, out Vector3 p0, out Vector3 p1)
    {
        var da = (float)a.Z - z;
        var db = (float)b.Z - z;
        var dc = (float)c.Z - z;

        p0 = default;
        p1 = default;
        var n = 0;
        AddCrossing(a, da, b, db, ref p0, ref p1, ref n);
        AddCrossing(b, db, c, dc, ref p0, ref p1, ref n);
        AddCrossing(c, dc, a, da, ref p0, ref p1, ref n);
        return n == 2 && (p1 - p0).Length > 1e-6f;
    }

    // Sign convention: distance >= 0 counts as the positive side, so an endpoint on the plane joins
    // one side only. The plane crosses an edge when its endpoints have opposite signs; t = du/(du-dv).
    static void AddCrossing(Vector3 u, float du, Vector3 v, float dv, ref Vector3 p0, ref Vector3 p1, ref int n)
    {
        if ((du >= 0) == (dv >= 0))
            return;
        var t = du / (du - dv);
        var p = u + (v - u) * t;
        if (n == 0) p0 = p;
        else if (n == 1) p1 = p;
        n++;
    }
}

/// <summary>
/// Keeps only the triangles whose centroid lies inside an axis-aligned crop box. The box is given as
/// fractions of the mesh bounds along each axis (0..1 per component), so cropping is resolution- and
/// scale-independent. Faces whose centroid falls outside the box on any axis are discarded; if nothing
/// survives, the original mesh is returned unchanged.
/// </summary>
[Category(Cat.Cut)]
[Description("Keeps only the triangles whose centroid lies inside a crop box expressed as fractions of the bounds.")]
public class BoxCrop : IModifier
{
    public Vector3 Min = (0f, 0f, 0f);
    public Vector3 Max = (1f, 1f, 1f);

    public int FacesBefore { get; private set; }
    public int FacesAfter { get; private set; }

    public TriangleMesh3D Eval(TriangleMesh3D mesh, EvalContext ctx)
    {
        var bounds = mesh.DerivedBounds();
        var origin = bounds.Min.Vector3;
        var lo = origin + bounds.Size * Min;
        var hi = origin + bounds.Size * Max;

        var kept = new List<int>();
        for (var i = 0; i < mesh.FaceIndices.Count; i++)
        {
            var f = mesh.FaceIndices[i];
            var centroid = (mesh.Points[f.A].Vector3 + mesh.Points[f.B].Vector3 + mesh.Points[f.C].Vector3) / 3f;
            if (Inside(centroid, lo, hi))
                kept.Add(i);
        }

        FacesBefore = mesh.FaceIndices.Count;
        FacesAfter = kept.Count;
        ctx.Services.RefreshUI(this);
        return kept.Count == 0 ? mesh : mesh.ExtractFaces(kept).Mesh;
    }

    static bool Inside(Vector3 p, Vector3 lo, Vector3 hi)
        => p.X >= lo.X && p.X <= hi.X
        && p.Y >= lo.Y && p.Y <= hi.Y
        && p.Z >= lo.Z && p.Z <= hi.Z;
}
