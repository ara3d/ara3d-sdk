using System;
using Ara3D.Collections;
using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    public class PointsGeometry : IPoints
    {
        public PointsGeometry(IArray<Vector3> points)
            => Points = points; 

        public IArray<Vector3> Points { get; }

        public IGeometry Transform(Matrix4x4 mat)
            => Deform(p => p.Transform(mat)); 

        public IGeometry Deform(Func<Vector3, Vector3> f)
            => new PointsGeometry(Points.Select(f));
    }
}