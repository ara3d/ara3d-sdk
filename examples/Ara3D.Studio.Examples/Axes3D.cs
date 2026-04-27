namespace Ara3D.Studio.Samples;

public readonly record struct Axes3D(Vector3 X, Vector3 Y, Vector3 Z)
{
    public static Axes3D Identity 
        => new(Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ);

    public float Determinant
        => X.Cross(Y).Dot(Z);

    public Axes3D Transpose
        => new((X.X, Y.X, Z.X), (X.Y, Y.Y, Z.Y), (X.Z, Y.Z, Z.Z));

    public Matrix4x4 Matrix => new(
        X.X, X.Y, X.Z, 0f,
        Y.X, Y.Y, Y.Z, 0f,
        Z.X, Z.Y, Z.Z, 0f,
        0f, 0f, 0f, 1f);

    public OrthonormalBasis3D ToOrthonormalBasis()
        => OrthonormalBasis3D.Create(this);
}