namespace Ara3D.PropKit;

public interface IPropProvider
{
    IReadOnlyList<string> Names { get; }
    Prop GetProp(string name);
}