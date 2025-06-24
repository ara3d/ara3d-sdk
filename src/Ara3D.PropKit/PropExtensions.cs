namespace Ara3D.PropKit;

public static class PropExtensions
{
    public static PropDescriptor GetDescriptor(this PropValue self)
        => self.Descriptor;

    public static IPropContainer SafeSetValues(this IPropContainer self, object obj, IEnumerable<PropValue> values)
    {
        var descriptors = self.GetDescriptors().ToDictionary(d => d.Name);
        self.SetValues(obj, values.Where(v => descriptors.ContainsKey(v.Name)));
        return self;
    }

    public static IPropContainer CopyValuesFrom(this IPropContainer self, IPropContainer other, object src, object dest)
        => other == null ? self : self.SafeSetValues(dest, other.GetValues(src));
}