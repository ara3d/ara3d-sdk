using System;
using Ara3D.Collections;
using Ara3D.Mathematics;
using Ara3D.Utils;

namespace Ara3D.Geometry
{
    public enum Axis
    {
        X,
        Y,
        Z,
        NegativeX,
        NegativeY,
        NegativeZ,
    }

    public static class Modifiers
    {
        public static TriMesh Triangulate(this IQuadMesh self)
            => new TriMesh(self.Points, self.FaceIndices.SelectMany(f =>
                LinqArray.Create(
                    new Int3(f.X, f.Y, f.Z),
                    new Int3(f.Z, f.W, f.X))));

        public static TriMesh FlipFaces(this ITriMesh mesh)
            => new TriMesh(mesh.Points, mesh.FaceIndices.Select(f => new Int3(f.Z, f.Y, f.X)));

        public static TriMesh Faceted(this ITriMesh mesh)
            => mesh.Points.SelectByIndex(mesh.Indices).ToTriMesh();

        public static ITriMesh ResetPivot(this ITriMesh triMesh)
            => triMesh.Translate(-triMesh.BoundingBox().CenterBottom);

        public static T SnapPoints<T>(this T mesh, float snapSize) where T : IDeformable<T>
            => MathUtil.Abs(snapSize) >= Constants.Tolerance
                ? mesh.Deform(v => (v * MathUtil.Inverse(snapSize)).Truncate() * snapSize)
                : mesh.Deform(v => Vector3.Zero);

    }
}