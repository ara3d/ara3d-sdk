using Ara3D.Geometry;
using Ara3D.Ifc.Mesher.Approach1;
using Ara3D.IfcLoader;
using Ara3D.IfcMeshingComparison.Harness;
using Ara3D.IfcMeshingComparison.Tests.Support;
using Ara3D.Utils;

namespace Ara3D.IfcMeshingComparison.Tests.PureCSharp;

[TestFixture]
public sealed class WpW4Tests
{
  static FilePath C20Institute => new(@"C:\Users\cdigg\git\studio\data\C20-Institute-Var-2.ifc");
  static FilePath Ac11Institute => new(@"C:\Users\cdigg\git\studio\data\231110AC11-Institute-Var-2-IFC.ifc");

  [Test]
  public void MeshesThinInstituteHelixTriangleShellFace()
  {
    // C20-Institute-Var-2.ifc #14947 — sub-millimeter facet on a helix shell; ear clipping used to skip these.
    using var model = MicroIfc.Parse("""
        #1=IFCCARTESIANPOINT((1.5075,0.348334,0.372727));
        #2=IFCCARTESIANPOINT((1.5075,0.,0.236364));
        #3=IFCCARTESIANPOINT((1.5074,0.348334,0.372727));
        #10=IFCPOLYLOOP((#1,#2,#3));
        #11=IFCFACEOUTERBOUND(#10,.T.);
        #12=IFCFACE((#11));
        #13=IFCOPENSHELL((#12));
        #14=IFCSHELLBASEDSURFACEMODEL((#13));
        """);

    var mesh = MeshRequired(model, 14);
    Assert.That(mesh.FaceIndices, Has.Count.EqualTo(1));
  }

  [Test]
  public void MeshesInstituteHelixOpenShellWithMultipleThinFacets()
  {
    using var model = MicroIfc.Parse("""
        #1=IFCCARTESIANPOINT((1.5075,0.348334,0.372727));
        #2=IFCCARTESIANPOINT((1.5075,0.,0.236364));
        #3=IFCCARTESIANPOINT((1.5074,0.348334,0.372727));
        #4=IFCCARTESIANPOINT((1.5074,0.,0.236364));
        #5=IFCCARTESIANPOINT((1.5075,0.348334,0.372827));
        #6=IFCCARTESIANPOINT((1.5075,0.,0.236464));
        #10=IFCPOLYLOOP((#1,#2,#3));
        #11=IFCFACEOUTERBOUND(#10,.T.);
        #12=IFCFACE((#11));
        #20=IFCPOLYLOOP((#2,#1,#5,#6));
        #21=IFCFACEOUTERBOUND(#20,.T.);
        #22=IFCFACE((#21));
        #30=IFCPOLYLOOP((#2,#4,#3));
        #31=IFCFACEOUTERBOUND(#30,.T.);
        #32=IFCFACE((#31));
        #40=IFCOPENSHELL((#12,#22,#32));
        #41=IFCSHELLBASEDSURFACEMODEL((#40));
        """);

    var mesh = MeshRequired(model, 41);
    Assert.That(mesh.FaceIndices, Has.Count.EqualTo(4));
  }

  [Test]
  [Explicit("WP-W4 stretch: C20-Institute-Var-2 parity and tri ratio")]
  [Category("Slow")]
  public void ScoreC20InstituteStretch()
  {
    TestFiles.RequireExists(C20Institute);
    EnsureOracle(C20Institute);

    var result = ModelComparer.CompareFile(C20Institute);
    TestContext.WriteLine(ModelComparer.FormatResult(result));

    var mergedRatio = SafeRatio(result.MergedMesh.CandidateTriangleCount, result.MergedMesh.OracleTriangleCount);
    TestContext.WriteLine(
      $"C20 gates: parity={result.ParityScore:F3} entityShape={result.EntityShape.Score:F3} " +
      $"mergedTri={result.MergedMesh.CandidateTriangleCount}/{result.MergedMesh.OracleTriangleCount} ({mergedRatio:F3}) " +
      $"inst={result.InstanceCount.Candidate}/{result.InstanceCount.Oracle}");

    Assert.That(result.ParityScore, Is.GreaterThanOrEqualTo(0.85));
    Assert.That(result.EntityShape.Score, Is.GreaterThanOrEqualTo(0.85));
    Assert.That(mergedRatio, Is.GreaterThanOrEqualTo(0.35));
  }

  [Test]
  [Explicit("WP-W4 stretch: AC11 Institute parity and tri ratio")]
  [Category("Slow")]
  public void ScoreAc11InstituteStretch()
  {
    TestFiles.RequireExists(Ac11Institute);
    EnsureOracle(Ac11Institute);

    var result = ModelComparer.CompareFile(Ac11Institute);
    TestContext.WriteLine(ModelComparer.FormatResult(result));

    var mergedRatio = SafeRatio(result.MergedMesh.CandidateTriangleCount, result.MergedMesh.OracleTriangleCount);
    TestContext.WriteLine(
      $"AC11 gates: parity={result.ParityScore:F3} entityShape={result.EntityShape.Score:F3} " +
      $"mergedTri={result.MergedMesh.CandidateTriangleCount}/{result.MergedMesh.OracleTriangleCount} ({mergedRatio:F3}) " +
      $"inst={result.InstanceCount.Candidate}/{result.InstanceCount.Oracle}");

    Assert.That(result.ParityScore, Is.GreaterThanOrEqualTo(0.85));
    Assert.That(result.EntityShape.Score, Is.GreaterThanOrEqualTo(0.85));
  }

  static void EnsureOracle(FilePath ifcPath)
  {
    var bfastPath = WebIfcBfastOracle.OraclePath(ifcPath);
    if (!bfastPath.Exists() || WebIfcBfastOracle.NeedsRegeneration(ifcPath, bfastPath))
      WebIfcBfastOracle.Generate(ifcPath, TestContext.WriteLine);
  }

  static float SafeRatio(int candidate, int oracle)
    => oracle <= 0 ? 0f : (float)candidate / oracle;

  static TriangleMesh3D MeshRequired(MicroIfcModel model, int entityId)
  {
    var mesh = GeometryDispatcher.TryBuild(model.Context, model.Entity(entityId));
    if (mesh is null)
    {
      var notes = model.Context.Diagnostics.Messages.TakeLast(3);
      var detail = notes.Any() ? string.Join("; ", notes) : "no diagnostics";
      throw new InvalidOperationException($"Could not mesh entity #{entityId}: {detail}");
    }
    return mesh.Value;
  }
}
