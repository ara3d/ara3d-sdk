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
}