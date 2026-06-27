using Ara3D.BimOpenSchema;
using Ara3D.BimOpenSchema.IO;
using System.Text;
using System.Text.Json.Nodes;
using Ara3D.Logging;
using static Ara3D.BimOpenSchema.Tests.IfcToBosConverterDiagnosticsTests;

namespace Ara3D.BimOpenSchema.Tests;

[TestFixture]
public static class IfcRoomJsonTests
{
    const string RoomIfc = """
        ISO-10303-21;
        HEADER;
        FILE_DESCRIPTION(('ViewDefinition'),'2;1');
        FILE_NAME('room-json-test.ifc','2024-01-01T00:00:00',(''),(''),'','','');
        FILE_SCHEMA(('IFC2X3'));
        ENDSEC;
        DATA;
        #10=IFCSITE('site-gid',$,'Site',$,$,$,$,$,.ELEMENT.,$,$,0.,$,$);
        #11=IFCBUILDING('bld-gid',$,'Building',$,$,$,$,$,.ELEMENT.,$,$,$);
        #12=IFCRELAGGREGATES('ra1-gid',$,$,$,#10,(#11));
        #20=IFCBUILDINGSTOREY('stry-gid',$,'Level 1',$,$,$,$,$,.ELEMENT.,0.);
        #21=IFCRELAGGREGATES('ra2-gid',$,$,$,#11,(#20));
        #30=IFCWALL('wall-gid',$,'Wall A',$,$,$,$);
        #40=IFCSPACE('space-gid',$,'Office 101',$,$,$,$,$,.ELEMENT.,$);
        #41=IFCRELCONTAINEDINSPATIALSTRUCTURE('rc1-gid',$,$,$,(#30),#20);
        #42=IFCRELCONTAINEDINSPATIALSTRUCTURE('rc2-gid',$,$,$,(#40),#20);
        #50=IFCPROPERTYSINGLEVALUE('Area',$,.IFCAREAMEASURE.,42.5,$);
        #51=IFCPROPERTYSET('ps-gid',$,'Pset_SpaceCommon',$,(#50));
        #52=IFCRELDEFINESBYPROPERTIES('rd-gid',$,$,$,(#40),#51);
        ENDSEC;
        END-ISO-10303-21;
        """;

    static string WriteTempIfc(string content)
    {
        var path = Path.Combine(Path.GetTempPath(), $"ara3d-room-json-{Guid.NewGuid():N}.ifc");
        File.WriteAllText(path, content, Encoding.ASCII);
        return path;
    }

    public static IfcSample[] Samples => IfcToBosConverterDiagnosticsTests.Samples;

    [TestCaseSource(nameof(Samples))]
    public static void LoadIfcSample(IfcSample sample)
    {
        var path = WriteTempIfc(RoomIfc);
        var logger = Logger.Console;
        IfcToBosConverter? converter = null;
        converter = new IfcToBosConverter(path);
        var bimData = converter.BimDataBuilder.Build();
        var rooms = bimData.ToRoomJsonObjects();
        logger.Log($"Found {rooms.Count} rooms");
        foreach (var room in rooms)
        {
            logger.Log(room.ToString());
        }
    }

    public static void ConvertIfcToRoomJson()
    {
        var path = WriteTempIfc(RoomIfc);
        IfcToBosConverter? converter = null;
        try
        {
            converter = new IfcToBosConverter(path);
            var bimData = converter.BimDataBuilder.Build();

            var rooms = bimData.ToRoomJsonObjects();

            Assert.That(rooms, Has.Count.EqualTo(1));
            var room = rooms[0];

            AssertEntity(room["entity"]!.AsObject(), "Office 101", "space-gid", 40);
            Assert.That(room["entity"]!["category"]!.GetValue<string>(), Is.EqualTo("IFCSPACE"));

            var properties = room["properties"]!.AsObject();
            Assert.That(properties, Is.Not.Empty);
            Assert.That(properties.ContainsKey("Area"), Is.True);

            var relations = room["relations"]!.AsObject();
            var outgoing = relations["outgoing"]!.AsArray();
            var incoming = relations["incoming"]!.AsArray();

            Assert.That(outgoing, Has.Count.EqualTo(1));
            AssertRelation(outgoing[0]!.AsObject(), "ContainedIn", "Level 1", "IFCBUILDINGSTOREY");

            Assert.That(incoming, Has.Count.EqualTo(0));

            Assert.That(room.ToJsonString(), Does.Contain("Office 101"));
            Assert.That(room.ToJsonString(), Does.Contain("ContainedIn"));
        }
        finally
        {
            converter?.IfcFile.Dispose();
            try { File.Delete(path); } catch (IOException) { }
        }
    }

    static void AssertEntity(JsonObject entity, string name, string globalId, long localId)
    {
        Assert.That(entity["name"]!.GetValue<string>(), Is.EqualTo(name));
        Assert.That(entity["globalId"]!.GetValue<string>(), Is.EqualTo(globalId));
        Assert.That(entity["localId"]!.GetValue<long>(), Is.EqualTo(localId));
        Assert.That(entity["id"]!.GetValue<int>(), Is.GreaterThanOrEqualTo(0));
    }

    static void AssertRelation(JsonObject relation, string type, string targetName, string targetCategory)
    {
        Assert.That(relation["type"]!.GetValue<string>(), Is.EqualTo(type));

        var target = relation["entity"]!.AsObject();
        Assert.That(target["name"]!.GetValue<string>(), Is.EqualTo(targetName));
        Assert.That(target["category"]!.GetValue<string>(), Is.EqualTo(targetCategory));
        Assert.That(target["id"]!.GetValue<int>(), Is.GreaterThanOrEqualTo(0));
    }
}
