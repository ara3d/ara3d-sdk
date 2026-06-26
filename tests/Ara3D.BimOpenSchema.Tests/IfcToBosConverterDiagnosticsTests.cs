using Ara3D.BimOpenSchema;
using Ara3D.BimOpenSchema.IO;
using Ara3D.BIMOpenSchema.Tests;
using Ara3D.IfcLoader;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.BimOpenSchema.Tests;

[TestFixture]
public static class IfcToBosConverterDiagnosticsTests
{
    public sealed record IfcSample(string FileName, bool ExpectLoadSuccess);

    public static DirectoryPath DataFolder => PathUtil.GetCallerSourceFolder().RelativeFolder("..", "..", "data");

    public static DirectoryPath OutputFolder => DataFolder.RelativeFolder("output", "ifc-diagnostics");

    public static IfcSample[] Samples =>
    [
        new("model_0.ifc", true),
        new("AC20-Institute-Var-2.ifc", true),
        new("schependomlaan.ifc", true),
        new("AC20-FZK-Haus.ifc", false),
    ];

    public sealed record ConversionSnapshot(
        string FileName,
        long InputBytes,
        string Schema,
        int TotalIfcEntities,
        int BosCandidateEntities,
        int CategoryCount,
        int TypeCount,
        int MappedIfcIds,
        int BosEntityCount,
        int InstanceEntityCount,
        int RelationCount,
        int ParameterCount,
        int DescriptorCount,
        int PropDataErrors,
        int MeshCount,
        int GeometryInstanceCount,
        int VertexCount,
        int FaceCount);

    [Test]
    public static void IfcSampleFilesExist()
    {
        foreach (var sample in Samples)
        {
            var path = DataFolder.RelativeFile(sample.FileName);
            Assert.That(path.Exists(), Is.True, $"Missing IFC sample: {path}");
        }
    }

    [TestCaseSource(nameof(Samples))]
    [Category("Slow")]
    public static void LoadIfcSample(IfcSample sample)
    {
        var logger = Logger.Console;
        var input = DataFolder.RelativeFile(sample.FileName);
        Assert.That(input.Exists(), Is.True);

        logger.Log($"=== IFC load diagnostics: {sample.FileName} ({input.GetFileSizeAsString()}) ===");

        IfcFileStats stats;
        using (logger.LogDuration($"load IFC data {sample.FileName}"))
            stats = IfcFileStats.Load(input, loadGeometry: false, logger);

        if (sample.ExpectLoadSuccess)
        {
            Assert.That(stats.Success, Is.True, stats.ErrorMessage);
            logger.Log($"Schema: {stats.File!.SchemaEnum}");
            logger.Log($"Entities: {stats.File.EntityResolver.EntityLookup.Count:N0}");
            logger.Log($"Load time: {stats.LoadTime.TotalMilliseconds:N0} msec");
            stats.File.Dispose();
        }
        else
        {
            Assert.That(stats.Success, Is.False, "Expected IFC load to fail for this sample");
            logger.LogWarning($"Load failed as expected: {stats.ErrorMessage}");
        }
    }

    public static IEnumerable<IfcSample> ConvertibleSamples()
        => Samples.Where(s => s.ExpectLoadSuccess);

    [TestCaseSource(nameof(ConvertibleSamples))]
    [Category("Slow")]
    public static void ConvertIfcSample(IfcSample sample)
    {
        var logger = Logger.Console;
        var input = DataFolder.RelativeFile(sample.FileName);
        Assert.That(input.Exists(), Is.True);

        logger.Log($"=== IFC conversion diagnostics: {sample.FileName} ({input.GetFileSizeAsString()}) ===");

        IfcToBosConverter? converter = null;
        ConversionSnapshot snapshot;
        FilePath bosPath;

        using (logger.LogDuration("convert IFC to BOS"))
            converter = new IfcToBosConverter(input, logger);

        snapshot = CaptureSnapshot(converter, input);
        OutputSnapshot(logger, snapshot);

        bosPath = OutputFolder.RelativeFile(Path.GetFileNameWithoutExtension(sample.FileName) + ".bos");
        bosPath.GetDirectory().Create();

        using (logger.LogDuration("save BOS parquet zip"))
            converter.SaveToBos(bosPath, logger);

        logger.Log($"Wrote {bosPath.GetFileSizeAsString()} to {bosPath.GetFileName()}");

        IBimData roundTripData;
        BimGeometry roundTripGeometry;
        using (logger.LogDuration("read BOS data"))
            roundTripData = bosPath.ReadBimDataFromParquetZip();

        using (logger.LogDuration("read BOS geometry"))
            roundTripGeometry = bosPath.ReadBimGeometryFromParquetZip();

        roundTripData.OutputSummary(logger);
        roundTripGeometry.OutputBimGeometryCounts(logger);
        OutputRelationBreakdown(roundTripData, logger);
        OutputTopCategories(converter, logger);

        AssertConversion(converter, snapshot, roundTripData, roundTripGeometry);

        converter.IfcFile.Dispose();
    }

