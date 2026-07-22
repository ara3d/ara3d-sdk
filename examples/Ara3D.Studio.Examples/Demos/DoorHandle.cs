namespace Ara3D.Studio.Samples.Demos;

[Category(Cat.ExperimentalDemos)]
[Description("A parametric door handle and back-plate built as a revolved surface.")]
public class DoorHandle : IGenerator
{
    [Range(3, 64)] public int RadialSegments { get; set; } = 17;
    [Range(3, 64)] public int AngularSegments { get; set; } = 5;
    [Range(0, 0.5f)] public float PlateRadius { get; set; } = 0.06f;
    [Range(0, 0.25f)] public float PlateThickness { get; set; } = 0.02f;
    [Range(0, 0.1f)] public float HandleRadius { get; set; } = 0.02f;
    [Range(0, 0.25f)] public float HandleExtrude { get; set; } = 0.10f;
    [Range(0, 0.25f)] public float HandleLength { get; set; } = 0.15f;
    [Range(-0.5f, +0.5f)] public float TurningX { get; set; } = -0.02f;

    public QuadMesh3D Eval()
    {
        var plateScale = new Vector3(PlateRadius / 2f, 1, PlateRadius / 2f);
        var handleScale = new Vector3(HandleRadius / 2f, 1, HandleRadius / 2f);

        var ring0 = Polygons.RegularPolygon(RadialSegments).Points.Map(p => new Point3D(p.X, 0, p.Y));
        var plateRing = ring0.Scale(plateScale);
        var handleRing = ring0.Scale(handleScale);

        var rings = new List<IReadOnlyList<Point3D>>();
        rings.Add(plateRing);
        rings.Add(plateRing.Translate(Vector3.UnitY * PlateThickness));
        rings.Add(handleRing.Translate(Vector3.UnitY * PlateThickness));
        var yOffset = PlateThickness + HandleExtrude;

        var turningRing = handleRing.Translate(Vector3.UnitY * yOffset);
        var turningPoint = new Point3D(TurningX, yOffset, 0);
        for (var i=0; i <= AngularSegments; i++)
        {
            var t = (float)i / AngularSegments;
            var angle = t * 90f.Degrees();
            var angledRing = turningRing.Rotate(angle, Vector3.UnitZ, -turningPoint.Vector3);
            rings.Add(angledRing);
        }

        var lastRing = rings[^1].Translate(new Vector3(-HandleLength, 0, 0));
        rings.Add(lastRing);

        return rings.RowsToArray().ToQuadGrid3D(false, false).ToQuadMesh3D();
    }
}