using System.Diagnostics;
using System.Runtime.InteropServices;
using Ara3D.Geometry;
using Ara3D.IO.BFAST;
using Ara3D.Memory;
using Ara3D.Utils;

namespace Ara3D.Models;

public static class RenderModelBfastSerializer
{
    public static string[] BufferNames = new[]
    {
        nameof(RenderModelData.VertexData),
        nameof(RenderModelData.IndexData),
        nameof(RenderModelData.MeshSliceData),
        nameof(RenderModelData.InstanceData),
        nameof(RenderModelData.MeshBoundsData),
        nameof(RenderModelData.InstanceBoundsData),
        nameof(RenderModelData.Meta)
    };

    public static unsafe void Write(this RenderModelData renderModelData, FilePath filePath)
    {
        var sizes = new[]
        {
            renderModelData.VertexData.Bytes.Count,
            renderModelData.IndexData.Bytes.Count,
            renderModelData.MeshSliceData.Bytes.Count,
            renderModelData.InstanceData.Bytes.Count,
            renderModelData.MeshBoundsData.Bytes.Count,
            renderModelData.InstanceBoundsData.Bytes.Count,
            sizeof(RenderModelData.MetaData),
        };

        Debug.Assert(sizes[0] % sizeof(Point3D) == 0);
        Debug.Assert(sizes[1] % 4 == 0);
        Debug.Assert(sizes[2] % sizeof(MeshSliceStruct) == 0);
        Debug.Assert(sizes[3] % InstanceStruct.Size == 0);

        fixed (RenderModelData.MetaData* metaDataPtr = &renderModelData.Meta)
        {
            var ptrs = new[]
            {
                renderModelData.VertexData.Bytes.Ptr,
                renderModelData.IndexData.Bytes.Ptr,
                renderModelData.MeshSliceData.Bytes.Ptr,
                renderModelData.InstanceData.Bytes.Ptr,
                renderModelData.MeshBoundsData.Bytes.Ptr,
                renderModelData.InstanceBoundsData.Bytes.Ptr,
                (byte*)metaDataPtr, 
            };

            long OnBuffer(Stream stream, int index, string name, long bytesToWrite)
            {
                var ptr = ptrs[index];
                var size = sizes[index];
                Debug.Assert(bytesToWrite == size);
                while (true)
                {
                    var tmp = Math.Min(size, int.MaxValue);
                    var span = new ReadOnlySpan<byte>(ptr, (int)tmp);
                    stream.Write(span);
                    size -= tmp;
                    if (size <= 0)
                        break;
                }

                stream.Flush();
                return bytesToWrite;
            }

            BFast.Write((string)filePath, BufferNames, sizes.Select(sz => (long)sz), OnBuffer);
        }
    }

    public static unsafe void AddRange<T>(this UnmanagedList<T> self, byte* ptr, long count)
        where T: unmanaged
    {
        var byteSlice = new ByteSlice(ptr, count);
        self.AddRange(byteSlice.AsReadOnlySpan<T>());
    }

    public static unsafe RenderModelData Load(FilePath fp)
    {
        var r = new RenderModelData(3);

        void OnView(string name, MemoryMappedView view, int index)
        {
            byte* srcPointer = null;
            view.Accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref srcPointer);
            try
            {
                srcPointer += view.Accessor.PointerOffset;

                switch (index)
                {
                    case 0: 
                        r.VertexData.AddRange(srcPointer, view.Size); 
                        break;
                    case 1: 
                        r.IndexData.AddRange(srcPointer, view.Size); 
                        break;
                    case 2: 
                        r.MeshSliceData.AddRange(srcPointer, view.Size); 
                        break;
                    case 3: 
                        r.InstanceData.AddRange(srcPointer, view.Size); 
                        break;
                    case 4:
                        r.MeshBoundsData.AddRange(srcPointer, view.Size);
                        break;
                    case 5:
                        r.InstanceBoundsData.AddRange(srcPointer, view.Size);
                        break;
                    case 6:
                        Debug.Assert(view.Size == sizeof(RenderModelData.MetaData));
                        r.Meta = Marshal.PtrToStructure<RenderModelData.MetaData>((IntPtr)srcPointer);
                        break;
                    default: 
                        throw new Exception($"Unrecognized memory buffer: {name} at position {index}");
                }
            }
            finally
            {
                view.Accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }

        BFastReader.Read(fp, OnView);
        return r;
    }
}