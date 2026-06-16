using Ara3D.Memory;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ara3D.IO.StepParser;

public unsafe struct StepToken 
{
    public readonly byte* Begin;

    // For most tokens this is the number of bytes, but for a list, this is the number of tokens 
    public int Length;

    // This is the index of list in the token list.
    public int Index; 

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StepToken(byte* begin, byte* end, int index)
        : this(begin, (int)(end - begin), index)
    { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StepToken(byte* begin, int length, int index)
    {
        Begin = begin;
        Length = length;
        Index = index;
        Debug.Assert(sizeof(StepToken) == 16);
        Debug.Assert(Marshal.SizeOf<StepToken>() == 16);
    }

    public readonly byte* End
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Begin + Length;
    }

    public readonly byte Last
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Begin[Length-1];
    }

    public readonly StepTokenType Type
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => StepTokenizer.TokenLookup[*Begin];
    }

    public readonly ByteSlice Slice 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new (Begin, End);
    }

    public readonly ReadOnlySpan<byte> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(Begin, (int)(End - Begin));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override string ToString()
        => Slice.ToAsciiString();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly double AsNumber()
        => double.Parse(Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly string AsString()
        => new ByteSlice(Begin + 1, End - 1).ToAsciiString();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int AsId()
        => ParseId();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int ParseId()
        => int.Parse(Span.Slice(1));

    public bool IsEntity
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Type == StepTokenType.Identifier; 
    }

    public readonly bool IsId
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => *Begin == (byte)'#';
    }

    public readonly bool IsList
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => *Begin == (byte)'(';
    }

    public readonly bool IsUnassignedOrRedeclared
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => IsUnassigned || IsRedeclared;
    }

    public readonly bool IsUnassigned
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => *Begin == (byte)'$';
    }

    public readonly bool IsRedeclared
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => *Begin == (byte)'*';
    }

    public readonly bool IsString
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => *Begin == (byte)'\'' || *Begin == (byte)'"';
    }

    public readonly bool IsNumber
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Type == StepTokenType.Number;
    }

    public readonly bool IsSymbol
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => *Begin == '.';
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(StepToken other)
        => Begin == other.Begin && Length == other.Length && Index == other.Index;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StepToken CreateListToken(byte* ptr, int index)
    {
        Debug.Assert(*ptr == '(');
        Debug.Assert(index >= 0);
        var r = new StepToken(ptr, -1, index);
        Debug.Assert(r.IsList);
        return r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Match(ReadOnlySpan<byte> bytes)
        => bytes.SequenceEqual(Span);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int GetListEndIndex()
        => Index + Length;
}