using Ara3D.Geometry;

namespace Ara3D.Studio;

public readonly record struct CameraState
{
    public CameraState(Angle yaw, Angle pitch, Vector3 position)
    {
        Yaw = yaw.Normalize();
        Pitch = pitch.Normalize();
        Position = position;
    }

    public Angle Yaw { get; }
    public Angle Pitch { get; }
    public Vector3 Position { get; }

    public bool AlmostEqual(CameraState cameraState)
    {
        if (!Yaw.AlmostEqual(cameraState.Yaw)) return false;
        if (!Pitch.AlmostEqual(cameraState.Pitch)) return false;
        if (!Position.AlmostEqual(cameraState.Position)) return false;
        return true;
    }

    public static Vector3 Up = new(0, 0, 1);

    public Matrix4x4 ViewMatrix
        => Matrix4x4.CreateLookAt(Position, Position + Forward, Up);

    public Vector3 Forward
        => new Vector3(
                Pitch.Cos * (-Yaw).Cos,
                Pitch.Cos * (-Yaw).Sin,
                Pitch.Sin).Normalize();

    public Vector3 Right 
        => Forward.NormalizedCross(Up);

    public CameraState WithYawPitch(Angle yaw, Angle pitch)
        => new(yaw, pitch, Position);

    public CameraState AddYawPitch(Angle yaw, Angle pitch)
        => WithYawPitch(Yaw + yaw, Pitch + pitch);

    public CameraState WithPosition(Vector3 position)
        => new(Yaw, Pitch, position);

    public CameraState Translate(Vector3 translation)
        => WithPosition(Position + translation);

    public CameraState WithTarget(Vector3 target)
    {
        var dir = (target - Position).Normalize;
        var pitch = MathF.Asin(Math.Clamp(dir.Z, -1f, 1f));
        var yaw = MathF.Atan2(dir.Y, dir.X);
        if (yaw < 0) yaw += 1.Turns();
        return WithYawPitch(yaw, pitch);
    }

    public CameraState Lerp(CameraState other, float t)
        => new(Yaw.AngularLerp(other.Yaw, t), 
            Pitch.Lerp(other.Pitch, t),
            Position.Lerp(other.Position, t));

    public CameraState RotateAroundPoint(Vector2 pt, Angle a)
        => WithPosition(Matrix4x4.CreateFromAxisAngleWithPivot(Vector3.UnitZ, a, (Position - pt.Vector3))
            .Transform(Position));
}