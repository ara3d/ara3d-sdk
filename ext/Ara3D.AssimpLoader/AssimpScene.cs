using Ara3D.Geometry;
using Ara3D.Models;
using Assimp;
using Material = Ara3D.Models.Material;
using Matrix4x4 = System.Numerics.Matrix4x4;
using Vector4 = System.Numerics.Vector4;

namespace Ara3D.AssimpLoader
{
    public class AssimpScene 
    {
        public Model3D Model { get; }
        private List<Material> _materials;
        private readonly Model3DBuilder _bldr = new Model3DBuilder();

        public AssimpScene(string filePath)
        {
            var scene = Load(filePath);
            _materials = GetMaterials(scene).ToList();
            var meshes = scene.Meshes.Select(FromAssimp);
            _bldr.Meshes.AddRange(meshes);
            AddNodes(scene);
            Model = _bldr.Build();
        }

        public static AssimpContext Context = new();

        public void AddNodes(Scene assimpScene)
            => AddNodes(assimpScene, assimpScene.RootNode, assimpScene.RootNode.Transform);

        public void AddNodes(Scene assimpScene, Node assimpNode, Matrix4x4 transform)
        {
            foreach (var meshIndex in assimpNode.MeshIndices)
            {
                var name = assimpNode.MeshIndices.Count > 1 
                    ? assimpNode.Name + "_" + meshIndex
                    : assimpNode.Name;
                
                if (meshIndex < 0 && meshIndex >= _bldr.Meshes.Count)
                    throw new Exception("Invalid mesh index found");
                
                var assimpMesh = assimpScene.Meshes[meshIndex];
                var matIndex = assimpMesh.MaterialIndex;
                _bldr.AddInstance(meshIndex, transform, _materials[matIndex]);
            }

            foreach (var child in assimpNode.Children)
                AddNodes(assimpScene, child, transform * child.Transform);
        }

        public static IEnumerable<Material> GetMaterials(Scene scene)
            => scene.MaterialCount == 0 ? [] : scene.Materials.Select(FromAssimp);

        public static Scene Load(string filePath)
            => Context.ImportFile(filePath, PostProcessSteps.Triangulate);

        public static bool CanLoad(string filePath)
            => Context.IsImportFormatSupported(Path.GetExtension(filePath));
        
        public static Material FromAssimp(Assimp.Material src)
        {
            // --- diffuse colour -------------------------------------------------
            var diff = src.HasColorDiffuse
                ? src.ColorDiffuse
                : new Vector4(0.5f, 0.5f, 0.5f, 1f);              // sensible default

            var color = new Color(diff.X, diff.Y, diff.Z, diff.W);

            // --- metallic -------------------------------------------------------
            var spec = src.HasColorSpecular
                ? src.ColorSpecular
                : new Vector4(0, 0, 0, 1);

            var diffAvg = (diff.X + diff.Y + diff.Z) / 3f;
            var specAvg = (spec.X + spec.Y + spec.Z) / 3f;
            var metallic = specAvg + diffAvg > 0
                ? Math.Clamp(specAvg / (specAvg + diffAvg), 0f, 1f)
                : 0f;

            // --- roughness ------------------------------------------------------
            var shininess = src.HasShininess ? src.Shininess : 0f;
            var strength = src.HasShininessStrength ? src.ShininessStrength : 1f;

            var roughness = MathF.Sqrt(2f / (2f + shininess * strength));
            roughness = Math.Clamp(roughness, 0.04f, 1f);             // stay within sensible limits

            // --- pack & return --------------------------------------------------
            return new Material(color, metallic, roughness);
        }

        public static TriangleMesh3D FromAssimp(Mesh mesh)
        {
            var vertices = new List<Point3D>();
            var indices = new List<Integer3>();
            foreach (var v in mesh.Vertices)
            {
                var p = (v.X, v.Y, v.Z);
                vertices.Add(p);
            }
            if (!mesh.HasFaces)
            {
                // When there are no faces we assume that the vertices describe triangles (like an STL)
                // Possibly this is a point cloud or something else, but we don't handle those at the current time. 
                if (vertices.Count % 3 != 0)
                    throw new Exception($"Number of vertices {vertices.Count} is not divisible by 3");
                for (var i = 0; i < vertices.Count; i += 3)
                    indices.Add((i, i + 1, i + 2));
            }
            else
            {
                foreach (var f in mesh.Faces)
                {
                    if (!f.HasIndices)
                        throw new Exception("Face has no indices");
                    if (f.IndexCount != 3)
                        throw new Exception("Face is not triangle");
                    var fi = (f.Indices[0], f.Indices[1], f.Indices[2]);
                    indices.Add(fi);
                }
            }
            return new TriangleMesh3D(vertices, indices);
        }
    }
}
