using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.IfcTypes;

namespace Ara3D.Ifc.Mesher.Approach1;

/// <summary>
/// Swept solid builders. Extrusion supports non-perpendicular direction (shear).
/// Revolve caps when angle &lt; 2π. Swept disk uses parallel-transport frames (approximate for twist).
/// </summary>
public static class SweptSolids
{
    /// <summary>
    /// Sweeps an open profile or curve along <see cref="IfcSurfaceOfLinearExtrusion.ExtrudedDirection"/>
    /// by <see cref="IfcSurfaceOfLinearExtrusion.Depth"/>. Meshed as a single-sided ribbon (no end caps).
    /// Arcs and B-splines are uniformly sampled.
    /// </summary>
    public static TriangleMesh3D BuildSurfaceOfLinearExtrusion(MeshingContext ctx, IfcEntity surface)
    {
        ctx.Diagnostics.RecordSupported("IFCSURFACEOFLINEAREXTRUSION");
        var sweptCurve = MeshHelpers.ResolveRequired(ctx, surface, IfcSweptSurface.Instance.SweptCurve);
        var frame = Placements.ReadOptionalAxis2Placement3D(ctx, surface, IfcSweptSurface.Instance.Position);
        var direction = Placements.ReadDirection3D(ctx,
            MeshHelpers.ResolveRequired(ctx, surface, IfcSurfaceOfLinearExtrusion.Instance.ExtrudedDirection),
            Vector3.UnitZ).Normalize;
        var depth = ctx.ScaleLength(MeshHelpers.ReadNumber(surface, IfcSurfaceOfLinearExtrusion.Instance.Depth));
        var extrusion = frame.ToWorldDirection(direction * depth);

        if (TryBuildOpenProfilePath(ctx, sweptCurve, out var openPath))
        {
            ctx.Diagnostics.RecordApproximate("IFCSURFACEOFLINEAREXTRUSION",
                "Open profile path extruded as ribbon surface; arcs uniformly sampled");
            return BuildExtrudedOpenPath(openPath, frame, extrusion);
        }

        if (IsCurveEntity(sweptCurve.GetEntityName()))
        {
            var path3D = CurveEvaluator.Evaluate3D(ctx, sweptCurve);
            if (path3D.Count >= 2)
            {
                ctx.Diagnostics.RecordApproximate("IFCSURFACEOFLINEAREXTRUSION",
                    "3D curve extruded as ribbon surface; arcs uniformly sampled");
                return BuildExtrudedOpenPath3D(path3D, frame, extrusion);
            }
        }

        throw new NotSupportedException(
            $"IFCSURFACEOFLINEAREXTRUSION #{surface.Id} SweptCurve #{sweptCurve.Id} is not an open path");
    }

    public static TriangleMesh3D BuildExtrudedAreaSolid(MeshingContext ctx, IfcEntity solid)
    {
        ctx.Diagnostics.RecordSupported("IFCEXTRUDEDAREASOLID");
        var profileEntity = MeshHelpers.ResolveRequired(ctx, solid, IfcExtrudedAreaSolid.Instance.SweptArea);
        var frame = Placements.ReadOptionalAxis2Placement3D(ctx, solid, IfcSweptAreaSolid.Instance.Position);
        var direction = Placements.ReadDirection3D(ctx,
            MeshHelpers.ResolveRequired(ctx, solid, IfcExtrudedAreaSolid.Instance.ExtrudedDirection),
            Vector3.UnitZ).Normalize;
        var depth = ctx.ScaleLength(MeshHelpers.ReadNumber(solid, IfcExtrudedAreaSolid.Instance.Depth));
        var extrusion = frame.ToWorldDirection(direction * depth);

        if (TryBuildOpenProfilePath(ctx, profileEntity, out var openPath))
        {
            ctx.Diagnostics.RecordApproximate("IFCEXTRUDEDAREASOLID", "Open profile extruded as ribbon surface");
            return BuildExtrudedOpenPath(openPath, frame, extrusion);
        }

        if (profileEntity.GetEntityName() == "IFCCOMPOSITEPROFILEDEF")
            return BuildExtrudedCompositeProfile(ctx, profileEntity, frame, extrusion);

        var profile = ProfileBuilder.Build(ctx, profileEntity);
        return MeshHelpers.BuildExtrusionWithHoles(profile, frame, extrusion);
    }

