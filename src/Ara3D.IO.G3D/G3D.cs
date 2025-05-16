using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Ara3D.BFAST;
using Ara3D.Collections;
using Ara3D.Memory;
using Ara3D.MemoryMappedFiles;

namespace Ara3D.Serialization.G3D
{
    /// <summary>
    /// This is the geometry layout within a VIM file
    /// </summary>
    public class G3D : GeometryAttributes
    {
        public new static readonly G3D Empty = Create();

        public G3dHeader Header { get; }

        /// <summary>
        /// Computed array of mesh data structures for convenience. 
        /// </summary>
        public List<G3dMesh> Meshes { get; }

        /// <summary>
        /// Material structures. 
        /// </summary>
        public List<G3dMaterial> Materials { get; }

        /// <summary>
        /// Vertex buffer. Required to be present. 
        /// </summary>
        public IBuffer<Vector3> Vertices { get; }

        /// <summary>
        /// Index buffer (one index per corner, and per half-edge). Computed if absent. 
        /// </summary>
        public IBuffer<int> Indices { get; }

        /// <summary>
        /// For each mesh, this is the index of the first sub-mesh that it is associated with.
        /// </summary>
        public IBuffer<int> MeshSubmeshOffset { get; }

        /// <summary>
        /// Index of the parent transforms. If present. Usually not used. 
        /// </summary>
        public IBuffer<int> InstanceParents { get; }

        /// <summary>
        /// // A 4x4 matrix in row-column order defining the transformed mesh.
        /// </summary>
        public IBuffer<Matrix4x4> InstanceTransforms { get; }

        /// <summary>
        /// The index of the mesh associated with the instance transform.
        /// </summary>
        public IBuffer<int> InstanceMeshes { get; }

        /// <summary>
        /// Custom data (for example visibility flags) associated with each instance.
        /// </summary>
        public IBuffer<ushort> InstanceFlags { get; }

        /// <summary>
        /// Material colors as RGBA
        /// </summary>
        public IBuffer<Vector4> MaterialColors { get; } 

        /// <summary>
        /// Material glossiness 
        /// </summary>
        public IBuffer<float> MaterialGlossiness { get; }
        
        /// <summary>
        /// Material smoothness 
        /// </summary>
        public IBuffer<float> MaterialSmoothness { get; }
        
        /// <summary>
        /// The offset into the index buffer for each submesh.
        /// Used to compute the offset into the index buffer for a corresponding mesh as well. 
        /// </summary>
        public IBuffer<int> SubmeshIndexOffsets { get; }

        /// <summary>
        /// The number of indices associated with a submesh.
        /// Usually computed. 
        /// </summary>
        public IBuffer<int> SubmeshIndexCount { get; }
        
        /// <summary>
        /// The index of the material associated with a submesh. 
        /// </summary>
        public IBuffer<int> SubmeshMaterials { get; }
        
        /// <summary>
        /// The number of sub-meshes for each mesh. Computed. 
        /// </summary>
        public IBuffer<int> MeshSubmeshCount { get; }
        
