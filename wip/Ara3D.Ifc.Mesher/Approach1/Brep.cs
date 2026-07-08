using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.IfcTypes;

namespace Ara3D.Ifc.Mesher.Approach1;

/// <summary>
/// Faceted BRep and shell models via planar projection + ear clipping.
/// Open shells produce single-sided faces.
/// </summary>
public static class Brep
{
    public static TriangleMesh3D BuildFacetedBrep(MeshingContext ctx, IfcEntity brep)
    {
        ctx.Diagnostics.RecordSupported("IFCFACETEDBREP");
        var shell = MeshHelpers.ResolveRequired(ctx, brep, IfcManifoldSolidBrep.Instance.Outer);
        return BuildShell(ctx, shell);
    }

    /// <summary>Advanced BRep: triangulate face bounds; curved face surfaces are ignored.</summary>
    public static TriangleMesh3D BuildAdvancedBrep(MeshingContext ctx, IfcEntity brep)
    {
        ctx.Diagnostics.RecordApproximate("IFCADVANCEDBREP", "Bounds only; curved advanced-face surfaces ignored");
        var shell = MeshHelpers.ResolveRequired(ctx, brep, IfcManifoldSolidBrep.Instance.Outer);
        return BuildShell(ctx, shell);
    }

    public static TriangleMesh3D BuildFaceBasedSurfaceModel(MeshingContext ctx, IfcEntity model)
    {
        ctx.Diagnostics.RecordSupported("IFCFACEBASEDSURFACEMODEL");
        var meshes = MeshHelpers.ReadIds(model, IfcFaceBasedSurfaceModel.Instance.FbsmFaces)
            .Select(id => BuildFaceBasedSurfaceElement(ctx, ctx.GetEntity(id)))
            .ToList();
        return MeshHelpers.Merge(meshes);
    }

    static TriangleMesh3D BuildFaceBasedSurfaceElement(MeshingContext ctx, IfcEntity element)
        => element.GetEntityName() switch
        {
            "IFCCONNECTEDFACESET" => BuildConnectedFaceSet(ctx, element),
            "IFCOPENSHELL" or "IFCOPENEDSHELL" or "IFCCLOSEDSHELL" => BuildShell(ctx, element),
            _ => BuildConnectedFaceSet(ctx, element),
        };

    /// <summary>Unwraps CfsFaces to IFCFACE entities and triangulates each face.</summary>
    public static TriangleMesh3D BuildConnectedFaceSet(MeshingContext ctx, IfcEntity faceSet)
    {
        ctx.Diagnostics.RecordSupported("IFCCONNECTEDFACESET");
        return BuildFaceSet(ctx, MeshHelpers.ReadIds(faceSet, IfcConnectedFaceSet.Instance.CfsFaces));
    }

    public static TriangleMesh3D BuildShellBasedSurfaceModel(MeshingContext ctx, IfcEntity model)
    {
        ctx.Diagnostics.RecordSupported("IFCSHELLBASEDSURFACEMODEL");
        var meshes = MeshHelpers.ReadIds(model, IfcShellBasedSurfaceModel.Instance.SbsmBoundary)
            .Select(id => BuildShell(ctx, ctx.GetEntity(id)))
            .ToList();
        return MeshHelpers.Merge(meshes);
    }

    public static TriangleMesh3D BuildSingleFace(MeshingContext ctx, IfcEntity face)
    {
        var name = face.GetEntityName();
        ctx.Diagnostics.RecordApproximate(name, "Single face via polyloop bounds");
        return BuildFaceSet(ctx, [face.Id]);
    }

    static TriangleMesh3D BuildShell(MeshingContext ctx, IfcEntity shell)
    {
        var faceIds = shell.GetEntityName() switch
        {
            "IFCCLOSEDSHELL" => MeshHelpers.ReadIds(shell, IfcClosedShell.Instance.CfsFaces),
            "IFCOPENSHELL" or "IFCOPENEDSHELL" => MeshHelpers.ReadIds(shell, IfcOpenShell.Instance.CfsFaces),
            _ => throw new NotSupportedException($"Unsupported shell {shell.GetEntityName()}"),
        };
        return BuildFaceSet(ctx, faceIds);
    }

