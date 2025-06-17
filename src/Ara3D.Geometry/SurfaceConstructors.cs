using Ara3D.Collections;
using System.Collections.Generic;

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

    public static QuadGrid3D ToQuadGrid3D(this IReadOnlyList2D<Point3D> pointGrid, bool connectU, bool connectV)
        => new(pointGrid, connectU, connectV);

    public static QuadGrid3D ToQuadGrid3D(this IReadOnlyList<Point3D> bottomRow, IReadOnlyList<Point3D> topRow, bool connectU, bool connectV)
        => ToQuadGrid3D(RowsToArray([bottomRow, topRow]), connectU, connectV);

    public static IReadOnlyList<Point3D> To3D(this IReadOnlyList<Point2D> points)
        => points.Map(p => p.To3D());

    public static IReadOnlyList<Point3D> Transform(this IReadOnlyList<Point3D> points, Transform3D transform)
        => points.Map(p => p.Transform(transform));

    public static IReadOnlyList<Point3D> Rotate(this IReadOnlyList<Point3D> points, Rotation3D rotation)
        => points.Transform(rotation);

    public static Angle FractionOfTurn(Number numerator, Number denominator)
        => (numerator / denominator).Turns;

    public static Rotation3D FractionalTurnAround(Vector3 axis, Number numerator, Number denominator)
        => Quaternion.CreateFromAxisAngle(axis, FractionOfTurn(numerator, denominator));

    public static QuadGrid3D Sweep(this IReadOnlyList<Point3D> points, IReadOnlyList<Transform3D> transforms, bool connectU, bool connectV)
        => transforms.Map(t => points.Transform(t)).RowsToArray().ToQuadGrid3D(connectU, connectV);

    public static QuadGrid3D Revolve(this IReadOnlyList<Point3D> points, Vector3 axis, int count)
        => count.MapRange(i => points.Rotate(FractionalTurnAround(axis, i, count)))
            .RowsToArray()
            .ToQuadGrid3D(false, true);

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