using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.IfcTypes;

namespace Ara3D.Ifc.Mesher.Approach1;

/// <summary>
/// Builds 2D profiles as <see cref="PolygonWithHoles"/>.
/// Parameterized profiles use arc sampling for fillets (approximate).
/// </summary>
public static class ProfileBuilder
{
    public static PolygonWithHoles Build(MeshingContext ctx, IfcEntity profile)
    {
        if (ctx.ProfileCache.TryGetValue(profile.Id, out var cached))
            return cached;

        var result = profile.GetEntityName() switch
        {
            "IFCRECTANGLEPROFILEDEF" => BuildRectangle(ctx, profile),
            "IFCRECTANGLEHOLLOWPROFILEDEF" => BuildRectangleHollow(ctx, profile),
            "IFCROUNDEDRECTANGLEPROFILEDEF" => BuildRoundedRectangle(ctx, profile),
            "IFCCIRCLEPROFILEDEF" => BuildCircle(ctx, profile),
            "IFCCIRCLEHOLLOWPROFILEDEF" => BuildCircleHollow(ctx, profile),
            "IFCELLIPSEPROFILEDEF" => BuildEllipse(ctx, profile),
            "IFCISHAPEPROFILEDEF" => BuildIShape(ctx, profile),
            "IFCLSHAPEPROFILEDEF" => BuildLShape(ctx, profile),
            "IFCUSHAPEPROFILEDEF" => BuildUShape(ctx, profile),
            "IFCTSHAPEPROFILEDEF" => BuildTShape(ctx, profile),
            "IFCZSHAPEPROFILEDEF" => BuildZShape(ctx, profile),
            "IFCCSHAPEPROFILEDEF" => BuildCShape(ctx, profile),
            "IFCTRAPEZIUMPROFILEDEF" => BuildTrapezium(ctx, profile),
            "IFCARBITRARYCLOSEDPROFILEDEF" => BuildArbitraryClosed(ctx, profile),
            "IFCARBITRARYOPENPROFILEDEF" => BuildArbitraryOpenAsClosed(ctx, profile),
            "IFCARBITRARYPROFILEDEF" => BuildArbitrary(ctx, profile),
            "IFCARBITRARYPROFILEDEFWITHVOIDS" => BuildArbitraryWithVoids(ctx, profile),
            "IFCCOMPOSITEPROFILEDEF" => BuildComposite(ctx, profile),
            "IFCDERIVEDPROFILEDEF" => BuildDerived(ctx, profile),
            _ => throw new NotSupportedException($"Unsupported profile {profile.GetEntityName()} #{profile.Id}"),
        };

        ctx.ProfileCache[profile.Id] = result;
        ctx.Diagnostics.RecordSupported(profile.GetEntityName());
        return result;
    }

    static PolygonWithHoles ApplyPosition(MeshingContext ctx, IfcEntity profile, IReadOnlyList<Vector2> points)
    {
        var placement = MeshHelpers.ResolveOptional(ctx, profile, IfcParameterizedProfileDef.Instance.Position);
        if (placement is null)
            return new PolygonWithHoles(points);
        var frame = Placements.ReadAxis2Placement2D(ctx, placement);
        return new PolygonWithHoles(points.Select(p =>
        {
            var w = frame.ToWorld(new Vector3(p.X, p.Y, 0));
            return new Vector2(w.X.Value, w.Y.Value);
        }).ToList());
    }

    public static bool IsOpenProfileType(string entityName)
        => entityName == "IFCARBITRARYOPENPROFILEDEF";

    /// <summary>Evaluates an open profile curve without forcing closure.</summary>
    public static IReadOnlyList<Vector2> BuildOpen(MeshingContext ctx, IfcEntity profile)
    {
        var points = profile.GetEntityName() switch
        {
            "IFCARBITRARYOPENPROFILEDEF" => BuildArbitraryOpen(ctx, profile),
            _ => throw new NotSupportedException($"Unsupported open profile {profile.GetEntityName()} #{profile.Id}"),
        };
        ctx.Diagnostics.RecordSupported(profile.GetEntityName());
        return points;
    }

