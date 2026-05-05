namespace Ara3D.Studio.Samples.Modifiers;

public class WeldVertices : IModifier 
{
    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        return mesh.WeldVertices();
    }
}

