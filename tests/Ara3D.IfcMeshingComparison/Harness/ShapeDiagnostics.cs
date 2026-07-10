using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.Models;

namespace Ara3D.IfcMeshingComparison.Harness;

/// <summary>
/// One shared entity's Tier 1 overlap diagnostics (all in [0,1], higher = better agreement).
/// </summary>
/// <param name="VoxelIoU">Surface-voxel occupancy intersection-over-union on a shared grid.</param>
/// <param name="ObbIoU">Oriented-bounding-box occupancy IoU (box orientation + placement + extent).</param>
/// <param name="SilhouetteXY">Voxel-silhouette IoU looking down each world axis (Z / Y / X dropped).</param>
public sealed record EntityTier1Diagnostic(
    int EntityId,
    double VoxelIoU,
    double ObbIoU,
    double SilhouetteXY,
    double SilhouetteXZ,
    double SilhouetteYZ);

/// <summary>
/// Directed and symmetric surface-distance diagnostics for one shared entity (lower = better agreement).
/// Distances are in world units; Chamfer is the mean vertex-to-surface distance, Hausdorff is the max.
/// </summary>
public sealed record EntityTier2Diagnostic(
    int EntityId,
    double ChamferAtoB,
    double ChamferBtoA,
    double ChamferSymmetric,
    double HausdorffAtoB,
    double HausdorffBtoA,
    double HausdorffSymmetric,
    double? ConvexHullIoU);

/// <summary>Mesh-level surface distance readout (candidate → oracle naming in entity comparisons).</summary>
public sealed record MeshSurfaceDistance(
    double ChamferAtoB,
    double ChamferBtoA,
    double ChamferSymmetric,
    double HausdorffAtoB,
    double HausdorffBtoA,
    double HausdorffSymmetric,
    double? ConvexHullIoU);

/// <summary>
/// Tier 1 on-demand per-entity shape diagnostics. Unlike the Tier 0 <c>entityShape</c> metric (which
/// is rotation/translation-invariant and so ignores placement), these voxelise both meshes in a shared
/// world-space grid and are <em>position- and orientation-sensitive</em>: they catch "right shape,
/// wrong place/orientation". They are not part of the parity score — they voxelise per entity, so run
/// them explicitly when localising the mesh-bbox frontier.
/// </summary>
public static class ShapeDiagnostics
{
    /// <summary>Per shared entity, worst (lowest voxel IoU) first.</summary>
    public static IReadOnlyList<EntityTier1Diagnostic> CompareEntities(
        Model3D candidate,
        Model3D oracle,
        int gridResolution = 20)
    {
        var mine = ModelComparer.EntityMeshes(candidate);
        var theirs = ModelComparer.EntityMeshes(oracle);
        var result = new List<EntityTier1Diagnostic>();
        foreach (var id in mine.Keys.Intersect(theirs.Keys))
        {
            var diag = Compare(id, mine[id], theirs[id], gridResolution);
            if (diag is not null)
                result.Add(diag);
        }
        return result.OrderBy(r => r.VoxelIoU).ToList();
    }

    static EntityTier1Diagnostic? Compare(int id, TriangleMesh3D a, TriangleMesh3D b, int n)
    {
        if (a.FaceIndices.Count == 0 || b.FaceIndices.Count == 0)
            return null;

        var bounds = MeshHelpers.GetBounds(a).Merge(MeshHelpers.GetBounds(b));
        var size = bounds.Max.Vector3 - bounds.Min.Vector3;
        var maxExt = MathF.Max(size.X.Value, MathF.Max(size.Y.Value, size.Z.Value));
        if (maxExt < 1e-9f)
            return null;

        var cell = maxExt / n;
        var min = bounds.Min.Vector3;
        var nx = Math.Clamp((int)MathF.Ceiling(size.X.Value / cell), 1, n);
        var ny = Math.Clamp((int)MathF.Ceiling(size.Y.Value / cell), 1, n);
        var nz = Math.Clamp((int)MathF.Ceiling(size.Z.Value / cell), 1, n);

        var occA = VoxelizeSurface(a, min, cell, nx, ny, nz);
        var occB = VoxelizeSurface(b, min, cell, nx, ny, nz);
        var voxelIoU = IoU(occA, occB);
        var silXY = SilhouetteIoU(occA, occB, drop: 2);
        var silXZ = SilhouetteIoU(occA, occB, drop: 1);
        var silYZ = SilhouetteIoU(occA, occB, drop: 0);

        var obbA = VoxelizeBox(SafeObb(a), min, cell, nx, ny, nz);
        var obbB = VoxelizeBox(SafeObb(b), min, cell, nx, ny, nz);
        var obbIoU = IoU(obbA, obbB);

        return new EntityTier1Diagnostic(id, voxelIoU, obbIoU, silXY, silXZ, silYZ);
    }

