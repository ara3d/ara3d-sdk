namespace Ara3D.Geometry;

/// <summary>
/// 3D Delaunay tetrahedralization (Bowyer-Watson) of a point set, with boundary-face
/// extraction to build the convex hull as a <see cref="TriangleMesh3D"/>.
/// Predicates are evaluated in double precision.
/// </summary>
public static class DelaunayTetrahedralizer
{
    public const double Eps = 1e-10;

    readonly record struct Tet(int A, int B, int C, int D);

    /// <summary>
    /// Delaunay-tetrahedralizes a 3D point set. Returns positively oriented tetrahedra
    /// (vertex D above the CCW triangle ABC) referencing the input point list.
    /// </summary>
    public static IReadOnlyList<Integer4> Tetrahedralize(IReadOnlyList<Point3D> points)
    {
        if (points == null)
            throw new ArgumentNullException(nameof(points));

        if (points.Count < 4)
            return Array.Empty<Integer4>();

        var tets = BowyerWatson(points);
        return tets.Select(t => new Integer4(t.A, t.B, t.C, t.D)).ToList();
    }

    /// <summary>
    /// Extracts the boundary (faces belonging to exactly one tetrahedron) as an
    /// outward-oriented triangle mesh. For a Delaunay tetrahedralization this is the convex hull.
    /// </summary>
    public static TriangleMesh3D BoundaryMesh(IReadOnlyList<Point3D> points, IReadOnlyList<Integer4> tets)
    {
        var faceCounts = new Dictionary<(int, int, int), (Integer3 Face, int Opposite)>();
        var duplicates = new HashSet<(int, int, int)>();

        foreach (var t in tets)
        {
            AddFace(faceCounts, duplicates, t.A, t.B, t.C, t.D);
            AddFace(faceCounts, duplicates, t.A, t.D, t.B, t.C);
            AddFace(faceCounts, duplicates, t.B, t.D, t.C, t.A);
            AddFace(faceCounts, duplicates, t.A, t.C, t.D, t.B);
        }

        var faces = new List<Integer3>();
        foreach (var (key, (face, opposite)) in faceCounts)
        {
            if (duplicates.Contains(key))
                continue;

            faces.Add(OrientAwayFrom(points, face, opposite));
        }

        return new TriangleMesh3D(points, faces);
    }

    static void AddFace(
        Dictionary<(int, int, int), (Integer3, int)> faceCounts,
        HashSet<(int, int, int)> duplicates,
        int a, int b, int c, int opposite)
    {
        var key = SortedKey(a, b, c);
        if (!faceCounts.TryAdd(key, (new Integer3(a, b, c), opposite)))
            duplicates.Add(key);
    }

    static (int, int, int) SortedKey(int a, int b, int c)
    {
        if (a > b) (a, b) = (b, a);
        if (b > c) (b, c) = (c, b);
        if (a > b) (a, b) = (b, a);
        return (a, b, c);
    }

    static Integer3 OrientAwayFrom(IReadOnlyList<Point3D> points, Integer3 face, int opposite)
        => Orient3D(
               points[face.A].Vector3, points[face.B].Vector3,
               points[face.C].Vector3, points[opposite].Vector3) > 0
            ? new Integer3(face.A, face.C, face.B)
            : face;

    static List<Tet> BowyerWatson(IReadOnlyList<Point3D> points)
    {
        var bounds = points.Bounds();
        var extent = bounds.Max.Vector3 - bounds.Min.Vector3;
        var dmax = Math.Max(Math.Max(extent.X, Math.Max(extent.Y, extent.Z)), (float)Eps);
        var center = (bounds.Min.Vector3 + bounds.Max.Vector3) * 0.5f;

        // Super-tetrahedron comfortably containing all points and their circumspheres.
        var s = 100f * dmax;
        var allPoints = new List<Vector3>(points.Count + 4)
        {
            center + new Vector3(-s, -s, -s),
            center + new Vector3(s, s, -s),
            center + new Vector3(s, -s, s),
            center + new Vector3(-s, s, s)
        };
        for (var i = 0; i < points.Count; ++i)
            allPoints.Add(points[i].Vector3);

        var tets = new List<Tet> { MakePositive(allPoints, new Tet(0, 1, 2, 3)) };

        for (var pi = 4; pi < allPoints.Count; ++pi)
        {
            var p = allPoints[pi];
            var badTets = new List<int>();
            for (var ti = 0; ti < tets.Count; ++ti)
            {
                var t = tets[ti];
                if (InCircumsphere(p, allPoints[t.A], allPoints[t.B], allPoints[t.C], allPoints[t.D]))
                    badTets.Add(ti);
            }

            if (badTets.Count == 0)
                continue;

            var boundary = new Dictionary<(int, int, int), (Integer3 Face, int Count)>();
            foreach (var ti in badTets)
            {
                var t = tets[ti];
                AddCavityFace(boundary, t.A, t.B, t.C);
                AddCavityFace(boundary, t.A, t.D, t.B);
                AddCavityFace(boundary, t.B, t.D, t.C);
                AddCavityFace(boundary, t.A, t.C, t.D);
            }

            for (var i = badTets.Count - 1; i >= 0; --i)
                tets.RemoveAt(badTets[i]);

            foreach (var (_, (face, count)) in boundary)
            {
                if (count != 1)
                    continue;

                var tet = MakePositive(allPoints, new Tet(face.A, face.B, face.C, pi));
                if (Math.Abs(Orient3D(allPoints[tet.A], allPoints[tet.B], allPoints[tet.C], allPoints[tet.D])) > Eps)
                    tets.Add(tet);
            }
        }

        return tets
            .Where(t => t.A >= 4 && t.B >= 4 && t.C >= 4 && t.D >= 4)
            .Select(t => new Tet(t.A - 4, t.B - 4, t.C - 4, t.D - 4))
            .ToList();
    }