    static TriangleMesh3D BuildExtrudedCompositeProfile(
        MeshingContext ctx,
        IfcEntity profileEntity,
        Frame3D frame,
        Vector3 extrusion)
    {
        ctx.Diagnostics.RecordSupported("IFCCOMPOSITEPROFILEDEF");
        var profiles = MeshHelpers.ReadIds(profileEntity, IfcCompositeProfileDef.Instance.Profiles)
            .Select(id => ProfileBuilder.Build(ctx, ctx.GetEntity(id)))
            .ToList();
        if (profiles.Count == 0)
            throw new InvalidOperationException($"Empty composite profile #{profileEntity.Id}");

        if (profiles.Count == 1)
            return MeshHelpers.BuildExtrusionWithHoles(profiles[0], frame, extrusion);

        if (TryCombineCompositeAsHoles(profiles, out var combined))
            return MeshHelpers.BuildExtrusionWithHoles(combined, frame, extrusion);

        ctx.Diagnostics.RecordApproximate("IFCCOMPOSITEPROFILEDEF", "Disjoint sub-profiles extruded separately and merged");
        var meshes = profiles
            .Select(p => MeshHelpers.BuildExtrusionWithHoles(p, frame, extrusion))
            .ToList();
        return MeshHelpers.Merge(meshes);
    }

    static bool TryCombineCompositeAsHoles(IReadOnlyList<PolygonWithHoles> profiles, out PolygonWithHoles combined)
    {
        combined = default!;
        var outer = profiles[0];
        var holes = outer.Holes.ToList();
        foreach (var candidate in profiles.Skip(1))
        {
            if (!IsProfileContainedIn(candidate, outer))
                return false;
            holes.Add(candidate.Outer);
        }

        combined = new PolygonWithHoles(outer.Outer, holes);
        return true;
    }

    static bool IsProfileContainedIn(PolygonWithHoles inner, PolygonWithHoles outer)
    {
        foreach (var p in inner.Outer)
        {
            if (!PointInPolygon(p, outer.Outer))
                return false;
            foreach (var hole in outer.Holes)
            {
                if (PointInPolygon(p, hole))
                    return false;
            }
        }
        return true;
    }

    static bool PointInPolygon(Vector2 p, IReadOnlyList<Vector2> polygon)
    {
        var inside = false;
        for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
        {
            var pi = polygon[i];
            var pj = polygon[j];
            if ((pi.Y > p.Y) != (pj.Y > p.Y) &&
                p.X < (pj.X - pi.X) * (p.Y - pi.Y) / (pj.Y - pi.Y + 1e-12f) + pi.X)
                inside = !inside;
        }
        return inside;
    }

    /// <summary>Legacy attribute-driven extrusion: depth on each segment, not on the solid.</summary>
    public static TriangleMesh3D? BuildAttDrivenExtrudedSolid(MeshingContext ctx, IfcEntity solid)
    {
        ctx.Diagnostics.RecordSupported("IFCATTDRIVENEXTRUDEDSOLID");
        var meshes = MeshHelpers.ReadIdsByIndex(solid, 0)
            .Select(id => BuildAttDrivenExtrudedSegment(ctx, ctx.GetEntity(id)))
            .ToList();
        return meshes.Count == 0 ? null : MeshHelpers.Merge(meshes);
    }

