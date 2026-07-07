using Ara3D.BimOpenSchema;
using Ara3D.BimOpenSchema.Harmonizer;
using Ara3D.BimOpenSchema.IO;
using Ara3D.Utils;

namespace Ara3D.BimOpenSchema.Harmonizer.Tests;

public static class HarmonizerTests
{
    public static DirectoryPath DataFolder
        => PathUtil.GetCallerSourceFolder().RelativeFolder("..", "..", "data");

    public static FilePath RevitBosFile => DataFolder.RelativeFile("rac_basic_sample_project-2025.bos");
    public static FilePath IfcFile => DataFolder.RelativeFile("AC20-FZK-Haus.ifc");

    private static BimData? _ifcData;
    private static BimData? _revitData;

    public static BimData IfcData
        => _ifcData ??= new IfcToBosConverter(IfcFile).BimDataBuilder.Build();

    public static BimData RevitData
        => _revitData ??= (BimData)RevitBosFile.ReadBimDataFromParquetZip();

    // =====================
    // Helpers
    // =====================

    public static int FindDescriptor(IBimData data, string name)
    {
        for (var i = 0; i < data.Descriptors.Length; ++i)
            if (data.Strings[(int)data.Descriptors[i].Name] == name)
                return i;
        return -1;
    }

    public static IEnumerable<Parameter> ParametersOf(IBimData data, string descriptorName)
    {
        var di = FindDescriptor(data, descriptorName);
        if (di < 0) yield break;
        foreach (var p in data.Parameters)
            if ((int)p.Descriptor == di)
                yield return p;
    }

    public static IEnumerable<string> StringValuesOf(IBimData data, string descriptorName)
        => ParametersOf(data, descriptorName).Select(p => data.Strings[p.Value]);

    // =====================
    // Unit conversion
    // =====================

    [Test]
    public static void UnitConversionFactors()
    {
        Assert.That(UnitConversion.RevitInternalToSI(1, QuantityKind.Length), Is.EqualTo(0.3048).Within(1e-9));
        Assert.That(UnitConversion.RevitInternalToSI(1, QuantityKind.Area), Is.EqualTo(0.09290304).Within(1e-9));
        Assert.That(UnitConversion.RevitInternalToSI(1, QuantityKind.Volume), Is.EqualTo(0.028316846592).Within(1e-9));
        Assert.That(UnitConversion.RevitInternalToSI(2.5, QuantityKind.None), Is.EqualTo(2.5));
        Assert.That(UnitConversion.ToSI(10.0, QuantityKind.Length, SourceKind.Ifc), Is.EqualTo(10.0));
        var p = UnitConversion.ToSI(new Point(1, 2, 3), SourceKind.Revit);
        Assert.That(p.X, Is.EqualTo(0.3048f).Within(1e-6));
        Assert.That(p.Z, Is.EqualTo(0.9144f).Within(1e-6));
    }

    // =====================
    // Source detection
    // =====================

    [Test, Category("Slow")]
    public static void DetectsIfcSource()
        => Assert.That(BosHarmonizer.DetectSource(IfcData), Is.EqualTo(SourceKind.Ifc));

    [Test, Category("Slow")]
    public static void DetectsRevitSource()
        => Assert.That(BosHarmonizer.DetectSource(RevitData), Is.EqualTo(SourceKind.Revit));

    // =====================
    // IFC pipeline
    // =====================

    [Test, Category("Slow")]
    public static void IfcHarmonizationAddsCanonicalData()
    {
        var input = IfcData;
        var output = BosHarmonizer.Harmonize(input);

        var categories = StringValuesOf(output, "Bos:Category").ToList();
        Assert.That(categories, Is.Not.Empty);
        Assert.That(categories, Does.Contain("Wall"));
        Assert.That(categories, Does.Contain("Space"));

        var numbers = StringValuesOf(output, "Bos:Number").ToList();
        Assert.That(numbers, Is.Not.Empty, "spaces should have gained Bos:Number");

        var levels = ParametersOf(output, "Bos:Level").ToList();
        Assert.That(levels, Is.Not.Empty, "elements should have gained Bos:Level from ContainedIn relations");
        foreach (var p in levels.Take(10))
        {
            var level = output.Entities[p.Value];
            var catName = output.Strings[(int)output.Entities[(int)level.Category].Name];
            Assert.That(catName, Is.EqualTo(BosHarmonizer.BuildingStoreyCategoryName));
        }
    }

    [Test, Category("Slow")]
    public static void HarmonizationIsLossless()
    {
        var input = IfcData;
        var output = BosHarmonizer.Harmonize(input);

        // Every input table is an unmodified prefix of the output table.
        Assert.That(output.Entities.Take(input.Entities.Length), Is.EqualTo(input.Entities));
        Assert.That(output.Documents.Take(input.Documents.Length), Is.EqualTo(input.Documents));
        Assert.That(output.Relations.Take(input.Relations.Length), Is.EqualTo(input.Relations));
        Assert.That(output.Points.Take(input.Points.Length), Is.EqualTo(input.Points));
        Assert.That(output.Numbers.Take(input.Numbers.Length), Is.EqualTo(input.Numbers));

        // Parameters must resolve to the same values (indices are preserved because
        // the builder starts empty, so the structs themselves should be identical).
        Assert.That(output.Parameters.Take(input.Parameters.Length), Is.EqualTo(input.Parameters));

        // Geometry is passed through untouched.
        Assert.That(output.Geometry, Is.SameAs(input.Geometry));
    }

