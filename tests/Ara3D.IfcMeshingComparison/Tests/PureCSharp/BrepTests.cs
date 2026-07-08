using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
public sealed class BrepTests
{
    [Test]
    public void FacetedBrep_DuplicateConsecutiveVertices_Meshes()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCCARTESIANPOINT((2.,0.,0.));
            #3=IFCCARTESIANPOINT((0.,2.,0.));
            #4=IFCPOLYLOOP((#1,#1,#2,#3,#1));
            #5=IFCFACEOUTERBOUND(#4,.T.);
            #6=IFCFACE((#5));
            #7=IFCCLOSEDSHELL((#6));
            #8=IFCFACETEDBREP(#7);
            """);

        var ctx = model.Context;
        var mesh = Brep.BuildFacetedBrep(ctx, model.Entity(8));
        Assert.That(mesh.FaceIndices, Has.Count.EqualTo(1));
    }

    [Test]
    public void FacetedBrep_TriangleFace_Meshes()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCCARTESIANPOINT((2.,0.,0.));
            #3=IFCCARTESIANPOINT((0.,2.,0.));
            #4=IFCPOLYLOOP((#1,#2,#3));
            #5=IFCFACEOUTERBOUND(#4,.T.);
            #6=IFCFACE((#5));
            #7=IFCCLOSEDSHELL((#6));
            #8=IFCFACETEDBREP(#7);
            """);

        var ctx = model.Context;
        var mesh = Brep.BuildFacetedBrep(ctx, model.Entity(8));
        Assert.That(mesh.FaceIndices, Has.Count.EqualTo(1));
        Assert.That(mesh.Points, Has.Count.GreaterThanOrEqualTo(3));
    }

    [Test]
    public void ConnectedFaceSet_TriangleFace_Meshes()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCCARTESIANPOINT((2.,0.,0.));
            #3=IFCCARTESIANPOINT((0.,2.,0.));
            #4=IFCPOLYLOOP((#1,#2,#3));
            #5=IFCFACEOUTERBOUND(#4,.T.);
            #6=IFCFACE((#5));
            #7=IFCCONNECTEDFACESET((#6));
            """);

        var ctx = model.Context;
        var mesh = Brep.BuildConnectedFaceSet(ctx, model.Entity(7));
        Assert.That(mesh.FaceIndices, Has.Count.EqualTo(1));
    }

    [Test]
    public void FaceBasedSurfaceModel_UnwrapsConnectedFaceSet()
    {
        using var model = MicroIfc.Parse("""
            #1=IFCCARTESIANPOINT((0.,0.,0.));
            #2=IFCCARTESIANPOINT((2.,0.,0.));
            #3=IFCCARTESIANPOINT((0.,2.,0.));
            #4=IFCPOLYLOOP((#1,#2,#3));
            #5=IFCFACEOUTERBOUND(#4,.T.);
            #6=IFCFACE((#5));
            #7=IFCCONNECTEDFACESET((#6));
            #8=IFCFACEBASEDSURFACEMODEL((#7));
            """);

        var ctx = model.Context;
        var mesh = Brep.BuildFaceBasedSurfaceModel(ctx, model.Entity(8));
        Assert.That(mesh.FaceIndices, Has.Count.EqualTo(1));
    }

    [Test]
    [Category("Slow")]
    public void DentalClinic_FaceBasedSurfaceModels_ProduceGeometry()
    {
        TestFiles.RequireExists(TestFiles.DentalClinic);
        using var file = TestFiles.LoadStep(TestFiles.DentalClinic);
        var (model, diagnostics) = ModelAssembler.BuildModel(file);
        Assert.That(model.Meshes.Count, Is.GreaterThan(0));
        Assert.That(diagnostics.EntityStatus.GetValueOrDefault("IFCCONNECTEDFACESET"),
            Is.EqualTo(GeometrySupportStatus.Supported));
    }

    [Test]
    [Category("Slow")]
    public void Duplex_FaceBasedSurfaceModels_ProduceGeometry()
    {
        TestFiles.RequireExists(TestFiles.Duplex);
        using var file = TestFiles.LoadStep(TestFiles.Duplex);
        var (model, _) = ModelAssembler.BuildModel(file);
        Assert.That(model.Meshes.Count, Is.GreaterThan(0));
    }

    [Test]
    [Category("Slow")]
    public void SteelPlates_BrepFile_ProducesGeometry()
    {
        TestFiles.RequireExists(TestFiles.SteelPlates);
        using var file = TestFiles.LoadStep(TestFiles.SteelPlates);
        var (model, _) = ModelAssembler.BuildModel(file);
        Assert.That(model.Instances.Count, Is.GreaterThan(0));
        Assert.That(model.Meshes.Count, Is.GreaterThan(0));
    }
}
