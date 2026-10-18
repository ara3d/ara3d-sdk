using Ara3D.IO.StepParser;
using Ara3D.Utils;

namespace Ara3D.SDK.Tests;

public static class StepParserTests
{
    const string Data = """
        ISO-10303-21;
        HEADER;
        FILE_DESCRIPTION(('test'),'2;1');
        FILE_NAME('t.stp','2026-10-17',(''),(''),'','','');
        FILE_SCHEMA(('AUTOMOTIVE_DESIGN'));
        ENDSEC;
        DATA;
        #1=CARTESIAN_POINT('',(0.,0.,0.));
        #2=(LENGTH_UNIT()NAMED_UNIT(*)SI_UNIT(.MILLI.,.METRE.));
        #3 = ( BOUNDED_SURFACE() B_SPLINE_SURFACE(1,1,((#1,#1),(#1,#1)),.UNSPECIFIED.,.F.,.F.,.F.)
           B_SPLINE_SURFACE_WITH_KNOTS((2,2),(2,2),(0.,1.),(0.,1.),.UNSPECIFIED.)
           GEOMETRIC_REPRESENTATION_ITEM() RATIONAL_B_SPLINE_SURFACE(((1.,0.5),(0.5,1.)))
           REPRESENTATION_ITEM('') SURFACE() );
        #4=DIRECTION('',(0.,0.,1.));
        ENDSEC;
        END-ISO-10303-21;
        """;

    static StepDocument Parse()
    {
        var path = Path.Combine(Path.GetTempPath(), $"step-parser-test-{Guid.NewGuid():N}.stp");
        File.WriteAllText(path, Data);
        return new StepDocument(new FilePath(path));
    }

    static string[] Names(StepDocument doc, int index)
        => doc.Definitions[index].PartialEntities(doc).Select(p => p.NameToken.ToString()).ToArray();

    static string Attributes(StepDocument doc, int index, int part)
        => doc.Definitions[index].PartialEntities(doc)[part].AttributesToken.ToString(doc);

    [Test]
    public static void SimpleInstancesHaveOnePartialEntity()
    {
        using var doc = Parse();
        Assert.That(doc.Definitions.Select(d => d.Id), Is.EqualTo(new[] { 1, 2, 3, 4 }));
        Assert.That(doc.Diagnostics, Is.Empty);
        Assert.That(doc.Definitions[0].IsComplex(), Is.False);
        Assert.That(Names(doc, 0), Is.EqualTo(new[] { "CARTESIAN_POINT" }));
        Assert.That(Attributes(doc, 0, 0), Is.EqualTo("('', (0., 0., 0.))"));
        Assert.That(Names(doc, 3), Is.EqualTo(new[] { "DIRECTION" }));
    }

    [Test]
    public static void SiUnitComplexInstance()
    {
        using var doc = Parse();
        Assert.That(doc.Definitions[1].IsComplex(), Is.True);
        Assert.That(Names(doc, 1), Is.EqualTo(new[] { "LENGTH_UNIT", "NAMED_UNIT", "SI_UNIT" }));
        Assert.That(Attributes(doc, 1, 0), Is.EqualTo("()"));
        Assert.That(Attributes(doc, 1, 1), Is.EqualTo("(*)"));
        Assert.That(Attributes(doc, 1, 2), Is.EqualTo("(.MILLI., .METRE.)"));
    }

    [Test]
    public static void RationalBSplineSurfaceComplexInstance()
    {
        using var doc = Parse();
        Assert.That(Names(doc, 2), Is.EqualTo(new[]
        {
            "BOUNDED_SURFACE", "B_SPLINE_SURFACE", "B_SPLINE_SURFACE_WITH_KNOTS", "GEOMETRIC_REPRESENTATION_ITEM",
            "RATIONAL_B_SPLINE_SURFACE", "REPRESENTATION_ITEM", "SURFACE",
        }));
        Assert.That(Attributes(doc, 2, 1), Is.EqualTo("(1, 1, ((#1, #1), (#1, #1)), .UNSPECIFIED., .F., .F., .F.)"));
        Assert.That(Attributes(doc, 2, 4), Is.EqualTo("(((1., 0.5), (0.5, 1.)))"));
        Assert.That(Attributes(doc, 2, 5), Is.EqualTo("('')"));
    }
}
