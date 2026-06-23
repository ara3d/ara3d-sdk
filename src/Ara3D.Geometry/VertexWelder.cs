namespace Ara3D.Geometry;

public static class VertexWelder
{
    public readonly record struct VertexKey(long X, long Y, long Z)
    {
        public const long Multiplier = 100_000; 
        public static VertexKey Create(Vector3 v)
            => new((long)(v.X * Multiplier), (long)(v.Y * Multiplier), (long)(v.Z * Multiplier));
    }

    public static TriangleMesh3D WeldVertices(this TriangleMesh3D mesh)
    {
        var vertexLookup = new Dictionary<VertexKey, int>();
        var indexLookup = new Dictionary<int, int>();
        var newPoints = new List<Point3D>();
        for (var i = 0; i < mesh.Points.Count; ++i)
        {
            var key = VertexKey.Create(mesh.Points[i].Vector3);
            if (vertexLookup.TryGetValue(key, out var index))
            {
                indexLookup[i] = index;
            }
            else
            {
                indexLookup[i] = newPoints.Count;
                vertexLookup[key] = vertexLookup.Count;
                newPoints.Add(mesh.Points[i]);
            }
        }
        var newFaceIndices = mesh.FaceIndices.Select(i => new Integer3(
            indexLookup[i.A], indexLookup[i.B], indexLookup[i.C])).ToList();
        return new TriangleMesh3D(newPoints, newFaceIndices);
    }
}