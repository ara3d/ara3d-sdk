using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.IfcTypes;

namespace Ara3D.Ifc.Mesher.Approach1;

/// <summary>IFC placements and transforms to <see cref="Frame3D"/> / <see cref="Matrix4x4"/>.</summary>
public static class Placements
{
    public static Vector3 ReadPoint3D(MeshingContext ctx, IfcEntity point)
    {
        if (ctx.PointCache.TryGetValue(point.Id, out var cached))
            return cached;

        if (point.GetEntityName() != "IFCCARTESIANPOINT")
            throw new NotSupportedException($"Expected IFCCARTESIANPOINT, got {point.GetEntityName()} #{point.Id}");

        var values = MeshHelpers.ReadNumbers(point, IfcCartesianPoint.Instance.Coordinates);
        var p = new Vector3(
            ctx.ScaleLength(values.Count > 0 ? values[0] : 0),
            ctx.ScaleLength(values.Count > 1 ? values[1] : 0),
            ctx.ScaleLength(values.Count > 2 ? values[2] : 0));
        ctx.PointCache[point.Id] = p;
        ctx.Diagnostics.RecordSupported("IFCCARTESIANPOINT");
        return p;
    }

    public static Vector2 ReadPoint2D(MeshingContext ctx, IfcEntity point)
    {
        var p = ReadPoint3D(ctx, point);
        return new Vector2(p.X, p.Y);
    }

    public static Vector3 ReadDirection3D(MeshingContext ctx, IfcEntity direction, Vector3 fallback)
    {
        if (ctx.DirectionCache.TryGetValue(direction.Id, out var cached))
            return cached;

        if (direction.GetEntityName() != "IFCDIRECTION")
            throw new NotSupportedException($"Expected IFCDIRECTION, got {direction.GetEntityName()} #{direction.Id}");

        var values = MeshHelpers.ReadNumbers(direction, IfcDirection.Instance.DirectionRatios);
        if (values.Count == 0)
            return fallback;

        var d = new Vector3(
            values.Count > 0 ? (float)values[0] : 0f,
            values.Count > 1 ? (float)values[1] : 0f,
            values.Count > 2 ? (float)values[2] : 0f);
        if (d.LengthSquared() < 1e-12f)
            return fallback;

        d = d.Normalize;
        ctx.DirectionCache[direction.Id] = d;
        ctx.Diagnostics.RecordSupported("IFCDIRECTION");
        return d;
    }

    public static Vector2 ReadDirection2D(MeshingContext ctx, IfcEntity direction, Vector2 fallback)
    {
        var values = MeshHelpers.ReadNumbers(direction, IfcDirection.Instance.DirectionRatios);
        if (values.Count == 0)
            return fallback;
        var d = new Vector2(
            values.Count > 0 ? (float)values[0] : 0f,
            values.Count > 1 ? (float)values[1] : 0f);
        return d.LengthSquared() < 1e-12f ? fallback : d.Normalize;
    }

    public static Frame3D ReadAxis2Placement3D(MeshingContext ctx, IfcEntity placement)
    {
        if (ctx._axis2Placements.TryGetValue(placement.Id, out var cached))
            return cached;

        var name = placement.GetEntityName();
        Frame3D frame = name switch
        {
            "IFCAXIS2PLACEMENT3D" => BuildAxis2Placement3D(ctx, placement),
            "IFCAXIS2PLACEMENT2D" => BuildAxis2Placement2DAs3D(ctx, placement),
            _ => throw new NotSupportedException($"Unsupported placement {name} #{placement.Id}"),
        };
        ctx._axis2Placements[placement.Id] = frame;
        ctx.Diagnostics.RecordSupported(name);
        return frame;
    }

    public static Frame3D ReadAxis2Placement2D(MeshingContext ctx, IfcEntity placement)
        => ReadAxis2Placement3D(ctx, placement);

    static Frame3D BuildAxis2Placement3D(MeshingContext ctx, IfcEntity placement)
    {
        var origin = ReadPoint3D(ctx, MeshHelpers.ResolveRequired(ctx, placement, IfcPlacement.Instance.Location));
        var z = MeshHelpers.ResolveOptional(ctx, placement, IfcAxis2Placement3D.Instance.Axis) is { } axis
            ? ReadDirection3D(ctx, axis, Vector3.UnitZ)
            : Vector3.UnitZ;
        var x = MeshHelpers.ResolveOptional(ctx, placement, IfcAxis2Placement3D.Instance.RefDirection) is { } refDir
            ? ReadDirection3D(ctx, refDir, Vector3.UnitX)
            : Vector3.UnitX;
        return FrameFromOriginXZ(origin, x, z);
    }

