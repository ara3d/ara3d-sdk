namespace Ara3D.Geometry;

public static class Intersection3D
{
    
    /// <summary>
    /// Returns true if the finite line segment intersects the triangle.
    /// If true, outputs the intersection point.
    /// </summary>
    public static bool Intersects(this Line3D line, Triangle3D triangle, out Point3D point, float tolerance = DefaultTolerance)
    {
        point = default;

        if (line.Length <= tolerance)
            return triangle.ContainsPoint(line.A, tolerance);

        var ray = line.Ray3D;
        if (!ray.Intersects(triangle, out var rayDistance, tolerance))
        {
            return false;
        }

        if (rayDistance < -tolerance || rayDistance > line.Length + tolerance)
            return false;

        point = ray.Origin + ray.Direction * rayDistance;
        return true;
    }

    /// <summary>
    /// Returns true if the finite line segment intersects the triangle.
    /// </summary>
    public static bool Intersects(
        this Line3D line,
        Triangle3D triangle,
        float tolerance = 1e-6f)
        => line.Intersects(triangle, out _, tolerance);


    /// <summary>
    /// Ray-triangle intersection.
    /// The ray is Origin + Direction * t, where Direction should be normalized.
    /// Returns the distance t along the ray.
    /// </summary>
    public static bool Intersects(this Ray3D ray, Triangle3D triangle, out float distance, float tolerance = DefaultTolerance)
    {
        distance = 0;

        var edge1 = triangle.B - triangle.A;
        var edge2 = triangle.C - triangle.A;

        var p = ray.Direction.Cross(edge2);
        var det = edge1.Dot(p);

        // Ray is parallel to triangle plane, or triangle is degenerate.
        if (MathF.Abs(det) <= tolerance)
            return false;

        var invDet = 1.0f / det;

        var t = ray.Origin - triangle.A;

        var u = t.Dot(p) * invDet;
        if (u < -tolerance || u > 1.0f + tolerance)
            return false;

        var q = t.Cross(edge1);
        var v = ray.Direction.Dot(q) * invDet;
        if (v < -tolerance || u + v > 1.0f + tolerance)
            return false;

        distance = edge2.Dot(q) * invDet;

        // For a ray, distance must be non-negative.
        if (distance < -tolerance)
            return false;

        return true;
    }

    /// <summary>
    /// Checks whether a point lies inside or on the boundary of the triangle.
    /// The point is expected to be close to the triangle plane.
    /// </summary>
    public static bool ContainsPoint(
        this Triangle3D triangle,
        Point3D point,
        float tolerance = 1e-6f)
    {
        var normal = triangle.Normal;

        if (normal.LengthSquared() <= tolerance * tolerance)
            return false;

        var distanceToPlane = Vector3.Dot(point - triangle.A, normal);

        if (MathF.Abs(distanceToPlane) > tolerance)
            return false;

        var bary = triangle.Barycentric(point);

        return bary.X >= -tolerance
               && bary.Y >= -tolerance
               && bary.Z >= -tolerance
               && bary.X <= 1.0f + tolerance
               && bary.Y <= 1.0f + tolerance
               && bary.Z <= 1.0f + tolerance;
    }

    /// <summary>
    /// Returns barycentric coordinates relative to triangle A/B/C.
    /// X corresponds to A, Y to B, Z to C.
    /// </summary>
    public static Vector3 Barycentric(
        this Triangle3D triangle,
        Point3D point)
    {
        var v0 = triangle.B - triangle.A;
        var v1 = triangle.C - triangle.A;
        var v2 = point - triangle.A;

        var d00 = v0.Dot(v0);
        var d01 = v0.Dot(v1);
        var d11 = v1.Dot(v1);
        var d20 = v2.Dot(v0);
        var d21 = v2.Dot(v1);
        var denominator = d00 * d11 - d01 * d01;

        if (denominator.Abs() <= 1e-12f)
            return new Vector3(float.NaN, float.NaN, float.NaN);

        var v = (d11 * d20 - d01 * d21) / denominator;
        var w = (d00 * d21 - d01 * d20) / denominator;
        var u = 1.0f - v - w;

        return new Vector3(u, v, w);
    }

