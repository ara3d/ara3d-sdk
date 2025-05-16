using Plato;

namespace Ara3D.Scenes
{
    public static class ModelExtensions
    {
        public static IReadOnlyList<T1> Select<T0, T1>(this IReadOnlyList<T0> self, Func<T0, T1> f)
            => new ReadOnlyList<T1>(self.Count, i => f(self[i]));
        
        public static ModelNode ToNode(this TriangleMesh3D mesh, Matrix4x4 transform, Material material)
            => new(Guid.NewGuid(), "", mesh, transform, material);

        public static ModelNode ToNode(this TriangleMesh3D mesh, Matrix4x4 transform)
            => new(Guid.NewGuid(), "", mesh, transform, Material.Default);

        public static ModelNode ToNode(this TriangleMesh3D mesh)
            => new(Guid.NewGuid(), "", mesh, Matrix4x4.Identity, Material.Default);

        public static ModelNode ToNode(this TriangleMesh3D mesh, Material material)
            => new(Guid.NewGuid(), "", mesh, Matrix4x4.Identity, material);
    }
}