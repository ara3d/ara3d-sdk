using System.Runtime.CompilerServices;
using Plato;

namespace Ara3D.Models
{
    public record ModelNode(
        Guid Id,
        string Name,    
        TriangleMesh3D Mesh,
        Matrix4x4 Matrix,
        ModelMaterial Material) : ITransformable3D<ModelNode>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ModelNode(TriangleMesh3D mesh)
            => new(Guid.NewGuid(), "", mesh, Matrix4x4.Identity, ModelMaterial.Default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ModelNode Transform(Matrix4x4 matrix)
            => this with { Matrix = Matrix * matrix };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ModelNode WithColor(Color color)
            => this with { Material = Material with { Color = color } };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ModelNode WithRoughness(Number roughness)
            => this with { Material = Material with { Roughness = roughness } };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ModelNode WithMetallic(Number metallic)
            => this with { Material = Material with { Metallic = metallic } };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ModelNode WithName(string name)
            => this with { Name = name };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ModelNode WithId(Guid id)
            => this with { Id = id };
    }
}