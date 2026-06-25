namespace Ara3D.PropKit;

public interface IPropAccessor
{
    bool HasSetter { get; }
    object GetValue(object host);
    void SetValue(ref object host, object value);
}
