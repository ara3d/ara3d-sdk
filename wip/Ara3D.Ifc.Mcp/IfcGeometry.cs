using Ara3D.Geometry;
using Ara3D.Ifc.Mesher;
using Ara3D.IfcLoader;
using Ara3D.Models;

namespace Ara3D.Ifc.Mcp;

public readonly record struct IfcVec(double X, double Y, double Z)
{
    public static IfcVec From(Vector3 v)
        => new(v.X, v.Y, v.Z);

    public static IfcVec From(Point3D p)
        => new(p.X, p.Y, p.Z);
}

/// <summary>An axis-aligned bounding box, reported four ways so a caller does not have to derive one
/// from another.</summary>
public readonly record struct IfcBox(IfcVec Min, IfcVec Max, IfcVec Center, IfcVec Size)
{
    public static IfcBox From(Bounds3D b)
        => new(IfcVec.From(b.Min), IfcVec.From(b.Max), IfcVec.From(b.Center), IfcVec.From(b.Size));
}

/// <summary>The geometry of one element, aggregated over every mesh instance the mesher emitted for
/// it. <paramref name="SignedVolume"/> is the divergence-theorem volume of the closed triangle soup
/// (negative if faces wind inward); <paramref name="SurfaceArea"/> is the summed triangle area.</summary>
public readonly record struct IfcElementGeometry(
    int Id,
    string Type,
    string Name,
    int InstanceCount,
    int TriangleCount,
    int VertexCount,
    double SignedVolume,
    double SurfaceArea,
    IfcBox Bounds);

/// <summary>Measures the meshes a session produced. The Approach1 mesher already ran by the time any
/// of this is called (via <see cref="IfcSession.Meshing"/>), so the work here is only transform,
/// grouping and accumulation over an in-memory <see cref="Model3D"/>.</summary>
public static class IfcGeometry
{
    public static Model3D Model(this IfcSession session)
        => session.Meshing.Model ?? Model3D.Empty;

    /// <summary>One entry per element that produced geometry, in first-seen instance order. When
    /// <paramref name="ids"/> is given only those entities are measured; ids that meshed nothing are
    /// simply absent from the result.</summary>
    public static IReadOnlyList<IfcElementGeometry> Elements(this IfcSession session, IReadOnlyList<int>? ids)
    {
        var model = session.Model();
        var filter = ids == null ? null : new HashSet<int>(ids);
        var groups = new Dictionary<int, Accumulator>();
        var order = new List<int>();

        foreach (var instance in model.Instances)
        {
            if (instance.MeshIndex < 0 || instance.MeshIndex >= model.Meshes.Count)
                continue;

            var id = instance.EntityIndex;
            if (filter != null && !filter.Contains(id))
                continue;

            if (!groups.TryGetValue(id, out var accumulator))
            {
                accumulator = new Accumulator();
                groups[id] = accumulator;
                order.Add(id);
            }

            accumulator.Add(model.Meshes[instance.MeshIndex], instance.Matrix4x4);
        }

        var result = new IfcElementGeometry[order.Count];
        for (var i = 0; i < order.Count; i++)
        {
            var id = order[i];
            var accumulator = groups[id];
            var entity = session.Resolver.GetEntity(id);
            var bounds = accumulator.Points.Count == 0 ? Bounds3D.Empty : accumulator.Points.Bounds();
            result[i] = new IfcElementGeometry(
                id,
                entity.GetEntityName(),
                entity.GetEntityLabel(),
                accumulator.Instances,
                accumulator.Triangles,
                accumulator.Points.Count,
                accumulator.Volume,
                accumulator.Area,
                IfcBox.From(bounds));
        }

        return result;
    }

    /// <summary>The same model with only the instances of the given entities kept. Meshes are shared
    /// unchanged — an unreferenced mesh costs nothing to leave in the list — so a filtered export
    /// stays byte-for-byte the geometry the mesher produced for those elements.</summary>
    public static Model3D Filtered(this IfcSession session, IReadOnlyList<int>? ids)
    {
        var model = session.Model();
        if (ids == null)
            return model;

        var filter = new HashSet<int>(ids);
        var instances = new List<InstanceStruct>();
        foreach (var instance in model.Instances)
            if (filter.Contains(instance.EntityIndex))
                instances.Add(instance);

        return new Model3D(model.Meshes, instances);
    }

    private sealed class Accumulator
    {
        public readonly List<Point3D> Points = [];
        public int Instances;
        public int Triangles;
        public double Volume;
        public double Area;

        public void Add(TriangleMesh3D mesh, Matrix4x4 matrix)
        {
            Instances++;
            var source = mesh.Points;
            var world = new Point3D[source.Count];
            for (var i = 0; i < source.Count; i++)
            {
                var v = source[i].Vector3.Transform(matrix);
                world[i] = new Point3D(v.X, v.Y, v.Z);
            }

            Points.AddRange(world);
            foreach (var face in mesh.FaceIndices)
            {
                var a = world[face.A].Vector3;
                var b = world[face.B].Vector3;
                var c = world[face.C].Vector3;
                Volume += a.Dot(b.Cross(c)) / 6.0;
                Area += b.Subtract(a).Cross(c.Subtract(a)).Length / 2.0;
                Triangles++;
            }
        }
    }
}
