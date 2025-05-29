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
        private IArray<Integer> _indices;
        private IArray<Point3D> _points;

        public GridMesh(IArray2D<Vector3> points, bool closedX, bool closedY)
        {
            Points = points;
            ClosedX = closedX;
            ClosedY = closedY;

            var nx = points.NumColumns - (closedX ? 0 : 1);
            var ny = points.NumRows - (closedY ? 0 : 1);
            FaceIndices = ny.Range().CartesianProduct(nx.Range(),
                (col, row) => SurfaceDiscretization.QuadMeshFaceVertices(col, row, points.NumColumns, points.NumRows));
        }

        public IArray2D<Vector3> Points { get; }
        public IArray<Integer4> FaceIndices { get; }
        public bool ClosedX { get; }
        public bool ClosedY { get; }
        
        //public Int4 GetFaceIndices(int column, int row) => FaceIndices[column + row * Columns];
        //public Quad GetFace(int column, int row) => this.Face(GetFaceIndices(column, row));

        public GridMesh Deform(Func<Vector3, Vector3> f) => new(Points.Map(f), ClosedX, ClosedY);

        public GridMesh Transform(Matrix4x4 mat)
            => Deform(p => p.Transform(mat));

        public Integer PrimitiveSize { get; }
        public Integer NumPrimitives { get; }

        IArray<Integer> IIndexedGeometry.Indices => _indices;

        public GridMesh Deform(Func<Point3D, Point3D> f)
        {
            throw new NotImplementedException();
        }

        public Bounds3D Bounds { get; }

        IArray<Point3D> IPointGeometry3D<GridMesh>.Points => _points;

        public IArray<Quad3D> Primitives { get; }
    }
}
