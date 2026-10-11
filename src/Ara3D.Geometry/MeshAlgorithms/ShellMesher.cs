namespace Ara3D.Geometry;

/// <summary>A shell of planar faces as plain arrays: its points, each face's outer loop (profile)
/// as point indices, and each hole as the profile it cuts and its loop.</summary>
public sealed record ShellLoops(
    IReadOnlyList<Point3D> Points,
    IReadOnlyList<int[]> Profiles,
    IReadOnlyList<(int Profile, int[] Loop)> Holes);

/// <summary>Triangulates planar faces given as loops of point indices. Each face, possibly with
/// holes, is projected onto the coordinate plane its normal is most aligned with, triangulated by
/// <see cref="Earcut"/>, and each triangle is wound to face the way the outer loop does (its
/// Newell normal). Triangles index the input points; no point is added.</summary>
public static class ShellMesher
{
    /// <summary>The mesh in the shell's own coordinates, with every point of the shell, and the
    /// number of profiles skipped because they are degenerate or index outside the points.</summary>
    public static (TriangleMesh3D Mesh, int SkippedProfiles) Triangulate(ShellLoops shell)
    {
        var holesByProfile = shell.Holes.ToLookup(h => h.Profile, h => (IReadOnlyList<int>)h.Loop);
        var faces = new List<Integer3>();
        var skipped = 0;
        for (var p = 0; p < shell.Profiles.Count; p++)
            if (!TriangulateFace(shell.Points, shell.Profiles[p], holesByProfile[p].ToList(), faces))
                skipped++;
        return (new TriangleMesh3D(shell.Points, faces), skipped);
    }

    /// <summary>Appends the triangles of one planar face to <paramref name="faces"/>. Returns false,
    /// adding nothing, when the outer loop has fewer than three points or no area, or a loop
    /// indexes outside <paramref name="points"/> or reaches a point that is not finite.</summary>
    public static bool TriangulateFace(
        IReadOnlyList<Point3D> points,
        IReadOnlyList<int> outer,
        IReadOnlyList<IReadOnlyList<int>> holes,
        List<Integer3> faces)
    {
        if (outer.Count < 3 || holes.Prepend(outer).Any(loop => loop.Any(i => (uint)i >= (uint)points.Count || !IsFinite(points[i]))))
            return false;

        var normal = NewellNormal(points, outer);
        if (normal.LengthSquared() == 0)
            return false;

        if (outer.Count == 3 && holes.Count == 0)
        {
            faces.Add(Oriented(points, outer[0], outer[1], outer[2], normal));
            return true;
        }

        // Project onto the plane that drops the normal's largest component, keeping the remaining
        // two in cyclic order (y,z), (z,x), or (x,y).
        var axis = MaxAxis(normal);
        var flat = new List<double>(2 * (outer.Count + holes.Sum(h => h.Count)));
        var holeStarts = new List<int>(holes.Count);
        var pointOf = new List<int>(flat.Capacity / 2);
        foreach (var loop in holes.Prepend(outer))
        {
            if (pointOf.Count > 0)
                holeStarts.Add(pointOf.Count);
            foreach (var i in loop)
            {
                var v = points[i];
                var (u, w) = axis switch { 0 => (v.Y, v.Z), 1 => (v.Z, v.X), _ => (v.X, v.Y) };
                flat.Add(u.Value);
                flat.Add(w.Value);
                pointOf.Add(i);
            }
        }

        var triangles = Earcut.Triangulate(flat, holeStarts);
        for (var t = 0; t + 2 < triangles.Count; t += 3)
            faces.Add(Oriented(points, pointOf[triangles[t]], pointOf[triangles[t + 1]], pointOf[triangles[t + 2]], normal));
        return true;
    }

    /// <summary>The Newell normal of a loop: the sum of its edges' cross products, robust for
    /// concave and nearly collinear loops. Its length is twice the loop's area.</summary>
    public static Vector3 NewellNormal(IReadOnlyList<Point3D> points, IReadOnlyList<int> loop)
    {
        double x = 0, y = 0, z = 0;
        for (var i = 0; i < loop.Count; i++)
        {
            var a = points[loop[i]];
            var b = points[loop[(i + 1) % loop.Count]];
            x += ((double)a.Y.Value - b.Y.Value) * ((double)a.Z.Value + b.Z.Value);
            y += ((double)a.Z.Value - b.Z.Value) * ((double)a.X.Value + b.X.Value);
            z += ((double)a.X.Value - b.X.Value) * ((double)a.Y.Value + b.Y.Value);
        }
        return new Vector3((float)x, (float)y, (float)z);
    }

    private static Integer3 Oriented(IReadOnlyList<Point3D> points, int a, int b, int c, Vector3 normal)
    {
        var n = (points[b] - points[a]).Cross(points[c] - points[a]);
        return n.Dot(normal) < 0 ? new Integer3(a, c, b) : new Integer3(a, b, c);
    }

    private static int MaxAxis(Vector3 n)
    {
        var (x, y, z) = (MathF.Abs(n.X.Value), MathF.Abs(n.Y.Value), MathF.Abs(n.Z.Value));
        return x >= y && x >= z ? 0 : y >= z ? 1 : 2;
    }

    private static bool IsFinite(Point3D p)
        => float.IsFinite(p.X.Value) && float.IsFinite(p.Y.Value) && float.IsFinite(p.Z.Value);
}