    // Mark every grid cell touched by a triangle (barycentric sampling at ~cell resolution).
    static HashSet<(int, int, int)> VoxelizeSurface(TriangleMesh3D m, Vector3 min, float cell, int nx, int ny, int nz)
    {
        var set = new HashSet<(int, int, int)>();

        void Mark(Vector3 p)
        {
            var i = Math.Clamp((int)((p.X.Value - min.X.Value) / cell), 0, nx - 1);
            var j = Math.Clamp((int)((p.Y.Value - min.Y.Value) / cell), 0, ny - 1);
            var k = Math.Clamp((int)((p.Z.Value - min.Z.Value) / cell), 0, nz - 1);
            set.Add((i, j, k));
        }

        foreach (var f in m.FaceIndices)
        {
            var a = m.Points[f.A].Vector3;
            var b = m.Points[f.B].Vector3;
            var c = m.Points[f.C].Vector3;
            var longest = MathF.Sqrt(MathF.Max(
                (b - a).LengthSquared.Value,
                MathF.Max((c - b).LengthSquared.Value, (a - c).LengthSquared.Value)));
            var steps = Math.Clamp((int)MathF.Ceiling(longest / cell), 1, 16);
            for (var s = 0; s <= steps; s++)
            for (var t = 0; t + s <= steps; t++)
            {
                var u = (float)s / steps;
                var v = (float)t / steps;
                Mark(a + (b - a) * u + (c - a) * v);
            }
        }
        return set;
    }

    // Oriented box for a mesh, falling back to an axis-aligned box when the point set is too degenerate
    // (collinear / coincident) for the PCA frame that FitOrientedBox needs.
    static OrientedBox3D SafeObb(TriangleMesh3D m)
    {
        try
        {
            return m.Points.FitOrientedBox();
        }
        catch
        {
            var b = MeshHelpers.GetBounds(m);
            var c = b.Center.Vector3;
            var identity = new OrthonormalBasis3D(new Axes3D(Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ), true);
            return new OrientedBox3D(new Frame3D(new Point3D(c.X, c.Y, c.Z), identity), b.Max.Vector3 - b.Min.Vector3);
        }
    }

    // Mark every grid cell whose center lies inside the oriented box (frame origin = centroid, Size = full extents).
    static HashSet<(int, int, int)> VoxelizeBox(OrientedBox3D box, Vector3 min, float cell, int nx, int ny, int nz)
    {
        var set = new HashSet<(int, int, int)>();
        var hx = box.Size.X.Value * 0.5f + 1e-6f;
        var hy = box.Size.Y.Value * 0.5f + 1e-6f;
        var hz = box.Size.Z.Value * 0.5f + 1e-6f;
        for (var i = 0; i < nx; i++)
        for (var j = 0; j < ny; j++)
        for (var k = 0; k < nz; k++)
        {
            var center = min + new Vector3((i + 0.5f) * cell, (j + 0.5f) * cell, (k + 0.5f) * cell);
            var local = box.Frame.ToLocal(center);
            if (MathF.Abs(local.X.Value) <= hx && MathF.Abs(local.Y.Value) <= hy && MathF.Abs(local.Z.Value) <= hz)
                set.Add((i, j, k));
        }
        return set;
    }

    static double SilhouetteIoU(HashSet<(int, int, int)> a, HashSet<(int, int, int)> b, int drop)
    {
        static HashSet<(int, int)> Project(HashSet<(int, int, int)> s, int drop)
        {
            var p = new HashSet<(int, int)>();
            foreach (var (i, j, k) in s)
                p.Add(drop == 0 ? (j, k) : drop == 1 ? (i, k) : (i, j));
            return p;
        }
        return IoU(Project(a, drop), Project(b, drop));
    }

