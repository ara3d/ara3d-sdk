namespace Ara3D.Studio.API;

public class RenderSettings
{
    public RenderSettings(bool vertexColors, bool wireframe, bool visible, bool shaded)
    {
        VertexColors = vertexColors;
        Wireframe = wireframe;
        Visible = visible;
        Shaded = shaded;
    }

    public RenderSettings()
    { }

    public bool VertexColors { get; set; } = false;
    public bool Wireframe { get; set; } = false;
    public bool Visible { get; set; } = true;
    public bool Shaded { get; set; } = true;

    /// <summary>
    /// Samples per dimension used when a parametric flow value (curve, surface, SDF) is
    /// realized into drawable geometry — see <see cref="FlowRenderContext.Resolution"/>.
    /// A view-time setting, not part of the flowing data: the same analytic value can be
    /// realized coarse or fine without re-evaluating the pipeline nodes.
    /// </summary>
    public int Resolution { get; set; } = 32;

    /// <summary>Shared fallback for hosts that supply no settings. Treat as read-only.</summary>
    public static RenderSettings Default { get; } = new();
}
