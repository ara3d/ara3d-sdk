// Autogenerated file: DO NOT EDIT
// Created on 2025-06-07 6:14:52 PM

using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
using Ara3D.Collections;

namespace Ara3D.Geometry
{
    [DataContract, StructLayout(LayoutKind.Sequential, Pack=1)]
    public partial struct CycloidOfCeva: IPolarCurve, IOpenShape
    {
        // Fields

        // With functions 

        // Regular Constructor

        // Static factory function
        [MethodImpl(AggressiveInlining)] public static CycloidOfCeva Create() => new CycloidOfCeva();

        // Static default implementation
        public static readonly CycloidOfCeva Default = default;

        // Object virtual function overrides: Equals, GetHashCode, ToString
        [MethodImpl(AggressiveInlining)] public override bool Equals(object obj) => obj is CycloidOfCeva;
        [MethodImpl(AggressiveInlining)] public Boolean Equals(CycloidOfCeva other) => true;
        [MethodImpl(AggressiveInlining)] public Boolean NotEquals(CycloidOfCeva other) => false;
        [MethodImpl(AggressiveInlining)] public static Boolean operator==(CycloidOfCeva a, CycloidOfCeva b) => true;
        [MethodImpl(AggressiveInlining)] public static Boolean operator!=(CycloidOfCeva a, CycloidOfCeva b) => false;
        [MethodImpl(AggressiveInlining)] public override int GetHashCode() => Intrinsics.CombineHashCodes();
        [MethodImpl(AggressiveInlining)] public override string ToString() => $"{{  }}";

        // Explicit implementation of interfaces by forwarding properties to fields

        // Implemented interface functions
        [MethodImpl(AggressiveInlining)] public Number GetRadius(Angle t) => t.CycloidOfCeva;
[MethodImpl(AggressiveInlining)] public PolarCoordinate EvalPolar(Angle t) => (this.GetRadius(t), t);
[MethodImpl(AggressiveInlining)] public Point2D Eval(Angle t) => this.EvalPolar(t);
[MethodImpl(AggressiveInlining)] public Point2D Eval(Number t) => this.Eval(t.Turns);
[MethodImpl(AggressiveInlining)] public IReadOnlyList<Point2D> Sample(Integer numPoints){
            var _var22 = this;
            return numPoints.LinearSpace.Map((x)  => _var22.Eval(x));
        }

[MethodImpl(AggressiveInlining)] public PolyLine2D ToPolyLine2D(Integer numPoints) => (this.Sample(numPoints), this.Closed);
public Boolean Closed { [MethodImpl(AggressiveInlining)] get  => ((Boolean)false); } 

        // Unimplemented interface functions
    }
    // Extension methods for the type
    public static class CycloidOfCevaExtensions
    {
    }
}