    static double IoU<T>(HashSet<T> a, HashSet<T> b)
    {
        if (a.Count == 0 && b.Count == 0)
            return 1.0;
        if (a.Count == 0 || b.Count == 0)
            return 0.0;
        var (small, large) = a.Count <= b.Count ? (a, b) : (b, a);
        var inter = 0;
        foreach (var x in small)
            if (large.Contains(x))
                inter++;
        return (double)inter / (a.Count + b.Count - inter);
    }

    // --- Tier 2: Chamfer / Hausdorff surface distance + optional convex-hull IoU -----------------

    /// <summary>Per shared entity, worst (highest symmetric Hausdorff) first.</summary>
    public static IReadOnlyList<EntityTier2Diagnostic> CompareEntitiesTier2(
        Model3D candidate,
        Model3D oracle,
        int gridResolution = 20,
        int maxHullVertices = 100)
    {
        var mine = ModelComparer.EntityMeshes(candidate);
        var theirs = ModelComparer.EntityMeshes(oracle);
        var result = new List<EntityTier2Diagnostic>();
        foreach (var id in mine.Keys.Intersect(theirs.Keys))
        {
            var diag = CompareTier2(id, mine[id], theirs[id], gridResolution, maxHullVertices);
            if (diag is not null)
                result.Add(diag);
        }
        return result.OrderByDescending(r => r.HausdorffSymmetric).ToList();
    }

    /// <summary>Surface-distance metrics between two meshes in the same frame.</summary>
    public static MeshSurfaceDistance? CompareMeshSurfaces(
        TriangleMesh3D a,
        TriangleMesh3D b,
        int gridResolution = 20,
        int maxHullVertices = 100)
    {
        if (a.FaceIndices.Count == 0 || b.FaceIndices.Count == 0)
            return null;

        var indexB = MeshSurfaceIndex.Build(b);
        var aToB = DirectedSurfaceDistance(a, indexB);
        var indexA = MeshSurfaceIndex.Build(a);
        var bToA = DirectedSurfaceDistance(b, indexA);
        var hullIoU = TryConvexHullIoU(a, b, gridResolution, maxHullVertices);
        return new MeshSurfaceDistance(
            aToB.Chamfer,
            bToA.Chamfer,
            (aToB.Chamfer + bToA.Chamfer) * 0.5,
            aToB.Hausdorff,
            bToA.Hausdorff,
            Math.Max(aToB.Hausdorff, bToA.Hausdorff),
            hullIoU);
    }

    static EntityTier2Diagnostic? CompareTier2(
        int id,
        TriangleMesh3D a,
        TriangleMesh3D b,
        int gridResolution,
        int maxHullVertices)
    {
        var metrics = CompareMeshSurfaces(a, b, gridResolution, maxHullVertices);
        if (metrics is null)
            return null;

        return new EntityTier2Diagnostic(
            id,
            metrics.ChamferAtoB,
            metrics.ChamferBtoA,
            metrics.ChamferSymmetric,
            metrics.HausdorffAtoB,
            metrics.HausdorffBtoA,
            metrics.HausdorffSymmetric,
            metrics.ConvexHullIoU);
    }

    readonly struct DirectedDistance(double chamfer, double hausdorff)
    {
        public double Chamfer { get; } = chamfer;
        public double Hausdorff { get; } = hausdorff;
    }

    sealed class MeshSurfaceIndex
    {
        public required IReadOnlyList<Triangle3D> Triangles { get; init; }
        public required AabbTree Tree { get; init; }

        public static MeshSurfaceIndex Build(TriangleMesh3D mesh)
        {
            var triangles = new Triangle3D[mesh.FaceIndices.Count];
            var bounds = new Bounds3D[triangles.Length];
            for (var i = 0; i < triangles.Length; i++)
            {
                var tri = mesh.GetTriangle(i);
                triangles[i] = tri;
                bounds[i] = tri.Bounds();
            }
            return new MeshSurfaceIndex
            {
                Triangles = triangles,
                Tree = bounds.ToAabbTree(),
            };
        }
    }

