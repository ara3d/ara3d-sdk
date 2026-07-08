using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Ara3D.Memory;

namespace Ara3D.IO.StepParser;

/// <summary>
/// Technically this is more a parser because it is recursive. 
/// </summary>
public static unsafe class StepTokenizer
{
    public static readonly StepTokenType[] TokenLookup =
        StepTokenizerLookupHelpers.CreateTokenLookup();

    public static readonly bool[] IsNumberLookup =
        StepTokenizerLookupHelpers.CreateNumberLookup();

    public static readonly bool[] IsIdentLookup =
        StepTokenizerLookupHelpers.CreateIdentLookup();

    public static readonly bool[] IsDigitLookup =
        StepTokenizerLookupHelpers.CreateDigitLookup();

    public static readonly bool[] IsWhiteSpaceLookup =
        StepTokenizerLookupHelpers.CreateWhiteSpaceLookup();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ShouldStoreType(StepTokenType type)
    {
        switch (type)
        {
            case StepTokenType.Identifier:
            case StepTokenType.SingleQuotedString:
            case StepTokenType.DoubleQuotedString:
            case StepTokenType.Number:
            case StepTokenType.Symbol:
            case StepTokenType.Id:
            case StepTokenType.Unassigned:
            case StepTokenType.Redeclared:
                return true;

            case StepTokenType.BeginGroup:
            case StepTokenType.Definition:
            case StepTokenType.EndGroup:
            case StepTokenType.Semicolon:
            case StepTokenType.None:
            case StepTokenType.Whitespace:
            case StepTokenType.Separator:
            case StepTokenType.Comment:
            case StepTokenType.Unknown:
                return false;

            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static StepTokenType ParseToken(ref byte* cur, byte* end)
    {
        Debug.Assert(cur < end);
        switch (*cur++)
        {
            case (byte)'(':
                return StepTokenType.BeginGroup;

            case (byte)')':
                return StepTokenType.EndGroup;

            case (byte)'=':
                return StepTokenType.Definition;
                
            case (byte)';':
                return StepTokenType.Semicolon;

            case (byte)'$':
                return StepTokenType.Unassigned;

            case (byte)',':
                return StepTokenType.Separator;

            case (byte)'*':
                return StepTokenType.Redeclared;

            case (byte)' ':
            case (byte)'\t':
            case (byte)'\n':
            case (byte)'\r':
                while (cur < end && IsWhiteSpaceLookup[*cur]) cur++;
                return StepTokenType.Whitespace;

            case (byte)'a':
            case (byte)'b':
            case (byte)'c':
            case (byte)'d':
            case (byte)'e':
            case (byte)'f':
            case (byte)'g':
            case (byte)'h':
            case (byte)'i':
            case (byte)'j':
            case (byte)'k':
            case (byte)'l':
            case (byte)'m':
            case (byte)'n':
            case (byte)'o':
            case (byte)'p':
            case (byte)'q':
            case (byte)'r':
            case (byte)'s':
            case (byte)'t':
            case (byte)'u':
            case (byte)'v':
            case (byte)'w':
            case (byte)'x':
            case (byte)'y':
            case (byte)'z':
            case (byte)'A':
            case (byte)'B':
            case (byte)'C':
            case (byte)'D':
            case (byte)'E':
            case (byte)'F':
            case (byte)'G':
            case (byte)'H':
            case (byte)'I':
            case (byte)'J':
            case (byte)'K':
            case (byte)'L':
            case (byte)'M':
            case (byte)'N':
            case (byte)'O':
            case (byte)'P':
            case (byte)'Q':
            case (byte)'R':
            case (byte)'S':
            case (byte)'T':
            case (byte)'U':
            case (byte)'V':
            case (byte)'W':
            case (byte)'X':
            case (byte)'Y':
            case (byte)'Z':
            case (byte)'_':
                while (cur < end && IsIdentLookup[*cur]) cur++;
                return StepTokenType.Identifier;

            case (byte)'"':
            {
                byte q = (byte)'"';
                while (cur < end)
                {
                    if (*cur != q) { cur++; continue; }

                    // *cur is a quote
                    if (cur + 1 < end && cur[1] == q)
                    {
                        // Escaped quote: "" -> consume both and continue
                        cur += 2;
                        continue;
                    }

                    // Closing quote
                    cur++; // consume terminator
                    return StepTokenType.DoubleQuotedString;
                }

                // Unterminated string: decide how you want to handle this (error token, etc.)
                return StepTokenType.DoubleQuotedString;
            }

            case (byte)'\'':
            {
                byte q = (byte)'\'';
                while (cur < end)
                {
                    if (*cur != q) { cur++; continue; }

                    // *cur is a quote
                    if (cur + 1 < end && cur[1] == q)
                    {
                        // Escaped quote: '' -> consume both and continue
                        cur += 2;
                        continue;
                    }

                    // Closing quote
                    cur++; // consume terminator
                    return StepTokenType.SingleQuotedString;
                }

                return StepTokenType.SingleQuotedString;
            }

            case (byte)'0':
            case (byte)'1':
            case (byte)'2':
            case (byte)'3':
            case (byte)'4':
            case (byte)'5':
            case (byte)'6':
            case (byte)'7':
            case (byte)'8':
            case (byte)'9':
                while (cur < end && IsNumberLookup[*cur]) cur++;
                return StepTokenType.Number;

            case (byte)'.':
                while (cur < end && IsIdentLookup[*cur]) cur++;
                cur++; // Skip the closing '.'
                return StepTokenType.Symbol;

            case (byte)'#':
                while (cur < end && IsDigitLookup[*cur]) cur++;
                return StepTokenType.Id;

            case (byte)'+':
            case (byte)'-':
                if (cur < end && (IsDigitLookup[*cur] || *cur == (byte)'.'))
                {
                    while (cur < end && IsNumberLookup[*cur]) cur++;
                    return StepTokenType.Number;
                }
                return StepTokenType.Unknown;

            case (byte)'/':
                var prev = *cur++;
                while (cur < end && (prev != '*' || *cur != '/'))
                    prev = *cur++;
                cur++;
                return StepTokenType.Comment;

            default:
                return StepTokenType.Unknown;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhiteSpace(StepTokenType type)
    {
        return type == StepTokenType.Whitespace || type == StepTokenType.Comment;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AdvancePast(ref byte* cur, byte* end, StepTokenType type)
    {
        if (cur >= end) return false;
        var r = ParseToken(ref cur, end);
        while (IsWhiteSpace(r))
        {
            if (cur >= end) return false;
            r = ParseToken(ref cur, end);
        }
        return r == type;
    }

    public static bool AdvanceToData(ref byte* cur, byte* end)
    {
        while (cur < end)
        {
            var begin = cur;
            var r = ParseToken(ref cur, end);
            if (r == StepTokenType.Identifier)
            {
                var span = new ReadOnlySpan<byte>(begin, (int)(cur - begin));
                if (span.SequenceEqual("DATA"u8))
                {
                    Debug.Assert(*cur == ';');
                    cur++;
                    return true;
                }
            }
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AdvanceTo(ref byte* cur, byte* end, out StepToken token, StepTokenType type, UnmanagedList<StepToken> tokens)
    {
        while (cur < end)
        {
            var begin = cur;
            var r = ParseToken(ref cur, end);
            if (r == type)
            {
                token = new StepToken(begin, cur, tokens.Count);
                return true;
            }

            if (r != StepTokenType.None
                && r != StepTokenType.Comment
                && r != StepTokenType.Whitespace
                && r != StepTokenType.Unknown)
            {
                break;
            }
        }
        token = default;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ParseList(ref byte* cur, byte* end, UnmanagedList<StepToken> tokens)
    {
        var startTokenIndex = tokens.Count;
        var begin = cur - 1;

        Debug.Assert(*begin == '(');
        var token = StepToken.CreateListToken(begin, startTokenIndex);
        tokens.Add(token);

        Debug.Assert(tokens.Count == startTokenIndex + 1);

        while (cur < end)
        {
            var prev = cur;
            var type = ParseToken(ref cur, end);

            if (type == StepTokenType.EndGroup)
            {
                // This is the raw number of tokens (it includes nested lists)
                var numTokensInList = tokens.Count - startTokenIndex - 1;
                Debug.Assert(numTokensInList >= 0);
                
                // Patch the list token with correct number of tokens
                tokens[startTokenIndex].Length = numTokensInList;

                Debug.Assert(tokens[startTokenIndex].Length >= 0);

                return;
            }

            if (type == StepTokenType.BeginGroup)
                ParseList(ref cur, end, tokens);

            if (type == StepTokenType.Semicolon)
                throw new Exception("Unexpected end of definition");

            if (ShouldStoreType(type))
                tokens.Add(new StepToken(prev, cur, tokens.Count));
        }

        throw new Exception("Missing end of list");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AdvanceToAndTokenizeDefinition(ref byte* cur, byte* end, out int id, out StepToken nameToken, out StepToken attrToken, UnmanagedList<StepToken> tokens)
    {
        id = 0;
        nameToken = default;
        attrToken = default;

        if (!AdvanceTo(ref cur, end, out var idToken, StepTokenType.Id, tokens))
        {
            if (idToken.Match("ENDSEC"u8))
            {
                throw new Exception($"Expected `ENDSEC` but was {idToken}");
            }
            return false;
        }


        id = idToken.AsId();

        if (!AdvancePast(ref cur, end, StepTokenType.Definition))
        {
            return false;
        }

        AdvanceTo(ref cur, end, out nameToken, StepTokenType.Identifier, tokens);

        var n = tokens.Count;

        if (!AdvancePast(ref cur, end, StepTokenType.BeginGroup))
            throw new Exception("Expected the beginning of a group");

        ParseList(ref cur, end, tokens);

        if (!AdvancePast(ref cur, end, StepTokenType.Semicolon))
        {
            throw new Exception("Expected a semicolon");
        }

        cur++;

        attrToken = tokens[n];
        Debug.Assert(attrToken.IsList);
        Debug.Assert(attrToken.Length >= 0);

        return true;
    }
}