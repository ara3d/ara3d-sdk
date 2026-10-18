using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Ara3D.Memory;

namespace Ara3D.IO.StepParser;

public enum StepReadResult : byte
{
    Definition,
    EndOfSection,
    Fault,
}

/// <summary>Where reading stopped and what was due there. <see cref="DefinitionId"/> is 0 and
/// <see cref="DefinitionBegin"/> null when the fault is not inside an instance.</summary>
public readonly unsafe struct StepFault
{
    public readonly byte* At;
    public readonly string Expected;
    public readonly int DefinitionId;
    public readonly byte* DefinitionBegin;

    public StepFault(byte* at, string expected, int definitionId = 0, byte* definitionBegin = null)
    {
        At = at;
        Expected = expected;
        DefinitionId = definitionId;
        DefinitionBegin = definitionBegin;
    }
}

public static unsafe partial class StepTokenizer
{
    /// <summary>Reads one instance of a DATA section, or the section's closing <c>ENDSEC;</c>. On a fault the
    /// tokens of the unfinished instance stay in <paramref name="tokens"/>; the caller trims them.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StepReadResult ReadDefinition(ref byte* cur, byte* end, UnmanagedList<StepToken> tokens,
        out StepDefinition definition, out StepFault fault)
    {
        definition = default;
        fault = default;

        var begin = cur;
        var type = AdvancePastWhiteSpace(ref cur, end, ref begin);
        while (type == StepTokenType.Unknown)
            type = AdvancePastWhiteSpace(ref cur, end, ref begin);

        if (type == StepTokenType.Identifier && Span(begin, cur).SequenceEqual("ENDSEC"u8))
            return Expect(ref cur, end, StepTokenType.Semicolon, out var afterEndSec)
                ? StepReadResult.EndOfSection
                : Fail(out fault, afterEndSec, "';' after ENDSEC");

        if (type != StepTokenType.Id || !int.TryParse(Span(begin + 1, cur), out var id))
            return Fail(out fault, type == StepTokenType.None ? cur : begin, "an instance '#n=' or ENDSEC closing the DATA section");

        if (!Expect(ref cur, end, StepTokenType.Definition, out var at))
            return Fail(out fault, at, "'=' after the instance id", id, begin);

        // A simple instance is NAME(attributes); a complex instance is (A(attributes)B(attributes)...)
        // and keeps an empty name token, its list holding each partial entity's name and attribute list.
        var nameToken = default(StepToken);
        var nameBegin = cur;
        var nameType = AdvancePastWhiteSpace(ref cur, end, ref nameBegin);
        if (nameType == StepTokenType.Identifier)
        {
            nameToken = new StepToken(nameBegin, cur, tokens.Count);
            if (!Expect(ref cur, end, StepTokenType.BeginGroup, out at))
                return Fail(out fault, at, "'(' after the entity name", id, begin);
        }
        else if (nameType != StepTokenType.BeginGroup)
        {
            return Fail(out fault, nameType == StepTokenType.None ? cur : nameBegin, "an entity name or '(' opening a complex instance", id, begin);
        }

        var listIndex = tokens.Count;
        if (!TryParseList(ref cur, end, tokens, out at))
            return Fail(out fault, at, "')' closing the list", id, begin);

        if (!Expect(ref cur, end, StepTokenType.Semicolon, out at))
            return Fail(out fault, at, "';' ending the instance", id, begin);

        definition = new StepDefinition(id, nameToken, tokens[listIndex]);
        return StepReadResult.Definition;
    }

    /// <summary>Skips to the <c>DATA</c> keyword, as the header's sections allow, and past its optional parameter
    /// list and its ';'. Returns false at the end of the data or at a malformed keyword, with
    /// <paramref name="faultAt"/> where it stopped.</summary>
    public static bool ReadDataStart(ref byte* cur, byte* end, UnmanagedList<StepToken> scratch, out byte* faultAt)
    {
        while (cur < end)
        {
            var begin = cur;
            if (ParseToken(ref cur, end) != StepTokenType.Identifier || !Span(begin, cur).SequenceEqual("DATA"u8))
                continue;

            var count = scratch.Count;
            faultAt = cur;
            var type = AdvancePastWhiteSpace(ref cur, end, ref faultAt);
            if (type == StepTokenType.BeginGroup && TryParseList(ref cur, end, scratch, out _))
                type = AdvancePastWhiteSpace(ref cur, end, ref faultAt);
            scratch.SetCount(count);
            if (type == StepTokenType.Semicolon)
                return true;
            if (type == StepTokenType.None)
                faultAt = cur;
            return false;
        }

        faultAt = cur;
        return false;
    }

    /// <summary>Reads a list whose '(' was just consumed, appending its tokens. Returns false where a ';' or the end
    /// of the data comes before the closing ')', with <paramref name="faultAt"/> there.</summary>
    public static bool TryParseList(ref byte* cur, byte* end, UnmanagedList<StepToken> tokens, out byte* faultAt)
    {
        var startTokenIndex = tokens.Count;
        Debug.Assert(*(cur - 1) == '(');
        tokens.Add(StepToken.CreateListToken(cur - 1, startTokenIndex));

        while (cur < end)
        {
            var prev = cur;
            var type = ParseToken(ref cur, end);

            if (type == StepTokenType.EndGroup)
            {
                // The raw number of tokens, nested lists included
                tokens[startTokenIndex].Length = tokens.Count - startTokenIndex - 1;
                faultAt = null;
                return true;
            }

            if (type == StepTokenType.BeginGroup && !TryParseList(ref cur, end, tokens, out faultAt))
                return false;

            if (type == StepTokenType.Semicolon)
            {
                faultAt = prev;
                return false;
            }

            if (ShouldStoreType(type))
                tokens.Add(new StepToken(prev, cur, tokens.Count));
        }

        faultAt = cur;
        return false;
    }

    /// <summary>Skips white space and comments and reads one token; <paramref name="at"/> is its first byte, or
    /// where the data ended.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Expect(ref byte* cur, byte* end, StepTokenType type, out byte* at)
    {
        at = cur;
        var found = AdvancePastWhiteSpace(ref cur, end, ref at);
        if (found == StepTokenType.None)
            at = cur;
        return found == type;
    }

    /// <summary>The offset of <paramref name="at"/> from <paramref name="origin"/>, clamped to the end of the data,
    /// which a token cut short by it can overshoot.</summary>
    public static long Offset(byte* at, byte* origin, byte* end)
        => (at < end ? at : end) - origin;

    public static ReadOnlySpan<byte> Span(byte* begin, byte* end)
        => new(begin, (int)(end - begin));

    private static StepReadResult Fail(out StepFault fault, byte* at, string expected, int definitionId = 0, byte* definitionBegin = null)
    {
        fault = new StepFault(at, expected, definitionId, definitionBegin);
        return StepReadResult.Fault;
    }
}
