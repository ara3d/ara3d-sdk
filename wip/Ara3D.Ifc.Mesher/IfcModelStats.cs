using Ara3D.Geometry;
using Ara3D.Models;

namespace Ara3D.Ifc.Mesher;

public sealed record IfcModelGeometryStats(
    int InstanceCount,
    int MeshCount,
    int TriangleCount,
    Bounds3D Bounds,
    double SignedVolume);

public static class IfcModelStats
{
    public static IfcModelGeometryStats FromModel(Model3D model)
    {
        var meshes = model.Meshes;
        var allPoints = new List<Point3D>();
        var triCount = 0;
        double volume = 0;

        foreach (var inst in model.Instances)
        {
            if (inst.MeshIndex < 0 || inst.MeshIndex >= meshes.Count)
                continue;

            var mesh = meshes[inst.MeshIndex];
            var transformed = Transform(mesh, inst.Matrix4x4);
            allPoints.AddRange(transformed.Points);
            triCount += transformed.FaceIndices.Count;
            volume += SignedVolume(transformed);
        }

        var bounds = allPoints.Count == 0 ? Bounds3D.Empty : allPoints.Bounds();
        return new IfcModelGeometryStats(model.Instances.Count, model.Meshes.Count, triCount, bounds, volume);
    }

    static TriangleMesh3D Transform(TriangleMesh3D mesh, Matrix4x4 matrix)
    {
        var points = mesh.Points.Select(p =>
        {
            var v = p.Vector3.Transform(matrix);
            return new Point3D(v.X, v.Y, v.Z);
        }).ToList();
        return new TriangleMesh3D(points, mesh.FaceIndices);
    }

    static double SignedVolume(TriangleMesh3D mesh)
    {
        double volume = 0;
        foreach (var face in mesh.FaceIndices)
        {
            var a = mesh.Points[face.A].Vector3;
            var b = mesh.Points[face.B].Vector3;
            var c = mesh.Points[face.C].Vector3;
            volume += Vector3.Dot(a, Vector3.Cross(b, c)) / 6.0;
        }
        return volume;
    }
}
