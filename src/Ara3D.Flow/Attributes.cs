namespace Ara3D.Studio.API;

/// <summary>
/// Shared menu-path constants. The <see cref="Delimiter"/> separates the segments of a
/// <c>[Category]</c> string into nested menu levels; both the examples project (when it
/// composes category paths) and the engine (<c>UIService</c>, when it splits them) reference
/// this single definition so the two never disagree.
/// </summary>
public static class MenuPath
{
    /// <summary>Segment separator for hierarchical menu category paths, e.g. "Experimental > Demos".</summary>
    public const string Delimiter = " > ";
}

[AttributeUsage(AttributeTargets.Class)]
public class OnDemandAttribute : Attribute
{
    public OnDemandAttribute() {}
}

[AttributeUsage(AttributeTargets.Class)]
public class AnimatedAttribute : Attribute
{
    public AnimatedAttribute() { }
}

/// <summary>
/// Re-evaluate this component while the pointer or camera moves, so it can follow
/// <see cref="IEvalServices.ViewportInput"/>. The viewport counterpart of [Animated].
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class PointerTrackingAttribute : Attribute
{
    public PointerTrackingAttribute() { }
}

