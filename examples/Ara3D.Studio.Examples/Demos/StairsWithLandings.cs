namespace Ara3D.Studio.Samples.Demos;

[Category(nameof(Categories.Demos))]
public class StairsWithLandings : IGenerator
{
    [Range(1, 100)] public int Flights { get; set; } = 3;
    public Material LandingMaterial { get; set; } = new(new(0.9f, 0.7f, 0.8f, 1f), 0f, 0.2f);

    [Range(0f, 1f)] public float MaxRiserHeight { get; set; } = 0.3f;
    [Range(0f, 4f)] public float TreadWidth { get; set; } = 0.8f;
    [Range(0f, 1f)] public float TreadThickness { get; set; } = 0.05f;

    public float LandingWidth => TreadWidth * 2 + LandingWidthExtra;
    [Range(0f, 10f)] public float LandingWidthExtra { get; set; } = 0.5f;
    [Range(0f, 10f)] public float LandingDepth { get; set; } = 1f;
    [Range(0.001f, 1f)] public float LandingThickness { get; set; } = 0.2f;

    [Range(0f, 20f)] public float LandingVerticalSpacing { get; set; } = 5f;
    [Range(0f, 20f)] public float LandingHorizontalSpacing { get; set; } = 4f;

    public bool CenterStringer { get; set; } = true;
    public bool SideStringers { get; set; } = true;
    public bool HasRiser { get; set; } = true;

    public TriangleMesh3D CreateLandingMesh()
    {
        var box = PlatonicSolids.TriangulatedCube;
        var scale = new Vector3(LandingWidth, LandingDepth, LandingThickness);
        return box.Scale(scale).Translate((0, 0, -LandingThickness / 2));
    }

    public void CreateLandings(Model3DBuilder bldr)
    {
        var landingMesh = CreateLandingMesh();
        var meshIndex = bldr.AddMeshWithoutInstance(landingMesh);
        for (var i = 0;i < Flights; i++)
        {
            var yoffset = (i % 2) * LandingHorizontalSpacing;
            yoffset += (i % 2 == 0) ? -LandingDepth / 2 : LandingDepth / 2;
            var landingCenter = new Point3D(0, yoffset, i * LandingVerticalSpacing);
            var landingMatrix = Matrix4x4.CreateTranslation(landingCenter);
            bldr.AddInstance(meshIndex, landingMatrix, LandingMaterial);
        }
    }

    public static Matrix4x4 CreateMirrorInXYPlane(Vector3 center)
    {
        return
            Matrix4x4.CreateTranslation(-center) *
            Matrix4x4.CreateScale(1f, 1f, -1f) *
            Matrix4x4.CreateTranslation(center);
    }

    public static Matrix4x4 CreateMirrorInXZPlane(Vector3 center)
    {
        return
            Matrix4x4.CreateTranslation(-center) *
            Matrix4x4.CreateScale(1f, -1f, 1f) *
            Matrix4x4.CreateTranslation(center);
    }

    public Model3D CreateOneFlightOfStairs()
    {
        if (MaxRiserHeight.AlmostZero()) 
            return new Model3D([], []);
        var flightStairsHeight = LandingVerticalSpacing - TreadThickness;
        var cnt = (int)(flightStairsHeight / MaxRiserHeight);
        if (cnt == 0)
            return new Model3D([], []);
        var riserHeight = flightStairsHeight / cnt;
        var treadGoing = LandingHorizontalSpacing / cnt;
        var stairs = new Stairs
        {
            Count = cnt,
            TreadGoing = treadGoing,
            TreadWidth = TreadWidth,
            RiserHeight = riserHeight,
            RiserWidth = TreadWidth,
            TreadThickness = TreadThickness,
            SideStringers = SideStringers,
            CenterStringer = CenterStringer,
            HasRiser = HasRiser,
        };
        return stairs.Eval();
    }

    public void CreateAllFlightsOfStairs(Model3DBuilder bldr)
    {
        var stairs = CreateOneFlightOfStairs();
        var center = stairs.GetBounds().Center;
        var reflect = CreateMirrorInXZPlane(center);
        for (var i = 0; i < Flights; i++)
        {
            var flightVerticalOffset = Vector3.UnitZ * i * LandingVerticalSpacing;
            var stairFlight = stairs.Translate<Model3D>(flightVerticalOffset) as IModel3D;
            var sideTranslation = new Vector3(LandingWidth / 2 - TreadWidth / 2, 0, 0);
            if (i % 2 == 1)
            {
                stairFlight = stairFlight.Transform(reflect).Translate(sideTranslation);
            }
            else
            {
                stairFlight = stairFlight.Translate(-sideTranslation);
            }

            bldr.AddModel(stairFlight);
        }
    }

    public Model3D Eval()
    {
        var mb = new Model3DBuilder();
        CreateLandings(mb);
        CreateAllFlightsOfStairs(mb);
        return mb.Build();
    }
}