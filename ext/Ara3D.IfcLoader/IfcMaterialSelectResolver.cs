namespace Ara3D.IfcLoader;

/// <summary>Expands IFC MaterialSelect values into HasMaterial / HasLayer edges.</summary>
public static class IfcMaterialSelectResolver
{
    public static void Expand(
        IfcEntityResolver resolver,
        int productId,
        IfcEntity materialSelect,
        List<IfcRelation> relations)
    {
        switch (materialSelect.GetEntityName())
        {
            case "IFCMATERIAL":
                relations.Add(new(productId, materialSelect.Id, IfcRelationKind.HasMaterial));
                break;
            case "IFCMATERIALLIST":
                foreach (var materialId in materialSelect.GetIdList(0))
                    if (materialId > 0)
                        relations.Add(new(productId, materialId, IfcRelationKind.HasMaterial));
                break;
            case "IFCMATERIALLAYERSETUSAGE":
                ExpandLayerSetUsage(resolver, productId, materialSelect, relations);
                break;
            case "IFCMATERIALLAYERSET":
                ExpandLayerSet(resolver, productId, materialSelect, relations);
                break;
            case "IFCMATERIALCONSTITUENTSET":
                ExpandConstituentSet(resolver, productId, materialSelect, relations);
                break;
        }
    }

    static void ExpandLayerSetUsage(
        IfcEntityResolver resolver,
        int productId,
        IfcEntity usage,
        List<IfcRelation> relations)
    {
        var layerSetId = usage.GetId(0);
        var layerSet = resolver.GetEntityOrDefault(layerSetId);
        if (layerSet != null)
            ExpandLayerSet(resolver, productId, layerSet, relations);
    }

    static void ExpandLayerSet(
        IfcEntityResolver resolver,
        int productId,
        IfcEntity layerSet,
        List<IfcRelation> relations)
    {
        foreach (var layerId in layerSet.GetIdList(0))
        {
            if (layerId <= 0)
                continue;
            relations.Add(new(productId, layerId, IfcRelationKind.HasLayer));
            var layer = resolver.GetEntityOrDefault(layerId);
            if (layer == null)
                continue;
            var materialId = layer.GetId(0);
            if (materialId > 0)
                relations.Add(new(layerId, materialId, IfcRelationKind.HasMaterial));
        }
    }

    static void ExpandConstituentSet(
        IfcEntityResolver resolver,
        int productId,
        IfcEntity constituentSet,
        List<IfcRelation> relations)
    {
        foreach (var constituentId in constituentSet.GetIdList(2))
        {
            if (constituentId <= 0)
                continue;
            var constituent = resolver.GetEntityOrDefault(constituentId);
            if (constituent == null)
                continue;
            var materialId = constituent.GetId(2);
            if (materialId > 0)
                relations.Add(new(productId, materialId, IfcRelationKind.HasMaterial));
        }
    }
}
