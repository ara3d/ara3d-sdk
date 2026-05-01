namespace Ara3D.Geometry;

public readonly record struct OrientedBox3D(Frame3D Frame, Vector3 Size) 
{
    public Matrix4x4 LocalToWorldMatrix()
        => Matrix4x4.CreateScale(Size.X, Size.Y, Size.Z) * Frame.Matrix;

    public Matrix4x4 WorldToLocalMatrix()
        => LocalToWorldMatrix().Invert;
}

public static class OrientedBox3DExtensions
{
    public static double GetVolume(this OrientedBox3D box)
        => box.Size.X * box.Size.Y * box.Size.Z;

    public static OrientedBox3D FitOrientedBox(this IReadOnlyList<Vector3> pts)
    {
        var pca = new PrincipalComponentAnalysis(pts);
        return FitOrientedBox(pts, pca.Frame);
    }

    public static OrientedBox3D FitOrientedBox(this IReadOnlyList<Point3D> pts)
        => pts.Map(p => p.Vector3).FitOrientedBox();

    public static OrientedBox3D FitOrientedBox(this IReadOnlyList<Vector3> pts, Frame3D frame)
    {
        if (pts == null)
            throw new ArgumentNullException(nameof(pts));
        if (pts.Count == 0)
            throw new ArgumentException("Point list is empty.", nameof(pts));

        var localBounds = BoundsInFrame(pts, frame);

        var localCenter = localBounds.Center;
        var worldCenter = frame.ToWorld(localCenter);

        return new OrientedBox3D(
            frame with { Origin = worldCenter },
            localBounds.Size);
    }

    public static Bounds3D BoundsInFrame(this IReadOnlyList<Vector3> pts, Frame3D frame)
    {
        var b = Bounds3D.Empty;
        foreach (var p in pts)
            b = b.Include(frame.ToLocal(p));
        return b;
    }

}