namespace Ara3D.PropKit;

/// <summary>
/// A bound object and a set of accessors.
/// </summary>
public class PropContainerWrapper : PropProvider
{
    public object BoundObject { get; }

    public PropContainerWrapper(object bound)
        : base(PropFactory.CreateFromObject(bound))
    {
        BoundObject = bound;
    }
}