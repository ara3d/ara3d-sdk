using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.IfcTypes;

namespace Ara3D.Ifc.Mesher.Approach1;

/// <summary>
/// Boolean ops: DIFFERENCE via half-space plane clipping (dominant clipping case).
/// General CSG union/intersection is unsupported and recorded as diagnostics.
/// </summary>
public static class Booleans
{
    public static TriangleMesh3D? BuildBooleanResult(MeshingContext ctx, IfcEntity booleanResult)
    {
        var op = booleanResult.GetString(IfcBooleanResult.Instance.Operator.Index);
        var first = MeshHelpers.ResolveRequired(ctx, booleanResult, IfcBooleanResult.Instance.FirstOperand);
        var second = MeshHelpers.ResolveRequired(ctx, booleanResult, IfcBooleanResult.Instance.SecondOperand);

        if (op.Contains(".DIFFERENCE."))
            return BuildDifference(ctx, first, second);

        ctx.Diagnostics.RecordUnsupported(booleanResult.GetEntityName(), $"Boolean operator {op} not implemented");
        return GeometryDispatcher.TryBuild(ctx, first);
    }

    public static TriangleMesh3D? BuildBooleanClippingResult(MeshingContext ctx, IfcEntity clip)
    {
        ctx.Diagnostics.RecordSupported("IFCBOOLEANCLIPPINGRESULT");
        return BuildBooleanResult(ctx, clip);
    }

    static TriangleMesh3D? BuildDifference(MeshingContext ctx, IfcEntity first, IfcEntity second)
    {
        var mesh = GeometryDispatcher.TryBuild(ctx, first);
        if (mesh is null)
            return null;

        if (second.GetEntityName() == "IFCHALFSPACESOLID")
        {
            ctx.Diagnostics.RecordApproximate("IFCBOOLEANRESULT", "Planar half-space difference clipping");
            return ClipByHalfSpace(ctx, mesh.Value, second, first);
        }

        ctx.Diagnostics.RecordUnsupported("IFCBOOLEANRESULT", $"Difference with {second.GetEntityName()} not supported");
        return mesh;
    }

    public static TriangleMesh3D ClipByHalfSpace(MeshingContext ctx, TriangleMesh3D mesh, IfcEntity halfSpace)
        => ClipByHalfSpace(ctx, mesh, halfSpace, firstOperand: null);

    public static TriangleMesh3D ClipByHalfSpace(
        MeshingContext ctx,
        TriangleMesh3D mesh,
        IfcEntity halfSpace,
        IfcEntity? firstOperand)
    {
        ctx.Diagnostics.RecordSupported("IFCHALFSPACESOLID");
        var agreement = halfSpace.GetString(IfcHalfSpaceSolid.Instance.AgreementFlag.Index).Contains(".T.");
        var (planePoint, planeNormal) = ResolveClipPlane(ctx, halfSpace, firstOperand, agreement);

        // Planar clips use !agreement; oblique gable-style planes (strong X component) use agreement directly.
        var keepPositiveSide = MathF.Abs(planeNormal.X) >= MathF.Abs(planeNormal.Z)
            ? agreement
            : !agreement;

        var points = new List<Point3D>();
        var faces = new List<Integer3>();
        foreach (var face in mesh.FaceIndices)
        {
            var clipped = ClipTriangle(
                mesh.Points[face.A].Vector3,
                mesh.Points[face.B].Vector3,
                mesh.Points[face.C].Vector3,
                planePoint, planeNormal, keepPositiveSide);
            if (clipped.Count == 3)
            {
                var i0 = AddPoint(points, clipped[0]);
                var i1 = AddPoint(points, clipped[1]);
                var i2 = AddPoint(points, clipped[2]);
                faces.Add(new Integer3(i0, i1, i2));
            }
            else if (clipped.Count == 4)
            {
                var i0 = AddPoint(points, clipped[0]);
                var i1 = AddPoint(points, clipped[1]);
                var i2 = AddPoint(points, clipped[2]);
                var i3 = AddPoint(points, clipped[3]);
                faces.Add(new Integer3(i0, i1, i2));
                faces.Add(new Integer3(i0, i2, i3));
            }
        }
        return new TriangleMesh3D(points, faces);
    }

