using System;
using System.Collections.Generic;
using System.Linq;
using Ara3D.Collections;
using Ara3D.Mathematics;
using Ara3D.Utils;

namespace Ara3D.Geometry
{
    public static class MeshExtensions
    {
        public static IArray<int> Indices(this ITriMesh triMesh)
            => triMesh.FaceIndices.SelectMany(f => LinqArray.Create(f.X, f.Y, f.Z));

        public static IArray<float> VerticesAsFloats(this ITriMesh triMesh)
            => triMesh.Points.SelectMany(v => Tuple.Create(v.X, v.Y, v.Z));

        // Computes the topology: this is a slow O(N) operation
        public static Topology ComputeTopology(this ITriMesh triMesh)
            => new Topology(triMesh);

        public static double Area(this ITriMesh triMesh)
            => triMesh.Triangles().Sum(t => t.Area);

        public static bool IsDegenerateVertexIndices(this Int3 vertexIndices)
            => vertexIndices.X == vertexIndices.Y || vertexIndices.X == vertexIndices.Z ||
               vertexIndices.Y == vertexIndices.Z;

        public static bool HasDegenerateFaceVertexIndices(this ITriMesh self)
            => self.FaceIndices.Any(IsDegenerateVertexIndices);

        public static bool GeometryEquals(this ITriMesh triMesh, ITriMesh other, float tolerance = Constants.Tolerance)
        {
            if (triMesh.GetNumFaces() != other.GetNumFaces())
                return false;
            return triMesh.Triangles().Zip(other.Triangles(), (t1, t2) => t1.AlmostEquals(t2, tolerance)).All(x => x);
        }

        public static ITriMesh SimplePolygonTessellate(this IEnumerable<Vector3> points)
        {
            var pts = points.ToList();
            var cnt = pts.Count;
            var sum = Vector3.Zero;
            var idxs = new List<int>(pts.Count * 3);
            for (var i = 0; i < pts.Count; ++i)
            {
                idxs.Add(i);
                idxs.Add(i + 1 % cnt);
                idxs.Add(cnt);
                sum += pts[i];
            }

            var midPoint = sum / pts.Count;
            pts.Add(midPoint);

            return pts.ToIArray().ToTriMesh(idxs.ToIArray());
        }

        /// <summary>
        /// Returns the closest point in a sequence of points
        /// </summary>
        public static Vector3 NearestPoint(this IEnumerable<Vector3> points, Vector3 x)
            => points.Minimize(float.MaxValue, p => p.DistanceSquared(x));

        /// <summary>
        /// Returns the closest point in a sequence of points
        /// </summary>
        public static Vector3 NearestPoint(this IArray<Vector3> points, Vector3 x)
            => points.ToEnumerable().NearestPoint(x);

        /// <summary>
        /// Returns the closest point in a geometry
        /// </summary>
        public static Vector3 NearestPoint(this IPoints self, Vector3 x)
            => self.Points.NearestPoint(x);

        public static Vector3 FurthestPoint(this IPoints self, Vector3 x0, Vector3 x1)
            => self.Points.FurthestPoint(x0, x1);

        public static Vector3 FurthestPoint(this IArray<Vector3> points, Vector3 x0, Vector3 x1)
            => points.ToEnumerable().FurthestPoint(x0, x1);

        public static Vector3 FurthestPoint(this IEnumerable<Vector3> points, Vector3 x0, Vector3 x1)
            => points.Maximize(float.MinValue, v => v.Distance(x0).Min(v.Distance(x1)));

        public static Vector3 FurthestPoint(this IPoints points, Vector3 x)
            => points.Points.FurthestPoint(x);

        public static Vector3 FurthestPoint(this IArray<Vector3> points, Vector3 x)
            => points.ToEnumerable().FurthestPoint(x);

        public static Vector3 FurthestPoint(this IEnumerable<Vector3> points, Vector3 x)
            => points.Maximize(float.MinValue, v => v.Distance(x));

        /// <summary>
        /// Returns the vertices organized by face corner. 
        /// </summary>
        public static IArray<Vector3> VerticesByIndex(this ITriMesh triMesh)
            => triMesh.Points.SelectByIndex(triMesh.Indices());

        /// <summary>
        /// Returns the vertices organized by face corner, normalized to the first position.
        /// This is useful for detecting if two meshes are the same except offset by 
        /// position.
        /// </summary>
        public static IArray<Vector3> NormalizedVerticesByCorner(this ITriMesh m)
        {
            if (m.GetNumCorners() == 0)
                return Vector3.Zero.Repeat(0);
            var firstVertex = m.Points[m.FaceIndices[0].X];
            return m.VerticesByIndex().Select(v => v - firstVertex);
        }

