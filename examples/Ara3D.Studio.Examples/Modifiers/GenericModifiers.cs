namespace Ara3D.Studio.Samples;

[Category(Cat.Color)]
[Description("Assigns a single material (color, metalness, roughness) to the whole object.")]
public class SetMaterial : IModifier
{
    [Range(0f, 1f)] public float Red = 0.2f;
    [Range(0f, 1f)] public float Green = 0.8f;
    [Range(0f, 1f)] public float Blue = 0.1f;
    [Range(0f, 1f)] public float Alpha = 1f;
    [Range(0f, 1f)] public float Metallic = 0f;
    [Range(0f, 1f)] public float Roughness = 0.5f;

    public Material Material =>
        new((Red, Green, Blue, Alpha), Metallic, Roughness);

    public FlowObject Eval(FlowObject input)
        => input.WithNewMaterial(Material);
}

[Category(Cat.Display)]
[Description("Per-object render overrides: wireframe, shading, visibility, vertex colors, and preview resolution.")]
public class SetRenderSettings : IModifier
{
    public bool Wireframe { get; set; }
    public bool VertexColors { get; set; }
    public bool Shaded { get; set; } = true;
    public bool Visible { get; set; } = true;
    [Range(2, 256)] public int Resolution { get; set; } = 32;

    public RenderSettings GetRenderSettings()
        => new(VertexColors, Wireframe, Visible, Shaded) { Resolution = Resolution };

    public FlowObject Eval(FlowObject input)
        => input.WithNewRenderSettings(GetRenderSettings());
}