    static (Vector3 point, Vector3 normal) ResolveClipPlane(
        MeshingContext ctx,
        IfcEntity halfSpace,
        IfcEntity? firstOperand,
        bool agreement)
    {
        var (planePoint, planeNormal) = ReadHalfSpacePlane(ctx, halfSpace);
        if (firstOperand is null || !TryGetExtrudedSolid(ctx, firstOperand, out var extruded))
            return (planePoint, planeNormal);

        var frame = Placements.ReadOptionalAxis2Placement3D(ctx, extruded, IfcSweptAreaSolid.Instance.Position);
        var direction = Placements.ReadDirection3D(ctx,
            MeshHelpers.ResolveRequired(ctx, extruded, IfcExtrudedAreaSolid.Instance.ExtrudedDirection),
            Vector3.UnitZ).Normalize;
        var depth = ctx.ScaleLength(MeshHelpers.ReadNumber(extruded, IfcExtrudedAreaSolid.Instance.Depth));
        var extrusion = frame.ToWorldDirection(direction * depth);
        if (extrusion.LengthSquared() < 1e-12f || depth <= 1e-6f)
            return (planePoint, planeNormal);

        var extAxis = extrusion.Normalize;
        if (MathF.Abs(Vector3.Dot(planeNormal.Normalize, extAxis)) > 0.25f)
            return (planePoint, planeNormal);

        var clipDistance = InferExtrusionClipDistance(planePoint, frame.Origin.Vector3, depth);
        if (clipDistance <= 1e-6f || clipDistance >= depth - 1e-6f)
            return (planePoint, planeNormal);

        ctx.Diagnostics.RecordApproximate("IFCHALFSPACESOLID", "Extrusion-end half-space clip");
        var clipPoint = frame.Origin.Vector3 + extAxis * clipDistance;
        var clipNormal = agreement ? extAxis : -extAxis;
        return (clipPoint, clipNormal);
    }

    static float InferExtrusionClipDistance(Vector3 planePoint, Vector3 solidOrigin, float depth)
    {
        var offset = planePoint - solidOrigin;
        var abs = new Vector3(MathF.Abs(offset.X), MathF.Abs(offset.Y), MathF.Abs(offset.Z));
        if (abs.X >= abs.Y && abs.X >= abs.Z)
            return abs.X;
        if (abs.Y >= abs.X && abs.Y >= abs.Z)
            return abs.Y;
        return abs.Z;
    }

    static bool TryGetExtrudedSolid(MeshingContext ctx, IfcEntity operand, out IfcEntity extruded)
    {
        extruded = operand;
        while (true)
        {
            switch (extruded.GetEntityName())
            {
                case "IFCEXTRUDEDAREASOLID":
                    return true;
                case "IFCBOOLEANCLIPPINGRESULT" or "IFCBOOLEANRESULT":
                    extruded = MeshHelpers.ResolveRequired(ctx, extruded, IfcBooleanResult.Instance.FirstOperand);
                    break;
                default:
                    return false;
            }
        }
    }

    static (Vector3 point, Vector3 normal) ReadHalfSpacePlane(MeshingContext ctx, IfcEntity halfSpace)
    {
        var surface = MeshHelpers.ResolveRequired(ctx, halfSpace, IfcHalfSpaceSolid.Instance.BaseSurface);
        if (surface.GetEntityName() != "IFCPLANE")
            throw new NotSupportedException($"Half space base {surface.GetEntityName()} not supported");

        var placement = Placements.ReadAxis2Placement3D(ctx,
            MeshHelpers.ResolveRequired(ctx, surface, IfcElementarySurface.Instance.Position));
        return (placement.Origin.Vector3, placement.Z);
    }

    static List<Vector3> ClipTriangle(
        Vector3 a, Vector3 b, Vector3 c,
        Vector3 planePoint, Vector3 planeNormal,
        bool keepPositiveSide)
    {
        var verts = new[] { a, b, c };
        var inside = verts.Select(v =>
        {
            var d = SignedDistance(v, planePoint, planeNormal);
            return keepPositiveSide ? d >= -1e-6f : d <= 1e-6f;
        }).ToArray();
        var insideCount = inside.Count(x => x);
        if (insideCount == 0)
            return [];
        if (insideCount == 3)
            return [a, b, c];

        var result = new List<Vector3>();
        for (var i = 0; i < 3; i++)
        {
            var j = (i + 1) % 3;
            var vi = verts[i];
            var vj = verts[j];
            if (inside[i])
                result.Add(vi);
            if (inside[i] != inside[j])
                result.Add(LerpPlane(vi, vj, planePoint, planeNormal));
        }
        return result;
    }

    static float SignedDistance(Vector3 p, Vector3 planePoint, Vector3 planeNormal)
        => Vector3.Dot(p - planePoint, planeNormal);

    static Vector3 LerpPlane(Vector3 a, Vector3 b, Vector3 planePoint, Vector3 planeNormal)
    {
        var da = SignedDistance(a, planePoint, planeNormal);
        var db = SignedDistance(b, planePoint, planeNormal);
        var t = da / (da - db);
        return a + (b - a) * t;
    }

    static int AddPoint(List<Point3D> points, Vector3 p)
    {
        const float eps = 1e-5f;
        for (var i = 0; i < points.Count; i++)
        {
            if (points[i].Vector3.DistanceSquared(p) < eps * eps)
                return i;
        }
        var idx = points.Count;
        points.Add(p);
        return idx;
    }

    public static void RecordOpeningDiagnostic(MeshingContext ctx)
        => ctx.Diagnostics.RecordUnsupported("IFCRELVOIDSELEMENT", "Openings not subtracted in first pass");
}
