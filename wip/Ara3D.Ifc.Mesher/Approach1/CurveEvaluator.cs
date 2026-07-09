using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.IfcTypes;
using Ara3D.IO.StepParser;

namespace Ara3D.Ifc.Mesher.Approach1;

/// <summary>
/// Evaluates IFC curves to polylines. Arcs and B-splines are uniformly sampled (approximate).
/// </summary>
public static class CurveEvaluator
{
    static float DefaultJoinToleranceSquared => PolygonTriangulator.Eps * PolygonTriangulator.Eps;

    static float CompositeJoinToleranceSquared(MeshingContext ctx)
    {
        var tol = Math.Max(PolygonTriangulator.Eps, MathF.Abs((float)ctx.LengthScale) * 0.05f);
        return tol * tol;
    }

    public static float JoinToleranceSquaredFor(MeshingContext ctx) => CompositeJoinToleranceSquared(ctx);

    public static IReadOnlyList<Vector2> Evaluate2D(MeshingContext ctx, IfcEntity curve)
        => Evaluate2D(ctx, curve, false);

    public static IReadOnlyList<Vector2> Evaluate2D(MeshingContext ctx, IfcEntity curve, bool dropClosure)
    {
        var points = curve.GetEntityName() switch
        {
            "IFCPOLYLINE" => EvaluatePolyline2D(ctx, curve),
            "IFCINDEXEDPOLYCURVE" => EvaluateIndexedPolyCurve2D(ctx, curve),
            "IFCCIRCLE" => EvaluateCircle2D(ctx, curve, full: true),
            "IFCELLIPSE" => EvaluateEllipse2D(ctx, curve, full: true),
            "IFCLINE" => EvaluateLine2D(ctx, curve),
            "IFCTRIMMEDCURVE" => EvaluateTrimmedCurve2D(ctx, curve),
            "IFCCOMPOSITECURVE" => EvaluateCompositeCurve2D(ctx, curve),
            "IFCBSPLINECURVEWITHKNOTS" or "IFCBSPLINECURVE" => EvaluateBSpline2D(ctx, curve),
            _ => throw new NotSupportedException($"Unsupported 2D curve {curve.GetEntityName()} #{curve.Id}"),
        };

        ctx.Diagnostics.RecordSupported(curve.GetEntityName());
        var list = dropClosure ? SanitizeRingPoints(points) : SanitizeOpenPathPoints(points);
        if (dropClosure)
            RemoveDuplicateClosure(list);
        return list;
    }

    public static IReadOnlyList<Vector3> Evaluate3D(MeshingContext ctx, IfcEntity curve)
    {
        var points = curve.GetEntityName() switch
        {
            "IFCPOLYLINE" => EvaluatePolyline3D(ctx, curve),
            "IFCINDEXEDPOLYCURVE" => EvaluateIndexedPolyCurve3D(ctx, curve),
            "IFCCIRCLE" => EvaluateCircle3D(ctx, curve, full: true),
            "IFCELLIPSE" => EvaluateEllipse3D(ctx, curve, full: true),
            "IFCLINE" => EvaluateLine3D(ctx, curve),
            "IFCTRIMMEDCURVE" => EvaluateTrimmedCurve3D(ctx, curve),
            "IFCCOMPOSITECURVE" => EvaluateCompositeCurve3D(ctx, curve),
            "IFCBSPLINECURVEWITHKNOTS" or "IFCBSPLINECURVE" => EvaluateBSpline3D(ctx, curve),
            _ => throw new NotSupportedException($"Unsupported 3D curve {curve.GetEntityName()} #{curve.Id}"),
        };
        ctx.Diagnostics.RecordSupported(curve.GetEntityName());
        return points;
    }

    static List<Vector2> EvaluatePolyline2D(MeshingContext ctx, IfcEntity polyline)
        => MeshHelpers.ReadIds(polyline, IfcPolyline.Instance.Points)
            .Select(id => Placements.ReadPoint2D(ctx, ctx.GetEntity(id)))
            .ToList();

    static List<Vector3> EvaluatePolyline3D(MeshingContext ctx, IfcEntity polyline)
        => MeshHelpers.ReadIds(polyline, IfcPolyline.Instance.Points)
            .Select(id => Placements.ReadPoint3D(ctx, ctx.GetEntity(id)))
            .ToList();

    static List<Vector2> EvaluateIndexedPolyCurve2D(MeshingContext ctx, IfcEntity curve)
    {
        var points = ReadPointList2D(ctx, curve);
        var segmentToken = curve.GetValue(IfcIndexedPolyCurve.Instance.Segments.Index);
        if (!segmentToken.IsList)
            return points;

        var result = new List<Vector2>();
        foreach (var segToken in segmentToken.AsList(curve.Document))
        {
            if (segToken.IsId)
                AppendIndexedPolyCurveSegment(ctx, result, points, ctx.GetEntity(segToken.AsId()));
            else if (segToken.IsEntity)
            {
                var (name, value) = segToken.AsSimpleEntity(curve.Document);
                AppendIndexedPolyCurveSegment(ctx, result, points, name, value, curve.Document);
            }
        }
        return DeduplicateConsecutive(result.Count > 0 ? result : points);
    }

