using Ara3D.Memory;
using Ara3D.Utils;
using Ara3D.Geometry;

namespace Ara3D.Studio.Data;

public static unsafe class RenderSceneExtensions
{
    public static long GetNumBytes<T>(this IBuffer<T> self) where T : unmanaged
        => self.Count * sizeof(T);

    public static IntPtr GetIntPtr<T>(this IBuffer<T> self) where T : unmanaged
        => new(self.GetPointer());

    public static TriangleMesh3D ToMesh(this MeshSliceStruct slice, IRenderScene scene)
    {
        var minVertex = uint.MaxValue;
        var maxVertex = uint.MinValue;
        var indices = new List<int>();
        var points = new List<Point3D>();

        for (var i=0; i < slice.IndexCount; i++)
        {
            var index = slice.FirstIndex + i;
            var vertexIndex = scene.Indices[(int)index];
            indices.Add((int)vertexIndex);
            minVertex = Math.Min(minVertex, vertexIndex);
            maxVertex = Math.Max(maxVertex, vertexIndex);
        }

        for (var i=minVertex; i <= maxVertex; i++)
        {
            var vertex = scene.Vertices[(int)i];
            points.Add(new Point3D(vertex.Position.X, vertex.Position.Y, vertex.Position.Z));
        }

        var faceIndices = new List<Integer3>();
        for (var i=0; i < slice.IndexCount; i += 3)
        {
            var index1 = indices[i] - minVertex;
            var index2 = indices[i + 1] - minVertex;
            var index3 = indices[i + 2] - minVertex;
            faceIndices.Add(new Integer3((int)index1, (int)index2, (int)index3));
        }

        return (points, faceIndices);
    }

    public static void VerifyIsValidNumber(float f, string name)
    {
        if (float.IsNaN(f))
            Verifier.Assert(false, $"Value {name} is NaN");
        if (float.IsInfinity(f))
            Verifier.Assert(false, $"Value {name} is Infinity");
    }

    public static void VerifyIsAValidVector(Vector3 v, string name)
    {
        VerifyIsValidNumber(v.X.Value, $"{name}.X");
        VerifyIsValidNumber(v.Y.Value, $"{name}.Y");
        VerifyIsValidNumber(v.Z.Value, $"{name}.Z");
    }

    public static void VerifyInRange(uint index, uint max, string name)
    {
        Verifier.Assert(index < max, $"{name} is greater than {max}");
    }

    public static void VerifyInRange(int index, int min, int max, string name)
    {
        Verifier.Assert(index < max, $"{name} is greater than {max}");
        Verifier.Assert(index >= min, $"{name} is less than {min}");
    }

    public static void VerifyIsAValidQuaternion(Quaternion q, string name)
    {
        VerifyIsValidNumber(q.X, $"{name}.X");
        VerifyIsValidNumber(q.Y, $"{name}.Y");
        VerifyIsValidNumber(q.Z, $"{name}.Z");
        VerifyIsValidNumber(q.W, $"{name}.W");
    }

    public static void Validate(this IRenderScene self)
    {
        if (self == null)
            throw new Exception("RenderScene is null");

        Verifier.Assert(self.Vertices != null, "Vertices is null");
        Verifier.Assert(self.Indices != null, "Indices is null");
        Verifier.Assert(self.Meshes != null, "Meshes is null");
        Verifier.Assert(self.Instances != null, "Instances is null");
        Verifier.Assert(self.Groups != null, "Groups is null");
        Verifier.Assert(self.Vertices.Count > 0, "Vertices count is zero");
        Verifier.Assert(self.Indices.Count > 0, "Indices count is zero");
        Verifier.Assert(self.Meshes.Count > 0, "Meshes count is zero");
        Verifier.Assert(self.Instances.Count > 0, "Instances count is zero");
        Verifier.Assert(self.Groups.Count > 0, "Groups count is zero");

        for (var i = 0; i < self.Vertices.Count; i++)
        {
            VerifyIsAValidVector(self.Vertices[i].Position, $"Vertex{i}");
        }

        for (var i = 0; i < self.Indices.Count; i++)
        {
            VerifyInRange(self.Indices[i], (uint)self.Vertices.Count, "Index");
        }

        for (var i = 0; i < self.Meshes.Count; i++)
        {
            var mesh = self.Meshes[i];
            VerifyInRange(mesh.FirstIndex, (uint)self.Indices.Count, "Mesh.FirstIndex");
            VerifyInRange(mesh.IndexCount, (uint)self.Indices.Count, "Mesh.IndexCount");
            VerifyInRange(mesh.BaseVertex, 0, (int)self.Vertices.Count, "Mesh.BaseVertex");
            VerifyIsAValidVector(mesh.Bounds.Min, $"Mesh{i}.Bounds.Min");
            VerifyIsAValidVector(mesh.Bounds.Max, $"Mesh{i}.Bounds.Max");

            Verifier.Assert(mesh.IndexCount > 0, "Mesh has no indices");
        }

        for (var i = 0; i < self.Instances.Count; i++)
        {
            var instance = self.Instances[i];
            VerifyInRange(instance.MeshIndex, 0, (int)self.Meshes.Count, "Instance.MeshIndex");
            VerifyIsAValidVector(instance.Position, $"Position {i}");
            VerifyIsAValidVector(instance.Scale, $"Scale {i}");
            VerifyIsAValidQuaternion(instance.Orientation, $"Rotation {i}");
        }

        for (var i = 0; i < self.Groups.Count; i++)
        {
            var group = self.Groups[i];
            VerifyInRange(group.BaseInstance, (uint)self.Instances.Count, "Group.BaseInstance");
            VerifyInRange(group.InstanceCount, (uint)self.Instances.Count, "Group.InstanceCount");
            VerifyInRange(group.BaseInstance + group.InstanceCount, (uint)(self.Instances.Count + 1), "Group.InstanceCount");
            VerifyInRange(group.MeshIndex, (uint)self.Meshes.Count, "Group.MeshIndex");

            Verifier.Assert(group.InstanceCount > 0, "Instance has no meshes");
        }
    }

    public static void AddScene(this RenderSceneBuilder rsb, IRenderScene model)
    {
        var vertexOffset = rsb.VertexList.Count;
        var indexOffset = rsb.IndexList.Count;
        var meshOffset = rsb.MeshList.Count;
        var instOffset = rsb.Instances.Count;

        foreach (var vertex in model.Vertices)
        {
            rsb.VertexList.Add(vertex);
        }

        foreach (var index in model.Indices)
        {
            rsb.IndexList.Add((uint)(index + vertexOffset));
        }

        foreach (var mesh in model.Meshes)
        {
            var tmp = mesh;
            tmp.BaseVertex += vertexOffset;
            tmp.FirstIndex += (uint)indexOffset;
            rsb.MeshList.Add(tmp);
        }

        foreach (var instance in model.Instances)
        {
            var tmp = instance;
            tmp.MeshIndex += meshOffset;
            rsb.InstanceList.Add(tmp);
        }

        foreach (var group in model.Groups)
        {
            var tmp = group;
            tmp.MeshIndex += (uint)meshOffset;
            tmp.BaseInstance += (uint)instOffset;
            rsb.GroupList.Add(tmp);
        }
    }

    public static Bounds TotalBounds(this IRenderScene self)
        => self.Instances.Count == 0 ? Bounds.Empty : self.Instances.Select(x => x.Bounds)
            .Aggregate((a, b) => a.Union(b));
}