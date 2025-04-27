using System.Diagnostics;
using Ara3D.Serialization.BFAST;
using Ara3D.Utils;

namespace Ara3D.Data
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

            Debug.Assert(sizes[0] % VertexStruct.Size == 0);
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

            BFast.Write(filePath, BufferNames, sizes, OnBuffer);
        }

        public static unsafe IRenderScene Load(FilePath fp)
        {
            MemoryBlock<VertexStruct> vertices = null;
            MemoryBlock<uint> indices = null;
            MemoryBlock<MeshSliceStruct> meshes = null;
            MemoryBlock<InstanceStruct> instances = null;
            MemoryBlock<InstanceGroupStruct> groups = null;
            
            void OnView(string name, MemoryMappedView view, int index)
            {
                byte* srcPointer = null;
                view.Accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref srcPointer);
                IntPtr ptr = IntPtr.Zero;
                try
                {
                    switch (index)
                    {
                        case 0: 
                            ptr = vertices = new MemoryBlock<VertexStruct>(view.Size / VertexStruct.Size);
                            break;
                        case 1: 
                            ptr = indices = new MemoryBlock<uint>(view.Size / sizeof(uint)); 
                            break;
                        case 2:
                            ptr = meshes = new MemoryBlock<MeshSliceStruct>(view.Size / MeshSliceStruct.Size); 
                            break;
                        case 3:
                            ptr = instances = new MemoryBlock<InstanceStruct>(view.Size / InstanceStruct.Size); 
                            break;
                        case 4:
                            ptr = groups = new MemoryBlock<InstanceGroupStruct>(view.Size / InstanceGroupStruct.Size); 
                            break;
                        default:
                            throw new Exception("Unrecognized memory buffer");
                    }

                    srcPointer += view.Accessor.PointerOffset;
                    Buffer.MemoryCopy(srcPointer, ptr.ToPointer(), view.Size, view.Size);
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