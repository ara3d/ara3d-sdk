namespace Ara3D.Geometry;

public readonly record struct OrientedBox3D(Frame3D Frame, Vector3 Size) 
{
    public Matrix4x4 LocalToWorldMatrix()
        => Matrix4x4.CreateScale(Size.X, Size.Y, Size.Z) * Frame.Matrix;

    public Matrix4x4 WorldToLocalMatrix()
        => LocalToWorldMatrix().Invert;
}