using Ara3D.Collections;

namespace Ara3D.Geometry;

/// <summary>
/// 2D Delaunay triangulation (Bowyer-Watson) with helpers to build <see cref="TriangleMesh3D"/> from 3D point sets.
/// </summary>
public static class DelaunayTriangulator
{
    public const float Eps = 1e-6f;

    public enum ProjectionPlane
    {
        XY,
        XZ,
        YZ,
        BestFit
    }

    readonly record struct Triangle(int A, int B, int C);

    /// <summary>
    /// Delaunay-triangulates a 2D point set. Returns triangle indices referencing the input point list.
    /// </summary>
    public static IReadOnlyList<Integer3> Triangulate(IReadOnlyList<Vector2> points)
    {
        if (points == null)
            throw new ArgumentNullException(nameof(points));

        if (points.Count < 3)
            return Array.Empty<Integer3>();

        var workPoints = new List<Vector2>(points);
        var indexMap = Enumerable.Range(0, points.Count).ToList();

        RemoveDuplicatePoints(workPoints, indexMap);
        if (workPoints.Count < 3)
            return Array.Empty<Integer3>();

        var triangles = BowyerWatson(workPoints);
        return triangles
            .Select(t => OrientCCW(
                workPoints[t.A],
                workPoints[t.B],
                workPoints[t.C],
                indexMap[t.A],
                indexMap[t.B],
                indexMap[t.C]))
            .ToList();
    }

    /// <summary>
    /// Projects 3D points onto a plane, Delaunay-triangulates, and returns a mesh using the original 3D coordinates.
    /// </summary>
    public static TriangleMesh3D Triangulate(
        IReadOnlyList<Point3D> points,
        ProjectionPlane plane = ProjectionPlane.XY,
        bool removeDuplicatePoints = true)
    {
        if (points == null)
            throw new ArgumentNullException(nameof(points));

        if (points.Count < 3)
            return new TriangleMesh3D(Array.Empty<Point3D>(), Array.Empty<Integer3>());

        var uniquePoints = removeDuplicatePoints ? RemoveDuplicatePoints3D(points) : points.ToList();
        if (uniquePoints.Count < 3)
            return new TriangleMesh3D(uniquePoints, Array.Empty<Integer3>());

        var projected = ProjectPoints(uniquePoints, plane);
        var faces = Triangulate(projected);
        return new TriangleMesh3D(uniquePoints, faces);
    }

    static List<Point3D> RemoveDuplicatePoints3D(IReadOnlyList<Point3D> points)
    {
        var lookup = new Dictionary<VertexWelder.VertexKey, Point3D>();
        foreach (var point in points)
        {
            var key = VertexWelder.VertexKey.Create(point.Vector3);
            lookup.TryAdd(key, point);
        }

        return lookup.Values.ToList();
    }

    static List<Vector2> ProjectPoints(IReadOnlyList<Point3D> points, ProjectionPlane plane)
    {
        return plane switch
        {
            ProjectionPlane.XY => points.Select(p => new Vector2(p.X, p.Y)).ToList(),
            ProjectionPlane.XZ => points.Select(p => new Vector2(p.X, p.Z)).ToList(),
            ProjectionPlane.YZ => points.Select(p => new Vector2(p.Y, p.Z)).ToList(),
            ProjectionPlane.BestFit => ProjectBestFit(points),
            _ => throw new ArgumentOutOfRangeException(nameof(plane), plane, null)
        };
    }

    static List<Vector2> ProjectBestFit(IReadOnlyList<Point3D> points)
    {
        var vectors = points.Select(p => p.Vector3).ToList();
        var frame = new PrincipalComponentAnalysis(vectors).Frame;
        return points
            .Select(p =>
            {
                var local = frame.ToLocal(p);
                return new Vector2(local.X, local.Y);
            })
            .ToList();
    }

    static void RemoveDuplicatePoints(List<Vector2> points, List<int> indexMap)
    {
        var seen = new HashSet<(long X, long Y)>();
        for (var i = points.Count - 1; i >= 0; --i)
        {
            var p = points[i];
            var key = ((long)MathF.Round(p.X / Eps), (long)MathF.Round(p.Y / Eps));
            if (!seen.Add(key))
            {
                points.RemoveAt(i);
                indexMap.RemoveAt(i);
            }
        }
    }

