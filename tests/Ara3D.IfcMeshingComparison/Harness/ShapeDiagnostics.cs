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
}
