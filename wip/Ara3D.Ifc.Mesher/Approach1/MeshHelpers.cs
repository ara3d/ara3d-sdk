using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.IfcTypes;
using Ara3D.IO.StepParser;

namespace Ara3D.Ifc.Mesher.Approach1;

public static class MeshHelpers
{
    public static IfcEntity ResolveRequired(MeshingContext ctx, IfcEntity entity, IfcAttribute attribute)
    {
        var id = ReadOptionalId(entity, attribute);
        if (id is null)
            throw new InvalidOperationException($"{entity.GetEntityName()} #{entity.Id} missing {attribute.Name}");
        return ctx.GetEntity(id.Value);
    }

    public static IfcEntity? ResolveOptional(MeshingContext ctx, IfcEntity entity, IfcAttribute attribute)
    {
        var id = ReadOptionalId(entity, attribute);
        return id is null ? null : ctx.GetEntityOrDefault(id.Value);
    }

    public static int? ReadOptionalId(IfcEntity entity, IfcAttribute attribute)
    {
        var token = entity.GetValue(attribute.Index);
        return token.IsId ? token.AsId() : null;
    }

    public static double ReadNumber(IfcEntity entity, IfcAttribute attribute)
        => ReadNumberByIndex(entity, attribute.Index);

    public static double ReadNumberByIndex(IfcEntity entity, int index)
    {
        var token = entity.GetValue(index);
        return token.IsNumber ? token.AsNumber() : 0.0;
    }

    public static int? ReadOptionalIdByIndex(IfcEntity entity, int index)
    {
        var token = entity.GetValue(index);
        return token.IsId ? token.AsId() : null;
    }

    public static bool ReadOptionalBool(IfcEntity entity, IfcAttribute attribute, bool defaultValue = true)
    {
        var token = entity.GetValue(attribute.Index);
        if (token.IsUnassignedOrRedeclared)
            return defaultValue;
        var text = token.IsString ? token.AsString() : token.ToString();
        if (text.Contains(".F.", StringComparison.Ordinal))
            return false;
        if (text.Contains(".T.", StringComparison.Ordinal))
            return true;
        return defaultValue;
    }

    public static IReadOnlyList<int> ReadIds(IfcEntity entity, IfcAttribute attribute)
        => ReadIdsByIndex(entity, attribute.Index);

    public static IReadOnlyList<int> ReadIdsByIndex(IfcEntity entity, int index)
    {
        var token = entity.GetValue(index);
        if (!token.IsList)
            return [];
        return token.AsList(entity.Document).Where(t => t.IsId).Select(t => t.AsId()).ToList();
    }

    public static IReadOnlyList<double> ReadNumbers(IfcEntity entity, IfcAttribute attribute)
    {
        var token = entity.GetValue(attribute.Index);
        if (!token.IsList)
            return [];
        return token.AsList(entity.Document).Where(t => t.IsNumber).Select(t => t.AsNumber()).ToList();
    }

    public static TriangleMesh3D Merge(IReadOnlyList<TriangleMesh3D> meshes)
    {
        if (meshes.Count == 0)
            return new TriangleMesh3D([], []);
        if (meshes.Count == 1)
            return meshes[0];

        var points = new List<Point3D>();
        var faces = new List<Integer3>();
        foreach (var mesh in meshes)
        {
            var offset = points.Count;
            points.AddRange(mesh.Points);
            faces.AddRange(mesh.FaceIndices.Select(f => new Integer3(f.A + offset, f.B + offset, f.C + offset)));
        }
        return new TriangleMesh3D(points, faces);
    }

    public static TriangleMesh3D Transform(TriangleMesh3D mesh, Matrix4x4 matrix)
    {
        var points = mesh.Points.Select(p =>
        {
            var v = p.Vector3.Transform(matrix);
            return new Point3D(v.X, v.Y, v.Z);
        }).ToList();
        // A mirror (negative-determinant operator) reverses handedness; flip triangle winding so
        // world-space normals stay outward.
        var faces = LinearDeterminant(matrix) < 0
            ? mesh.FaceIndices.Select(f => new Integer3(f.A, f.C, f.B)).ToList()
            : mesh.FaceIndices;
        return new TriangleMesh3D(points, faces);
    }

    /// <summary>Determinant of the upper-left 3×3 (rotation/scale/mirror) block of an affine transform.</summary>
    public static float LinearDeterminant(Matrix4x4 m)
        => m.M11 * (m.M22 * m.M33 - m.M23 * m.M32)
         - m.M12 * (m.M21 * m.M33 - m.M23 * m.M31)
         + m.M13 * (m.M21 * m.M32 - m.M22 * m.M31);

