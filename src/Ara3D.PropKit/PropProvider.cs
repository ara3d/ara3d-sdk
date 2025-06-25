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

    public IReadOnlyList<PropValue> GetPropValues(object obj)
        => Accessors.Select(acc => acc.GetPropValue(obj)).ToList();

    public PropAccessor GetAccessor(PropDescriptor propDesc)
    {
        var r = GetAccessor(propDesc.Name);
        if (r.Descriptor != propDesc)
            throw new Exception($"Stored descriptor {r.Descriptor} does not match {propDesc}");
        return r;
    }

    public PropAccessor GetAccessor(string name)
        => _dictionary.GetValueOrDefault(name);

    public PropValue GetPropValue(object obj, string name)
        => GetAccessor(name).GetPropValue(obj);

    public PropDescriptor GetDescriptor(string name)
        => GetAccessor(name).Descriptor;

    public bool TrySetValue(object obj, string name, object value)
    {
        var acc = GetAccessor(name);
        if (acc == null)
            return false;
        var cur = acc.GetValue(obj);
        if (cur.Equals(value))
            return true;
        acc.SetValue(obj, value);
        NotifyPropertyChanged(name);
        return true;
    }

    public bool TrySetValue(object obj, PropDescriptor descriptor, object value)
        => TrySetValue(obj, descriptor.Name, value);

    public void SetPropValues(object obj, IEnumerable<PropValue> values)
    {
        foreach (var value in values)
            TrySetValue(obj, value.Descriptor, value.Value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void NotifyPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public void Dispose()
        => PropertyChanged = null;

}