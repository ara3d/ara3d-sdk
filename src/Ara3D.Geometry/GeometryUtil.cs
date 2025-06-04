namespace Ara3D.Geometry
{
    public enum StandardPlane
    {
        Xy,
        Xz,
        Yz,
    }

    // TODO: many of these functions should live in other places, particular in the math 3D
    public static class GeometryUtil
    {
        public const float AngleTolerance = MathF.PI / 1000;

        public const float NumberTolerance = float.Epsilon * 10;

        public static IReadOnlyList<Vector3> Normalize(this IReadOnlyList<Vector3> vectors)
            => vectors.Map(v => v.Normalize);

        public static IReadOnlyList<Vector3> Rotate(this IReadOnlyList<Vector3> self, Vector3 axis, float angle)
            => self.Transform(Matrix4x4.CreateFromAxisAngle(axis, angle));

        public static IReadOnlyList<Vector3> Transform(this IReadOnlyList<Vector3> self, Matrix4x4 matrix)
            => self.Map(x => x.Transform(matrix));

        public static Integer3 Sort(this Integer3 v) =>
            v.A < v.B
                ? (v.B < v.C)
                    ? (v.A, v.B, v.C)
                    : (v.A < v.C)
                        ? (v.A, v.C, v.B)
                        : (v.C, v.A, v.B)
                : (v.A < v.C)
                    ? (v.B, v.A, v.C)
                    : (v.B < v.C)
                        ? (v.B, v.C, v.A)
                        : (v.C, v.B, v.A);

        // Fins the intersection between two lines.
        // Returns true if they intersect
        // References:
        // https://www.codeproject.com/Tips/862988/Find-the-Intersection-Point-of-Two-Line-Segments
        // https://gist.github.com/unitycoder/10241239e080720376830f84511ccd3c
        // https://en.m.wikipedia.org/wiki/Line%E2%80%93line_intersection#Given_two_points_on_each_line
        // https://stackoverflow.com/questions/4543506/algorithm-for-intersection-of-2-lines
        public static bool Intersection(this Line2D line1, Line2D line2, out Vector2 point, float epsilon = 0.000001f)
        {

            var x1 = line1.A.X;
            var y1 = line1.A.Y;
            var x2 = line1.B.X;
            var y2 = line1.B.Y;
            var x3 = line2.A.X;
            var y3 = line2.A.Y;
            var x4 = line2.B.X;
            var y4 = line2.B.Y;

            var denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            if (denominator.Abs < epsilon)
            {
                point = Vector2.Zero;
                return false;
            }

            var num1 = (x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4);
            var num2 = (x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3);
            var t1 = num1 / denominator;
            var t2 = -num2 / denominator;
            var p1 = line1.Lerp(t1);
            var p2 = line2.Lerp(t2);
            point = p1.Average(p2);

            return true;
        }

        // Returns the distance between two lines
        // t and u are the distances if the intersection points along the two lines 
        public static float LineLineDistance(Line2D line1, Line2D line2, out float t, out float u, float epsilon = 0.0000001f)
        {
            var x1 = line1.A.X;
            var y1 = line1.A.Y;
            var x2 = line1.B.X;
            var y2 = line1.B.Y;
            var x3 = line2.A.X;
            var y3 = line2.A.Y;
            var x4 = line2.B.X;
            var y4 = line2.B.Y;

            var denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            if (denominator.Abs >= epsilon)
            {
                // Lines are not parallel, they should intersect nicely
                var num1 = (x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4);
                var num2 = (x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3);

                t = num1 / denominator;
                u = -num2 / denominator;

                var e = 0.0;
                if (t >= -e && t <= 1.0 + e && u >= -e && u <= 1.0 + e)
                {
                    t = float.Clamp(t, 0.0f, 1.0f);
                    u = float.Clamp(t, 0.0f, 1.0f);
                    return 0;
                }
            }

            // Parallel or non intersecting lines - default to point to line checks

            u = 0.0f;
            var minDistance = Distance(line1, line2.A, out t);
            var distance = Distance(line1, line2.B, out var amount);
            if (distance < minDistance)
            {
                minDistance = distance;
                t = amount;
                u = 1.0f;
            }

            distance = Distance(line2, line1.A, out amount);
            if (distance < minDistance)
            {
                minDistance = distance;
                u = amount;
                t = 0.0f;
            }

            distance = Distance(line2, line1.B, out amount);
            if (distance < minDistance)
            {
                minDistance = distance;
                u = amount;
                t = 1.0f;
            }

            return minDistance;
        }

        // Returns the distance between a line and a point.
        // t is the distance along the line of the closest point
        public static float Distance(this Line2D line, Point2D p, out float t)
        {
            var (a, b) = line;

            // Return minimum distance between line segment vw and point p
            var l2 = (a - b).LengthSquared; // i.e. |w-v|^2 -  avoid a sqrt
            if (l2 == 0.0f) // v == w case
            {
                t = 0.5f;
                return (p - a).Length;
            }

            // Consider the line extending the segment, parameterized as v + t (w - v).
            // We find projection of point p onto the line. 
            // It falls where t = [(p-v) . (w-v)] / |w-v|^2
            // We clamp t from [0,1] to handle points outside the segment vw.
            t = ((p - a).Dot(b - a) / l2).Clamp(0.0f, 1.0f);
            var closestPoint = a + t * (b - a); // Projection falls on the segment
            return (p - closestPoint).Length;
        }
        
    }
}
