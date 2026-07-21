namespace Ara3D.Geometry;

/// <summary>
/// A mesh extracted from a larger mesh, with maps back to the source:
/// SourceVertices[i] / SourceFaces[i] give the source index of vertex/face i.
/// </summary>
public readonly record struct SubMesh(
    TriangleMesh3D Mesh,
    IReadOnlyList<int> SourceVertices,
    IReadOnlyList<int> SourceFaces);

public static class MeshSelectionOps
{
    public static bool[] ToMask(this IReadOnlyList<int> indices, int count)
    {
        var mask = new bool[count];
        for (var i = 0; i < indices.Count; i++)
            mask[indices[i]] = true;
        return mask;
    }

    public static int[] Complement(this IReadOnlyList<int> indices, int count)
    {
        var mask = indices.ToMask(count);
        var result = new int[count - indices.Count];
        var j = 0;
        for (var i = 0; i < count; i++)
            if (!mask[i])
                result[j++] = i;
        return result;
    }

    public static SubMesh ExtractFaces(this TriangleMesh3D mesh, IReadOnlyList<int> faces)
    {
        var vertexMap = new int[mesh.Points.Count];
        Array.Fill(vertexMap, -1);
        var sourceVertices = new List<int>();
        var newPoints = new List<Point3D>();
        var newFaces = new Integer3[faces.Count];
        for (var i = 0; i < faces.Count; i++)
        {
            var face = mesh.FaceIndices[faces[i]];
            newFaces[i] = new Integer3(
                RemapVertex(mesh, vertexMap, sourceVertices, newPoints, face.A),
                RemapVertex(mesh, vertexMap, sourceVertices, newPoints, face.B),
                RemapVertex(mesh, vertexMap, sourceVertices, newPoints, face.C));
        }
        return new(new(newPoints, newFaces), sourceVertices, faces);
    }

    private static int RemapVertex(TriangleMesh3D mesh, int[] vertexMap, List<int> sourceVertices, List<Point3D> newPoints, int oldIndex)
    {
        if (vertexMap[oldIndex] >= 0)
            return vertexMap[oldIndex];
        var newIndex = newPoints.Count;
        vertexMap[oldIndex] = newIndex;
        sourceVertices.Add(oldIndex);
        newPoints.Add(mesh.Points[oldIndex]);
        return newIndex;
    }

    public static TriangleMesh3D DeleteFaces(this TriangleMesh3D mesh, IReadOnlyList<int> faces)
        => mesh.ExtractFaces(faces.Complement(mesh.FaceIndices.Count)).Mesh;

    public static (SubMesh Selected, SubMesh Remaining) SplitByFaces(this TriangleMesh3D mesh, IReadOnlyList<int> faces)
        => (mesh.ExtractFaces(faces), mesh.ExtractFaces(faces.Complement(mesh.FaceIndices.Count)));
}
