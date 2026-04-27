
namespace Ara3D.Studio.Samples.Generators
{
    [Category(nameof(Categories.Lattices))]
    public class CylindricalLattice : IGenerator
    {
        [Range(3, 100)] public int RadialSides = 16;
        [Range(1, 128)] public int VerticalSegments = 4;
        [Range(0.01f, 100f)] public float Height = 5f;
        [Range(0.01f, 100f)] public float InnerRadius = 1f;
        [Range(0.01f, 10f)] public float RadialThickness = 0.1f;

        public QuadMesh3D Eval()
        {
            var shape = new CellGridBuilder3D(
                RadialSides * 3,
                1,
                VerticalSegments * 3);

            for (var x = 0; x < shape.SizeX; x++)
            {
                for (var z = 0; z < shape.SizeZ; z++)
                {
                    if (x % 3 == 1 && z % 3 == 1)
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
