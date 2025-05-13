using System.ComponentModel;

namespace Ara3D.PropKit;

public interface IPropContainer : INotifyPropertyChanged, ICustomTypeDescriptor
{
    IReadOnlyList<PropValue> GetValues();
    void SetValues(IEnumerable<PropValue> values);
    object this[string name] { get; set; }
}

public static class PropExtensions
{
    public static IEnumerable<PropDescriptor> GetDescriptors(this IPropContainer self)
        => self.GetValues().GetDescriptors();

    public static IEnumerable<PropDescriptor> GetDescriptors(this IEnumerable<PropValue> self)
        => self.Select(GetDescriptor);
    
    public static PropDescriptor GetDescriptor(this PropValue self)
        => self.Descriptor;
}