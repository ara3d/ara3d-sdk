namespace Ara3D.PropKit;

public class PropDescriptorString : TypedPropDescriptor<string>
{
    public string DefaultValue { get; }

    public PropDescriptorString(string name, string displayName, string description = "", string units = "",
        bool isReadOnly = false, string defaultValue = "")
        : base(name, displayName, description, units, isReadOnly)
    {
        DefaultValue = defaultValue;
    }

    public override string Update(string value, PropUpdateType propUpdate) => propUpdate switch
    {
        PropUpdateType.Min => DefaultValue,
        PropUpdateType.Max => DefaultValue,
        PropUpdateType.Default => DefaultValue,
        PropUpdateType.Inc => value,
        PropUpdateType.Dec => value,
        _ => value
    };

    public override string Validate(string value) => value;
    public override bool IsValid(string value) => true;
    public override bool AreEqual(string value1, string value2) => value1 == value2;
    public override object FromString(string value) => value;
    public override string ToString(string value) => value;

    protected override bool TryParse(string value, out string parsed)
    {
        parsed = value;
        return true;
    }
}
