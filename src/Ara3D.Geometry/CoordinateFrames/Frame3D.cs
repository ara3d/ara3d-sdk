namespace Ara3D.Geometry;

/// <summary>
/// A rigid 3D coordinate frame represented by a world-space origin and
/// an orthonormal basis.
/// </summary>
/// <remarks>
/// Assumptions:
/// - Basis.X, Basis.Y, and Basis.Z are unit length and mutually perpendicular.
/// - Basis vectors are expressed in world coordinates.
/// - Local coordinates are interpreted as coefficients along the basis vectors.
/// - ToWorld maps a local point to world space:
///     world = Origin + X * local.X + Y * local.Y + Z * local.Z
/// - ToLocal maps a world point into this frame using dot products:
///     local = (dot(world - Origin, X), dot(..., Y), dot(..., Z))
/// - Direction transforms ignore Origin.
/// - Compose(other) means "apply other in this frame", equivalent to:
///     this.ToWorld(other.ToWorld(local))
/// - Matrix/RotationMatrix layout must agree with Basis.Matrix and
///   System.Numerics.Matrix4x4 conventions.
/// </remarks>
public readonly record struct Frame3D(
    Point3D Origin,
    OrthonormalBasis3D Basis) : IRigidTransform3D
{
    public Matrix4x4 RotationMatrix
        => Basis.Matrix;

    public Matrix4x4 Matrix
        => RotationMatrix.WithTranslation(Origin);

    public Quaternion Rotation
        => Quaternion.CreateFromRotationMatrix(RotationMatrix);
    
    public Vector3 X => Basis.X;
    public Vector3 Y => Basis.Y;
    public Vector3 Z => Basis.Z;

    public Vector3 ToLocal(Vector3 worldPoint)
        => ToLocalDirection(worldPoint - Origin.Vector3);
    
    public Vector3 ToWorld(Vector3 localPoint)
        => Origin.Vector3
         + X * localPoint.X
         + Y * localPoint.Y
         + Z * localPoint.Z;

    public Point3D ToLocal(Point3D worldPoint)
        => ToLocal(worldPoint.Vector3);

    public Point3D ToWorld(Point3D localPoint)
        => ToWorld(localPoint.Vector3);

    public Vector3 ToLocalDirection(Vector3 d)
        => (d.Dot(X), d.Dot(Y),d.Dot(Z));

    public Vector3 ToWorldDirection(Vector3 dir)
        => X * dir.X + Y * dir.Y + Z * dir.Z;
    
    public Frame3D WithOrigin(Vector3 origin)
        => new(origin, Basis);

    public Frame3D WithBasis(OrthonormalBasis3D basis)
        => new(Origin, basis);

    public Frame3D Translate(Vector3 delta)
        => new(Origin + delta, Basis);
    
    public Frame3D Inverse
    {
        get
        {
            var invBasis = Basis.Inverse;
            var invOrigin = -invBasis.ToWorldDirection(Origin.Vector3);
            return new Frame3D(invOrigin, invBasis);
        }
    }

    public Frame3D Compose(Frame3D other)
        => new(ToWorld(other.Origin), Basis.Compose(other.Basis));

    public Vector3 GetAxis(int n)
        => n switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            _ => throw new ArgumentOutOfRangeException(nameof(n), "Axis index must be 0, 1, or 2.")
        };
}