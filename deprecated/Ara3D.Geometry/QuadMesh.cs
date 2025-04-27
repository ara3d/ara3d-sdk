using Ara3D.Collections;
using Ara3D.Mathematics;
using Ara3D.Utils;
using System;

namespace Ara3D.Geometry
{
    public class QuadMesh : PointsGeometry, IQuadMesh, IDeformable<QuadMesh>
    {
        public QuadMesh(IArray<Vector3> points, IArray<Int4> faceIndices)
            : base(points)
        {
            FaceIndices = faceIndices;
        }
        public IArray<int> Indices => FaceIndices.SelectMany(f => f.ToTuple());
        public IArray<Int4> FaceIndices { get; }

        public QuadMesh Deform(Func<Vector3, Vector3> f)
            => new QuadMesh(Points.Select(f), FaceIndices);

        public QuadMesh Transform(Matrix4x4 mat)
            => Deform(p => p.Transform(mat));

        IQuadMesh ITransformable<IQuadMesh>.Transform(Matrix4x4 mat)
            => Transform(mat);

        IQuadMesh IDeformable<IQuadMesh>.Deform(Func<Vector3, Vector3> f)
            => Deform(f);
    }
}
