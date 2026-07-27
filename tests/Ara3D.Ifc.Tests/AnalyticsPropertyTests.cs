namespace Ara3D.Ifc.Tests;

public static class AnalyticsPropertyTests
{
    public const string PsetName = "Ara3D_Analytics";

    [Test]
    public static void CsvRowsResolveToDuplexElements()
    {
        TestData.RequireTestKit();
        using var file = IfcSourceFile.Load(TestData.DuplexIfc);
        var rows = AnalyticsCsv.Load(TestData.AnalyticsCsvPath);
        Assert.That(rows, Is.Not.Empty);

        var lookup = file.GlobalIdToEntityId();
        var missing = rows.Where(r => !lookup.ContainsKey(r.GlobalId)).ToList();
        Assert.That(missing, Is.Empty,
            $"{missing.Count} of {rows.Count} rows have no matching element, e.g. {missing.FirstOrDefault().GlobalId}");
    }

    [Test]
    public static void AddAnalyticsPropertiesDiffAndRestore()
    {
        TestData.RequireTestKit();
        using var original = IfcSourceFile.Load(TestData.DuplexIfc);
        var rows = AnalyticsCsv.Load(TestData.AnalyticsCsvPath);
        var lookup = original.GlobalIdToEntityId();

        var builder = new IfcPropertySetBuilder(original.MaxId + 1, original.FirstIdOfType("IFCOWNERHISTORY"));
        foreach (var row in rows)
            builder.AddPropertySet(lookup[row.GlobalId], PsetName, row.ToProperties(), $"{PsetName}:{row.GlobalId}");

        // 4 property values + 1 pset + 1 rel per row.
        Assert.That(builder.Lines, Has.Count.EqualTo(rows.Count * 6));

        var modifiedPath = Path.Combine(TestData.OutputFolder, "duplex-analytics.ifc");
        File.WriteAllBytes(modifiedPath, IfcPatcher.Append(original, builder.Lines));

        using var modified = IfcSourceFile.Load(modifiedPath);
        var diff = IfcDiff.Compare(original, modified);
        Assert.That(diff.Added, Is.EqualTo(builder.Ids));
        Assert.That(diff.Deleted, Is.Empty);
        Assert.That(diff.Changed, Is.Empty);

        // New entities parse as the expected types.
        Assert.That(modified.GetSpan(builder.Ids[0])!.Value.TypeName, Is.EqualTo("IFCPROPERTYSINGLEVALUE"));
        Assert.That(modified.GetSpan(builder.Ids[^1])!.Value.TypeName, Is.EqualTo("IFCRELDEFINESBYPROPERTIES"));

        var restoredPath = Path.Combine(TestData.OutputFolder, "duplex-analytics-restored.ifc");
        File.WriteAllBytes(restoredPath, IfcPatcher.Remove(modified, diff.Added));
        Assert.That(File.ReadAllBytes(restoredPath), Is.EqualTo(File.ReadAllBytes(TestData.DuplexIfc)),
            "Removing the added entities must restore the original file byte-for-byte");
    }
}
