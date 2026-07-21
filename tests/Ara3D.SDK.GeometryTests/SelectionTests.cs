using Ara3D.Geometry;
using Ara3D.Studio.API;
using Domain = Ara3D.Studio.API.FlowAttribute.AttributeDomain;
using DomainMask = Ara3D.Studio.API.FlowAttribute.AttributeDomainMask;

namespace Ara3D.SDK.GeometryTests;

[TestFixture]
public class SelectionTests
{
    // Two triangles sharing an edge: vertices 0-3, faces (0,1,2) and (2,1,3)
    static TriangleMesh3D TwoTriangles
        => new(
            new[] { new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(1, 1, 0) },
            new[] { new Integer3(0, 1, 2), new Integer3(2, 1, 3) });

    static TriangleMesh3D Cube => PlatonicSolids.TriangulatedCube;

    [Test]
    public static void ExtractFacesCompactsVerticesAndMaps()
    {
        var sub = TwoTriangles.ExtractFaces([0]);
        Assert.That(sub.Mesh.FaceIndices.Count, Is.EqualTo(1));
        Assert.That(sub.Mesh.Points.Count, Is.EqualTo(3));
        Assert.That(sub.SourceFaces, Is.EqualTo(new[] { 0 }));
        Assert.That(sub.SourceVertices, Is.EqualTo(new[] { 0, 1, 2 }));
        Assert.That(sub.Mesh.Points[0], Is.EqualTo(TwoTriangles.Points[0]));
    }

    [Test]
    public static void DeleteFacesIsComplementOfExtract()
    {
        var remaining = TwoTriangles.DeleteFaces([0]);
        Assert.That(remaining.FaceIndices.Count, Is.EqualTo(1));
        Assert.That(remaining.Points.Count, Is.EqualTo(3));
    }

    [Test]
    public static void SplitByFacesPartitionsAllFaces()
    {
        var faceCount = Cube.FaceIndices.Count;
        var (selected, remaining) = Cube.SplitByFaces([0, 2, 4]);
        Assert.That(selected.Mesh.FaceIndices.Count, Is.EqualTo(3));
        Assert.That(remaining.Mesh.FaceIndices.Count, Is.EqualTo(faceCount - 3));
    }

    [Test]
    public static void CreateSortsAndDeduplicates()
    {
        var s = SelectionSet.Create(Domain.Face, [3, 1, 3, 2, 1], 5);
        Assert.That(s.Indices, Is.EqualTo(new[] { 1, 2, 3 }));
        Assert.That(s.Count, Is.EqualTo(3));
        Assert.That(s.Contains(2), Is.True);
        Assert.That(s.Contains(0), Is.False);
    }

    [Test]
    public static void CreateRejectsOutOfRangeIndices()
        => Assert.Throws<ArgumentOutOfRangeException>(() => SelectionSet.Create(Domain.Face, [5], 5));

    [Test]
    public static void SetOperationsWork()
    {
        var a = SelectionSet.Create(Domain.Face, [0, 1, 2], 5);
        var b = SelectionSet.Create(Domain.Face, [2, 3], 5);
        Assert.That(a.Union(b).Indices, Is.EqualTo(new[] { 0, 1, 2, 3 }));
        Assert.That(a.Intersect(b).Indices, Is.EqualTo(new[] { 2 }));
        Assert.That(a.Subtract(b).Indices, Is.EqualTo(new[] { 0, 1 }));
        Assert.That(a.Invert().Indices, Is.EqualTo(new[] { 3, 4 }));
        Assert.That(SelectionSet.All(Domain.Face, 3).Indices, Is.EqualTo(new[] { 0, 1, 2 }));
        Assert.That(SelectionSet.Empty(Domain.Face, 3).IsEmpty, Is.True);
    }

    [Test]
    public static void SetOperationsRejectMismatchedDomains()
    {
        var a = SelectionSet.Create(Domain.Face, [0], 5);
        var b = SelectionSet.Create(Domain.Vertex, [0], 5);
        var c = SelectionSet.Create(Domain.Face, [0], 6);
        Assert.Throws<ArgumentException>(() => a.Union(b));
        Assert.Throws<ArgumentException>(() => a.Union(c));
    }

    static FlowObject EmptyFlowObject
        => new(null, null, [], []);

    [Test]
    public static void SelectionRoundTripsThroughFlowObject()
    {
        var s = SelectionSet.Create(Domain.Face, [1, 2], 12);
        var fo = EmptyFlowObject.WithSelection(s);
        Assert.That(fo.GetSelection(Domain.Face), Is.SameAs(s));
        Assert.That(fo.GetSelection(Domain.Vertex), Is.Null);
    }

    [Test]
    public static void WithSelectionReplacesSameDomainKeepsOther()
    {
        var face1 = SelectionSet.Create(Domain.Face, [1], 12);
        var face2 = SelectionSet.Create(Domain.Face, [2], 12);
        var vertex = SelectionSet.Create(Domain.Vertex, [0], 8);
        var fo = EmptyFlowObject.WithSelection(face1).WithSelection(vertex).WithSelection(face2);
        Assert.That(fo.Attributes.Count, Is.EqualTo(2));
        Assert.That(fo.GetSelection(Domain.Face), Is.SameAs(face2));
        Assert.That(fo.GetSelection(Domain.Vertex), Is.SameAs(vertex));
    }

    [Test]
    public static void WithNewContentDropsSelectionByDefault()
    {
        var fo = EmptyFlowObject.WithSelection(SelectionSet.Create(Domain.Face, [1], 12));
        Assert.That(fo.WithNewContent(new object()).Attributes, Is.Empty);
    }

    [Test]
    public static void WithNewContentKeepsPreservedDomains()
    {
        var fo = EmptyFlowObject
            .WithSelection(SelectionSet.Create(Domain.Face, [1], 12))
            .WithSelection(SelectionSet.Create(Domain.Vertex, [0], 8));
        var kept = fo.WithNewContent(new object(), DomainMask.Face);
        Assert.That(kept.Attributes.Count, Is.EqualTo(1));
        Assert.That(kept.GetSelection(Domain.Face), Is.Not.Null);
        Assert.That(kept.GetSelection(Domain.Vertex), Is.Null);
        Assert.That(fo.WithNewContent(new object(), DomainMask.All).Attributes.Count, Is.EqualTo(2));
    }
}