    /// <summary>Attribute-driven extrusion with half-space clipping per segment batch.</summary>
    public static TriangleMesh3D? BuildAttDrivenClippedExtrudedSolid(MeshingContext ctx, IfcEntity solid)
    {
        ctx.Diagnostics.RecordApproximate("IFCATTDRIVENCLIPPEDEXTRUDEDSOLID", "Segment depth + half-space clip");
        var mesh = BuildAttDrivenExtrudedSolid(ctx, solid);
        if (mesh is null)
            return null;

        var result = mesh.Value;
        foreach (var clipId in MeshHelpers.ReadIdsByIndex(solid, 1))
        {
            var halfSpace = ctx.GetEntity(clipId);
            if (halfSpace.GetEntityName() == "IFCHALFSPACESOLID")
                result = Booleans.ClipByHalfSpace(ctx, result, halfSpace);
        }
        return result;
    }

    static TriangleMesh3D BuildAttDrivenExtrudedSegment(MeshingContext ctx, IfcEntity segment)
    {
        ctx.Diagnostics.RecordSupported("IFCATTDRIVENEXTRUDEDSEGMENT");
        var depth = ctx.ScaleLength(MeshHelpers.ReadNumberByIndex(segment, 2));
        var placement = MeshHelpers.ReadOptionalIdByIndex(segment, 3) is { } placementId
            ? ctx.GetEntity(placementId)
            : null;
        var profileId = MeshHelpers.ReadOptionalIdByIndex(segment, 4)
            ?? throw new InvalidOperationException($"AttDriven segment #{segment.Id} missing profile");
        var profileEntity = ctx.GetEntity(profileId);

        var frame = placement is null
            ? Placements.IdentityFrame
            : Placements.ReadAxis2Placement3D(ctx, placement);
        var extrusion = frame.ToWorldDirection(Vector3.UnitZ * depth);

        if (TryBuildOpenProfilePath(ctx, profileEntity, out var openPath))
            return BuildExtrudedOpenPath(openPath, frame, extrusion);

        var profile = ProfileBuilder.Build(ctx, profileEntity);
        return MeshHelpers.BuildExtrusionWithHoles(profile, frame, extrusion);
    }

    static bool TryBuildOpenProfilePath(MeshingContext ctx, IfcEntity profileEntity, out IReadOnlyList<Vector2> path)
    {
        if (ProfileBuilder.IsOpenProfileType(profileEntity.GetEntityName()))
        {
            path = ProfileBuilder.BuildOpen(ctx, profileEntity);
            return path.Count >= 2;
        }

        if (profileEntity.GetEntityName() == "IFCARBITRARYPROFILEDEF")
        {
            var curveId = MeshHelpers.ReadOptionalIdByIndex(profileEntity, 2);
            if (curveId is null)
            {
                path = [];
                return false;
            }
            var curve = ctx.GetEntity(curveId.Value);
            if (!ProfileBuilder.IsCurveClosed(ctx, curve))
            {
                path = CurveEvaluator.Evaluate2D(ctx, curve, dropClosure: false);
                return path.Count >= 2;
            }
        }

        path = [];
        return false;
    }

    /// <summary>Extrudes an open 2D polyline in the placement XY plane into a ribbon surface.</summary>
    static TriangleMesh3D BuildExtrudedOpenPath(IReadOnlyList<Vector2> path, Frame3D frame, Vector3 extrusion)
    {
        var bottomRow = path.Select(p => (Point3D)frame.ToWorld(new Vector3(p.X, p.Y, 0))).ToList();
        return BuildExtrudedRibbon(bottomRow, extrusion);
    }

    /// <summary>Extrudes an open 3D polyline (local to placement) into a ribbon surface.</summary>
    static TriangleMesh3D BuildExtrudedOpenPath3D(IReadOnlyList<Vector3> path, Frame3D frame, Vector3 extrusion)
    {
        var bottomRow = path.Select(p => (Point3D)frame.ToWorld(p)).ToList();
        return BuildExtrudedRibbon(bottomRow, extrusion);
    }

