using System.Text;
using Ara3D.BimOpenSchema.IO;
using Ara3D.IfcLoader;
using Ara3D.IO.StepParser;
using Ara3D.Memory;

namespace Ara3D.BimOpenSchema.Tests;

[TestFixture]
public static class IfcStructuralRelationsTests
{
    const string MinimalIfc = """
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

    static (StepDocument Doc, IfcEntityResolver Resolver) Parse()
    {
        var doc = new StepDocument(Encoding.ASCII.GetBytes(MinimalIfc).Fix());
        return (doc, new IfcEntityResolver(doc));
    }

    [Test]
    public static void ParseStructuralRelations()
    {
        var (doc, resolver) = Parse();
        using (doc)
        {
            var rels = new IfcStructuralRelations(resolver);

            Assert.That(rels.Relations, Has.Count.EqualTo(4));
            Assert.That(rels.Relations, Does.Contain(new IfcStructuralRelation(11, 10, IfcStructuralRelationKind.MemberOf)));
            Assert.That(rels.Relations, Does.Contain(new IfcStructuralRelation(20, 11, IfcStructuralRelationKind.MemberOf)));
            Assert.That(rels.Relations, Does.Contain(new IfcStructuralRelation(30, 20, IfcStructuralRelationKind.ContainedIn)));
            Assert.That(rels.Relations, Does.Contain(new IfcStructuralRelation(41, 40, IfcStructuralRelationKind.ChildOf)));
        }
    }

    [Test]
    public static void IsMaybeIfcElement_ExcludesIfcRelEntities()
    {
        var (doc, resolver) = Parse();
        using (doc)
        {
            var relEntity = resolver.GetEntity(12);
            Assert.That(IfcToBosConverter.IsMaybeIfcElement(relEntity), Is.False);

            var wall = resolver.GetEntity(30);
            Assert.That(IfcToBosConverter.IsMaybeIfcElement(wall), Is.True);
        }
    }

    static string WriteTempIfc()
    {
        var path = Path.Combine(Path.GetTempPath(), $"ara3d-structural-rels-{Guid.NewGuid():N}.ifc");
        File.WriteAllText(path, MinimalIfc, Encoding.ASCII);
        return path;
    }

    [Test]
    public static void ConverterEmitsStructuralRelations()
    {
        var path = WriteTempIfc();
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
