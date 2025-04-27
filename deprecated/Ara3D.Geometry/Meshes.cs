using Ara3D.Collections;
using Ara3D.Mathematics;
using Ara3D.Utils;

namespace Ara3D.Geometry
{
    public static class Meshes
    {
        public static ITriMesh Triangle(Vector3 a, Vector3 b, Vector3 c)
            => LinqArray.Create(a, b, c).ToTriMesh();

        public static Quad Face(this IQuadMesh mesh, Int4 face)
            => (mesh.Points[face.X], 
                mesh.Points[face.Y], 
                mesh.Points[face.Z], 
                mesh.Points[face.W]);

        public static Quad Face(this IQuadMesh mesh, int face)
            => mesh.Face(mesh.FaceIndices[face]);

        public static Vector3 Eval(this Quad quad, Vector2 uv)
        {
            var lower = quad.A.Lerp(quad.B, uv.X);
            var upper = quad.D.Lerp(quad.C, uv.X);
            return lower.Lerp(upper, uv.Y);
        }

        public static QuadMesh Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
            => new[] { a, b, c, d }.ToIArray().ToQuadMesh();

        public static ITriMesh ToIMesh(this AABox box)
            => PlatonicSolids.Cube.Scale(box.Extent).Translate(box.Center);

        public static readonly float Sqrt3
            = MathOps.Sqrt(3.0f);

        public static readonly float HalfSqrt3
            = Sqrt3 / 2;

        public static readonly IArray<Vector2> TrianglePoints
            = LinqArray.Create<Vector2>(
                (-0.5f, -HalfSqrt3),
                (0f, HalfSqrt3),
                (0.5f, -HalfSqrt3));

        public static TriMesh TriangleTriMesh
            = TrianglePoints.To3D().ToTriMesh();

        public static readonly IArray<Vector2> SquarePoints
            = LinqArray.Create<Vector2>(
                (-0.5f, -0.5f),
                (-0.5f, 0.5f),
                (0.5f, 0.5f),
                (0.5f, -0.5f));

        public static IArray<Vector3> To3D(this IArray<Vector2> self)
            => self.Select(x => x.ToVector3());

        public static readonly QuadMesh Square
            = SquarePoints.To3D().ToQuadMesh();

        public static TessellatedMesh TorusMesh(float r1, float r2, int uSegs, int vSegs)
            => ParametricSurfaces.Torus(r1, r2).Tesselate(uSegs, vSegs);

        public static GridMesh Extrude(this IPolyLine3D polyLine, Vector3 direction)
            => Rule(polyLine, polyLine.Translate(direction));

        public static GridMesh Extrude(this IPolyLine2D polyLine, float amount = 1.0f)
            => polyLine.To3D().Extrude(Vector3.UnitZ * amount);

        public static bool HasSameTopology(this IPolyLine2D a, IPolyLine2D b)
            => a.Points.Count == b.Points.Count && a.Closed == b.Closed;

        public static bool HasSameTopology(this IPolyLine3D a, IPolyLine3D b)
            => a.Points.Count == b.Points.Count && a.Closed == b.Closed;

        public static GridMesh Rule(this IPolyLine2D a, IPolyLine2D b)
            => a.To3D().Rule(b.To3D());

        public static GridMesh Rule(this IPolyLine3D a, IPolyLine3D b)
            => a.Points.QuadStrip(b.Points, a.Closed);

        public static GridMesh QuadStrip(this IArray<Vector3> lower, IArray<Vector3> upper, bool closedX)
        {
            Verifier.Assert(lower.Count == upper.Count);
            return new GridMesh(Array2D.Create(lower, upper), closedX, false);
        }
     }
}
