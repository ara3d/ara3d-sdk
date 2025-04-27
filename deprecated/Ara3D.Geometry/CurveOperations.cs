using System;

namespace Ara3D.Geometry
{
    public static class CurveOperations
    {
        public static ICurve3D ToCurve3D(this ICurve2D curve, StandardPlane plane = StandardPlane.Xy)
        {
            switch (plane)
            {
                case StandardPlane.Yz: return new Curve3D(t => curve.Eval(t).ToVector3().ZXY, curve.Closed);
                case StandardPlane.Xz: return new Curve3D(t => curve.Eval(t).ToVector3().XZY, curve.Closed);
                case StandardPlane.Xy: return new Curve3D(t => curve.Eval(t).ToVector3(), curve.Closed);
                default: throw new ArgumentOutOfRangeException(nameof(plane));
            }
        }
    }
}