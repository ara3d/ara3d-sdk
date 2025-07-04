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
    public partial struct FigureEightKnot: IAngularCurve3D, IClosedShape
    {
        // Fields

        // With functions 

        // Regular Constructor

        // Static factory function
        [MethodImpl(AggressiveInlining)] public static FigureEightKnot Create() => new FigureEightKnot();

        // Static default implementation
        public static readonly FigureEightKnot Default = default;

        // Object virtual function overrides: Equals, GetHashCode, ToString
        [MethodImpl(AggressiveInlining)] public override bool Equals(object obj) => obj is FigureEightKnot;
        [MethodImpl(AggressiveInlining)] public Boolean Equals(FigureEightKnot other) => true;
        [MethodImpl(AggressiveInlining)] public Boolean NotEquals(FigureEightKnot other) => false;
        [MethodImpl(AggressiveInlining)] public static Boolean operator==(FigureEightKnot a, FigureEightKnot b) => true;
        [MethodImpl(AggressiveInlining)] public static Boolean operator!=(FigureEightKnot a, FigureEightKnot b) => false;
        [MethodImpl(AggressiveInlining)] public override int GetHashCode() => Intrinsics.CombineHashCodes();
        [MethodImpl(AggressiveInlining)] public override string ToString() => $"{{  }}";

        // Explicit implementation of interfaces by forwarding properties to fields

        // Implemented interface functions
        [MethodImpl(AggressiveInlining)] public Point3D Eval(Angle t) => t.FigureEightKnot;
[MethodImpl(AggressiveInlining)] public Point3D Eval(Number t) => this.Eval(t.Turns);
[MethodImpl(AggressiveInlining)] public IReadOnlyList<Point3D> Sample(Integer numPoints){
            var _var37 = this;
            return numPoints.LinearSpace.Map((x)  => _var37.Eval(x));
        }

[MethodImpl(AggressiveInlining)] public PolyLine3D ToPolyLine3D(Integer numPoints) => (this.Sample(numPoints), this.Closed);
public Boolean Closed { [MethodImpl(AggressiveInlining)] get  => ((Boolean)true); } 

        // Unimplemented interface functions
    }
    // Extension methods for the type
    public static class FigureEightKnotExtensions
    {
    }
}