    public static bool IsCurveClosed(MeshingContext ctx, IfcEntity curve)
    {
        var points = CurveEvaluator.Evaluate2D(ctx, curve);
        return points.Count >= 3 && points[0].DistanceSquared(points[^1]) < 1e-8f;
    }

    static PolygonWithHoles BuildRectangle(MeshingContext ctx, IfcEntity profile)
    {
        if (TryReadLegacyRectangleDims(ctx, profile, out var lx, out var ly))
        {
            var legacyPoints = new List<Vector2>
            {
                new(-lx / 2f, -ly / 2f), new(lx / 2f, -ly / 2f),
                new(lx / 2f, ly / 2f), new(-lx / 2f, ly / 2f),
            };
            var placementId = MeshHelpers.ReadOptionalIdByIndex(profile, 0);
            if (placementId is not null)
            {
                var placement = ctx.GetEntity(placementId.Value);
                var frame = Placements.ReadAxis2Placement2D(ctx, placement);
                return new PolygonWithHoles(legacyPoints.Select(p =>
                {
                    var w = frame.ToWorld(new Vector3(p.X, p.Y, 0));
                    return new Vector2(w.X.Value, w.Y.Value);
                }).ToList());
            }
            return new PolygonWithHoles(legacyPoints);
        }

        var x = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcRectangleProfileDef.Instance.XDim));
        var y = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcRectangleProfileDef.Instance.YDim));
        var points = new List<Vector2>
        {
            new(-x / 2f, -y / 2f), new(x / 2f, -y / 2f),
            new(x / 2f, y / 2f), new(-x / 2f, y / 2f),
        };
        return ApplyPosition(ctx, profile, points);
    }

    static bool TryReadLegacyRectangleDims(MeshingContext ctx, IfcEntity profile, out float x, out float y)
    {
        x = y = 0;
        if (profile.GetEntityName() != "IFCRECTANGLEPROFILEDEF" || profile.Attributes.Count != 4)
            return false;
        var token0 = profile.GetValue(0);
        if (!token0.IsId)
            return false;
        x = ctx.ScaleLength(MeshHelpers.ReadNumberByIndex(profile, 2));
        y = ctx.ScaleLength(MeshHelpers.ReadNumberByIndex(profile, 3));
        return x > 0 && y > 0;
    }

    static PolygonWithHoles BuildRectangleHollow(MeshingContext ctx, IfcEntity profile)
    {
        var outer = BuildRectangle(ctx, profile);
        var x = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcRectangleHollowProfileDef.Instance.XDim));
        var y = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcRectangleHollowProfileDef.Instance.YDim));
        var wt = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcRectangleHollowProfileDef.Instance.WallThickness));
        var inner = new List<Vector2>
        {
            new(-x / 2f + wt, -y / 2f + wt), new(x / 2f - wt, -y / 2f + wt),
            new(x / 2f - wt, y / 2f - wt), new(-x / 2f + wt, y / 2f - wt),
        };
        var positioned = ApplyPosition(ctx, profile, inner);
        return new PolygonWithHoles(outer.Outer, [positioned.Outer]);
    }

    static PolygonWithHoles BuildRoundedRectangle(MeshingContext ctx, IfcEntity profile)
    {
        var x = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcRoundedRectangleProfileDef.Instance.XDim));
        var y = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcRoundedRectangleProfileDef.Instance.YDim));
        var r = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcRoundedRectangleProfileDef.Instance.RoundingRadius));
        r = Math.Min(r, Math.Min(x, y) / 2f);
        var points = RoundRectPoints(x, y, r, ctx.ArcSegments / 4);
        return ApplyPosition(ctx, profile, points);
    }

    static List<Vector2> RoundRectPoints(float x, float y, float r, int arcSeg)
    {
        var hx = x / 2f - r;
        var hy = y / 2f - r;
        var pts = new List<Vector2>();
        AddArc(pts, new Vector2(hx, hy), r, 0, MathF.PI / 2, arcSeg);
        AddArc(pts, new Vector2(-hx, hy), r, MathF.PI / 2, MathF.PI, arcSeg);
        AddArc(pts, new Vector2(-hx, -hy), r, MathF.PI, 3 * MathF.PI / 2, arcSeg);
        AddArc(pts, new Vector2(hx, -hy), r, 3 * MathF.PI / 2, MathF.Tau, arcSeg);
        return pts;
    }

    static PolygonWithHoles BuildCircle(MeshingContext ctx, IfcEntity profile)
    {
        var radius = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcCircleProfileDef.Instance.Radius));
        var points = SampleCircle(radius, ctx.CircleSegments);
        return ApplyPosition(ctx, profile, points);
    }

    static PolygonWithHoles BuildCircleHollow(MeshingContext ctx, IfcEntity profile)
    {
        var outerR = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcCircleHollowProfileDef.Instance.Radius));
        var wall = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcCircleHollowProfileDef.Instance.WallThickness));
        var innerR = outerR - wall;
        if (innerR <= 1e-6f)
            throw new InvalidOperationException($"Invalid hollow circle profile #{profile.Id}: inner radius <= 0");
        var outer = ApplyPosition(ctx, profile, SampleCircle(outerR, ctx.CircleSegments));
        var inner = ApplyPosition(ctx, profile, SampleCircle(innerR, ctx.CircleSegments));
        return new PolygonWithHoles(outer.Outer, [inner.Outer]);
    }

    static PolygonWithHoles BuildEllipse(MeshingContext ctx, IfcEntity profile)
    {
        var a = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcEllipseProfileDef.Instance.SemiAxis1));
        var b = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcEllipseProfileDef.Instance.SemiAxis2));
        var points = Enumerable.Range(0, ctx.CircleSegments)
            .Select(i =>
            {
                var t = MathF.Tau * i / ctx.CircleSegments;
                return new Vector2(a * MathF.Cos(t), b * MathF.Sin(t));
            }).ToList();
        return ApplyPosition(ctx, profile, points);
    }

    static PolygonWithHoles BuildIShape(MeshingContext ctx, IfcEntity profile)
    {
        var w = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcIShapeProfileDef.Instance.OverallWidth));
        var d = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcIShapeProfileDef.Instance.OverallDepth));
        var tw = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcIShapeProfileDef.Instance.WebThickness));
        var tf = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcIShapeProfileDef.Instance.FlangeThickness));
        var fillet = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcIShapeProfileDef.Instance.FilletRadius));
        var hw = w / 2f;
        var hd = d / 2f;
        var points = fillet > 1e-6f
            ? BuildIShapeWithFillet(hw, hd, tw, tf, fillet, ctx.ArcSegments / 4)
            :
            [
                new(-hw, -hd), new(hw, -hd), new(hw, -hd + tf), new(tw / 2, -hd + tf),
                new(tw / 2, hd - tf), new(hw, hd - tf), new(hw, hd), new(-hw, hd),
                new(-hw, hd - tf), new(-tw / 2, hd - tf), new(-tw / 2, -hd + tf), new(-hw, -hd + tf),
            ];
        return ApplyPosition(ctx, profile, points);
    }

    static List<Vector2> BuildIShapeWithFillet(float hw, float hd, float tw, float tf, float r, int arcSeg)
    {
        r = Math.Min(r, Math.Min(tf, tw) / 2f);
        var pts = new List<Vector2> { new(-hw, -hd), new(hw, -hd), new(hw, -hd + tf) };
        AddArc(pts, new Vector2(tw / 2 + r, -hd + tf + r), r, MathF.PI, 3 * MathF.PI / 2, arcSeg);
        pts.Add(new(tw / 2, hd - tf));
        AddArc(pts, new Vector2(tw / 2 + r, hd - tf - r), r, MathF.PI / 2, MathF.PI, arcSeg);
        pts.AddRange([new(hw, hd - tf), new(hw, hd), new(-hw, hd), new(-hw, hd - tf)]);
        AddArc(pts, new Vector2(-tw / 2 - r, hd - tf - r), r, 0, MathF.PI / 2, arcSeg);
        pts.Add(new(-tw / 2, -hd + tf));
        AddArc(pts, new Vector2(-tw / 2 - r, -hd + tf + r), r, 3 * MathF.PI / 2, MathF.Tau, arcSeg);
        pts.Add(new(-hw, -hd + tf));
        return pts;
    }

    static PolygonWithHoles BuildLShape(MeshingContext ctx, IfcEntity profile)
    {
        var depth = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcLShapeProfileDef.Instance.Depth));
        var width = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcLShapeProfileDef.Instance.Width));
        var t = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcLShapeProfileDef.Instance.Thickness));
        var hw = width / 2f;
        var hd = depth / 2f;
        var points = new List<Vector2>
        {
            new(-hw, -hd), new(hw, -hd), new(hw, -hd + t), new(-hw + t, -hd + t),
            new(-hw + t, hd), new(-hw, hd),
        };
        return ApplyPosition(ctx, profile, points);
    }

    static PolygonWithHoles BuildUShape(MeshingContext ctx, IfcEntity profile)
    {
        var d = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcUShapeProfileDef.Instance.Depth));
        var w = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcUShapeProfileDef.Instance.FlangeWidth));
        var tf = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcUShapeProfileDef.Instance.FlangeThickness));
        var tw = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcUShapeProfileDef.Instance.WebThickness));
        var points = new List<Vector2>
        {
            new(-w / 2, -d / 2), new(w / 2, -d / 2), new(w / 2, -d / 2 + tf),
            new(tw / 2, -d / 2 + tf), new(tw / 2, d / 2 - tf),
            new(w / 2, d / 2 - tf), new(w / 2, d / 2), new(-w / 2, d / 2),
            new(-w / 2, d / 2 - tf), new(-tw / 2, d / 2 - tf),
            new(-tw / 2, -d / 2 + tf), new(-w / 2, -d / 2 + tf),
        };
        return ApplyPosition(ctx, profile, points);
    }

    static PolygonWithHoles BuildTShape(MeshingContext ctx, IfcEntity profile)
    {
        var depth = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcTShapeProfileDef.Instance.Depth));
        var flangeWidth = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcTShapeProfileDef.Instance.FlangeWidth));
        var webThick = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcTShapeProfileDef.Instance.WebThickness));
        var flangeThick = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcTShapeProfileDef.Instance.FlangeThickness));
        var points = new List<Vector2>
        {
            new(-flangeWidth / 2, depth / 2 - flangeThick), new(flangeWidth / 2, depth / 2 - flangeThick),
            new(flangeWidth / 2, depth / 2), new(-flangeWidth / 2, depth / 2),
            new(-flangeWidth / 2, depth / 2 - flangeThick), new(-webThick / 2, depth / 2 - flangeThick),
            new(-webThick / 2, -depth / 2), new(webThick / 2, -depth / 2),
            new(webThick / 2, depth / 2 - flangeThick), new(-flangeWidth / 2, depth / 2 - flangeThick),
        };
        return ApplyPosition(ctx, profile, SimplifyPolygon(points));
    }

    static PolygonWithHoles BuildZShape(MeshingContext ctx, IfcEntity profile)
    {
        var depth = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcZShapeProfileDef.Instance.Depth));
        var flangeWidth = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcZShapeProfileDef.Instance.FlangeWidth));
        var webThick = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcZShapeProfileDef.Instance.WebThickness));
        var flangeThick = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcZShapeProfileDef.Instance.FlangeThickness));
        var points = new List<Vector2>
        {
            new(-flangeWidth / 2, depth / 2), new(flangeWidth / 2, depth / 2),
            new(flangeWidth / 2, depth / 2 - flangeThick), new(webThick / 2, depth / 2 - flangeThick),
            new(webThick / 2, -depth / 2 + flangeThick), new(flangeWidth / 2, -depth / 2 + flangeThick),
            new(flangeWidth / 2, -depth / 2), new(-flangeWidth / 2, -depth / 2),
            new(-flangeWidth / 2, -depth / 2 + flangeThick), new(-webThick / 2, -depth / 2 + flangeThick),
            new(-webThick / 2, depth / 2 - flangeThick), new(-flangeWidth / 2, depth / 2 - flangeThick),
        };
        return ApplyPosition(ctx, profile, points);
    }

    static PolygonWithHoles BuildCShape(MeshingContext ctx, IfcEntity profile)
    {
        var depth = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcCShapeProfileDef.Instance.Depth));
        var width = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcCShapeProfileDef.Instance.Width));
        var wallThick = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcCShapeProfileDef.Instance.WallThickness));
        var girth = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcCShapeProfileDef.Instance.Girth));
        var points = new List<Vector2>
        {
            new(-width / 2, -depth / 2), new(width / 2 - girth, -depth / 2),
            new(width / 2 - girth, -depth / 2 + wallThick), new(-width / 2 + wallThick, -depth / 2 + wallThick),
            new(-width / 2 + wallThick, depth / 2 - wallThick),
            new(width / 2 - girth, depth / 2 - wallThick),
            new(width / 2 - girth, depth / 2), new(-width / 2, depth / 2),
        };
        return ApplyPosition(ctx, profile, points);
    }

    static PolygonWithHoles BuildTrapezium(MeshingContext ctx, IfcEntity profile)
    {
        var bx = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcTrapeziumProfileDef.Instance.BottomXDim));
        var tx = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcTrapeziumProfileDef.Instance.TopXDim));
        var y = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcTrapeziumProfileDef.Instance.YDim));
        var dx = ctx.ScaleLength(MeshHelpers.ReadNumber(profile, IfcTrapeziumProfileDef.Instance.TopXOffset));
        var points = new List<Vector2>
        {
            new(-bx / 2, -y / 2), new(bx / 2, -y / 2),
            new(dx + tx / 2, y / 2), new(dx - tx / 2, y / 2),
        };
        return ApplyPosition(ctx, profile, points);
    }

    static PolygonWithHoles BuildArbitraryClosed(MeshingContext ctx, IfcEntity profile)
    {
        var curve = MeshHelpers.ResolveRequired(ctx, profile, IfcArbitraryClosedProfileDef.Instance.OuterCurve);
        return BuildArbitraryClosedFromCurve(ctx, curve);
    }

    static PolygonWithHoles BuildArbitraryClosedFromCurve(MeshingContext ctx, IfcEntity curve)
    {
        var points = CurveEvaluator.Evaluate2D(ctx, curve, dropClosure: true);
        return new PolygonWithHoles(PolygonWithHoles.CleanRing(points));
    }

    static IReadOnlyList<Vector2> BuildArbitraryOpen(MeshingContext ctx, IfcEntity profile)
    {
        var curve = MeshHelpers.ResolveRequired(ctx, profile, IfcArbitraryOpenProfileDef.Instance.Curve);
        return CurveEvaluator.Evaluate2D(ctx, curve, dropClosure: false);
    }

    /// <summary>Open profiles cannot form solids; only used when callers need a closed fallback.</summary>
    static PolygonWithHoles BuildArbitraryOpenAsClosed(MeshingContext ctx, IfcEntity profile)
        => throw new NotSupportedException(
            $"Open profile #{profile.Id} cannot form a closed area; use {nameof(BuildOpen)} for ribbon extrusion");

    static PolygonWithHoles BuildArbitrary(MeshingContext ctx, IfcEntity profile)
    {
        var curveId = MeshHelpers.ReadOptionalIdByIndex(profile, 2)
            ?? throw new InvalidOperationException($"IFCARBITRARYPROFILEDEF #{profile.Id} missing curve");
        var curve = ctx.GetEntity(curveId);
        if (IsCurveClosed(ctx, curve))
            return BuildArbitraryClosedFromCurve(ctx, curve);
        throw new NotSupportedException(
            $"Open IFCARBITRARYPROFILEDEF #{profile.Id}; use open-path extrusion instead of {nameof(Build)}");
    }

    static PolygonWithHoles BuildArbitraryWithVoids(MeshingContext ctx, IfcEntity profile)
    {
        var outerCurve = MeshHelpers.ResolveRequired(ctx, profile, IfcArbitraryProfileDefWithVoids.Instance.OuterCurve);
        var outer = BuildArbitraryClosedFromCurve(ctx, outerCurve);
        var holes = MeshHelpers.ReadIds(profile, IfcArbitraryProfileDefWithVoids.Instance.InnerCurves)
            .Select(id => PolygonWithHoles.CleanRing(CurveEvaluator.Evaluate2D(ctx, ctx.GetEntity(id), dropClosure: true)))
            .Cast<IReadOnlyList<Vector2>>()
            .ToList();
        return new PolygonWithHoles(outer.Outer, holes);
    }

    static PolygonWithHoles BuildComposite(MeshingContext ctx, IfcEntity profile)
    {
        var profiles = MeshHelpers.ReadIds(profile, IfcCompositeProfileDef.Instance.Profiles)
            .Select(id => Build(ctx, ctx.GetEntity(id)))
            .ToList();
        if (profiles.Count == 0)
            throw new InvalidOperationException("Empty composite profile");
        var outer = profiles[0].Outer.ToList();
        var holes = profiles.Skip(1).Select(p => p.Outer).ToList();
        return new PolygonWithHoles(outer, holes);
    }

    static PolygonWithHoles BuildDerived(MeshingContext ctx, IfcEntity profile)
    {
        var parent = Build(ctx, MeshHelpers.ResolveRequired(ctx, profile, IfcDerivedProfileDef.Instance.ParentProfile));
        var op = MeshHelpers.ResolveRequired(ctx, profile, IfcDerivedProfileDef.Instance.Operator);
        var map = Placements.ReadProfileTransformationOperator(ctx, op);
        var outer = parent.Outer.Select(p =>
        {
            var w = new Vector3(p.X, p.Y, 0).Transform(map);
            return new Vector2(w.X.Value, w.Y.Value);
        }).ToList();
        var holes = parent.Holes.Select(h => h.Select(p =>
        {
            var w = new Vector3(p.X, p.Y, 0).Transform(map);
            return new Vector2(w.X.Value, w.Y.Value);
        }).ToList()).Cast<IReadOnlyList<Vector2>>().ToList();
        return new PolygonWithHoles(outer, holes);
    }

    static List<Vector2> SampleCircle(float radius, int segments)
        => Enumerable.Range(0, segments).Select(i =>
        {
            var t = MathF.Tau * i / segments;
            return new Vector2(MathF.Cos(t) * radius, MathF.Sin(t) * radius);
        }).ToList();

    static void AddArc(List<Vector2> pts, Vector2 center, float r, float a0, float a1, int segments)
    {
        for (var i = 0; i <= segments; i++)
        {
            var a = a0 + (a1 - a0) * i / segments;
            pts.Add(center + new Vector2(MathF.Cos(a), MathF.Sin(a)) * r);
        }
    }

    static List<Vector2> SimplifyPolygon(List<Vector2> points)
    {
        if (points.Count < 2)
            return points;
        var result = new List<Vector2> { points[0] };
        for (var i = 1; i < points.Count; i++)
        {
            if (points[i].DistanceSquared(result[^1]) > 1e-10f)
                result.Add(points[i]);
        }
        return result;
    }
}

internal static class Point3DVector2Ext
{
    public static Vector2 Vector2(this Point3D p) => new(p.X.Value, p.Y.Value);
}