    static void AddCavityFace(Dictionary<(int, int, int), (Integer3, int)> boundary, int a, int b, int c)
    {
        var key = SortedKey(a, b, c);
        if (boundary.TryGetValue(key, out var existing))
            boundary[key] = (existing.Item1, existing.Item2 + 1);
        else
            boundary[key] = (new Integer3(a, b, c), 1);
    }

    static Tet MakePositive(IReadOnlyList<Vector3> points, Tet t)
        => Orient3D(points[t.A], points[t.B], points[t.C], points[t.D]) < 0
            ? new Tet(t.A, t.C, t.B, t.D)
            : t;

    /// <summary>
    /// Signed volume predicate: positive when d is on the positive side of CCW triangle abc.
    /// </summary>
    static double Orient3D(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        double adx = a.X - d.X, ady = a.Y - d.Y, adz = a.Z - d.Z;
        double bdx = b.X - d.X, bdy = b.Y - d.Y, bdz = b.Z - d.Z;
        double cdx = c.X - d.X, cdy = c.Y - d.Y, cdz = c.Z - d.Z;

        return -(adx * (bdy * cdz - bdz * cdy)
               - ady * (bdx * cdz - bdz * cdx)
               + adz * (bdx * cdy - bdy * cdx));
    }

    static bool InCircumsphere(Vector3 p, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        // Local convention: det[a-d, b-d, c-d] (the negation of Orient3D) must be
        // positive for the insphere determinant below to mean "inside".
        var orient = -Orient3D(a, b, c, d);
        if (Math.Abs(orient) <= Eps)
            return false;

        if (orient < 0)
            (c, d) = (d, c);

        double apx = a.X - p.X, apy = a.Y - p.Y, apz = a.Z - p.Z;
        double bpx = b.X - p.X, bpy = b.Y - p.Y, bpz = b.Z - p.Z;
        double cpx = c.X - p.X, cpy = c.Y - p.Y, cpz = c.Z - p.Z;
        double dpx = d.X - p.X, dpy = d.Y - p.Y, dpz = d.Z - p.Z;

        var ap = apx * apx + apy * apy + apz * apz;
        var bp = bpx * bpx + bpy * bpy + bpz * bpz;
        var cp = cpx * cpx + cpy * cpy + cpz * cpz;
        var dp = dpx * dpx + dpy * dpy + dpz * dpz;

        var abc = apx * (bpy * cpz - bpz * cpy) - apy * (bpx * cpz - bpz * cpx) + apz * (bpx * cpy - bpy * cpx);
        var abd = apx * (bpy * dpz - bpz * dpy) - apy * (bpx * dpz - bpz * dpx) + apz * (bpx * dpy - bpy * dpx);
        var acd = apx * (cpy * dpz - cpz * dpy) - apy * (cpx * dpz - cpz * dpx) + apz * (cpx * dpy - cpy * dpx);
        var bcd = bpx * (cpy * dpz - cpz * dpy) - bpy * (cpx * dpz - cpz * dpx) + bpz * (cpx * dpy - cpy * dpx);

        return dp * abc - cp * abd + bp * acd - ap * bcd > Eps;
    }
}

public static class DelaunayTetrahedralizerExtensions
{
    /// <summary>
    /// Builds the convex hull of a point set via Delaunay tetrahedralization.
    /// </summary>
    public static TriangleMesh3D DelaunayHull(this IReadOnlyList<Point3D> points)
        => DelaunayTetrahedralizer.BoundaryMesh(points, DelaunayTetrahedralizer.Tetrahedralize(points));
}
