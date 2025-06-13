namespace Ara3D.Studio.Samples;

public class PushNodes : IModelModifier
{
    [Range(-10f, 10f)]
    public float Amount = 2;

    public static Matrix4x4 Push(Matrix4x4 self, Vector3 center, float amount)
    {
        var vec = (Vector3)self.Value.Translation - center;
        var newPos = center + vec * amount;
        return self * Matrix4x4.CreateTranslation(newPos);   
    }

    public Model3D Eval(Model3D m, EvalContext eval)
    {
        if (m.Elements.Count == 0) return m;
        var center = m.NodeCenter;
        return m.ModifyTransforms(mat => Push(mat, center, Amount));
    }   
}