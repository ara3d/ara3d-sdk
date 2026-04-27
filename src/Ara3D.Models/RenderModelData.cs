using System.Diagnostics;
using Ara3D.Collections;
using Ara3D.Geometry;
using Ara3D.Memory;
using System.Runtime.InteropServices;

namespace Ara3D.Models;

/// <summary>
/// This is a long-lived data structure that contains the memory data buffers that contain
/// the data for rendering models. It minimizes reallocation. Memory is unmanaged, and not
/// touched by the garbage collector. This is also the data structure used when storing loaded data.
/// All meshes in this structure must have the same PrimitiveKind (e.g., all Lines or all Triangles)
/// </summary>
public class RenderModelData : IDisposable
{
    public int UseVertexColorsFlag = 0x1;

    // 48 bytes
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MetaData
    {
        // 6 floats = 24 bytes
        public Bounds3D TotalBounds;
        // 8 bytes
        public long TotalVertexCount;
        // 8 bytes
        public long TotalFaceCount;
        // 4 bytes 
        public int PrimitiveSize;
        // 4 bytes 
        public int Flags;
    }

    public MetaData Meta;
    public UnmanagedList<float> VertexData { get; }
    public UnmanagedList<uint> IndexData { get; }
    public UnmanagedList<MeshSliceStruct> MeshSliceData { get; }
    public UnmanagedList<InstanceStruct> InstanceData { get; }
    public UnmanagedList<Bounds3D> MeshBoundsData { get; }
    public UnmanagedList<Bounds3D> InstanceBoundsData { get; }

    public int PrimitiveSize => Meta.PrimitiveSize;
    public Bounds3D TotalBounds => Meta.TotalBounds;
    public long TotalVertexCount => Meta.TotalVertexCount;
    public long TotalFaceCount => Meta.TotalFaceCount;
    public int VertexCount => VertexData.Count;
    public int IndexCount => IndexData.Count;
    public int FaceCount => IndexCount / PrimitiveSize;
    public int MeshCount => MeshSliceData.Count;
    public int InstanceCount => InstanceData.Count;
    public bool UseVertexColors => (Meta.Flags & UseVertexColorsFlag) != 0;

    public RenderModelData(int primSize)
    {
        if (primSize < 1 || primSize > 4)
            throw new Exception($"Render model data primitive size ({primSize}) must be from 1 to 4 inclusive");

        Meta.PrimitiveSize = primSize;
        VertexData = new();
        IndexData = new();
        MeshSliceData = new();
        InstanceData = new();
        MeshBoundsData = new();
        InstanceBoundsData = new();
    }
    
    public Model3D ToModel3D()
    {
        if (PrimitiveSize != 3) throw new Exception("Not a triangle mesh");
        var meshes = MeshSliceData.Select(GetMesh);
        return new Model3D(meshes, InstanceData);
    }

    public void Dispose()
    {
        VertexData?.Dispose();
        IndexData?.Dispose();
        MeshSliceData?.Dispose();
        InstanceData?.Dispose();
        MeshBoundsData?.Dispose();
        InstanceBoundsData?.Dispose();
    }

    public void Clear()
    {
        VertexData?.Clear();
        IndexData?.Clear();
        MeshSliceData?.Clear();
        InstanceData?.Clear();
        MeshBoundsData?.Clear();
        InstanceBoundsData?.Clear();
        Meta.TotalBounds = Bounds3D.Empty;
    }

    public void Update(
        IBuffer<float> vertexBuffer, 
        IBuffer<uint> indexBuffer, 
        IBuffer<MeshSliceStruct> meshSlices,
        IBuffer<InstanceStruct> instances)
    {
        VertexData.CopyFrom(vertexBuffer);
        IndexData.CopyFrom(indexBuffer);
        MeshSliceData.CopyFrom(meshSlices);
        InstanceData.CopyFrom(instances);
        RecomputeTotalBounds();
    }

    public void UpdateVertexBuffer(IBuffer<float> vertexBuffer) => VertexData.CopyFrom(vertexBuffer);
    public void UpdateIndexBuffer(IBuffer<uint> indexBuffer) => IndexData.CopyFrom(indexBuffer);
    public void UpdateMeshBuffer(IBuffer<MeshSliceStruct> meshBuffer) => MeshSliceData.CopyFrom(meshBuffer);
    public void UpdateInstanceBuffer(IBuffer<InstanceStruct> instanceBuffer) => InstanceData.CopyFrom(instanceBuffer);

    public void UpdateVertexBuffer(IReadOnlyList<float> vertexBuffer) => VertexData.CopyFrom(vertexBuffer);
    public void UpdateIndexBuffer(IReadOnlyList<uint> indexBuffer) => IndexData.CopyFrom(indexBuffer);
    public void UpdateMeshBuffer(IReadOnlyList<MeshSliceStruct> meshBuffer) => MeshSliceData.CopyFrom(meshBuffer);
    public void UpdateInstanceBuffer(IReadOnlyList<InstanceStruct> instanceBuffer) => InstanceData.CopyFrom(instanceBuffer);

