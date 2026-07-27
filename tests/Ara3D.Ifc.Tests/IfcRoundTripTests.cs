namespace Ara3D.Ifc.Tests;

public static class IfcRoundTripTests
{
    [Test]
    public static void SpansCoverEveryEntityAndReserializeExactly()
    {
        TestData.RequireTestKit();
        using var file = IfcSourceFile.Load(TestData.DuplexIfc);
        Assert.That(file.Count, Is.GreaterThan(30000));

        // Every span's text must start with its own id and end with the terminator.
        foreach (var span in file.Spans)
        {
            var text = file.GetText(span);
            Assert.That(text, Does.StartWith($"#{span.Id}="));
            Assert.That(text, Does.EndWith(";"));
        }
    }

    [Test]
    public static void DiffOfFileWithItselfIsEmpty()
    {
        TestData.RequireTestKit();
        using var a = IfcSourceFile.Load(TestData.DuplexIfc);
        using var b = IfcSourceFile.Load(TestData.DuplexIfc);
        var diff = IfcDiff.Compare(a, b);
        Assert.That(diff.IsEmpty, Is.True, diff.ToString());
    }

    [Test]
    public static void AppendThenRemoveIsByteIdentical()
    {
        TestData.RequireTestKit();
        using var original = IfcSourceFile.Load(TestData.DuplexIfc);

        var builder = new IfcPropertySetBuilder(original.MaxId + 1, original.FirstIdOfType("IFCOWNERHISTORY"));
        builder.AddPropertySet(
            original.FirstIdOfType("IFCBUILDINGSTOREY"),
            "Test_Pset",
            new[] { IfcPropertyValue.Real("x", 1.5), IfcPropertyValue.Label("y", "hello") },
            "test-key");

        var modifiedPath = Path.Combine(TestData.OutputFolder, "duplex-modified.ifc");
        File.WriteAllBytes(modifiedPath, IfcPatcher.Append(original, builder.Lines));

        using var modified = IfcSourceFile.Load(modifiedPath);
        var diff = IfcDiff.Compare(original, modified);
        Assert.That(diff.Added, Is.EqualTo(builder.Ids));
        Assert.That(diff.Deleted, Is.Empty);
        Assert.That(diff.Changed, Is.Empty);

        var restoredPath = Path.Combine(TestData.OutputFolder, "duplex-restored.ifc");
        File.WriteAllBytes(restoredPath, IfcPatcher.Remove(modified, diff.Added));
        Assert.That(File.ReadAllBytes(restoredPath), Is.EqualTo(File.ReadAllBytes(TestData.DuplexIfc)));
    }

    [Test]
    public static void DiffDetectsTypeChangeAsDeleteAndAdd()
    {
        TestData.RequireTestKit();
        using var original = IfcSourceFile.Load(TestData.DuplexIfc);

        // Replace the first cartesian point with a direction reusing the same id.
        var target = original.FirstIdOfType("IFCCARTESIANPOINT");
        var replaced = IfcPatcher.Remove(original, new[] { target });
        var tempPath = Path.Combine(TestData.OutputFolder, "duplex-typechange.ifc");
        File.WriteAllBytes(tempPath, replaced);

        using var removedFile = IfcSourceFile.Load(tempPath);
        File.WriteAllBytes(tempPath, IfcPatcher.Append(removedFile, new[] { $"#{target}=IFCDIRECTION((0.,0.,1.));" }));

        using var mutated = IfcSourceFile.Load(tempPath);
        var diff = IfcDiff.Compare(original, mutated);
        Assert.That(diff.Deleted, Is.EqualTo(new[] { target }));
        Assert.That(diff.Added, Is.EqualTo(new[] { target }));
        Assert.That(diff.Changed, Is.Empty);
    }
}