    static TriangleMesh3D BuildFaceSet(MeshingContext ctx, IReadOnlyList<int> faceIds)
    {
        var points = new List<Point3D>();
        var faces = new List<Integer3>();
        var pointMap = new Dictionary<(int, int, int), int>();

        foreach (var faceId in faceIds)
        {
            var face = ctx.GetEntity(faceId);
            RecordFaceDiagnostics(ctx, face);
            var (outer, holes, sameSense) = ReadFaceBounds(ctx, face);
            if (outer.Count < 3)
                continue;

            var plane = ComputeNewellPlane(outer);
            var outer2 = DedupeConsecutive(outer.Select(p => ProjectToPlane2D(p, plane)).ToList());
            var holes2 = holes
                .Select(h => DedupeConsecutive(h.Select(p => ProjectToPlane2D(p, plane)).ToList()))
                .Where(h => h.Count >= 3)
                .ToList();
            if (outer2.Count < 3)
                continue;

            IReadOnlyList<Triangle2D> tris;
            try
            {
                tris = holes2.Count == 1 && PolygonWithHoles.TryTriangulateCongruentRing(outer2, holes2[0], out var ringTris)
                    ? ringTris
                    : PolygonTriangulator.GetTriangles(outer2, holes2);
            }
            catch
            {
                continue;
            }

            var indexMap = new Dictionary<(int, int, int), int>();
            int GetIndex(Vector3 p3)
            {
                var key = Quantize3(p3);
                if (indexMap.TryGetValue(key, out var idx))
                    return idx;
                if (pointMap.TryGetValue(key, out idx))
                {
                    indexMap[key] = idx;
                    return idx;
                }
                idx = points.Count;
                points.Add(p3);
                pointMap[key] = idx;
                indexMap[key] = idx;
                return idx;
            }

            foreach (var tri in tris)
            {
                var a = GetIndex(Unproject(tri.A.Vector2, plane));
                var b = GetIndex(Unproject(tri.B.Vector2, plane));
                var c = GetIndex(Unproject(tri.C.Vector2, plane));
                if (sameSense)
                    faces.Add(new Integer3(a, b, c));
                else
                    faces.Add(new Integer3(a, c, b));
            }
        }

        return new TriangleMesh3D(points, faces);
    }

    static void RecordFaceDiagnostics(MeshingContext ctx, IfcEntity face)
    {
        var name = face.GetEntityName();
        if (name is "IFCADVANCEDFACE" or "IFCFACESURFACE")
        {
            ctx.Diagnostics.RecordApproximate(name, "Bounds only; curved advanced-face surfaces ignored");
            var surface = MeshHelpers.ResolveOptional(ctx, face, IfcFaceSurface.Instance.FaceSurface);
            if (surface?.GetEntityName() == "IFCPLANE")
                ctx.Diagnostics.RecordSupported("IFCPLANE");
        }
    }

    static (List<Vector3> outer, List<List<Vector3>> holes, bool sameSense) ReadFaceBounds(MeshingContext ctx, IfcEntity face)
    {
        var bounds = MeshHelpers.ReadIds(face, IfcFace.Instance.Bounds);
        var outer = new List<Vector3>();
        var holes = new List<List<Vector3>>();
        var sameSense = true;

        foreach (var boundId in bounds)
        {
            var bound = ctx.GetEntity(boundId);
            var loop = MeshHelpers.ResolveRequired(ctx, bound, IfcFaceBound.Instance.Bound);
            var pts = ReadLoop(ctx, loop);
            var isOuter = bound.GetEntityName() == "IFCFACEOUTERBOUND";
            if (isOuter)
                outer = pts;
            else
                holes.Add(pts);
            if (bound.GetEntityName() == "IFCFACEBOUND" && bound.GetString(1).Contains(".F."))
                sameSense = false;
        }
        return (outer, holes, sameSense);
    }

