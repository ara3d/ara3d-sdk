using System.Text;
using Ara3D.BimOpenSchema.IO;
using Ara3D.IfcLoader;
using Ara3D.IO.StepParser;
using Ara3D.Memory;

namespace Ara3D.BimOpenSchema.Tests;

[TestFixture]
public static class IfcRelationsTests
{
    const string MinimalStructuralIfc = """
        ISO-10303-21;
        HEADER;
        FILE_DESCRIPTION(('ViewDefinition'),'2;1');
        FILE_NAME('structural-relations-test.ifc','2024-01-01T00:00:00',(''),(''),'','','');
        FILE_SCHEMA(('IFC2X3'));
        ENDSEC;
        DATA;
        #10=IFCSITE('site-gid',$,'Site',$,$,$,$,$,.ELEMENT.,$,$,0.,$,$);
        #11=IFCBUILDING('bld-gid',$,'Building',$,$,$,$,$,.ELEMENT.,$,$,$);
        #12=IFCRELAGGREGATES('ra1-gid',$,$,$,#10,(#11));
        #20=IFCBUILDINGSTOREY('stry-gid',$,'Storey',$,$,$,$,$,.ELEMENT.,0.);
        #21=IFCRELAGGREGATES('ra2-gid',$,$,$,#11,(#20));
        #30=IFCWALL('wall-gid',$,'Wall',$,$,$,$);
        #31=IFCRELCONTAINEDINSPATIALSTRUCTURE('rc1-gid',$,$,$,(#30),#20);
        #40=IFCELEMENTASSEMBLY('asm-gid',$,'Asm',$,$,$,$,$,$,$);
        #41=IFCMEMBER('mem-gid',$,'Member',$,$,$,$);
        #42=IFCRELNESTS('rn1-gid',$,$,$,#40,(#41));
        ENDSEC;
        END-ISO-10303-21;
        """;

    const string GroupAndProjectIfc = """
        ISO-10303-21;
        HEADER;
        FILE_DESCRIPTION(('ViewDefinition'),'2;1');
        FILE_NAME('group-project-test.ifc','2024-01-01T00:00:00',(''),(''),'','','');
        FILE_SCHEMA(('IFC2X3'));
        ENDSEC;
        DATA;
        #10=IFCGROUP('grp-gid',$,'Group',$,$);
        #20=IFCWALL('wall-gid',$,'Wall',$,$,$,$);
        #21=IFCRELASSIGNSTOGROUP('ag1-gid',$,$,$,(#20),$,#10);
        #30=IFCWALL('host-gid',$,'Host',$,$,$,$);
        #31=IFCPROJECTIONELEMENT('proj-gid',$,'Proj',$,$,$,$,$,$);
        #32=IFCRELPROJECTSELEMENT('pe1-gid',$,$,$,#30,#31);
        ENDSEC;
        END-ISO-10303-21;
        """;

    const string OpeningIfc = """
        ISO-10303-21;
        HEADER;
        FILE_DESCRIPTION(('ViewDefinition'),'2;1');
        FILE_NAME('opening-relations-test.ifc','2024-01-01T00:00:00',(''),(''),'','','');
        FILE_SCHEMA(('IFC2X3'));
        ENDSEC;
        DATA;
        #10=IFCWALL('wall-gid',$,'Wall',$,$,$,$);
        #11=IFCOPENINGELEMENT('open-gid',$,'Opening',$,$,$,$,$);
        #12=IFCRELVOIDSELEMENT('ve1-gid',$,$,$,#10,#11);
        #20=IFCDOOR('door-gid',$,'Door',$,$,$,$,$,$);
        #21=IFCRELFILLSELEMENT('fe1-gid',$,$,$,#11,#20);
        ENDSEC;
        END-ISO-10303-21;
        """;

    const string MaterialLayerSetIfc = """
        ISO-10303-21;
        HEADER;
        FILE_DESCRIPTION(('ViewDefinition'),'2;1');
        FILE_NAME('material-layer-test.ifc','2024-01-01T00:00:00',(''),(''),'','','');
        FILE_SCHEMA(('IFC2X3'));
        ENDSEC;
        DATA;
        #10=IFCMATERIAL('Brick',$,$);
        #11=IFCMATERIAL('Insulation',$,$);
        #12=IFCMATERIALLAYER(#10,100.,$);
        #13=IFCMATERIALLAYER(#11,50.,$);
        #14=IFCMATERIALLAYERSET((#12,#13),'WallLayers');
        #15=IFCMATERIALLAYERSETUSAGE(#14,.AXIS2.,.POSITIVE.,0.);
        #20=IFCWALL('wall-gid',$,'Wall',$,$,$,$);
        #21=IFCRELASSOCIATESMATERIAL('am1-gid',$,$,$,(#20),#15);
        ENDSEC;
        END-ISO-10303-21;
        """;

