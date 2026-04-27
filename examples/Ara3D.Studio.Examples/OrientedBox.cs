namespace Ara3D.Studio.Samples;

/// <summary>
/// An oriented bounding box represented by center, orthonormal axes, and half lengths.
/// </summary>
public readonly record struct OrientedBox(Frame3D Frame, Vector3 Size)
{
    /// <summary>
    /// Computes a PCA-aligned oriented bounding box of the supplied points.
    /// This is a stable best-fit box, not guaranteed to be the minimum-volume OBB.
    /// </summary>
    public static OrientedBox Fit(IReadOnlyList<Vector3> pts)
    {
        if (pts is null || pts.Count == 0)
            throw new ArgumentException("Point list is empty.", nameof(pts));

        var pca = new PrincipalComponentAnalysis(pts);

        var center0 = pca.Mean;

        var ux = pca.PrincipalAxis;
        var uy = pca.SecondaryAxis;
        var uz = pca.TertiaryAxis;

        Debug.Assert(Math.Abs(Vector3.Dot(ux, uy)) < 1e-5f);
        Debug.Assert(Math.Abs(Vector3.Dot(ux, uz)) < 1e-5f);
        Debug.Assert(Math.Abs(Vector3.Dot(uy, uz)) < 1e-5f);

        Debug.Assert(Math.Abs(ux.LengthSquared() - 1f) < 1e-5f);
        Debug.Assert(Math.Abs(uy.LengthSquared() - 1f) < 1e-5f);
        Debug.Assert(Math.Abs(uz.LengthSquared() - 1f) < 1e-5f);

        float minX = float.PositiveInfinity, maxX = float.NegativeInfinity;
        float minY = float.PositiveInfinity, maxY = float.NegativeInfinity;
        float minZ = float.PositiveInfinity, maxZ = float.NegativeInfinity;

        foreach (var p in pts)
        {
            var d = p - center0;

            var x = Vector3.Dot(d, ux);
            var y = Vector3.Dot(d, uy);
            var z = Vector3.Dot(d, uz);

            if (x < minX) minX = x;
            if (x > maxX) maxX = x;

            if (y < minY) minY = y;
            if (y > maxY) maxY = y;

            if (z < minZ) minZ = z;
            if (z > maxZ) maxZ = z;
        }

        var size = new Vector3(
            maxX - minX,
            maxY - minY,
            maxZ - minZ);

        var center =
            center0
            + ux * ((minX + maxX) * 0.5f)
            + uy * ((minY + maxY) * 0.5f)
            + uz * ((minZ + maxZ) * 0.5f);

        var axes = new Axes3D(ux, uy, uz);
        var frame = new Frame3D(center, axes.ToOrthonormalBasis());

        return new OrientedBox(frame, size);
    }

}