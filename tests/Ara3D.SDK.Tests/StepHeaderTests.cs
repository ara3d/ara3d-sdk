using Ara3D.IO.StepParser;
using Ara3D.Utils;

namespace Ara3D.SDK.Tests;

public static class StepHeaderTests
{
    static StepHeader HeaderOf(string fixture)
    {
        using var doc = new StepDocument(new FilePath(Path.Combine(TestContext.CurrentContext.TestDirectory, "StepFixtures", fixture)));
        return doc.Header;
    }

    [Test]
    public static void EveryFieldAsWritten()
    {
        var header = HeaderOf("unit-cube.stp");
        Assert.That(header.Description, Is.EqualTo(new[] { "unit cube for the Part21File adapter tests" }));
        Assert.That(header.ImplementationLevel, Is.EqualTo("2;1"));
        Assert.That(header.Name, Is.EqualTo("unit-cube.stp"));
        Assert.That(header.TimeStamp, Is.EqualTo("2026-10-17T00:00:00"));
        Assert.That(header.Author, Is.EqualTo(new[] { @"Andr\X2\00E9\X0\ Lefort" }));
        Assert.That(header.Organization, Is.EqualTo(new[] { "BIM Open Toolkit" }));
        Assert.That(header.PreprocessorVersion, Is.EqualTo("hand-written for the Part21File adapter tests"));
        Assert.That(header.OriginatingSystem, Is.EqualTo("none"));
        Assert.That(header.Authorization, Is.Empty);
        Assert.That(header.SchemaIdentifiers, Is.EqualTo(new[] { "AUTOMOTIVE_DESIGN { 1 0 10303 214 1 1 1 1 }" }));
    }

    [Test]
    public static void OlderSummariesAreKept()
    {
        var header = HeaderOf("unit-cube.stp");
        Assert.That(header.FileDescription, Is.EqualTo("unit cube for the Part21File adapter tests | 2;1"));
        Assert.That(header.FileName, Is.EqualTo("unit-cube.stp"));
        Assert.That(header.FileSchema, Is.EqualTo("AUTOMOTIVE_DESIGN { 1 0 10303 214 1 1 1 1 }"));
    }

    [Test]
    public static void IfcSchemaIsShortenedOnlyInTheSummary()
    {
        var path = Path.Combine(Path.GetTempPath(), $"step-header-test-{Guid.NewGuid():N}.ifc");
        File.WriteAllText(path, "ISO-10303-21;\nHEADER;\nFILE_DESCRIPTION(('ViewDefinition [CoordinationView]'),'2;1');\n"
            + "FILE_NAME('it''s.ifc','',(''),(''),'',$,'');\nFILE_SCHEMA(('IFC4X3_ADD2'));\nENDSEC;\nDATA;\nENDSEC;\nEND-ISO-10303-21;\n");
        using var doc = new StepDocument(new FilePath(path));
        Assert.That(doc.Diagnostics, Is.Empty);
        Assert.That(doc.Header.SchemaIdentifiers, Is.EqualTo(new[] { "IFC4X3_ADD2" }));
        Assert.That(doc.Header.FileSchema, Is.EqualTo("IFC4X3"));
        Assert.That(doc.Header.Name, Is.EqualTo("it''s.ifc"));
        Assert.That(doc.Header.FileName, Is.EqualTo("it's.ifc"));
        Assert.That(doc.Header.OriginatingSystem, Is.Empty);
    }
}