        /// <summary>
        /// Compares the face positions of two meshes normalized by the vertex buffer, returning the maximum distance, or null
        /// if the meshes have different topology. 
        /// </summary>
        public static float? MaxNormalizedDistance(this ITriMesh triMesh, ITriMesh other)
        {
            var xs = triMesh.NormalizedVerticesByCorner();
            var ys = other.NormalizedVerticesByCorner();
            if (xs.Count != ys.Count)
                return null;
            return xs.Zip(ys, (x, y) => x.Distance(y)).Max();
        }

        public static AABox BoundingBox(this IPoints points)
            => AABox.Create(points.Points.ToEnumerable());

        public static Sphere BoundingSphere(this IPoints points)
            => points.BoundingBox().ToSphere();

        public static float BoundingRadius(this IPoints points)
            => points.BoundingSphere().Radius;

        public static Vector3 Center(this IPoints points)
            => points.BoundingBox().Center;

        public static Vector3 Centroid(this IPoints points)
            => points.Points.Aggregate(Vector3.Zero, (x, y) => x + y) / points.Points.Count;

        public static bool AreIndicesValid(this ITriMesh triMesh)
            => triMesh.Indices().All(i => i >= 0 && i < triMesh.Points.Count);

        public static bool AreAllVerticesUsed(this ITriMesh triMesh)
        {
            var used = new bool[triMesh.Points.Count];
            for (var i = 0; i < triMesh.Indices().Count; i++)
                used[i] = true;
            return used.All(b => b);
        }

        public static Triangle VertexIndicesToTriangle(this ITriMesh triMesh, Int3 indices)
            => new Triangle(
                triMesh.Points[indices.X], 
                triMesh.Points[indices.Y], 
                triMesh.Points[indices.Z]);

        public static Triangle Triangle(this ITriMesh triMesh, int face)
            => triMesh.VertexIndicesToTriangle(triMesh.FaceIndices[face]);

        public static int GetNumFaces(this ITriMesh triMesh)
            => triMesh.FaceIndices.Count;

        public static int GetNumCorners(this ITriMesh triMesh)
            => triMesh.GetNumFaces() * 3;

        public static int GetNumVertices(this ITriMesh triMesh)
            => triMesh.Points.Count;

        public static int GetNumHalfEdges(this ITriMesh triMesh)
            => triMesh.GetNumCorners();

        public static int CornerToFace(this ITriMesh triMesh, int corner)
            => corner % 3;

        public static IArray<Triangle> Triangles(this ITriMesh triMesh)
            => triMesh.GetNumFaces().Select(triMesh.Triangle);

        public static IArray<Line> GetAllEdgesAsLines(this ITriMesh triMesh)
            => triMesh.Triangles().SelectMany(tri => Tuple.Create(tri.AB, tri.BC, tri.CA));

        public static IArray<Vector3> ComputedNormals(this ITriMesh triMesh)
            => triMesh.Triangles().Select(t => t.Normal);

        public static bool Planar(this ITriMesh triMesh, float tolerance = Constants.Tolerance)
        {
            if (triMesh.GetNumFaces() <= 1) return true;
            var normal = triMesh.Triangle(0).Normal;
            return triMesh.ComputedNormals().All(n => n.AlmostEquals(normal, tolerance));
        }

        public static IArray<Vector3> MidPoints(this ITriMesh triMesh)
            => triMesh.Triangles().Select(t => t.MidPoint);

        public static IArray<int> FacesToCorners(this ITriMesh triMesh)
            => triMesh.GetNumFaces().Select(i => i * 3);

        public static IArray<Int2> ToInt2s(this IArray<int> self)
            => self.SelectPairs((a, b) => new Int2(a, b));

        public static IArray<Int3> ToInt3s(this IArray<int> self)
            => self.SelectTriplets((a, b, c) => new Int3(a, b, c));

        public static IArray<Int4> ToInt4s(this IArray<int> self)
            => self.SelectQuartets((a, b, c, d) => new Int4(a, b, c, d));

        public static TriMesh ToTriMesh(this IArray<Vector3> points)
            => points.ToTriMesh(points.Indices());

        public static TriMesh ToTriMesh(this IArray<Vector3> points, IArray<int> indices)
            => points.ToTriMesh(indices.ToInt3s());

        public static TriMesh ToTriMesh(this IArray<Vector3> points, IArray<Int3> faceIndices)
            => new TriMesh(points, faceIndices);

        public static TriMesh ToTriMesh(this IArray<Vector3> points, params Int3[] faceIndices)
            => new TriMesh(points, faceIndices.ToIArray());

        public static QuadMesh ToQuadMesh(this IArray<Vector3> points)
            => points.ToQuadMesh(points.Indices());

        public static QuadMesh ToQuadMesh(this IArray<Vector3> points, IArray<int> indices)
            => points.ToQuadMesh(indices.ToInt4s());

        public static QuadMesh ToQuadMesh(this IArray<Vector3> points, IArray<Int4> faceIndices)
            => new QuadMesh(points, faceIndices);

        public static QuadMesh ToQuadMesh(this IArray<Vector3> points, params Int4[] faceIndices)
            => new QuadMesh(points, faceIndices.ToIArray());
    }
}