    /// <summary>Extrudes a polyline into a single-sided ribbon (surface, not solid; no end caps).</summary>
    static TriangleMesh3D BuildExtrudedRibbon(IReadOnlyList<Point3D> bottomRow, Vector3 extrusion)
    {
        if (bottomRow.Count < 2)
            return new TriangleMesh3D([], []);

        var points = new List<Point3D>(bottomRow.Count * 2);
        foreach (var bottom in bottomRow)
        {
            points.Add(bottom);
            points.Add(bottom + extrusion);
        }

        var faces = new List<Integer3>();
        for (var i = 0; i < bottomRow.Count - 1; i++)
        {
            var b0 = i * 2;
            var b1 = (i + 1) * 2;
            var t0 = b0 + 1;
            var t1 = b1 + 1;
            faces.Add(new Integer3(b0, b1, t1));
            faces.Add(new Integer3(b0, t1, t0));
        }
        return new TriangleMesh3D(points, faces);
    }

    static bool IsCurveEntity(string entityName)
        => entityName is "IFCPOLYLINE" or "IFCINDEXEDPOLYCURVE" or "IFCCIRCLE" or "IFCELLIPSE"
            or "IFCLINE" or "IFCTRIMMEDCURVE" or "IFCCOMPOSITECURVE" or "IFCBSPLINECURVE"
            or "IFCBSPLINECURVEWITHKNOTS";

    public static TriangleMesh3D BuildRevolvedAreaSolid(MeshingContext ctx, IfcEntity solid)
    {
        ctx.Diagnostics.RecordSupported("IFCREVOLVEDAREASOLID");
        var profile = ProfileBuilder.Build(ctx, MeshHelpers.ResolveRequired(ctx, solid, IfcRevolvedAreaSolid.Instance.SweptArea));
        var frame = Placements.ReadOptionalAxis2Placement3D(ctx, solid, IfcSweptAreaSolid.Instance.Position);
        var axisEntity = MeshHelpers.ResolveRequired(ctx, solid, IfcRevolvedAreaSolid.Instance.Axis);
        var axisPlacement = axisEntity.GetEntityName() == "IFCAXIS1PLACEMENT"
            ? Placements.IdentityFrame
            : Placements.ReadAxis2Placement3D(ctx, axisEntity);
        var axisPoint = axisEntity.GetEntityName() == "IFCAXIS1PLACEMENT"
            ? Placements.ReadPoint3D(ctx, MeshHelpers.ResolveRequired(ctx, axisEntity, IfcPlacement.Instance.Location))
            : axisPlacement.Origin.Vector3;
        var axisDir = axisEntity.GetEntityName() == "IFCAXIS1PLACEMENT"
            ? (MeshHelpers.ResolveOptional(ctx, axisEntity, IfcAxis1Placement.Instance.Axis) is { } ax
                ? Placements.ReadDirection3D(ctx, ax, Vector3.UnitZ)
                : Vector3.UnitZ)
            : axisPlacement.Z;
        var angle = (float)MeshHelpers.ReadNumber(solid, IfcRevolvedAreaSolid.Instance.Angle);
        if (angle <= 0)
            angle = MathF.Tau;

        var profile3D = profile.Outer.Select(p => (Point3D)frame.ToWorld(new Vector3(p.X, p.Y, 0))).ToList();
        var segments = Math.Max(8, (int)(angle / MathF.Tau * ctx.CircleSegments));
        var meshes = new List<TriangleMesh3D>();

        var origin = axisPoint;
        for (var i = 0; i < segments; i++)
        {
            var t0 = angle * i / segments;
            var t1 = angle * (i + 1) / segments;
            var row0 = RotatePoints(profile3D, origin, axisDir, t0);
            var row1 = RotatePoints(profile3D, origin, axisDir, t1);
            meshes.Add(BuildRevolveSegment(row0, row1));
        }

        if (angle < MathF.Tau - 1e-4f)
        {
            meshes.AddRange(BuildRevolveEndCaps(profile, frame, origin, axisDir, angle));
        }

        return MeshHelpers.Merge(meshes);
    }

