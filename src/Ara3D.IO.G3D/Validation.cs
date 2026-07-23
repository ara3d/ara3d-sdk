using System.Collections.Generic;
using System.Linq;

namespace Ara3D.IO.G3D
{
    public enum G3dErrors
    {
        NodesCountMismatch,
        MaterialsCountMismatch,
        IndicesInvalidCount,
        IndicesOutOfRange,
        AttributeElementCountMismatch,

        //Submeshes
        SubmeshesCountMismatch,
        SubmeshesIndexOffsetInvalidIndex,
        SubmeshesIndexOffsetOutOfRange,
        SubmeshesNonPositive,
        SubmeshesMaterialOutOfRange,

        //Meshes
        MeshesSubmeshOffsetOutOfRange,
        MeshesSubmeshCountNonPositive,
        
        // Instances
        InstancesCountMismatch,
        InstancesParentOutOfRange,
        InstancesMeshOutOfRange,
    }

    public static class Validation
    {
        public static IEnumerable<G3dErrors> Validate(G3D g3d)
        {
            var errors = new List<G3dErrors>();

            void Validate(bool value, G3dErrors error)
            {
                if (!value) errors.Add(error);
            }

            // Every attribute sharing an association must agree on its component count
            // (all vertex attributes agree, corner attributes match the index/corner count, etc.).
            // A negative expected count means the association has no fixed component count to check against.
            int ExpectedElementCount(Association assoc)
            {
                switch (assoc)
                {
                    case Association.assoc_vertex: return g3d.NumVertices;
                    case Association.assoc_corner:
                    case Association.assoc_edge: return g3d.NumCorners;
                    case Association.assoc_face: return g3d.NumFaces;
                    case Association.assoc_instance: return g3d.NumInstances;
                    case Association.assoc_submesh: return g3d.NumSubmeshes;
                    case Association.assoc_material: return g3d.NumMaterials;
                    case Association.assoc_mesh: return g3d.NumMeshes;
                    case Association.assoc_shapevertex: return g3d.NumShapeVertices;
                    case Association.assoc_shape: return g3d.NumShapes;
                    default: return -1;
                }
            }

            foreach (var attr in g3d.Attributes)
            {
                var expected = ExpectedElementCount(attr.Descriptor.Association);
                if (expected >= 0)
                    Validate(attr.ElementCount == expected, G3dErrors.AttributeElementCountMismatch);
            }

            //Indices
            Validate(g3d.Indices.Count % 3 == 0, G3dErrors.IndicesInvalidCount);
            Validate(g3d.Indices.All(i => i >= 0  && i < g3d.NumVertices), G3dErrors.IndicesOutOfRange);
            //Triangle should have 3 distinct vertices
            //Assert.That(g3d.Indices.SubArrays(3).Select(face => face.ToEnumerable().Distinct().Count()).All(c => c == 3));

            //Submeshes
            Validate(g3d.NumSubmeshes >= g3d.NumMeshes, G3dErrors.SubmeshesCountMismatch);
            Validate(g3d.NumSubmeshes == g3d.SubmeshMaterials.Count, G3dErrors.SubmeshesCountMismatch);
            Validate(g3d.NumSubmeshes == g3d.SubmeshIndexOffsets.Count, G3dErrors.SubmeshesCountMismatch);
            Validate(g3d.SubmeshIndexOffsets.All(i => i % 3 == 0), G3dErrors.SubmeshesIndexOffsetInvalidIndex);
            Validate(g3d.SubmeshIndexOffsets.All(i => i >= 0 && i < g3d.NumCorners), G3dErrors.SubmeshesIndexOffsetOutOfRange);
            Validate(g3d.SubmeshIndexCount.All(i => i > 0), G3dErrors.SubmeshesNonPositive);
            Validate(g3d.SubmeshMaterials.All(m => m < g3d.NumMaterials), G3dErrors.SubmeshesMaterialOutOfRange);

            //Mesh
            Validate(g3d.MeshSubmeshOffset.All(i => i >= 0 && i < g3d.NumSubmeshes), G3dErrors.MeshesSubmeshOffsetOutOfRange);
            Validate(g3d.MeshSubmeshCount.All(i => i > 0), G3dErrors.MeshesSubmeshCountNonPositive);

            //Instances
            Validate(g3d.NumInstances == g3d.InstanceParents.Count, G3dErrors.InstancesCountMismatch);
            Validate(g3d.NumInstances == g3d.InstanceMeshes.Count, G3dErrors.InstancesCountMismatch);
            Validate(g3d.NumInstances == g3d.InstanceTransforms.Count, G3dErrors.InstancesCountMismatch);
            Validate(g3d.NumInstances == g3d.InstanceFlags.Count, G3dErrors.InstancesCountMismatch);
            Validate(g3d.InstanceParents.All(i => i < g3d.NumInstances), G3dErrors.InstancesParentOutOfRange);
            Validate(g3d.InstanceMeshes.All(i => i < g3d.NumMeshes), G3dErrors.InstancesMeshOutOfRange);

            //Materials
            Validate(g3d.NumMaterials == g3d.MaterialColors.Count, G3dErrors.MaterialsCountMismatch);
            Validate(g3d.NumMaterials == g3d.MaterialGlossiness.Count, G3dErrors.MaterialsCountMismatch);
            Validate(g3d.NumMaterials == g3d.MaterialSmoothness.Count, G3dErrors.MaterialsCountMismatch);

            return errors;
        }
    }
}
