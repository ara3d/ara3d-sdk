using System.ComponentModel;

namespace Ara3D.PropKit;

public interface IPropContainer 
    : INotifyPropertyChanged, IDisposable
{
    IReadOnlyList<PropDescriptor> GetDescriptors();
    IReadOnlyList<PropValue> GetPropValues(object host);
    void SetPropValues(ref object host, IEnumerable<PropValue> values);
    PropValue GetPropValue(object host, string name);
    PropDescriptor GetDescriptor(string name);
}