    private const float DefaultTolerance = 1e-5f;

    //===
    // Oriented Bounding Box intersections
     
    public static bool TryIntersectAabb(
        this OrientedBox3D a,
        OrientedBox3D b,
        out Bounds3D result,
        float tolerance = DefaultTolerance)
    {
        var points = GetIntersectionPoints(a, b, tolerance);

        if (points.Count == 0)
        {
            result = default;
            return false;
        }

        result = points.Bounds();
        return true;
    }

    public static bool TryIntersectBoxInFrame(
        this OrientedBox3D a,
        OrientedBox3D b,
        Frame3D frame,
        out OrientedBox3D result,
        float tolerance = DefaultTolerance)
    {
        var points = GetIntersectionPoints(a, b, tolerance);

        if (points.Count == 0)
        {
            result = default;
            return false;
        }

        var localBounds = points.Map(frame.ToLocal).Bounds();
        var worldCenter = frame.ToWorld(localBounds.Center);

        result = new OrientedBox3D(
            frame.WithOrigin(worldCenter),
            localBounds.Size);

        return true;
    }

    public static bool TryIntersectBoxInFrameOfA(
        this OrientedBox3D a,
        OrientedBox3D b,
        out OrientedBox3D result,
        float tolerance = DefaultTolerance)
        => TryIntersectBoxInFrame(a, b, a.Frame, out result, tolerance);

    public static bool TryIntersectBoxInFrameOfB(
        this OrientedBox3D a,
        OrientedBox3D b,
        out OrientedBox3D result,
        float tolerance = DefaultTolerance)
        => TryIntersectBoxInFrame(a, b, b.Frame, out result, tolerance);

    public static IReadOnlyList<Point3D> GetIntersectionPoints(
        OrientedBox3D a,
        OrientedBox3D b,
        float tolerance = DefaultTolerance)
    {
        var result = new List<Point3D>();

        var cornersA = a.GetCorners();
        var cornersB = b.GetCorners();

        // 1. Vertices of A inside B.
        foreach (var p in cornersA)
            if (b.Contains(p, tolerance))
                result.Add(p);

        // 2. Vertices of B inside A.
        foreach (var p in cornersB)
            if (a.Contains(p, tolerance))
                result.Add(p);

        // 3. Edges of A clipped against B.
        foreach (var line in a.GetLines())
            if (TryClipSegmentToBox(line, b, out var clipped, tolerance))
                result.AddRange(clipped.Points);

        // 4. Edges of B clipped against A.
        foreach (var line in a.GetLines())
            if (TryClipSegmentToBox(line, b, out var clipped, tolerance))
                result.AddRange(clipped.Points);

        return result;
    }

    public static bool Contains(
        this OrientedBox3D box,
        Vector3 worldPoint,
        float tolerance = DefaultTolerance)
    {
        var p = box.Frame.ToLocal(worldPoint);
        var h = box.Size * 0.5f;

        return p.X >= -h.X - tolerance && p.X <= h.X + tolerance
            && p.Y >= -h.Y - tolerance && p.Y <= h.Y + tolerance
            && p.Z >= -h.Z - tolerance && p.Z <= h.Z + tolerance;
    }

    public static Point3D[] GetCorners(this OrientedBox3D box)
    {
        var h = box.Size * 0.5f;

        return
        [
            box.Frame.ToWorld(new Vector3(-h.X, -h.Y, -h.Z)),
            box.Frame.ToWorld(new Vector3(+h.X, -h.Y, -h.Z)),
            box.Frame.ToWorld(new Vector3(+h.X, +h.Y, -h.Z)),
            box.Frame.ToWorld(new Vector3(-h.X, +h.Y, -h.Z)),

            box.Frame.ToWorld(new Vector3(-h.X, -h.Y, +h.Z)),
            box.Frame.ToWorld(new Vector3(+h.X, -h.Y, +h.Z)),
            box.Frame.ToWorld(new Vector3(+h.X, +h.Y, +h.Z)),
            box.Frame.ToWorld(new Vector3(-h.X, +h.Y, +h.Z)),
        ];
    }

