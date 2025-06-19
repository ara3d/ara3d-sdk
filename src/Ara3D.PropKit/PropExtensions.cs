namespace Ara3D.PropKit;

public static class PropExtensions
{
    public static IEnumerable<PropDescriptor> GetDescriptors(this IPropContainer self)
        => self.GetValues().GetDescriptors();

    public static IEnumerable<PropDescriptor> GetDescriptors(this IEnumerable<PropValue> self)
        => self.Select(GetDescriptor);
    
    public static PropDescriptor GetDescriptor(this PropValue self)
        => self.Descriptor;

    public static IPropContainer SafeSetValues(this IPropContainer self, IEnumerable<PropValue> values)
    {
        var descriptors = self.GetDescriptors().ToDictionary(d => d.Name);
        self.SetValues(values.Where(v => descriptors.ContainsKey(v.Name)));
        return self;
    }

    public static IPropContainer CopyValuesFrom(this IPropContainer self, IPropContainer other)
        => other == null ? self : self.SafeSetValues(other.GetValues());
}