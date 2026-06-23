using System.Diagnostics;
using Ara3D.Utils;

namespace Ara3D.Geometry
{
    public enum StandardPlane
    {
        Xy,
        Xz,
        Yz,
    }

    public partial struct Vector3
    {
        public Vector3 NormalizedCross(Vector3 other)
            => Vector3.Cross(this, other).Normalize;
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
        public static float LineLineDistance(Line2D line1, Line2D line2, out float t, out float u,
            float epsilon = 0.0000001f)
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

        public static Number Unlerp(this Number value, Number min, Number max)
        {
            var d = max - min;
            if (d.AlmostZero)
                return 0.5f; // If min and max are equal, return 0.5 to avoid division by zero.
            return (value - min) / (max - min);
        }

        public static Color Lerp(this Color c0, Color c1, float amount)
            => (
                c0.R.Lerp(c1.R, amount),
                c0.G.Lerp(c1.G, amount),
                c0.B.Lerp(c1.B, amount),
                c0.A.Lerp(c1.A, amount));

        public static Vector3 InverseLerp(this Vector3 v, Vector3 min, Vector3 max)
            => (v.X.Unlerp(min.X, max.X),
                v.Y.Unlerp(min.Y, max.Y),
                v.Z.Unlerp(min.Z, max.Z));

        public static Vector3 InverseLerp(this Point3D point, Bounds3D bounds)
            => InverseLerp(point, bounds.Min, bounds.Max);

        public static Vector3 InverseLerp(this Point3D point, Line3D line)
            => InverseLerp(point, line.A, line.B);

        public static Point3D CenterBottom(this Bounds3D self) => self.Center.WithZ(self.Min.Z);
        public static Point3D CenterTop(this Bounds3D self) => self.Center.WithZ(self.Max.Z);

        public static Vector3 Along(this Vector3 v, int axisIndex)
            => axisIndex switch
            {
                0 => v.AlongX(),
                1 => v.AlongY(),
                2 => v.AlongZ(),
                _ => throw new ArgumentOutOfRangeException(nameof(axisIndex))
            };

        public static Vector3 AlongX(this Vector3 v) => (v.X, 0, 0);
        public static Vector3 AlongY(this Vector3 v) => (0, v.Y, 0);
        public static Vector3 AlongZ(this Vector3 v) => (0, 0, v.Z);

        public static Vector3 SizeAlongX(this Bounds3D bounds) => bounds.Size.AlongX();
        public static Vector3 SizeAlongY(this Bounds3D bounds) => bounds.Size.AlongY();
        public static Vector3 SizeAlongZ(this Bounds3D bounds) => bounds.Size.AlongZ();

        public static Vector3 HalfSize(this Bounds3D bounds) => bounds.Size.Half;
        public static Vector3 HalfSizeAlong(this Bounds3D bounds, int axisIndex) => bounds.HalfSize().Along(axisIndex);
        public static Vector3 HalfSizeAlongX(this Bounds3D bounds) => bounds.HalfSize().AlongX();
        public static Vector3 HalfSizeAlongY(this Bounds3D bounds) => bounds.HalfSize().AlongY();
        public static Vector3 HalfSizeAlongZ(this Bounds3D bounds) => bounds.HalfSize().AlongZ();

        public static Line3D CenterLine(this Bounds3D bounds, int axisIndex) =>
            bounds.Center.ToCenteredLine(bounds.HalfSizeAlong(axisIndex));

        public static Line3D CenterLineX(this Bounds3D bounds) => bounds.Center.ToCenteredLine(bounds.HalfSizeAlongX());
        public static Line3D CenterLineY(this Bounds3D bounds) => bounds.Center.ToCenteredLine(bounds.HalfSizeAlongY());
        public static Line3D CenterLineZ(this Bounds3D bounds) => bounds.Center.ToCenteredLine(bounds.HalfSizeAlongZ());

        public static Vector3 DiagonalVector(this Bounds3D bounds) => (bounds.Max - bounds.Min);
        public static Vector3 HalfDiagonalVector(this Bounds3D bounds) => (bounds.Max - bounds.Center);
        public static Number DiagonalLength(this Bounds3D bounds) => bounds.DiagonalVector().Length;
        public static Number Volume(this Bounds3D bounds) => bounds.Size.X * bounds.Size.Y * bounds.Size.Z;

        public static Integer3 GetOrderedAxisIndicesDescending(this Vector3 v)
        {
            var (x, y, z) = v.Abs();
            if (x >= y && x >= z) return y >= z ? (0, 1, 2) : (0, 2, 1);
            if (y >= x && y >= z) return x >= z ? (1, 0, 2) : (1, 2, 0);
            return x >= y ? (2, 0, 1) : (2, 1, 0);
        }

        public static int GetPrimaryAxisIndex(this Vector3 v) => v.GetOrderedAxisIndicesDescending().A;
        public static int GetSecondaryAxisIndex(this Vector3 v) => v.GetOrderedAxisIndicesDescending().B;
        public static int GetTertiaryAxisIndex(this Vector3 v) => v.GetOrderedAxisIndicesDescending().C;

        public static Vector3 GetLongestAxis(this Vector3 v) => v.Along(v.GetLongestAxisIndex());
        public static Vector3 GetMiddleAxis(this Vector3 v) => v.Along(v.GetMiddleAxisIndex());
        public static Vector3 GetShortestAxis(this Vector3 v) => v.Along(v.GetShortestAxisIndex());

        public static Vector3[] GetOrderedAxisDescending(this Vector3 v) =>
            [v.GetLongestAxis(), v.GetMiddleAxis(), v.GetShortestAxis()];

        public static int GetLongestAxisIndex(this Vector3 v) => v.GetPrimaryAxisIndex();
        public static int GetMiddleAxisIndex(this Vector3 v) => v.GetSecondaryAxisIndex();
        public static int GetShortestAxisIndex(this Vector3 v) => v.GetTertiaryAxisIndex();

        public static float GetPrimaryComponent(this Vector3 v) => v.GetComponent(v.GetPrimaryAxisIndex());
        public static float GetSecondaryComponent(this Vector3 v) => v.GetComponent(v.GetSecondaryAxisIndex());
        public static float GetTertiaryComponent(this Vector3 v) => v.GetComponent(v.GetTertiaryAxisIndex());

