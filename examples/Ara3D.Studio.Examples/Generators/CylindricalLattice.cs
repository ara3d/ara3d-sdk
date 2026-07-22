
namespace Ara3D.Studio.Samples.Generators
{
    [Category(Cat.Structures)]
    [Description("A cylindrical lattice of radial and vertical bars, with configurable cell resolution and bar thickness.")]
    public class CylindricalLattice : IGenerator
    {
        [Range(3, 100)] public int RadialSides = 16;
        [Range(1, 128)] public int VerticalSegments = 4;

        [Range(1, 32)] public int RadialCellResolution = 6;
        [Range(1, 32)] public int VerticalCellResolution = 6;

        [Range(1, 31)] public int RadialBarThickness = 1;
        [Range(1, 31)] public int VerticalBarThickness = 1;

        [Range(0.01f, 100f)] public float Height = 5f;
        [Range(0.01f, 100f)] public float InnerRadius = 1f;
        [Range(0.01f, 10f)] public float RadialThickness = 0.1f;

        public QuadMesh3D Eval()
        {
            RadialCellResolution = Math.Max(1, RadialCellResolution);
            VerticalCellResolution = Math.Max(1, VerticalCellResolution);

            RadialBarThickness = Math.Clamp(
                RadialBarThickness,
                1,
                Math.Max(1, RadialCellResolution / 2));

            VerticalBarThickness = Math.Clamp(
                VerticalBarThickness,
                1,
                Math.Max(1, VerticalCellResolution / 2));

            var shape = new CellGridBuilder3D(
                RadialSides * RadialCellResolution,
                1,
                VerticalSegments * VerticalCellResolution);

            for (var x = 0; x < shape.SizeX; x++)
            {
                var rx = x % RadialCellResolution;

                var inRadialOpening =
                    rx >= RadialBarThickness &&
                    rx < RadialCellResolution - RadialBarThickness;

                for (var z = 0; z < shape.SizeZ; z++)
                {
                    var rz = z % VerticalCellResolution;

                    var inVerticalOpening =
                        rz >= VerticalBarThickness &&
                        rz < VerticalCellResolution - VerticalBarThickness;

                    if (inRadialOpening && inVerticalOpening)
                        shape.Remove(x, 0, z);
                }
            }

            var vertices = new List<Point3D>(
                (shape.SizeX + 1) *
                (shape.SizeY + 1) *
                (shape.SizeZ + 1));

            for (var x = 0; x <= shape.SizeX; x++)
            {
                var angle = MathF.Tau * x / shape.SizeX;
                var cos = MathF.Cos(angle);
                var sin = MathF.Sin(angle);

                for (var y = 0; y <= shape.SizeY; y++)
                {
                    var radius = InnerRadius + y * RadialThickness;

                    for (var z = 0; z <= shape.SizeZ; z++)
                    {
                        var height = ((float)z / shape.SizeZ - 0.5f) * Height;

                        vertices.Add(new Point3D(
                            cos * radius,
                            sin * radius,
                            height));
                    }
                }
            }

            return new QuadMesh3D(vertices, shape.GetQuadFaces());
        }
    }
}
