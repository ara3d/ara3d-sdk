using System.ComponentModel;

namespace Ara3D.PropKit;

public interface IPropContainer : INotifyPropertyChanged, ICustomTypeDescriptor, IDisposable
{
    IReadOnlyList<PropValue> GetValues();
    void SetValues(IEnumerable<PropValue> values);
    object this[string name] { get; set; }
}