    public static Bounds3D GetBounds(TriangleMesh3D mesh)
        => mesh.Points.Count == 0 ? Bounds3D.Empty : mesh.Points.Bounds();

    public static double SignedVolume(TriangleMesh3D mesh)
    {
        double volume = 0;
        foreach (var face in mesh.FaceIndices)
        {
            var a = mesh.Points[face.A].Vector3;
            var b = mesh.Points[face.B].Vector3;
            var c = mesh.Points[face.C].Vector3;
            volume += Vector3.Dot(a, Vector3.Cross(b, c)) / 6.0;
        }
        return volume;
    }

    public static TriangleMesh3D BuildExtrusionWithHoles(
        PolygonWithHoles profile,
        Frame3D frame,
        Vector3 extrusion)
    {
        // Offset-ring triangulation resamples a single hole to match the outer vertex count.
        // Wall loops must use that same resampled ring or hole seams stay open.
        profile = AlignHoleRingsForExtrusion(profile);

        var bottomByKey = new Dictionary<(int X, int Y), int>();
        var points = new List<Point3D>();

        int AddBottom(Vector2 p2)
        {
            var key = Quantize(p2);
            if (bottomByKey.TryGetValue(key, out var idx))
                return idx;
            idx = points.Count;
            points.Add(frame.ToWorld(new Vector3(p2.X, p2.Y, 0)));
            bottomByKey[key] = idx;
            return idx;
        }

        foreach (var p in profile.Outer)
            AddBottom(p);
        foreach (var hole in profile.Holes)
            foreach (var p in hole)
                AddBottom(p);

        // Cap triangulation can introduce vertices not on the profile rings (ear-clip Steiner
        // points, resampled inner rings). Register them before duplicating the top ring so
        // TopIndex(bottom) always has a matching top vertex.
        var capTris = profile.Triangulate();
        foreach (var tri in capTris)
        {
            AddBottom(tri.A.Vector2);
            AddBottom(tri.B.Vector2);
            AddBottom(tri.C.Vector2);
        }

        var bottomCount = points.Count;
        var topOffset = bottomCount;
        for (var i = 0; i < bottomCount; i++)
            points.Add(points[i] + extrusion);

        int TopIndex(int bottomIdx) => bottomIdx + topOffset;

        var faces = new List<Integer3>();
        foreach (var tri in capTris)
        {
            var a = AddBottom(tri.A.Vector2);
            var b = AddBottom(tri.B.Vector2);
            var c = AddBottom(tri.C.Vector2);
            faces.Add(new Integer3(c, b, a));
            faces.Add(new Integer3(TopIndex(a), TopIndex(b), TopIndex(c)));
        }

        AddWallLoop(faces, profile.Outer, AddBottom, TopIndex);
        foreach (var hole in profile.Holes)
            AddWallLoop(faces, hole, AddBottom, TopIndex, reverse: true);

        return new TriangleMesh3D(points, faces);
    }

    /// <summary>
    /// Mirrors <see cref="PolygonWithHoles"/> offset-ring resampling so extrusion walls share
    /// the same hole vertices as the annular cap triangulation.
    /// </summary>
    static PolygonWithHoles AlignHoleRingsForExtrusion(PolygonWithHoles profile)
    {
        if (profile.Holes.Count != 1)
            return profile;
        var hole = profile.Holes[0];
        if (hole.Count == profile.Outer.Count || hole.Count < 3 || profile.Outer.Count < 3)
            return profile;
        if (PolygonTriangulator.HasSelfIntersection(profile.Outer) ||
            PolygonTriangulator.HasSelfIntersection(hole))
            return profile;

        var resampled = PolygonWithHoles.ResampleClosedRing(hole, profile.Outer.Count);
        return new PolygonWithHoles(profile.Outer, [resampled]);
    }

    static (int X, int Y) Quantize(Vector2 p)
        => ((int)MathF.Round(p.X * 1e6f), (int)MathF.Round(p.Y * 1e6f));

    static void AddWallLoop(
        List<Integer3> faces,
        IReadOnlyList<Vector2> loop,
        Func<Vector2, int> bottomIndex,
        Func<int, int> topIndex,
        bool reverse = false)
    {
        var n = loop.Count;
        for (var i = 0; i < n; i++)
        {
            var j = (i + 1) % n;
            var bi = bottomIndex(loop[i]);
            var bj = bottomIndex(loop[j]);
            var ti = topIndex(bi);
            var tj = topIndex(bj);
            if (!reverse)
            {
                faces.Add(new Integer3(bi, bj, tj));
                faces.Add(new Integer3(bi, tj, ti));
            }
            else
            {
                faces.Add(new Integer3(bj, bi, ti));
                faces.Add(new Integer3(bj, ti, tj));
            }
        }
    }
}
