namespace Ara3D.Studio.Samples.Modifiers
{
    [Category(Cat.Color)]
    [Description("Colors mesh vertices from a palette by a chosen vertex attribute, with optional standard-deviation scaling.")]
    public class VertexColorSample : IModifier
    {
        public Palettes Palette { get; set; }

        private ScalarStatistics _stats { get; set; }

        public Vector3 VertexColor(Color[] colors, double v)
            => colors.GetColor(Normalize(v)).ToVector3();

        public bool UseStdDev { get; set; }
        [Range(0, 5)] float StdDevRange { get; set; } = 2;

        public float Min => (float)(_stats?.Min ?? float.NaN);
        public float Max => (float)(_stats?.Max ?? float.NaN);
        public float Avg => (float)(_stats?.Average ?? float.NaN);
        public float StdDev => (float)(_stats?.StdDev ?? float.NaN);
        
        public VertexAttributeEnum Feature { get; set; }

        public float Normalize(double v)
        {
            var min = UseStdDev ? Avg - StdDevRange * StdDev : Min;
            var max = UseStdDev ? Avg + StdDevRange * StdDev : Max;
            var range = max - min;
            if (range == 0) return 0;
            return (float)Math.Clamp((v - min) / range, 0.0, 1.0);
        }

        public ColoredTriangleMesh3D Eval(IModel3D model)
        {
            var mesh = model.ToColoredMesh();
            var mac = mesh.Mesh.DerivedAttributes();
            var values = mac.Vertices.GetAttribute(Feature);
            var colors = Palette.GetColors();
            _stats = values.Statistics();
            var vertexColors = values.Select(c => VertexColor(colors, c));
            return new ColoredTriangleMesh3D(mesh.Mesh, vertexColors);
        }
    }
}