        public static float GetLongestComponent(this Vector3 v) => v.GetPrimaryComponent();
        public static float GetMiddleComponent(this Vector3 v) => v.GetSecondaryComponent();
        public static float GetShortestComponent(this Vector3 v) => v.GetTertiaryComponent();

        public static float GetPrimaryMagnitude(this Vector3 v) => MathF.Abs(v.GetPrimaryComponent());
        public static float GetSecondaryMagnitude(this Vector3 v) => MathF.Abs(v.GetSecondaryComponent());
        public static float GetTertiaryMagnitude(this Vector3 v) => MathF.Abs(v.GetTertiaryComponent());

        public static int PrimaryAxisIndex(this Bounds3D bounds) => bounds.Size.GetPrimaryAxisIndex();
        public static int SecondaryAxisIndex(this Bounds3D bounds) => bounds.Size.GetSecondaryAxisIndex();
        public static int TertiaryAxisIndex(this Bounds3D bounds) => bounds.Size.GetTertiaryAxisIndex();

        public static int LongestAxisIndex(this Bounds3D bounds) => bounds.PrimaryAxisIndex();
        public static int MiddleAxisIndex(this Bounds3D bounds) => bounds.SecondaryAxisIndex();
        public static int ShortestAxisIndex(this Bounds3D bounds) => bounds.TertiaryAxisIndex();

        public static Line3D CenterLinePrimary(this Bounds3D bounds) => bounds.CenterLine(bounds.PrimaryAxisIndex());

        public static Line3D CenterLineSecondary(this Bounds3D bounds) =>
            bounds.CenterLine(bounds.SecondaryAxisIndex());

        public static Line3D CenterLineTertiary(this Bounds3D bounds) => bounds.CenterLine(bounds.TertiaryAxisIndex());

        public static Line3D CenterLineLongest(this Bounds3D bounds) => bounds.CenterLinePrimary();
        public static Line3D CenterLineMiddle(this Bounds3D bounds) => bounds.CenterLineSecondary();
        public static Line3D CenterLineShortest(this Bounds3D bounds) => bounds.CenterLineTertiary();

        public static float PrimarySize(this Bounds3D bounds) => bounds.Size.GetPrimaryComponent();
        public static float SecondarySize(this Bounds3D bounds) => bounds.Size.GetSecondaryComponent();
        public static float TertiarySize(this Bounds3D bounds) => bounds.Size.GetTertiaryComponent();

        public static float LongestSize(this Bounds3D bounds) => bounds.PrimarySize();
        public static float MiddleSize(this Bounds3D bounds) => bounds.SecondarySize();
        public static float ShortestSize(this Bounds3D bounds) => bounds.TertiarySize();

        public static Cylinder ToCylinder(this Bounds3D bounds) =>
            new(bounds.CenterLinePrimary(), bounds.MiddleSize().Half());

        public static Matrix4x4 ToMatrix(this Bounds3D bounds) =>
            bounds.Center.ToTranslationMatrix() * bounds.Size.ToScaleMatrix();

        public static (float, float) GetOtherComponents(this Vector3 v, int axisIndex)
            => axisIndex switch
            {
                0 => (v.Y, v.Z),
                1 => (v.X, v.Z),
                2 => (v.X, v.Y),
                _ => throw new ArgumentOutOfRangeException(nameof(axisIndex))
            };

        public static float GetComponent(this Vector3 v, int axisIndex)
            => axisIndex switch
            {
                0 => v.X,
                1 => v.Y,
                2 => v.Z,
                _ => throw new ArgumentOutOfRangeException(nameof(axisIndex))
            };

        public static Number DistanceFromZAxis(this Bounds3D bounds, Vector3 p)
        {
            var c = bounds.Center;
            var x = p.X - c.X;
            var y = p.Y - c.Y;
            return MathF.Sqrt(x * x + y * y);
        }

        public static Number InverseLerp(this Number self, Number a, Number b)
            => self.Unlerp(a, b);

        public static Line3D CenterXAxis(this Bounds3D bounds)
        {
            var c = bounds.Center;
            return ((bounds.Min.X, c.Y, c.Z), (bounds.Max.X, c.Y, c.Z));
        }

        public static Point3D Lerp(this Bounds3D bounds, Vector3 v)
            => (bounds.Min.X.Lerp(bounds.Max.X, v.X),
                bounds.Min.Y.Lerp(bounds.Max.Y, v.Y),
                bounds.Min.Z.Lerp(bounds.Max.Z, v.Z));

        public static Vector3 WithComponent(this Vector3 self, Integer component, Number value)
        {
            if (component == 0) return self.WithX(value);
            if (component == 1) return self.WithY(value);
            if (component == 2) return self.WithZ(value);
            throw new IndexOutOfRangeException();
        }

        public static Vector3 AxisVector(this int i)
            => AxisVector((Integer)i);

        public static Vector3 AxisVector(this Integer i)
            => Vector3.Zero.WithComponent(i, 1);

        public static IReadOnlyList<Point3D> GetPoints(this IReadOnlyList<Triangle3D> self)
        {
            var points = new List<Point3D>(self.Count * 3);
            foreach (var triangle in self)
                points.AddRange(triangle.Points);
            return points;
        }

        public static IReadOnlyList<Integer3> GetFaces(this IReadOnlyList<Triangle3D> self)
        {
            var faces = new List<Integer3>(self.Count);
            for (var i = 0; i < self.Count; i++)
                faces.Add((i * 3, i * 3 + 1, i * 3 + 2));
            return faces;
        }

        public static TriangleMesh3D ToTriangleMesh3D(this IReadOnlyList<Triangle3D> self)
            => new(self.GetPoints(), self.GetFaces());

        public static IReadOnlyList<Integer> CornerIndices(this LineMesh3D self)
            => self.FaceIndices.FlatMap(x => (x.A, x.B));

        public static IReadOnlyList<Integer> CornerIndices(this TriangleMesh3D self)
            => self.FaceIndices.FlatMap(x => (x.A, x.B, x.C));

        public static IReadOnlyList<Integer> CornerIndices(this QuadMesh3D self)
            => self.FaceIndices.FlatMap(x => (x.A, x.B, x.C, x.D));

        public static Quad3D Inset(this Quad3D q, float amount)
            => (q.A.Lerp(q.Center, amount), q.B.Lerp(q.Center, amount),
                q.C.Lerp(q.Center, amount), q.D.Lerp(q.Center, amount));

