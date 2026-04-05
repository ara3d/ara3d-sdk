namespace Ara3D.Studio.Samples;

    
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
        => input.WithMaterial(Material);
}

public class SetRenderSettings : IModifier
{
    public bool Wireframe { get; set; }
    public bool VertexColors { get; set; }
    public bool Shaded { get; set; }
    public bool Visible { get; set; }

    public RenderSettings GetRenderSettings() 
        => new(VertexColors, Wireframe, Shaded, Visible);

    public FlowObject Eval(FlowObject input)
        => input.WithNewRenderSettings(GetRenderSettings());
}