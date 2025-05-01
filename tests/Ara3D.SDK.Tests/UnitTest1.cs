using Ara3D.BFAST;
using Ara3D.Logging;
using Ara3D.Serialization.VIM;
using Ara3D.Utils;
using SharpGLTF.Schema2;
using SharpGLTF.Validation;

namespace Ara3D.SDK.Tests
{
    public static class FileTests
    {
        public static DirectoryPath DataFolder => PathUtil.GetCallerSourceFolder().RelativeFolder("..", "..", "data");

        public static FilePath Residence => DataFolder.RelativeFile("residence.vim");

        [Test]
        public static void DataFiles()
        {
            foreach (var file in DataFolder.GetFiles())
            {
                Console.WriteLine($"File {file} exists");
            }
        }
        
        public static void OutputVimData(SerializableDocument vim)
        {
            foreach (var table in vim.EntityTables)
            {
                Console.WriteLine($"{table.Name} #{table.ColumnNames.Count()} columns");
            }

            var g = vim.Geometry;
            Console.WriteLine($"# meshes = {g.Meshes.Count}");
            Console.WriteLine($"# indices = {g.Indices.Count}");
            Console.WriteLine($"# vertices = {g.Vertices.Count}");
            Console.WriteLine($"# submeshes = {g.SubmeshIndexCount.Count}");
            Console.WriteLine($"# instances = {g.InstanceMeshes.Count}");
        }

        [Test]
        public static void OpenVIM()
        {
            var f = Residence;
            var logger = Logger.Console;
            logger.Log($"Opening {f}");
            var vim = VimSerializer.Deserialize(Residence);
            logger.Log("Loaded VIM");
            OutputVimData(vim);
            logger.Log("Completed test");
        }

        [Test]
        public static void OpenBFast()
        {
            var f = Residence;
            var logger = Logger.Console;
            logger.Log($"Opening {f}");
            var buffers = BFastReader.ReadBuffers(f);
            logger.Log("Loaded BFAST");
            foreach (var (buffer, name) in buffers)
            {
                logger.Log($"Buffer {name} has {buffer.NumBytes} bytes");
            }
        }

        [Test]
        public static void OpenBigGltf()
        {
            var f = @"C:\Users\cdigg\git\3d-format-shootout\data\big\Montreal.glb";
            var logger = Logger.Console;
            logger.Log($"Opening {f}");
            var settings = new ReadSettings() { Validation = ValidationMode.Skip };
            var model = SharpGLTF.Schema2.ModelRoot.Load(f, settings);
            logger.Log($"Finished loading");
            var meshes = model.LogicalMeshes;
            logger.Log($"# meshes = {meshes.Count}");
            
            /*foreach (var mesh in meshes)
            {
                logger.Log($"Mesh {mesh.Name} has {mesh.Primitives.Count} primitives");
            }*/

            var scenes = model.LogicalScenes;
            logger.Log($"# scenes = {scenes.Count}");
            foreach (var scene in scenes)
            {
                logger.Log($"Scene {scene.Name} has {scene.VisualChildren.Count()} visual nodes");
                foreach (var node in scene.VisualChildren)
                {
                    logger.Log($"Node {node.Name} has {node.VisualChildren.Count()} children");

                    foreach (var child in node.VisualChildren)
                    {
                        logger.Log($"Child {child.Name} has {child.VisualChildren.Count()} children");
                    }
                }
            }
            var nodes = model.LogicalNodes;
            logger.Log($"# nodes = {nodes.Count}");
            var materials = model.LogicalMaterials;
            logger.Log($"# materials = {materials.Count}");
            var textures = model.LogicalTextures;
            logger.Log($"# textures = {textures.Count}");
            logger.Log("Completed test");
        }
    }
}