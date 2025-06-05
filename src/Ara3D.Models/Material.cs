using Ara3D.Geometry;

namespace Ara3D.Models
{
    public record Material(Color Color, float Metallic, float Roughness)
    {
        public static Material Default 
            => new(new Color(0.5f,0.5f,0.5f,0), 0.1f, 0.5f);
        
        public Material WithColor(Color color)
            => new(color, Metallic, Roughness);

        public Material WithMetallic(float metallic)
            => new(Color, metallic, Roughness);

        public Material WithRoughness(float roughness)
            => new(Color, Metallic, roughness);
    }
}
