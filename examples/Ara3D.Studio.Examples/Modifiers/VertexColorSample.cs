namespace Ara3D.Studio.Samples.Modifiers
{
    public class VertexColorSample : IModifier
    {
        public Palettes Palette { get; set; }

        public static Vector3 VertexColor(Color[] colors, ScalarStatistics stats, double curvature)
            => colors.GetColor((float)stats.Normalize(curvature)).ToVector3();

        public enum AttributeEnum
        {
            X,
            Y,
            Z,
            Curvature,
            Valence,
            NormalX,
            NormalY,
            NormalZ,
            IsBoundary,
            AverageEdgeLength,
            AverageFaceArea,
            AverageDihedralAngle,
        }

        public AttributeEnum Attribute { get; set; }


        public IReadOnlyList<double> GetValues(Topology topology)
        {
            switch (Attribute)
            {
                case AttributeEnum.X:
                    return topology.GetVertexIds().Select(id => (double)topology.GetPoint(id).X);
                case AttributeEnum.Y:
                    return topology.GetVertexIds().Select(id => (double)topology.GetPoint(id).Y);
                case AttributeEnum.Z:
                    return topology.GetVertexIds().Select(id => (double)topology.GetPoint(id).Z);
                case AttributeEnum.Curvature:
                    return topology.GetVertexIds().Select(id => (double)topology.GetCurvature(id));
                case AttributeEnum.Valence:
                    return topology.GetVertexIds().Select(id => (double)topology.Valence(id));
                case AttributeEnum.NormalX:
                    return topology.GetVertexIds().Select(id => (double)topology.GetVertexNormal(id, Topology.VertexNormalWeighting.UniformFace).X);
                case AttributeEnum.NormalY:
                    return topology.GetVertexIds().Select(id => (double)topology.GetVertexNormal(id, Topology.VertexNormalWeighting.UniformFace).Y);
                case AttributeEnum.NormalZ:
                    return topology.GetVertexIds().Select(id => (double)topology.GetVertexNormal(id, Topology.VertexNormalWeighting.UniformFace).Z);
                case AttributeEnum.IsBoundary:
                    return topology.GetVertexIds().Select(id => topology.IsBoundary(id) ? 1.0 : 0.0);
                case AttributeEnum.AverageEdgeLength:
                    return topology.GetVertexIds().Select(id => topology.GetEdges(id).Average(e => (double)e.Length));
                case AttributeEnum.AverageFaceArea:
                    return topology.GetVertexIds().Select(id => topology.GetFaces(id).Average(e => (double)e.Area));
                case AttributeEnum.AverageDihedralAngle:
                    return topology.GetVertexIds().Select(id => topology.GetEdges(id).Average(e => (double)e.DihedralAngle));
            }

            return topology.GetVertexIds().Select(id => 0.0);
        }

        public ColoredTriangleMesh3D Eval(RenderModelData data)
        {
            var mesh = data.ToModel3D().Meshes[0];
            var topology = new Topology(mesh);
            var values = GetValues(topology);
            var colors = Palette.GetColors();
            var stats = values.Statistics();
            var vertexColors = values.Select(c => VertexColor(colors, stats, c));
            return new ColoredTriangleMesh3D(mesh, vertexColors);
        }
    }
}
