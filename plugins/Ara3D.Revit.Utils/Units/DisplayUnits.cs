using Autodesk.Revit.DB;

namespace Ara3D.Revit.Utils;

/// <summary>
/// Document-bound display-unit formatting (ForgeTypeId path only; the legacy UnitType
/// overloads are absorbed by C6's version firewall, not here, per P7). Pure math lives
/// in the other half of this partial class (see UnitMath.cs) so it stays testable.
/// </summary>
public static partial class Units
{
    public static double ToDisplayUnits(this double internalValue, Document doc, ForgeTypeId spec)
        => UnitUtils.ConvertFromInternalUnits(internalValue, doc.GetUnits().GetFormatOptions(spec).GetUnitTypeId());

    public static string FormatLength(this double internalFeet, Document doc)
        => UnitFormatUtils.Format(doc.GetUnits(), SpecTypeId.Length, internalFeet, false);

    public static string GetUnitSymbol(this Document doc, ForgeTypeId spec)
    {
        var symbol = doc.GetUnits().GetFormatOptions(spec).GetSymbolTypeId();
        return symbol.Empty() ? string.Empty : LabelUtils.GetLabelForSymbol(symbol);
    }

    /// <summary>
    /// The single parameter-display entry point (decision 012): the parameter's raw
    /// value converted into whatever unit the document currently displays it in.
    /// </summary>
    public static double ParameterValueInDisplayUnits(this Parameter parameter)
        => UnitUtils.ConvertFromInternalUnits(parameter.AsDouble(), parameter.GetUnitTypeId());
}