    static List<Vector3> ReadLoop(MeshingContext ctx, IfcEntity loop)
    {
        if (loop.GetEntityName() != "IFCPOLYLOOP")
            throw new NotSupportedException($"Unsupported loop {loop.GetEntityName()}");

        var points = MeshHelpers.ReadIds(loop, IfcPolyLoop.Instance.Polygon)
            .Select(id => Placements.ReadPoint3D(ctx, ctx.GetEntity(id)))
            .ToList();
        return DedupeConsecutive3D(points);
    }

    static List<Vector3> DedupeConsecutive3D(IReadOnlyList<Vector3> points)
    {
        if (points.Count < 2)
            return points.ToList();

        const float epsSq = 1e-12f;
        var cleaned = new List<Vector3> { points[0] };
        for (var i = 1; i < points.Count; i++)
        {
            if ((points[i] - cleaned[^1]).LengthSquared() > epsSq)
                cleaned.Add(points[i]);
        }

        if (cleaned.Count > 1 && (cleaned[0] - cleaned[^1]).LengthSquared() <= epsSq)
            cleaned.RemoveAt(cleaned.Count - 1);
        return cleaned;
    }

    static List<Vector2> DedupeConsecutive(IReadOnlyList<Vector2> points)
    {
        if (points.Count < 2)
            return points.ToList();

        var epsSq = PolygonTriangulator.Eps * PolygonTriangulator.Eps;
        var cleaned = new List<Vector2> { points[0] };
        for (var i = 1; i < points.Count; i++)
        {
            if (points[i].DistanceSquared(cleaned[^1]) > epsSq)
                cleaned.Add(points[i]);
        }

        if (cleaned.Count > 1 && cleaned[0].DistanceSquared(cleaned[^1]) <= epsSq)
            cleaned.RemoveAt(cleaned.Count - 1);
        return cleaned;
    }

    readonly record struct FacePlane(Vector3 Origin, Vector3 Normal, Vector3 U, Vector3 V);

    static FacePlane ComputeNewellPlane(IReadOnlyList<Vector3> points)
    {
        var normal = Vector3.Zero;
        float nx = 0, ny = 0, nz = 0;
        for (var i = 0; i < points.Count; i++)
        {
            var p0 = points[i];
            var p1 = points[(i + 1) % points.Count];
            nx += (p0.Y - p1.Y) * (p0.Z + p1.Z);
            ny += (p0.Z - p1.Z) * (p0.X + p1.X);
            nz += (p0.X - p1.X) * (p0.Y + p1.Y);
        }
        normal = new Vector3(nx, ny, nz);
        if (normal.LengthSquared() < 1e-12f)
            normal = Vector3.UnitZ;
        else
            normal = normal.Normalize;
        var u = MathF.Abs(normal.Y) < 0.9f
            ? Vector3.Cross(Vector3.UnitY, normal).Normalize
            : Vector3.Cross(Vector3.UnitX, normal).Normalize;
        var v = Vector3.Cross(normal, u);
        return new FacePlane(points[0], normal, u, v);
    }

    static Vector2 ProjectToPlane2D(Vector3 p, FacePlane plane)
    {
        var d = p - plane.Origin;
        return new Vector2(Vector3.Dot(d, plane.U), Vector3.Dot(d, plane.V));
    }

    static Vector3 Unproject(Vector2 p2, FacePlane plane)
        => plane.Origin + plane.U * p2.X + plane.V * p2.Y;

    static (int, int, int) Quantize3(Vector3 p)
        => ((int)MathF.Round(p.X * 1e5f), (int)MathF.Round(p.Y * 1e5f), (int)MathF.Round(p.Z * 1e5f));
}
