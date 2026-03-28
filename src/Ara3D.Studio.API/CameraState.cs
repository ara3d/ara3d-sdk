using Ara3D.Geometry;
using System;
using Matrix4x4 = System.Numerics.Matrix4x4;
using Vector3 = System.Numerics.Vector3;

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
        => Vector3.Normalize(Vector3.Cross(Forward, Up));

    public CameraState SetYawPitch(Angle yaw, Angle pitch)
        => new(yaw, pitch, Position);

    public CameraState AddYawPitch(Angle yaw, Angle pitch)
        => SetYawPitch(Yaw + yaw, Pitch + pitch);

    public CameraState SetPosition(Vector3 position)
        => new(Yaw, Pitch, position);

    public CameraState Translate(Vector3 translation)
        => SetPosition(Position + translation);

    public CameraState SetTarget(Vector3 target)
    {
        // Direction from the camera to the point of interest
        var dir = Vector3.Normalize(target - Position);

        // --- Pitch (rotation around Right axis) ------------------------------
        // sin(pitch) = z‑component of the forward vector
        var pitch = MathF.Asin(Math.Clamp(dir.Z, -1f, 1f));
        
        // --- Yaw (rotation around Up axis) -----------------------------------
        // Forward.X =  cos(pitch) * cos(-yaw)
        // Forward.Y =  cos(pitch) * sin(-yaw)
        var yaw = -MathF.Atan2(dir.Y, dir.X);
        
        // Keep yaw in [0, 360) simply for convenience
        if (yaw < 0) yaw += 1.Turns();

        return SetYawPitch(yaw, pitch);
    }

    public CameraState Lerp(CameraState other, float t)
        => new(Yaw.AngularLerp(other.Yaw, t), 
            Pitch.Lerp(other.Pitch, t),
            Position.Lerp(other.Position, t));
}