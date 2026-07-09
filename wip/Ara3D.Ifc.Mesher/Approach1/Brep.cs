using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.IfcTypes;
using Ara3D.IO.StepParser;

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

    public static TriangleMesh3D BuildFaceBasedSurfaceElement(MeshingContext ctx, IfcEntity element)
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

            var map = ResolveSurfaceMap(ctx, face, outer);
            var outer2 = DedupeConsecutive(map.ProjectRing(outer));
            var holes2 = holes
                .Select(h => DedupeConsecutive(map.ProjectRing(h)))
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
                var a = GetIndex(map.Unproject(tri.A.Vector2));
                var b = GetIndex(map.Unproject(tri.B.Vector2));
                var c = GetIndex(map.Unproject(tri.C.Vector2));
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
            var surface = MeshHelpers.ResolveOptional(ctx, face, IfcFaceSurface.Instance.FaceSurface);
            if (surface?.GetEntityName() is "IFCPLANE" or "IFCCURVEBOUNDEDPLANE")
            {
                ctx.Diagnostics.RecordSupported(surface.GetEntityName());
                ctx.Diagnostics.RecordApproximate(name, "Planar advanced face via edge-loop bounds");
            }
            else if (surface?.GetEntityName() is "IFCCYLINDRICALSURFACE")
            {
                ctx.Diagnostics.RecordSupported(surface.GetEntityName());
                ctx.Diagnostics.RecordApproximate(name, "Cylindrical advanced face tessellated in surface parameter space");
            }
            else if (surface?.GetEntityName() is "IFCSURFACEOFREVOLUTION")
            {
                ctx.Diagnostics.RecordSupported(surface.GetEntityName());
                ctx.Diagnostics.RecordApproximate(name, "Surface-of-revolution advanced face tessellated in (angle, meridian) space");
            }
            else if (surface?.GetEntityName() is "IFCBSPLINESURFACEWITHKNOTS" or "IFCRATIONALBSPLINESURFACEWITHKNOTS" or "IFCBSPLINESURFACE")
            {
                ctx.Diagnostics.RecordSupported(surface.GetEntityName());
                ctx.Diagnostics.RecordApproximate(name, "B-spline advanced face tessellated over its (u,v) control-net parameter grid");
            }
            else
                ctx.Diagnostics.RecordApproximate(name, "Bounds only; curved advanced-face surfaces ignored");
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

        if (outer.Count < 3 && IsFaceSurfaceEntity(face))
            TryAppendCurveBoundedPlaneBounds(ctx, face, ref outer, holes);

        return (outer, holes, sameSense);
    }

    static bool IsFaceSurfaceEntity(IfcEntity face)
        => face.GetEntityName() is "IFCADVANCEDFACE" or "IFCFACESURFACE";

    static void TryAppendCurveBoundedPlaneBounds(
        MeshingContext ctx,
        IfcEntity face,
        ref List<Vector3> outer,
        List<List<Vector3>> holes)
    {
        var surface = MeshHelpers.ResolveOptional(ctx, face, IfcFaceSurface.Instance.FaceSurface);
        if (surface?.GetEntityName() != "IFCCURVEBOUNDEDPLANE")
            return;

        ctx.Diagnostics.RecordSupported("IFCCURVEBOUNDEDPLANE");
        var outerCurve = MeshHelpers.ResolveOptional(ctx, surface, IfcCurveBoundedPlane.Instance.OuterBoundary);
        if (outerCurve != null && outer.Count < 3)
            outer = EvaluateBoundaryCurve3D(ctx, outerCurve);

        foreach (var innerId in MeshHelpers.ReadIds(surface, IfcCurveBoundedPlane.Instance.InnerBoundaries))
            holes.Add(EvaluateBoundaryCurve3D(ctx, ctx.GetEntity(innerId)));
    }

    static List<Vector3> ReadLoop(MeshingContext ctx, IfcEntity loop)
        => loop.GetEntityName() switch
        {
            "IFCPOLYLOOP" => ReadPolyLoop(ctx, loop),
            "IFCEDGELOOP" => ReadEdgeLoop(ctx, loop),
            _ => throw new NotSupportedException($"Unsupported loop {loop.GetEntityName()}"),
        };

    static List<Vector3> ReadPolyLoop(MeshingContext ctx, IfcEntity loop)
    {
        var points = MeshHelpers.ReadIds(loop, IfcPolyLoop.Instance.Polygon)
            .Select(id => Placements.ReadPoint3D(ctx, ctx.GetEntity(id)))
            .ToList();
        return DedupeConsecutive3D(points);
    }

    static List<Vector3> ReadEdgeLoop(MeshingContext ctx, IfcEntity loop)
    {
        ctx.Diagnostics.RecordSupported("IFCEDGELOOP");
        var joinTolSq = CurveEvaluator.JoinToleranceSquaredFor(ctx);
        var result = new List<Vector3>();
        foreach (var edgeId in MeshHelpers.ReadIds(loop, IfcEdgeLoop.Instance.EdgeList))
        {
            var pts = EvaluateOrientedEdgePoints(ctx, ctx.GetEntity(edgeId));
            if (result.Count > 0 && pts.Count > 0)
            {
                if ((result[^1] - pts[0]).LengthSquared() <= joinTolSq)
                    pts = pts.Skip(1).ToList();
            }
            result.AddRange(pts);
        }
        return DedupeConsecutive3D(result);
    }

    static List<Vector3> EvaluateOrientedEdgePoints(MeshingContext ctx, IfcEntity orientedEdge)
    {
        var edgeEntity = MeshHelpers.ResolveRequired(ctx, orientedEdge, IfcOrientedEdge.Instance.EdgeElement);
        var orientation = MeshHelpers.ReadOptionalBool(orientedEdge, IfcOrientedEdge.Instance.Orientation, true);
        var pts = edgeEntity.GetEntityName() switch
        {
            "IFCEDGECURVE" => EvaluateEdgeCurvePoints(ctx, edgeEntity),
            "IFCORIENTEDEDGE" => EvaluateOrientedEdgePoints(ctx, edgeEntity),
            "IFCEDGE" => EvaluatePlainEdgePoints(ctx, edgeEntity),
            _ => throw new NotSupportedException($"Unsupported edge {edgeEntity.GetEntityName()}"),
        };
        if (!orientation)
            pts.Reverse();
        return pts;
    }

    static List<Vector3> EvaluateEdgeCurvePoints(MeshingContext ctx, IfcEntity edgeCurve)
    {
        ctx.Diagnostics.RecordSupported("IFCEDGECURVE");
        var curve = MeshHelpers.ResolveRequired(ctx, edgeCurve, IfcEdgeCurve.Instance.EdgeGeometry);
        var sameSense = MeshHelpers.ReadOptionalBool(edgeCurve, IfcEdgeCurve.Instance.SameSense, true);
        var pts = CurveEvaluator.Evaluate3D(ctx, curve).ToList();
        if (!sameSense)
            pts.Reverse();
        return pts;
    }

    static List<Vector3> EvaluatePlainEdgePoints(MeshingContext ctx, IfcEntity edge)
    {
        var pts = new List<Vector3>(2);
        var start = MeshHelpers.ResolveOptional(ctx, edge, IfcEdge.Instance.EdgeStart);
        var end = MeshHelpers.ResolveOptional(ctx, edge, IfcEdge.Instance.EdgeEnd);
        if (start != null)
            pts.Add(ReadVertexPoint(ctx, start));
        if (end != null)
            pts.Add(ReadVertexPoint(ctx, end));
        return pts;
    }

    static Vector3 ReadVertexPoint(MeshingContext ctx, IfcEntity vertex)
    {
        if (vertex.GetEntityName() != "IFCVERTEXPOINT")
            throw new NotSupportedException($"Expected IFCVERTEXPOINT, got {vertex.GetEntityName()}");
        var point = MeshHelpers.ResolveRequired(ctx, vertex, IfcVertexPoint.Instance.VertexGeometry);
        return Placements.ReadPoint3D(ctx, point);
    }

    static List<Vector3> EvaluateBoundaryCurve3D(MeshingContext ctx, IfcEntity curve)
        => DedupeConsecutive3D(CurveEvaluator.Evaluate3D(ctx, curve).ToList());

    static bool TryGetFacePlane(MeshingContext ctx, IfcEntity face, out FacePlane plane)
    {
        plane = default;
        var surface = MeshHelpers.ResolveOptional(ctx, face, IfcFaceSurface.Instance.FaceSurface);
        if (surface == null)
            return false;

        return surface.GetEntityName() switch
        {
            "IFCCURVEBOUNDEDPLANE" => TryGetCurveBoundedPlaneSurface(ctx, surface, out plane),
            _ => false,
        };
    }

    static bool TryGetPlaneSurface(MeshingContext ctx, IfcEntity planeEntity, out FacePlane plane)
    {
        var frame = Placements.ReadOptionalAxis2Placement3D(ctx, planeEntity, IfcElementarySurface.Instance.Position);
        plane = FrameToFacePlane(frame);
        return true;
    }

    static bool TryGetCurveBoundedPlaneSurface(MeshingContext ctx, IfcEntity bounded, out FacePlane plane)
    {
        var basis = MeshHelpers.ResolveRequired(ctx, bounded, IfcCurveBoundedPlane.Instance.BasisSurface);
        return TryGetPlaneSurface(ctx, basis, out plane);
    }

    static FacePlane FrameToFacePlane(Frame3D frame)
        => new(frame.Origin.Vector3, frame.Z, frame.X, frame.Y);

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

    /// <summary>
    /// Maps face-bound points between 3D world space and a 2D domain suitable for triangulation,
    /// and back. Planar faces use an in-plane projection; parametric surfaces (cylinder) project
    /// into surface parameter space so wrapping curved patches triangulate without self-overlap and
    /// unproject back onto the true curved surface.
    /// </summary>
    abstract class SurfaceMap
    {
        public abstract List<Vector2> ProjectRing(IReadOnlyList<Vector3> ring);
        public abstract Vector3 Unproject(Vector2 uv);
    }

    sealed class PlanarMap(FacePlane plane) : SurfaceMap
    {
        public override List<Vector2> ProjectRing(IReadOnlyList<Vector3> ring)
            => ring.Select(p => ProjectToPlane2D(p, plane)).ToList();

        public override Vector3 Unproject(Vector2 uv) => Brep.Unproject(uv, plane);
    }

    /// <summary>
    /// Cylindrical surface parameterization: 2D domain is (arc-length = radius*angle, height).
    /// Angles are unwrapped along each ring so a partial or full wrap forms a simple 2D polygon;
    /// holes align to the outer ring's angular band.
    /// </summary>
    sealed class CylinderMap(Frame3D frame, float radius) : SurfaceMap
    {
        float _refU;
        bool _hasRef;

        public override List<Vector2> ProjectRing(IReadOnlyList<Vector3> ring)
        {
            var result = new List<Vector2>(ring.Count);
            var prevU = 0f;
            var first = true;
            foreach (var p in ring)
            {
                var local = frame.ToLocal(p);
                var u = MathF.Atan2(local.Y.Value, local.X.Value);
                if (!first)
                    u += MathF.Round((prevU - u) / MathF.Tau) * MathF.Tau;
                else if (_hasRef)
                    u += MathF.Round((_refU - u) / MathF.Tau) * MathF.Tau;
                prevU = u;
                first = false;
                result.Add(new Vector2(radius * u, local.Z.Value));
            }
            if (!_hasRef && result.Count > 0)
            {
                _refU = result[0].X / radius;
                _hasRef = true;
            }
            return result;
        }

        public override Vector3 Unproject(Vector2 uv)
        {
            var u = uv.X / radius;
            return frame.ToWorld(new Vector3(radius * MathF.Cos(u), radius * MathF.Sin(u), uv.Y));
        }
    }

    static SurfaceMap ResolveSurfaceMap(MeshingContext ctx, IfcEntity face, IReadOnlyList<Vector3> outer)
    {
        if (IsFaceSurfaceEntity(face))
        {
            var surface = MeshHelpers.ResolveOptional(ctx, face, IfcFaceSurface.Instance.FaceSurface);
            var surfaceName = surface?.GetEntityName();
            if (surfaceName == "IFCCYLINDRICALSURFACE"
                && TryBuildCylinderMap(ctx, surface!, out var cyl))
                return cyl;
            if (surfaceName == "IFCSURFACEOFREVOLUTION"
                && TryBuildRevolutionMap(ctx, surface!, out var rev))
                return rev;
            if (surfaceName is "IFCBSPLINESURFACEWITHKNOTS" or "IFCRATIONALBSPLINESURFACEWITHKNOTS" or "IFCBSPLINESURFACE"
                && TryBuildBSplineSurfaceMap(ctx, surface!, out var bsp))
                return bsp;
            if (TryGetFacePlane(ctx, face, out var facePlane))
                return new PlanarMap(facePlane);
        }
        return new PlanarMap(ComputeNewellPlane(outer));
    }

    static bool TryBuildCylinderMap(MeshingContext ctx, IfcEntity surface, out CylinderMap map)
    {
        map = null!;
        var radius = (float)ctx.ScaleLength(MeshHelpers.ReadNumber(surface, IfcCylindricalSurface.Instance.Radius));
        if (radius <= 1e-7f)
            return false;
        var frame = Placements.ReadOptionalAxis2Placement3D(ctx, surface, IfcElementarySurface.Instance.Position);
        map = new CylinderMap(frame, radius);
        return true;
    }

    /// <summary>
    /// Surface-of-revolution parameterization. The 2D domain is (refRadius·angle, meridian arc-length):
    /// each boundary point is decomposed into its revolution angle about the axis and its position along
    /// the meridian (the swept profile expressed in the axis frame). Interior points triangulated in this
    /// domain unproject by evaluating the meridian at the given arc-length and rotating about the axis, so
    /// the tessellation follows the true revolved surface instead of a flat chord across it.
    /// </summary>
    sealed class RevolutionMap : SurfaceMap
    {
        readonly Frame3D _frame; // origin = axis point, Z = axis dir, X = meridian radial reference
        readonly float _refRadius;
        readonly float[] _s; // cumulative meridian arc-length
        readonly float[] _r; // meridian radial distance from axis
        readonly float[] _a; // meridian axial position along axis
        float _refAngle;
        bool _hasRef;

        RevolutionMap(Frame3D frame, float refRadius, float[] s, float[] r, float[] a)
            => (_frame, _refRadius, _s, _r, _a) = (frame, refRadius, s, r, a);

        public static RevolutionMap? Build(Vector3 axisPoint, Vector3 axisDir, IReadOnlyList<Vector3> meridian)
        {
            if (meridian.Count < 2)
                return null;
            axisDir = axisDir.Normalize;

            // Radial reference direction: from the axis to the meridian point farthest from it.
            var bestRadial = Vector3.Zero;
            var bestR = 0f;
            foreach (var m in meridian)
            {
                var rel = m - axisPoint;
                var axial = Vector3.Dot(rel, axisDir).Value;
                var radial = rel - axisDir * axial;
                var rr = radial.Length.Value;
                if (rr > bestR)
                {
                    bestR = rr;
                    bestRadial = radial;
                }
            }
            if (bestR < 1e-7f)
                return null;

            var x0 = bestRadial.Normalize;
            var y0 = Vector3.Cross(axisDir, x0).Normalize;
            var frame = new Frame3D(
                new Point3D(axisPoint.X, axisPoint.Y, axisPoint.Z),
                new OrthonormalBasis3D(new Axes3D(x0, y0, axisDir), true));

            var n = meridian.Count;
            var s = new float[n];
            var r = new float[n];
            var a = new float[n];
            for (var i = 0; i < n; i++)
            {
                var local = frame.ToLocal(meridian[i]);
                r[i] = MathF.Sqrt(local.X.Value * local.X.Value + local.Y.Value * local.Y.Value);
                a[i] = local.Z.Value;
                s[i] = i == 0 ? 0f : s[i - 1] + Hypot(r[i] - r[i - 1], a[i] - a[i - 1]);
            }
            if (s[n - 1] < 1e-9f)
                return null;

            return new RevolutionMap(frame, bestR, s, r, a);
        }

        public override List<Vector2> ProjectRing(IReadOnlyList<Vector3> ring)
        {
            var result = new List<Vector2>(ring.Count);
            var prevAngle = 0f;
            var first = true;
            foreach (var p in ring)
            {
                var local = _frame.ToLocal(p);
                var angle = MathF.Atan2(local.Y.Value, local.X.Value);
                if (!first)
                    angle += MathF.Round((prevAngle - angle) / MathF.Tau) * MathF.Tau;
                else if (_hasRef)
                    angle += MathF.Round((_refAngle - angle) / MathF.Tau) * MathF.Tau;
                prevAngle = angle;
                first = false;
                var radial = MathF.Sqrt(local.X.Value * local.X.Value + local.Y.Value * local.Y.Value);
                result.Add(new Vector2(_refRadius * angle, ArcLengthAt(radial, local.Z.Value)));
            }
            if (!_hasRef && result.Count > 0)
            {
                _refAngle = result[0].X / _refRadius;
                _hasRef = true;
            }
            return result;
        }

        public override Vector3 Unproject(Vector2 uv)
        {
            var angle = uv.X / _refRadius;
            var (radius, axial) = MeridianAt(uv.Y);
            return _frame.ToWorld(new Vector3(radius * MathF.Cos(angle), radius * MathF.Sin(angle), axial));
        }

        // Arc-length of the meridian point nearest to (r, a) in the meridian half-plane.
        float ArcLengthAt(float r, float a)
        {
            var bestS = _s[0];
            var bestDsq = float.MaxValue;
            for (var i = 0; i + 1 < _s.Length; i++)
            {
                var dr = _r[i + 1] - _r[i];
                var da = _a[i + 1] - _a[i];
                var lenSq = dr * dr + da * da;
                var t = lenSq < 1e-20f ? 0f : ((r - _r[i]) * dr + (a - _a[i]) * da) / lenSq;
                t = Math.Clamp(t, 0f, 1f);
                var pr = _r[i] + dr * t;
                var pa = _a[i] + da * t;
                var dsq = (r - pr) * (r - pr) + (a - pa) * (a - pa);
                if (dsq < bestDsq)
                {
                    bestDsq = dsq;
                    bestS = _s[i] + (_s[i + 1] - _s[i]) * t;
                }
            }
            return bestS;
        }

        (float Radius, float Axial) MeridianAt(float s)
        {
            var last = _s.Length - 1;
            if (s <= _s[0])
                return (_r[0], _a[0]);
            if (s >= _s[last])
                return (_r[last], _a[last]);
            for (var i = 0; i + 1 < _s.Length; i++)
            {
                if (s <= _s[i + 1])
                {
                    var seg = _s[i + 1] - _s[i];
                    var t = seg < 1e-20f ? 0f : (s - _s[i]) / seg;
                    return (_r[i] + (_r[i + 1] - _r[i]) * t, _a[i] + (_a[i + 1] - _a[i]) * t);
                }
            }
            return (_r[last], _a[last]);
        }

        static float Hypot(float x, float y) => MathF.Sqrt(x * x + y * y);
    }

    static bool TryBuildRevolutionMap(MeshingContext ctx, IfcEntity surface, out RevolutionMap map)
    {
        map = null!;
        try
        {
            var axisEntity = MeshHelpers.ResolveOptional(ctx, surface, IfcSurfaceOfRevolution.Instance.AxisPosition);
            if (axisEntity is null)
                return false;
            var position = Placements.ReadOptionalAxis2Placement3D(ctx, surface, IfcSweptSurface.Instance.Position);

            var axisLocal = Placements.ReadPoint3D(ctx, MeshHelpers.ResolveRequired(ctx, axisEntity, IfcPlacement.Instance.Location));
            var axisDirLocal = MeshHelpers.ResolveOptional(ctx, axisEntity, IfcAxis1Placement.Instance.Axis) is { } ax
                ? Placements.ReadDirection3D(ctx, ax, Vector3.UnitZ)
                : Vector3.UnitZ;

            var axisPoint = position.ToWorld(axisLocal);
            var axisDir = position.ToWorldDirection(axisDirLocal);

            var sweptCurve = MeshHelpers.ResolveRequired(ctx, surface, IfcSweptSurface.Instance.SweptCurve);
            var meridian = EvaluateMeridianCurve3D(ctx, sweptCurve)
                .Select(p => position.ToWorld(p))
                .ToList();

            var built = RevolutionMap.Build(axisPoint, axisDir, meridian);
            if (built is null)
                return false;
            map = built;
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>Evaluates a swept-surface profile (curve or arbitrary profile wrapping a curve) to 3D points.</summary>
    static List<Vector3> EvaluateMeridianCurve3D(MeshingContext ctx, IfcEntity sweptCurve)
    {
        var curve = sweptCurve.GetEntityName() switch
        {
            "IFCARBITRARYOPENPROFILEDEF" or "IFCCENTERLINEPROFILEDEF"
                => MeshHelpers.ResolveOptional(ctx, sweptCurve, IfcArbitraryOpenProfileDef.Instance.Curve),
            "IFCARBITRARYCLOSEDPROFILEDEF"
                => MeshHelpers.ResolveOptional(ctx, sweptCurve, IfcArbitraryClosedProfileDef.Instance.OuterCurve),
            _ => sweptCurve,
        };
        return curve is null ? [] : DedupeConsecutive3D(CurveEvaluator.Evaluate3D(ctx, curve).ToList());
    }

    /// <summary>
    /// B-spline (NURBS) surface parameterization. The 2D domain is the surface (u,v) parameter rectangle
    /// scaled to world-comparable units. Boundary points are located to (u,v) via nearest-point search on
    /// a pre-sampled surface grid; interior points unproject by evaluating the surface (Cox-de Boor tensor
    /// product), so wrapping curved panels tessellate on the true surface rather than a flat chord.
    /// </summary>
    sealed class BSplineSurfaceMap : SurfaceMap
    {
        readonly Vector3[][] _ctrl; // [uIndex][vIndex]
        readonly float[][] _w;
        readonly float[] _uKnots, _vKnots;
        readonly int _pu, _pv;
        readonly float _uMin, _uMax, _vMin, _vMax, _uScale, _vScale;
        readonly Vector3[] _grid3D;
        readonly Vector2[] _grid2D;

        BSplineSurfaceMap(Vector3[][] ctrl, float[][] w, float[] uKnots, float[] vKnots, int pu, int pv)
        {
            _ctrl = ctrl;
            _w = w;
            _uKnots = uKnots;
            _vKnots = vKnots;
            _pu = pu;
            _pv = pv;
            _uMin = uKnots[pu];
            _uMax = uKnots[ctrl.Length];
            _vMin = vKnots[pv];
            _vMax = vKnots[ctrl[0].Length];

            var uMid = 0.5f * (_uMin + _uMax);
            var vMid = 0.5f * (_vMin + _vMax);
            var uLen = (Eval(_uMax, vMid) - Eval(_uMin, vMid)).Length.Value;
            var vLen = (Eval(uMid, _vMax) - Eval(uMid, _vMin)).Length.Value;
            _uScale = _uMax > _uMin ? uLen / (_uMax - _uMin) : 1f;
            _vScale = _vMax > _vMin ? vLen / (_vMax - _vMin) : 1f;
            if (_uScale < 1e-6f) _uScale = 1f;
            if (_vScale < 1e-6f) _vScale = 1f;

            const int gridN = 32;
            _grid3D = new Vector3[(gridN + 1) * (gridN + 1)];
            _grid2D = new Vector2[(gridN + 1) * (gridN + 1)];
            var k = 0;
            for (var i = 0; i <= gridN; i++)
            {
                var u = _uMin + (_uMax - _uMin) * i / gridN;
                for (var j = 0; j <= gridN; j++)
                {
                    var v = _vMin + (_vMax - _vMin) * j / gridN;
                    _grid3D[k] = Eval(u, v);
                    _grid2D[k] = new Vector2((u - _uMin) * _uScale, (v - _vMin) * _vScale);
                    k++;
                }
            }
        }

        public static BSplineSurfaceMap? Build(MeshingContext ctx, IfcEntity surface)
        {
            var ctrl = ReadControlGrid(ctx, surface);
            if (ctrl.Length < 2 || ctrl[0].Length < 2)
                return null;
            var pu = (int)MeshHelpers.ReadNumber(surface, IfcBSplineSurface.Instance.UDegree);
            var pv = (int)MeshHelpers.ReadNumber(surface, IfcBSplineSurface.Instance.VDegree);
            if (pu < 1 || pv < 1)
                return null;

            var uKnots = ExpandKnots(
                MeshHelpers.ReadNumbers(surface, IfcBSplineSurfaceWithKnots.Instance.UKnots),
                MeshHelpers.ReadNumbers(surface, IfcBSplineSurfaceWithKnots.Instance.UMultiplicities));
            var vKnots = ExpandKnots(
                MeshHelpers.ReadNumbers(surface, IfcBSplineSurfaceWithKnots.Instance.VKnots),
                MeshHelpers.ReadNumbers(surface, IfcBSplineSurfaceWithKnots.Instance.VMultiplicities));
            if (uKnots.Length != ctrl.Length + pu + 1 || vKnots.Length != ctrl[0].Length + pv + 1)
                return null;

            var w = ReadWeightGrid(surface, ctrl);
            return new BSplineSurfaceMap(ctrl, w, uKnots, vKnots, pu, pv);
        }

        public override List<Vector2> ProjectRing(IReadOnlyList<Vector3> ring)
        {
            var result = new List<Vector2>(ring.Count);
            foreach (var p in ring)
            {
                var best = 0;
                var bestDsq = float.MaxValue;
                for (var i = 0; i < _grid3D.Length; i++)
                {
                    var dsq = (_grid3D[i] - p).LengthSquared.Value;
                    if (dsq < bestDsq)
                    {
                        bestDsq = dsq;
                        best = i;
                    }
                }
                result.Add(_grid2D[best]);
            }
            return result;
        }

        public override Vector3 Unproject(Vector2 uv)
        {
            var u = Math.Clamp(_uMin + uv.X / _uScale, _uMin, _uMax);
            var v = Math.Clamp(_vMin + uv.Y / _vScale, _vMin, _vMax);
            return Eval(u, v);
        }

        Vector3 Eval(float u, float v)
        {
            var nu = _ctrl.Length;
            var nv = _ctrl[0].Length;
            var su = FindSpan(nu - 1, _pu, u, _uKnots);
            var sv = FindSpan(nv - 1, _pv, v, _vKnots);
            var bu = BasisFuns(su, u, _pu, _uKnots);
            var bv = BasisFuns(sv, v, _pv, _vKnots);

            var num = Vector3.Zero;
            var den = 0f;
            for (var i = 0; i <= _pu; i++)
            {
                var ci = su - _pu + i;
                for (var j = 0; j <= _pv; j++)
                {
                    var cj = sv - _pv + j;
                    var b = bu[i] * bv[j] * _w[ci][cj];
                    num += _ctrl[ci][cj] * b;
                    den += b;
                }
            }
            return den < 1e-12f ? num : num / den;
        }

        static int FindSpan(int n, int p, float u, float[] knots)
        {
            if (u >= knots[n + 1]) return n;
            if (u <= knots[p]) return p;
            int low = p, high = n + 1, mid = (low + high) / 2;
            while (u < knots[mid] || u >= knots[mid + 1])
            {
                if (u < knots[mid]) high = mid;
                else low = mid;
                mid = (low + high) / 2;
            }
            return mid;
        }

        static float[] BasisFuns(int span, float u, int p, float[] knots)
        {
            var n = new float[p + 1];
            var left = new float[p + 1];
            var right = new float[p + 1];
            n[0] = 1f;
            for (var j = 1; j <= p; j++)
            {
                left[j] = u - knots[span + 1 - j];
                right[j] = knots[span + j] - u;
                var saved = 0f;
                for (var r = 0; r < j; r++)
                {
                    var denom = right[r + 1] + left[j - r];
                    var temp = denom == 0f ? 0f : n[r] / denom;
                    n[r] = saved + right[r + 1] * temp;
                    saved = left[j - r] * temp;
                }
                n[j] = saved;
            }
            return n;
        }

        static Vector3[][] ReadControlGrid(MeshingContext ctx, IfcEntity surface)
        {
            var token = surface.GetValue(IfcBSplineSurface.Instance.ControlPointsList.Index);
            if (!token.IsList)
                return [];
            var rows = new List<Vector3[]>();
            foreach (var rowTok in token.AsList(surface.Document))
            {
                if (!rowTok.IsList)
                    continue;
                var row = new List<Vector3>();
                foreach (var idTok in rowTok.AsList(surface.Document))
                    if (idTok.IsId)
                        row.Add(Placements.ReadPoint3D(ctx, ctx.GetEntity(idTok.AsId())));
                if (row.Count > 0)
                    rows.Add(row.ToArray());
            }
            return rows.ToArray();
        }

        static float[][] ReadWeightGrid(IfcEntity surface, Vector3[][] ctrl)
        {
            var w = ctrl.Select(row => Enumerable.Repeat(1f, row.Length).ToArray()).ToArray();
            if (surface.GetEntityName() != "IFCRATIONALBSPLINESURFACEWITHKNOTS")
                return w;
            var token = surface.GetValue(IfcRationalBSplineSurfaceWithKnots.Instance.WeightsData.Index);
            if (!token.IsList)
                return w;
            var i = 0;
            foreach (var rowTok in token.AsList(surface.Document))
            {
                if (!rowTok.IsList || i >= w.Length)
                    continue;
                var j = 0;
                foreach (var wTok in rowTok.AsList(surface.Document))
                {
                    if (wTok.IsNumber && j < w[i].Length)
                        w[i][j] = (float)wTok.AsNumber();
                    j++;
                }
                i++;
            }
            return w;
        }

        static float[] ExpandKnots(IReadOnlyList<double> knots, IReadOnlyList<double> mults)
        {
            var full = new List<float>();
            for (var i = 0; i < knots.Count && i < mults.Count; i++)
                for (var m = 0; m < (int)mults[i]; m++)
                    full.Add((float)knots[i]);
            return full.ToArray();
        }
    }

    static bool TryBuildBSplineSurfaceMap(MeshingContext ctx, IfcEntity surface, out BSplineSurfaceMap map)
    {
        map = null!;
        try
        {
            var built = BSplineSurfaceMap.Build(ctx, surface);
            if (built is null)
                return false;
            map = built;
            return true;
        }
        catch
        {
            return false;
        }
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