    static List<Vector2> DeduplicateConsecutive(IReadOnlyList<Vector2> points)
        => SanitizeRingPoints(points);

    internal static List<Vector2> SanitizeRingPoints(IReadOnlyList<Vector2> points, float joinToleranceSquared = 0)
    {
        var result = SanitizeOpenPathPoints(points, joinToleranceSquared);
        if (joinToleranceSquared <= 0)
            joinToleranceSquared = DefaultJoinToleranceSquared;
        if (result.Count >= 2 && result[0].DistanceSquared(result[^1]) <= joinToleranceSquared)
            result.RemoveAt(result.Count - 1);
        return result;
    }

    /// <summary>Deduplicates consecutive points without treating near-coincident endpoints as closure.</summary>
    internal static List<Vector2> SanitizeOpenPathPoints(IReadOnlyList<Vector2> points, float joinToleranceSquared = 0)
    {
        if (joinToleranceSquared <= 0)
            joinToleranceSquared = DefaultJoinToleranceSquared;
        var result = new List<Vector2>(points.Count);
        foreach (var p in points)
        {
            if (result.Count == 0 || result[^1].DistanceSquared(p) > joinToleranceSquared)
                result.Add(p);
        }
        return result;
    }

    static void AppendIndexedPolyCurveSegment(
        MeshingContext ctx,
        List<Vector2> result,
        IReadOnlyList<Vector2> points,
        IfcEntity seg)
    {
        if (seg.GetEntityName() == "IFCLINEINDEX")
        {
            ctx.Diagnostics.RecordSupported("IFCLINEINDEX");
            foreach (var idx in ReadOneBasedIndices(seg))
                result.Add(points[idx - 1]);
        }
        else if (seg.GetEntityName() == "IFCARCINDEX")
        {
            var indices = ReadOneBasedIndices(seg);
            if (indices.Count >= 3)
                AppendArc2D(result, points[indices[0] - 1], points[indices[1] - 1], points[indices[2] - 1], ctx.ArcSegments);
        }
    }

    static void AppendIndexedPolyCurveSegment(
        MeshingContext ctx,
        List<Vector2> result,
        IReadOnlyList<Vector2> points,
        string segmentName,
        StepToken value,
        StepDocument doc)
    {
        if (segmentName == "IFCLINEINDEX")
        {
            ctx.Diagnostics.RecordSupported("IFCLINEINDEX");
            foreach (var idx in ReadOneBasedIndices(value, doc))
                result.Add(points[idx - 1]);
        }
        else if (segmentName == "IFCARCINDEX")
        {
            var indices = ReadOneBasedIndices(value, doc);
            if (indices.Count >= 3)
                AppendArc2D(result, points[indices[0] - 1], points[indices[1] - 1], points[indices[2] - 1], ctx.ArcSegments);
        }
    }

    static List<Vector3> EvaluateIndexedPolyCurve3D(MeshingContext ctx, IfcEntity curve)
    {
        var points3D = ReadPointList3D(ctx, curve);
        if (points3D.Count > 0)
            return points3D;

        return EvaluateIndexedPolyCurve2D(ctx, curve).Select(p => new Vector3(p.X, p.Y, 0)).ToList();
    }

    static List<Vector3> ReadPointList3D(MeshingContext ctx, IfcEntity indexed)
    {
        var coordsId = indexed.GetId(IfcIndexedPolyCurve.Instance.Points.Index);
        var list = ctx.GetEntity(coordsId);
        if (list.GetEntityName() != "IFCCARTESIANPOINTLIST3D")
            return [];

        var result = new List<Vector3>();
        var token = list.GetValue(IfcCartesianPointList3D.Instance.CoordList.Index);
        if (!token.IsList)
            return result;
        foreach (var item in token.AsList(list.Document))
        {
            if (!item.IsList)
                continue;
            var nums = item.AsList(list.Document).Where(t => t.IsNumber).Select(t => t.AsNumber()).ToList();
            result.Add(new Vector3(
                ctx.ScaleLength(nums.Count > 0 ? nums[0] : 0),
                ctx.ScaleLength(nums.Count > 1 ? nums[1] : 0),
                ctx.ScaleLength(nums.Count > 2 ? nums[2] : 0)));
        }
        return result;
    }

    static List<Vector2> ReadPointList2D(MeshingContext ctx, IfcEntity indexed)
    {
        var coordsId = indexed.GetId(IfcIndexedPolyCurve.Instance.Points.Index);
        var list = ctx.GetEntity(coordsId);
        if (list.GetEntityName() == "IFCCARTESIANPOINTLIST2D")
        {
            return ReadCoordList2D(ctx, list);
        }
        return MeshHelpers.ReadIds(indexed, IfcIndexedPolyCurve.Instance.Points)
            .Select(id => Placements.ReadPoint2D(ctx, ctx.GetEntity(id)))
            .ToList();
    }

