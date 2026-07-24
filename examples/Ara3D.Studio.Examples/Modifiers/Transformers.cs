namespace Ara3D.Studio.Samples.Modifiers;

/// <summary>Which transform group the viewport gizmo edits. Eval always applies all three
/// (translate, then rotate, then scale) regardless — Mode only chooses the visible widget.</summary>
public enum TransformMode
{
    Translate,
    Rotate,
    Scale,
}

[Category(Cat.Transform)]
[Description("Applies translate, rotate, and scale together. Mode selects which single gizmo is shown; stack Translate/Rotate/Scale nodes instead if you prefer selecting by node.")]
public class Transform : IModifier, IGizmoProvider
{
    public TransformMode Mode;

    public Vector3 Offset;

    [Range(-360f, 360f)] public float Yaw;
    [Range(-360f, 360f)] public float Pitch;
    [Range(-360f, 360f)] public float Roll;

    public Vector3 Scaling = (1f, 1f, 1f);

    public IModel3D Eval(IModel3D input)
        => input
            .Translate(Offset)
            .Rotate(Yaw.Degrees(), Pitch.Degrees(), Roll.Degrees())
            .Scale(Scaling);

    public IReadOnlyList<GizmoElement> GetGizmoElements()
        => Mode switch
        {
            TransformMode.Rotate => GizmoElements.RotationRings(nameof(Yaw), nameof(Pitch), nameof(Roll)),
            TransformMode.Scale => GizmoElements.ScaleHandles(nameof(Scaling)),
            _ => GizmoElements.Translation(nameof(Offset)),
        };
}

[Category(Cat.Transform)]
[Description("Rotates the model by a whole number of quarter-turns about the X, Y, or Z axis.")]
public class AxisRotation : IModifier
{
    [Range(0, 4)] public int QuarterTurns;

    public int Degrees => QuarterTurns * 90;

    public List<string> AxisNames() => ["X", "Y", "Z"];

    [Options(nameof(AxisNames))] public int Axis;

    public IModel3D Eval(IModel3D model)
    {
        var axis = Axis == 0 ? Vector3.UnitX : Axis == 1 ? Vector3.UnitY : Vector3.UnitZ;
        var mat = Matrix4x4.CreateFromAxisAngle(axis, Degrees.Degrees());
        return model.Transform(mat);
    }
}

[Category(Cat.Transform)]
[Description("Moves the model by an offset vector, editable with a translation gizmo.")]
public class Translate : IModifier, IGizmoProvider
{
    public Vector3 Offset;

    public IModel3D Eval(IModel3D model)
        => model.Translate(Offset);

    public IReadOnlyList<GizmoElement> GetGizmoElements()
        => GizmoElements.Translation(nameof(Offset));
}

[Category(Cat.Transform)]
[Description("Rotates the model by Euler angles, editable with a rotation gizmo.")]
public class Rotate : IModifier, IGizmoProvider
{
    [Range(-180f, 180f)] public float XDegrees;
    [Range(-180f, 180f)] public float YDegrees;
    [Range(-180f, 180f)] public float ZDegrees;

    public IModel3D Eval(IModel3D model)
        => model.Transform(
            Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, XDegrees.Degrees())
            * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, YDegrees.Degrees())
            * Matrix4x4.CreateFromAxisAngle(Vector3.UnitZ, ZDegrees.Degrees()));

    public IReadOnlyList<GizmoElement> GetGizmoElements()
        => GizmoElements.RotationRings(nameof(XDegrees), nameof(YDegrees), nameof(ZDegrees));
}

[Category(Cat.Transform)]
[Description("Scales the model per-axis, editable with a scale gizmo.")]
public class Scale : IModifier, IGizmoProvider
{
    public Vector3 Amount = (1f, 1f, 1f);

    public IModel3D Eval(IModel3D model)
        => model.Scale(Amount);

    public IReadOnlyList<GizmoElement> GetGizmoElements()
        => GizmoElements.ScaleHandles(nameof(Amount));
}