    const string MaterialConstituentIfc = """
        ISO-10303-21;
        HEADER;
        FILE_DESCRIPTION(('ViewDefinition'),'2;1');
        FILE_NAME('material-constituent-test.ifc','2024-01-01T00:00:00',(''),(''),'','','');
        FILE_SCHEMA(('IFC4'));
        ENDSEC;
        DATA;
        #10=IFCMATERIAL('Concrete',$,'Generic');
        #11=IFCMATERIAL('Steel',$,'Metal');
        #12=IFCMATERIALCONSTITUENT('Layer1',$,#10,$,$);
        #13=IFCMATERIALCONSTITUENT('Layer2',$,#11,$,$);
        #14=IFCMATERIALCONSTITUENTSET('Set',$,(#12,#13));
        #20=IFCWALL('wall-gid',$,'Wall',$,$,$,$,$,$);
        #21=IFCRELASSOCIATESMATERIAL('am1-gid',$,$,$,(#20),#14);
        ENDSEC;
        END-ISO-10303-21;
        """;

    const string ConnectionIfc = """
        ISO-10303-21;
        HEADER;
        FILE_DESCRIPTION(('ViewDefinition'),'2;1');
        FILE_NAME('connection-relations-test.ifc','2024-01-01T00:00:00',(''),(''),'','','');
        FILE_SCHEMA(('IFC2X3'));
        ENDSEC;
        DATA;
        #10=IFCWALL('wall1-gid',$,'Wall1',$,$,$,$);
        #11=IFCWALL('wall2-gid',$,'Wall2',$,$,$,$);
        #12=IFCRELCONNECTSELEMENTS('ce1-gid',$,$,$,$,#10,#11);
        #20=IFCFLOWSEGMENT('seg-gid',$,'Segment',$,$,$,$,$);
        #21=IFCPORT('port-gid',$,'Port',$,$,$,$);
        #22=IFCRELNESTS('nest1-gid',$,$,$,#20,(#21));
        #23=IFCRELCONNECTSPORTTOELEMENT('cp1-gid',$,$,$,#21,#20);
        #30=IFCPORT('portA-gid',$,'PortA',$,$,$,$);
        #31=IFCPORT('portB-gid',$,$,'PortB',$,$,$);
        #32=IFCRELCONNECTSPORTS('cp2-gid',$,$,$,#30,#31);
        ENDSEC;
        END-ISO-10303-21;
        """;

    static (StepDocument Doc, IfcEntityResolver Resolver) Parse(string ifc)
    {
        var doc = new StepDocument(Encoding.ASCII.GetBytes(ifc).Fix());
        return (doc, new IfcEntityResolver(doc));
    }

    [Test]
    public static void ParseDefinitionWithExtraWhitespaceBeforeGroup()
    {
        const string ifc = """
            ISO-10303-21;
            HEADER;
            FILE_DESCRIPTION(('ViewDefinition'),'2;1');
            FILE_NAME('whitespace-test.ifc','2024-01-01T00:00:00',(''),(''),'','','');
            FILE_SCHEMA(('IFC2X3'));
            ENDSEC;
            DATA;
            #1= IFCWALL  ($,$,$,$,$,$);
            ENDSEC;
            END-ISO-10303-21;
            """;

        using var doc = new StepDocument(Encoding.ASCII.GetBytes(ifc).Fix());
        Assert.That(doc.Definitions, Has.Count.EqualTo(1));
        Assert.That(doc.Definitions[0].Id, Is.EqualTo(1));
    }

    [Test]
    public static void ParseStructuralRelations()
    {
        var (doc, resolver) = Parse(MinimalStructuralIfc);
        using (doc)
        {
            var rels = new IfcRelations(resolver);

            Assert.That(rels.Relations, Has.Count.EqualTo(4));
            Assert.That(rels.Relations, Does.Contain(new IfcRelation(11, 10, IfcRelationKind.MemberOf)));
            Assert.That(rels.Relations, Does.Contain(new IfcRelation(20, 11, IfcRelationKind.MemberOf)));
            Assert.That(rels.Relations, Does.Contain(new IfcRelation(30, 20, IfcRelationKind.ContainedIn)));
            Assert.That(rels.Relations, Does.Contain(new IfcRelation(41, 40, IfcRelationKind.ChildOf)));
        }
    }

    [Test]
    public static void ParseGroupAndProjectRelations()
    {
        var (doc, resolver) = Parse(GroupAndProjectIfc);
        using (doc)
        {
            var rels = new IfcRelations(resolver);

            Assert.That(rels.Relations, Has.Count.EqualTo(2));
            Assert.That(rels.Relations, Does.Contain(new IfcRelation(20, 10, IfcRelationKind.MemberOf)));
            Assert.That(rels.Relations, Does.Contain(new IfcRelation(31, 30, IfcRelationKind.PartOf)));
        }
    }

    [Test]
    public static void ParseOpeningRelations()
    {
        var (doc, resolver) = Parse(OpeningIfc);
        using (doc)
        {
            var rels = new IfcRelations(resolver);

            Assert.That(rels.Relations, Has.Count.EqualTo(2));
            Assert.That(rels.Relations, Does.Contain(new IfcRelation(11, 10, IfcRelationKind.Voids)));
            Assert.That(rels.Relations, Does.Contain(new IfcRelation(20, 11, IfcRelationKind.Fills)));
        }
    }