    static List<Vector2> ReadCoordList2D(MeshingContext ctx, IfcEntity list)
    {
        var result = new List<Vector2>();
        var token = list.GetValue(0);
        if (!token.IsList)
            return result;
        foreach (var item in token.AsList(list.Document))
        {
            if (!item.IsList)
                continue;
            var nums = item.AsList(list.Document).Where(t => t.IsNumber).Select(t => t.AsNumber()).ToList();
            result.Add(new Vector2(
                ctx.ScaleLength(nums.Count > 0 ? nums[0] : 0),
                ctx.ScaleLength(nums.Count > 1 ? nums[1] : 0)));
        }
        return result;
    }

    static List<int> ReadOneBasedIndices(IfcEntity seg)
    {
        var token = seg.GetValue(0);
        return ReadOneBasedIndices(token, seg.Document);
    }

    static List<int> ReadOneBasedIndices(StepToken token, StepDocument doc)
    {
        if (!token.IsList)
            return [];
        return token.AsList(doc).Where(t => t.IsNumber).Select(t => (int)t.AsNumber()).ToList();
    }

    static List<Vector2> EvaluateCircle2D(MeshingContext ctx, IfcEntity circle, bool full)
    {
        var radius = (float)ctx.ScaleLength(MeshHelpers.ReadNumber(circle, IfcCircle.Instance.Radius));
        var placement = Placements.ReadOptionalAxis2Placement3D(ctx, circle, IfcConic.Instance.Position);
        return SampleCircle2D(radius, ctx.CircleSegments, full)
            .Select(p => PlacementsReadWorld2(placement, p))
            .ToList();
    }

    static List<Vector3> EvaluateCircle3D(MeshingContext ctx, IfcEntity circle, bool full)
    {
        var radius = (float)ctx.ScaleLength(MeshHelpers.ReadNumber(circle, IfcCircle.Instance.Radius));
        var placement = Placements.ReadOptionalAxis2Placement3D(ctx, circle, IfcConic.Instance.Position);
        return SampleCircle2D(radius, ctx.CircleSegments, full)
            .Select(p => placement.ToWorld(new Vector3(p.X, p.Y, 0)))
            .ToList();
    }

    static IEnumerable<Vector2> SampleCircle2D(float radius, int segments, bool full)
    {
        var count = full ? segments : segments / 2;
        for (var i = 0; i <= count; i++)
        {
            var t = MathF.Tau * i / segments;
            yield return new Vector2(MathF.Cos(t) * radius, MathF.Sin(t) * radius);
        }
    }

    static List<Vector2> EvaluateEllipse2D(MeshingContext ctx, IfcEntity ellipse, bool full)
    {
        var semiMajor = (float)ctx.ScaleLength(MeshHelpers.ReadNumber(ellipse, IfcEllipse.Instance.SemiAxis1));
        var semiMinor = (float)ctx.ScaleLength(MeshHelpers.ReadNumber(ellipse, IfcEllipse.Instance.SemiAxis2));
        var placement = Placements.ReadOptionalAxis2Placement3D(ctx, ellipse, IfcConic.Instance.Position);
        var result = new List<Vector2>();
        var count = full ? ctx.CircleSegments : ctx.CircleSegments / 2;
        for (var i = 0; i <= count; i++)
        {
            var t = MathF.Tau * i / ctx.CircleSegments;
            var local = new Vector3(semiMajor * MathF.Cos(t), semiMinor * MathF.Sin(t), 0);
            var w = placement.ToWorld(local);
            result.Add(new Vector2(w.X.Value, w.Y.Value));
        }
        return result;
    }

    static List<Vector3> EvaluateEllipse3D(MeshingContext ctx, IfcEntity ellipse, bool full)
        => EvaluateEllipse2D(ctx, ellipse, full).Select(p => new Vector3(p.X, p.Y, 0)).ToList();

    static List<Vector2> EvaluateLine2D(MeshingContext ctx, IfcEntity line)
    {
        var origin = Placements.ReadPoint3D(ctx, MeshHelpers.ResolveRequired(ctx, line, IfcLine.Instance.Pnt));
        var dir = ReadLineDirection(ctx, line);
        return [new Vector2(origin.X, origin.Y), new Vector2(origin.X + dir.X, origin.Y + dir.Y)];
    }

    static List<Vector3> EvaluateLine3D(MeshingContext ctx, IfcEntity line)
    {
        var origin = Placements.ReadPoint3D(ctx, MeshHelpers.ResolveRequired(ctx, line, IfcLine.Instance.Pnt));
        var dir = ReadLineDirection(ctx, line);
        return [origin, origin + dir];
    }

