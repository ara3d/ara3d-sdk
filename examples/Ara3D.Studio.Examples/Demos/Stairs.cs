namespace Ara3D.Studio.Samples.Demos;

[Category(nameof(Categories.Demos))]
public class Stairs : IGenerator
{
    public Material TreadMaterial { get; set; } = new(new(0.9f, 0.7f, 0f, 1f), 0f, 0.8f);
    public Material RiserMaterial { get; set; } = new(new(0.9f, 0.9f, 1f, 1f), 0f, 0.5f);
    public bool HasRiser { get; set; } = true;

    [Range(0f, 1f)] public float NosingOverhang { get; set; } = 0.05f;
    [Range(0f, 4f)] public float StairWidth { get; set; } = 0.8f;
    [Range(0f, 0.5f)] public float RiserHeight { get; set; } = 0.2f;
    [Range(1, 100)] public int Count { get; set; } = 10;
    [Range(0f, 2f)] public float TreadGoing { get; set; } = 0.4f;
    [Range(0f, 1f)] public float TreadThickness { get; set; } = 0.05f;
    [Range(0f, 1f)] public float RiserThickness { get; set; } = 0.05f;
    public float TreadDepth => TreadGoing + NosingOverhang;

    [Range(0.01f, 2f)] public float StringerFromTop{ get; set; } = 0.20f;
    [Range(0.01f, 2f)] public float StringerFromFront { get; set; } = 0.20f;
    [Range(0.01f, 0.5f)] public float StringerThickness { get; set; } = 0.04f;
    
    public IModel3D Eval(EvalContext context)
    {
        var box = PlatonicSolids.TriangulatedCube;

        var riserScale = new Vector3(StairWidth, RiserThickness, RiserHeight);
        var treadScale = new Vector3(StairWidth, TreadDepth, TreadThickness);
        
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

            treads.Add(new InstanceStruct(0, treadMatrix, 0, TreadMaterial, 0));
            
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
        var inst = new InstanceStruct(-1, Matrix4x4.Identity, 1, RiserMaterial, 0);
        instances.Add(inst);

        return new Model3D([box, stringer], instances);
    }

    public TriangleMesh3D CreateStringer(IReadOnlyList<Point3D> points)
    {
        var pointsA = points.Translate(Vector3.UnitX * -(StringerThickness / 2f));
        var pointsB = pointsA.Translate(Vector3.UnitX * StringerThickness);
        return pointsA.ToQuadGrid3D(pointsB, true, true).Triangulate();
    }

}