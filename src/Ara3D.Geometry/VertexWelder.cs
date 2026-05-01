namespace Ara3D.Geometry;

public static class VertexWelder
{
    public readonly record struct VertexKey(int X, int Y, int Z)
    {
        public const int Multiplier = 10_000; 
        public static VertexKey Create(Vector3 v)
            => new((int)(v.X * Multiplier), (int)(v.Y * Multiplier), (int)(v.Z * Multiplier));
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