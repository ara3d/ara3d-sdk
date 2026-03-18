using Ara3D.Collections;
using Ara3D.Geometry;
using Ara3D.Memory;
using System.Diagnostics;
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
        public int Unused;
    }

    public MetaData Meta;
    public UnmanagedList<float> VertexBuffer { get; private set; }
    public UnmanagedList<uint> IndexBuffer { get; private set; }
    public UnmanagedList<MeshSliceStruct> MeshBuffer { get; private set; }
    public UnmanagedList<InstanceStruct> InstanceBuffer { get; private set; }
    public UnmanagedList<Bounds3D> MeshBounds { get; }
    public UnmanagedList<Bounds3D> InstanceBounds { get; }


    public int PrimitiveSize => Meta.PrimitiveSize;
    public Bounds3D TotalBounds => Meta.TotalBounds;
    public long TotalVertexCount => Meta.TotalVertexCount;
    public long TotalFaceCount => Meta.TotalFaceCount;
    public int VertexCount => VertexBuffer.Count;
    public int IndexCount => IndexBuffer.Count;
    public int FaceCount => IndexCount / PrimitiveSize;
    public int MeshCount => MeshBuffer.Count;
    public int InstanceCount => InstanceBuffer.Count;

    public RenderModelData(int primSize)
    {
        if (primSize < 1 || primSize > 4)
            throw new Exception($"Render model data primitive size ({primSize}) must be from 1 to 4 inclusive");

        Meta.PrimitiveSize = primSize;
        VertexBuffer = new();
        IndexBuffer = new();
        MeshBuffer = new();
        InstanceBuffer = new();
        MeshBounds = new();
        InstanceBounds = new();
    }
    
    public bool IsModel3D
        => PrimitiveSize == 3;

    public Model3D ToModel3D()
    {
        if (PrimitiveSize != 3) throw new Exception("Not a triangle mesh");
        var meshes = MeshBuffer.Select(GetMesh);
        return new Model3D(meshes, InstanceBuffer);
    }

    public void Dispose()
    {
        VertexBuffer?.Dispose();
        IndexBuffer?.Dispose();
        MeshBuffer?.Dispose();
        InstanceBuffer?.Dispose();
        VertexBuffer = null;
        IndexBuffer = null;
        MeshBuffer = null;
        InstanceBuffer = null;
    }

    public void Clear()
    {
        VertexBuffer?.Clear();
        IndexBuffer?.Clear();
        MeshBuffer?.Clear();
        InstanceBuffer?.Clear();
        Meta.TotalBounds = Bounds3D.Empty;
    }

    public void Update(
        IBuffer<float> vertexBuffer, 
        IBuffer<uint> indexBuffer, 
        IBuffer<MeshSliceStruct> meshSlices,
        IBuffer<InstanceStruct> instances)
    {
        VertexBuffer.CopyFrom(vertexBuffer);
        IndexBuffer.CopyFrom(indexBuffer);
        MeshBuffer.CopyFrom(meshSlices);
        InstanceBuffer.CopyFrom(instances);
        RecomputeTotalBounds();
    }

    public void UpdateVertexBuffer(IBuffer<float> vertexBuffer) => VertexBuffer.CopyFrom(vertexBuffer);
    public void UpdateIndexBuffer(IBuffer<uint> indexBuffer) => IndexBuffer.CopyFrom(indexBuffer);
    public void UpdateMeshBuffer(IBuffer<MeshSliceStruct> meshBuffer) => MeshBuffer.CopyFrom(meshBuffer);
    public void UpdateInstanceBuffer(IBuffer<InstanceStruct> instanceBuffer) => InstanceBuffer.CopyFrom(instanceBuffer);

    public void UpdateVertexBuffer(IReadOnlyList<float> vertexBuffer) => VertexBuffer.CopyFrom(vertexBuffer);
    public void UpdateIndexBuffer(IReadOnlyList<uint> indexBuffer) => IndexBuffer.CopyFrom(indexBuffer);
    public void UpdateMeshBuffer(IReadOnlyList<MeshSliceStruct> meshBuffer) => MeshBuffer.CopyFrom(meshBuffer);
    public void UpdateInstanceBuffer(IReadOnlyList<InstanceStruct> instanceBuffer) => InstanceBuffer.CopyFrom(instanceBuffer);

    public void Update(RenderModelData data)
    {
        Meta.PrimitiveSize = data.PrimitiveSize; 
        UpdateVertexBuffer(data.VertexBuffer);
        UpdateIndexBuffer(data.IndexBuffer);
        UpdateMeshBuffer(data.MeshBuffer);
        UpdateInstanceBuffer(data.InstanceBuffer);
        ValidateMeshSlices();
        MeshBounds.Clear();
        MeshBounds.AddRange(data.MeshBounds);
        InstanceBounds.Clear();
        InstanceBounds.AddRange(data.InstanceBounds);
        Meta.TotalBounds = data.TotalBounds;
        Meta.TotalVertexCount = data.TotalVertexCount;
        Meta.TotalFaceCount = data.TotalFaceCount;
    }

    public void Update(IModel3D model)
    {
        Meta.PrimitiveSize = 3;

        if (!IsModel3D)
            throw new Exception("Not a model 3D");

        VertexBuffer.Clear();
        IndexBuffer.Clear();
        MeshBuffer.Clear();
        InstanceBuffer.Clear();

        foreach (var mesh in model.Meshes)
        {
            var faceIndices = mesh.FaceIndices;
            var points = mesh.Points;

            var meshSlice = new MeshSliceStruct()
            {
                FirstIndex = (uint)IndexBuffer.Count,
                IndexCount = (uint)faceIndices.Count * (uint)PrimitiveSize,
                BaseVertex = VertexBuffer.Count / PrimitiveSize,
                VertexCount = points.Count
            };

            MeshBuffer.Add(meshSlice);

            // TODO: optimization opportunity 

            foreach (var point3D in points)
            {
                VertexBuffer.Add(point3D.X);
                VertexBuffer.Add(point3D.Y);
                VertexBuffer.Add(point3D.Z);
            }

            foreach (var index3 in faceIndices)
            {
                IndexBuffer.Add((uint)index3.A.Value);
                IndexBuffer.Add((uint)index3.B.Value);
                IndexBuffer.Add((uint)index3.C.Value);
            }
        }

        InstanceBuffer.AddRange(model.Instances);
        ValidateMeshSlices();
        ComputeBounds();
    }

    public void Update(IEnumerable<RenderModelData> models)
    {
        Meta.PrimitiveSize = 3;

        if (!IsModel3D)
            throw new Exception("Not a model 3D");

        VertexBuffer.Clear();
        IndexBuffer.Clear();
        MeshBuffer.Clear();
        InstanceBuffer.Clear();

        foreach (var model in models)
        {
            if (model.PrimitiveSize != model.PrimitiveSize)
                continue;

            VertexBuffer.AddRange(model.VertexBuffer);
            IndexBuffer.AddRange(model.IndexBuffer);
            var meshOffset = MeshBuffer.Count;
            MeshBuffer.AddRange(model.MeshBuffer);
            foreach (var i in model.InstanceBuffer)
                InstanceBuffer.Add(i.WithMeshIndex(i.MeshIndex + meshOffset));
        }

        ComputeBounds();
    }

    public void Update(LineMesh3D lines, Matrix4x4 transform, Material material)
    {
        Meta.PrimitiveSize = 2;

        VertexBuffer.Clear();
        IndexBuffer.Clear();
        MeshBuffer.Clear();
        InstanceBuffer.Clear();

        var points = lines.Points;
        var faceIndices = lines.FaceIndices;
        
        var meshSlice = new MeshSliceStruct()
        {
            FirstIndex = (uint)0,
            IndexCount = (uint)faceIndices.Count * (uint)PrimitiveSize,
            BaseVertex = 0,
            VertexCount = points.Count
        };

        MeshBuffer.Add(meshSlice);

        foreach (var point3D in points)
        {
            VertexBuffer.Add(point3D.X);
            VertexBuffer.Add(point3D.Y);
            VertexBuffer.Add(point3D.Z);
        }

        foreach (var index2 in faceIndices)
        {
            IndexBuffer.Add((uint)index2.A.Value);
            IndexBuffer.Add((uint)index2.B.Value);
        }

        var inst = new InstanceStruct(-1, transform, 0, material, 0);

        InstanceBuffer.Add(inst);
        ValidateMeshSlices();
        ComputeBounds();
    }

    public void ValidateMeshSlices()
    {
#if DEBUG
        for (var i = 0; i < MeshBuffer.Count; i++)
        {
            var meshSlice = MeshBuffer[i];
            Debug.Assert(meshSlice.BaseVertex >= 0);
            Debug.Assert(meshSlice.BaseVertex <= VertexBuffer.Count);
            Debug.Assert(meshSlice.VertexCount + meshSlice.BaseVertex <= VertexBuffer.Count);
            Debug.Assert(meshSlice.FirstIndex <= IndexBuffer.Count);
            Debug.Assert(meshSlice.FirstIndex + meshSlice.IndexCount <= IndexBuffer.Count);
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
        MeshBounds.Clear();
        foreach (var meshSlice in MeshBuffer)
            MeshBounds.Add(ComputeBounds(meshSlice));
    }

    public void RecomputeInstanceBounds()
    {
        Meta.TotalFaceCount = 0;
        Meta.TotalVertexCount = 0;
        InstanceBounds.Clear();
        foreach (var inst in InstanceBuffer)
        {
            if (inst.MeshIndex < 0 && inst.IsVisible)
                continue;

            var mesh = MeshBuffer[inst.MeshIndex];
            var meshBounds = MeshBounds[inst.MeshIndex];
            var transformed = meshBounds.FastTransform(inst.Matrix4x4);
            InstanceBounds.Add(transformed);
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
        Meta.TotalBounds = InstanceBounds.GetTotalBoundsTrimOutliers();
    }

    public TriangleMesh3D GetMesh(MeshSliceStruct meshSlice)
    {
        if (PrimitiveSize != 3)
            throw new Exception($"Primitive size must be 3 to get a triangle mesh instead it is {PrimitiveSize}");

        var faceSlice = IndexBuffer.Slice(meshSlice.FirstIndex, meshSlice.IndexCount).Reinterpret<Integer3>();
        return new(GetPoints(meshSlice), faceSlice);
    }

    public IBuffer<Point3D> GetPoints(MeshSliceStruct meshSlice)
        => BufferExtensions.Slice(VertexBuffer?.Reinterpret<Point3D>(), meshSlice.BaseVertex, meshSlice.VertexCount);

    public IBuffer<int> GetIndices(MeshSliceStruct meshSlice)
        => BufferExtensions.Slice(IndexBuffer?.Reinterpret<int>(), meshSlice.FirstIndex, meshSlice.IndexCount);
    }