    public static bool TryClipSegmentToBox(
        this Line3D world,
        OrientedBox3D box,
        out Line3D clipped,
        float tolerance = DefaultTolerance)
    {
        var local = world.Transform(box.Frame.Matrix);

        var h = box.Size * 0.5f;

        if (!TryClipSegmentToCenteredAabb(local, h, out var t0, out var t1, tolerance))
        {
            clipped = default;
            return false;
        }

        var localClippedA = local.A.Lerp(local.B, t0);
        var localClippedB = local.A.Lerp(local.B, t1);

        clipped = (
            box.Frame.ToWorld(localClippedA),
            box.Frame.ToWorld(localClippedB));

        return true;
    }

    public static bool TryClipSegmentToCenteredAabb(
        Line3D line,
        Vector3 halfSize,
        out float t0,
        out float t1,
        float tolerance = DefaultTolerance)
    {
        t0 = 0f;
        t1 = 1f;

        var a = line.A;
        var d = line.Direction;

        return ClipAxis(a.X, d.X, -halfSize.X, halfSize.X, ref t0, ref t1, tolerance)
            && ClipAxis(a.Y, d.Y, -halfSize.Y, halfSize.Y, ref t0, ref t1, tolerance)
            && ClipAxis(a.Z, d.Z, -halfSize.Z, halfSize.Z, ref t0, ref t1, tolerance);
    }

    private static bool ClipAxis(
        float origin,
        float direction,
        float min,
        float max,
        ref float t0,
        ref float t1,
        float tolerance)
    {
        min -= tolerance;
        max += tolerance;

        if (MathF.Abs(direction) <= tolerance)
            return origin >= min && origin <= max;

        var inv = 1f / direction;
        var ta = (min - origin) * inv;
        var tb = (max - origin) * inv;

        if (ta > tb)
            (ta, tb) = (tb, ta);

        if (ta > t0) t0 = ta;
        if (tb < t1) t1 = tb;

        return t0 <= t1 + tolerance;
    }
    
    public static Integer GetNumLines(this OrientedBox3D self)
        => BoxEdges.Length;

    public static IReadOnlyList<Line3D> GetLines(this OrientedBox3D self)
    {
        var corners = self.GetCorners();
        return BoxEdges.Map(e => new Line3D(corners[e.A], corners[e.B]));
    }

    public static readonly Integer2[] BoxEdges =
    [
        // Bottom face
        (0, 1), (1, 2), (2, 3), (3, 0),

        // Top face
        (4, 5), (5, 6), (6, 7), (7, 4),

        // Vertical edges
        (0, 4), (1, 5), (2, 6), (3, 7),
    ];

    /// <summary>
    /// Returns true if the ray intersects the axis-aligned bounds.
    /// </summary>
    public static bool Intersects(this Ray3D ray, Bounds3D bounds)
        => Intersects(ray, bounds, out _);

    /// <summary>
    /// Returns true if the ray intersects the axis-aligned bounds.
    /// tMin and tMax are the entry and exit distances along the ray.
    /// </summary>
    public static bool Intersects(this Ray3D ray, Bounds3D bounds, out NumberInterval hitInterval)
    {
        hitInterval = (0f, float.PositiveInfinity);
        return IntersectAxis(ray.Origin.X, ray.Direction.X, bounds.IntervalX(), ref hitInterval)
               && IntersectAxis(ray.Origin.Y, ray.Direction.Y, bounds.IntervalY(), ref hitInterval)
               && IntersectAxis(ray.Origin.Z, ray.Direction.Z, bounds.IntervalZ(), ref hitInterval);
    }

