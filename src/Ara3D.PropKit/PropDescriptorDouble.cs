using System.Globalization;

namespace Ara3D.PropKit;

public class PropDescriptorDouble: TypedPropDescriptor<double>
{
    public override double MinValue { get; }
    public override double MaxValue { get; }
    public override double DefaultValue { get; }

    public PropDescriptorDouble(string name, string displayName, string description = "", string units = "",
        bool isReadOnly = false, double defaultValue = 0f,
        double minValue = double.MinValue, double maxValue = double.MaxValue)
        : base(name, displayName, description, units, isReadOnly)
    {
        if (minValue > maxValue)
            throw new Exception($"The minValue {minValue} cannot be greater than maxValue {maxValue}");
        if (defaultValue < minValue || defaultValue > maxValue)
            throw new Exception(
                $"The defaultValue {defaultValue} cannot be less than {minValue} or greater than {maxValue}");
        DefaultValue = defaultValue;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    public override double Validate(double value) => Math.Clamp(value, MinValue, MaxValue);
    public override bool IsValid(double value) => value >= MinValue && value <= MaxValue;
    public override bool AreEqual(double value1, double value2) => Math.Abs(value1 - value2) < 1e-5;
    public override object FromString(string value) => double.Parse(value, CultureInfo.InvariantCulture);
    public override string ToString(double value) => value.ToString(CultureInfo.InvariantCulture);
    protected override bool TryParse(string value, out double parsed) => double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed);
}