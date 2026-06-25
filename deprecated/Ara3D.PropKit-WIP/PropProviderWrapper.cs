using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;

namespace Ara3D.PropKit;

/// <summary>
/// Wraps a specific object, and provides an ICustomTypeDescriptor to that object.
/// This is useful for Data binding scenarios.
/// It is designed to allow you to change the wrapped object, which allows you to
/// reuse a set of properties. 
/// </summary>
public class PropProviderWrapper : 
    DynamicObject, IBoundPropContainer
{
    private object _wrapped;
    public object Wrapped
    {
        get => _wrapped;
        private set
        {
            _wrapped = value;
        }
    }

    public PropProvider Props { get; }

    public PropProviderWrapper(object wrapped, PropProvider props)
    {
        Wrapped = wrapped;
        Props = props;
    }

    public void UpdateWrappedObject(object bound)
    {
        Wrapped = bound;
        NotifyPropertyChanged();
    }

    public void NotifyPropertyChanged(string propName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
    }

    public dynamic AsDynamic
        => this;

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        try
        {
            result = Props.GetValue(Wrapped, binder.Name);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        try
        {
            return TrySetValue(binder.Name, value);
        }
        catch
        {
            return false;
        }
    }

    public bool TrySetValue(PropDescriptor desc, object value)
        => TrySetValue(desc.Name, value);

    public bool TrySetValue(string name, object value)
    {
        if (!Props.TrySetValue(ref _wrapped, name, value))
            return false;
        NotifyPropertyChanged(name);
        return true;
    }

    public object GetValue(string name)
        => Props.GetValue(Wrapped, name);

    public object GetValue(PropDescriptor descriptor)
        => GetValue(descriptor.Name);

    public IReadOnlyList<PropValue> GetPropValues()
        => Props.GetPropValues(Wrapped);

    public override IEnumerable<string> GetDynamicMemberNames()
        => Props.Accessors.Select(acc => acc.Descriptor.Name);

    public PropValue GetPropValue(object host, string name)
        => Props.GetPropValue(host, name);

    public PropDescriptor GetDescriptor(string name)
        => Props.GetDescriptor(name);

    public event PropertyChangedEventHandler PropertyChanged;

    //== 
    // ICustomTypeDescriptor implementation

    public AttributeCollection GetAttributes() => AttributeCollection.Empty;
    public string GetClassName() => nameof(PropProviderWrapper);
    public string GetComponentName() => null;
    public TypeConverter GetConverter() => new();
    public EventDescriptor GetDefaultEvent() => null;
    public PropertyDescriptor GetDefaultProperty() => null;
    public object GetEditor(Type editorBaseType) => null;
    public EventDescriptorCollection GetEvents(Attribute[] attributes) => EventDescriptorCollection.Empty;
    public EventDescriptorCollection GetEvents() => EventDescriptorCollection.Empty;
    public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => new(Props.GetDescriptors().Select(CreateAdapter).ToArray());
    public PropertyDescriptor CreateAdapter(PropDescriptor desc) => new ComponentModelAdapter(this, desc);
    public PropertyDescriptorCollection GetProperties() => GetProperties([]);
    public object GetPropertyOwner(PropertyDescriptor pd) => this;

    public void Dispose()
    {
        Wrapped = null;
        Props.Dispose();
    }

    public IReadOnlyList<PropDescriptor> GetDescriptors()
        => Props.GetDescriptors();

    public IReadOnlyList<PropValue> GetPropValues(object host)
        => Props.GetPropValues(host);
    
    public void SetPropValues(ref object host, IEnumerable<PropValue> values)
        => Props.SetPropValues(ref host, values);
}