    public static bool IntersectAxis(float origin, float direction, NumberInterval bounds, ref NumberInterval hitInterval)
    {
        const float epsilon = 1e-8f;

        if (direction.Abs() < epsilon)
            return bounds.Contains(origin);

        var t0 = (bounds.Start - origin) * direction.Reciprocal();
        var t1 = (bounds.End - origin) * direction.Reciprocal();

        if (t0 > t1)
            (t0, t1) = (t1, t0);

        hitInterval = (MathF.Max(hitInterval.Start, t0), MathF.Min(hitInterval.End, t1));

        return hitInterval.Start <= hitInterval.End;
    }

    /// <summary>
    /// Returns true if two 3D triangles intersect.
    /// Handles:
    /// - crossing triangles
    /// - shared vertices/edges
    /// - containment
    /// - coplanar overlap
    /// 
    /// This is designed as a robust practical implementation, not an exact arithmetic predicate.
    /// </summary>
    public static bool Intersects(this Triangle3D a, Triangle3D b, float tolerance = DefaultTolerance)
        => IntersectsTriangleTriangle(a.A, a.B, a.C, b.A, b.B, b.C, tolerance);
    

    private static bool IntersectsTriangleTriangle(
        Vector3 a0,
        Vector3 a1,
        Vector3 a2,
        Vector3 b0,
        Vector3 b1,
        Vector3 b2,
        float eps)
    {
        if (IsDegenerate((a0, a1, a2), eps) || IsDegenerate((b0, b1, b2), eps))
            return false;

        var na = Vector3.Cross(a1 - a0, a2 - a0);
        var nb = Vector3.Cross(b1 - b0, b2 - b0);

        var da0 = SignedDistanceToPlane(b0, a0, na);
        var da1 = SignedDistanceToPlane(b1, a0, na);
        var da2 = SignedDistanceToPlane(b2, a0, na);

        // Triangle B is completely on one side of triangle A's plane.
        if (SameStrictSign(da0, da1, da2, eps))
            return false;

        var db0 = SignedDistanceToPlane(a0, b0, nb);
        var db1 = SignedDistanceToPlane(a1, b0, nb);
        var db2 = SignedDistanceToPlane(a2, b0, nb);

        // Triangle A is completely on one side of triangle B's plane.
        if (SameStrictSign(db0, db1, db2, eps))
            return false;

        var coplanar =
            MathF.Abs(da0) <= eps &&
            MathF.Abs(da1) <= eps &&
            MathF.Abs(da2) <= eps &&
            MathF.Abs(db0) <= eps &&
            MathF.Abs(db1) <= eps &&
            MathF.Abs(db2) <= eps;

        if (coplanar)
            return IntersectsCoplanar(a0, a1, a2, b0, b1, b2, na, eps);

        // Non-coplanar case.
        // If any edge of A crosses triangle B, they intersect.
        if (IntersectsSegmentTriangle(a0, a1, b0, b1, b2, eps)) return true;
        if (IntersectsSegmentTriangle(a1, a2, b0, b1, b2, eps)) return true;
        if (IntersectsSegmentTriangle(a2, a0, b0, b1, b2, eps)) return true;

        // If any edge of B crosses triangle A, they intersect.
        if (IntersectsSegmentTriangle(b0, b1, a0, a1, a2, eps)) return true;
        if (IntersectsSegmentTriangle(b1, b2, a0, a1, a2, eps)) return true;
        if (IntersectsSegmentTriangle(b2, b0, a0, a1, a2, eps)) return true;

        // Handles cases where a vertex lies exactly on the other triangle.
        if (PointInTriangle3D(a0, b0, b1, b2, eps)) return true;
        if (PointInTriangle3D(a1, b0, b1, b2, eps)) return true;
        if (PointInTriangle3D(a2, b0, b1, b2, eps)) return true;

        if (PointInTriangle3D(b0, a0, a1, a2, eps)) return true;
        if (PointInTriangle3D(b1, a0, a1, a2, eps)) return true;
        if (PointInTriangle3D(b2, a0, a1, a2, eps)) return true;

        return false;
    }