    static Vector3 ReadLineDirection(MeshingContext ctx, IfcEntity line)
    {
        var vector = MeshHelpers.ResolveRequired(ctx, line, IfcLine.Instance.Dir);
        var dirEntity = MeshHelpers.ResolveRequired(ctx, vector, IfcVector.Instance.Orientation);
        var dir = Placements.ReadDirection3D(ctx, dirEntity, Vector3.UnitX);
        var magnitude = (float)ctx.ScaleLength(MeshHelpers.ReadNumber(vector, IfcVector.Instance.Magnitude));
        if (magnitude > 1e-12f)
            dir *= magnitude;
        return dir.Normalize;
    }

    static List<Vector2> EvaluateTrimmedCurve2D(MeshingContext ctx, IfcEntity trimmed, bool invertSenseAgreement = false)
    {
        var basis = MeshHelpers.ResolveRequired(ctx, trimmed, IfcTrimmedCurve.Instance.BasisCurve);
        var sense = MeshHelpers.ReadOptionalBool(trimmed, IfcTrimmedCurve.Instance.SenseAgreement, true);
        if (invertSenseAgreement)
            sense = !sense;
        return basis.GetEntityName() switch
        {
            "IFCCIRCLE" => TrimCircle2D(ctx, basis, trimmed, sense),
            "IFCELLIPSE" => TrimEllipse2D(ctx, basis, trimmed, sense),
            "IFCLINE" => TrimLine2D(ctx, basis, trimmed, sense),
            _ => TrimSampledPolyline2D(ctx, trimmed, Evaluate2D(ctx, basis), sense),
        };
    }

    static List<Vector3> EvaluateTrimmedCurve3D(MeshingContext ctx, IfcEntity trimmed)
    {
        var basis = MeshHelpers.ResolveRequired(ctx, trimmed, IfcTrimmedCurve.Instance.BasisCurve);
        var sense = MeshHelpers.ReadOptionalBool(trimmed, IfcTrimmedCurve.Instance.SenseAgreement, true);
        return basis.GetEntityName() switch
        {
            "IFCCIRCLE" => TrimCircle3D(ctx, basis, trimmed, sense),
            "IFCELLIPSE" => TrimEllipse3D(ctx, basis, trimmed, sense),
            "IFCLINE" => TrimLine3D(ctx, basis, trimmed, sense),
            _ => TrimSampledPolyline3D(ctx, trimmed, Evaluate3D(ctx, basis).ToList(), sense),
        };
    }

    static List<Vector2> TrimCircle2D(MeshingContext ctx, IfcEntity circle, IfcEntity trimmed, bool sense)
    {
        var u1 = ReadTrimParameter(ctx, trimmed, circle, IfcTrimmedCurve.Instance.Trim1);
        var u2 = ReadTrimParameter(ctx, trimmed, circle, IfcTrimmedCurve.Instance.Trim2);
        var radius = (float)ctx.ScaleLength(MeshHelpers.ReadNumber(circle, IfcCircle.Instance.Radius));
        var placement = Placements.ReadOptionalAxis2Placement3D(ctx, circle, IfcConic.Instance.Position);
        return SampleConicArc2D(placement, radius, radius, u1, u2, sense, ctx.CircleSegments);
    }

    static List<Vector3> TrimCircle3D(MeshingContext ctx, IfcEntity circle, IfcEntity trimmed, bool sense)
    {
        var u1 = ReadTrimParameter(ctx, trimmed, circle, IfcTrimmedCurve.Instance.Trim1);
        var u2 = ReadTrimParameter(ctx, trimmed, circle, IfcTrimmedCurve.Instance.Trim2);
        var radius = (float)ctx.ScaleLength(MeshHelpers.ReadNumber(circle, IfcCircle.Instance.Radius));
        var placement = Placements.ReadOptionalAxis2Placement3D(ctx, circle, IfcConic.Instance.Position);
        return SampleConicArc3D(placement, radius, radius, u1, u2, sense, ctx.CircleSegments);
    }

    static List<Vector2> TrimEllipse2D(MeshingContext ctx, IfcEntity ellipse, IfcEntity trimmed, bool sense)
    {
        var u1 = ReadTrimParameter(ctx, trimmed, ellipse, IfcTrimmedCurve.Instance.Trim1);
        var u2 = ReadTrimParameter(ctx, trimmed, ellipse, IfcTrimmedCurve.Instance.Trim2);
        var a = (float)ctx.ScaleLength(MeshHelpers.ReadNumber(ellipse, IfcEllipse.Instance.SemiAxis1));
        var b = (float)ctx.ScaleLength(MeshHelpers.ReadNumber(ellipse, IfcEllipse.Instance.SemiAxis2));
        var placement = Placements.ReadOptionalAxis2Placement3D(ctx, ellipse, IfcConic.Instance.Position);
        return SampleConicArc2D(placement, a, b, u1, u2, sense, ctx.CircleSegments);
    }

