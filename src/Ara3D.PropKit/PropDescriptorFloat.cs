using System.Globalization;

namespace Ara3D.PropKit;

public class PropDescriptorFloat : TypedPropDescriptor<float>
{
    public override float MinValue { get; }
    public override float MaxValue { get; }
    public override float DefaultValue { get; }

    public PropDescriptorFloat(string name, string displayName, string description = "", string units = "",
        bool isReadOnly = false, float defaultValue = 0f,
        float minValue = float.MinValue, float maxValue = float.MaxValue)
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

    public override float Validate(float value) => Math.Clamp(value, MinValue, MaxValue);
    public override bool IsValid(float value) => value >= MinValue && value <= MaxValue;
    public override bool AreEqual(float value1, float value2) => Math.Abs(value1 - value2) < 1e-5;
    public override object FromString(string value) => float.Parse(value, CultureInfo.InvariantCulture);
    public override string ToString(float value) => value.ToString(CultureInfo.InvariantCulture);
    protected override bool TryParse(string value, out float parsed) => float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed);
}