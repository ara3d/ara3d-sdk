using Ara3D.Geometry;

namespace Ara3D.Models;

public static class TransformableExtensions
{
    public static T Scale<T>(this ITransformable3D<T> self, Vector3 amount) where T : ITransformable3D<T>
        => self.Transform(Matrix4x4.CreateScale(amount.X, amount.Y, amount.Z));

    public static T Scale<T>(this ITransformable3D<T> self, float x) where T : ITransformable3D<T>
        => self.Transform(Matrix4x4.CreateScale(x));

    public static T Translate<T>(this ITransformable3D<T> self, Vector3 amount) where T : ITransformable3D<T>
        => self.Transform(Matrix4x4.CreateTranslation(amount));

    public static T Rotate<T>(this ITransformable3D<T> self, Vector3 axis, Angle angle) where T : ITransformable3D<T>
        => self.Transform(Quaternion.CreateFromAxisAngle(axis, angle));

    public static T Rotate<T>(this ITransformable3D<T> self, float yaw, float pitch, float roll) where T : ITransformable3D<T>
        => self.Transform(Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll));
}