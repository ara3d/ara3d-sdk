
namespace Ara3D.Studio.Samples
{
    public class FrenetFrame
    {
        public Point3D Origin { get; }
        public Vector3 Tangent { get; }
        public Vector3 Normal { get; }
        public Vector3 Binormal { get;}

        public FrenetFrame(Point3D origin, Vector3 tangent, Vector3 normal, Vector3 binormal)
        {
            Origin = origin;
            Tangent = tangent;
            Normal = normal;
            Binormal = binormal;
        }

        public Matrix4x4 ToRotationMatrix()
        {
            var n = Normal.Normalize;
            var b = Binormal.Normalize;
            var t = Tangent.Normalize;
            return new Matrix4x4(
                t.X, t.Y, t.Z, 0f,
                b.X, b.Y, b.Z, 0f,
                n.X, n.Y, n.Z, 0f,   
                0f, 0f, 0f, 1f);
        }

        public Quaternion ToQuaternion()
            => Quaternion.CreateFromRotationMatrix(ToRotationMatrix());

        public Matrix4x4 Matrix()
            => ToRotationMatrix().WithTranslation(Origin);
    }

    internal class SweepDemo
    {
    }
}
