using System.ComponentModel;

namespace Ara3D.PropKit;

/// <summary>
/// Given a list of property accessors, this class implements IPropContainer.
/// </summary>
public class PropProvider : IPropContainer
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

    public IReadOnlyList<PropDescriptor> GetDescriptors()
        => Accessors.Select(acc => acc.Descriptor).ToList();

    public IReadOnlyList<PropValue> GetValues(object obj)
        => Accessors.Select(acc => acc.GetValue(obj)).ToList();

    public PropValue GetValue(object obj, string name)
        => GetAccessor(name).GetValue(obj);

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

    public PropValue GetValue(object obj, PropDescriptor propDesc)
        => GetAccessor(propDesc).GetValue(obj);

    public void SetValue(object obj, string name, object value)
    {
        GetAccessor(name).SetValue(obj, value);
        NotifyPropertyChanged(name);
    }

    public void SetValue(object obj, PropDescriptor descriptor, object value)
    {
        GetAccessor(descriptor).SetValue(obj, value);
        NotifyPropertyChanged(descriptor.Name);
    }

    public void SetValue(object obj, PropValue value)
        => SetValue(obj, value.Descriptor, value.Value);

    public void SetValues(object obj, IEnumerable<PropValue> values)
    {
        foreach (var value in values)
            SetValue(obj,value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void NotifyPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public void Dispose()
    {
        PropertyChanged = null;
    }

}