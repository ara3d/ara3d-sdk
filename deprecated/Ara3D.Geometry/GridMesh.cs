using Ara3D.Collections;
using Ara3D.Mathematics;
using System;

namespace Ara3D.Geometry
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
    public class GridMesh: PointsGeometry, IQuadMesh, IParametricSurface, 
        IDeformable<GridMesh>
    {
        public GridMesh(IArray2D<Vector3> points, bool closedX, bool closedY)
            : base(points)
        {
            Points = points;
            ClosedX = closedX;
            ClosedY = closedY;

            var nx = points.Columns - (closedX ? 0 : 1);
            var ny = points.Rows - (closedY ? 0 : 1);
            FaceIndices = ny.Range().CartesianProduct(nx.Range(),
                (col, row) => SurfaceDiscretization.QuadMeshFaceVertices(col, row, points.Columns, points.Rows))
                .Evaluate();
        }
        public new IArray2D<Vector3> Points { get; }
        public IArray<Int4> FaceIndices { get; }
        public bool ClosedX { get; }
        public bool ClosedY { get; }
        public IArray<int> Indices => FaceIndices.SelectMany(f => f.ToTuple());
        
        //public Int4 GetFaceIndices(int column, int row) => FaceIndices[column + row * Columns];
        //public Quad GetFace(int column, int row) => this.Face(GetFaceIndices(column, row));

        // TODO: ??
        public Vector3 Eval(Vector2 uv)
        {
            throw new NotImplementedException();
            /*
            Verifier.Assert(Columns >= 2);
            Verifier.Assert(Rows >= 2);
            var (lowerX, amountX) = GeometryUtil.InterpolateArraySize(Columns, uv.X, ClosedX);
            var (lowerY, amountY) = GeometryUtil.InterpolateArraySize(Rows, uv.Y, ClosedY);
            Verifier.Assert(lowerX >= 0);
            Verifier.Assert(lowerY >= 0);
            Verifier.Assert(lowerX < Columns - 1);
            Verifier.Assert(lowerY < Rows - 1);
            // TODO: the math here needs to be validated or different kinds of surfaces. 
            var quad = GetFace(lowerX, lowerY);
            return quad.Eval(((float)amountX, (float)amountY));
            */
        }

        public GridMesh Deform(Func<Vector3, Vector3> f)
            => new GridMesh(Points.Select(f), ClosedX, ClosedY);

        public GridMesh Transform(Matrix4x4 mat)
            => Deform(p => p.Transform(mat));

        IQuadMesh ITransformable<IQuadMesh>.Transform(Matrix4x4 mat)
            => Transform(mat);

        IQuadMesh IDeformable<IQuadMesh>.Deform(Func<Vector3, Vector3> f)
            => Deform(f);
    }
}
