using Ara3D.DataTable;
using Plato;
using Ara3D.Memory;
using Plato.Geometry;

namespace Ara3D.Models
{
    public class Model3D : ITransformable3D<Model3D>, IModel3D
    {
        public Model3D(IReadOnlyList<TriangleMesh3D> meshes, IReadOnlyList<Material> materials, IReadOnlyList<Matrix4x4>
            nodeTransforms, IReadOnlyList<string> nodeNames, IReadOnlyList<Node> nodes, IDataTable dataTable)
        {
            Meshes = meshes;
            Materials = materials;
            NodeTransforms = nodeTransforms;
            NodeNames = nodeNames;
            Nodes = nodes;
            DataTable = dataTable;
        }

        public IReadOnlyList<TriangleMesh3D> Meshes { get; }
        public IReadOnlyList<Material> Materials { get; }
        public IReadOnlyList<Matrix4x4> NodeTransforms { get; }
        public IReadOnlyList<string> NodeNames { get; }
        public IReadOnlyList<Node> Nodes { get; }
        public IDataTable DataTable { get; }

        public Model3D Transform(Matrix4x4 matrix)
            => new(Nodes.Select(n => n.Transform(matrix)).ToList(), DataTable);

        public static Integer3 Offset(Integer3 self, Integer offset)
            => (self.A + offset, self.B + offset, self.C + offset);

        public static implicit operator Model3D(TriangleMesh3D mesh)
            => (Model3DNode)mesh;

        public static implicit operator TriangleMesh3D(Model3D model)
            => model.ToMesh();

        public TriangleMesh3D ToMesh()
        {
            var points = new UnmanagedList<Point3D>();
            var indices = new UnmanagedList<Integer3>();
            var indexOffset = 0;

            foreach (var node in Nodes)
            {
                var mesh = node.Mesh;
                var mat = node.Matrix;

                if (!mat.Equals(Matrix4x4.Identity))
                {
                    foreach (var p in mesh.Points)
                        points.Add(mat.Transform(p));
                }
                else
                {
                    // Fast path
                    points.AddRange(mesh.Points);
                }

                if (indexOffset != 0)
                {
                    foreach (var f in mesh.FaceIndices)
                        indices.Add(Offset(f, indexOffset));
                }
                else
                {
                    // Fast path
                    indices.AddRange(mesh.FaceIndices);
                }

                indexOffset = points.Count;
            }

            // TODO: we need  to be able to work more efficiently with buffers 
            return new TriangleMesh3D(points.ToIArray(), indices.ToIArray());
        }

        public Model3D ModifyNodes(Func<Model3DNode, Model3DNode> f)
            => new(Nodes.Select(f).ToList());

        public Model3D ModifyTransforms(Func<Matrix4x4, Matrix4x4> f)
            => new(Nodes.Select(n => n with { Matrix = f(n.Matrix) }).ToList());

        public Point3D CenterOfNodes
            => Nodes.Select(n => n.Matrix.Value.Translation).Aggregate(
                    Vector3.Zero, (v, p) => v + (Vector3)p);

        public Model3D ModifyMeshes(Func<TriangleMesh3D, TriangleMesh3D> f)
        {
            var d = new Dictionary<TriangleMesh3D, TriangleMesh3D>();
            foreach (var n in Nodes)
            {
                if (!d.ContainsKey(n.Mesh))
                    d.Add(n.Mesh, f(n.Mesh));
            }

            return ModifyNodes(n => n with { Mesh = d[n.Mesh] });
        }
     
    }
}