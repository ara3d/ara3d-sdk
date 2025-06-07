using Ara3D.Geometry;

namespace Ara3D.Models
{
    public record Material(Color Color, float Metallic, float Roughness)
    {
        public static Color DefaultColor
            = new(0.5f, 0.5f, 0.5f, 1f);

        public static float DefaultMetallic
            = 0.1f;

        public static float DefaultRoughness
            = 0.5f;

        public static Material Default 
            = new(DefaultColor, DefaultMetallic, DefaultRoughness);
        
        public Material WithColor(Color color)
            => new(color, Metallic, Roughness);

        public Material WithMetallic(float metallic)
            => new(Color, metallic, Roughness);

        public Material WithRoughness(float roughness)
            => new(Color, Metallic, roughness);

    }
}