    static Frame3D BuildAxis2Placement2DAs3D(MeshingContext ctx, IfcEntity placement)
    {
        var origin2 = ReadPoint2D(ctx, MeshHelpers.ResolveRequired(ctx, placement, IfcPlacement.Instance.Location));
        var x2 = MeshHelpers.ResolveOptional(ctx, placement, IfcAxis2Placement2D.Instance.RefDirection) is { } refDir
            ? ReadDirection2D(ctx, refDir, Vector2.UnitX)
            : Vector2.UnitX;
        var origin = new Vector3(origin2.X, origin2.Y, 0);
        var x = new Vector3(x2.X, x2.Y, 0);
        return FrameFromOriginXZ(origin, x, Vector3.UnitZ);
    }

    public static Frame3D ReadOptionalAxis2Placement3D(MeshingContext ctx, IfcEntity entity, IfcAttribute attribute)
    {
        var placement = MeshHelpers.ResolveOptional(ctx, entity, attribute);
        return placement is null ? IdentityFrame : ReadAxis2Placement3D(ctx, placement);
    }

    public static Frame3D ReadLocalPlacement(MeshingContext ctx, IfcEntity placement)
    {
        if (ctx._localPlacements.TryGetValue(placement.Id, out var cached))
            return cached;

        if (placement.GetEntityName() != "IFCLOCALPLACEMENT")
            throw new NotSupportedException($"Expected IFCLOCALPLACEMENT, got {placement.GetEntityName()}");

        var parent = MeshHelpers.ResolveOptional(ctx, placement, IfcLocalPlacement.Instance.PlacementRelTo);
        var parentFrame = parent is null ? IdentityFrame : ReadLocalPlacement(ctx, parent);
        var relative = MeshHelpers.ResolveRequired(ctx, placement, IfcLocalPlacement.Instance.RelativePlacement);
        var frame = parentFrame.Compose(ReadAxis2Placement3D(ctx, relative));
        ctx._localPlacements[placement.Id] = frame;
        ctx.Diagnostics.RecordSupported("IFCLOCALPLACEMENT");
        return frame;
    }

    public static Matrix4x4 ReadCartesianTransformationOperator2D(MeshingContext ctx, IfcEntity op)
    {
        if (ctx._mappedTransforms.TryGetValue(op.Id, out var cached))
            return cached;

        var name = op.GetEntityName();
        if (name is not ("IFCCARTESIANTRANSFORMATIONOPERATOR2D" or "IFCCARTESIANTRANSFORMATIONOPERATOR2DNONUNIFORM"))
            throw new NotSupportedException($"Unsupported 2D mapping operator {name}");

        var origin = MeshHelpers.ResolveOptional(ctx, op, IfcCartesianTransformationOperator.Instance.LocalOrigin) is { } loc
            ? ReadPoint2D(ctx, loc)
            : Vector2.Zero;
        // Axis1 (X) / Axis2 (Y): honor verbatim so a mirrored profile operator (negative determinant)
        // survives instead of collapsing to axis-aligned scale. Absent → identity axes.
        var x2 = MeshHelpers.ResolveOptional(ctx, op, IfcCartesianTransformationOperator.Instance.Axis1) is { } a1
            ? ReadDirection2D(ctx, a1, Vector2.UnitX)
            : Vector2.UnitX;
        var y2 = MeshHelpers.ResolveOptional(ctx, op, IfcCartesianTransformationOperator.Instance.Axis2) is { } a2
            ? ReadDirection2D(ctx, a2, new Vector2(-x2.Y, x2.X))
            : new Vector2(-x2.Y, x2.X);
        var scale = MeshHelpers.ReadNumber(op, IfcCartesianTransformationOperator.Instance.Scale);
        if (scale <= 0)
            scale = 1.0;

        var sx = (float)scale;
        var sy = (float)scale;
        if (name == "IFCCARTESIANTRANSFORMATIONOPERATOR2DNONUNIFORM")
        {
            sy = (float)MeshHelpers.ReadNumber(op, IfcCartesianTransformationOperator2DnonUniform.Instance.Scale2);
            if (sy <= 0)
                sy = sx;
        }

        var matrix = BasisMatrix(
            new Vector3(x2.X, x2.Y, 0f) * sx,
            new Vector3(y2.X, y2.Y, 0f) * sy,
            Vector3.UnitZ,
            new Vector3(origin.X, origin.Y, 0f));
        ctx._mappedTransforms[op.Id] = matrix;
        ctx.Diagnostics.RecordSupported(name);
        return matrix;
    }