    private static bool IntersectsSegmentTriangle(
        Vector3 p0,
        Vector3 p1,
        Vector3 t0,
        Vector3 t1,
        Vector3 t2,
        float eps)
    {
        // Möller-Trumbore ray/triangle intersection, clamped to segment [0, 1].
        var dir = p1 - p0;
        var edge1 = t1 - t0;
        var edge2 = t2 - t0;

        var h = Vector3.Cross(dir, edge2);
        var det = Vector3.Dot(edge1, h);

        if (MathF.Abs(det) <= eps)
        {
            // Segment is parallel to triangle plane.
            // Coplanar cases are handled separately.
            return false;
        }

        var invDet = 1.0f / det;
        var s = p0 - t0;

        var u = invDet * Vector3.Dot(s, h);
        if (u < -eps || u > 1.0f + eps)
            return false;

        var q = Vector3.Cross(s, edge1);

        var v = invDet * Vector3.Dot(dir, q);
        if (v < -eps || u + v > 1.0f + eps)
            return false;

        var distanceAlongSegment = invDet * Vector3.Dot(edge2, q);

        return distanceAlongSegment >= -eps &&
               distanceAlongSegment <= 1.0f + eps;
    }

    private static bool PointInTriangle3D(
        Vector3 p,
        Vector3 a,
        Vector3 b,
        Vector3 c,
        float eps)
    {
        var n = Vector3.Cross(b - a, c - a);

        if (n.LengthSquared() <= eps * eps)
            return false;

        var dist = SignedDistanceToPlane(p, a, n);

        if (MathF.Abs(dist) > eps)
            return false;

        return PointInTriangleUsingBarycentric(p, a, b, c, eps);
    }

    private static bool PointInTriangleUsingBarycentric(
        Vector3 p,
        Vector3 a,
        Vector3 b,
        Vector3 c,
        float eps)
    {
        var v0 = b - a;
        var v1 = c - a;
        var v2 = p - a;

        var d00 = Vector3.Dot(v0, v0);
        var d01 = Vector3.Dot(v0, v1);
        var d11 = Vector3.Dot(v1, v1);
        var d20 = Vector3.Dot(v2, v0);
        var d21 = Vector3.Dot(v2, v1);

        var denom = d00 * d11 - d01 * d01;

        if (MathF.Abs(denom) <= eps)
            return false;

        var v = (d11 * d20 - d01 * d21) / denom;
        var w = (d00 * d21 - d01 * d20) / denom;
        var u = 1.0f - v - w;

        return u >= -eps && v >= -eps && w >= -eps;
    }

    private static bool IntersectsCoplanar(
        Vector3 a0,
        Vector3 a1,
        Vector3 a2,
        Vector3 b0,
        Vector3 b1,
        Vector3 b2,
        Vector3 normal,
        float eps)
    {
        var axis = normal.GetPrimaryAxisIndex();

        var aa0 = Project(a0, axis);
        var aa1 = Project(a1, axis);
        var aa2 = Project(a2, axis);

        var bb0 = Project(b0, axis);
        var bb1 = Project(b1, axis);
        var bb2 = Project(b2, axis);

        // Edge-edge overlap in projected 2D.
        if (SegmentsIntersect2D(aa0, aa1, bb0, bb1, eps)) return true;
        if (SegmentsIntersect2D(aa0, aa1, bb1, bb2, eps)) return true;
        if (SegmentsIntersect2D(aa0, aa1, bb2, bb0, eps)) return true;

        if (SegmentsIntersect2D(aa1, aa2, bb0, bb1, eps)) return true;
        if (SegmentsIntersect2D(aa1, aa2, bb1, bb2, eps)) return true;
        if (SegmentsIntersect2D(aa1, aa2, bb2, bb0, eps)) return true;

        if (SegmentsIntersect2D(aa2, aa0, bb0, bb1, eps)) return true;
        if (SegmentsIntersect2D(aa2, aa0, bb1, bb2, eps)) return true;
        if (SegmentsIntersect2D(aa2, aa0, bb2, bb0, eps)) return true;

        // Full containment with no edge crossings.
        if (PointInTriangle2D(aa0, bb0, bb1, bb2, eps)) return true;
        if (PointInTriangle2D(bb0, aa0, aa1, aa2, eps)) return true;

        return false;
    }

