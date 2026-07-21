using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Named category one-liners (decision 013) — this is their home so C1 stays
/// generic (P6); each delegates to <see cref="FamilyQueries.GetFamilyInstances"/>
/// (no duplication, P2).
/// </summary>
public static class FamilyCategoryQueries
{
    public static IReadOnlyList<FamilyInstance> GetDoors(this Document doc)
        => doc.GetFamilyInstances(BuiltInCategory.OST_Doors);

    public static IReadOnlyList<FamilyInstance> GetWindows(this Document doc)
        => doc.GetFamilyInstances(BuiltInCategory.OST_Windows);

    public static IReadOnlyList<FamilyInstance> GetLights(this Document doc)
        => doc.GetFamilyInstances(BuiltInCategory.OST_LightingFixtures);

    public static IReadOnlyList<FamilyInstance> GetSockets(this Document doc)
        => doc.GetFamilyInstances(BuiltInCategory.OST_ElectricalFixtures);
}
