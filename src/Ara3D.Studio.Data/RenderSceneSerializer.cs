using System.Diagnostics;
using Ara3D.Geometry;
using Ara3D.IO.BFAST;
using Ara3D.Memory;
using Ara3D.MemoryMappedFiles;
using Ara3D.Utils;

namespace Ara3D.Studio.Data
{
    public static class RenderSceneSerializer
    {
        public static string[] BufferNames = new[]
        {
            nameof(RenderScene.VertexBlock),
            nameof(RenderScene.IndexBlock),
            nameof(RenderScene.MeshBlock),
            nameof(RenderScene.InstanceBlock),
            nameof(RenderScene.GroupBlock),
        };

        public static unsafe void Save(IRenderScene renderScene, FilePath filePath)
        {
            var sizes = new[]
            {
                renderScene.Vertices.GetNumBytes(),
                renderScene.Indices.GetNumBytes(),
                renderScene.Meshes.GetNumBytes(),
                renderScene.Instances.GetNumBytes(),
                renderScene.Groups.GetNumBytes(),
            };

            Debug.Assert(sizes[0] % sizeof(Point3D) == 0);
            Debug.Assert(sizes[1] % 4 == 0);
            Debug.Assert(sizes[2] % MeshSliceStruct.Size == 0);
            Debug.Assert(sizes[3] % InstanceStruct.Size == 0);
            Debug.Assert(sizes[4] % InstanceGroupStruct.Size == 0);

            var ptrs = new[]             
            {
                renderScene.Vertices.GetIntPtr(),
                renderScene.Indices.GetIntPtr(),
                renderScene.Meshes.GetIntPtr(),
                renderScene.Instances.GetIntPtr(),
                renderScene.Groups.GetIntPtr(),
            };

            long OnBuffer(Stream stream, int index, string name, long bytesToWrite)
            {
                var ptr = ptrs[index];
                var size = sizes[index];
                Debug.Assert(bytesToWrite == size);
                while (true)
                {
                    var tmp = Math.Min(size, int.MaxValue);
                    var span = new ReadOnlySpan<byte>(ptr.ToPointer(), (int)tmp);
                    stream.Write(span);
                    size -= tmp;
                    if (size <= 0)
                        break;
                }
                stream.Flush();
                return bytesToWrite;
            }

            BFast.Write((string)filePath, (IEnumerable<string>)BufferNames, (IEnumerable<long>)sizes, OnBuffer);
        }

        public static unsafe IRenderScene Load(FilePath fp)
        {
            AlignedMemory<Point3D> vertices = null;
            AlignedMemory<uint> indices = null;
            AlignedMemory<MeshSliceStruct> meshes = null;
            AlignedMemory<InstanceStruct> instances = null;
            AlignedMemory<InstanceGroupStruct> groups = null;
            
            void OnView(string name, MemoryMappedView view, int index)
            {
                byte* srcPointer = null;
                view.Accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref srcPointer);
                try
                {
                    IBuffer buffer = index switch
                    {
                        0 => vertices = new AlignedMemory<Point3D>(view.Size / sizeof(Point3D)),
                        1 => indices = new AlignedMemory<uint>(view.Size / sizeof(uint)),
                        2 => meshes = new AlignedMemory<MeshSliceStruct>(view.Size / MeshSliceStruct.Size),
                        3 => instances = new AlignedMemory<InstanceStruct>(view.Size / InstanceStruct.Size),
                        4 => groups = new AlignedMemory<InstanceGroupStruct>(view.Size / InstanceGroupStruct.Size),
                        _ => throw new Exception("Unrecognized memory buffer")
                    };

                    srcPointer += view.Accessor.PointerOffset;
                    Buffer.MemoryCopy(srcPointer, buffer.GetPointer(), view.Size, view.Size);
                }
                finally
                {
                    view.Accessor.SafeMemoryMappedViewHandle.ReleasePointer();
                }
            }

            BFastReader.Read(fp, OnView);
            
            return new RenderScene(vertices, indices, meshes, instances, groups);
        }
    }
}