using System.Text.Json;
using Ara3D.BimOpenSchema;
using Ara3D.BimOpenSchema.IO;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.BIMOpenSchema.Tests;

public static class BosTests
{
    public static DirectoryPath DataFolder => PathUtil.GetCallerSourceFolder().RelativeFolder("..", "..", "data");

    public static FilePath TestFile => DataFolder.RelativeFile("rac_basic_sample_project-2025.bos");
    
    [Test, Category("Slow")]
    public static void TestLoadBimDataAndBimGeometry()
    {
        var logger = Logger.Console;
        logger.Log("Loading BIM Geometry");
        var bg = TestFile.ReadBimGeometryFromParquetZip();
        logger.Log("Loading BIM Data");
        var bd = TestFile.ReadBimDataFromParquetZip();
        logger.Log("Loaded data");

        logger.Log("Creating BIM object model");
        var bom = new BimObjectModel(bd, true );
        logger.Log("Created object models");

        //bd.OutputDiagnostics(logger);
        bd.OutputSummary(logger);
        bg.OutputBimGeometryCounts(logger);

        /*
        logger.Log("Strings");
        for (var i = 0; i < 20; i++)
        {
            logger.Log($"  {bd.Strings[i]}");
        }
        */

        GroupAndCountBy("ClassName", em => em.ClassName, bom);
        GroupAndCountBy("Category", em => em.Category, bom);
        GroupAndCountBy("CategoryType", em => em.CategoryType, bom);
        GroupAndCountBy("BuiltInCategory", em => em.BuiltInCategory, bom);

        OutputSomeParameters(bom);
    }

    public static void OutputSomeParameters(BimObjectModel bom)
    {
        Console.WriteLine("Some parameters:");
        var i = 0;
        foreach (var p in bom.Entities.Take(20).SelectMany(e => e.ParameterValues))
        {
            Console.WriteLine($"  {i++}. {p.Key} = {p.Value}");
        }
    }

    public static void GroupAndCountBy(string groupType, Func<EntityModel, string> f, BimObjectModel bom)
    {
        Console.WriteLine($"Grouping by {groupType}");

        var keysAndCounts = bom
            .Entities
            .GroupBy(f)
            .Select(g => (g.Key, count: g.Count()))
            .OrderByDescending(pair => pair.count);

        var i = 0;
        foreach (var kc in keysAndCounts)
        {
            Console.WriteLine($"  {i++}. {kc.Key} {kc.count}");
        }
    }

    public static void OutputDiagnostics(this IBimData bd, ILogger logger)
    {
        logger.Log("Diagnostics:");
        foreach (var d in bd.GetDiagnosticStrings())
            logger.Log($"  {d}");
    }

    public static void OutputSummary(this IBimData bd, ILogger logger)
    {
        var json = JsonSerializer.Serialize(
            bd.Manifest,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        Console.WriteLine(json); 
        
        logger.Log($"# documents = {bd.Documents.Length}");
        logger.Log($"# entities = {bd.Entities.Length}");

        logger.Log($"   # instance entities = {bd.Entities.Count(e => e.Type >= 0)}");

        var numCats = bd.Entities.Select(e => e.Category).Distinct().Count(i => i >= 0);
        logger.Log($"   # distinct categories = {numCats}");

        var numTypes = bd.Entities.Select(e => e.Type).Distinct().Count(i => i >= 0);
        logger.Log($"   # distinct types = {numTypes}");

        logger.Log($"# diagnostics = {bd.Diagnostics.Length}");
        logger.Log($"# descriptors = {bd.Descriptors.Length}");
        logger.Log($"# points = {bd.Points.Length}");
        logger.Log($"# numbers = {bd.Numbers.Length}");
        logger.Log($"# string = {bd.Strings.Length}");
        logger.Log($"# relations = {bd.Relations.Length}");
        logger.Log($"# parameters = {bd.GetNumParameters()}");

        logger.Log($" entities with no element {bd.Entities.Count(e => e.LocalId < 0)}");
        /*
        var cats = bd.GetCategoryNames();
        logger.Log($"Categories");
        foreach (var cat in cats)
            logger.Log($"  {cat}");

        var types = bd.GetTypeNames();
        logger.Log($"Types");
        foreach (var type in types)
            logger.Log($"  {type}");
        */
    }

    public static void OutputBimGeometryCounts(this BimGeometry bimGeometry, ILogger logger)
    {
        logger.Log($"# transforms = {bimGeometry.GetNumTransforms()}");
        logger.Log($"# meshes = {bimGeometry.GetNumMeshes()}");
        logger.Log($"# elements = {bimGeometry.GetNumInstances()}");
        logger.Log($"# faces = {bimGeometry.GetNumFaces()}");
        logger.Log($"# vertices = {bimGeometry.GetNumVertices()}");
        logger.Log($"# materials = {bimGeometry.GetNumMaterials()}");
    }
}