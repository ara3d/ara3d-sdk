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

    public static QuadGrid3D Extrude(this IReadOnlyList<Point3D> points, Vector3 vector, int count = 1)
        => RowsToArray((count + 1).MapRange(i => points.Translate(vector * i))).ToQuadGrid3D(false, false);

    public static QuadGrid3D Extrude(this IReadOnlyList<Point3D> points, Number height, int count = 1)
        => points.Extrude(height * Vector3.UnitZ, count);

    public static QuadGrid3D Extrude(this IReadOnlyList<Point2D> points, Vector3 vector, int count = 1)
        => points.To3D().Extrude(vector, count);

    public static QuadGrid3D Extrude(this IReadOnlyList<Point2D> points, Number height, int count = 1)
        => points.To3D().Extrude(height, count);

    public static QuadGrid3D Extrude(this RegularPolygon polygon, Number height, int count = 1)
        => polygon.Points.To3D().Extrude(height, count);

    public static QuadMesh3D ToQuadMesh3D(this QuadGrid3D grid)
        => new(grid.Points, grid.FaceIndices);

    public static TriangleMesh3D Triangulate(this QuadGrid3D grid)
        => grid.ToQuadMesh3D().Triangulate();

    public static IEnumerable<IReadOnlyList<int>> GetLineGroups(this LineMesh3D lineMesh)
    {
        var faces = lineMesh.FaceIndices;
        if (faces.Count == 0) yield break;
        var prev = faces[0].A;
        var group = new List<int>();
        foreach (var face in faces)
        {
            if (face.A != prev)
            {
                yield return group;
                group = [];
            }

            group.Add(face.A);
            prev = face.B;
        }

        if (group.Count != 0)
            yield return group;
    }

    public static QuadGrid3D ExtrudeLineGroup(this IReadOnlyList<int> indices, IReadOnlyList<Point3D> points, Vector3 dir, int count)
    {
        var newPoints = indices.Map(i => points[i]);
        var connectU = indices[0] == indices[^1];
        return RowsToArray(count.MapRange(i => newPoints.Translate(dir * i))).ToQuadGrid3D(connectU, false);
    }

    public static QuadMesh3D Extrude(this LineMesh3D lineMesh, Vector3 direction, int count)
    {
        var groups = lineMesh.GetLineGroups().ToList();
        var meshes = groups.Map(g => g.ExtrudeLineGroup(lineMesh.Points, direction, count));
        return meshes.ToQuadMesh();
    }

    public static QuadMesh3D ToQuadMesh(this IEnumerable<QuadGrid3D> grids)
    {
        var points = new List<Point3D>();
        var faces = new List<Integer4>();
        var offset = 0;

        foreach (var grid in grids)
        {
            points.AddRange(grid.Points);
            foreach (var face in grid.FaceIndices)
            {
                faces.Add(new Integer4(
                    face.A + offset,
                    face.B + offset,
                    face.C + offset,
                    face.D + offset));
            }
            offset = points.Count;
        }

        return new QuadMesh3D(points, faces);
    }

    public static QuadGrid3D ToQuadGrid3D(IReadOnlyList<Point3D> points, Vector3 v)
        => points.Extrude(v);

}