        public static Vector3 Bilinear(this Quad3D q, Vector2 uv)
        {
            var ab = q.A.Lerp(q.B, uv.X);
            var dc = q.D.Lerp(q.C, uv.X);
            return ab.Lerp(dc, uv.Y);
        }

        public static Quad3D RemapQuad(this Quad3D q, Vector2 a, Vector2 b, Vector2 c, Vector2 d)
            => (Bilinear(q, a), Bilinear(q, b), Bilinear(q, c), Bilinear(q, d));

        public static Quad3D InsetQuad(this Quad3D q, float x0, float x1, float y0, float y1)
            => RemapQuad(q, (x0, y0), (1f - x1, y0), (1f - x1, 1f - y1), (x0, 1f - y1));

        public static Quad3D InsetAbs(this Quad3D q, float inset)
            => InsetAbs(q, inset, inset, inset, inset);

        public static float AbsToRel(this Vector3 a, Vector3 b, float f)
            => f / (b - a).Length;

        public static float AbsToRel(this Point3D a, Point3D b, float f)
            => f / (b - a).Length;

        public static Quad3D InsetAbs(this Quad3D q, float x0Abs, float x1Abs, float y0Abs, float y1Abs)
        {
            var ab = AbsToRel(q.A, q.B, x0Abs);
            var ba = AbsToRel(q.B, q.A, x1Abs);

            var cd = AbsToRel(q.C, q.D, x0Abs);
            var dc = AbsToRel(q.D, q.C, x1Abs);

            var ad = AbsToRel(q.A, q.D, y0Abs);
            var da = AbsToRel(q.D, q.A, y1Abs);

            var bc = AbsToRel(q.B, q.C, y0Abs);
            var cb = AbsToRel(q.C, q.B, y1Abs);

            var x0 = (ab + dc) / 2f;
            var x1 = (ba + cd) / 2f;
            var y0 = (ad + bc) / 2f;
            var y1 = (da + cb) / 2f;

            return q.InsetQuad(x0, x1, y0, y1);
        }

        public static Quad3D GetQuad(this IReadOnlyList<Point3D> points, Integer4 face)
            => (points[face.A], points[face.B], points[face.C], points[face.D]);

        public static Triangle3D GetTriangle(this IReadOnlyList<Point3D> points, Integer3 face)
            => (points[face.A], points[face.B], points[face.C]);

        public static Quad3D Push(this Quad3D q, float distance)
            => q.Translate(q.Normal * distance);

        public static QuadGrid3D Subdivide(this Quad3D q, int xSegments, int ySegments)
        {
            var leftSidePoints = q.A.Sample(q.D, ySegments + 1);
            var rightSidePoints = q.B.Sample(q.C, ySegments + 1);
            var rows = (ySegments + 1).MapRange(i => leftSidePoints[i].Sample(rightSidePoints[i], xSegments + 1));
            return rows.RowsToArray().ToQuadGrid3D(false, false);
        }

        public static QuadMesh3D ToQuadMesh3D(this IReadOnlyList<Quad3D> quads)
            => new(quads.SelectMany(q => q.Points).ToList(),
                quads.Count.MapRange(i => new Integer4(i * 4, i * 4 + 1, i * 4 + 2, i * 4 + 3)));

        public static Quad3D UnitXZQuad = ((0, 0, 0), (1, 0, 0), (1, 0, 1), (0, 0, 1));

        public static Quad3D XZQuad(float width, float height) => UnitXZQuad.Scale((width, 1, height));

        public static float GetBottomLength(this Quad3D q)
            => (q.B - q.A).Length;

        public static float GetHeight(this Quad3D q)
            => ((q.D - q.A).Length + (q.C - q.B).Length) / 2;


        /// <summary>
        /// Returns a quaternion that rotates the model's Z axis (0,0,1) to align with the given direction.
        /// </summary>
        public static Quaternion AlignZAxisWith(this Vector3 targetZ)
        {
            targetZ = targetZ.Normalize;
            var currentZ = Vector3.UnitZ;

            if (currentZ.Distance(targetZ) < 1e-6f)
                return Quaternion.Identity;

            if (currentZ.Distance(-targetZ) < 1e-6f)
            {
                // 180-degree rotation around any axis perpendicular to Z
                var axis = currentZ.NormalizedCross(Vector3.UnitX);
                if (axis.LengthSquared() < 1e-6f)
                    axis = currentZ.NormalizedCross(Vector3.UnitY);
                return Quaternion.CreateFromAxisAngle(axis, MathF.PI);
            }

            var rotationAxis = currentZ.NormalizedCross(targetZ);
            var angle = currentZ.Dot(targetZ).Clamp(-1.0f, 1.0f).Acos;
            return Quaternion.CreateFromAxisAngle(rotationAxis, angle);
        }

        public static Matrix4x4 AlignToQuad(this Quad3D q)
            => AlignZAxisWith(q.Normal) * Matrix4x4.CreateTranslation(q.Center);

        /// <summary>
        /// Returns a quaternion that rotates vector <paramref name="from"/> to align with <paramref name="to"/>.
        /// </summary>
        /*
        public static Quaternion RotateTo(this Vector3 from, Vector3 to, float epsilon = 1e-6f)
        {
            var aLen = from.Length();
            var bLen = to.Length();
            if (aLen < epsilon || bLen < epsilon)
                return Quaternion.Identity; // undefined rotation; choose identity

            var a = from / aLen;
            var b = to / bLen;

            var dot = a.Dot(b);

            // Almost identical direction
            if (dot > 1f - 1e-6f)
                return Quaternion.Identity;

            // Almost opposite direction: 180° around any axis orthogonal to 'a'
            if (dot < -1f + 1e-6f)
            {
                // Pick an orthogonal axis: try X, else Y
                Vector3 axis = Vector3.Cross(a, Vector3.UnitX);
                if (axis.LengthSquared() < 1e-12f)
                    axis = Vector3.Cross(a, Vector3.UnitY);
                axis = axis.Normalize;
                return Quaternion.CreateFromAxisAngle(axis, MathF.PI);
            }

            // General case
            var v = Vector3.Cross(a, b);
            // Closed-form quaternion (normalized):
            // q = [v * (1/s), s/2], where s = sqrt((1+dot)*2)
            float s = MathF.Sqrt((1f + dot) * 2f);
            float invS = 1f / s;

            var q = new Quaternion(v.X * invS, v.Y * invS, v.Z * invS, s * 0.5f);

            // Ensure unit quaternion (good practice against FP drift)
            return q.Normalize;
        }
        */
        public static Quaternion RotateTo(this Vector3 from, Vector3 to, float epsilon = 1e-6f)
        {
            float aLen = from.Length();
            float bLen = to.Length();
            if (aLen < epsilon || bLen < epsilon)
                return Quaternion.Identity;

            var a = from / aLen;
            var b = to / bLen;

            // Clamp dot for safety
            float dot = Math.Clamp(Vector3.Dot(a, b), -1f, 1f);

            // Same direction
            if (dot > 1f - 1e-6f)
                return Quaternion.Identity;

            // Opposite direction: 180° around any axis orthogonal to 'a'
            if (dot < -1f + 1e-6f)
            {
                // Pick an orthogonal axis robustly
                Vector3 axis = Vector3.Cross(a, MathF.Abs(a.X) < 0.9f ? Vector3.UnitX : Vector3.UnitY);
                axis = axis.Normalize;
                return Quaternion.CreateFromAxisAngle(axis, MathF.PI); // already unit
            }

            // Stable general case: q ~ [a×b, 1 + a·b], then normalize
            Vector3 c = Vector3.Cross(a, b);
            var q = new Quaternion(c.X, c.Y, c.Z, 1f + dot);
            q = q.Normalize;

            return q;
        }