    [Test, Category("Slow")]
    public static void HarmonizationIsIdempotent()
    {
        var once = BosHarmonizer.Harmonize(IfcData);
        var twice = BosHarmonizer.Harmonize(once);
        Assert.That(twice.Parameters.Length, Is.EqualTo(once.Parameters.Length));
        Assert.That(twice.Descriptors.Length, Is.EqualTo(once.Descriptors.Length));
        Assert.That(twice.Entities.Length, Is.EqualTo(once.Entities.Length));
        Assert.That(twice.Diagnostics.Length, Is.EqualTo(once.Diagnostics.Length));
    }

    // =====================
    // Revit pipeline
    // =====================

    [Test, Category("Slow")]
    public static void RevitHarmonizationAddsCanonicalData()
    {
        var input = RevitData;
        var output = BosHarmonizer.Harmonize(input);

        var categories = StringValuesOf(output, "Bos:Category").ToList();
        Assert.That(categories, Is.Not.Empty);
        Assert.That(categories, Does.Contain("Wall"));

        var numbers = StringValuesOf(output, "Bos:Number").ToList();
        Assert.That(numbers, Is.Not.Empty, "rooms should have gained Bos:Number");

        // Room numbers must match the source Rvt:Room:Number values.
        var sourceNumbers = StringValuesOf(input, "Rvt:Room:Number").ToHashSet();
        Assert.That(numbers.ToHashSet(), Is.EquivalentTo(sourceNumbers));
    }

    [Test, Category("Slow")]
    public static void RevitAreasAreConvertedToSquareMeters()
    {
        var input = RevitData;
        var output = BosHarmonizer.Harmonize(input);

        // Find the source "Area" (Dimensions) descriptor and compare a few conversions.
        var areaParams = ParametersOf(output, "Bos:Area").ToList();
        Assert.That(areaParams, Is.Not.Empty, "entities should have gained Bos:Area");

        var srcDi = FindDescriptor(input, "Area");
        Assert.That(srcDi, Is.GreaterThanOrEqualTo(0));

        // Compare per entity: canonical = source * 0.3048^2 (float precision).
        var srcByEntity = input.Parameters
            .Where(p => (int)p.Descriptor == srcDi)
            .GroupBy(p => p.Entity)
            .ToDictionary(g => g.Key, g => input.Numbers[g.First().Value]);

        var checkedCount = 0;
        foreach (var p in areaParams)
        {
            if (!srcByEntity.TryGetValue(p.Entity, out var srcVal))
                continue;
            var canonical = output.Numbers[p.Value];
            Assert.That(canonical, Is.EqualTo(srcVal * 0.09290304f).Within(0.001f * Math.Max(1f, Math.Abs(srcVal))));
            ++checkedCount;
        }
        Assert.That(checkedCount, Is.GreaterThan(0));
    }

    // =====================
    // Diagnostics (prints actual data content; useful when extending mappings)
    // =====================

    [Test, Category("Slow")]
    public static void DumpDataOverview()
    {
        foreach (var (name, data) in new[] { ("IFC", IfcData), ("Revit", RevitData) })
        {
            Console.WriteLine($"===== {name} =====");
            Console.WriteLine($"Source: {BosHarmonizer.DetectSource(data)}");
            Console.WriteLine($"Entities: {data.Entities.Length}, Parameters: {data.Parameters.Length}, Descriptors: {data.Descriptors.Length}");

            var catCounts = data.Entities
                .Where(e => e.Category >= 0)
                .GroupBy(e => data.Strings[(int)data.Entities[(int)e.Category].Name])
                .OrderByDescending(g => g.Count())
                .Take(25);
            Console.WriteLine("Top categories:");
            foreach (var g in catCounts)
                Console.WriteLine($"  {g.Key}: {g.Count()}");

            Console.WriteLine("Descriptors mentioning Area/Volume/Number/Level/Room (name | group | units | type):");
            foreach (var d in data.Descriptors)
            {
                var n = data.Strings[(int)d.Name];
                if (n.Contains("Area") || n.Contains("Volume") || n.Contains("Number") || n.Contains("Level") || n.Contains("Room"))
                    Console.WriteLine($"  {n} | {data.Strings[(int)d.Group]} | {data.Strings[(int)d.Units]} | {d.Type}");
            }

            Console.WriteLine($"Params with 'Rvt:Room:Number' descriptor: {ParametersOf(data, "Rvt:Room:Number").Count()}");
            Console.WriteLine($"Params with 'Area' descriptor: {ParametersOf(data, "Area").Count()}");
            Console.WriteLine("First 10 parameters (descriptor name | type | entity):");
            foreach (var p in data.Parameters.Take(10))
            {
                var d = data.Descriptors[(int)p.Descriptor];
                Console.WriteLine($"  {data.Strings[(int)d.Name]} | {d.Type} | {p.Entity}");
            }
            var descUsage = data.Parameters.GroupBy(p => (int)p.Descriptor).Count();
            Console.WriteLine($"Distinct descriptors used by parameters: {descUsage} of {data.Descriptors.Length}");
            var maxDesc = data.Parameters.Length == 0 ? -1 : data.Parameters.Max(p => (int)p.Descriptor);
            Console.WriteLine($"Max descriptor index referenced: {maxDesc}");
        }
    }
}