    static DirectedDistance DirectedSurfaceDistance(TriangleMesh3D from, MeshSurfaceIndex to)
    {
        double sum = 0;
        var max = 0.0;
        var count = 0;
        foreach (var pt in from.Points)
        {
            var d = NearestSurfaceDistance(pt, to);
            sum += d;
            if (d > max)
                max = d;
            count++;
        }
        foreach (var f in from.FaceIndices)
        {
            var a = from.Points[f.A].Vector3;
            var b = from.Points[f.B].Vector3;
            var c = from.Points[f.C].Vector3;
            var centroid = new Point3D(
                (a.X + b.X + c.X) / 3f,
                (a.Y + b.Y + c.Y) / 3f,
                (a.Z + b.Z + c.Z) / 3f);
            var d = NearestSurfaceDistance(centroid, to);
            sum += d;
            if (d > max)
                max = d;
            count++;
        }
        return new DirectedDistance(count > 0 ? sum / count : 0, max);
    }

    static double NearestSurfaceDistance(Point3D point, MeshSurfaceIndex target)
    {
        var query = new PointClosestTriangleQuery(point, target.Triangles);
        target.Tree.Traverse(ref query);
        return Math.Sqrt(query.BestDistanceSq);
    }

    struct PointClosestTriangleQuery : IAabbTreeQuery
    {
        public Point3D Point;
        public IReadOnlyList<Triangle3D> Triangles;
        public float BestDistanceSq;

        public PointClosestTriangleQuery(Point3D point, IReadOnlyList<Triangle3D> triangles)
        {
            Point = point;
            Triangles = triangles;
            BestDistanceSq = float.PositiveInfinity;
        }

        public bool ShouldStop => false;

        public bool ShouldVisit(in Bounds3D bounds, out float priority)
        {
            var d = DistancePointToBounds(Point, in bounds);
            priority = d * d;
            return d * d <= BestDistanceSq;
        }

        public bool Visit(int index, in Bounds3D bounds)
        {
            if (!ShouldVisit(in bounds, out _))
                return false;

            var d = DistancePointToTriangle(Point, Triangles[index]);
            var dSq = (float)(d * d);
            if (dSq < BestDistanceSq)
                BestDistanceSq = dSq;
            return true;
        }
    }