    public void Update(RenderModelData data)
    {
        Meta.PrimitiveSize = data.PrimitiveSize; 
        UpdateVertexBuffer(data.VertexData);
        UpdateIndexBuffer(data.IndexData);
        UpdateMeshBuffer(data.MeshSliceData);
        UpdateInstanceBuffer(data.InstanceData);
        ValidateMeshSlices();
        MeshBoundsData.Clear();
        MeshBoundsData.AddRange(data.MeshBoundsData);
        InstanceBoundsData.Clear();
        InstanceBoundsData.AddRange(data.InstanceBoundsData);
        Meta.TotalBounds = data.TotalBounds;
        Meta.TotalVertexCount = data.TotalVertexCount;
        Meta.TotalFaceCount = data.TotalFaceCount;
        Meta.Flags = data.Meta.Flags;
    }

    public void SetFeatures(int primSize, bool enabled)
    {
        Meta.PrimitiveSize = primSize;
        Meta.Flags = enabled
            ? Meta.Flags | 0x1
            : Meta.Flags & ~UseVertexColorsFlag;
    }

    public void Update(LineMesh3D mesh, IReadOnlyList<Vector3> colors, Material material, Matrix4x4 matrix)
        => Update(mesh.Points, mesh.CornerIndices().Select(i => (uint)i.Value), colors, 2, material, matrix);

    public void Update(TriangleMesh3D mesh, IReadOnlyList<Vector3> colors, Material material, Matrix4x4 matrix)
        => Update(mesh.Points, mesh.CornerIndices().Select(i => (uint)i.Value), colors, 3, material, matrix);

    public void Update(QuadMesh3D mesh, IReadOnlyList<Vector3> colors, Material material, Matrix4x4 matrix)
        => Update(mesh.Points, mesh.CornerIndices().Select(i => (uint)i.Value), colors, 4, material, matrix);

    public void Update(IReadOnlyList<Point3D> points, IReadOnlyList<uint> indices, int primSize, Material material, Matrix4x4 matrix)
        => Update(points, indices, null, primSize, material, matrix);

    public void Update(IReadOnlyList<Point3D> points, IReadOnlyList<uint> indices, IReadOnlyList<Vector3> colors, int primSize, Material material, Matrix4x4 matrix)
    {
        var n = points.Count;
        if (colors != null && colors.Count != n)
            throw new Exception($"Expected {n} colors");

        SetFeatures(primSize, colors != null);

        VertexData.Clear();
        IndexData.Clear();
        MeshSliceData.Clear();
        InstanceData.Clear();

        var meshSlice = new MeshSliceStruct()
         {
            FirstIndex = 0,
            IndexCount = (uint)indices.Count,
            BaseVertex = 0,
            VertexCount = n
        };

        MeshSliceData.Add(meshSlice);

        var minX = float.MaxValue;
        var minY = float.MaxValue; 
        var minZ = float.MaxValue;

        var maxX = float.MinValue;
        var maxY = float.MinValue;
        var maxZ = float.MinValue;

        for (var i=0; i < n; i++)
        {
            var p = points[i];
            var x = p.X;
            var y = p.Y;
            var z = p.Z;
            
            VertexData.Add(p.X);
            VertexData.Add(p.Y);
            VertexData.Add(p.Z);

            minX = Math.Min(minX, x);
            minY = Math.Min(minY, y);
            minZ = Math.Min(minZ, z);

            maxX = Math.Max(maxX, x);
            maxY = Math.Max(maxY, y);
            maxZ = Math.Max(maxZ, z);

            if (colors == null)
                continue;
            
            var c = colors[i];
            VertexData.Add(c.X);
            VertexData.Add(c.Y);
            VertexData.Add(c.Z);
        }

        IndexData.AddRange(indices);

        var inst = new InstanceStruct(-1, matrix, 0, material, 0);
        InstanceData.Add(inst);

        Meta.TotalBounds = ((minX, minY, minZ), (maxX, maxY, maxZ));

        MeshBoundsData.Add(Meta.TotalBounds);
        InstanceBoundsData.Add(Meta.TotalBounds);
    }

    public void Update(IModel3D model)
    {
        SetFeatures(3, false);

        VertexData.Clear();
        IndexData.Clear();
        MeshSliceData.Clear();
        InstanceData.Clear();

        foreach (var mesh in model.Meshes)
        {
            var faceIndices = mesh.FaceIndices;
            var points = mesh.Points;

            var meshSlice = new MeshSliceStruct()
            {
                FirstIndex = (uint)IndexData.Count,
                IndexCount = (uint)faceIndices.Count * (uint)PrimitiveSize,
                BaseVertex = VertexData.Count / PrimitiveSize,
                VertexCount = points.Count
            };

            MeshSliceData.Add(meshSlice);

            // TODO: optimization opportunity 

            foreach (var point3D in points)
            {
                VertexData.Add(point3D.X);
                VertexData.Add(point3D.Y);
                VertexData.Add(point3D.Z);
            }

            foreach (var index3 in faceIndices)
            {
                IndexData.Add((uint)index3.A.Value);
                IndexData.Add((uint)index3.B.Value);
                IndexData.Add((uint)index3.C.Value);
            }
        }

        InstanceData.AddRange(model.Instances);
        ValidateMeshSlices();
        ComputeBounds();
    }

