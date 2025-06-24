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
public class PropProviderWrapper : DynamicObject, ICustomTypeDescriptor, IPropContainer
{
    public object Wrapped { get; private set; }
    public PropProvider Props { get; }

    public PropProviderWrapper(object wrapped, PropProvider props)
    {
        Wrapped = wrapped;
        Props = props;
    }

    public void UpdateWrappedObject(object bound)
    {
        Wrapped = bound;
        Props.NotifyPropertyChanged(string.Empty);
    }

    public dynamic AsDynamic
        => this;

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
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

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        try
        {
            Props.SetValue(Wrapped, binder.Name, value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override IEnumerable<string> GetDynamicMemberNames()
        => Props.Accessors.Select(acc => acc.Descriptor.Name);

    public event PropertyChangedEventHandler PropertyChanged
    {
        add => Props.PropertyChanged += value;
        remove => Props.PropertyChanged -= value;
    }


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
    public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => new(Props.GetDescriptors().Select(CreateAdapter).ToArray());
    public PropertyDescriptor CreateAdapter(PropDescriptor desc) => new ComponentModelAdapter(Props, desc);
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

        public override object GetValue(object obj)
        {
            if (obj is not PropProviderWrapper ppw)
                throw new Exception($"Internal error: expected a {nameof(PropProviderWrapper)}");
            return _provider.GetValue(ppw.Wrapped, _desc).Value;
        }

        public override void ResetValue(object component) => SetValue(component, _desc.Default);

        public override void SetValue(object obj, object value)
        {
            if (obj is not PropProviderWrapper ppw)
                throw new Exception($"Internal error: expected a {nameof(PropProviderWrapper)}");
            _provider.SetValue(ppw.Wrapped, _desc, value);
        }

        public override bool ShouldSerializeValue(object component) => false;
        public override bool SupportsChangeEvents => true;
    }

    public void Dispose()
    {
        Wrapped = null;
        Props.Dispose();
    }

    public IReadOnlyList<PropDescriptor> GetDescriptors()
        => Props.GetDescriptors();

    public IReadOnlyList<PropValue> GetValues(object obj)
        => Props.GetValues(obj);

    public IReadOnlyList<PropValue> GetValues()
        => GetValues(Wrapped);

    public void SetValues(object obj, IEnumerable<PropValue> values)
        => Props.SetValues(obj, values);

    public void SetValues(IEnumerable<PropValue> values)
        => SetValues(Wrapped, values);
}