using System.ComponentModel;
using System.Dynamic;

namespace Ara3D.PropKit;

/// <summary>
/// Given a list of property accessors, this class implements IPropContainer.
/// It also derives from DynamicObject and can be declared as "dynamic" in C#. 
/// </summary>
public class PropProvider : DynamicObject, IPropContainer, IDisposable
{
    public IReadOnlyList<PropAccessor> Accessors { get; }
    private readonly Dictionary<string, PropAccessor> _dictionary;

    public static PropProvider Default 
        = new ([]);

    public PropProvider(IEnumerable<PropAccessor> accessors)
    {
        Accessors = accessors.ToList();
        _dictionary = Accessors.ToDictionary(acc => acc.Descriptor.Name, acc => acc);
    }

    public IReadOnlyList<PropValue> GetValues()
        => Accessors.Select(acc => acc.GetValue()).ToList();

    public PropValue GetValue(string name)
        => GetAccessor(name).GetValue();

    public PropAccessor GetAccessor(PropDescriptor propDesc)
    {
        var r = GetAccessor(propDesc.Name);
        if (r.Descriptor != propDesc)
            throw new Exception($"Stored descriptor {r.Descriptor} does not match {propDesc}");
        return r;
    }

    public PropAccessor GetAccessor(string name)
        => (!_dictionary.TryGetValue(name, out var accessor) 
                ? throw new Exception($"Could not find {name}") 
                : accessor)!;

    public PropValue GetValue(PropDescriptor propDesc)
        => GetAccessor(propDesc).GetValue();

    public void SetValue(string name, object value)
    {
        GetAccessor(name).SetValue(value);
        NotifyPropertyChanged(name);
    }

    public void SetValue(PropDescriptor descriptor, object value)
    {
        GetAccessor(descriptor).SetValue(value);
        NotifyPropertyChanged(descriptor.Name);
    }

    public void SetValue(PropValue value)
        => SetValue(value.Descriptor, value.Value);

    public void SetValues(IEnumerable<PropValue> values)
    {
        foreach (var value in values)
            SetValue(value);
    }

    public object this[string name]
    {
        get => GetValue(name);
        set => SetValue(name, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void NotifyPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public void Dispose()
    {
        PropertyChanged = null;
    }

    //==
    // DynamicObject overrides

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        try
        {
            result = GetValue(binder.Name);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        try
        {
            
            SetValue(binder.Name, value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override IEnumerable<string> GetDynamicMemberNames()
        => Accessors.Select(acc => acc.Descriptor.Name);
    
    //== 
    // ICustomTypeDescriptor implementation

    public AttributeCollection GetAttributes() => AttributeCollection.Empty;
    public string GetClassName() => nameof(PropProvider);
    public string GetComponentName() => null;
    public TypeConverter GetConverter() => new();
    public EventDescriptor GetDefaultEvent() => null;
    public PropertyDescriptor GetDefaultProperty() => null;
    public object GetEditor(Type editorBaseType) => null;
    public EventDescriptorCollection GetEvents(Attribute[] attributes) => EventDescriptorCollection.Empty;
    public EventDescriptorCollection GetEvents() => EventDescriptorCollection.Empty;
    public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => new(Accessors.Select(PropertyDescriptor (acc) => new ComponentModelAdapter(this, acc.Descriptor)).ToArray());
    public PropertyDescriptorCollection GetProperties() => GetProperties([]);
    public object GetPropertyOwner(PropertyDescriptor pd) => this;

    /// <summary>
    /// Adapts the PropertyAccessor to the "PropertyDescriptor" class in the System.ComponentModel namespace.
    /// </summary>
    private class ComponentModelAdapter : PropertyDescriptor
    {
        private readonly PropProvider _provider;
        private readonly PropDescriptor _desc;

        public ComponentModelAdapter(PropProvider provider, PropDescriptor desc) : base(desc.Name, null)
        {
            _provider = provider;
            _desc = desc;
        }

        public override Type ComponentType => typeof(PropProvider);
        public override bool IsReadOnly => _desc.IsReadOnly;
        public override Type PropertyType => _desc.Type;
        public override bool CanResetValue(object component) => true;
        public override object GetValue(object _) => _provider.GetValue(_desc).Value;
        public override void ResetValue(object component) => SetValue(component, _desc.Default);
        public override void SetValue(object _, object value) => _provider.SetValue(_desc, value);
        public override bool ShouldSerializeValue(object component) => false;
        public override bool SupportsChangeEvents => true;
    }
}