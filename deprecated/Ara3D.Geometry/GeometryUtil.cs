using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ara3D.Collections;
using Ara3D.Mathematics;

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
        public static IArray<Vector3> Normalize(this IArray<Vector3> vectors)
            => vectors.Select(v => v.Normalize());

        public static bool SequenceAlmostEquals(this IArray<Vector3> vs1, IArray<Vector3> vs2,
            float tolerance = Constants.Tolerance)
            => vs1.Count == vs2.Count && vs1.Indices().All(i => vs1[i].AlmostEquals(vs2[i], tolerance));

        public static bool AreColinear(this IEnumerable<Vector3> vectors, Vector3 reference,
            float tolerance = (float)Constants.OneTenthOfADegree)
            => !reference.IsNaN() && vectors.All(v => v.Colinear(reference, tolerance));

        public static bool AreColinear(this IEnumerable<Vector3> vectors,
            float tolerance = (float)Constants.OneTenthOfADegree)
            => vectors.ToList().AreColinear(tolerance);

        public static bool AreColinear(this System.Collections.Generic.IList<Vector3> vectors,
            float tolerance = (float)Constants.OneTenthOfADegree)
            => vectors.Count <= 1 || vectors.Skip(1).AreColinear(vectors[0], tolerance);

        public static AABox BoundingBox(this IArray<Vector3> vertices)
            => AABox.Create(vertices.ToEnumerable());

        public static IArray<float> Interpolate(this int count)
            => InterpolateExclusive(count);

        public static IArray<float> InterpolateInclusive(this int count)
            => count <= 0
                ? LinqArray.Empty<float>()
                : count == 1
                    ? LinqArray.Create(0f)
                    : count.Select(i => i / (float)(count - 1));

        public static IArray<float> InterpolateExclusive(this int count)
            => count <= 0
                ? LinqArray.Empty<float>()
                : count.Select(i => i / (float)count);

        public static IArray<Vector3> InterpolateInclusive(this int count, Func<float, Vector3> function)
            => count.InterpolateInclusive().Select(function);

        public static IArray<Vector3> Interpolate(this Line self, int count)
            => count.InterpolateInclusive(self.Lerp);

        public static IArray<Vector3> Rotate(this IArray<Vector3> self, Vector3 axis, float angle)
            => self.Transform(Matrix4x4.CreateFromAxisAngle(axis, angle));

        public static IArray<Vector3> Transform(this IArray<Vector3> self, Matrix4x4 matrix)
            => self.Select(x => x.Transform(matrix));

        public static Int3 Sort(this Int3 v)
        {
            if (v.X < v.Y)
            {
                return (v.Y < v.Z)
                    ? new Int3(v.X, v.Y, v.Z)
                    : (v.X < v.Z)
                        ? new Int3(v.X, v.Z, v.Y)
                        : new Int3(v.Z, v.X, v.Y);
            }
            else
            {
                return (v.X < v.Z)
                    ? new Int3(v.Y, v.X, v.Z)
                    : (v.Y < v.Z)
                        ? new Int3(v.Y, v.Z, v.X)
                        : new Int3(v.Z, v.Y, v.X);
            }
        }

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

            if (denominator.Abs() < epsilon)
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

            if (denominator.Abs() >= epsilon)
            {
                // Lines are not parallel, they should intersect nicely
                var num1 = (x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4);
                var num2 = (x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3);

                t = num1 / denominator;
                u = -num2 / denominator;

                var e = 0.0;
                if (t >= -e && t <= 1.0 + e && u >= -e && u <= 1.0 + e)
                {
                    t = t.Clamp(0.0f, 1.0f);
                    u = u.Clamp(0.0f, 1.0f);
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
        public static float Distance(this Line2D line, Vector2 p, out float t)
        {
            var (a, b) = line;

            // Return minimum distance between line segment vw and point p
            var l2 = (a - b).LengthSquared(); // i.e. |w-v|^2 -  avoid a sqrt
            if (l2 == 0.0f) // v == w case
            {
                t = 0.5f;
                return (p - a).Length();
            }

            // Consider the line extending the segment, parameterized as v + t (w - v).
            // We find projection of point p onto the line. 
            // It falls where t = [(p-v) . (w-v)] / |w-v|^2
            // We clamp t from [0,1] to handle points outside the segment vw.
            t = ((p - a).Dot(b - a) / l2).Clamp(0.0f, 1.0f);
            var closestPoint = a + t * (b - a); // Projection falls on the segment
            return (p - closestPoint).Length();
        }
        
        public static IArray<Vector2> Offset(this IArray<Vector2> points, float offset, bool closed)
        {
            if (points.Count < 2) return LinqArray.Empty<Vector2>();
            var lines = points.ToLines(closed).Select(line => line.ParallelOffset(offset));
            var r = new List<Vector2>();
            var n = lines.Count - (closed ? 0 : 1);

            if (!closed)
            {
                r.Add(lines[0].A);
            }

            for (var i = 0; i < n; ++i)
            {
                var line1 = lines[i];
                var line2 = lines.ElementAtModulo(i + 1);
                var intersects = line1.Intersection(line2, out var intersection);
                if (intersects)
                {
                    // They interesect
                    r.Add(intersection);
                }
                else
                {
                    // NOTE: this should be exceedingly rare or impossible.
                    // We probably have virtually coincident points, or maybe a bug in the line algorithm.
                    Debugger.Break();

                    // If we couldn't determine an intersection point 
                    // Add the end of the first line, and the beginning of the next
                    r.Add(line1.B);
                    r.Add(line2.A);
                }
            }

            if (!closed)
            {
                r.Add(lines.Last().B);
            }

            return r.ToIArray();
        }

        public static IPolyLine2D Offset(this IPolyLine2D self, float amount = 1f)
            => self.Points.Offset(amount, self.Closed).ToPolyLine2D(self.Closed);

        public static IArray<Line2D> ToLines(this IArray<Vector2> points, bool closed)
            => (points.Count - (closed ? 0 : 1)).Range()
                .Select(i => new Line2D(points[i], points.ElementAtModulo(i + 1)));
    }
}
