using System;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

public static class ParameterLookup
{
    public static bool TryGetParameter(this Element element, string name, out Parameter parameter)
    {
        parameter = element.LookupParameter(name);
        return parameter is not null;
    }

    public static bool TryGetParameter(this Element element, BuiltInParameter builtInParameter, out Parameter parameter)
    {
        parameter = element.get_Parameter(builtInParameter);
        return parameter is not null;
    }

    public static Parameter GetParameter(this Element element, BuiltInParameter builtInParameter)
        => element.get_Parameter(builtInParameter)
           ?? throw new InvalidOperationException($"Element {element.Id} has no '{builtInParameter}' parameter.");

    /// <summary>
    /// Looks up a parameter by name and reports whether it is a shared parameter.
    /// </summary>
    public static bool HasSharedParameter(this Element element, string name, out Parameter parameter)
    {
        parameter = element.LookupParameter(name);
        return parameter is not null && parameter.IsShared;
    }

    /// <summary>
    /// The parameter's spec (unit family) as a ForgeTypeId, e.g. SpecTypeId.Length or
    /// SpecTypeId.String.Text — the modern replacement for the legacy ParameterType/UnitType
    /// enums (absorbed here so nothing downstream branches on them, P7).
    /// </summary>
    public static ForgeTypeId GetSpecTypeId(this Parameter parameter)
        => parameter.Definition.GetDataType();
}
