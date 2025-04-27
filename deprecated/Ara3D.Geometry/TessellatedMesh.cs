using Ara3D.Collections;
using Ara3D.Mathematics;
using System;

namespace Ara3D.Geometry
{
    public class TessellatedMesh : PointsGeometry, IQuadMesh, IDeformable<TessellatedMesh>
    {
        public TessellatedMesh(IArray<Vector3> points, IArray<Int4> faceIndices)
            : base(points)
        {
            FaceIndices = faceIndices;
            Indices = FaceIndices.SelectMany(f => f.ToTuple()).Evaluate();
        }

        public IArray<int> Indices { get; }
        public IArray<Int4> FaceIndices { get; }

        public TessellatedMesh Deform(Func<Vector3, Vector3> f) => new TessellatedMesh(Points.Select(f), FaceIndices);
        public TessellatedMesh Transform(Matrix4x4 mat) => Deform(p => p.Transform(mat));
        
        IQuadMesh ITransformable<IQuadMesh>.Transform(Matrix4x4 mat) => Transform(mat);
        IQuadMesh IDeformable<IQuadMesh>.Deform(Func<Vector3, Vector3> f) => Deform(f);
    }
}
