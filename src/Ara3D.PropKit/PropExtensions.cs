namespace Ara3D.PropKit;

public static class PropExtensions
{
   public static IPropContainer SafeSetValues(this IPropContainer self, object obj, IEnumerable<PropValue> values)
    {
        var descriptors = self.GetDescriptors().ToDictionary(d => d.Name);
        self.SetPropValues(obj, values.Where(v => descriptors.ContainsKey(v.Name)));
        return self;
    }

    public static IPropContainer CopyValuesFrom(this IPropContainer self, IPropContainer other, object src, object dest)
        => other == null ? self : self.SafeSetValues(dest, other.GetPropValues(src));

    public static object GetValue(this IPropContainer self, object host, string name)
        => self.GetPropValue(host, name).Value;

    public static bool HasProperty(this IPropContainer container, string name)
        => container.GetDescriptor(name) != null;
}