namespace Ara3D.Studio.Samples.Modifiers;

[Category(nameof(Categories.Transformers))]
public class Transform : IModifier
{
    [Range(0.01f, 10f)] public float XScale = 1f;
    [Range(0.01f, 10f)] public float YScale = 1f;
    [Range(0.01f, 10f)] public float ZScale = 1f;

    [Range(-100f, 100f)] public float XOffset;
    [Range(-100f, 100f)] public float YOffset; 
    [Range(-100f, 100f)] public float ZOffset;

    [Range(-360, 360)] public int Yaw;
    [Range(-360, 360)] public int Pitch;
    [Range(-360, 360)] public int Roll;

    public FlowObject Eval(FlowObject input)
        => input
            .Translate((XOffset, YOffset, ZOffset))
            .Rotate(Yaw.Degrees(), Pitch.Degrees(), Roll.Degrees())
            .Scale((XScale, YScale, ZScale));
}

[Category(nameof(Categories.Transformers))]
public class AxisRotation : IModifier
{
    [Range(0, 8)] public int RotationAmount;

    public int Degrees => RotationAmount * 45;

    public List<string> AxisNames() => ["X", "Y", "Z"];

    [Options(nameof(AxisNames))] public int Axis;

    public IModel3D Eval(IModel3D model)
    {
        var axis = Axis == 0 ? Vector3.UnitX : Axis == 1 ? Vector3.UnitY : Vector3.UnitZ;
        var mat = Matrix4x4.CreateFromAxisAngle(axis, Degrees.Degrees());
        return model.Transform(mat);
    }
}

[Category(nameof(Categories.Transformers))]
public class Translate : IModifier
{
    public Vector3 Offset;

    public IModel3D Eval(IModel3D model, EvalContext eval)
        => model.Translate(Offset);
}