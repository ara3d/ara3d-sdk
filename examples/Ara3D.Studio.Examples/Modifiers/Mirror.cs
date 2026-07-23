namespace Ara3D.Studio.Samples.Modifiers;

[Category(Cat.Transform)]
[Description("Reflects the mesh across an axis-aligned plane, flipping face winding so normals stay outward. Optionally keeps the original to produce a symmetric result.")]
public class Mirror : IModifier
{
    [Range(0, 2)] public int Axis = 0;   // plane normal: 0=X, 1=Y, 2=Z
    public bool AboutCenter = true;       // reflect about bounds center (true) or world origin (false)
    public bool KeepOriginal;             // merge original with its reflection (symmetrize) instead of returning only the reflection

    public TriangleMesh3D Eval(TriangleMesh3D mesh)
    {
        var pivot = AboutCenter ? mesh.DerivedBounds().Center.Vector3 : Vector3.Zero;
        var sign = Vector3.One.WithComponent(Axis, -1f);
        var mat = Matrix4x4.CreateTranslation(pivot)
                  * Matrix4x4.CreateScale(sign.X, sign.Y, sign.Z)
                  * Matrix4x4.CreateTranslation(-pivot);
        var reflected = mesh.Transform(mat).FlipFaces();
        return KeepOriginal ? new[] { mesh, reflected }.Merge() : reflected;
    }
}
