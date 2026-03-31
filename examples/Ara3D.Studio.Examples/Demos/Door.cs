namespace Ara3D.Studio.Samples.Demos;

[Category(nameof(Categories.Demos))]
public class Door : IGenerator
{
    [Range(0f, 5f)] public float Height = 3f;
    [Range(0f, 5f)] public float Width = 1f;
    [Range(0f, 0.5f)] public float Thickness = 0.07f;
    [Range(0f, 1f)] public float PanelMargin = 0.15f;
    [Range(0, 0.5f)] public float PanelInset1 = 0.03f;
    [Range(0, 0.5f)] public float PanelInset2 = 0.09f;
    [Range(0, 0.25f)] public float PanelIntrusion = 0.02f;
    public bool RaisedPanel = true;

    public bool HasPanels = true;
    [Range(1, 3)] public int NumHorizontalSections = 3;
    [Range(1, 2)] public int NumVerticalSections = 1;

    [Range(0, 100f)] public float FirstHorizontalPercentage = 40;
    [Range(0, 100f)] public float SecondHorizontalPercentage = 60;

    public const int FaceNumber = 3;

    public void BuildPanels(Quad3DFaceHandle f, Panel panelBuilder)
    {
        var panes = new List<Quad3DFaceHandle>();

        if (NumHorizontalSections > 1)
        {
            var (top, bottom) = f.SplitTopBottom(1f - FirstHorizontalPercentage / 100f);
            panes.Add(bottom);

            if (NumHorizontalSections > 2)
            {
                var (topTop, topBottom) = top.SplitTopBottom(1f - SecondHorizontalPercentage / 100f);
                panes.Add(topTop);
                panes.Add(topBottom);
            }
            else
            {
                panes.Add(top);
            }
        }
        else
        {
            panes.Add(f);
        }

        if (NumVerticalSections > 1)
        {
            // NOTE: we only support 1 vertical section for now. 
            Debug.Assert(NumVerticalSections == 2);

            var tmp = panes.ToArray();
            panes.Clear();

            foreach (var p in tmp)
            {
                var (left, right) = p.SplitLeftRight();
                panes.Add(left);
                panes.Add(right);
            }
        }

        foreach (var p in panes)
        {
            panelBuilder.BuildPanel(p); 
        }
    }

    public TriangleMesh3D Eval(EvalContext context)
    {
        var bldr = PlatonicSolids.Cube.Scale((Width, Thickness, Height)).ToBuilder();

        if (HasPanels)
        {
            var panelBuilder = new Panel
            {
                PanelInset1 = PanelInset1,
                PanelInset2 = PanelInset2,
                PanelMargin = PanelMargin,
                PanelIntrusion = PanelIntrusion,
                RaisedPanel = RaisedPanel
            };

            var f = bldr.GetFace(FaceNumber);
            f.FaceData.CornerIndices = f.FaceData.CornerIndices.RotateLeft();
            BuildPanels(f, panelBuilder);
        }

        return bldr.ToTriangleMesh3D();
    }
}

public static class Helpers
{
    public static Integer4 RotateLeft(this Integer4 self)
        => (self.B, self.C, self.D, self.A);

    public static Integer4 RotateRight(this Integer4 self)
        => (self.D, self.A, self.B, self.C);
}