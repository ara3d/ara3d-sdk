using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

// Typed reads. Values are raw Revit internal units (feet, radians, …) — never converted
// silently (P7); display-unit formatting is C5's job. Parameter-level TryGet* are the
// building blocks; the Element-level name/BuiltInParameter overloads route through them.
public static class ParameterReads
{
    public static bool TryGetString(this Parameter? parameter, out string value)
    {
        if (parameter is { StorageType: StorageType.String, HasValue: true })
        {
            value = parameter.AsString() ?? string.Empty;
            return true;
        }
        value = string.Empty;
        return false;
    }

    public static bool TryGetDouble(this Parameter? parameter, out double value)
    {
        if (parameter is { StorageType: StorageType.Double, HasValue: true })
        {
            value = parameter.AsDouble();
            return true;
        }
        value = default;
        return false;
    }

    public static bool TryGetInt(this Parameter? parameter, out int value)
    {
        if (parameter is { StorageType: StorageType.Integer, HasValue: true })
        {
            value = parameter.AsInteger();
            return true;
        }
        value = default;
        return false;
    }

    public static bool TryGetElementId(this Parameter? parameter, out ElementId value)
    {
        if (parameter is { StorageType: StorageType.ElementId, HasValue: true })
        {
            value = parameter.AsElementId();
            return true;
        }
        value = ElementId.InvalidElementId;
        return false;
    }

    public static bool TryGetString(this Element element, string name, out string value)
        => element.LookupParameter(name).TryGetString(out value);

    public static bool TryGetDouble(this Element element, string name, out double value)
        => element.LookupParameter(name).TryGetDouble(out value);

    public static bool TryGetInt(this Element element, string name, out int value)
        => element.LookupParameter(name).TryGetInt(out value);

    public static bool TryGetElementId(this Element element, string name, out ElementId value)
        => element.LookupParameter(name).TryGetElementId(out value);

    public static bool TryGetString(this Element element, BuiltInParameter builtInParameter, out string value)
        => element.get_Parameter(builtInParameter).TryGetString(out value);

    public static bool TryGetDouble(this Element element, BuiltInParameter builtInParameter, out double value)
        => element.get_Parameter(builtInParameter).TryGetDouble(out value);

    public static bool TryGetInt(this Element element, BuiltInParameter builtInParameter, out int value)
        => element.get_Parameter(builtInParameter).TryGetInt(out value);

    public static bool TryGetElementId(this Element element, BuiltInParameter builtInParameter, out ElementId value)
        => element.get_Parameter(builtInParameter).TryGetElementId(out value);

    public static string GetStringOrEmpty(this Element element, BuiltInParameter builtInParameter)
        => element.TryGetString(builtInParameter, out var value) ? value : string.Empty;

    public static string GetStringOrEmpty(this Element element, string name)
        => element.TryGetString(name, out var value) ? value : string.Empty;

    public static double? GetDouble(this Element element, BuiltInParameter builtInParameter)
        => element.TryGetDouble(builtInParameter, out var value) ? value : null;

    public static double? GetDouble(this Element element, string name)
        => element.TryGetDouble(name, out var value) ? value : null;

    public static int? GetInt(this Element element, BuiltInParameter builtInParameter)
        => element.TryGetInt(builtInParameter, out var value) ? value : null;

    public static int? GetInt(this Element element, string name)
        => element.TryGetInt(name, out var value) ? value : null;
}