    public void Update(IEnumerable<RenderModelData> models)
    {
        SetFeatures(3, false);

        VertexData.Clear();
        IndexData.Clear();
        MeshSliceData.Clear();
        InstanceData.Clear();

        foreach (var model in models)
        {
            if (model.PrimitiveSize != Meta.PrimitiveSize)
                continue;

            VertexData.AddRange(model.VertexData);
            IndexData.AddRange(model.IndexData);
            var meshOffset = MeshSliceData.Count;
            MeshSliceData.AddRange(model.MeshSliceData);
            foreach (var i in model.InstanceData)
                InstanceData.Add(i.WithMeshIndex(i.MeshIndex + meshOffset));
        }

        ComputeBounds();
    }

    public void ValidateMeshSlices()
    {
#if DEBUG
        for (var i = 0; i < MeshSliceData.Count; i++)
        {
            var meshSlice = MeshSliceData[i];
            Debug.Assert(meshSlice.BaseVertex >= 0);
            Debug.Assert(meshSlice.BaseVertex <= VertexData.Count);
            Debug.Assert(meshSlice.VertexCount + meshSlice.BaseVertex <= VertexData.Count);
            Debug.Assert(meshSlice.FirstIndex <= IndexData.Count);
            Debug.Assert(meshSlice.FirstIndex + meshSlice.IndexCount <= IndexData.Count);
        }
#endif
    }

    public Bounds3D ComputeBounds(MeshSliceStruct slice)
    {
        var points = GetPoints(slice);
        var indices = GetIndices(slice);

        float minX, minY, minZ, maxX, maxY, maxZ;

        minX = minY = minZ = float.MaxValue;
        maxX = maxY = maxZ = float.MinValue;

        for (var i = 0; i < indices.Count; i++)
        {
            var p = points[indices[i]];
            minX = minX.Min(p.X);
            minY = minY.Min(p.Y);
            minZ = minZ.Min(p.Z);
            maxX = maxX.Max(p.X);
            maxY = maxY.Max(p.Y);
            maxZ = maxZ.Max(p.Z);
        }

        return ((minX, minY, minZ), (maxX, maxY, maxZ));
    }

    public void RecomputeMeshBounds()
    {
        MeshBoundsData.Clear();
        foreach (var meshSlice in MeshSliceData)
            MeshBoundsData.Add(ComputeBounds(meshSlice));
    }

    public void RecomputeInstanceBounds()
    {
        Meta.TotalFaceCount = 0;
        Meta.TotalVertexCount = 0;
        InstanceBoundsData.Clear();
        foreach (var inst in InstanceData)
        {
            if (inst.MeshIndex < 0 && inst.IsVisible)
                continue;

            var mesh = MeshSliceData[inst.MeshIndex];
            var meshBounds = MeshBoundsData[inst.MeshIndex];
            var transformed = meshBounds.FastTransform(inst.Matrix4x4);
            InstanceBoundsData.Add(transformed);
            Meta.TotalVertexCount += mesh.VertexCount;
            Meta.TotalFaceCount += mesh.IndexCount / PrimitiveSize;
        }
    }

    public void ComputeBounds()
    {
        RecomputeMeshBounds();
        RecomputeInstanceBounds();
        RecomputeTotalBounds();
    }

    public void RecomputeTotalBounds()
    {
        Meta.TotalBounds = InstanceBoundsData.GetTotalBoundsTrimOutliers();
    }

    public TriangleMesh3D GetMesh(MeshSliceStruct meshSlice)
    {
        if (PrimitiveSize != 3)
            throw new Exception($"Primitive size must be 3 to get a triangle mesh instead it is {PrimitiveSize}");

        var faceSlice = IndexData.Slice(meshSlice.FirstIndex, meshSlice.IndexCount).Reinterpret<Integer3>();
        return new(GetPoints(meshSlice), faceSlice);
    }

    public IBuffer<Point3D> GetPoints(MeshSliceStruct meshSlice)
        => BufferExtensions.Slice(VertexData?.Reinterpret<Point3D>(), meshSlice.BaseVertex, meshSlice.VertexCount);

    public IBuffer<int> GetIndices(MeshSliceStruct meshSlice)
        => BufferExtensions.Slice(IndexData?.Reinterpret<int>(), meshSlice.FirstIndex, meshSlice.IndexCount);
}