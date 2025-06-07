using Ara3D.Collections;

namespace Ara3D.Geometry;

public static class SurfaceConstructors
{
    public static IReadOnlyList2D<T> RowsToArray<T>(this IReadOnlyList<IReadOnlyList<T>> listOfLists)
    {
        if (listOfLists.Count == 0)
            return FunctionalReadOnlyList2D<T>.Default;
        var numColumns = listOfLists[0].Count;
        var numRows = listOfLists.Count;
        if (listOfLists.Any(row => row.Count != numColumns))
            throw new ArgumentException("All rows must have the same number of columns.");
        return new FunctionalReadOnlyList2D<T>(numColumns, numRows, (col, row) => listOfLists[row][col]);
    }

    public static QuadGrid3D ToQuadGrid3D(this IReadOnlyList2D<Point3D> pointGrid, bool connectedX, bool connectedY)
        => new(pointGrid, connectedX, connectedY);

    public static QuadGrid3D ToQuadGrid3D(this IReadOnlyList<Point3D> bottomRow, IReadOnlyList<Point3D> topRow, bool connectX, bool connectY)
        => ToQuadGrid3D(RowsToArray([bottomRow, topRow]), connectX, connectY);

    public static IReadOnlyList<Point3D> To3D(this IReadOnlyList<Point2D> points)
        => points.Map(p => p.To3D());

    public static QuadGrid3D Extrude(this IReadOnlyList<Point3D> points, Vector3 vector)
        => RowsToArray([points, points.Translate(vector)]).ToQuadGrid3D(false, false);

    public static QuadGrid3D Extrude(this IReadOnlyList<Point3D> points, Number height)
        => points.Extrude(height * Vector3.UnitZ);

    public static QuadGrid3D Extrude(this IReadOnlyList<Point2D> points, Number height)
        => points.To3D().Extrude(height);

    public static QuadGrid3D Extrude(this RegularPolygon polygon, Number height)
        => polygon.Points.To3D().Extrude(height);

    public static QuadMesh3D ToQuadMesh3D(this QuadGrid3D grid)
        => new(grid.Points, grid.FaceIndices);

    public static TriangleMesh3D Triangulate(this QuadGrid3D grid)
        => grid.ToQuadMesh3D().Triangulate();
}