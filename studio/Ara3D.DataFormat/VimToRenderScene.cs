using System.Diagnostics;
using Ara3D.Logging;
using Ara3D.Serialization.G3D;
using System.Numerics;
using Ara3D.Serialization.VIM;
using Matrix4x4 = System.Numerics.Matrix4x4;
using Quaternion = System.Numerics.Quaternion;

namespace Ara3D.Data
{
    public static class VimToRenderScene
    {
        public static RenderSceneBuilder BuildRenderScene(this G3D g3d, ILogger logger = null)
            => BuildRenderScene(VimGeometryBuffers.Create(g3d));

        public static RenderSceneBuilder BuildRenderScene(this VimGeometryBuffers g, ILogger logger = null)
        {
            var gb = new RenderSceneBuilder();

            logger?.Log("Computing normals");
            var normals = RenderSceneBuilder.ToNormals(g.Vertices.Span(), g.Indices.Span());

            logger?.Log("Adding vertex structures");

            var n = (uint)g.Vertices.Count;
            gb.VertexList = new UnmanagedList<VertexStruct>(n, n);
            for (var i = 0; i < n; ++i)
            {
                gb.VertexList[i].Position = g.Vertices[i];
                gb.VertexList[i].N = NormalEncoder.EncodeNormal(normals[i]);
            }

            logger?.Log("Adding indices");
            n = (uint)g.Indices.Count;
            gb.IndexList = new UnmanagedList<uint>(n, n);
            for (var i = 0; i < n; ++i)
                gb.IndexList[i] = (uint)g.Indices[i];

            logger?.Log("Adding meshes, and computing bounds");
            gb.MeshList = new UnmanagedList<MeshSliceStruct>((uint)g.NumSubMeshes);

            for (var i = 0; i < g.NumSubMeshes; ++i)
            {
                var start = g.SubMeshIndexOffsets[i];
                var end = i < g.NumSubMeshes - 1 ? g.SubMeshIndexOffsets[i + 1] : g.Indices.Count;
                var count = end - start;

                var min = new Vector3(float.MaxValue);
                var max = new Vector3(float.MinValue);
                for (var j = 0; j < count; j++)
                {
                    var index = g.Indices[j + start];
                    var v = g.Vertices[index];
                    min = Vector3.Min(min, v);
                    max = Vector3.Max(max, v);
                }

                var slice = new MeshSliceStruct
                {
                    FirstIndex = (uint)start,
                    IndexCount = (uint)count,
                    BaseVertex = 0,
                    Bounds = new Bounds(min, max)
                };

                gb.MeshList.Add(slice);
            }

            // Convert the received instance transforms into instance structs
            logger?.Log("Computing instance structs");
            var initialInstanceStructs = new List<InstanceStruct>();            
            for (var i=0; i < g.InstanceTransforms.Count; i++)
            {
                var t = g.InstanceTransforms[i];
                var m = new Matrix4x4(
                    t.M11, t.M12, t.M13, t.M14,
                    t.M21, t.M22, t.M23, t.M24,
                    t.M31, t.M32, t.M33, t.M34,
                    t.M41, t.M42, t.M43, t.M44);
                Matrix4x4.Decompose(m, out var scale, out var rotation, out var translation);
                initialInstanceStructs.Add(new()
                {
                    PosX = translation.X,
                    PosY = translation.Y,
                    PosZ = translation.Z,
                    Orientation = new Quaternion(rotation.X, rotation.Y, rotation.Z, rotation.W),
                    Scale = scale,
                });
            }

            logger?.Log("Creating submesh lookup");

            // Create a look-up from submesh to the instance which references it. 
            var submeshToInstance = new List<int>[g.NumSubMeshes];

            // Go the through the list of VIM instances and create the instance groups
            for (var i = 0; i < g.NumInstances; ++i)
            {
                var instancedMesh = g.InstanceMeshes[i];
                if (instancedMesh < 0)
                    continue;

                // The VIM instances a mesh, which constitutes a list of sub-meshes.
                // An alternative approach would be to merge the lists of sub-meshes into a single mesh 
                var start = g.MeshSubMeshOffset[instancedMesh];

                var end = instancedMesh < g.NumMeshes - 1
                    ? g.MeshSubMeshOffset[instancedMesh + 1]
                    : start + 1;

                for (var j = start; j < end; ++j)
                {
                    if (submeshToInstance[j] == null)
                        submeshToInstance[j] = new List<int>();
                    submeshToInstance[j].Add(i);
                }
            }

            logger?.Log("Creating instance groups");

            // Go through the list of sub-meshes and create the instance groups
            for (var subMesh = 0; subMesh < g.NumSubMeshes; subMesh++)
            {
                var instanceGroup = submeshToInstance[subMesh];

                if (instanceGroup == null)
                    continue;

                if (instanceGroup.Count == 0)
                    continue;

                var materialIndex = g.SubMeshMaterials[subMesh];
                var materialColor =
                    materialIndex >= 0
                        ? g.MaterialColors[materialIndex]
                        : new Vector4(0.5f, 0.5f, 0.5f, 1.0f);

                var baseInstance = gb.InstanceList.Count;
                var numInstances = instanceGroup.Count;
                for (var i = 0; i < numInstances; ++i)
                {
                    var instance = initialInstanceStructs[instanceGroup[i]];
                    instance.Color = new Vector4(
                        materialColor.X,
                        materialColor.Y,
                        materialColor.Z,
                        materialColor.W);
                    if (materialIndex >= 0)
                        instance.Roughness = 1f - g.MaterialSmoothness[materialIndex];
                    else
                        instance.Roughness = 0.5f;
                    instance.Metallic = 0;
                    gb.InstanceList.Add(instance);
                }

                Debug.Assert(gb.InstanceList.Count == baseInstance + numInstances);
                Debug.Assert(numInstances > 0);

                var group = new InstanceGroupStruct
                {
                    BaseInstance = (uint)baseInstance,
                    InstanceCount = (uint)numInstances,
                    MeshIndex = (uint)subMesh
                };

                gb.InstanceGroupList.Add(group);
            }

            // NOTE: we already computed the per-mesh bounds 
            gb.ComputeAllInstanceBounds();

            // Validate the values 
            gb.Validate();

            return gb;
        }
    }
}