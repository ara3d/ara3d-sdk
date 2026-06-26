using Ara3D.IfcTypes.Ifc2x3;

namespace Ara3D.IfcLoader;

public enum IfcStructuralRelationKind
{
    ContainedIn,
    MemberOf,
    ChildOf,
}

public readonly record struct IfcStructuralRelation(int From, int To, IfcStructuralRelationKind Kind);

/// <summary>Parses spatial and decomposition IFC relationships into directed edges.</summary>
public sealed class IfcStructuralRelations
{
    public readonly List<IfcStructuralRelation> Relations = [];

    public IfcStructuralRelations(IfcFile file)
        : this(file.EntityResolver)
    { }

    public IfcStructuralRelations(IfcEntityResolver resolver)
    {
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
                ParseDecomposition(entity, IfcStructuralRelationKind.MemberOf);
                break;
            case IfcRelNests.ENTITY_CODE:
                ParseDecomposition(entity, IfcStructuralRelationKind.ChildOf);
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
                Relations.Add(new(elementId, structureId, IfcStructuralRelationKind.ContainedIn));
    }

    void ParseDecomposition(IfcEntity entity, IfcStructuralRelationKind kind)
    {
        var parentId = entity.GetId(4);
        if (parentId <= 0)
            return;
        foreach (var childId in entity.GetIdList(5))
            if (childId > 0)
                Relations.Add(new(childId, parentId, kind));
    }
}
