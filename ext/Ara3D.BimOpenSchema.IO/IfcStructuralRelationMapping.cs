using System;
using Ara3D.IfcLoader;

namespace Ara3D.BimOpenSchema.IO;

public static class IfcStructuralRelationMapping
{
    public static RelationType ToBos(IfcStructuralRelationKind kind)
        => kind switch
        {
            IfcStructuralRelationKind.ContainedIn => RelationType.ContainedIn,
            IfcStructuralRelationKind.MemberOf => RelationType.MemberOf,
            IfcStructuralRelationKind.ChildOf => RelationType.ChildOf,
            _ => throw new ArgumentOutOfRangeException(nameof(kind)),
        };
}