    static ConversionSnapshot CaptureSnapshot(IfcToBosConverter converter, FilePath input)
    {
        var bimData = converter.BimDataBuilder.Build();
        var geometry = converter.BimGeometry;

        return new ConversionSnapshot(
            input.GetFileName(),
            input.GetFileSize(),
            converter.IfcFile.SchemaEnum.ToString(),
            converter.IfcFile.EntityResolver.EntityLookup.Count,
            converter.BosEntities.Count,
            converter.CatEntities.Count,
            converter.TypeRelations.TypeIds.Count,
            converter.IfcIdToBosId.Count,
            bimData.Entities.Length,
            bimData.Entities.Count(e => e.Type >= 0),
            bimData.Relations.Length,
            bimData.Parameters.Length,
            bimData.Descriptors.Length,
            converter.PropData.Errors.Count,
            geometry.GetNumMeshes(),
            geometry.GetNumInstances(),
            geometry.GetNumVertices(),
            geometry.GetNumFaces());
    }

    static void OutputSnapshot(ILogger logger, ConversionSnapshot s)
    {
        logger.Log($"Schema: {s.Schema}");
        logger.Log($"IFC entities: {s.TotalIfcEntities:N0}");
        logger.Log($"BOS candidate IFC entities: {s.BosCandidateEntities:N0}");
        logger.Log($"Categories: {s.CategoryCount:N0}");
        logger.Log($"Types: {s.TypeCount:N0}");
        logger.Log($"Mapped IFC ids: {s.MappedIfcIds:N0}");
        logger.Log($"BOS entities: {s.BosEntityCount:N0} ({s.InstanceEntityCount:N0} instances)");
        logger.Log($"Relations: {s.RelationCount:N0}");
        logger.Log($"Parameters: {s.ParameterCount:N0} ({s.DescriptorCount:N0} descriptors)");
        logger.Log($"Property extraction errors: {s.PropDataErrors:N0}");
        logger.Log($"Geometry: {s.MeshCount:N0} meshes, {s.GeometryInstanceCount:N0} instances, {s.VertexCount:N0} vertices, {s.FaceCount:N0} faces");
    }

    static void OutputRelationBreakdown(IBimData bimData, ILogger logger)
    {
        logger.Log("Relations by type:");
        foreach (var (type, count) in bimData.Relations
                     .GroupBy(r => r.RelationType)
                     .Select(g => (g.Key, count: g.Count()))
                     .OrderByDescending(x => x.count))
            logger.Log($"  {type}: {count:N0}");
    }

    static void OutputTopCategories(IfcToBosConverter converter, ILogger logger)
    {
        logger.Log("Top instance categories:");
        var catNames = converter.CatEntities.ToDictionary(kv => kv.Value, kv => kv.Key);
        var counts = converter.BosEntities
            .Select(e => converter.GetBosEntityIndexFromIfc(e.Id))
            .Where(i => i >= 0)
            .Select(i => converter.BimDataBuilder.Entities[(int)i])
            .GroupBy(e => catNames.GetValueOrDefault(e.Category, "_unknown_"))
            .Select(g => (g.Key, count: g.Count()))
            .OrderByDescending(x => x.count)
            .Take(15);

        foreach (var (name, count) in counts)
            logger.Log($"  {name}: {count:N0}");
    }

    static void AssertConversion(
        IfcToBosConverter converter,
        ConversionSnapshot snapshot,
        IBimData roundTrip,
        BimGeometry roundTripGeometry)
    {
        Assert.That(snapshot.BosCandidateEntities, Is.GreaterThan(0), "Expected convertible IFC entities");
        Assert.That(snapshot.MappedIfcIds, Is.GreaterThan(0), "Expected mapped IFC ids");
        Assert.That(snapshot.InstanceEntityCount, Is.GreaterThan(0), "Expected BOS instance entities");
        Assert.That(snapshot.MeshCount, Is.GreaterThan(0), "Expected tessellated meshes");
        Assert.That(snapshot.GeometryInstanceCount, Is.GreaterThan(0), "Expected geometry instances");
        Assert.That(snapshot.RelationCount, Is.GreaterThan(0), "Expected structural relations");

        Assert.That(
            converter.BosEntities.Any(e => e.GetEntityName().StartsWith("IFCREL", StringComparison.Ordinal)),
            Is.False,
            "BOS candidates must not include IFC relationship entities");

        Assert.That(
            roundTrip.Entities.Length,
            Is.EqualTo(snapshot.BosEntityCount),
            "Round-trip entity count should match converted data");

        Assert.That(
            roundTrip.Relations.Length,
            Is.EqualTo(snapshot.RelationCount),
            "Round-trip relation count should match converted data");

        Assert.That(
            roundTripGeometry.GetNumMeshes(),
            Is.EqualTo(snapshot.MeshCount),
            "Round-trip mesh count should match converted geometry");

        Assert.That(
            roundTripGeometry.GetNumInstances(),
            Is.EqualTo(snapshot.GeometryInstanceCount),
            "Round-trip instance count should match converted geometry");
    }
}