    private static bool SegmentsIntersect2D(
        Vector2 a,
        Vector2 b,
        Vector2 c,
        Vector2 d,
        float eps)
    {
        var o1 = Orient2D(a, b, c);
        var o2 = Orient2D(a, b, d);
        var o3 = Orient2D(c, d, a);
        var o4 = Orient2D(c, d, b);

        if (MathF.Abs(o1) <= eps && PointOnSegment2D(c, a, b, eps)) return true;
        if (MathF.Abs(o2) <= eps && PointOnSegment2D(d, a, b, eps)) return true;
        if (MathF.Abs(o3) <= eps && PointOnSegment2D(a, c, d, eps)) return true;
        if (MathF.Abs(o4) <= eps && PointOnSegment2D(b, c, d, eps)) return true;

        return (o1 > eps && o2 < -eps || o1 < -eps && o2 > eps) &&
               (o3 > eps && o4 < -eps || o3 < -eps && o4 > eps);
    }

    private static bool PointInTriangle2D(
        Vector2 p,
        Vector2 a,
        Vector2 b,
        Vector2 c,
        float eps)
    {
        var o1 = Orient2D(a, b, p);
        var o2 = Orient2D(b, c, p);
        var o3 = Orient2D(c, a, p);

        var hasNegative = o1 < -eps || o2 < -eps || o3 < -eps;
        var hasPositive = o1 > eps || o2 > eps || o3 > eps;

        return !(hasNegative && hasPositive);
    }

    private static bool PointOnSegment2D(
        Vector2 p,
        Vector2 a,
        Vector2 b,
        float eps)
    {
        return p.X >= MathF.Min(a.X, b.X) - eps &&
               p.X <= MathF.Max(a.X, b.X) + eps &&
               p.Y >= MathF.Min(a.Y, b.Y) - eps &&
               p.Y <= MathF.Max(a.Y, b.Y) + eps;
    }

    private static float Orient2D(Vector2 a, Vector2 b, Vector2 c)
        => (b.X - a.X) * (c.Y - a.Y) -
           (b.Y - a.Y) * (c.X - a.X);

    private static Vector2 Project(this Vector3 p, int droppedAxis)
    {
        // Drop the dominant normal axis to get the most stable 2D projection.
        return droppedAxis switch
        {
            0 => new Vector2(p.Y, p.Z), // drop X
            1 => new Vector2(p.X, p.Z), // drop Y
            _ => new Vector2(p.X, p.Y), // drop Z
        };
    }

    private static bool SameStrictSign(float a, float b, float c, float eps)
        => a > eps && b > eps && c > eps || a < -eps && b < -eps && c < -eps;

    private static float SignedDistanceToPlane(
        Vector3 p,
        Vector3 planePoint,
        Vector3 planeNormal)
    {
        // Not normalized; only sign matters for most tests.
        // For tolerance comparison, normalize scale by normal length.
        var len = planeNormal.Length();

        if (len == 0)
            return 0;

        return Vector3.Dot(p - planePoint, planeNormal) / len;
    }

    public static Vector3 AB(this Triangle3D t)
        => t.B - t.A;

    public static Vector3 AC(this Triangle3D t)
        => t.C - t.A;

    public static bool IsDegenerate(this Triangle3D t, float eps)
        => t.AB().Cross(t.AC()).LengthSquared() <= eps * eps;

    public static float OverlapVolume(this Bounds3D a, Bounds3D b)
    {
        var dx = MathF.Max(0, MathF.Min(a.Max.X, b.Max.X) - MathF.Max(a.Min.X, b.Min.X));
        var dy = MathF.Max(0, MathF.Min(a.Max.Y, b.Max.Y) - MathF.Max(a.Min.Y, b.Min.Y));
        var dz = MathF.Max(0, MathF.Min(a.Max.Z, b.Max.Z) - MathF.Max(a.Min.Z, b.Min.Z));

        return dx * dy * dz;
    }
}