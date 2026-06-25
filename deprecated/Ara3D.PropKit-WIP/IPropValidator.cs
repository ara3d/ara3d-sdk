namespace Ara3D.PropKit;

public interface IPropValidator
{
    object Coerce(object value);
    bool IsValid(object value);
}
