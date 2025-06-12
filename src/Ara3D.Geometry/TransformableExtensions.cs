namespace Ara3D.Geometry;

public static class TransformableExtensions
{
    public static T Scale<T>(this ITransformable3D<T> self, Vector3 amount) where T : ITransformable3D<T>
        => self.Transform(Matrix4x4.CreateScale(amount.X, amount.Y, amount.Z));

    public static T Scale<T>(this ITransformable3D<T> self, Number x) where T : ITransformable3D<T>
        => self.Transform(Matrix4x4.CreateScale(x));

    public static T Translate<T>(this ITransformable3D<T> self, Vector3 amount) where T : ITransformable3D<T>
        => self.Transform(Matrix4x4.CreateTranslation(amount));

    public static T Rotate<T>(this ITransformable3D<T> self, AxisAngle axisAngle) where T : ITransformable3D<T>
        => self.Transform(axisAngle);

    public static T Rotate<T>(this ITransformable3D<T> self, Angle yaw, Angle pitch, Angle roll)
        where T : ITransformable3D<T>
        => self.RotateZ(yaw).RotateX(pitch).RotateY(roll);

    public static T RotateX<T>(this ITransformable3D<T> self, Angle angle) where T : ITransformable3D<T>
        => self.Transform(angle.RotateX);

    public static T RotateY<T>(this ITransformable3D<T> self, Angle angle) where T : ITransformable3D<T>
        => self.Transform(angle.RotateY);
    
    public static T RotateZ<T>(this ITransformable3D<T> self, Angle angle) where T : ITransformable3D<T>
        => self.Transform(angle.RotateZ);
}