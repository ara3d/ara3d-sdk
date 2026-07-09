using Ara3D.IfcTypes;
using Ara3D.IO.StepParser;

namespace Ara3D.Ifc.Mesher.Approach1;

/// <summary>Resolves IFC length units to a scale factor (file units → meters).</summary>
public static class Units
{
    public static double ResolveLengthScaleToMeters(MeshingContext ctx)
    {
        foreach (var entity in ctx.Resolver.GetEntities())
        {
            if (entity.GetEntityName() is not ("IFCPROJECT" or "IFCUNITASSIGNMENT"))
                continue;

            if (entity.GetEntityName() == "IFCPROJECT")
            {
                var unitsId = entity.GetId(IfcProject.Instance.UnitsInContext.Index);
                if (unitsId > 0)
                {
                    var scale = ParseUnitAssignment(ctx, ctx.GetEntity(unitsId));
                    if (scale.HasValue)
                        return scale.Value;
                }
            }
        }

        foreach (var entity in ctx.Resolver.GetEntities())
        {
            if (entity.GetEntityName() == "IFCUNITASSIGNMENT")
            {
                var scale = ParseUnitAssignment(ctx, entity);
                if (scale.HasValue)
                    return scale.Value;
            }
        }

        return 1.0;
    }

    static double? ParseUnitAssignment(MeshingContext ctx, IfcEntity assignment)
    {
        var units = MeshHelpers.ReadIds(assignment, IfcUnitAssignment.Instance.Units);
        double? lengthScale = null;
        foreach (var unitId in units)
        {
            var unit = ctx.GetEntity(unitId);
            var scale = ParseUnit(ctx, unit);
            if (scale.HasValue)
                lengthScale = scale;
        }
        return lengthScale;
    }

    static double? ParseUnit(MeshingContext ctx, IfcEntity unit)
    {
        return unit.GetEntityName() switch
        {
            "IFCSIUNIT" => ParseSiUnit(unit),
            "IFCCONVERSIONBASEDUNIT" => ParseConversionBasedUnit(ctx, unit),
            "IFCMONETARYUNIT" => null,
            _ => null,
        };
    }

    static double? ParseSiUnit(IfcEntity unit)
    {
        var unitType = unit.GetString(IfcSIUnit.Instance.UnitType.Index);
        if (!unitType.Contains("LENGTH", StringComparison.OrdinalIgnoreCase))
            return null;

        var prefix = unit.GetString(IfcSIUnit.Instance.Prefix.Index);
        var name = unit.GetString(IfcSIUnit.Instance.Name.Index);
        if (!name.Contains("METRE", StringComparison.OrdinalIgnoreCase))
            return null;

        return prefix.ToUpperInvariant() switch
        {
            ".MILLI." or "MILLI" => 0.001,
            ".CENTI." or "CENTI" => 0.01,
            ".DECI." or "DECI" => 0.1,
            ".KILO." or "KILO" => 1000.0,
            _ when prefix.Contains("MILLI", StringComparison.OrdinalIgnoreCase) => 0.001,
            _ => 1.0,
        };
    }

    static double? ParseConversionBasedUnit(MeshingContext ctx, IfcEntity unit)
    {
        var unitType = unit.GetString(IfcConversionBasedUnit.Instance.UnitType.Index);
        if (!unitType.Contains("LENGTH", StringComparison.OrdinalIgnoreCase))
            return null;

        var factorId = unit.GetId(IfcConversionBasedUnit.Instance.ConversionFactor.Index);
        if (factorId <= 0)
            return null;

        var factorEntity = ctx.GetEntity(factorId);
        if (factorEntity.GetEntityName() != "IFCMEASUREWITHUNIT")
            return null;

        return ReadMeasureWithUnitScale(ctx, factorEntity);
    }

    static double ReadMeasureWithUnitScale(MeshingContext ctx, IfcEntity measureWithUnit)
    {
        var value = ReadNumericToken(
            measureWithUnit.GetValue(IfcMeasureWithUnit.Instance.ValueComponent.Index),
            measureWithUnit.Document);

        var unitId = measureWithUnit.GetId(IfcMeasureWithUnit.Instance.UnitComponent.Index);
        if (unitId <= 0)
            return value;

        var unitScale = ParseUnit(ctx, ctx.GetEntity(unitId));
        return unitScale.HasValue ? value * unitScale.Value : value;
    }

    static double ReadNumericToken(StepToken token, StepDocument doc)
    {
        if (token.IsNumber)
            return token.AsNumber();
        if (token.IsEntity)
        {
            var (_, inner) = token.AsSimpleEntity(doc);
            if (inner.IsNumber)
                return inner.AsNumber();
        }
        if (token.IsList)
        {
            foreach (var item in token.AsList(doc))
            {
                if (item.IsNumber)
                    return item.AsNumber();
                if (item.IsEntity)
                {
                    var (_, inner) = item.AsSimpleEntity(doc);
                    if (inner.IsNumber)
                        return inner.AsNumber();
                }
            }
        }
        return 0.0;
    }
}