    static List<Vector3> TrimEllipse3D(MeshingContext ctx, IfcEntity ellipse, IfcEntity trimmed, bool sense)
    {
        var u1 = ReadTrimParameter(ctx, trimmed, ellipse, IfcTrimmedCurve.Instance.Trim1);
        var u2 = ReadTrimParameter(ctx, trimmed, ellipse, IfcTrimmedCurve.Instance.Trim2);
        var a = (float)ctx.ScaleLength(MeshHelpers.ReadNumber(ellipse, IfcEllipse.Instance.SemiAxis1));
        var b = (float)ctx.ScaleLength(MeshHelpers.ReadNumber(ellipse, IfcEllipse.Instance.SemiAxis2));
        var placement = Placements.ReadOptionalAxis2Placement3D(ctx, ellipse, IfcConic.Instance.Position);
        return SampleConicArc3D(placement, a, b, u1, u2, sense, ctx.CircleSegments);
    }

    static List<Vector2> TrimLine2D(MeshingContext ctx, IfcEntity line, IfcEntity trimmed, bool sense)
    {
        var origin = Placements.ReadPoint3D(ctx, MeshHelpers.ResolveRequired(ctx, line, IfcLine.Instance.Pnt));
        var dir = ReadLineDirection(ctx, line);
        var u1 = ReadTrimParameter(ctx, trimmed, line, IfcTrimmedCurve.Instance.Trim1);
        var u2 = ReadTrimParameter(ctx, trimmed, line, IfcTrimmedCurve.Instance.Trim2);
        var p1 = origin + dir * u1;
        var p2 = origin + dir * u2;
        var pts = new List<Vector2> { new(p1.X, p1.Y), new(p2.X, p2.Y) };
        if (!sense)
            pts.Reverse();
        return pts;
    }

    static List<Vector3> TrimLine3D(MeshingContext ctx, IfcEntity line, IfcEntity trimmed, bool sense)
    {
        var origin = Placements.ReadPoint3D(ctx, MeshHelpers.ResolveRequired(ctx, line, IfcLine.Instance.Pnt));
        var dir = ReadLineDirection(ctx, line);
        var u1 = ReadTrimParameter(ctx, trimmed, line, IfcTrimmedCurve.Instance.Trim1);
        var u2 = ReadTrimParameter(ctx, trimmed, line, IfcTrimmedCurve.Instance.Trim2);
        var pts = new List<Vector3> { origin + dir * u1, origin + dir * u2 };
        if (!sense)
            pts.Reverse();
        return pts;
    }

    static List<Vector2> TrimSampledPolyline2D(MeshingContext ctx, IfcEntity trimmed, IReadOnlyList<Vector2> full, bool sense)
    {
        var list = TrimSampledPolyline(ctx, trimmed, full, sense);
        return list;
    }

    static List<Vector3> TrimSampledPolyline3D(MeshingContext ctx, IfcEntity trimmed, IReadOnlyList<Vector3> full, bool sense)
    {
        if (full.Count == 0)
            return [];

        if (TryReadTrimPoints3D(ctx, trimmed, out var p1, out var p2))
        {
            var i1 = NearestIndex3D(full, p1);
            var i2 = NearestIndex3D(full, p2);
            var list = i1 <= i2
                ? full.Skip(i1).Take(i2 - i1 + 1).ToList()
                : full.Skip(i1).Concat(full.Take(i2 + 1)).ToList();
            if (!sense)
                list.Reverse();
            return list;
        }

        var u1 = ReadTrimParameter(ctx, trimmed, null, IfcTrimmedCurve.Instance.Trim1);
        var u2 = ReadTrimParameter(ctx, trimmed, null, IfcTrimmedCurve.Instance.Trim2);
        var start = (int)(u1 * (full.Count - 1));
        var end = (int)(u2 * (full.Count - 1));
        start = Math.Clamp(start, 0, full.Count - 1);
        end = Math.Clamp(end, 0, full.Count - 1);
        var slice = start <= end ? full.Skip(start).Take(end - start + 1) : full.Skip(start).Concat(full.Take(end + 1));
        var result = slice.ToList();
        if (!sense)
            result.Reverse();
        return result;
    }

    static List<Vector2> TrimSampledPolyline(MeshingContext ctx, IfcEntity trimmed, IReadOnlyList<Vector2> full, bool sense)
    {
        if (full.Count == 0)
            return [];

        if (TryReadTrimPoints2D(ctx, trimmed, out var p1, out var p2))
        {
            var i1 = NearestIndex(full, p1);
            var i2 = NearestIndex(full, p2);
            var list = i1 <= i2
                ? full.Skip(i1).Take(i2 - i1 + 1).ToList()
                : full.Skip(i1).Concat(full.Take(i2 + 1)).ToList();
            if (!sense)
                list.Reverse();
            return list;
        }

        var u1 = ReadTrimParameter(ctx, trimmed, null, IfcTrimmedCurve.Instance.Trim1);
        var u2 = ReadTrimParameter(ctx, trimmed, null, IfcTrimmedCurve.Instance.Trim2);
        var start = (int)(u1 * (full.Count - 1));
        var end = (int)(u2 * (full.Count - 1));
        start = Math.Clamp(start, 0, full.Count - 1);
        end = Math.Clamp(end, 0, full.Count - 1);
        var slice = start <= end ? full.Skip(start).Take(end - start + 1) : full.Skip(start).Concat(full.Take(end + 1));
        var result = slice.ToList();
        if (!sense)
            result.Reverse();
        return result;
    }

