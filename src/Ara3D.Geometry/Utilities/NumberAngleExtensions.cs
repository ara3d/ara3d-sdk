using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace Ara3D.Geometry
{
    /// <summary>
    /// Handwritten angle-conversion extensions for <see cref="Number"/> receivers.
    /// The generated <c>Degrees(this float)</c> extension does not apply to
    /// <see cref="Number"/> receivers because C# extension methods are not
    /// resolved through user-defined implicit conversions on the receiver.
    /// This method was previously a manual addition to the generated
    /// Extensions.g.cs; it lives here so regeneration cannot lose it
    /// (see tools\regen-plato.ps1 and docs\plato-roadmap.md Phase 0.3).
    /// </summary>
    public static class NumberAngleExtensions
    {
        [MethodImpl(AggressiveInlining)]
        public static Angle Degrees(this Number n) => n.Value.Degrees();
    }
}
