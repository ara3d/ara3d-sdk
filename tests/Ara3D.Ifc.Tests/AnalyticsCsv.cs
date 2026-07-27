namespace Ara3D.Ifc.Tests;

/// <summary>One row of the analytics dataset that gets attached to IFC elements as a property set.</summary>
public readonly record struct AnalyticsRow(
    string GlobalId, string Name, string Level, double OperationalCarbon, double EnergyIntensity, string Category)
{
    public IReadOnlyList<IfcPropertyValue> ToProperties()
        => new[]
        {
            IfcPropertyValue.Label("Level", Level),
            IfcPropertyValue.Real("operational_carbon", OperationalCarbon),
            IfcPropertyValue.Real("energy_intensity", EnergyIntensity),
            IfcPropertyValue.Label("category", Category),
        };
}

public static class AnalyticsCsv
{
    /// <summary>Expects header GlobalId,Name,Level,operational_carbon,energy_intensity,category.</summary>
    public static IReadOnlyList<AnalyticsRow> Load(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        if (lines.Length == 0)
            throw new FormatException("Empty CSV");
        var rows = new List<AnalyticsRow>(lines.Length - 1);
        for (var i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (line.Length == 0)
                continue;
            var f = line.Split(',');
            if (f.Length != 6)
                throw new FormatException($"Line {i + 1}: expected 6 fields, got {f.Length}");
            rows.Add(new AnalyticsRow(f[0], f[1], f[2], double.Parse(f[3]), double.Parse(f[4]), f[5]));
        }
        return rows;
    }
}
