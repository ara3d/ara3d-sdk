using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;

namespace Ara3D.Revit.Utils;

/// <summary>Supply-air terminal queries and flow rounding. Values are internal units (cfm math is the caller's concern, P7).</summary>
public static class AirTerminalQueries
{
    public const double DefaultTerminalFlowIncrementCfm = 5;
    private const string SupplyAirClassification = "Supply Air";

    /// <summary>Duct-terminal family instances classified as supply air. One line on C1's parameter-filter building block (<see cref="ElementFilters.GetElementsWithParameter{T}"/>).</summary>
    public static IReadOnlyList<FamilyInstance> GetSupplyAirTerminals(this Document doc)
        => doc.GetElementsWithParameter<FamilyInstance>(BuiltInCategory.OST_DuctTerminal,
            BuiltInParameter.RBS_SYSTEM_CLASSIFICATION_PARAM, SupplyAirClassification);

    /// <summary>Supply-air terminals grouped by the number of the space they serve; terminals with no space are omitted.</summary>
    public static IReadOnlyDictionary<string, IReadOnlyList<FamilyInstance>> GetTerminalsPerSpace(this Document doc)
        => doc.GetSupplyAirTerminals()
            .Where(t => !string.IsNullOrEmpty(t.Space?.Number))
            .GroupBy(t => t.Space.Number)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<FamilyInstance>)g.ToList());

    /// <summary>Rounds a cfm flow value to the nearest terminal increment, away from zero.</summary>
    public static double RoundFlowToIncrement(double valueCfm, double incrementCfm = DefaultTerminalFlowIncrementCfm)
        => Math.Round(valueCfm / incrementCfm, 0, MidpointRounding.AwayFromZero) * incrementCfm;
}
