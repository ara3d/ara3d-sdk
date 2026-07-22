namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
public class ClonePyramidOnFace : IModifier
{
    public static IReadOnlyList<Quad3D> ToQuads(IReadOnlyList<Triangle3D> triangles)
    {
        var r = new List<Quad3D>();
        for (var i = 0; i < triangles.Count; i += 2)
        {
            var a = triangles[i];
            var b = triangles[i + 1];
            var q = new Quad3D(a.A, a.B, b.A, b.B);
            r.Add(q);
        }

        return r;
    }


    public IModel3D Eval(IModel3D model3D, EvalContext context)
    {
        var firstMesh = model3D.Meshes[0];
        var firstMat = model3D.FirstOrDefaultMaterial();
        var quads = ToQuads(firstMesh.Triangles);
        var mesh = PlatonicSolids.Tetrahedron;
        var transforms = quads.Map(GeometryUtil.AlignToQuad);
        return mesh.Clone(firstMat, transforms);
    }
}