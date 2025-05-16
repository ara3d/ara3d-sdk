namespace Ara3D.PropKit;

public class TypedPropDescriptorInt : TypedPropDescriptor<int>
{
    public int MinValue { get; }
    public int MaxValue { get; }
    public int SmallChange { get; }
    public int BigChange { get; }
    public int DefaultValue { get; }

    public TypedPropDescriptorInt(string name, string displayName, string description = "", string units = "",
        bool isReadOnly = false, bool isDeprecated = false, int defaultValue = 0,
        int minValue = int.MinValue, int maxValue = int.MaxValue, int smallChange = 1, int bigChange = 10)
        : base(name, displayName, description, units, isReadOnly, isDeprecated)
    {
        if (minValue > maxValue)
            throw new Exception($"The minValue {minValue} cannot be greater than maxValue {maxValue}");
        if (defaultValue < minValue || defaultValue > maxValue)
            throw new Exception(
                $"The defaultValue {defaultValue} cannot be less than {minValue} or greater than {maxValue}");
        DefaultValue = defaultValue;
        MinValue = minValue;
        MaxValue = maxValue;
        SmallChange = smallChange;
        BigChange = bigChange;
    }

    public override int Update(int value, PropUpdateType propUpdate) => Validate(propUpdate switch
    {
        PropUpdateType.Min => MinValue,
        PropUpdateType.Max => MaxValue,
        PropUpdateType.Default => DefaultValue,
        PropUpdateType.SmallInc => value + SmallChange,
        PropUpdateType.LargeInc => value + BigChange,
        PropUpdateType.SmallDec => value - SmallChange,
        PropUpdateType.LargeDec => value - BigChange,
        _ => value
    });

    public override int Validate(int value) => Math.Clamp(value, MinValue, MaxValue);
    public override bool IsValid(int value) => value >= MinValue && value <= MaxValue;
    public override bool AreEqual(int value1, int value2) => value1 == value2;
    public override object FromString(string value) => int.Parse(value);
    public override string ToString(int value) => value.ToString();
    protected override bool TryParse(string value, out int parsed) => int.TryParse(value, out parsed);
}