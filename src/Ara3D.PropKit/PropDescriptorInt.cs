namespace Ara3D.PropKit;

public class PropDescriptorInt : TypedPropDescriptor<int>
{
    public int MinValue { get; }
    public int MaxValue { get; }
    public int DefaultValue { get; }

    public PropDescriptorInt(string name, string displayName, string description = "", string units = "",
        bool isReadOnly = false, int defaultValue = 0,
        int minValue = int.MinValue, int maxValue = int.MaxValue)
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

    public override int Update(int value, PropUpdateType propUpdate) => Validate(propUpdate switch
    {
        PropUpdateType.Min => MinValue,
        PropUpdateType.Max => MaxValue,
        PropUpdateType.Default => DefaultValue,
        PropUpdateType.Inc => value + 1,
        PropUpdateType.Dec => value - 1,
        _ => value
    });

    public override int Validate(int value) => Math.Clamp(value, MinValue, MaxValue);
    public override bool IsValid(int value) => value >= MinValue && value <= MaxValue;
    public override bool AreEqual(int value1, int value2) => value1 == value2;
    public override object FromString(string value) => int.Parse(value);
    public override string ToString(int value) => value.ToString();
    protected override bool TryParse(string value, out int parsed) => int.TryParse(value, out parsed);
}