using Plato;

namespace Ara3D.Models
{
    public static class ModelExtensions
    {
        public static ModelNode ToNode(this TriangleMesh3D mesh, Matrix4x4 transform, ModelMaterial material)
            => new(Guid.NewGuid(), "", mesh, transform, material);

        public static ModelNode ToNode(this TriangleMesh3D mesh, Matrix4x4 transform)
            => new(Guid.NewGuid(), "", mesh, transform, ModelMaterial.Default);

        public static ModelNode ToNode(this TriangleMesh3D mesh)
            => new(Guid.NewGuid(), "", mesh, Matrix4x4.Identity, ModelMaterial.Default);

        public static ModelNode ToNode(this TriangleMesh3D mesh, ModelMaterial material)
            => new(Guid.NewGuid(), "", mesh, Matrix4x4.Identity, material);
    }
}