    static List<Vector2> SampleConicArc2D(Frame3D placement, float a, float b, float u1, float u2, bool sense, int segments)
    {
        var span = NormalizeAngleSpan(u2 - u1, sense);
        var steps = ConicArcSampleCount(span, segments, Math.Max(a, b));
        var result = new List<Vector2>(steps + 1);
        for (var i = 0; i <= steps; i++)
        {
            var u = u1 + span * i / steps;
            var local = new Vector3(a * MathF.Cos(u), b * MathF.Sin(u), 0);
            var w = placement.ToWorld(local);
            result.Add(new Vector2(w.X.Value, w.Y.Value));
        }
        return result;
    }

    static List<Vector3> SampleConicArc3D(Frame3D placement, float a, float b, float u1, float u2, bool sense, int segments)
    {
        var span = NormalizeAngleSpan(u2 - u1, sense);
        var steps = ConicArcSampleCount(span, segments, Math.Max(a, b));
        var result = new List<Vector3>(steps + 1);
        for (var i = 0; i <= steps; i++)
        {
            var u = u1 + span * i / steps;
            var local = new Vector3(a * MathF.Cos(u), b * MathF.Sin(u), 0);
            result.Add(placement.ToWorld(local));
        }
        return result;
    }

    static int ConicArcSampleCount(float span, int segments, float radius)
    {
        var fromCircumference = (int)MathF.Ceiling(MathF.Abs(span) / MathF.Tau * segments);
        var perQuadrant = Math.Max(2, segments / 4);
        var fromSpan = (int)MathF.Ceiling(MathF.Abs(span) / (MathF.PI / 2f) * perQuadrant);
        return Math.Clamp(Math.Max(fromCircumference, fromSpan), 2, 64);
    }

    static float NormalizeAngleSpan(float span, bool sense)
    {
        if (sense)
        {
            while (span < 0)
                span += MathF.Tau;
            if (span > MathF.PI)
                span -= MathF.Tau;
        }
        else
        {
            while (span > 0)
                span -= MathF.Tau;
            if (span < -MathF.PI)
                span += MathF.Tau;
        }
        return span;
    }

    static float ReadTrimParameter(MeshingContext ctx, IfcEntity trimmed, IfcEntity? basisCurve, IfcAttribute attribute)
    {
        var token = trimmed.GetValue(attribute.Index);
        if (!token.IsList)
            return 0f;
        foreach (var item in token.AsList(trimmed.Document))
        {
            if (item.IsNumber)
                return ConvertTrimValue(ctx, trimmed, basisCurve, (float)item.AsNumber());
            if (item.IsId)
            {
                var value = ReadTrimScalar(ctx, ctx.GetEntity(item.AsId()));
                if (value.HasValue)
                    return ConvertTrimValue(ctx, trimmed, basisCurve, value.Value);
            }
            if (item.IsEntity)
            {
                var (name, value) = item.AsSimpleEntity(trimmed.Document);
                if (value.IsNumber)
                    return ConvertTrimValue(ctx, trimmed, basisCurve, (float)value.AsNumber());
                if (name.Contains("PARAMETER", StringComparison.Ordinal) && value.IsNumber)
                    return ConvertTrimValue(ctx, trimmed, basisCurve, (float)value.AsNumber());
            }
        }
        return 0f;
    }

    static float? ReadTrimScalar(MeshingContext ctx, IfcEntity entity)
    {
        for (var i = 0; i < entity.Attributes.Count; i++)
        {
            var token = entity.GetValue(i);
            if (token.IsNumber)
                return (float)token.AsNumber();
        }
        return null;
    }

    static float ConvertTrimValue(MeshingContext ctx, IfcEntity trimmed, IfcEntity? basisCurve, float value)
    {
        var master = trimmed.GetString(IfcTrimmedCurve.Instance.MasterRepresentation.Index);
        if (!master.Contains("PARAMETER", StringComparison.Ordinal))
            return value;

        if (basisCurve?.GetEntityName() is "IFCCIRCLE" or "IFCELLIPSE")
            return value * MathF.PI / 180f;

        return value;
    }

    static bool TryReadTrimPoints2D(MeshingContext ctx, IfcEntity trimmed, out Vector2 p1, out Vector2 p2)
    {
        p1 = default;
        p2 = default;
        var trim1 = trimmed.GetArray(IfcTrimmedCurve.Instance.Trim1.Index);
        var trim2 = trimmed.GetArray(IfcTrimmedCurve.Instance.Trim2.Index);
        if (trim1.Count == 0 || trim2.Count == 0 || !trim1[0].IsId || !trim2[0].IsId)
            return false;
        p1 = Placements.ReadPoint2D(ctx, ctx.GetEntity(trim1[0].AsId()));
        p2 = Placements.ReadPoint2D(ctx, ctx.GetEntity(trim2[0].AsId()));
        return true;
    }

