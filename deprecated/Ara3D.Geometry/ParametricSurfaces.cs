using System;
using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    public class ParametricSurface :
        Procedural<Vector2, Vector3>,
        IParametricSurface,
        IDeformable<ParametricSurface>
    {
        public bool ClosedX { get; }
        public bool ClosedY { get; }

        public ParametricSurface(Func<Vector2, Vector3> func, bool closedX, bool closedY)
            : base(func) => (ClosedX, ClosedY) = (closedX, closedY);

        public IParametricSurface TransformInput(Func<Vector2, Vector2> f)
            => new ParametricSurface(x => Eval(f(x)), ClosedX, ClosedY);

        IGeometry IDeformable<IGeometry>.Deform(Func<Vector3, Vector3> f)
            => ((IDeformable<ParametricSurface>)this).Deform(f);

        ParametricSurface IDeformable<ParametricSurface>.Deform(Func<Vector3, Vector3> f)
            => new ParametricSurface(uv => f(Eval(uv)), ClosedX, ClosedY);

        IGeometry ITransformable<IGeometry>.Transform(Matrix4x4 mat)
            => ((IDeformable<ParametricSurface>)this).Deform(p => p.Transform(mat));

        ParametricSurface ITransformable<ParametricSurface>.Transform(Matrix4x4 mat)
            => ((IDeformable<ParametricSurface>)this).Deform(p => p.Transform(mat));
    }

    public static class ParametricSurfaces
    {
        public static ParametricSurface Sphere
            => new ParametricSurface(SurfaceFunctions.Sphere, true, true);

        public static ParametricSurface Torus(float r1, float r2)
            => new ParametricSurface(uv => uv.Torus(r1, r2), true, true);

        public static ParametricSurface MonkeySaddle
            => new ParametricSurface(SurfaceFunctions.MonkeySaddle, false, false);

        public static ParametricSurface Plane
            => new ParametricSurface(SurfaceFunctions.Plane, false, false);

        public static ParametricSurface Disc
            => new ParametricSurface(SurfaceFunctions.Disc, false, false);

        public static ParametricSurface Cylinder
            => new ParametricSurface(SurfaceFunctions.Cylinder, false, false);

        public static ParametricSurface ConicalSection(float r1, float r2)
            => new ParametricSurface(uv => uv.ConicalSection(r1, r2), true, false);

        public static ParametricSurface Trefoil(float r)
            => new ParametricSurface(uv => SurfaceFunctions.Trefoil(uv, r), true, true);
    }
}