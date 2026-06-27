using System;
using Ara3D.IfcLoader;

namespace Ara3D.BimOpenSchema.IO;

public static class IfcRelationMapping
{
    public static RelationType ToBos(IfcRelationKind kind)
        => kind switch
        {
            IfcRelationKind.ContainedIn => RelationType.ContainedIn,
            IfcRelationKind.MemberOf => RelationType.MemberOf,
            IfcRelationKind.ChildOf => RelationType.ChildOf,
            IfcRelationKind.PartOf => RelationType.PartOf,
            IfcRelationKind.HasMaterial => RelationType.HasMaterial,
            IfcRelationKind.HasLayer => RelationType.HasLayer,
            IfcRelationKind.Voids => RelationType.Voids,
            IfcRelationKind.Fills => RelationType.Fills,
            IfcRelationKind.ConnectsTo => RelationType.ConnectsTo,
            IfcRelationKind.HasConnector => RelationType.HasConnector,
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
}