    public static Matrix4x4 ReadProfileTransformationOperator(MeshingContext ctx, IfcEntity op)
    {
        return op.GetEntityName() switch
        {
            "IFCCARTESIANTRANSFORMATIONOPERATOR2D" or "IFCCARTESIANTRANSFORMATIONOPERATOR2DNONUNIFORM"
                => ReadCartesianTransformationOperator2D(ctx, op),
            "IFCCARTESIANTRANSFORMATIONOPERATOR3D" or "IFCCARTESIANTRANSFORMATIONOPERATOR3DNONUNIFORM"
                => ReadCartesianTransformationOperator3D(ctx, op),
            _ => throw new NotSupportedException($"Unsupported profile operator {op.GetEntityName()}"),
        };
    }

    public static Matrix4x4 ReadCartesianTransformationOperator3D(MeshingContext ctx, IfcEntity op)
    {
        if (ctx._mappedTransforms.TryGetValue(op.Id, out var cached))
            return cached;

        var name = op.GetEntityName();
        if (name is not ("IFCCARTESIANTRANSFORMATIONOPERATOR3D" or "IFCCARTESIANTRANSFORMATIONOPERATOR3DNONUNIFORM"))
            throw new NotSupportedException($"Unsupported mapping operator {name}");

        var origin = MeshHelpers.ResolveOptional(ctx, op, IfcCartesianTransformationOperator.Instance.LocalOrigin) is { } loc
            ? ReadPoint3D(ctx, loc)
            : Vector3.Zero;
        var axis1 = MeshHelpers.ResolveOptional(ctx, op, IfcCartesianTransformationOperator3D.Instance.Axis1) is { } a1
            ? ReadDirection3D(ctx, a1, Vector3.UnitX)
            : Vector3.UnitX;
        var axis3 = MeshHelpers.ResolveOptional(ctx, op, IfcCartesianTransformationOperator3D.Instance.Axis3) is { } a3
            ? ReadDirection3D(ctx, a3, Vector3.UnitZ)
            : Vector3.UnitZ;
        // Axis2 (Y): honor it verbatim when present so a mirror / negative-determinant operator is
        // preserved (web-ifc uses the axes as given). Absent → right-handed default from Z × X.
        var axis2 = MeshHelpers.ResolveOptional(ctx, op, IfcCartesianTransformationOperator.Instance.Axis2) is { } a2
            ? ReadDirection3D(ctx, a2, Vector3.Cross(axis3, axis1))
            : Vector3.Cross(axis3, axis1);
        var scale = MeshHelpers.ReadNumber(op, IfcCartesianTransformationOperator.Instance.Scale);
        if (scale <= 0)
            scale = 1.0;

        var sx = scale;
        var sy = scale;
        var sz = scale;
        if (name == "IFCCARTESIANTRANSFORMATIONOPERATOR3DNONUNIFORM")
        {
            sy = MeshHelpers.ReadNumber(op, IfcCartesianTransformationOperator3DnonUniform.Instance.Scale2);
            sz = MeshHelpers.ReadNumber(op, IfcCartesianTransformationOperator3DnonUniform.Instance.Scale3);
            if (sy <= 0) sy = scale;
            if (sz <= 0) sz = scale;
        }

        var matrix = BasisMatrix(axis1 * (float)sx, axis2 * (float)sy, axis3 * (float)sz, origin);
        ctx._mappedTransforms[op.Id] = matrix;
        ctx.Diagnostics.RecordSupported(name);
        return matrix;
    }

    /// <summary>Row-vector transform from explicit basis rows (X/Y/Z) and origin, preserving handedness.</summary>
    static Matrix4x4 BasisMatrix(Vector3 x, Vector3 y, Vector3 z, Vector3 origin)
        => new(
            x.X, x.Y, x.Z, 0f,
            y.X, y.Y, y.Z, 0f,
            z.X, z.Y, z.Z, 0f,
            origin.X, origin.Y, origin.Z, 1f);

    public static Frame3D FrameFromOriginXZ(Vector3 origin, Vector3 xAxis, Vector3 zAxis)
    {
        var z = zAxis.Normalize;
        var x = xAxis.Normalize;
        if (MathF.Abs(Vector3.Dot(x, z)) > 0.999f)
            x = MathF.Abs(Vector3.Dot(Vector3.UnitX, z)) < 0.999f ? Vector3.UnitX : Vector3.UnitY;
        var y = Vector3.Cross(z, x).Normalize;
        x = Vector3.Cross(y, z).Normalize;
        return new Frame3D(origin, new OrthonormalBasis3D(new Axes3D(x, y, z), true));
    }

    public static Matrix4x4 ToMatrix(Frame3D frame) => frame.Matrix;

    public static Frame3D IdentityFrame
        => new(Point3D.Zero, OrthonormalBasis3D.Identity);
}
