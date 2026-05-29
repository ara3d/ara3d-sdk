namespace Ara3D.Studio.Samples.Demos;

[Category(nameof(Categories.Demos))]
public class CloneCubeOnMesh : IModifier
{
    public bool AtFaceCenters;
    [Range(0f, 1f)] public float Scale = 0.1f;

    public IModel3D Eval(IModel3D m)
    {
        var material = m.FirstOrDefaultMaterial();
        var instancedMesh = PlatonicSolids.TriangulatedCube.Scale(Scale);
        var mergedMesh = m.ToColoredMesh();
        var points = AtFaceCenters ? mergedMesh.Mesh.Triangles.Map(f => f.Center) : mergedMesh.Mesh.Points;
        return instancedMesh.Clone(material, points);
    }
}