    static float DistancePointToBounds(Point3D p, in Bounds3D b)
    {
        static float AxisDist(float v, float min, float max) =>
            v < min ? min - v : v > max ? v - max : 0f;

        var dx = AxisDist(p.X.Value, b.Min.X.Value, b.Max.X.Value);
        var dy = AxisDist(p.Y.Value, b.Min.Y.Value, b.Max.Y.Value);
        var dz = AxisDist(p.Z.Value, b.Min.Z.Value, b.Max.Z.Value);
        return MathF.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    static double DistancePointToTriangle(Point3D point, Triangle3D tri)
    {
        var p = point.Vector3;
        var a = tri.A.Vector3;
        var ab = tri.B.Vector3 - a;
        var ac = tri.C.Vector3 - a;
        var ap = p - a;

        var d1 = Vector3.Dot(ab, ap);
        var d2 = Vector3.Dot(ac, ap);
        if (d1 <= 0 && d2 <= 0)
            return (p - a).Length();

        var bp = p - tri.B.Vector3;
        var d3 = Vector3.Dot(ab, bp);
        var d4 = Vector3.Dot(ac, bp);
        if (d3 >= 0 && d4 <= d3)
            return (p - tri.B.Vector3).Length();

        var vc = d1 * d4 - d3 * d2;
        if (vc <= 0 && d1 >= 0 && d3 <= 0)
        {
            var v = d1 / (d1 - d3);
            return (p - (a + v * ab)).Length();
        }

        var cp = p - tri.C.Vector3;
        var d5 = Vector3.Dot(ab, cp);
        var d6 = Vector3.Dot(ac, cp);
        if (d6 >= 0 && d5 <= d6)
            return (p - tri.C.Vector3).Length();

        var vb = d5 * d2 - d1 * d6;
        if (vb <= 0 && d2 >= 0 && d6 <= 0)
        {
            var w = d2 / (d2 - d6);
            return (p - (a + w * ac)).Length();
        }

        var va = d3 * d6 - d5 * d4;
        if (va <= 0 && d4 - d3 >= 0 && d5 - d6 >= 0)
        {
            var w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
            return (p - (tri.B.Vector3 + w * (tri.C.Vector3 - tri.B.Vector3))).Length();
        }

        var denom = 1f / (va + vb + vc);
        var v2 = vb * denom;
        var w2 = vc * denom;
        return (p - (a + ab * v2 + ac * w2)).Length();
    }

    static double? TryConvexHullIoU(
        TriangleMesh3D a,
        TriangleMesh3D b,
        int gridResolution,
        int maxHullVertices)
    {
        if (!TryConvexHullMesh(a, maxHullVertices, out var hullMeshA))
            return null;
        if (!TryConvexHullMesh(b, maxHullVertices, out var hullMeshB))
            return null;

        var bounds = MeshHelpers.GetBounds(hullMeshA).Merge(MeshHelpers.GetBounds(hullMeshB));
        var size = bounds.Max.Vector3 - bounds.Min.Vector3;
        var maxExt = MathF.Max(size.X.Value, MathF.Max(size.Y.Value, size.Z.Value));
        if (maxExt < 1e-9f)
            return null;

        var cell = maxExt / gridResolution;
        var min = bounds.Min.Vector3;
        var nx = Math.Clamp((int)MathF.Ceiling(size.X.Value / cell), 1, gridResolution);
        var ny = Math.Clamp((int)MathF.Ceiling(size.Y.Value / cell), 1, gridResolution);
        var nz = Math.Clamp((int)MathF.Ceiling(size.Z.Value / cell), 1, gridResolution);

        var occA = VoxelizeSurface(hullMeshA, min, cell, nx, ny, nz);
        var occB = VoxelizeSurface(hullMeshB, min, cell, nx, ny, nz);
        return IoU(occA, occB);
    }

    static bool TryConvexHullMesh(TriangleMesh3D mesh, int maxVertices, out TriangleMesh3D hullMesh)
    {
        hullMesh = default;
        var points = SubsampleVertices(mesh.Points, maxVertices);
        if (points.Count < 4)
            return false;

        var faces = new List<Integer3>();
        var n = points.Count;
        const float eps = 1e-6f;
        for (var i = 0; i < n; i++)
        for (var j = i + 1; j < n; j++)
        for (var k = j + 1; k < n; k++)
        {
            var tri = new Triangle3D(points[i], points[j], points[k]);
            var normal = tri.Normal;
            if (normal.LengthSquared() < eps * eps)
                continue;

            var hasPositive = false;
            var hasNegative = false;
            for (var m = 0; m < n; m++)
            {
                if (m == i || m == j || m == k)
                    continue;

                var d = Vector3.Dot(points[m].Vector3 - points[i].Vector3, normal);
                if (d > eps)
                    hasPositive = true;
                if (d < -eps)
                    hasNegative = true;
                if (hasPositive && hasNegative)
                    break;
            }

            if (!hasPositive || !hasNegative)
                faces.Add(new Integer3(i, j, k));
        }

        if (faces.Count == 0)
            return false;

        hullMesh = new TriangleMesh3D(points, faces);
        return true;
    }

    static List<Point3D> SubsampleVertices(IReadOnlyList<Point3D> points, int maxVertices)
    {
        var unique = new List<Point3D>();
        var seen = new HashSet<(int, int, int)>();
        const float quantize = 1e-5f;
        foreach (var p in points)
        {
            var key = (
                (int)MathF.Round(p.X.Value / quantize),
                (int)MathF.Round(p.Y.Value / quantize),
                (int)MathF.Round(p.Z.Value / quantize));
            if (seen.Add(key))
                unique.Add(p);
        }

        if (unique.Count <= maxVertices)
            return unique;

        var step = (float)unique.Count / maxVertices;
        var sampled = new List<Point3D>(maxVertices);
        for (var i = 0; i < maxVertices; i++)
            sampled.Add(unique[(int)(i * step)]);
        return sampled;
    }
}
