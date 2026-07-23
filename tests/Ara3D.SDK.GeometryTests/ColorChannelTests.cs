using Ara3D.Geometry;
using Ara3D.Studio.API;
using DomainMask = Ara3D.Studio.API.FlowAttribute.AttributeDomainMask;

namespace Ara3D.SDK.GeometryTests;

[TestFixture]
public class ColorChannelTests
{
    // Two triangles sharing an edge: 4 vertices.
    static TriangleMesh3D TwoTriangles
        => new(
            new[] { new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(1, 1, 0) },
            new[] { new Integer3(0, 1, 2), new Integer3(2, 1, 3) });

    static Vector3[] FourColors
        => new[] { new Vector3(1, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 1), new Vector3(1, 1, 1) };

    static FlowObject EmptyFlowObject
        => new(null, null, [], []);

    [Test]
    public static void ColorsRoundTripThroughFlowObject()
    {
        var colors = FourColors;
        var fo = EmptyFlowObject.WithColors(colors);
        Assert.That(fo.GetColors(), Is.EqualTo(colors));
        Assert.That(fo.Attributes.Count, Is.EqualTo(1));
    }

    [Test]
    public static void GetColorsIsNullWhenAbsent()
        => Assert.That(EmptyFlowObject.GetColors(), Is.Null);

    [Test]
    public static void WithColorsReplacesExistingChannel()
    {
        var first = FourColors;
        var second = new[] { new Vector3(9, 9, 9), Vector3.Zero, Vector3.Zero, Vector3.Zero };
        var fo = EmptyFlowObject.WithColors(first).WithColors(second);
        Assert.That(fo.Attributes.Count, Is.EqualTo(1));
        Assert.That(fo.GetColors(), Is.EqualTo(second));
    }

    [Test]
    public static void WithoutColorsRemovesChannel()
    {
        var fo = EmptyFlowObject.WithColors(FourColors).WithoutColors();
        Assert.That(fo.GetColors(), Is.Null);
        Assert.That(fo.Attributes, Is.Empty);
    }

    [Test]
    public static void MeshValidatedAccessorRejectsCountMismatch()
    {
        var fo = EmptyFlowObject.WithColors(FourColors);
        Assert.That(fo.GetColors(TwoTriangles), Is.EqualTo(FourColors));
        // ExtractFaces compacts a two-triangle selection down to three vertices,
        // so the four-color channel no longer matches.
        var threeVertMesh = TwoTriangles.ExtractFaces([0]).Mesh;
        Assert.That(fo.GetColors(threeVertMesh), Is.Null);
    }

    [Test]
    public static void ColorChannelIsVertexDomainAndDropsOnContentChange()
    {
        var fo = EmptyFlowObject.WithColors(FourColors);
        Assert.That(fo.Attributes[0].Domain, Is.EqualTo(FlowAttribute.AttributeDomain.Vertex));
        // Vertex-domain indexing is not preserved by default, so the channel is dropped.
        Assert.That(fo.WithNewContent(new object()).GetColors(), Is.Null);
        // A modifier that declares the vertex domain preserved keeps it.
        Assert.That(fo.WithNewContent(new object(), DomainMask.Vertex).GetColors(), Is.EqualTo(FourColors));
    }
}
