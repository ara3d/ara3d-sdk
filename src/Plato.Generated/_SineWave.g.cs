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
    public partial struct SineWave: IRealFunction, IOpenShape
    {
        // Fields
        [DataMember] public readonly Number Amplitude;
        [DataMember] public readonly Number Frequency;
        [DataMember] public readonly Number Phase;

        // With functions 
        [MethodImpl(AggressiveInlining)] public SineWave WithAmplitude(Number amplitude) => new SineWave(amplitude, Frequency, Phase);
        [MethodImpl(AggressiveInlining)] public SineWave WithFrequency(Number frequency) => new SineWave(Amplitude, frequency, Phase);
        [MethodImpl(AggressiveInlining)] public SineWave WithPhase(Number phase) => new SineWave(Amplitude, Frequency, phase);

        // Regular Constructor
        [MethodImpl(AggressiveInlining)] public SineWave(Number amplitude, Number frequency, Number phase) { Amplitude = amplitude; Frequency = frequency; Phase = phase; }

        // Static factory function
        [MethodImpl(AggressiveInlining)] public static SineWave Create(Number amplitude, Number frequency, Number phase) => new SineWave(amplitude, frequency, phase);

        // Static default implementation
        public static readonly SineWave Default = default;

        // Implicit converters to/from value-tuples and deconstructor
        [MethodImpl(AggressiveInlining)] public static implicit operator (Number, Number, Number)(SineWave self) => (self.Amplitude, self.Frequency, self.Phase);
        [MethodImpl(AggressiveInlining)] public static implicit operator SineWave((Number, Number, Number) value) => new SineWave(value.Item1, value.Item2, value.Item3);
        [MethodImpl(AggressiveInlining)] public void Deconstruct(out Number amplitude, out Number frequency, out Number phase) { amplitude = Amplitude; frequency = Frequency; phase = Phase;  }

        // Object virtual function overrides: Equals, GetHashCode, ToString
        [MethodImpl(AggressiveInlining)] public Boolean Equals(SineWave other) => Amplitude.Equals(other.Amplitude) && Frequency.Equals(other.Frequency) && Phase.Equals(other.Phase);
        [MethodImpl(AggressiveInlining)] public Boolean NotEquals(SineWave other) => !Amplitude.Equals(other.Amplitude) && Frequency.Equals(other.Frequency) && Phase.Equals(other.Phase);
        [MethodImpl(AggressiveInlining)] public override bool Equals(object obj) => obj is SineWave other ? Equals(other).Value : false;
        [MethodImpl(AggressiveInlining)] public override int GetHashCode() => Intrinsics.CombineHashCodes(Amplitude, Frequency, Phase);
        [MethodImpl(AggressiveInlining)] public override string ToString() => $"{{ \"Amplitude\" = {Amplitude}, \"Frequency\" = {Frequency}, \"Phase\" = {Phase} }}";

        // Explicit implementation of interfaces by forwarding properties to fields

        // Implemented interface functions
        [MethodImpl(AggressiveInlining)] public Number Eval(Number x) => x.SineWave(this.Amplitude, this.Frequency, this.Phase);
public Boolean Closed { [MethodImpl(AggressiveInlining)] get  => ((Boolean)false); } 

        // Unimplemented interface functions
    }
    // Extension methods for the type
    public static class SineWaveExtensions
    {
    }
}
