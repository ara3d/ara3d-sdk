namespace Ara3D.Studio.Samples.Modifiers
{
    public class VertexColorSample : IModifier
    {
        public Palettes Palette { get; set; }

        private ScalarStatistics _stats { get; set; }

        public static Vector3 VertexColor(Color[] colors, ScalarStatistics stats, double v)
            => colors.GetColor((float)stats.Normalize(v)).ToVector3();

        public float Min => (float)(_stats?.Min ?? float.NaN);
        public float Max => (float)(_stats?.Max ?? float.NaN);
        public float Avg => (float)(_stats?.Average ?? float.NaN);
        public float Plus3StdDev => (float)(_stats?.Plus3StdDev ?? float.NaN);
        public float Minus3StdDev => (float)(_stats?.Minus3StdDev ?? float.NaN);

        public VertexAttributeEnum Feature { get; set; }

        public ColoredTriangleMesh3D Eval(RenderModelData data)
        {
            var mesh = data.ToModel3D().Meshes[0];
            var topology = new Topology(mesh);
            var mac = new MeshAttributes(mesh, topology);
            var values = mac.Vertices.GetAttribute(Feature);
            var colors = Palette.GetColors();
            _stats = values.Statistics();
            var vertexColors = values.Select(c => VertexColor(colors, _stats, c));
            return new ColoredTriangleMesh3D(mesh, vertexColors);
        }
    }
}
