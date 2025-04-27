using Ara3D.Collections;
using Ara3D.Mathematics;
using Ara3D.Utils;
using System;

namespace Ara3D.Geometry
{
    public class TriMesh : PointsGeometry, ITriMesh, IDeformable<TriMesh>
    {
        public TriMesh(IArray<Vector3> points, IArray<Int3> faceIndices)
            : base(points)
        {
            FaceIndices = faceIndices;
        }

        public IArray<int> Indices => FaceIndices.SelectMany(f => f.ToTuple());
        public IArray<Int3> FaceIndices { get; }

        public TriMesh Deform(Func<Vector3, Vector3> f)
            => new TriMesh(Points.Select(f), FaceIndices);

        public TriMesh Transform(Matrix4x4 mat)
            => Deform(p => p.Transform(mat));

        ITriMesh ITransformable<ITriMesh>.Transform(Matrix4x4 mat)
            => Transform(mat);

        ITriMesh IDeformable<ITriMesh>.Deform(Func<Vector3, Vector3> f)
            => Deform(f);
    }
}