        public static Matrix4x4 ToBoxTransform(this Line3D line, float thickness, float height)
            => Matrix4x4.CreateScale(line.Length, thickness, height)
               * Vector3.UnitX.RotateTo(line.Direction)
               * Matrix4x4.CreateTranslation(line.Center);

        public static Matrix4x4 AlignZAxisTransform(this Line3D line)
            => Matrix4x4.CreateScale(1, 1, line.Length)
               * Vector3.UnitZ.RotateTo(line.Direction)
               * Matrix4x4.CreateTranslation(line.A);

        public static LineMesh3D ToLineMesh(this IReadOnlyList<Point3D> points, bool closed)
        {
            var lines = new List<Integer2>();
            for (var i = 0; i < points.Count - 1; i++)
                lines.Add((i, i + 1));
            if (closed)
                lines.Add((points.Count - 1, 0));
            return new(points, lines);
        }

        public static Bounds3D FastTransform(this Bounds3D b, Matrix4x4 m)
        {
            // Center / extents in local space
            var c = (b.Min + b.Max) * 0.5f;
            var e = (b.Max - b.Min) * 0.5f;

            // Transform center (full affine)
            var c4 = new Vector4(c.X, c.Y, c.Z, 1f).Transform(m);
            var ct = new Vector3(c4.X, c4.Y, c4.Z);

            // Upper-left 3x3 of m (System.Numerics is row-major fields Mij)
            // Compute new extents using absolute value of linear part
            var ex = MathF.Abs(m.M11) * e.X + MathF.Abs(m.M21) * e.Y + MathF.Abs(m.M31) * e.Z;
            var ey = MathF.Abs(m.M12) * e.X + MathF.Abs(m.M22) * e.Y + MathF.Abs(m.M32) * e.Z;
            var ez = MathF.Abs(m.M13) * e.X + MathF.Abs(m.M23) * e.Y + MathF.Abs(m.M33) * e.Z;

            var et = new Vector3(ex, ey, ez);
            return new(ct - et, ct + et);
        }

        public static Vector3 Median(this IReadOnlyList<Vector3> pts)
        {
            // Simple approach: copy to arrays and sort each axis.
            var xs = new float[pts.Count];
            var ys = new float[pts.Count];
            var zs = new float[pts.Count];
            for (int i = 0; i < pts.Count; i++)
            {
                xs[i] = pts[i].X;
                ys[i] = pts[i].Y;
                zs[i] = pts[i].Z;
            }

            Array.Sort(xs);
            Array.Sort(ys);
            Array.Sort(zs);
            int mid = pts.Count / 2;
            return new Vector3(xs[mid], ys[mid], zs[mid]);
        }

        public static float Quantile(this IReadOnlyList<float> values, float q)
        {
            // q in [0,1]. Uses selection-by-sorting; fine for thousands of instances.
            var tmp = values.ToArray();
            Array.Sort(tmp);
            int idx = (int)MathF.Round(q * (tmp.Length - 1));
            idx = Math.Clamp(idx, 0, tmp.Length - 1);
            return tmp[idx];
        }

        public static Point3D GetMedianCenter(this IReadOnlyList<Bounds3D> bounds)
            => bounds.Map(b => b.Center.Vector3).Median();

        public static Bounds3D GetTotalBounds(this IReadOnlyList<Bounds3D> bounds)
        {
            var total = Bounds3D.Empty;
            foreach (var b in bounds)
                total = total.Include(b);
            return total;
        }

        public static Bounds3D GetTotalBoundsTrimOutliers(this IReadOnlyList<Bounds3D> bounds,
            float trimFraction = 0.1f)
        {
            if (bounds.Count < 5)
                return bounds.GetTotalBounds();

            var center = GetMedianCenter(bounds);
            var distances = bounds.Map(b => b.Center.Vector3.Distance(center).Value);

            // Find distance cutoff at (1-trimFraction) quantile
            var cutoff = Quantile(distances, 1.0f - trimFraction);

            // Union only inliers
            var total = Bounds3D.Empty;
            for (int i = 0; i < bounds.Count; i++)
                if (distances[i] <= cutoff)
                    total = total.Include(bounds[i]);

            return total;
        }

        public static IReadOnlyList<Point3D> GetCircularPoints(this int n, float radius = 1f)
        {
            var poly = new RegularPolygon(Point2D.Default, (n + 1));
            return poly.Points.To3D().Map(p => p * radius);
        }

        //==

