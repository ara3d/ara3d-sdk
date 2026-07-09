using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.IfcTypes;

namespace Ara3D.Ifc.Mesher.Approach1;

/// <summary>
/// Subtracts IFCOPENINGELEMENT solids from their host products, following IFCRELVOIDSELEMENT
/// relations. Uses a self-contained convex-solid carve (opening prisms are convex extrusions):
/// host triangles fully inside the prism are dropped, straddling triangles are split so the
/// portion outside the prism is retained (producing the reveal "frame"). Does not perform
/// general CSG and leaves non-convex openings untouched.
/// </summary>
public static class OpeningCarver
{
    /// <summary>host express id -&gt; opening express ids to subtract.</summary>
    public static Dictionary<int, List<int>> CollectVoidRelations(MeshingContext ctx)
    {
        var map = new Dictionary<int, List<int>>();
        foreach (var e in ctx.Resolver.GetEntities())
        {
            if (e.GetEntityName() != "IFCRELVOIDSELEMENT")
                continue;
            var hostId = MeshHelpers.ReadOptionalId(e, IfcRelVoidsElement.Instance.RelatingBuildingElement);
            var openId = MeshHelpers.ReadOptionalId(e, IfcRelVoidsElement.Instance.RelatedOpeningElement);
            if (hostId is null || openId is null)
                continue;
            if (!map.TryGetValue(hostId.Value, out var list))
                map[hostId.Value] = list = new List<int>();
            list.Add(openId.Value);
        }
        return map;
    }

    /// <summary>Builds the opening solid meshes in world coordinates (may be several rep items).</summary>
    public static List<TriangleMesh3D> BuildOpeningWorldSolids(MeshingContext ctx, int openingId)
    {
        var result = new List<TriangleMesh3D>();
        var opening = ctx.GetEntityOrDefault(openingId);
        if (opening is null)
            return result;

        var rep = MeshHelpers.ResolveOptional(ctx, opening, IfcProduct.Instance.Representation);
        if (rep is null)
            return result;

        var parts = new List<CollectedPart>();
        ctx.Try(() => GeometryPartCollector.CollectParts(ctx, rep, Matrix4x4.Identity, openingId, parts),
            "IFCOPENINGELEMENT", $"opening #{openingId}");
        if (parts.Count == 0)
            return result;

        var placement = MeshHelpers.ResolveOptional(ctx, opening, IfcProduct.Instance.ObjectPlacement);
        var world = placement is null
            ? Matrix4x4.Identity
            : Placements.ReadLocalPlacement(ctx, placement).Matrix;

        foreach (var part in parts)
        {
            var local = MeshHelpers.Transform(part.Mesh, part.Transform);
            result.Add(MeshHelpers.Transform(local, world));
        }
        return result;
    }

    readonly record struct Plane(Vector3 Point, Vector3 Normal);

    /// <summary>
    /// Returns the host mesh with the (convex) prism volume carved out, or the original mesh
    /// unchanged when the prism is degenerate / non-convex / disjoint.
    /// </summary>
    public static TriangleMesh3D CarveConvex(TriangleMesh3D host, TriangleMesh3D prism)
    {
        if (host.FaceIndices.Count == 0 || prism.FaceIndices.Count == 0)
            return host;

        var planes = DerivePlanes(prism);
        if (planes is null || planes.Count < 4)
            return host;

        // Prism AABB (with margin) for cheap rejection of far triangles.
        var min = prism.Points[0].Vector3;
        var max = min;
        foreach (var p in prism.Points)
        {
            var v = p.Vector3;
            min = Min(min, v);
            max = Max(max, v);
        }
        const float margin = 1e-4f;
        min -= new Vector3(margin, margin, margin);
        max += new Vector3(margin, margin, margin);

        const float eps = 1e-5f;
        var outPoints = new List<Point3D>();
        var outFaces = new List<Integer3>();
        var index = new Dictionary<(int, int, int), int>();

        int AddPoint(Vector3 v)
        {
            var key = ((int)MathF.Round(v.X * 1e5f), (int)MathF.Round(v.Y * 1e5f), (int)MathF.Round(v.Z * 1e5f));
            if (index.TryGetValue(key, out var i))
                return i;
            i = outPoints.Count;
            outPoints.Add(new Point3D(v.X, v.Y, v.Z));
            index[key] = i;
            return i;
        }

        void EmitTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            if (Vector3.Cross(b - a, c - a).LengthSquared() < 1e-16f)
                return;
            outFaces.Add(new Integer3(AddPoint(a), AddPoint(b), AddPoint(c)));
        }

        void EmitFan(List<Vector3> poly)
        {
            for (var i = 1; i + 1 < poly.Count; i++)
                EmitTriangle(poly[0], poly[i], poly[i + 1]);
        }

