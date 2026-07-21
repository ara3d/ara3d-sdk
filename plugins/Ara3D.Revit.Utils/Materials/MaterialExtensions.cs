using System.Collections.Generic;
using Autodesk.Revit.DB;
using RevitMaterial = Autodesk.Revit.DB.Material;
using Material = Ara3D.Models.Material;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Material queries. Pure (P1) — never opens a transaction. Bridges Revit's <see cref="RevitMaterial"/>
/// to the sdk's <see cref="Material"/> (P2); no bespoke material DTO. The bridge covers the graphics
/// color/transparency only — walking the appearance (rendering) asset for PBR base color/metallic/
/// roughness is deferred (see D6 work log).
/// </summary>
public static class MaterialExtensions
{
    public static IReadOnlyList<RevitMaterial> GetMaterials(this Document doc)
        => doc.GetElements<RevitMaterial>();

    /// <summary>The Revit material with an exact name match, or null (P8). Call <see cref="ToMaterial"/> to bridge to the sdk type.</summary>
    public static RevitMaterial? FindMaterialByName(this Document doc, string name)
        => doc.FindByName<RevitMaterial>(name);

    /// <summary>Throwing form of <see cref="FindMaterialByName"/> (§3.3 <c>Get…</c> contract).</summary>
    public static RevitMaterial GetMaterialByName(this Document doc, string name)
        => doc.GetByName<RevitMaterial>(name);

    public static Material ToMaterial(this RevitMaterial material)
        => new(material.Color.ToColor().WithA(1f - material.Transparency / 100f),
            Material.DefaultMetallic, Material.DefaultRoughness);

    public static Material? GetMaterial(this Document doc, ElementId materialId)
        => materialId is null || materialId == ElementId.InvalidElementId
            ? null
            : (doc.GetElement(materialId) as RevitMaterial)?.ToMaterial();

    /// <summary>The element's own material parameter, or its category's material as a fallback.</summary>
    public static Material? GetMaterial(this Element e)
        => e.Document.GetMaterial(e.GetMaterialId());

    private static ElementId GetMaterialId(this Element e)
    {
        if (e.TryGetParameter(BuiltInParameter.MATERIAL_ID_PARAM, out var p)
            && p.StorageType == StorageType.ElementId
            && p.AsElementId() != ElementId.InvalidElementId)
            return p.AsElementId();
        return e.Category?.Material?.Id ?? ElementId.InvalidElementId;
    }
}
