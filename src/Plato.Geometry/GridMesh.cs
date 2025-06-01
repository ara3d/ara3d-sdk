
namespace Plato.Geometry
{
    /// <summary>
    /// A type of mesh, that has the topology of a grid. Even though
    /// points might not be orthogonal, they have a forward, left, top, and right
    /// neighbour (unless on an edge).
    /// Grids may or may not have edges, for example a cylinder aligned to the Z-axis
    /// would be closed on Y.
    /// A grid mesh, may be created from a parametric surface, but can also
    /// be treated as a parametric surface.
    /// </summary>
    public class GridMesh: IQuadMesh3D<GridMesh> 
    {
        public GridMesh(IArray2D<Point3D> points, bool closedX, bool closedY)
        {
            PointGrid = points;
            ClosedX = closedX;
            ClosedY = closedY;

            var nx = points.NumColumns - (closedX ? 0 : 1);
            var ny = points.NumRows - (closedY ? 0 : 1);
            FaceIndices = ny.Range().CartesianProduct(nx.Range(),
                (col, row) => SurfaceDiscretization.QuadMeshFaceVertices(col, row, points.NumColumns, points.NumRows));
        }

        public IArray2D<Point3D> PointGrid { get; }
        public IArray<Point3D> Points => PointGrid;
        public IArray<Integer4> FaceIndices { get; }
        public bool ClosedX { get; }
        public bool ClosedY { get; }

        public GridMesh Deform(Func<Point3D, Point3D> f) 
            => new(PointGrid.Map(f), ClosedX, ClosedY);

        public Integer PrimitiveSize
            => 4;

        public Integer NumPrimitives
            => FaceIndices.Count;
        
        // TODO:  

        public Bounds3D Bounds => throw new NotImplementedException();
        public IArray<Integer> Indices => throw new NotImplementedException();
        public IArray<Quad3D> Primitives => throw new NotImplementedException();
    }

    public static class DeformableExtensions
    {
        public static T Translate<T>(this T self, Vector3 v) where T : IDeformable3D<T>
            => self.Deform(p => p + v);
    }

    public static class GridMeshExtensions
    {
        public static IArray2D<T> ToArray2D<T>(IArray<T>[] arrays)
        {
            if (arrays.Length == 0)
                return new Array2D<T>(0, 0, (_,_) => default);
            var n = arrays[0].Count;
            if (arrays.Any(xs => xs.Count != n))
                throw new Exception("All arrays must be of the same length");
            return new Array2D<T>(n, arrays.Length, (row, col) => arrays[row][col]);
        }

        public static GridMesh ExtrudeTo(this PointArray3D row1, PointArray3D row2, bool closedX)
            => new(ToArray2D([row1.Points, row2.Points]), closedX, false);

        public static PointArray3D To3D(this Polygon p)
            => p;

        public static GridMesh Extrude(this Polygon polygon, Number n)
            => ExtrudeTo(polygon, polygon.To3D().Translate(Vector3.UnitZ * n), true);
    }
}
