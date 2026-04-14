
namespace Ara3D.PropKit;

public class PropDescriptorBool : TypedPropDescriptor<bool>
{
    public PropDescriptorBool(string name, string displayName, string description = "", string units = "", bool isReadOnly = false)
        : base(name, displayName, description, units, isReadOnly) { }

    public override bool MinValue => false;
    public override bool MaxValue => true;
    public override bool DefaultValue => false;

    public override bool IsValid(bool value) => true;
    public override bool Validate(bool value) => value;
    public override bool AreEqual(bool value1, bool value2) => value1 == value2;
    public override object FromString(string value) => bool.Parse(value);
    public override string ToString(bool value) => value.ToString();
    protected override bool TryParse(string value, out bool parsed) => bool.TryParse(value, out parsed);
}