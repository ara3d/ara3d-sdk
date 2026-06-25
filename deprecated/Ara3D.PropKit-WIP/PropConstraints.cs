namespace Ara3D.PropKit;

public readonly record struct PropConstraints(
    object Default = null,
    object Min = null,
    object Max = null
)
{
    public bool HasMinMax => Min != null && Max != null;
}