        var anyCarved = false;
        foreach (var face in host.FaceIndices)
        {
            var a = host.Points[face.A].Vector3;
            var b = host.Points[face.B].Vector3;
            var c = host.Points[face.C].Vector3;

            // Cheap AABB reject: keep triangles clear of the prism verbatim.
            if (!TriangleOverlapsBox(a, b, c, min, max))
            {
                EmitTriangle(a, b, c);
                continue;
            }

            // Separating face-plane test: entirely outside one plane => keep verbatim.
            var separated = false;
            foreach (var pl in planes)
            {
                var da = Vector3.Dot(a - pl.Point, pl.Normal);
                var db = Vector3.Dot(b - pl.Point, pl.Normal);
                var dc = Vector3.Dot(c - pl.Point, pl.Normal);
                if (da > eps && db > eps && dc > eps)
                {
                    separated = true;
                    break;
                }
            }
            if (separated)
            {
                EmitTriangle(a, b, c);
                continue;
            }

            // Split triangle, collecting the pieces lying outside the prism.
            var current = new List<Vector3> { a, b, c };
            var producedFragment = false;
            foreach (var pl in planes)
            {
                var (inside, outside) = SplitPolygon(current, pl, eps);
                if (outside.Count >= 3)
                {
                    EmitFan(outside);
                    producedFragment = true;
                }
                current = inside;
                if (current.Count < 3)
                    break;
            }
            // 'current' (if any) is fully inside the prism -> removed (the hole).
            if (producedFragment || current.Count < 3)
                anyCarved = true;
            else
                EmitTriangle(a, b, c); // never actually inside; retain
        }

        return anyCarved ? new TriangleMesh3D(outPoints, outFaces) : host;
    }

    static List<Plane>? DerivePlanes(TriangleMesh3D prism)
    {
        var centroid = Vector3.Zero;
        foreach (var p in prism.Points)
            centroid += p.Vector3;
        centroid /= prism.Points.Count;

        var planes = new List<Plane>();
        foreach (var face in prism.FaceIndices)
        {
            var a = prism.Points[face.A].Vector3;
            var b = prism.Points[face.B].Vector3;
            var c = prism.Points[face.C].Vector3;
            var n = Vector3.Cross(b - a, c - a);
            if (n.LengthSquared() < 1e-16f)
                continue;
            n = n.Normalize;
            if (Vector3.Dot(centroid - a, n) > 0f) // orient outward
                n = -n;

            var merged = false;
            foreach (var pl in planes)
            {
                if (Vector3.Dot(pl.Normal, n) > 0.999f &&
                    MathF.Abs(Vector3.Dot(a - pl.Point, pl.Normal)) < 1e-4f)
                {
                    merged = true;
                    break;
                }
            }
            if (!merged)
                planes.Add(new Plane(a, n));
        }

        // Convexity guard: every vertex must lie inside (or on) all face planes.
        foreach (var p in prism.Points)
        {
            var v = p.Vector3;
            foreach (var pl in planes)
            {
                if (Vector3.Dot(v - pl.Point, pl.Normal) > 1e-3f)
                    return null;
            }
        }
        return planes;
    }

    static (List<Vector3> Inside, List<Vector3> Outside) SplitPolygon(List<Vector3> poly, Plane pl, float eps)
    {
        var inside = new List<Vector3>();
        var outside = new List<Vector3>();
        var n = poly.Count;
        for (var i = 0; i < n; i++)
        {
            var cur = poly[i];
            var nxt = poly[(i + 1) % n];
            var dc = Vector3.Dot(cur - pl.Point, pl.Normal);
            var dn = Vector3.Dot(nxt - pl.Point, pl.Normal);
            if (dc <= eps)
                inside.Add(cur);
            else
                outside.Add(cur);
            if ((dc > eps) != (dn > eps))
            {
                var t = dc / (dc - dn);
                var ip = cur + (nxt - cur) * t;
                inside.Add(ip);
                outside.Add(ip);
            }
        }
        return (inside, outside);
    }

    static bool TriangleOverlapsBox(Vector3 a, Vector3 b, Vector3 c, Vector3 min, Vector3 max)
    {
        var tmin = Min(Min(a, b), c);
        var tmax = Max(Max(a, b), c);
        return tmin.X <= max.X && tmax.X >= min.X &&
               tmin.Y <= max.Y && tmax.Y >= min.Y &&
               tmin.Z <= max.Z && tmax.Z >= min.Z;
    }

    static Vector3 Min(Vector3 a, Vector3 b) => new(MathF.Min(a.X, b.X), MathF.Min(a.Y, b.Y), MathF.Min(a.Z, b.Z));
    static Vector3 Max(Vector3 a, Vector3 b) => new(MathF.Max(a.X, b.X), MathF.Max(a.Y, b.Y), MathF.Max(a.Z, b.Z));
}
