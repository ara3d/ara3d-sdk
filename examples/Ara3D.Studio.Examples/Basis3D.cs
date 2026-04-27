 namespace Ara3D.Studio.Samples;

public readonly record struct Basis3D
    : ITransform3D
{
    public Axes3D Axes { get; }
    public Vector3 X => Axes.X;
    public Vector3 Y => Axes.Y;
    public Vector3 Z => Axes.Z;
    
    public Basis3D(Axes3D axes)
    {
        Axes = axes;
        Validate(axes);
    }

    public static Basis3D Identity 
        = new(Axes3D.Identity);

    public Matrix4x4 Matrix
        => Axes.Matrix;

    public float Determinant
        => Axes.Determinant;

    public bool IsRightHanded
        => Determinant > 0f;

    public OrthonormalBasis3D ToOrthonormalBasis()
        => Axes.ToOrthonormalBasis();

    public static void Validate(Axes3D axes)
    {
        const float eps = 1e-12f;

        if (axes.X.LengthSquared() <= eps)
            throw new ArgumentException("Basis X axis must be non-zero.", nameof(axes));

        if (axes.Y.LengthSquared() <= eps)
            throw new ArgumentException("Basis Y axis must be non-zero.", nameof(axes));

        if (axes.Z.LengthSquared() <= eps)
            throw new ArgumentException("Basis Z axis must be non-zero.", nameof(axes));

        if (Math.Abs(axes.Determinant) <= eps)
            throw new ArgumentException("Basis vectors must be linearly independent.", nameof(axes));
    }
}