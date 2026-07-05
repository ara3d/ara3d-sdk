using Ara3D.BimOpenSchema.IO;
using Ara3D.Logging;
using static Ara3D.BimOpenSchema.Tests.IfcBosJsonTestHelpers;
using static Ara3D.BimOpenSchema.Tests.IfcToBosConverterDiagnosticsTests;

namespace Ara3D.BimOpenSchema.Tests;

[TestFixture]
public static class IfcToiletJsonTests
{
    const string ToiletIfc = """
        ISO-10303-21;
        HEADER;
        FILE_DESCRIPTION(('ViewDefinition'),'2;1');
        FILE_NAME('toilet-json-test.ifc','2024-01-01T00:00:00',(''),(''),'','','');
        FILE_SCHEMA(('IFC4'));
        ENDSEC;
        DATA;
        #10=IFCSITE('site-gid',$,'Site',$,$,$,$,$,.ELEMENT.,$,$,0.,$,$);
        #11=IFCBUILDING('bld-gid',$,'Building',$,$,$,$,$,.ELEMENT.,$,$,$);
        #12=IFCRELAGGREGATES('ra1-gid',$,$,$,#10,(#11));
        #20=IFCBUILDINGSTOREY('stry-gid',$,'Level 1',$,$,$,$,$,.ELEMENT.,0.);
        #21=IFCRELAGGREGATES('ra2-gid',$,$,$,#11,(#20));
        #30=IFCWALL('wall-gid',$,'Wall A',$,$,$,$);
        #41=IFCRELCONTAINEDINSPATIALSTRUCTURE('rc1-gid',$,$,$,(#30),#20);
        #60=IFCSANITARYTERMINAL('toilet-gid',$,'Water Closet 1',$,$,$,$,$,.TOILETPAN.);
        #61=IFCSANITARYTERMINAL('urinal-gid',$,'Urinal 1',$,$,$,$,$,.URINAL.);
        #62=IFCRELCONTAINEDINSPATIALSTRUCTURE('rc2-gid',$,$,$,(#60,#61),#20);
        ENDSEC;
        END-ISO-10303-21;
        """;

    public static IfcSample[] Samples => IfcToBosConverterDiagnosticsTests.Samples;

    [TestCaseSource(nameof(Samples))]
    [Category("Slow")]
    public static void ParseToilets(IfcSample sample)
    {
        var path = sample.Path;
        var logger = Logger.Console;
        IfcToBosConverter? converter = null;
        converter = new IfcToBosConverter(path);
        var bimData = converter.BimDataBuilder.Build();
        var toilets = bimData.ToToiletJsonObjects();
        logger.Log($"Found {toilets.Count} toilet-category entities in {sample.FileName}");
        foreach (var toilet in toilets)
            logger.Log(toilet.ToString());
    }

    [Test]
    public static void ConvertIfcToToiletJson()
    {
        var path = WriteTempIfc("toilet-json", ToiletIfc);
        IfcToBosConverter? converter = null;
        try
        {
            converter = new IfcToBosConverter(path);
            var bimData = converter.BimDataBuilder.Build();

            var toilets = bimData.ToToiletJsonObjects();

            Assert.That(toilets, Has.Count.EqualTo(2));

            var toiletPan = toilets.First(t =>
                t["entity"]!["globalId"]!.GetValue<string>() == "toilet-gid");
            AssertEntity(toiletPan["entity"]!.AsObject(), "Water Closet 1", "toilet-gid", 60);
            Assert.That(toiletPan["entity"]!["category"]!.GetValue<string>(), Is.EqualTo("IFCSANITARYTERMINAL"));
            Assert.That(toiletPan["properties"]!.AsObject()["Ifc:PredefinedType"]!.GetValue<string>(), Is.EqualTo(".TOILETPAN."));

            var urinal = toilets.First(t =>
                t["entity"]!["globalId"]!.GetValue<string>() == "urinal-gid");
            Assert.That(urinal["properties"]!.AsObject()["Ifc:PredefinedType"]!.GetValue<string>(), Is.EqualTo(".URINAL."));

            var relations = toiletPan["relations"]!.AsObject();
            var outgoing = relations["outgoing"]!.AsArray();
            Assert.That(outgoing, Has.Count.EqualTo(1));
            AssertRelation(outgoing[0]!.AsObject(), "ContainedIn", "Level 1", "IFCBUILDINGSTOREY");

            Assert.That(toilets.Any(t => t["entity"]!["category"]!.GetValue<string>() == "IFCWALL"), Is.False);
        }
        finally
        {
            converter?.IfcFile.Dispose();
            try { File.Delete(path); } catch (IOException) { }
        }
    }
}
