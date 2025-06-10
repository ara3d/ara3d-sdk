using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Ara3D.Geometry;
using Ara3D.Models;

namespace Ara3D.IO.GltfExporter;

public class GltfBuilder
{
    public class GltfMeshSlice
    {
        public int VertexOffset;
        public int VertexCount;
        public int FaceOffset;
        public int FaceCount;

        public int VertexByteOffset => VertexOffset * 3 * sizeof(float);
        public int FaceByteOffset => FaceOffset * 3 * sizeof(int);
    }

    public List<Point3D> Vertices { get; } = new();
    public List<Integer3> Faces { get; } = new();
    public GltfData Data = new();

    const string SCALAR_STR = "SCALAR";
    const string FACE_STR = "FACE";
    const string VEC3_STR = "VEC3";
    const string POSITION_STR = "POSITION";

    const int VERTEX_VIEW_INDEX = 0;
    const int FACE_VIEW_INDEX = 1; 

    public List<GltfMeshSlice> MeshSlices = new();
    
    public GltfMaterial ToMaterial(Material mat)
        => new()
        {
            pbrMetallicRoughness = new GltfPbr()
            {
                baseColorFactor = [mat.Color.R, mat.Color.G, mat.Color.B],
                metallicFactor = mat.Metallic,
                roughnessFactor = mat.Roughness
            }
        };

    public GltfAccessor GetVertexAccessor(GltfMeshSlice slice)
    {
        var minX = float.MaxValue;
        var minY = float.MaxValue;
        var minZ = float.MaxValue;
        var maxX = float.MinValue;
        var maxY = float.MinValue;
        var maxZ = float.MinValue;
        for (var i = 0; i < slice.VertexCount; i++)
        {
            var v = Vertices[slice.VertexOffset + i];
            minX = Math.Min(minX, v.X);
            minY = Math.Min(minY, v.Y);
            minZ = Math.Min(minZ, v.Z);
            maxX = Math.Max(maxX, v.X);
            maxY = Math.Max(maxY, v.Y);
            maxZ = Math.Max(maxZ, v.Z);
        }

        return new GltfAccessor(VERTEX_VIEW_INDEX, slice.VertexByteOffset, 
            GltfComponentType.FLOAT, slice.VertexCount, VEC3_STR,
            [minX, minY, minZ], [maxX, maxY, maxZ], POSITION_STR);
    }
    
    public GltfAccessor GetIndexAccessor(GltfMeshSlice slice)
    {
        var min = int.MaxValue;
        var max = int.MinValue;        
        for (var i = 0; i < slice.FaceCount; i++)
        {
            var f = Faces[slice.FaceOffset + i];
            for (var j = 0; j < 3; j++)
            {
                min = Math.Min(min, f[j]);
                max = Math.Max(max, f[j]);
            }
        }

        return new GltfAccessor(FACE_VIEW_INDEX, slice.FaceByteOffset, 
            GltfComponentType.UNSIGNED_INT, slice.FaceCount * 3, SCALAR_STR, 
            [min], [max], FACE_STR);
    }

    public GltfMeshSlice CreateMeshSlice(TriangleMesh3D mesh)
    {
        var slice = new GltfMeshSlice
        {
            VertexOffset = Vertices.Count,
            VertexCount = mesh.Points.Count,
            FaceOffset = Faces.Count,
            FaceCount = mesh.FaceIndices.Count
        };
        Vertices.AddRange(mesh.Points);
        Faces.AddRange(mesh.FaceIndices);
        return slice;
    }

    public void AddModel(Model3D model)
    {
        var matOffset = Data.materials.Count;
        var slicesOffset = MeshSlices.Count;
        var nodesOffset = Data.nodes.Count;
        var accOffset = Data.accessors.Count;

        Data.materials.AddRange(model.Materials.Select(ToMaterial));

        var slices = model.Meshes.Select(CreateMeshSlice).ToList();
        MeshSlices.AddRange(slices);

        foreach (var slice in slices)
        {
            var vertexAccessor = GetVertexAccessor(slice);
            var indexAccessor = GetIndexAccessor(slice);

            Data.accessors.Add(vertexAccessor);
            Data.accessors.Add(indexAccessor);
        }

        Debug.Assert(Data.accessors.Count == accOffset + slices.Count * 2);

        foreach (var es in model.ElementStructs)
        {
            var matIndex = es.MaterialIndex + matOffset;
            var transform = model.Transforms[es.TransformIndex];
            
            var meshIndex = Data.meshes.Count;
            
            var vertexAccessorIndex = accOffset + meshIndex * 2;
            var indexAccessorIndex = accOffset + meshIndex * 2 + 1;

            var vertexAccessor = Data.accessors[vertexAccessorIndex];
            var indexAccessor = Data.accessors[indexAccessorIndex];

            Debug.Assert(vertexAccessor.GltfComponentType == GltfComponentType.FLOAT);
            Debug.Assert(indexAccessor.GltfComponentType == GltfComponentType.UNSIGNED_INT);

            Debug.Assert(vertexAccessor.count == slices[es.MeshIndex].VertexCount);
            Debug.Assert(indexAccessor.count == slices[es.MeshIndex].FaceCount * 3);

            var prim = new GltfMeshPrimitive(vertexAccessorIndex, indexAccessorIndex, matIndex);
            var mesh = new GltfMesh { primitives = [prim] };

            Data.meshes.Add(mesh);
            var node = new GltfNode(transform, meshIndex);
            Data.nodes.Add(node);
        }
    }

    public GltfData Build()
    {
        var lastSlice = MeshSlices[^1];

        var vertexByteSize = lastSlice.VertexCount * 3 * sizeof(float);
        var indexByteSize = lastSlice.FaceCount * 3 * sizeof(int);

        var totalBufferSize = vertexByteSize + indexByteSize;

        var vertexBufferView = new GltfBufferView(0, 0, vertexByteSize, GltfTargets.ARRAY_BUFFER, "POSITION");
        var indexBufferView = new GltfBufferView(0, vertexByteSize, indexByteSize, GltfTargets.ELEMENT_ARRAY_BUFFER, "INDICES");

        Data.bufferViews.Add(vertexBufferView);
        Data.bufferViews.Add(indexBufferView);
        
        var buffer = new GltfBuffer 
        {
            byteLength = totalBufferSize,
        };
        Data.buffers.Add(buffer);

        var vertexBytes = MemoryMarshal.AsBytes(Vertices.ToArray().AsSpan());
        var indexBytes = MemoryMarshal.AsBytes(Faces.ToArray().AsSpan());
        var byteCount = vertexBytes.Length + indexBytes.Length;
        
        Data.bytes.AddRange(BitConverter.GetBytes((uint)byteCount));
        Data.bytes.AddRange(GltfExporter.BinChunkType);
        Data.bytes.AddRange(vertexBytes);
        Data.bytes.AddRange(indexBytes);

        Debug.Assert(Data.accessors.Count == MeshSlices.Count * 2);

        var scene = new GltfScene();
        for (var i = 0; i < Data.nodes.Count; i++)
            scene.nodes.Add(i);
        Data.scenes = [scene];
        return Data;
    }
}

