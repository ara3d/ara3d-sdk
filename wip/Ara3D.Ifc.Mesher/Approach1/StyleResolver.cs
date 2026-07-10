using Ara3D.Geometry;
using Ara3D.IfcLoader;
using Ara3D.IfcTypes;
using Ara3D.Models;

namespace Ara3D.Ifc.Mesher.Approach1;

/// <summary>Resolves IFC presentation styles on <c>IfcStyledItem</c> into instance materials.</summary>
public static class StyleResolver
{
    const float StyledMetallic = 0.1f;
    const float StyledRoughness = 0.5f;

    /// <summary>
    /// Maps geometry representation-item express ids to materials from every <c>IfcStyledItem</c>
    /// that references them (IFC often lists the solid in Body items and styles it via inverse
    /// <c>StyledByItem</c>, rather than putting the styled item in the representation).
    /// </summary>
    public static Dictionary<int, Material> BuildItemMaterialMap(MeshingContext ctx)
    {
        var map = new Dictionary<int, Material>();
        foreach (var entity in ctx.Resolver.GetEntities())
        {
            if (entity.GetEntityName() != "IFCSTYLEDITEM")
                continue;
            var itemId = MeshHelpers.ReadOptionalId(entity, IfcStyledItem.Instance.Item);
            if (itemId is null)
                continue;
            var material = TryResolveMaterial(ctx, entity);
            if (material is null || map.ContainsKey(itemId.Value))
                continue;
            map[itemId.Value] = material.Value;
        }
        return map;
    }

    /// <summary>Returns a material from the first resolvable surface style, or null if none.</summary>
    public static Material? TryResolveMaterial(MeshingContext ctx, IfcEntity styledItem)
    {
        foreach (var styleId in MeshHelpers.ReadIds(styledItem, IfcStyledItem.Instance.Styles))
        {
            var style = ctx.GetEntityOrDefault(styleId);
            if (style is null)
                continue;
            var material = TryResolveStyleSelect(ctx, style);
            if (material is not null)
                return material;
        }
        return null;
    }

    static Material? TryResolveStyleSelect(MeshingContext ctx, IfcEntity entity)
        => entity.GetEntityName() switch
        {
            "IFCPRESENTATIONSTYLEASSIGNMENT" => TryResolvePresentationStyleAssignment(ctx, entity),
            "IFCSURFACESTYLE" => TryResolveSurfaceStyle(ctx, entity),
            _ => null,
        };

    static Material? TryResolvePresentationStyleAssignment(MeshingContext ctx, IfcEntity assignment)
    {
        foreach (var styleId in MeshHelpers.ReadIds(assignment, IfcPresentationStyleAssignment.Instance.Styles))
        {
            var style = ctx.GetEntityOrDefault(styleId);
            if (style is null)
                continue;
            var material = TryResolveStyleSelect(ctx, style);
            if (material is not null)
                return material;
        }
        return null;
    }

    static Material? TryResolveSurfaceStyle(MeshingContext ctx, IfcEntity surfaceStyle)
    {
        foreach (var elementId in MeshHelpers.ReadIds(surfaceStyle, IfcSurfaceStyle.Instance.Styles))
        {
            var element = ctx.GetEntityOrDefault(elementId);
            if (element is null)
                continue;
            var name = element.GetEntityName();
            if (name is not ("IFCSURFACESTYLERENDERING" or "IFCSURFACESTYLESHADING"))
                continue;
            var material = TryResolveShading(ctx, element);
            if (material is not null)
                return material;
        }
        return null;
    }

    static Material? TryResolveShading(MeshingContext ctx, IfcEntity shading)
    {
        var colour = MeshHelpers.ResolveOptional(ctx, shading, IfcSurfaceStyleShading.Instance.SurfaceColour);
        if (colour is null)
            return null;

        var r = (float)MeshHelpers.ReadNumber(colour, IfcColourRgb.Instance.Red);
        var g = (float)MeshHelpers.ReadNumber(colour, IfcColourRgb.Instance.Green);
        var b = (float)MeshHelpers.ReadNumber(colour, IfcColourRgb.Instance.Blue);

        var transparencyToken = shading.GetValue(IfcSurfaceStyleShading.Instance.Transparency.Index);
        var transparency = transparencyToken.IsNumber ? transparencyToken.AsNumber() : 0.0;
        var alpha = 1f - (float)transparency;

        return new Material(new Color(r, g, b, alpha), StyledMetallic, StyledRoughness);
    }
}
