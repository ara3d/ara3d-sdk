namespace Ara3D.PropKit;

public interface IPropStringCodec
{
    bool TryParse(PropDescriptor descriptor, string text, out object value);
    string Format(PropDescriptor descriptor, object value);
}