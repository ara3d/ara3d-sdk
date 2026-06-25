using Ara3D.IO.PLY;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.SDK.GeometryTests
{
    public static class PlyLoaderTests
    {
        public static DirectoryPath DataFolder => PathUtil.GetCallerSourceFolder().RelativeFolder("..", "..", "data");

        public static FilePath Lucy => DataFolder.RelativeFile("Lucy100k.ply");
        public static FilePath BunZipper => DataFolder.RelativeFile("bun_zipper.ply");
        public static FilePath ColoredDolphins => DataFolder.RelativeFile("dolphins_colored.ply");
        public static FilePath Dolphins => DataFolder.RelativeFile("dolphins.ply");
        public static FilePath Wuson => DataFolder.RelativeFile("Wuson.ply");

        public static FilePath[] Files = [Lucy, BunZipper, ColoredDolphins, Dolphins, Wuson];

        [Test, Category("Slow"), TestCaseSource(nameof(Files))]
        public static void TestLoadFile(FilePath filePath)
        {
            var logger = Logger.Console;
            logger.Log($"Loading {filePath.GetFileName()}");
            var mesh = PlyImporter.LoadMesh(filePath);
            logger.Log($"Loaded # points = {mesh.Points.Count}, # triangles = {mesh.FaceIndices.Count}");
        }
    }
}