    static bool TryReadTrimPoints3D(MeshingContext ctx, IfcEntity trimmed, out Vector3 p1, out Vector3 p2)
    {
        p1 = default;
        p2 = default;
        var trim1 = trimmed.GetArray(IfcTrimmedCurve.Instance.Trim1.Index);
        var trim2 = trimmed.GetArray(IfcTrimmedCurve.Instance.Trim2.Index);
        if (trim1.Count == 0 || trim2.Count == 0 || !trim1[0].IsId || !trim2[0].IsId)
            return false;
        p1 = Placements.ReadPoint3D(ctx, ctx.GetEntity(trim1[0].AsId()));
        p2 = Placements.ReadPoint3D(ctx, ctx.GetEntity(trim2[0].AsId()));
        return true;
    }

    static int NearestIndex(IReadOnlyList<Vector2> points, Vector2 target)
    {
        var best = 0;
        var bestDist = float.MaxValue;
        for (var i = 0; i < points.Count; i++)
        {
            var d = points[i].DistanceSquared(target);
            if (d < bestDist)
            {
                bestDist = d;
                best = i;
            }
        }
        return best;
    }

    static int NearestIndex3D(IReadOnlyList<Vector3> points, Vector3 target)
    {
        var best = 0;
        var bestDist = float.MaxValue;
        for (var i = 0; i < points.Count; i++)
        {
            var d = points[i].DistanceSquared(target);
            if (d < bestDist)
            {
                bestDist = d;
                best = i;
            }
        }
        return best;
    }

    static List<Vector2> EvaluateCompositeCurve2D(MeshingContext ctx, IfcEntity composite)
    {
        var joinTolSq = CompositeJoinToleranceSquared(ctx);
        var result = new List<Vector2>();
        foreach (var segId in MeshHelpers.ReadIds(composite, IfcCompositeCurve.Instance.Segments))
        {
            var seg = ctx.GetEntity(segId);
            var segEntity = MeshHelpers.ResolveRequired(ctx, seg, IfcCompositeCurveSegment.Instance.ParentCurve);
            var sameSense = MeshHelpers.ReadOptionalBool(seg, IfcCompositeCurveSegment.Instance.SameSense, true);
            var transition = seg.GetString(IfcCompositeCurveSegment.Instance.Transition.Index);
            var pts = EvaluateCompositeSegment2D(ctx, segEntity, sameSense);
            if (result.Count > 0 && pts.Count > 0)
            {
                if (segEntity.GetEntityName() == "IFCTRIMMEDCURVE"
                    && TryGetTrimmedCircleCenter(ctx, segEntity, out var arcCenter))
                {
                    if (result[^1].DistanceSquared(arcCenter) <= joinTolSq * 4f)
                        result.RemoveAt(result.Count - 1);
                }

                var continuous = !transition.Contains("DISCONTINUOUS", StringComparison.Ordinal);
                if (continuous && result[^1].DistanceSquared(pts[0]) <= joinTolSq)
                    pts = pts.Skip(1).ToList();
            }
            result.AddRange(pts);
        }
        return SanitizeOpenPathPoints(result, joinTolSq);
    }

    static bool TryGetTrimmedCircleCenter(MeshingContext ctx, IfcEntity trimmed, out Vector2 center)
    {
        center = default;
        var basis = MeshHelpers.ResolveRequired(ctx, trimmed, IfcTrimmedCurve.Instance.BasisCurve);
        if (basis.GetEntityName() != "IFCCIRCLE")
            return false;
        var placement = Placements.ReadOptionalAxis2Placement3D(ctx, basis, IfcConic.Instance.Position);
        var w = placement.Origin;
        center = new Vector2(w.X, w.Y);
        return true;
    }

    static List<Vector2> EvaluateCompositeSegment2D(MeshingContext ctx, IfcEntity segEntity, bool sameSense)
    {
        var pts = segEntity.GetEntityName() == "IFCTRIMMEDCURVE"
            ? EvaluateTrimmedCurve2D(ctx, segEntity).ToList()
            : Evaluate2D(ctx, segEntity, dropClosure: true).ToList();
        if (!sameSense)
            pts.Reverse();
        return pts;
    }

    static List<Vector3> EvaluateCompositeCurve3D(MeshingContext ctx, IfcEntity composite)
    {
        var result = new List<Vector3>();
        foreach (var segId in MeshHelpers.ReadIds(composite, IfcCompositeCurve.Instance.Segments))
        {
            var seg = ctx.GetEntity(segId);
            var segEntity = MeshHelpers.ResolveRequired(ctx, seg, IfcCompositeCurveSegment.Instance.ParentCurve);
            var sameSense = MeshHelpers.ReadOptionalBool(seg, IfcCompositeCurveSegment.Instance.SameSense, true);
            var transition = seg.GetString(IfcCompositeCurveSegment.Instance.Transition.Index);
            var pts = Evaluate3D(ctx, segEntity).ToList();
            if (!sameSense)
                pts.Reverse();
            if (result.Count > 0 && pts.Count > 0)
            {
                var continuous = !transition.Contains("DISCONTINUOUS", StringComparison.Ordinal);
                if (continuous && result[^1].DistanceSquared(pts[0]) < 1e-8f)
                    pts = pts.Skip(1).ToList();
            }
            result.AddRange(pts);
        }
        return result;
    }

