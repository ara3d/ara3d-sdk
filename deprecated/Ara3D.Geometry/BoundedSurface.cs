using System;
using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    public class BoundedSurface : IBoundedSurface, IDeformable<BoundedSurface>
    {
        public bool ClosedX => false;
        public bool ClosedY => false;
        public AABox Bounds { get; }
        public ISurface Surface { get; }

        public BoundedSurface(ISurface surface, AABox bounds)
            => (Bounds, Surface) = (bounds, surface);

        public BoundedSurface Transform(Matrix4x4 mat)
            => Deform(v => v.Transform(mat));

        public BoundedSurface Deform(Func<Vector3, Vector3> f)
            => new BoundedSurface((ISurface)Surface.Deform(f), Bounds);

        IGeometry ITransformable<IGeometry>.Transform(Matrix4x4 mat)
            => Transform(mat);

        IGeometry IDeformable<IGeometry>.Deform(Func<Vector3, Vector3> f)
            => Deform(f);
    }
}