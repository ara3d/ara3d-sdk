
namespace Ara3D.PropKit;

public class PropDescriptorBool : TypedPropDescriptor<bool>
{
    public PropDescriptorBool(string name, string displayName, string description = "", string units = "", bool isReadOnly = false)
        : base(name, displayName, description, units, isReadOnly) { }

    public override bool Update(bool value, PropUpdateType propUpdate) => propUpdate switch
    {
        PropUpdateType.Min => false,
        PropUpdateType.Max => true,
        PropUpdateType.Default => false,
        PropUpdateType.Inc => true,
        PropUpdateType.Dec => false,
        _ => value
    };

    public override bool IsValid(bool value) => true;
    public override bool Validate(bool value) => value;
    public override bool AreEqual(bool value1, bool value2) => value1 == value2;
    public override object FromString(string value) => bool.Parse(value);
    public override string ToString(bool value) => value.ToString();
    protected override bool TryParse(string value, out bool parsed) => bool.TryParse(value, out parsed);
}