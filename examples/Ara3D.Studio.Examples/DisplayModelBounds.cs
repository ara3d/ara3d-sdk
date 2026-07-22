namespace Ara3D.Studio.Samples;

[Category(Cat.Display)]
[Description("Overlays the whole model's bounding box as a wireframe frame.")]
public class DisplayModelBounds : IModifier
{
    private Bounds3D _bounds;
    private IModel3D _model;

    [Range(0.001f, 0.5f)] public float FrameSize = 0.02f;

    public IModel3D Eval(IModel3D m)
    {
        if (_model != m)
        {
            _model = m;
            if (_model == null) 
                return m;
            _bounds = _model.GetBounds();
        }

        var mb = new Model3DBuilder();
        var frameMesh = new BoxFrameMeshBuilder(FrameSize).Mesh.FitToBounds(_bounds).Triangulate();
        mb.AddModel(m);
        mb.AddInstance(frameMesh);
        return mb.Build();
    }
}