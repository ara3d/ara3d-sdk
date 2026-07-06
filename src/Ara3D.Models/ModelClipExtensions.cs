using Ara3D.Geometry;

namespace Ara3D.Models;

/// <summary>Clips model instance meshes to half-spaces and bounding boxes.</summary>
public static class ModelClipExtensions
{
    /// <summary>Clips each instance mesh to the bounds, skipping instances that do not intersect.</summary>
    public static IModel3D ClipToBounds(this IModel3D model, Bounds3D bounds, Number padding = default)
    {
        var clipBounds = padding != 0
            ? bounds.Expand(new Vector3(padding, padding, padding))
            : bounds;

        var mb = new Model3DBuilder();
        var meshRemap = new Dictionary<int, int>();
        var instanceBounds = model.GetInstanceBounds();

        for (var i = 0; i < model.Instances.Count; i++)
        {
            var inst = model.Instances[i];
            if (inst.MeshIndex < 0)
            {
                mb.AddInstance(inst);
                continue;
            }

            var ib = instanceBounds[i];
            if (!ib.Intersects(clipBounds))
                continue;

            if (clipBounds.Contains(ib))
            {
                mb.AddInstanceAndRemapMesh(inst, model, meshRemap);
                continue;
            }

            var mesh = model.GetTransformedMesh(inst);
            var cutMesh = mesh.ClipToBounds(clipBounds);
            if (cutMesh.FaceIndices.Count == 0)
                continue;

            mb.AddInstance(cutMesh, inst.Material);
        }

        return mb.Build();
    }
}
