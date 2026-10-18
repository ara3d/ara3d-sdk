using Ara3D.IO.StepParser;
using Ara3D.Utils;

namespace Ara3D.SDK.Tests;

public static class StepParserDiagnosticsTests
{
    const string Header = "ISO-10303-21;\nHEADER;\nFILE_DESCRIPTION(('d'),'2;1');\nFILE_NAME('n','t',(''),(''),'p','o','');\nFILE_SCHEMA(('IFC4X3_ADD2'));\nENDSEC;\n";

    static StepDocument ParseText(string text)
    {
        var path = Path.Combine(Path.GetTempPath(), $"step-parser-test-{Guid.NewGuid():N}.stp");
        File.WriteAllText(path, text);
        return new StepDocument(new FilePath(path));
    }

    static StepDocument ParseFixture(string name)
        => new(new FilePath(Path.Combine(TestContext.CurrentContext.TestDirectory, "StepFixtures", name)));

    static int[] Ids(StepDocument doc)
        => doc.Definitions.Select(d => d.Id).ToArray();

    [Test]
    public static void CompleteFileHasNoDiagnostics()
    {
        using var doc = ParseFixture("unit-cube.stp");
        Assert.That(doc.Diagnostics, Is.Empty);
        Assert.That(Ids(doc).Order(), Is.EqualTo(Enumerable.Range(1, 142)));
    }

    [Test]
    public static void TruncatedFileKeepsEveryCompleteDefinition()
    {
        using var doc = ParseFixture("unit-cube-truncated.stp");
        Assert.That(Ids(doc).Order(), Is.EqualTo(Enumerable.Range(1, 124)));
        Assert.That(doc.Diagnostics, Is.EqualTo(new[]
        {
            new StepDiagnostic(StepSeverity.Error, StepDiagnosticCode.Syntax, 4232, "')' closing the list", 125, 4198),
        }));
        var last = doc.Definitions[^1].AttributesToken;
        Assert.That(doc.Tokens.Count, Is.EqualTo(last.GetListEndIndex() + 1), "the unfinished #125 leaves no tokens");
        Assert.That(doc.Definitions[^1].AttributesToken.ToString(doc), Is.EqualTo("('', (#100), #118, .T.)"));
    }

    [Test]
    public static void TruncatedHeaderIsAnError()
    {
        const string text = "ISO-10303-21;\nHEADER;\nFILE_DESCRIPTION(('d'),'2;1');\nFILE_NAME('n";
        using var doc = ParseText(text);
        Assert.That(doc.Diagnostics, Is.EqualTo(new[]
        {
            new StepDiagnostic(StepSeverity.Error, StepDiagnosticCode.Syntax, text.Length, "')' closing the list"),
        }));
        Assert.That(doc.Header.Description, Is.EqualTo(new[] { "d" }));
        Assert.That(doc.Header.ImplementationLevel, Is.EqualTo("2;1"));
        Assert.That(doc.Header.Name, Is.Empty);
        Assert.That(doc.Definitions, Is.Empty);
    }

    [Test]
    public static void MissingDataSectionIsAnError()
    {
        using var doc = ParseText(Header + "END-ISO-10303-21;\n");
        Assert.That(doc.Diagnostics, Is.EqualTo(new[]
        {
            new StepDiagnostic(StepSeverity.Error, StepDiagnosticCode.Syntax, Header.Length + 18, "DATA; opening the DATA section"),
        }));
        Assert.That(doc.Header.OriginatingSystem, Is.EqualTo("o"));
    }

    [Test]
    public static void TextThatIsNotStepIsAnError()
    {
        using var doc = ParseText("  hello;");
        Assert.That(doc.Diagnostics, Is.EqualTo(new[]
        {
            new StepDiagnostic(StepSeverity.Error, StepDiagnosticCode.NotStepFile, 2, "ISO-10303-21; opening the file"),
        }));
        Assert.That(doc.Header.SchemaIdentifiers, Is.Empty);
    }

