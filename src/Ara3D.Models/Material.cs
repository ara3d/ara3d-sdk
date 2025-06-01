using Plato;

namespace Ara3D.Models
{
    public record Material(Color Color, float Metallic, float Roughness)
    {
        public static Material Default 
            => new(new Color(0.5f,0.5f,0.5f,0), 0.1f, 0.5f);
    }
}
