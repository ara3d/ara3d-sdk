namespace Ara3D.Studio.API;

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

