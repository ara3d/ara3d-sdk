using Ara3D.DataTable;
using Ara3D.Geometry;
using Ara3D.Memory;
using Ara3D.Collections;

namespace Ara3D.Models
{
    /// <summary>
    /// A model is a collection of elements, meshes, transforms, materials, and meta-data.
    /// Elements are the parts of a model. They may share references to meshes, materials, and transforms.
    /// If multiple elements share a reference to a transform, then they are intended to move together.
    /// </summary>
    public class Model3D : ITransformable3D<Model3D>
    {
        public Model3D(
            IReadOnlyList<TriangleMesh3D> meshes, 
            IReadOnlyList<Material> materials, 
            IReadOnlyList<Matrix4x4> transforms, 
            IReadOnlyList<ElementStruct> elements, 
            IDataSet? dataSet = null)
        {
            Meshes = meshes;
            Materials = materials;
            Transforms = transforms;
            ElementStructs = elements;
            DataSet = dataSet ?? new ReadOnlyDataSet([]);
            Elements = elements.Select(GetElement);     
        }

        public IReadOnlyList<TriangleMesh3D> Meshes { get; }
        public IReadOnlyList<Material> Materials { get; }
        public IReadOnlyList<Matrix4x4> Transforms { get; }
        public IReadOnlyList<ElementStruct> ElementStructs { get; }
        public IReadOnlyList<Element> Elements { get; }
        public IDataSet DataSet { get; }

        public Element GetElement(ElementStruct es)
            => new(
               Meshes[es.MeshIndex],
               Materials[es.MaterialIndex],
               Transforms[es.TransformIndex]);

        public Model3D Transform(Transform3D transform)
            => new(Meshes, Materials, Transforms.Select(t => t * transform).ToList(), ElementStructs, DataSet);

        public static Integer3 Offset(Integer3 self, Integer offset)
            => (self.A + offset, self.B + offset, self.C + offset);

        public TriangleMesh3D ToMesh()
        {
            var points = new UnmanagedList<Point3D>();
            var indices = new UnmanagedList<Integer3>();
            var indexOffset = 0;

            foreach (var node in Elements)
            {
                var mesh = node.Mesh;
                var mat = node.Transform;

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
            return new TriangleMesh3D(points, indices);
        }

        public Model3D WithTransforms(IReadOnlyList<Matrix4x4> transforms)
            => new(Meshes, Materials, transforms, ElementStructs, DataSet);

        public Model3D WithDataSet(IDataSet dataSet)
            => new(Meshes, Materials, Transforms, ElementStructs, dataSet);

        public Model3D ModifyTransforms(Func<Matrix4x4, Matrix4x4> f)
            => WithTransforms(Transforms.Select(f));

        public Point3D NodeCenter
            => Elements.Select(n => n.Transform.Value.Translation).Aggregate(
                    Vector3.Zero, (v, p) => v + (Vector3)p) / Elements.Count;

        public Model3D WithMeshes(IReadOnlyList<TriangleMesh3D> meshes)
            => new(meshes, Materials, Transforms, ElementStructs, DataSet);

        public Model3D ModifyMeshes(Func<TriangleMesh3D, TriangleMesh3D> f)
            => WithMeshes(Meshes.Select(f));

        public static Model3D Create(IEnumerable<Element> elements)
        {
            var bldr = new Model3DBuilder();
            bldr.AddElements(elements);
            return bldr.Build();
        }

        public static Model3D Create(Element e)
            => Create([e]);

        public static Model3D Create(TriangleMesh3D mesh)
            => Create(new Element(mesh));

        public static implicit operator Model3D(Element e)
            => Create([e]);

        public static implicit operator Model3D(TriangleMesh3D m)
            => new Element(m, Material.Default, Matrix4x4.Identity);

        public Model3D AddColumnsToTable(string tableName, IReadOnlyList<IDataColumn> columns)
            => WithDataSet(DataSet.AddColumnsToTable(tableName, columns));

        public Model3D MergeTable(IDataTable table)
            => WithDataSet(DataSet.AddColumnsToTable(table.Name, table.Columns));

        public Model3D WithStructs(IReadOnlyList<ElementStruct> structs)
            => new(Meshes, Materials, Transforms, structs, DataSet);
    }
}