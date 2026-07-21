using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static class ParameterHarvest
{
    public static IReadOnlyList<Parameter> GetParameters(this Element element)
    {
        var result = new List<Parameter>(element.Parameters.Size);
        foreach (Parameter parameter in element.Parameters)
            result.Add(parameter);
        return result;
    }

    /// <summary>
    /// Every parameter's display name mapped to its Revit-formatted display string
    /// (<see cref="ToDisplayString"/>) — a read-only snapshot for reporting/inspection, not a
    /// unit conversion (P7): the formatting comes from Revit's own <c>AsValueString</c>.
    /// Dedupes the WB/RS `GetParameterMap` twins.
    /// </summary>
    public static IReadOnlyDictionary<string, string> GetParameterMap(this Element element)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (Parameter parameter in element.Parameters)
        {
            var name = parameter.Definition?.Name;
            if (name is null)
                continue;
            map[name] = parameter.ToDisplayString();
        }

        return map;
    }

    /// <summary>
    /// Formats a parameter's value the way Revit itself would display it, handling every
    /// storage type. Dedupes WB `ToDisplayString`/`ParameterToString` and RS `ParameterToString`.
    /// </summary>
    public static string ToDisplayString(this Parameter parameter)
    {
        if (!parameter.HasValue)
            return string.Empty;

        return parameter.StorageType switch
        {
            StorageType.String => parameter.AsString() ?? string.Empty,
            StorageType.Integer => parameter.AsValueString() ?? parameter.AsInteger().ToString(),
            StorageType.Double => parameter.AsValueString() ?? parameter.AsDouble().ToString("G17"),
            StorageType.ElementId => FormatElementId(parameter.AsElementId()),
            _ => string.Empty,
        };
    }

    private static string FormatElementId(ElementId id)
        => id == ElementId.InvalidElementId ? string.Empty : id.ToLong().ToString();
}
