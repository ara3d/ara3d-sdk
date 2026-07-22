namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
[Description("A single raised-center panel surface, such as a door or cabinet panel.")]
public class Panel : IGenerator
{
    [Range(0f, 5f)] public float Height  = 3f;
    [Range(0f, 5f)] public float Width = 1f;
    [Range(0f, 0.5f)] public float Thickness = 0.07f;
    [Range(0f, 1f)] public float PanelMargin = 0.5f;
    [Range(0, 0.5f)] public float PanelInset1 = 0.03f;
    [Range(0, 0.5f)] public float PanelInset2 = 0.09f;
    [Range(0, 0.25f)] public float PanelIntrusion = 0.02f;
    public bool RaisedPanel = true; 

    public const int FaceNumber = 3;

    public QuadMesh3DBuilder BuildPanel(Quad3DFaceHandle f)
    {
        var f1 = f.Inset(PanelMargin);;
        var f2 = f1.Inset(PanelInset1);
        if (RaisedPanel)
            f2.Inset(PanelInset2);
        return f2.Push(-PanelIntrusion);
    }

    public TriangleMesh3D Eval(EvalContext context)
    {
        var bldr = PlatonicSolids.Cube.Scale((Width, Thickness, Height)).ToBuilder();
        BuildPanel(bldr.GetFace(FaceNumber));
        return bldr.ToTriangleMesh3D();
    }
}