        public static QuadMesh3D Subdivide(this QuadMesh3D self, int n)
        {
            if (n <= 0) throw new ArgumentOutOfRangeException(nameof(n), "n must be >= 1.");
            if (n == 1) return self;

            var (srcPoints, srcFaces) = self;
            var builder = new QuadMesh3DBuilder();

            // Reuse original vertices
            var vertexMap = new Dictionary<int, int>(srcPoints.Count);

            int GetOrAddVertex(int srcIndex)
            {
                if (vertexMap.TryGetValue(srcIndex, out var dst)) return dst;
                dst = builder.Points.Count;
                builder.Points.Add(srcPoints[srcIndex]);
                vertexMap[srcIndex] = dst;
                return dst;
            }

            // Reuse subdivided edge vertices across adjacent faces.
            // Key = (minVertex, maxVertex, kFromMin) where kFromMin is 1..n-1.
            var edgeMap = new Dictionary<(int lo, int hi, int kFromLo), int>();

            int GetOrAddEdgePoint(int i0, int i1, int k) // k in 0..n
            {
                if (k <= 0) return GetOrAddVertex(i0);
                if (k >= n) return GetOrAddVertex(i1);

                var lo = Math.Min(i0, i1);
                var hi = Math.Max(i0, i1);

                // Convert "k from i0->i1" into "k from lo->hi"
                var kFromLo = (i0 == lo) ? k : (n - k);

                var key = (lo, hi, kFromLo);
                if (edgeMap.TryGetValue(key, out var dstIndex))
                    return dstIndex;

                var pLo = srcPoints[lo];
                var pHi = srcPoints[hi];
                var t = kFromLo / (float)n;

                dstIndex = builder.Points.Count;
                builder.Points.Add(pLo.Lerp(pHi, t));
                edgeMap[key] = dstIndex;
                return dstIndex;
            }

            for (var faceIndex = 0; faceIndex < srcFaces.Count; faceIndex++)
            {
                var f = srcFaces[faceIndex];
                int ia = f.A, ib = f.B, ic = f.C, id = f.D;

                var q = new Quad3D(srcPoints[ia], srcPoints[ib], srcPoints[ic], srcPoints[id]);

                // Grid of vertex indices for this face (size (n+1)x(n+1))
                var grid = new int[n + 1, n + 1];

                for (var v = 0; v <= n; v++)
                {
                    for (var u = 0; u <= n; u++)
                    {
                        int idx;

                        // Corners
                        if (u == 0 && v == 0) idx = GetOrAddVertex(ia);
                        else if (u == n && v == 0) idx = GetOrAddVertex(ib);
                        else if (u == n && v == n) idx = GetOrAddVertex(ic);
                        else if (u == 0 && v == n) idx = GetOrAddVertex(id);

                        // Edges (reuse across faces)
                        else if (v == 0) idx = GetOrAddEdgePoint(ia, ib, u); // A->B
                        else if (u == n) idx = GetOrAddEdgePoint(ib, ic, v); // B->C
                        else if (v == n) idx = GetOrAddEdgePoint(id, ic, u); // D->C (left->right along top edge)
                        else if (u == 0) idx = GetOrAddEdgePoint(ia, id, v); // A->D

                        // Interior (unique to this face)
                        else
                        {
                            var uu = u / (float)n;
                            var vv = v / (float)n;
                            var uv = new Vector2(uu, vv);
                            idx = builder.Points.Count;
                            builder.Points.Add(q.Bilinear(uv));
                        }

                        grid[u, v] = idx;
                    }
                }

                // Emit n*n quads
                for (var v = 0; v < n; v++)
                {
                    for (var u = 0; u < n; u++)
                    {
                        var a = grid[u, v];
                        var b = grid[u + 1, v];
                        var c = grid[u + 1, v + 1];
                        var d = grid[u, v + 1];
                        builder.AddFace(new Integer4(a, b, c, d));
                    }
                }
            }

            return builder.ToQuadMesh3D();
        }

        /// <summary>
        /// Creates a quad cap from the top numPoints. 
        /// </summary>
        public static QuadMesh3D Cap(this QuadMesh3D mesh, IReadOnlyList<int> indices, int segments)
        {
            if (indices == null) throw new ArgumentNullException(nameof(indices));
            if (indices.Count < 3)
                throw new ArgumentException("Need at least 3 indices to form a polygon.", nameof(indices));
            if (segments < 1) throw new ArgumentOutOfRangeException(nameof(segments), "segments must be >= 1.");

            var (srcPoints, srcFaces) = mesh;

            // Start with a copy of the input mesh (so we "add a cap" onto it)
            var builder = new QuadMesh3DBuilder();
            builder.Points.AddRange(srcPoints);
            builder.AddFaces(srcFaces);

            // Compute polygon center as average of boundary vertices
            float cx = 0, cy = 0, cz = 0;
            for (var i = 0; i < indices.Count; i++)
            {
                var p = srcPoints[indices[i]];
                cx += p.X;
                cy += p.Y;
                cz += p.Z;
            }

            var inv = 1.0f / indices.Count;
            var center = new Point3D(cx * inv, cy * inv, cz * inv);

            var m = indices.Count;

            // Ring 0 uses existing boundary indices
            var prevRing = new int[m];
            for (var i = 0; i < m; i++)
                prevRing[i] = indices[i];

            // Create inner rings 1..segments.
            // Ring "segments" collapses to the center point (all vertices equal), producing a closed cap.
            for (var r = 1; r <= segments; r++)
            {
                var t = r / (float)segments;

                var currRing = new int[m];

                for (var i = 0; i < m; i++)
                {
                    var bp = srcPoints[indices[i]];
                    var p = bp.Lerp(center, t);

                    currRing[i] = builder.Points.Count;
                    builder.Points.Add(p);
                }

                // Connect prev ring to current ring with a quad strip
                for (var i = 0; i < m; i++)
                {
                    var iNext = (i + 1) % m;

                    var a = prevRing[i];
                    var b = prevRing[iNext];
                    var c = currRing[iNext];
                    var d = currRing[i];

                    builder.AddFace(new Integer4(a, b, c, d));
                }

                prevRing = currRing;
            }

            return builder.ToQuadMesh3D();
        }

        public static QuadMesh3D ToMesh(this Bounds3D self)
            => PlatonicSolids.Cube.Scale(self.Size).Translate(self.Center);

        public static T Normalize<T>(this ITransformable3D<T> mesh, Bounds3D meshBounds, bool uniformScaling = true,
            bool center = true)
            where T : ITransformable3D<T>
        {
            var maxSide = meshBounds.Size.MaxComponent;
            var invScale = uniformScaling
                ? new Vector3(maxSide, maxSide, maxSide)
                : meshBounds.Size;
            var scale = invScale.Reciprocal;
            var translatedMesh = mesh.Translate(-meshBounds.Center.Vector3);
            var scaledMesh = translatedMesh.Scale(scale);
            return center
                ? scaledMesh
                : scaledMesh.Translate(meshBounds.Center);
        }

