using Plato;

namespace Ara3D.Models
{
    public static class ModelExtensions
    {
        public static Model3DNode ToNode(this TriangleMesh3D mesh, Matrix4x4 transform, Material material)
            => new(Guid.NewGuid(), "", mesh, transform, material);

        public static Model3DNode ToNode(this TriangleMesh3D mesh, Matrix4x4 transform)
            => new(Guid.NewGuid(), "", mesh, transform, Material.Default);

        public static Model3DNode ToNode(this TriangleMesh3D mesh)
            => new(Guid.NewGuid(), "", mesh, Matrix4x4.Identity, Material.Default);

        public static Model3DNode ToNode(this TriangleMesh3D mesh, Material material)
            => new(Guid.NewGuid(), "", mesh, Matrix4x4.Identity, material);
    }
}