        /// <summary>
        /// Constructor 
        /// </summary>
        public G3D(IEnumerable<GeometryAttribute> attributes, G3dHeader? header = null, int numCornersPerFaceOverride = -1)
            : base(attributes, numCornersPerFaceOverride)
        {
            Header = header ?? new G3dHeader();

            foreach (var attr in Attributes.ToEnumerable())
            {
                var desc = attr.Descriptor;
                switch (desc.Semantic)
                {
                    case Semantic.Index:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_corner))
                            Indices = Indices ?? attr.AsType<int>().Data;
                        break;

                    case Semantic.Position:
                        if (attr.IsTypeAndAssociation<Vector3>(Association.assoc_vertex))
                            Vertices = Vertices ?? attr.AsType<Vector3>().Data;
                        break;

                    case Semantic.Color:
                        if (desc.Association == Association.assoc_material)
                            MaterialColors = MaterialColors ?? attr.AsType<Vector4>().Data;
                        break;

                    case Semantic.IndexOffset:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_mesh))
                            throw new NotImplementedException("TEMP");
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_submesh))
                            SubmeshIndexOffsets = SubmeshIndexOffsets ?? attr.AsType<int>().Data;
                        break;

                    case Semantic.Material:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_submesh))
                            SubmeshMaterials = SubmeshMaterials ?? attr.AsType<int>().Data;
                        break;

                    case Semantic.Parent:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_instance))
                            InstanceParents = InstanceParents ?? attr.AsType<int>().Data;
                        break;

                    case Semantic.Mesh:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_instance))
                            InstanceMeshes = InstanceMeshes ?? attr.AsType<int>().Data;
                        break;

                    case Semantic.Transform:
                        if (attr.IsTypeAndAssociation<Matrix4x4>(Association.assoc_instance))
                            InstanceTransforms = InstanceTransforms ?? attr.AsType<Matrix4x4>().Data;
                        break;

                    case Semantic.Glossiness:
                        if (attr.IsTypeAndAssociation<float>(Association.assoc_material))
                            MaterialGlossiness = attr.AsType<float>().Data;
                        break;

                    case Semantic.Smoothness:
                        if (attr.IsTypeAndAssociation<float>(Association.assoc_material))
                            MaterialSmoothness = attr.AsType<float>().Data;
                        break;
                            
                    case Semantic.SubMeshOffset:
                        if (attr.IsTypeAndAssociation<int>(Association.assoc_mesh))
                            MeshSubmeshOffset = attr.AsType<int>().Data;
                        break;

                    case Semantic.Flags:
                        if (attr.IsTypeAndAssociation<ushort>(Association.assoc_instance))
                            InstanceFlags = attr.AsType<ushort>().Data;
                        break;
                }
            }

            if (NumMeshes > 0)
            {
                // Mesh offset is the same as the offset of its first sub-mesh.
                if (MeshSubmeshOffset != null)
                {
                    MeshSubmeshCount = GetSubArrayCounts(MeshSubmeshOffset.Count, MeshSubmeshOffset, NumSubmeshes)
                        .Fix();
                }
            }
            else
            {
                MeshSubmeshCount = FixedArray<int>.Empty;
            }

            // Compute for each submesh, how many indices does it use. 
            if (SubmeshIndexOffsets != null)
                SubmeshIndexCount = GetSubArrayCounts(SubmeshIndexOffsets.Count, SubmeshIndexOffsets, NumCorners)
                    .Fix();

            // Compute structures for the meshes and sub-meshes 
            var meshes = new List<G3dMesh>();
            for (var i = 0; i < NumMeshes; ++i)
            {
                var curSubMesh = MeshSubmeshOffset[i];
                var numSubMeshes = MeshSubmeshCount[i];

                var subMeshes = new G3dSubMesh[numSubMeshes];
                for (var j = 0; j < numSubMeshes; ++j)
                {
                    subMeshes[j] = new G3dSubMesh(this, curSubMesh + j, i);
                }

                meshes.Add(new G3dMesh(i, subMeshes));
            }
            Meshes = meshes;

            // Compute all of the materials 
            if (MaterialColors != null)
                Materials = MaterialColors.Count.Select(i => new G3dMaterial(this, i))
                    .ToList();

            // Update the instance options
            if (InstanceFlags == null)
                InstanceFlags = ((ushort) 0).Repeat(NumInstances).ToArray().Fix();
        }

        private static int[] GetSubArrayCounts(int numItems, IReadOnlyList<int> offsets, int totalCount)
            => numItems.Select(i => i < (numItems - 1)
                ? offsets[i + 1] - offsets[i]
                : totalCount - offsets[i]).ToArray();

        public static G3D Read(string filePath)
        {
            G3D r = null;
            MemoryMappedView.ReadFile(filePath, view => r = Read(view));
            return r;
        }

        public static G3D ReadFromGeometryBufferIfPossible(BFastReader reader)
        {
            // Check if there is a geometry buffer (e.g., as in a VIM file)
            try
            {
                var tmp = reader.BufferNames.ToList().IndexOf("geometry");
                if (tmp < 0) return null;
                var (name, range) = reader.GetNameAndRange(tmp);
                using (var subView = reader.View.CreateSubView(range.Begin, range.Count))
                {
                    return Read(subView);
                }
            }
            catch 
            {
                return null;
            }
        }

        public static G3D Read(MemoryMappedView view)
        {
            var reader = new BFastReader(view);
            var r = ReadFromGeometryBufferIfPossible(reader);
            if (r != null) 
                return r;

            var header = new G3dHeader();
            var attributes = new List<GeometryAttribute>();

            void OnBuffer(string name, MemoryMappedView subView, int index)
            {
                if (name == "meta")
                {
                    subView.Accessor.Read(0, out header);
                    return;
                }

                if (!AttributeDescriptor.TryParse(name, out var attributeDescriptor))
                    throw new Exception("Failed to parse attribute");

                // Populate a default attribute with the parsed attribute descriptor.
                var geometryAttribute = GeometryAttribute.Read(subView, attributeDescriptor);
                attributes.Add(geometryAttribute);
            }

            // Assume it is a normal G3D
            reader.Read(OnBuffer);
            return new G3D(attributes, header);
        }
        
        public static G3D Create(params GeometryAttribute[] attributes)
            => new G3D(attributes);

        public static G3D Create(G3dHeader header, params GeometryAttribute[] attributes)
            => new G3D(attributes, header);
    }
}
