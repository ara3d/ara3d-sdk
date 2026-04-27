namespace Ara3D.Studio.Samples;

public readonly record struct OrthonormalBasis3D 
    : IRotation3D
{
    public Axes3D Axes { get; }
    public Vector3 X => Axes.X;
    public Vector3 Y => Axes.Y;
    public Vector3 Z => Axes.Z;

    public OrthonormalBasis3D(Axes3D axes)
        : this(axes, false) { }

    public OrthonormalBasis3D(Axes3D axes, bool alreadyValidated)
        => Axes = alreadyValidated ? axes : Create(axes).Axes;

    public static OrthonormalBasis3D Identity 
        = new(Axes3D.Identity, true);

    public static OrthonormalBasis3D Create(Axes3D axes)
    {
        const float eps = 1e-12f;

        var x = axes.X;
        var y = axes.Y;
        var z = axes.Z;

        if (x.LengthSquared() <= eps)
            throw new ArgumentException("X axis must be non-zero.", nameof(x));

        var ux = x.Normalize;

        var uy = y - ux * y.Dot(ux);

        if (uy.LengthSquared() <= eps)
        {
            uy = z - ux * z.Dot(ux);
            if (uy.LengthSquared() <= eps)
                uy = ux.AnyPerpendicular();
        }

        uy = uy.Normalize;
        var uz = ux.NormalizedCross(uy);

        if (uz.Dot(z) < 0f)
        {
            uy = -uy;
            uz = -uz;
        }

        return new OrthonormalBasis3D(new(ux, uy, uz), true);
    }

    public Matrix4x4 Matrix => new(
        X.X, X.Y, X.Z, 0f,
        Y.X, Y.Y, Y.Z, 0f,
        Z.X, Z.Y, Z.Z, 0f,
        0f, 0f, 0f, 1f);

    public Quaternion Quaternion
        => Quaternion.CreateFromRotationMatrix(Matrix);

    // NOTE: transposing an axis, is only an inverse in the context of an orthonormal basis 
    public OrthonormalBasis3D Inverse =>
        new(Axes.Transpose, true);

    public Vector3 ToLocalDirection(Vector3 worldDirection)
        => new(
            worldDirection.Dot(X),
            worldDirection.Dot(Y),
            worldDirection.Dot(Z));

    public Vector3 ToWorldDirection(Vector3 localDirection)
        => X * localDirection.X
           + Y * localDirection.Y
           + Z * localDirection.Z;

    public OrthonormalBasis3D Compose(OrthonormalBasis3D other)
        => new(
            new Axes3D(
                ToWorldDirection(other.X),
                ToWorldDirection(other.Y),
                ToWorldDirection(other.Z)),
            true);
}