    static List<Triangle> BowyerWatson(IReadOnlyList<Vector2> points)
    {
        var bounds = points.GetBounds();
        var dx = bounds.Max.X - bounds.Min.X;
        var dy = bounds.Max.Y - bounds.Min.Y;
        var dmax = MathF.Max(MathF.Max(dx, dy), Eps);
        var midX = (bounds.Min.X + bounds.Max.X) * 0.5f;
        var midY = (bounds.Min.Y + bounds.Max.Y) * 0.5f;

        var allPoints = new List<Vector2>(points.Count + 3)
        {
            new(midX - 20f * dmax, midY - dmax),
            new(midX, midY + 20f * dmax),
            new(midX + 20f * dmax, midY - dmax)
        };
        allPoints.AddRange(points);

        var triangles = new List<Triangle>
        {
            new(0, 1, 2)
        };

        for (var pi = 3; pi < allPoints.Count; ++pi)
        {
            var point = allPoints[pi];
            var badTriangles = new List<Triangle>();
            foreach (var triangle in triangles)
            {
                if (InCircumcircle(point, allPoints[triangle.A], allPoints[triangle.B], allPoints[triangle.C]))
                    badTriangles.Add(triangle);
            }

            if (badTriangles.Count == 0)
                continue;

            var edgeCounts = new Dictionary<Integer2, int>();
            foreach (var triangle in badTriangles)
            {
                AddEdge(edgeCounts, triangle.A, triangle.B);
                AddEdge(edgeCounts, triangle.B, triangle.C);
                AddEdge(edgeCounts, triangle.C, triangle.A);
            }

            triangles.RemoveAll(badTriangles.Contains);

            foreach (var (edge, count) in edgeCounts)
            {
                if (count != 1)
                    continue;

                if (SignedArea(allPoints[edge.A], allPoints[edge.B], allPoints[pi]) > Eps)
                    triangles.Add(new Triangle(edge.A, edge.B, pi));
                else
                    triangles.Add(new Triangle(edge.B, edge.A, pi));
            }
        }

        return triangles
            .Where(t => t.A >= 3 && t.B >= 3 && t.C >= 3)
            .Select(t => new Triangle(t.A - 3, t.B - 3, t.C - 3))
            .ToList();
    }

    static void AddEdge(Dictionary<Integer2, int> edgeCounts, int a, int b)
    {
        var edge = a < b ? new Integer2(a, b) : new Integer2(b, a);
        edgeCounts.TryGetValue(edge, out var count);
        edgeCounts[edge] = count + 1;
    }

    static Integer3 OrientCCW(Vector2 a, Vector2 b, Vector2 c, int ia, int ib, int ic)
    {
        if (SignedArea(a, b, c) >= 0)
            return new Integer3(ia, ib, ic);
        return new Integer3(ia, ic, ib);
    }

    static float SignedArea(Vector2 a, Vector2 b, Vector2 c)
        => (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);

    static bool InCircumcircle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        var orient = SignedArea(a, b, c);
        if (MathF.Abs(orient) <= Eps)
            return false;

        if (orient < 0)
            (b, c) = (c, b);

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

        return det > Eps;
    }
}

public static class TriangleMesh3DDelaunayExtensions
{
    public static TriangleMesh3D DelaunayTriangulate(
        this IReadOnlyList<Point3D> points,
        DelaunayTriangulator.ProjectionPlane plane = DelaunayTriangulator.ProjectionPlane.XY,
        bool removeDuplicatePoints = true)
        => DelaunayTriangulator.Triangulate(points, plane, removeDuplicatePoints);

    /// <summary>
    /// Re-triangulates the unique vertices of a mesh using Delaunay triangulation.
    /// </summary>
    public static TriangleMesh3D DelaunayTriangulateVertices(
        this TriangleMesh3D mesh,
        DelaunayTriangulator.ProjectionPlane plane = DelaunayTriangulator.ProjectionPlane.XY)
        => mesh.Points.DelaunayTriangulate(plane);
}
