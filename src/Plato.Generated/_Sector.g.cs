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
    public partial struct Sector: IClosedShape2D
    {
        // Fields
        [DataMember] public readonly Arc Arc;

        // With functions 
        [MethodImpl(AggressiveInlining)] public Sector WithArc(Arc arc) => new Sector(arc);

        // Regular Constructor
        [MethodImpl(AggressiveInlining)] public Sector(Arc arc) { Arc = arc; }

        // Static factory function
        [MethodImpl(AggressiveInlining)] public static Sector Create(Arc arc) => new Sector(arc);

        // Static default implementation
        public static readonly Sector Default = default;

        // Implicit converters to/from single field
        [MethodImpl(AggressiveInlining)] public static implicit operator Arc(Sector self) => self.Arc;
        [MethodImpl(AggressiveInlining)] public static implicit operator Sector(Arc value) => new Sector(value);

        // Object virtual function overrides: Equals, GetHashCode, ToString
        [MethodImpl(AggressiveInlining)] public Boolean Equals(Sector other) => Arc.Equals(other.Arc);
        [MethodImpl(AggressiveInlining)] public Boolean NotEquals(Sector other) => !Arc.Equals(other.Arc);
        [MethodImpl(AggressiveInlining)] public override bool Equals(object obj) => obj is Sector other ? Equals(other).Value : false;
        [MethodImpl(AggressiveInlining)] public override int GetHashCode() => Intrinsics.CombineHashCodes(Arc);
        [MethodImpl(AggressiveInlining)] public override string ToString() => $"{{ \"Arc\" = {Arc} }}";

        // Explicit implementation of interfaces by forwarding properties to fields

        // Implemented interface functions
        public Boolean Closed { [MethodImpl(AggressiveInlining)] get  => ((Boolean)true); } 

        // Unimplemented interface functions
    }
    // Extension methods for the type
    public static class SectorExtensions
    {
    }
}
