using Ara3D.IfcTypes.Ifc2x3;

namespace Ara3D.IfcLoader;

public enum IfcRelationKind
{
    ContainedIn,
    MemberOf,
    ChildOf,
    PartOf,
    HasMaterial,
    HasLayer,
    Voids,
    Fills,
    ConnectsTo,
    HasConnector,
}

public readonly record struct IfcRelation(int From, int To, IfcRelationKind Kind);

/// <summary>Parses IFC relationship entities into directed BOS-oriented edges.</summary>
public sealed class IfcRelations
{
    public readonly List<IfcRelation> Relations = [];

    readonly IfcEntityResolver _resolver;

    public IfcRelations(IfcFile file)
        : this(file.EntityResolver)
    { }

    public IfcRelations(IfcEntityResolver resolver)
    {
        _resolver = resolver;
        foreach (var entity in resolver.GetEntities())
            ParseEntity(entity);
    }

    void ParseEntity(IfcEntity entity)
    {
        switch (entity.GetEntityCode())
        {
            case IfcRelContainedInSpatialStructure.ENTITY_CODE:
                ParseContainedIn(entity);
                break;
            case IfcRelAggregates.ENTITY_CODE:
                ParseDecomposition(entity, IfcRelationKind.MemberOf);
                break;
            case IfcRelNests.ENTITY_CODE:
                ParseNests(entity);
                break;
            case IfcRelAssignsToGroup.ENTITY_CODE:
                ParseAssignsToGroup(entity);
                break;
            case IfcRelProjectsElement.ENTITY_CODE:
                ParseProjectsElement(entity);
                break;
            case IfcRelVoidsElement.ENTITY_CODE:
                ParseVoidsElement(entity);
                break;
            case IfcRelFillsElement.ENTITY_CODE:
                ParseFillsElement(entity);
                break;
            case IfcRelAssociatesMaterial.ENTITY_CODE:
                ParseAssociatesMaterial(entity);
                break;
            case IfcRelConnectsElements.ENTITY_CODE:
            case IfcRelConnectsPathElements.ENTITY_CODE:
                ParseConnectsElements(entity);
                break;
            case IfcRelConnectsPorts.ENTITY_CODE:
                ParseConnectsPorts(entity);
                break;
            case IfcRelConnectsPortToElement.ENTITY_CODE:
                ParseConnectsPortToElement(entity);
                break;
        }
    }

    void ParseContainedIn(IfcEntity entity)
    {
        var structureId = entity.GetId(5);
        if (structureId <= 0)
            return;
        foreach (var elementId in entity.GetIdList(4))
            if (elementId > 0)
                Relations.Add(new(elementId, structureId, IfcRelationKind.ContainedIn));
    }

    void ParseDecomposition(IfcEntity entity, IfcRelationKind kind)
    {
        var parentId = entity.GetId(4);
        if (parentId <= 0)
            return;
        foreach (var childId in entity.GetIdList(5))
            if (childId > 0)
                Relations.Add(new(childId, parentId, kind));
    }

    void ParseNests(IfcEntity entity)
    {
        var parentId = entity.GetId(4);
        if (parentId <= 0)
            return;
        foreach (var childId in entity.GetIdList(5))
        {
            if (childId <= 0)
                continue;
            var child = _resolver.GetEntityOrDefault(childId);
            var kind = child != null && child.GetEntityCode() == IfcPort.ENTITY_CODE
                ? IfcRelationKind.HasConnector
                : IfcRelationKind.ChildOf;
            Relations.Add(new(
                kind == IfcRelationKind.HasConnector ? parentId : childId,
                kind == IfcRelationKind.HasConnector ? childId : parentId,
                kind));
        }
    }

    void ParseAssignsToGroup(IfcEntity entity)
    {
        var groupId = entity.GetId(6);
        if (groupId <= 0)
            return;
        foreach (var objectId in entity.GetIdList(4))
            if (objectId > 0)
                Relations.Add(new(objectId, groupId, IfcRelationKind.MemberOf));
    }

    void ParseProjectsElement(IfcEntity entity)
    {
        var hostId = entity.GetId(4);
        var featureId = entity.GetId(5);
        if (hostId <= 0 || featureId <= 0)
            return;
        Relations.Add(new(featureId, hostId, IfcRelationKind.PartOf));
    }

    void ParseVoidsElement(IfcEntity entity)
    {
        var hostId = entity.GetId(4);
        var openingId = entity.GetId(5);
        if (hostId <= 0 || openingId <= 0)
            return;
        Relations.Add(new(openingId, hostId, IfcRelationKind.Voids));
    }

    void ParseFillsElement(IfcEntity entity)
    {
        var openingId = entity.GetId(4);
        var elementId = entity.GetId(5);
        if (openingId <= 0 || elementId <= 0)
            return;
        Relations.Add(new(elementId, openingId, IfcRelationKind.Fills));
    }

    void ParseAssociatesMaterial(IfcEntity entity)
    {
        var materialId = entity.GetId(5);
        if (materialId <= 0)
            return;
        var materialEntity = _resolver.GetEntityOrDefault(materialId);
        if (materialEntity == null)
            return;
        foreach (var objectId in entity.GetIdList(4))
            if (objectId > 0)
                IfcMaterialSelectResolver.Expand(_resolver, objectId, materialEntity, Relations);
    }

    void ParseConnectsElements(IfcEntity entity)
    {
        var relatingId = entity.GetId(5);
        var relatedId = entity.GetId(6);
        if (relatingId <= 0 || relatedId <= 0)
            return;
        Relations.Add(new(relatedId, relatingId, IfcRelationKind.ConnectsTo));
    }

    void ParseConnectsPorts(IfcEntity entity)
    {
        var relatingPortId = entity.GetId(4);
        var relatedPortId = entity.GetId(5);
        if (relatingPortId <= 0 || relatedPortId <= 0)
            return;
        Relations.Add(new(relatedPortId, relatingPortId, IfcRelationKind.ConnectsTo));
    }

    void ParseConnectsPortToElement(IfcEntity entity)
    {
        var portId = entity.GetId(4);
        var elementId = entity.GetId(5);
        if (portId <= 0 || elementId <= 0)
            return;
        Relations.Add(new(elementId, portId, IfcRelationKind.HasConnector));
    }
}
