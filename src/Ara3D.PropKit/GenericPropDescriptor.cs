namespace Ara3D.PropKit;

public class GenericPropDescriptor : PropDescriptor
{
    public object DefaultValue; 

    public override bool AreEqual(object value1, object value2) => value1.Equals(value2);
    public override object FromString(string value) => null;
    public override bool IsValid(object value) => true;
    public override bool IsValidString(string value) => false;
    public override string ToString(object value) => value.ToString();
    public override object Update(object value, PropUpdateType propUpdate) => propUpdate == PropUpdateType.Default ? DefaultValue : value;
    public override object Validate(object value) => value;

    public GenericPropDescriptor(object defaultValue, Type type, string name = null, string displayName = null,
        string description = null, string units = null, bool isReadOnly = false)
        : base(type, name, displayName, description, units, isReadOnly)
    {
        DefaultValue = defaultValue;
    }
}