using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using System;
using Color = Ara3D.Geometry.Color;
using Material = Autodesk.Revit.DB.Material;

namespace Ara3D.Bowerbird.RevitSamples;

public static class MaterialExtensions
{
    public static PbrMaterialInfo? GetPbrInfo(this Document doc, long materialId)
    {
        // 1. Look up the Material
        var mat = doc.GetElement(new ElementId(materialId)) as Material;
        if (mat is null) return null;
            
        // 2. Graphics transparency (0..100), convert to opacity (0..1)
        //    0   => fully opaque
        //    100 => fully transparent
        var opacityGraphics = 1.0 - mat.Transparency / 100.0;

        // 3. Graphics shading colour (will be used as fallback / shading color)
        var shadingColor = new Color(
            mat.Color.Red / 255f,
            mat.Color.Green / 255f,
            mat.Color.Blue / 255f,
            1f); // alpha applied later

        // 4. Try to reach the rendering (appearance) asset
        var assetEl = doc.GetElement(mat.AppearanceAssetId) as AppearanceAssetElement;
        if (assetEl is null)
        {
            // No appearance asset: fall back entirely to graphics
            var legacy = shadingColor.WithA((float)opacityGraphics);
            return new PbrMaterialInfo(mat.Name, legacy, null, null, null, null);
        }

        var asset = assetEl.GetRenderingAsset();

        // 5. Parameters we care about
        Color? baseCol = null;
        Color? emissive = null;
        double? metallic = null;
        double? roughness = null;

        // Transparency/opacity and related
        double? opacityPbr = null;    // e.g. generic_opacity / UnifiedOpacity
        double? transparency = null;  // e.g. generic_transparency (0..1, 1=fully transparent)
        double? glossiness = null;    // e.g. generic_glossiness (0..1, 1=very glossy -> low roughness)

        // 6. Walk top-level asset properties
        for (var i = 0; i < asset.Size; i++)
        {
            var prop = asset[i];

            switch (prop)
            {
                case AssetPropertyDoubleArray4d col:
                    {
                        var c = ToDrawingColor(col);
                        switch (prop.Name)
                        {
                            // Base / diffuse colour
                            case "generic_diffuse":
                            case "UnifiedDiffuse":
                            case "BaseColor":
                                baseCol = c;
                                break;

                            // Emissive colour
                            case "generic_emission":
                            case "UnifiedEmission":
                                emissive = c;
                                break;
                        }
                        break;
                    }

                case AssetPropertyDouble d:
                    {
                        switch (prop.Name)
                        {
                            // Metalness
                            case "generic_metallic":
                            case "UnifiedMetallic":
                                metallic = d.Value;
                                break;

                            // Roughness
                            case "generic_roughness":
                            case "UnifiedRoughness":
                                roughness = d.Value;
                                break;

                            // Explicit opacity factor (0..1 opaque)
                            case "generic_opacity":
                            case "UnifiedOpacity":
                                opacityPbr = d.Value;
                                break;

                            // Legacy transparency (0..1, where 1 = fully transparent)
                            case "generic_transparency":
                                transparency = d.Value;
                                break;

                            // Legacy glossiness (0..1, where 1 = very glossy -> low roughness)
                            case "generic_glossiness":
                                glossiness = d.Value;
                                break;
                        }
                        break;
                    }
            }
        }

        // 7. Derive roughness from glossiness if needed (simple heuristic)
        if (!roughness.HasValue && glossiness.HasValue)
            roughness = 1.0 - glossiness.Value;

        // 8. Combine graphics opacity with appearance opacity/transparency
        double opacityAsset = 1.0;

        // generic_transparency: 0 = opaque, 1 = fully transparent
        if (transparency.HasValue)
            opacityAsset *= (1.0 - transparency.Value);

        // generic_opacity / UnifiedOpacity: already opaque in [0..1]
        if (opacityPbr.HasValue)
            opacityAsset *= opacityPbr.Value;

        // Final alpha 0..1
        var alpha = (float)(opacityGraphics * opacityAsset);
        if (alpha < 0f) alpha = 0f;
        else if (alpha > 1f) alpha = 1f;

        // Apply alpha to colours
        shadingColor = shadingColor.WithA(alpha);
        if (baseCol.HasValue)
            baseCol = baseCol.Value.WithA(alpha);

        return new PbrMaterialInfo(
            mat.Name,
            shadingColor,   // "shading" colour (graphics)
            baseCol,        // appearance base colour (if any)
            metallic,
            roughness,
            emissive);
    }

    private static Color ToDrawingColor(AssetPropertyDoubleArray4d col)
    {
        var dbls = col.GetValueAsDoubles();
        var r = (float)dbls[0];
        var g = (float)dbls[1];
        var b = (float)dbls[2];
        var a = (float)dbls[3];
        return new Color(r, g, b, a);
    }

    public static Models.Material? ToAra3DMaterial(this PbrMaterialInfo pbr)
        => pbr == null
            ? null
            : new Models.Material(
                pbr.BaseColor ?? pbr.ShadingColor, 
                (float)(pbr.Metallic ?? 0),
                (float)(pbr.Roughness ?? 1));

    public static Models.Material? ToAra3DMaterial(this Document doc, ElementId? materialId)
    {
        if (doc == null)
            return null;
        if (materialId == null)
            return null;
        if (materialId == ElementId.InvalidElementId)
            return null;
        return (doc.GetElement(materialId) as Material).ToAra3DMaterial();
    }

    public static Models.Material? ToAra3DMaterial(this Material mat)
    {
        if (mat == null)
            return null;
        var alpha = 1f - mat.Transparency / 100f;
        var metallic = 0f;
        var roughness = 1f;
        var color = new Color(
            mat.Color.Red / 255f,
            mat.Color.Green / 255f,
            mat.Color.Blue / 255f,
            alpha);
        return new(color, metallic, roughness);
    }


    public static Models.Material? GetAra3DMaterial(this Document self, Face f)
        => f == null ? null : ToAra3DMaterial(self, f?.MaterialElementId);

    public static Models.Material? GetAra3DMaterial(this Document self, Mesh m)
        => m == null ? null : ToAra3DMaterial(self, m?.MaterialElementId);

    public static Models.Material? ResolveFallbackMaterial(this Element e)
        => e == null ? null : ToAra3DMaterial(e.Document, ResolveFallbackMaterialId(e));

    public static ElementId ResolveFallbackMaterialId(Element e)
    {
        // Try element's own material parameter first 
        var pMat = e.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM);
        if (pMat is { StorageType: StorageType.ElementId })
        {
            var id = pMat.AsElementId();
            if (id != ElementId.InvalidElementId)
                return id;
        }

        // Use the category material as a fallback 
        var mat = e.Category?.Material;
        return mat != null
            ? mat.Id
            : ElementId.InvalidElementId;
    }
}