        public static TriangleMesh3D Normalize(this TriangleMesh3D mesh, bool uniformScaling = true, bool center = true)
            => mesh.Normalize(mesh.Bounds, uniformScaling, center);

        public static QuadMesh3D Normalize(this QuadMesh3D mesh, bool uniformScaling = true, bool center = true)
            => mesh.Normalize(mesh.Points.Bounds(), uniformScaling, center);

        public static TriangleMesh3D FitToBounds(this TriangleMesh3D mesh, Bounds3D bounds)
            => mesh.Normalize().Scale(bounds.Size).Translate(bounds.Center);

        public static QuadMesh3D FitToBounds(this QuadMesh3D mesh, Bounds3D bounds)
            => mesh.Normalize().Scale(bounds.Size).Translate(bounds.Center);

        public static bool Within(this Vector2 v, Vector2 a, Vector2 b)
            => v.X >= a.X && v.Y <= b.X && v.Y >= a.Y && v.Y <= b.Y;

        public static bool Within(this Vector3 v, Vector3 a, Vector3 b)
            => v.X >= a.X && v.Y <= b.X && v.Y >= a.Y && v.Y <= b.Y && v.Z >= a.Z && v.Z <= b.Z;

        public static bool Within(this Vector4 v, Vector4 a, Vector4 b)
            => v.X >= a.X && v.Y <= b.X && v.Y >= a.Y && v.Y <= b.Y && v.Z >= a.Z && v.Z <= b.Z && v.W >= a.W &&
               v.W <= b.W;

        //==

        public static Vector3 ToVector3(this Color c)
            => (c.R, c.G, c.B);

        public static Vector3 WeightedSum(this IEnumerable<(Vector3, double)> weightedVectors)
        {
            var sum = Vector3.Zero;
            foreach (var pair in weightedVectors)
                sum += pair.Item1 * (float)pair.Item2;
            return sum;
        }

        public static Vector3 WeightedAverage(this IEnumerable<(Vector3, double)> weightedVectors)
            => weightedVectors.WeightedSum().SafeNormalize();

        public static Vector3 WeightedSum<T>(this IReadOnlyList<T> values, Func<T, Vector3> vectorSelect,
            Func<T, double> weightSelect)
            => values.Select(v => (vectorSelect(v), weightSelect(v))).WeightedSum();

        public static Vector3 WeightedAverage<T>(this IReadOnlyList<T> values, Func<T, Vector3> vectorSelect,
            Func<T, double> weightSelect)
            => values.Select(v => (vectorSelect(v), weightSelect(v))).WeightedAverage();

        public const double Epsilon = 1e-15;

        public static bool IsFinite(this Number x)
            => x.Value.IsFinite();

        public static Vector3 SafeNormalize(this Vector3 v)
        {
            var len = v.Length();
            return len <= Epsilon || !len.IsFinite() ? Vector3.Zero : v / len;
        }

        // NOTE: the old "Angle" function does not work. 
        public static double AngleBetween(this Vector3 a, Vector3 b)
        {
            var cos = a.Dot(b).SafeDivide(a.Length * b.Length);
            return !cos.IsFinite() ? 0.0 : Clamp(cos, -1.0, 1.0).Acos();
        }

        public static double Cotangent(this Vector3 a, Vector3 b)
            => a.Dot(b).Value.SafeDivide(a.Cross(b).Length());

        public static double SafeDivide(this double numerator, double denominator, double fallback = 0.0)
            => Math.Abs(denominator) <= Epsilon
                ? fallback
                : (numerator / denominator).FiniteOrFallback(fallback);

        public static float SafeDivide(this Number numerator, float denominator, float fallback = 0.0f)
            => numerator.Value.SafeDivide(denominator, fallback);

        public static float SafeDivide(this float numerator, float denominator, float fallback = 0.0f)
            => Math.Abs(denominator) <= Epsilon
                ? fallback
                : (numerator / denominator).FiniteOrFallback(fallback);

        public static double SafeNonNegative(this double value)
            => !value.IsFinite() || value < 0.0 ? 0.0 : value;

        public static double Clamp(this double v, double min, double max)
            => Math.Max(min, Math.Min(v, max));

        public static bool IsFinite(this double x)
            => !double.IsNaN(x) && !double.IsInfinity(x);

        public static bool IsFinite(this float x)
            => !float.IsNaN(x) && !float.IsInfinity(x);

        public static float FiniteOrFallback(this float f, float fallback = 0)
            => f.IsFinite() ? f : fallback;

        public static double FiniteOrFallback(this double d, double fallback = 0)
            => d.IsFinite() ? d : fallback;

        public static LineMesh3D Subdivide(this LineMesh3D mesh, int count)
        {
            if (count <= 1)
                return mesh;
            var pts = mesh.Points.ToList();
            var lines = new List<Integer2>();
            foreach (var f in mesh.FaceIndices)
            {
                var pt0 = mesh.Points[f.A];
                var pt1 = mesh.Points[f.B];
                for (var i = 0; i <= count; i++)
                {
                    var pt = pt0.Lerp(pt1, (float)i / count);
                    var n = pts.Count;
                    if (i == 0)
                    {
                        pts.Add(pt);
                        lines.Add((f.A, n));
                    }
                    else if (i == count)
                    {
                        lines.Add((n - 1, f.B));
                    }
                    else
                    {
                        pts.Add(pt);
                        lines.Add((n - 1, n));
                    }
                }
            }

            return new LineMesh3D(pts, lines);
        }

        public static LineMesh3D ToLineMesh(this Quad3D q)
            => new(q.Points, [(0, 1), (1, 2), (2, 3), (3, 0)]);

        public static LineMesh3D ToLineMesh(this Quad2D q)
            => q.To3D.ToLineMesh();

        public static Matrix4x4 ToMatrix(this Cylinder cylinder)
            => Matrix4x4.CreateScale(cylinder.Radius * 2, cylinder.Radius * 2, 1f) * cylinder.Line.ToMatrix();

        public static Matrix4x4 ToMatrix(this Sphere sphere)
            => Matrix4x4.CreateScale(sphere.Radius) * Matrix4x4.CreateTranslation(sphere.Center.Vector3);

