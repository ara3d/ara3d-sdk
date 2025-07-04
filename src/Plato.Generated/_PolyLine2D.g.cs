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
    public partial struct PolyLine2D: IGeometry2D
    {
        // Fields
        [DataMember] public readonly IReadOnlyList<Point2D> Points;
        [DataMember] public readonly Boolean Closed;

        // With functions 
        [MethodImpl(AggressiveInlining)] public PolyLine2D WithPoints(IReadOnlyList<Point2D> points) => new PolyLine2D(points, Closed);
        [MethodImpl(AggressiveInlining)] public PolyLine2D WithClosed(Boolean closed) => new PolyLine2D(Points, closed);

        // Regular Constructor
        [MethodImpl(AggressiveInlining)] public PolyLine2D(IReadOnlyList<Point2D> points, Boolean closed) { Points = points; Closed = closed; }

        // Static factory function
        [MethodImpl(AggressiveInlining)] public static PolyLine2D Create(IReadOnlyList<Point2D> points, Boolean closed) => new PolyLine2D(points, closed);

        // Static default implementation
        public static readonly PolyLine2D Default = default;

        // Implicit converters to/from value-tuples and deconstructor
        [MethodImpl(AggressiveInlining)] public static implicit operator (IReadOnlyList<Point2D>, Boolean)(PolyLine2D self) => (self.Points, self.Closed);
        [MethodImpl(AggressiveInlining)] public static implicit operator PolyLine2D((IReadOnlyList<Point2D>, Boolean) value) => new PolyLine2D(value.Item1, value.Item2);
        [MethodImpl(AggressiveInlining)] public void Deconstruct(out IReadOnlyList<Point2D> points, out Boolean closed) { points = Points; closed = Closed;  }

        // Object virtual function overrides: Equals, GetHashCode, ToString
        [MethodImpl(AggressiveInlining)] public Boolean Equals(PolyLine2D other) => Points.Equals(other.Points) && Closed.Equals(other.Closed);
        [MethodImpl(AggressiveInlining)] public Boolean NotEquals(PolyLine2D other) => !Points.Equals(other.Points) && Closed.Equals(other.Closed);
        [MethodImpl(AggressiveInlining)] public override bool Equals(object obj) => obj is PolyLine2D other ? Equals(other).Value : false;
        [MethodImpl(AggressiveInlining)] public override int GetHashCode() => Intrinsics.CombineHashCodes(Points, Closed);
        [MethodImpl(AggressiveInlining)] public override string ToString() => $"{{ \"Points\" = {Points}, \"Closed\" = {Closed} }}";

        // Explicit implementation of interfaces by forwarding properties to fields

        // Implemented interface functions
        public PolyLine3D To3D { [MethodImpl(AggressiveInlining)] get  => (this.Points.Map((p)  => p.To3D), this.Closed); } 
public PolyLine3D PolyLine3D { [MethodImpl(AggressiveInlining)] get  => this.To3D; } 
[MethodImpl(AggressiveInlining)]  public static implicit operator PolyLine3D(PolyLine2D x) => x.PolyLine3D;

        // Unimplemented interface functions
    }
    // Extension methods for the type
    public static class PolyLine2DExtensions
    {
    }
}
