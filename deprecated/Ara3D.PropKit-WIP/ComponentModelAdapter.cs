using System.ComponentModel;
using Ara3D.Utils;

namespace Ara3D.PropKit;

/// <summary>
/// Adapts the PropertyAccessor to the "PropertyDescriptor" class in the System.ComponentModel namespace.
/// </summary>
public class ComponentModelAdapter : PropertyDescriptor
{
    private object _wrapped;
    private readonly PropDescriptor _desc;
    private readonly PropProviderWrapper _provider;
    private readonly IPropAccessor _accessor;

    public ComponentModelAdapter(PropProviderWrapper provider, PropDescriptor desc)
        : base(desc.Name, null)
    {
        _desc = desc ?? throw new ArgumentNullException(nameof(desc));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _wrapped = _provider?.Wrapped;
        _accessor = provider.Props.GetAccessor(desc) ?? throw new Exception("Could not find accessor");
    }

    public override Type ComponentType 
        => typeof(PropProviderWrapper);
    
    public override bool IsReadOnly 
        => _desc.IsReadOnly;
    
    public override Type PropertyType 
        => _desc.Type;
    
    public override bool CanResetValue(object component) 
        => true;

    public override object GetValue(object obj)
        => _accessor.GetValue(_wrapped);

    public override void ResetValue(object component)
        => SetValue(component, _desc.Default);

    public override void SetValue(object obj, object value)
    {
        _accessor.SetValue(ref _wrapped, value);
        _provider.NotifyPropertyChanged(Name);
    }

    public override bool ShouldSerializeValue(object component) 
        => false;
    
    public override bool SupportsChangeEvents 
        => true;
}