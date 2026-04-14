namespace Ara3D.PropKit;

public class PropDescriptorAction: TypedPropDescriptor<Action>
{
    public PropDescriptorAction(string name, string displayName, string description = "", string units = "", bool isReadOnly = false)
        : base(name, displayName, description, units, isReadOnly) { }

    public override Action MinValue => null;
    public override Action MaxValue => null;
    public override Action DefaultValue => null;

    public override bool IsValid(Action value) => true;
    public override Action Validate(Action value) => value;
    public override bool AreEqual(Action value1, Action value2) => value1 == value2;
    public override object FromString(string value) => null;
    public override string ToString(Action value) => "";

    protected override bool TryParse(string value, out Action parsed)
    {
        parsed = null;
        return false;
    }
}