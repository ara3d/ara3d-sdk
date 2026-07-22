namespace Ara3D.Studio.Samples.Modifiers;

[Category(Cat.Convert)]
public class TriangleMeshToLineMesh : IModifier
{
    public LineMesh3D Eval(TriangleMesh3D mesh)
        => mesh.ToLineMesh3D();
}