    [Test]
    public static void ParseMaterialLayerSetRelations()
    {
        var (doc, resolver) = Parse(MaterialLayerSetIfc);
        using (doc)
        {
            var rels = new IfcRelations(resolver);

            Assert.That(rels.Relations, Does.Contain(new IfcRelation(20, 12, IfcRelationKind.HasLayer)));
            Assert.That(rels.Relations, Does.Contain(new IfcRelation(20, 13, IfcRelationKind.HasLayer)));
            Assert.That(rels.Relations, Does.Contain(new IfcRelation(12, 10, IfcRelationKind.HasMaterial)));
            Assert.That(rels.Relations, Does.Contain(new IfcRelation(13, 11, IfcRelationKind.HasMaterial)));
        }
    }

    [Test]
    public static void ParseMaterialConstituentRelations()
    {
        var (doc, resolver) = Parse(MaterialConstituentIfc);
        using (doc)
        {
            var rels = new IfcRelations(resolver);

            Assert.That(rels.Relations, Has.Count.EqualTo(2));
            Assert.That(rels.Relations, Does.Contain(new IfcRelation(20, 10, IfcRelationKind.HasMaterial)));
            Assert.That(rels.Relations, Does.Contain(new IfcRelation(20, 11, IfcRelationKind.HasMaterial)));
        }
    }

    [Test]
    public static void ParseConnectionRelations()
    {
        var (doc, resolver) = Parse(ConnectionIfc);
        using (doc)
        {
            var rels = new IfcRelations(resolver);

            Assert.That(rels.Relations, Does.Contain(new IfcRelation(11, 10, IfcRelationKind.ConnectsTo)));
            Assert.That(rels.Relations, Does.Contain(new IfcRelation(20, 21, IfcRelationKind.HasConnector)));
            Assert.That(rels.Relations, Does.Contain(new IfcRelation(31, 30, IfcRelationKind.ConnectsTo)));
        }
    }

    [Test]
    public static void IsMaybeIfcElement_ExcludesIfcRelEntities()
    {
        var (doc, resolver) = Parse(MinimalStructuralIfc);
        using (doc)
        {
            var relEntity = resolver.GetEntity(12);
            Assert.That(IfcToBosConverter.IsMaybeIfcElement(relEntity), Is.False);

            var wall = resolver.GetEntity(30);
            Assert.That(IfcToBosConverter.IsMaybeIfcElement(wall), Is.True);
        }
    }

    static string WriteTempIfc(string content)
    {
        var path = Path.Combine(Path.GetTempPath(), $"ara3d-rels-{Guid.NewGuid():N}.ifc");
        File.WriteAllText(path, content, Encoding.ASCII);
        return path;
    }

    [Test]
    public static void ConverterEmitsStructuralRelations()
    {
        var path = WriteTempIfc(MinimalStructuralIfc);
        IfcToBosConverter? converter = null;
        try
        {
            converter = new IfcToBosConverter(path);
            var bosRels = converter.BimDataBuilder.Relations;

            Assert.That(bosRels, Has.Count.EqualTo(4));

            var ifcToBos = converter.IfcIdToBosId;
            AssertRelation(bosRels, ifcToBos, 11, 10, RelationType.MemberOf);
            AssertRelation(bosRels, ifcToBos, 20, 11, RelationType.MemberOf);
            AssertRelation(bosRels, ifcToBos, 30, 20, RelationType.ContainedIn);
            AssertRelation(bosRels, ifcToBos, 41, 40, RelationType.ChildOf);

            Assert.That(converter.BosEntities.Any(e => e.GetEntityName().StartsWith("IFCREL")), Is.False);
        }
        finally
        {
            converter?.IfcFile.Dispose();
            try { File.Delete(path); } catch (IOException) { }
        }
    }

    [Test]
    public static void ConverterEmitsMaterialRelations()
    {
        var path = WriteTempIfc(MaterialLayerSetIfc);
        IfcToBosConverter? converter = null;
        try
        {
            converter = new IfcToBosConverter(path);
            var bosRels = converter.BimDataBuilder.Relations;
            var ifcToBos = converter.IfcIdToBosId;

            AssertRelation(bosRels, ifcToBos, 20, 12, RelationType.HasLayer);
            AssertRelation(bosRels, ifcToBos, 12, 10, RelationType.HasMaterial);
        }
        finally
        {
            converter?.IfcFile.Dispose();
            try { File.Delete(path); } catch (IOException) { }
        }
    }

    static void AssertRelation(
        IReadOnlyList<EntityRelation> rels,
        Dictionary<int, EntityIndex> ifcToBos,
        int fromIfcId,
        int toIfcId,
        RelationType type)
    {
        var from = ifcToBos[fromIfcId];
        var to = ifcToBos[toIfcId];
        Assert.That(rels, Does.Contain(new EntityRelation(from, to, type)));
    }
}
