using System.Collections.Generic;
using Ara3D.Collections;
using Ara3D.Mathematics;

namespace Ara3D.Geometry
{
    public static class CloneOperations
    {
        public static ITriMesh Merge(this IEnumerable<ITriMesh> meshes)
            => meshes.ToIArray().Merge();

        public static ITriMesh Merge(params ITriMesh[] meshes)
            => meshes.Merge();

        public static ITriMesh Merge(this IArray<ITriMesh> meshes)
        {
            var bldr = new TriMeshBuilder();
            foreach (var mesh in meshes.Enumerate())
                bldr.Add(mesh);
            return bldr.ToMesh();
        }

        public static IArray<ITriMesh> Clone(this ITriMesh self, IArray<Vector3> points)
            => points.Select(p => self.Translate(p));

        public static IArray<ITriMesh> Clone(this ITriMesh self, IEnumerable<Vector3> points)
            => self.Clone(points.ToIArray());
    }
}