        public static Matrix4x4 ToMatrix(this Line3D line)
        {
            var a = line.A.Vector3;
            var b = line.B.Vector3;

            var dz = b - a;
            var length = dz.Length();

            var c = line.Center.Vector3;

            if (length <= float.Epsilon)
                return Matrix4x4.CreateTranslation(c);

            var z = dz / length;

            // Pick a stable reference vector that is not parallel to the line.
            var reference = MathF.Abs(Vector3.Dot(z, Vector3.UnitZ)) < 0.999f
                ? Vector3.UnitZ
                : Vector3.UnitY;

            var x = reference.NormalizedCross(z);
            var y = z.Cross(x);

            // Maps a unit-height cylinder running from local Z=-0.5 to local Z=0.5
            // onto the line from A to B.
            return new Matrix4x4(
                x.X, x.Y, x.Z, 0,
                y.X, y.Y, y.Z, 0,
                z.X * length, z.Y * length, z.Z * length, 0,
                c.X, c.Y, c.Z, 1);
        }

        public static YawPitch YawPitch(this Line3D line)
        {
            var d = line.B.Vector3 - line.A.Vector3;

            Debug.Assert(d.LengthSquared() > 0, "Line segment must have non-zero length.");

            var horizontalLength = Math.Sqrt(d.X * d.X + d.Y * d.Y);

            // Heading:
            // atan2(x, y) instead of atan2(y, x), because +Y is forward / zero heading.
            var heading = Math.Atan2(d.X, d.Y);

            // Elevation:
            // angle above the XY plane.
            var elevation = Math.Atan2(d.Z, horizontalLength);

            Debug.Assert(!double.IsNaN(heading));
            Debug.Assert(!double.IsNaN(elevation));
            Debug.Assert(elevation >= -Math.PI / 2 && elevation <= Math.PI / 2);

            return ((float)heading, (float)elevation);
        }

        public static Matrix4x4 ToScaleMatrix(this Vector3 v)
            => Matrix4x4.CreateScale(v.X, v.Y, v.Z);

        public static Matrix4x4 ToTranslationMatrix(this Point3D p)
            => p.Vector3.ToTranslationMatrix();

        public static Matrix4x4 ToTranslationMatrix(this Vector3 v)
            => Matrix4x4.CreateTranslation(v);

        public static QuadMesh3D UnitCylinder(int sides = 16)
            => sides.GetCircularPoints(0.5f).Translate((0, 0, -0.5f)).Extrude(1).ToQuadMesh3D();

        public static QuadMesh3D ToCylinderMesh(this Line3D line, int sides = 16)
            => UnitCylinder(sides).Transform(line.ToMatrix());

        public static QuadMesh3D ToBoxMesh(this Line3D line, int sides = 16)
            => PlatonicSolids.Cube.Transform(line.ToMatrix());

        public static bool IsMostlyParallelTo(this Vector3 a, Vector3 b)
            => a.IsMostlyParallelTo(b, 15.Degrees());

        public static bool IsMostlyParallelTo(this Vector3 a, Vector3 b, Angle tolerance)
        {
            if (!a.IsFinite() || !b.IsFinite())
                return false;

            var la = a.Length();
            var lb = b.Length();

            if (la <= 0 || lb <= 0)
                return false;

            return (a.Dot(b) / (la * lb)).Abs()
                   >= tolerance.Cos;
        }

        public static bool IsMostlyPerpendicularTo(this Vector3 a, Vector3 b)
            => a.IsMostlyPerpendicularTo(b, 15.Degrees());

        public static bool IsMostlyPerpendicularTo(this Vector3 a, Vector3 b, Angle tolerance)
        {
            if (!a.IsFinite() || !b.IsFinite())
                return false;

            var la = a.Length();
            var lb = b.Length();

            if (la <= 0 || lb <= 0)
                return false;

            return (a.Dot(b) / (la * lb)).Abs
                   <= tolerance.Sin;
        }

        public static Line3D ToCenteredLine(this Point3D point, Vector3 halfDir)
            => (point - halfDir, point + halfDir);

        public static Matrix4x4 LookAtMatrix(this Point3D pos, Point3D target)
            => Matrix4x4.CreateLookAt(pos, target, Vector3.UnitZ);

        public static Matrix4x4 LookInDirectionMatrix(this Point3D pos, Vector3 dir)
            => pos.LookAtMatrix(pos + dir);

        public static IReadOnlyList<Transform3D> GetTransforms(this Curve3D curve, int count)
        {
            var r = new Transform3D[count + 1];
            for (var i = 0; i <= count; i++)
            {
                var t0 = (float)i / count;
                var t1 = t0 + 0.001f;
                var pos0 = curve.Eval(t0);
                var pos1 = curve.Eval(t1);
                var pose = pos0.LookAt(pos1);
                r[i] = pose;
            }

            return r;
        }

        public static OrthonormalBasis3D CreateBasisFromZ(this Vector3 z)
        {
            // Pick the world axis least parallel to z.
            // This avoids unstable cross products when z is near world X/Y/Z.
            var helper = MathF.Abs(z.Z) < 0.9f
                ? Vector3.UnitZ
                : Vector3.UnitY;
            var x = helper.NormalizedCross(z);
            var y = z.Cross(x);
            var axes = new Axes3D(x, y, z);
            return new OrthonormalBasis3D(axes);
        }

        public static Bounds3D Combine(this IReadOnlyList<Bounds3D> bounds)
        {
            if (bounds == null || bounds.Count == 0)
                return Bounds3D.Empty;
            var combined = bounds[0];
            foreach (var b in bounds)
                combined = combined.Include(b);
            return combined;
        }

        public static Bounds3D Bounds(this Triangle3D tri)
            => tri.Points.Bounds();

        public static Bounds3D Merge(this Bounds3D b1, Bounds3D b2)
            // TODO: I am not sure that the name "Include" make sense.
            => b1.Include(b2);

        public static Bounds3D Bounds(this IEnumerable<Point3D> p)
            => p.Aggregate(Bounds3D.Empty, (b1, b2) => b1.Include(b2));

        public static Bounds3D Bounds(this IEnumerable<Line3D> lines)
            => lines.SelectMany(t => t.Points).Bounds();

        public static Bounds3D Bounds(this IEnumerable<Triangle3D> triangles)
            => triangles.SelectMany(t => t.Points).Bounds();

        public static Bounds3D Bounds(this IEnumerable<Quad3D> quads)
            => quads.SelectMany(t => t.Points).Bounds();

        public static Bounds3D Bounds(this IEnumerable<Bounds3D> boxes)
            => boxes.SelectMany(b => new[] { b.Min, b.Max }).Bounds();

