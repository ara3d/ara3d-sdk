using Ara3D.Collections;

namespace Ara3D.Geometry;

public static class ParametricSurfaceExtensions
{
    public static QuadGrid3D ToQuadGrid(this ParametricSurface surface, int numColumns, int numRows = 0)
    {
        if (numRows <= 0)
            numRows = numColumns;
        var points = new FunctionalReadOnlyList2D<Point3D>(numColumns, numRows, (i, j) =>
        {
            var u = i / (float)(numColumns - 1);
            var v = j / (float)(numRows - 1);
            return surface.Eval(new Vector2(u, v));
        });
        return new QuadGrid3D(points, surface.ClosedU, surface.ClosedV);
    }

    public static Vector2 RemapUV(Vector2 uv, Vector2 startUV, Vector2 extentUV)
        => (uv * extentUV) - startUV;

    public static ParametricSurface SetDomain(this ParametricSurface surface, Vector2 startUV, Vector2 extentUV)
        => new(uv => surface.Eval(RemapUV(uv, startUV, extentUV)), surface.ClosedU, surface.ClosedV);

    public static TriangleMesh3D Triangulate(this ParametricSurface surface, int numColumns, int numRows = 0)
        => surface.ToQuadGrid(numColumns, numRows).Triangulate();

    public static ParametricSurface Repeat(this ParametricSurface surface, float repeatU, float repeatV)
        => new(uv => surface.Eval((repeatU * uv.X, repeatV * uv.Y)), surface.ClosedU, surface.ClosedV);

}