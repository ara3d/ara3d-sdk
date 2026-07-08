using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
public sealed class TessellatedTests
{
    [Test]
    public void TriangulatedFaceSet_CopiesIndexedTriangles()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINTLIST3D(((0.,0.,0.),(1.,0.,0.),(0.,1.,0.)));
            #2=IFCTRIANGULATEDFACESET(#1,$,$,((1,2,3)),$);
            """);

        var ctx = model.Context;
        var mesh = Tessellated.BuildTriangulatedFaceSet(ctx, model.Entity(2));
        Assert.That(mesh.Points, Has.Count.EqualTo(3));
        Assert.That(mesh.FaceIndices, Has.Count.EqualTo(1));
    }
}
