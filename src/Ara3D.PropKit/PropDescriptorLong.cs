namespace Ara3D.PropKit;

public class PropDescriptorLong : TypedPropDescriptor<long>
{
    public long MinValue { get; }
    public long MaxValue { get; }
    public long DefaultValue { get; }

    public PropDescriptorLong(string name, string displayName, string description = "", string units = "",
        bool isReadOnly = false, long defaultValue = 0,
        long minValue = long.MinValue, long maxValue = long.MaxValue)
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

    public override long Update(long value, PropUpdateType propUpdate) => Validate(propUpdate switch
    {
        PropUpdateType.Min => MinValue,
        PropUpdateType.Max => MaxValue,
        PropUpdateType.Default => DefaultValue,
        PropUpdateType.Inc => value + 1,
        PropUpdateType.Dec => value - 1,
        _ => value
    });

    public override long Validate(long value) => Math.Clamp(value, MinValue, MaxValue);
    public override bool IsValid(long value) => value >= MinValue && value <= MaxValue;
    public override bool AreEqual(long value1, long value2) => value1 == value2;
    public override object FromString(string value) => long.Parse(value);
    public override string ToString(long value) => value.ToString();
    protected override bool TryParse(string value, out long parsed) => long.TryParse(value, out parsed);
}