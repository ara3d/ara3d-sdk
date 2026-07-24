namespace Ara3D.Studio.Samples.Modifiers;

[Category(Cat.Convert)]
[Description("Converts a triangle mesh into a line mesh of its edges (a wireframe).")]
public class Edges : IModifier
{
    public LineMesh3D Eval(TriangleMesh3D mesh)
        => mesh.ToLineMesh3D();
}
