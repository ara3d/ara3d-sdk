using System.ComponentModel;

namespace Ara3D.PropKit;

public interface IPropContainer 
    : INotifyPropertyChanged, IDisposable
{
    IReadOnlyList<PropDescriptor> GetDescriptors();
    IReadOnlyList<PropValue> GetValues(object obj);
    void SetValues(object obj, IEnumerable<PropValue> values);
}