        public static Bounds3D Bounds(this IEnumerable<TriangleMesh3D> meshes)
            => meshes.SelectMany(m => m.Points).Bounds();

        public static Bounds3D Bounds(this IEnumerable<LineMesh3D> meshes)
            => meshes.SelectMany(m => m.Points).Bounds();

        public static Bounds3D Bounds(this IEnumerable<QuadMesh3D> meshes)
            => meshes.SelectMany(m => m.Points).Bounds();

        public static Bounds3D Bounds(this IEnumerable<QuadGrid3D> meshes)
            => meshes.SelectMany(m => m.Points).Bounds();

        public static Bounds3D Bounds(this IEnumerable<IEnumerable<Point3D>> pointLists)
            => pointLists.SelectMany(list => list).Bounds();

        public const double DefaultEpsilon = 1e-12;
        public const float DefaultFloatEpsilon = 1e-6f;

        public static bool IsFinite(this Vector3 v)
            => float.IsFinite(v.X) && float.IsFinite(v.Y) && float.IsFinite(v.Z);

        public static bool IsUnit(this Vector3 v, float tolerance = 1e-4f)
            => Math.Abs(v.LengthSquared() - 1f) <= tolerance;

        public static Vector3 NormalizeSafe(this Vector3 v, Vector3 fallback, float epsilon = DefaultFloatEpsilon)
        {
            Debug.Assert(fallback.IsFinite());
            Debug.Assert(fallback.LengthSquared() > epsilon * epsilon);

            if (!v.IsFinite())
                return fallback.Normalize;

            var lenSq = v.LengthSquared();
            return lenSq > epsilon * epsilon
                ? v / MathF.Sqrt(lenSq)
                : fallback.Normalize;
        }

        public static Vector3 AnyPerpendicular(Vector3 axis)
        {
            Debug.Assert(axis.IsFinite());
            Debug.Assert(axis.LengthSquared() > 0);

            axis = axis.Normalize();

            var other = Math.Abs(axis.X) < 0.9f
                ? Vector3.UnitX
                : Vector3.UnitY;

            return axis.NormalizedCross(other);
        }

        public static double Distance(this Point3D p, Line3D line)
            => p.Vector3.DistanceToLine(line.A, line.Direction.Normalize);

        // TODO: create Normal3D, Normal2D, Normal1D, UV

        public static double DistanceToLine(this Vector3 p, Vector3 linePoint, Vector3 lineDir)
        {
            var d = p - linePoint;
            var axial = d.Dot(lineDir);
            var radial = d - axial * lineDir;
            return radial.Length();
        }

        public static Vector3 ProjectOntoLine(Vector3 p, Vector3 linePoint, Vector3 lineDir)
        {
            var d = p - linePoint;
            var t = d.Dot(lineDir);
            return linePoint + t * lineDir;
        }

        public static double SignedDistanceAlongLine(Vector3 p, Vector3 linePoint, Vector3 lineDir)
        {
            return Vector3.Dot(p - linePoint, lineDir);
        }

        public static Vector3 Reject(this Vector3 v, Vector3 unitAxis)
        {
            Debug.Assert(unitAxis.IsUnit());
            return v - Vector3.Dot(v, unitAxis) * unitAxis;
        }

        public static bool Intersects(this Bounds3D bounds, Bounds3D other)
            => bounds.IntervalX().Intersects(other.IntervalX())
               && bounds.IntervalY().Intersects(other.IntervalY())
               && bounds.IntervalZ().Intersects(other.IntervalZ());

        public static bool Intersects(this NumberInterval a, NumberInterval b)
            => a.Start <= b.End && a.End >= b.Start;

        public static NumberInterval IntervalX(this Bounds3D bounds) => new(bounds.Min.X, bounds.Max.X);
        public static NumberInterval IntervalY(this Bounds3D bounds) => new(bounds.Min.Y, bounds.Max.Y);
        public static NumberInterval IntervalZ(this Bounds3D bounds) => new(bounds.Min.Z, bounds.Max.Z);

        public static Bounds3D Expand(this Bounds3D bounds, Vector3 v)
            => (bounds.Min - v, bounds.Max + v);

        public static Bounds3D OffsetMax(this Bounds3D bounds, Vector3 v)
            => bounds.WithMax(bounds.Max + v);

        public static Bounds3D OffsetMin(this Bounds3D bounds, Vector3 v)
            => bounds.WithMin(bounds.Min + v);

        public static Vector3 Average(this IEnumerable<Vector3> vs)
        {
            var sum = Vector3.Zero;
            var n = 0;
            foreach (var v in vs)
            {
                sum += v;
                n++;
            }

            if (n <= 1)
                return sum;
            return sum / n;
        }

        public static bool IsMostlyFlat(this TriangleMesh3D mesh)
            => mesh.IsMostlyFlat(1.Degrees());

        public static bool IsMostlyFlat(this TriangleMesh3D mesh, Angle cutOff)
        {
            var averageNormal = mesh.GetFaceNormals().Average();
            var faceNormals = mesh.GetFaceNormals();
            return !faceNormals.Any(n => averageNormal.Angle(n) > cutOff);
        }

        public static IReadOnlyList<TriangleMesh3D> SplitByCreaseAngle(this TriangleMesh3D mesh, Angle cutOff)
            => mesh.FaceGroupsByCreaseAngle(cutOff).Split(mesh);

        public static FaceGroups FaceGroupsByCreaseAngle(this TriangleMesh3D mesh, Angle cutOff)
        {
            var topo = mesh.GetTopology();
            return FaceGroups.Create(mesh.FaceIndices.Count, i => topo
                .GetFaceNeighborsWithAngleCutOff((FaceId)i, cutOff)
                .Select(f => (int)f.Id));
        }

        public static TriangleMesh3D RemoveFlatSurfaces(this TriangleMesh3D mesh, FaceGroups groups, Angle cutOff)
        {
            var meshes = groups.Split(mesh);
            var groupsToKeep = new List<FaceGroup>();
            for (var i = 0; i < meshes.Count; i++)
            {
                if (meshes[i].IsMostlyFlat(cutOff))
                    continue;
                groupsToKeep.Add(groups[i]);
            }
            return groupsToKeep.Merge(mesh);
        }

        public static int NumFaces(this TriangleMesh3D mesh)
            => mesh.FaceIndices?.Count ?? 0;
    }
}