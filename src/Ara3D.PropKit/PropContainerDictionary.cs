using System.ComponentModel;
namespace Ara3D.PropKit;

/// <summary>
/// Stores property values associated with names in a dictionary. The Values themselves are associated with specific
/// PropValues which bind a Descriptor to a Value. This allows us to validate the object. 
/// Descriptors are compared by reference to see if they are the same one.
/// If you want to use this class a Dynamic object, you can use the Provider.
/// </summary>
public class PropContainerDictionary : IPropContainer
{
    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    // This dictionary is kept here as a field for debugging purposes
    private readonly Dictionary<string, PropValue> _dict;
    public readonly PropProvider Provider;

    public static IEnumerable<PropAccessor> CreateAccessors(Dictionary<string, PropValue> dictionary)
    {
        foreach (var kv in dictionary)
        {
            var desc = kv.Value.Descriptor;
            var name = desc.Name;
            if (name != kv.Key)
                throw new Exception("Name does not match dictionary key");
            if (desc.IsReadOnly)
                yield return new PropAccessor(desc, () => dictionary[name].Value, null);
            else 
                yield return new PropAccessor(desc, () => dictionary[name].Value, val => dictionary[name] = new(val, desc));
        }
    }

    public PropContainerDictionary(IEnumerable<PropDescriptor> descriptors)
    {
        _dict = descriptors.ToDictionary(d => d.Name, d => d.DefaultPropValue);
        Provider = new PropProvider(CreateAccessors(_dict));
    }

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => Provider.PropertyChanged += value;
        remove => Provider.PropertyChanged -= value;
    }

    public dynamic AsDynamic
        => Provider;

    public void Dispose()
    {
        Provider.Dispose();
        _dict.Clear();
    }

    //==
    // Implementation of IPropContainer is delegated to Provider.

    public AttributeCollection GetAttributes() => Provider.GetAttributes();

    public string? GetClassName() => Provider.GetClassName();

    public string? GetComponentName() => Provider.GetComponentName();

    public TypeConverter? GetConverter() => Provider.GetConverter();

    public EventDescriptor? GetDefaultEvent() => Provider.GetDefaultEvent();

    public PropertyDescriptor? GetDefaultProperty() => Provider.GetDefaultProperty();

    public object? GetEditor(Type editorBaseType) => Provider.GetEditor(editorBaseType);

    public EventDescriptorCollection GetEvents() => Provider.GetEvents();

    public EventDescriptorCollection GetEvents(Attribute[]? attributes) => Provider.GetEvents(attributes);

    public PropertyDescriptorCollection GetProperties() => Provider.GetProperties();

    public PropertyDescriptorCollection GetProperties(Attribute[]? attributes) => Provider.GetProperties(attributes);

    public object? GetPropertyOwner(PropertyDescriptor? pd) => Provider.GetPropertyOwner(pd);

    public IReadOnlyList<PropValue> GetValues() => Provider.GetValues();

    public void SetValues(IEnumerable<PropValue> values) => Provider.SetValues(values);

    public object this[string name]
    {
        get => Provider[name];
        set => Provider[name] = value;
    }
}