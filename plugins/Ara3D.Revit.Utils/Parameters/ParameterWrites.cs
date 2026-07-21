using System;
using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

// Typed writes. Values are raw Revit internal units (feet, radians, …) — callers convert
// before calling in (P7). Every member here requires an open transaction; none is opened
// by this class (P1).
public static class ParameterWrites
{
    /// <summary>requires an open transaction</summary>
    public static void SetParameter(this Element element, string name, double value)
        => element.GetWritableParameter(name).Set(value);

    /// <summary>requires an open transaction</summary>
    public static void SetParameter(this Element element, string name, int value)
        => element.GetWritableParameter(name).Set(value);

    /// <summary>requires an open transaction</summary>
    public static void SetParameter(this Element element, string name, string value)
        => element.GetWritableParameter(name).Set(value);

    /// <summary>requires an open transaction</summary>
    public static void SetParameter(this Element element, string name, ElementId value)
        => element.GetWritableParameter(name).Set(value);

    /// <summary>requires an open transaction</summary>
    public static void SetParameter(this Element element, BuiltInParameter builtInParameter, double value)
        => element.GetWritableParameter(builtInParameter).Set(value);

    /// <summary>requires an open transaction</summary>
    public static void SetParameter(this Element element, BuiltInParameter builtInParameter, int value)
        => element.GetWritableParameter(builtInParameter).Set(value);

    /// <summary>requires an open transaction</summary>
    public static void SetParameter(this Element element, BuiltInParameter builtInParameter, string value)
        => element.GetWritableParameter(builtInParameter).Set(value);

    /// <summary>requires an open transaction</summary>
    public static void SetParameter(this Element element, BuiltInParameter builtInParameter, ElementId value)
        => element.GetWritableParameter(builtInParameter).Set(value);

    /// <summary>
    /// Sets a parameter by name, returning false instead of throwing when it is missing or
    /// read-only (P8). Throws <see cref="ArgumentException"/> for a value type the parameter
    /// system doesn't accept — that is a caller bug, not an expected absence.
    /// requires an open transaction
    /// </summary>
    public static bool TrySetParameter(this Element element, string name, object value)
        => TrySetValue(element.LookupParameter(name), value);

    /// <summary>Built-in-parameter form of <see cref="TrySetParameter(Element, string, object)"/>. requires an open transaction</summary>
    public static bool TrySetParameter(this Element element, BuiltInParameter builtInParameter, object value)
        => TrySetValue(element.get_Parameter(builtInParameter), value);

    private static bool TrySetValue(Parameter? parameter, object value)
    {
        if (parameter is not { IsReadOnly: false })
            return false;

        switch (value)
        {
            case double d:
                parameter.Set(d);
                return true;
            case int i:
                parameter.Set(i);
                return true;
            case string s:
                parameter.Set(s);
                return true;
            case ElementId id:
                parameter.Set(id);
                return true;
            default:
                throw new ArgumentException($"Unsupported parameter value type '{value?.GetType()}'.", nameof(value));
        }
    }

    private static Parameter GetWritableParameter(this Element element, string name)
        => RequireWritable(element.LookupParameter(name), element, name);

    private static Parameter GetWritableParameter(this Element element, BuiltInParameter builtInParameter)
        => RequireWritable(element.get_Parameter(builtInParameter), element, builtInParameter.ToString());

    private static Parameter RequireWritable(Parameter? parameter, Element element, string name)
    {
        if (parameter is null)
            throw new ArgumentException($"Element {element.Id.ToLong()} has no parameter named '{name}'.", nameof(name));
        if (parameter.IsReadOnly)
            throw new InvalidOperationException($"Parameter '{name}' on element {element.Id.ToLong()} is read-only.");
        return parameter;
    }
}
