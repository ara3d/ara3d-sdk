using Color = Ara3D.Geometry.Color;

namespace Ara3D.Bowerbird.RevitSamples;

/// <summary>PBR parameters we care about.</summary>
public sealed record PbrMaterialInfo
(
    string Name,
    Color ShadingColor,    // Legacy material.Color (always present)
    Color? BaseColor,       // “generic_diffuse” / “UnifiedDiffuse”
    double? Metallic,        // “generic_metallic” / “UnifiedMetallic”  (0–1)
    double? Roughness,       // “generic_roughness”/ “UnifiedRoughness” (0–1)
    Color? EmissiveColor    // “generic_emission” / “UnifiedEmission”
);