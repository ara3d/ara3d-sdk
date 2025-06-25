using System.ComponentModel;

namespace Ara3D.PropKit;

public interface IBoundPropContainer
    : ICustomTypeDescriptor, IPropContainer
{
    bool TrySetValue(string name, object value);
    object GetValue(string name);
    IReadOnlyList<PropValue> GetPropValues();
}