    public static TriangleMesh3D BuildSweptDiskSolid(MeshingContext ctx, IfcEntity solid)
    {
        var entityName = solid.GetEntityName();
        ctx.Diagnostics.RecordApproximate(entityName, "Parallel-transport frames for curved paths; straight paths use annular caps");
        var directrix = MeshHelpers.ResolveRequired(ctx, solid, IfcSweptDiskSolid.Instance.Directrix);
        var path = CurveEvaluator.Evaluate3D(ctx, directrix);
        if (path.Count < 2)
            throw new InvalidOperationException("Directrix too short");

        var outerR = ctx.ScaleLength(MeshHelpers.ReadNumber(solid, IfcSweptDiskSolid.Instance.Radius));
        var innerR = ctx.ScaleLength(MeshHelpers.ReadNumber(solid, IfcSweptDiskSolid.Instance.InnerRadius));

        if (path.Count == 2 && (path[1] - path[0]).LengthSquared > 1e-12f)
            return BuildStraightSweptDisk(
                new Point3D(path[0].X, path[0].Y, path[0].Z),
                new Point3D(path[1].X, path[1].Y, path[1].Z),
                outerR, innerR, ctx.CircleSegments);

        var frames = BuildParallelTransportFrames(path);
        var circle = SampleCircleProfile(outerR, ctx.CircleSegments);

        var meshes = new List<TriangleMesh3D>();
        for (var i = 0; i < frames.Count - 1; i++)
        {
            var c0 = circle.Select(p => (Point3D)frames[i].ToWorld(new Vector3(p.X, p.Y, 0))).ToList();
            var c1 = circle.Select(p => (Point3D)frames[i + 1].ToWorld(new Vector3(p.X, p.Y, 0))).ToList();
            meshes.Add(BuildRevolveSegment(c0, c1));
        }

        if (innerR > 1e-6f)
            ctx.Diagnostics.RecordApproximate(entityName, "Inner radius tube not subtracted on curved directrix");

        return MeshHelpers.Merge(meshes);
    }

    static TriangleMesh3D BuildStraightSweptDisk(Point3D start, Point3D end, float radius, float innerRadius, int segments)
    {
        if (radius <= 0f)
            throw new InvalidOperationException("Swept disk radius must be positive.");
        if (innerRadius < 0f || innerRadius >= radius)
            throw new InvalidOperationException("Swept disk inner radius must be smaller than radius.");

        var tangent = (end.Vector3 - start.Vector3).Normalize;
        var (xAxis, yAxis) = CreatePerpendicularAxes(tangent);
        var points = new List<Point3D>();
        AddRing(points, start, xAxis, yAxis, radius, segments);
        AddRing(points, end, xAxis, yAxis, radius, segments);
        var faces = new List<Integer3>();

        AddTubeSideFaces(faces, 0, segments, segments, reverse: false);

        if (innerRadius > 0f)
        {
            var innerStart = points.Count;
            AddRing(points, start, xAxis, yAxis, innerRadius, segments);
            var innerEnd = points.Count;
            AddRing(points, end, xAxis, yAxis, innerRadius, segments);
            AddTubeSideFaces(faces, innerStart, innerEnd, segments, reverse: true);
            AddAnnularCapFaces(faces, 0, innerStart, segments, reverse: true);
            AddAnnularCapFaces(faces, segments, innerEnd, segments, reverse: false);
        }
        else
        {
            AddFanCapFaces(faces, 0, segments, reverse: true);
            AddFanCapFaces(faces, segments, segments, reverse: false);
        }

        return new TriangleMesh3D(points, faces);
    }

    static (Vector3 xAxis, Vector3 yAxis) CreatePerpendicularAxes(Vector3 tangent)
    {
        var z = tangent.Normalize;
        var x = MathF.Abs(Vector3.Dot(Vector3.UnitX, z)) < 0.9f ? Vector3.UnitX : Vector3.UnitY;
        var y = Vector3.Cross(z, x).Normalize;
        x = Vector3.Cross(y, z).Normalize;
        return (x, y);
    }

