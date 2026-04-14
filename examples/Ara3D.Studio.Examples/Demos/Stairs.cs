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
        var meshIndex = bldr.AddMesh(landingMesh);
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

[Category(nameof(Categories.Demos))]
public class Stairs : IGenerator
{
    public Material TreadMaterial { get; set; } = new(new(0.9f, 0.7f, 0f, 1f), 0f, 0.8f);
    public Material RiserMaterial { get; set; } = new(new(0.9f, 0.9f, 1f, 1f), 0f, 0.5f);
    public Material StringerMaterial { get; set; } = new(new(0.8f, 0.8f, 0f, 1f), 0f, 0.9f);
    public bool HasRiser { get; set; } = true;

    [Range(0f, 1f)] public float NosingOverhang { get; set; } = 0.05f;
    [Range(0f, 4f)] public float TreadWidth { get; set; } = 0.8f;
    [Range(0f, 4f)] public float RiserWidth { get; set; } = 0.8f;
    [Range(0f, 0.5f)] public float RiserHeight { get; set; } = 0.2f;
    [Range(1, 100)] public int Count { get; set; } = 10;
    [Range(0f, 2f)] public float TreadGoing { get; set; } = 0.4f;
    [Range(0f, 1f)] public float TreadThickness { get; set; } = 0.05f;
    [Range(0f, 1f)] public float RiserThickness { get; set; } = 0.05f;
    public float TreadDepth => TreadGoing + NosingOverhang;

    [Range(0.0f, 2f)] public float StringerFromTop{ get; set; } = 0.20f;
    [Range(0.0f, 2f)] public float StringerFromFront { get; set; } = 0.20f;
    [Range(0.0f, 1f)] public float StringerThickness { get; set; } = 0.04f;

    public bool CenterStringer { get; set; } = true;
    public bool SideStringers { get; set; } = true;
    [Range(0f, 1f)] public float SideStringerOffset { get; set; } = 0.15f;

    public Model3D Eval()
    {
        var box = PlatonicSolids.TriangulatedCube;

        var riserScale = new Vector3(RiserWidth, RiserThickness, RiserHeight);
        var treadScale = new Vector3(TreadWidth, TreadDepth, TreadThickness);
        
        var riserScaleMatrix = Matrix4x4.CreateScale(riserScale.X, riserScale.Y, riserScale.Z);
        var treadScaleMatrix = Matrix4x4.CreateScale(treadScale.X, treadScale.Y, treadScale.Z);

        var risers = new List<InstanceStruct>();
        var treads = new List<InstanceStruct>();

        var profile = new List<Point3D>();

        for (var i = 0; i < Count; i++)
        {
            var riserFront = i * TreadGoing;
            var riserBack = riserFront + RiserThickness;
            var riserBottom = i * RiserHeight;
            var riserTop = riserBottom + RiserHeight;

            var riserCenter = new Point3D(0, riserFront.Average(riserBack), riserBottom.Average(riserTop));

            var treadBottom = riserTop;
            var treadTop = treadBottom + TreadThickness;
            var treadFront = riserFront - NosingOverhang;
            var treadBack = treadFront + TreadDepth;

            var treadCenter = new Point3D(0, treadFront.Average(treadBack), treadBottom.Average(treadTop));
                
            var riserMatrix = riserScaleMatrix * Matrix4x4.CreateTranslation(riserCenter);
            var treadMatrix = treadScaleMatrix * Matrix4x4.CreateTranslation(treadCenter);

            if (HasRiser)
                risers.Add(new InstanceStruct(0, riserMatrix, 0, RiserMaterial, 0));

            if (i < Count)
            {
                treads.Add(new InstanceStruct(0, treadMatrix, 0, TreadMaterial, 0));
            }
            
            profile.Add((0, riserBack, riserBottom));
            profile.Add((0, riserBack, riserTop));
            profile.Add((0, treadBack, treadBottom));
        }

        if (Count > 0)
        {
            var first = profile[0];
            var last = profile[^1];
            profile.Add((0, last.Y, last.Z - StringerFromTop));
            profile.Add((0, first.Y + StringerFromFront, first.Z));
        }

        var instances = risers.Concat(treads).ToList();

        var stringer = CreateStringer(profile);

        if (CenterStringer)
        {
            var inst = new InstanceStruct(-1, Matrix4x4.Identity, 1, StringerMaterial, 0);
            instances.Add(inst);
        }

        if (SideStringers)
        {
            var xA = -TreadWidth / 2 + SideStringerOffset;
            var xB = TreadWidth / 2 - SideStringerOffset;
            var instA = new InstanceStruct(-1, Matrix4x4.CreateTranslation((xA, 0, 0)), 1, StringerMaterial, 0);
            var instB = new InstanceStruct(-1, Matrix4x4.CreateTranslation((xB, 0, 0)), 1, StringerMaterial, 0);
            instances.AddRange([instA, instB]);
        }

        return new Model3D([box, stringer], instances);
    }

    public TriangleMesh3D CreateStringer(IReadOnlyList<Point3D> points)
    {
        var pointsA = points.Translate(Vector3.UnitX * -(StringerThickness / 2f));
        var pointsB = pointsA.Translate(Vector3.UnitX * StringerThickness);
        
        var vector2ds = points.Select(p => new Vector2(p.Y, p.Z)).ToList();
        var indices = PolygonTriangulator.Triangulate(vector2ds);

        // Indices of the Top 
        var indicesA = indices.Select(f => new Integer3(f.C, f.B, f.A));
        var n = pointsA.Count;

        // Indices of the Bottom
        var indicesB = indices.Select(f => new Integer3(f.A + n, f.B + n, f.C + n));

        // TODO: add two more triangles for the end
        //newTopIndices.Add()

        var triMesh = pointsA.ToQuadGrid3D(pointsB, true, true).Triangulate();

        var finalPoints = triMesh.Points;
        var finalIndices = triMesh.FaceIndices.Concat(indicesA).Concat(indicesB);

        return new(finalPoints, finalIndices);
    }

}
