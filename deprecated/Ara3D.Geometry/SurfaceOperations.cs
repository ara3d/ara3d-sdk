using System;
using Ara3D.Collections;
using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    public static class SurfaceOperations
    {
        public static IParametricSurface Create(this Func<Vector2, Vector3> func, bool closedOnX, bool closedOnY)
            => new ParametricSurface(func, closedOnX, closedOnY);

        public static IParametricSurface Create(this Func<float, Func<float, Vector3>> func)
            => new ParametricSurface(uv => func(uv.X)(uv.Y), false, false);

        public static IParametricSurface Sweep(this ICurve<Vector3> profile, ICurve<Vector3> path)
            => Create(uv => profile.Eval(uv.X) + path.Eval(uv.Y), profile.Closed, path.Closed);

        // https://en.wikipedia.org/wiki/Ruled_surface
        public static IParametricSurface Rule(this ICurve<Vector3> profile1, ICurve<Vector3> profile2)
            => Create(uv => profile1.Eval(uv.X).Lerp(profile2.Eval(uv.X), uv.Y), profile1.Closed && profile2.Closed, false);

        // https://en.wikipedia.org/wiki/Lathe_(graphics) 
        // https://en.wikipedia.org/wiki/Surface_of_revolution
        // TODO: needs to be around a line. 
        public static IParametricSurface Revolve(this ICurve<Vector3> profile, Vector3 axis, float angleInRad, bool closed)
            => Create((uv) => profile.Eval(uv.X).RotateAround(axis, angleInRad / uv.Y), profile.Closed, closed);

        public static Vector3 LerpAlongVector(this Vector3 v, Vector3 direction, float t)
            => v.Lerp(v + direction, t);

        public static IParametricSurface Extrude(this ICurve3D profile, Vector3 vector)
            => Create((uv) => profile.Eval(uv.X).LerpAlongVector(vector, uv.Y), profile.Closed, false);

        public static float Epsilon => 1 / 1000000f;

        public static Vector3 GetNormal(this IParametricSurface parametricSurface, Vector2 uv)
        {
            var p0 = parametricSurface.Eval(uv);
            const float eps = Constants.Tolerance;
            var p1 = parametricSurface.Eval(uv + (eps, 0));
            var p2 = parametricSurface.Eval(uv + (0, eps));
            return (p2 - p0).Cross((p1 - p0)).Normalize();
        }

        public static IParametricSurface ToParametricSurface(this IProcedural<Vector2, float> self)
            => new ParametricSurface(uv => uv.ToVector3().SetZ(self.Eval(uv)), false, false);

        public static TessellatedMesh Tesselate(this IParametricSurface parametricSurface, int cols, int rows = 0)
        {
            var discreteSurface = new SurfaceDiscretization(cols, rows, parametricSurface.ClosedX, parametricSurface.ClosedY);
            var vertices = discreteSurface.Uvs.Select(parametricSurface.Eval).Evaluate();
            var faceVertices = discreteSurface.QuadIndices.Evaluate();
            return new TessellatedMesh(vertices, faceVertices);
        }
    }
}