    static void AddRing(List<Point3D> points, Point3D center, Vector3 xAxis, Vector3 yAxis, float radius, int segments)
    {
        for (var i = 0; i < segments; i++)
        {
            var t = MathF.Tau * i / segments;
            points.Add(center + xAxis * (MathF.Cos(t) * radius) + yAxis * (MathF.Sin(t) * radius));
        }
    }

    static void AddTubeSideFaces(List<Integer3> faces, int startA, int startB, int segments, bool reverse)
    {
        for (var i = 0; i < segments; i++)
        {
            var j = (i + 1) % segments;
            var a = startA + i;
            var b = startA + j;
            var c = startB + j;
            var d = startB + i;
            if (reverse)
            {
                faces.Add(new Integer3(a, c, b));
                faces.Add(new Integer3(a, d, c));
            }
            else
            {
                faces.Add(new Integer3(a, b, c));
                faces.Add(new Integer3(a, c, d));
            }
        }
    }

    static void AddFanCapFaces(List<Integer3> faces, int center, int ringStart, bool reverse)
    {
        for (var i = 0; i < ringStart; i++)
        {
            var j = (i + 1) % ringStart;
            faces.Add(reverse
                ? new Integer3(center, ringStart + j, ringStart + i)
                : new Integer3(center, ringStart + i, ringStart + j));
        }
    }

    static void AddAnnularCapFaces(List<Integer3> faces, int outerStart, int innerStart, int segments, bool reverse)
    {
        for (var i = 0; i < segments; i++)
        {
            var j = (i + 1) % segments;
            var a = outerStart + i;
            var b = outerStart + j;
            var c = innerStart + j;
            var d = innerStart + i;
            if (reverse)
            {
                faces.Add(new Integer3(a, c, b));
                faces.Add(new Integer3(a, d, c));
            }
            else
            {
                faces.Add(new Integer3(a, b, c));
                faces.Add(new Integer3(a, c, d));
            }
        }
    }

    /// <summary>Basic directrix-frame sweep; fixed reference not fully implemented.</summary>
    public static TriangleMesh3D BuildSurfaceCurveSweptAreaSolid(MeshingContext ctx, IfcEntity solid)
    {
        ctx.Diagnostics.RecordApproximate("IFCSURFACECURVESWEPTAREASOLID", "Directrix frames only; surface ref ignored");
        var profile = ProfileBuilder.Build(ctx, MeshHelpers.ResolveRequired(ctx, solid, IfcSurfaceCurveSweptAreaSolid.Instance.SweptArea));
        var directrix = MeshHelpers.ResolveRequired(ctx, solid, IfcSurfaceCurveSweptAreaSolid.Instance.Directrix);
        var path = CurveEvaluator.Evaluate3D(ctx, directrix);
        var frames = BuildParallelTransportFrames(path);
        var profile2D = profile.Outer;

        var meshes = new List<TriangleMesh3D>();
        for (var i = 0; i < frames.Count - 1; i++)
        {
            var p0 = profile2D.Select(p => (Point3D)frames[i].ToWorld(new Vector3(p.X, p.Y, 0))).ToList();
            var p1 = profile2D.Select(p => (Point3D)frames[i + 1].ToWorld(new Vector3(p.X, p.Y, 0))).ToList();
            meshes.Add(BuildRevolveSegment(p0, p1));
        }
        return MeshHelpers.Merge(meshes);
    }

    public static TriangleMesh3D BuildFixedReferenceSweptAreaSolid(MeshingContext ctx, IfcEntity solid)
    {
        ctx.Diagnostics.RecordApproximate("IFCFIXEDREFERENCESWEPTAREASOLID", "Treated as surface curve swept with fixed Z reference");
        return BuildSurfaceCurveSweptAreaSolid(ctx, solid);
    }

