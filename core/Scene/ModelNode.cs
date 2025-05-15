using System.Runtime.CompilerServices;
using Ara3D.NarwhalDB;
using Plato;

namespace Ara3D.Scenes
{
    public record ModelNode(
        IModel Model, 
        Guid Id,
        string Name,
        TriangleMesh3D Mesh,
        Matrix4x4 Transform,
        Material Material) : IModelNode
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ModelNode(TriangleMesh3D mesh)
            => new(Guid.NewGuid(), mesh, Matrix4x4.Identity, Material.Default);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ModelNode Translate(Vector3 v)
            => this with { Transform = Transform * Matrix4x4.CreateTranslation(v) };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ModelNode TranslateTo(Vector3 v)
            => this with { Transform = Transform.WithTranslation(v) };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ModelNode Rotate(Vector3 v, Angle angle)
            => this with { Transform = Transform * Quaternion.CreateFromAxisAngle(v, angle) };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ModelNode Scale(Vector3 v)
            => this with { Transform = Transform * Matrix4x4.CreateScale(v.X, v.Y, v.Z) };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ModelNode Scale(Number n)
            => this with { Transform = Transform * Matrix4x4.CreateScale(n) };

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