    static List<Vector2> EvaluateBSpline2D(MeshingContext ctx, IfcEntity bspline)
    {
        var points = MeshHelpers.ReadIds(bspline, IfcBSplineCurve.Instance.ControlPointsList)
            .Select(id => Placements.ReadPoint2D(ctx, ctx.GetEntity(id)))
            .ToList();
        return UniformSamplePolyline(points, Math.Max(points.Count * 2, 8));
    }

    static List<Vector3> EvaluateBSpline3D(MeshingContext ctx, IfcEntity bspline)
    {
        var points = MeshHelpers.ReadIds(bspline, IfcBSplineCurve.Instance.ControlPointsList)
            .Select(id => Placements.ReadPoint3D(ctx, ctx.GetEntity(id)))
            .ToList();
        return UniformSamplePolyline3D(points, Math.Max(points.Count * 2, 8));
    }

    static List<Vector3> UniformSamplePolyline3D(IReadOnlyList<Vector3> control, int samples)
    {
        if (control.Count <= 1)
            return control.ToList();
        var result = new List<Vector3>(samples);
        for (var i = 0; i < samples; i++)
        {
            var t = (float)i / (samples - 1) * (control.Count - 1);
            var idx = (int)t;
            var frac = t - idx;
            var a = control[Math.Min(idx, control.Count - 1)];
            var b = control[Math.Min(idx + 1, control.Count - 1)];
            result.Add(a + (b - a) * frac);
        }
        return result;
    }

    static List<Vector2> UniformSamplePolyline(IReadOnlyList<Vector2> control, int samples)
    {
        if (control.Count <= 1)
            return control.ToList();
        var result = new List<Vector2>(samples);
        for (var i = 0; i < samples; i++)
        {
            var t = (float)i / (samples - 1) * (control.Count - 1);
            var idx = (int)t;
            var frac = t - idx;
            var a = control[Math.Min(idx, control.Count - 1)];
            var b = control[Math.Min(idx + 1, control.Count - 1)];
            result.Add(a + (b - a) * frac);
        }
        return result;
    }

    static void AppendArc2D(List<Vector2> dest, Vector2 start, Vector2 mid, Vector2 end, int segments)
    {
        if (!TryArcCenter(start, mid, end, out var center, out var radius, out var a0, out var a1))
        {
            dest.Add(start);
            dest.Add(mid);
            dest.Add(end);
            return;
        }

        var step = (a1 - a0) / segments;
        for (var i = 0; i <= segments; i++)
        {
            var a = a0 + step * i;
            dest.Add(center + new Vector2(MathF.Cos(a), MathF.Sin(a)) * radius);
        }
    }

    static bool TryArcCenter(Vector2 p0, Vector2 p1, Vector2 p2, out Vector2 center, out float radius, out float a0, out float a1)
    {
        var d = 2 * (p0.X * (p1.Y - p2.Y) + p1.X * (p2.Y - p0.Y) + p2.X * (p0.Y - p1.Y));
        if (MathF.Abs(d) < 1e-12f)
        {
            center = default;
            radius = 0;
            a0 = a1 = 0;
            return false;
        }

        var p0sq = p0.LengthSquared();
        var p1sq = p1.LengthSquared();
        var p2sq = p2.LengthSquared();
        var ux = (p0sq * (p1.Y - p2.Y) + p1sq * (p2.Y - p0.Y) + p2sq * (p0.Y - p1.Y)) / d;
        var uy = (p0sq * (p2.X - p1.X) + p1sq * (p0.X - p2.X) + p2sq * (p1.X - p0.X)) / d;
        center = new Vector2(ux, uy);
        radius = center.Distance(p0);
        a0 = MathF.Atan2(p0.Y - center.Y, p0.X - center.X);
        var am = MathF.Atan2(p1.Y - center.Y, p1.X - center.X);
        a1 = MathF.Atan2(p2.Y - center.Y, p2.X - center.X);
        if ((am - a0) * (a1 - am) < 0)
            a1 += a1 < a0 ? MathF.Tau : -MathF.Tau;
        return true;
    }

    static Vector2 PlacementsReadWorld2(Frame3D frame, Vector2 local)
    {
        var w = frame.ToWorld(new Vector3(local.X, local.Y, 0));
        return new Vector2(w.X.Value, w.Y.Value);
    }

    static void RemoveDuplicateClosure(List<Vector2> points, float joinToleranceSquared = 0)
    {
        var cleaned = PolygonWithHoles.CleanRing(points, joinToleranceSquared);
        points.Clear();
        points.AddRange(cleaned);
    }
}

internal static class Vector3Extensions
{
    public static Vector2 Vector2(this Vector3 v) => new(v.X, v.Y);
}
