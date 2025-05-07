using Plato;

namespace Ara3D.Scenes
{
    public record SceneNode(
        TriangleMesh3D Mesh,
        Matrix4x4 Transform,
        Material Material)
    {
        public static implicit operator SceneNode(TriangleMesh3D mesh)
            => new(mesh, Matrix4x4.Identity, Material.Default);

        public SceneNode Translate(Vector3 v)
            => this with { Transform = Transform * Matrix4x4.CreateTranslation(v) };

        public SceneNode TranslateTo(Vector3 v)
            => this with { Transform = Transform.WithTranslation(v) };

        public SceneNode Rotate(Vector3 v, Angle angle)
            => this with { Transform = Transform * Quaternion.CreateFromAxisAngle(v, angle) };

        public SceneNode Scale(Vector3 v)
            => this with { Transform = Transform * Matrix4x4.CreateScale(v.X, v.Y, v.Z) };

        public SceneNode Scale(Number n)
            => this with { Transform = Transform * Matrix4x4.CreateScale(n) };

        public SceneNode WithColor(Color color)
            => this with { Material = Material with { Color = color } };

        public SceneNode WithRoughness(Number roughness)
            => this with { Material = Material with { Roughness = roughness } };

        public SceneNode WithMetallic(Number metallic)
            => this with { Material = Material with { Metallic = metallic } };
    }
}