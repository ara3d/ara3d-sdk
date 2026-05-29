namespace Ara3D.Studio.Samples.Modifiers;

[Category(nameof(Categories.Converters))]
public class TriangleMeshToLineMesh : IModifier
{
    public LineMesh3D Eval(TriangleMesh3D mesh)
        => mesh.ToLineMesh3D();
}