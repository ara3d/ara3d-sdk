using System.Numerics;
using Assimp;

namespace Ara3D.AssimpLoader;

public class AssimpMaterial
{
    public Material Material { get; }
    public string Name => Material.Name;
    public Vector4 ColorDiffuse => Material.ColorDiffuse;
    public bool HasDiffuse => Material.HasColorDiffuse;
    public Vector4 ColorSpecular => Material.ColorSpecular;
    public bool HasSpecular => Material.HasColorSpecular;
    public float Shininess => Material.Shininess;
    public bool HasShininess => Material.HasShininess;
    public float ShininessStrength => Material.ShininessStrength;
    public bool HasShininessStrength => Material.HasShininessStrength;

    public AssimpMaterial(Material material)
        => Material = material;
        
    public static AssimpMaterial Create(Material material)
        => new(material);
}