    [Test]
    public static void MissingHeaderEntityIsAWarning()
    {
        const string text = "ISO-10303-21;\nHEADER;\nFILE_SCHEMA(('IFC4'));\nENDSEC;\nDATA;\n#1=A();\nENDSEC;\nEND-ISO-10303-21;\n";
        using var doc = ParseText(text);
        var after = text.IndexOf("ENDSEC;", StringComparison.Ordinal) + 7;
        Assert.That(doc.Diagnostics, Is.EqualTo(new[]
        {
            new StepDiagnostic(StepSeverity.Warning, StepDiagnosticCode.HeaderIncomplete, after, "FILE_DESCRIPTION in the header"),
            new StepDiagnostic(StepSeverity.Warning, StepDiagnosticCode.HeaderIncomplete, after, "FILE_NAME in the header"),
        }));
        Assert.That(Ids(doc), Is.EqualTo(new[] { 1 }));
        Assert.That(doc.Header.FileSchema, Is.EqualTo("IFC4"));
        Assert.That(doc.Header.FileName, Is.Empty);
    }

    [Test]
    public static void TextBetweenDefinitionsEndsTheSection()
    {
        const string data = "DATA;\n#1=A();\nFOO;\n#2=B();\nENDSEC;\n";
        using var doc = ParseText(Header + data);
        Assert.That(Ids(doc), Is.EqualTo(new[] { 1 }));
        Assert.That(doc.Diagnostics, Is.EqualTo(new[]
        {
            new StepDiagnostic(StepSeverity.Error, StepDiagnosticCode.Syntax, Header.Length + data.IndexOf("FOO", StringComparison.Ordinal),
                "an instance '#n=' or ENDSEC closing the DATA section"),
        }));
    }

    [Test]
    public static void FileCutBetweenDefinitionsLacksEndSec()
    {
        var text = Header + "DATA;\n#1=A();\n#2=B(1);\n";
        using var doc = ParseText(text);
        Assert.That(Ids(doc), Is.EqualTo(new[] { 1, 2 }));
        Assert.That(doc.Diagnostics, Is.EqualTo(new[]
        {
            new StepDiagnostic(StepSeverity.Error, StepDiagnosticCode.Syntax, text.Length, "an instance '#n=' or ENDSEC closing the DATA section"),
        }));
    }

    [Test]
    public static void SemicolonInsideAListIsAnError()
    {
        const string data = "DATA;\n#1=A();\n#2=B(1;\n#3=C();\nENDSEC;\n";
        using var doc = ParseText(Header + data);
        Assert.That(Ids(doc), Is.EqualTo(new[] { 1 }));
        Assert.That(doc.Diagnostics, Is.EqualTo(new[]
        {
            new StepDiagnostic(StepSeverity.Error, StepDiagnosticCode.Syntax, Header.Length + data.IndexOf("1;", StringComparison.Ordinal) + 1,
                "')' closing the list", 2, Header.Length + data.IndexOf("#2", StringComparison.Ordinal)),
        }));
    }

    [Test]
    public static void DefinitionsNeedNoSpaceBetweenThem()
    {
        using var doc = ParseText(Header + "DATA;#1=A();#2=B(#1);#3=C(.T.);ENDSEC;END-ISO-10303-21;");
        Assert.That(Ids(doc), Is.EqualTo(new[] { 1, 2, 3 }));
        Assert.That(doc.Diagnostics, Is.Empty);
    }

    [Test]
    public static void DataSectionMayHaveParameters()
    {
        using var doc = ParseText(Header + "DATA('main',('SCHEMA'));\n#1=A();\nENDSEC;\n");
        Assert.That(Ids(doc), Is.EqualTo(new[] { 1 }));
        Assert.That(doc.Diagnostics, Is.Empty);
        Assert.That(doc.Definitions[0].AttributesToken.Index, Is.EqualTo(0));
    }
}