    static List<Frame3D> BuildParallelTransportFrames(IReadOnlyList<Vector3> path)
    {
        var frames = new List<Frame3D>(path.Count);
        if (path.Count == 0)
            return frames;

        var z = path.Count > 1 ? (path[1] - path[0]).Normalize : Vector3.UnitZ;
        var x = MathF.Abs(Vector3.Dot(Vector3.UnitX, z)) < 0.9f ? Vector3.UnitX : Vector3.UnitY;
        var y = Vector3.Cross(z, x).Normalize;
        x = Vector3.Cross(y, z).Normalize;
        frames.Add(new Frame3D(new Point3D(path[0].X, path[0].Y, path[0].Z), new OrthonormalBasis3D(new Axes3D(x, y, z), true)));

        for (var i = 1; i < path.Count; i++)
        {
            var prev = frames[^1];
            var tangent = (path[i] - path[i - 1]).Normalize;
            if (tangent.LengthSquared() < 1e-12f)
            {
                frames.Add(prev.WithOrigin(new Point3D(path[i].X, path[i].Y, path[i].Z)));
                continue;
            }
            var newZ = tangent;
            var oldZ = prev.Z;
            var axis = Vector3.Cross(oldZ, newZ);
            if (axis.LengthSquared() < 1e-12f)
            {
                frames.Add(new Frame3D(new Point3D(path[i].X, path[i].Y, path[i].Z), prev.Basis));
            }
            else
            {
                var angle = MathF.Acos(Math.Clamp(Vector3.Dot(oldZ, newZ), -1f, 1f));
                var rot = Matrix4x4.CreateFromAxisAngle(axis.Normalize, angle);
                var newX = prev.X.Transform(rot);
                var newY = prev.Y.Transform(rot);
                frames.Add(new Frame3D(new Point3D(path[i].X, path[i].Y, path[i].Z), new OrthonormalBasis3D(new Axes3D(newX, newY, newZ), true)));
            }
        }
        return frames;
    }

    static List<Point3D> RotatePoints(IReadOnlyList<Point3D> points, Vector3 origin, Vector3 axis, float angle)
    {
        var rot = Matrix4x4.CreateFromAxisAngle(axis, angle);
        var t = Matrix4x4.CreateTranslation(-origin) * rot * Matrix4x4.CreateTranslation(origin);
        return points.Select(p => p.Transform(t)).ToList();
    }

    static IEnumerable<TriangleMesh3D> BuildRevolveEndCaps(
        PolygonWithHoles profile,
        Frame3D frame,
        Vector3 origin,
        Vector3 axisDir,
        float angle)
    {
        foreach (var rotation in new[] { 0f, angle })
        {
            foreach (var tri in profile.Triangulate())
            {
                var points = new[] { tri.A, tri.B, tri.C }
                    .Select(p => ProfilePointToWorld(frame, origin, axisDir, p, rotation))
                    .ToList();
                yield return new TriangleMesh3D(points, [new Integer3(0, 1, 2)]);
            }
        }
    }

    static Point3D ProfilePointToWorld(Frame3D frame, Vector3 origin, Vector3 axisDir, Point2D p, float rotation)
    {
        var pt = (Point3D)frame.ToWorld(new Vector3(p.X.Value, p.Y.Value, 0));
        return rotation == 0f ? pt : RotatePoints([pt], origin, axisDir, rotation)[0];
    }

    static TriangleMesh3D BuildRevolveSegment(IReadOnlyList<Point3D> row0, IReadOnlyList<Point3D> row1)
    {
        var points = row0.Concat(row1).ToList();
        var n = row0.Count;
        var faces = new List<Integer3>();
        for (var i = 0; i < n; i++)
        {
            var j = (i + 1) % n;
            faces.Add(new Integer3(i, j, j + n));
            faces.Add(new Integer3(i, j + n, i + n));
        }
        return new TriangleMesh3D(points, faces);
    }

    static List<Vector2> SampleCircleProfile(float radius, int segments)
        => Enumerable.Range(0, segments).Select(i =>
        {
            var t = MathF.Tau * i / segments;
            return new Vector2(MathF.Cos(t) * radius, MathF.Sin(t) * radius);
        }).ToList();
}
