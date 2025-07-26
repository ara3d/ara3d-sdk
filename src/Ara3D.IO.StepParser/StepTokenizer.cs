using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Ara3D.Memory;

namespace Ara3D.IO.StepParser
{
    public static unsafe class StepTokenizer
    {
        public static readonly StepTokenType[] TokenLookup =
            CreateTokenLookup();

        public static readonly bool[] IsNumberLookup =
            CreateNumberLookup();

        public static readonly bool[] IsIdentLookup =
            CreateIdentLookup();

        public static StepTokenType[] CreateTokenLookup()
        {
            var r = new StepTokenType[256];
            for (var i = 0; i < 256; i++)
                r[i] = StepToken.GetTokenType((byte)i);
            return r;
        }

        public static bool[] CreateNumberLookup()
        {
            var r = new bool[256];
            for (var i = 0; i < 256; i++)
                r[i] = IsNumberChar((byte)i);
            return r;
        }

        public static bool[] CreateIdentLookup()
        {
            var r = new bool[256];
            for (var i = 0; i < 256; i++)
                r[i] = IsIdentOrDigitChar((byte)i);
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StepTokenType LookupToken(byte b)
            => TokenLookup[b];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumberChar(byte b)
        {
            switch (b)
            {
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
                case (byte)'E':
                case (byte)'e':
                case (byte)'+':
                case (byte)'-':
                case (byte)'.':
                    return true;
            }

            return false;
        }

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
                case StepTokenType.EndGroup:
                case StepTokenType.EndOfLine:
                case StepTokenType.Definition:
                    return true;

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
        public static bool IsWhiteSpace(byte b)
            => b == ' ' || b == '\t' || b == '\n' || b == '\r';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIdent(byte b)
            => b >= 'A' && b <= 'Z' || b >= 'a' && b <= 'z' || b == '_';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDigit(byte b)
            => b >= '0' && b <= '9';

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsIdentOrDigitChar(byte b)
            => IsIdent(b) || IsDigit(b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte* AdvancePast(byte* begin, byte* end, string s)
        {
            if (end - begin < s.Length)
                return null;
            foreach (var c in s)
                if (*begin++ != (byte)c)
                    return null;
            return begin;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StepToken ParseToken(byte* begin, byte* end)
        {
            var cur = begin;
            _ = InternalParseToken(ref cur, end);
            Debug.Assert(cur <= end);
            return new StepToken(begin, cur);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StepTokenType ParseToken(byte* begin, byte* end, out StepToken token)
        {
            var cur = begin;
            var r = InternalParseToken(ref cur, end);
            Debug.Assert(cur <= end);
            token = new StepToken(begin, cur);
            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EatWSpace(ref StepToken cur, byte* end)
        {
            while (cur.Type == StepTokenType.Comment  || cur.Type == StepTokenType.Whitespace)
            {
                if (!ParseNextToken(ref cur, end))
                    return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ParseNextToken(ref StepToken prev, byte* end)
        {
            var cur = prev.Slice.End;
            if (cur >= end) return false;
            prev = ParseToken(cur, end);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static StepTokenType InternalParseToken(ref byte* cur, byte* end)
        {
            Debug.Assert(cur < end);
            var type = TokenLookup[*cur++];

            switch (type)
            {
                case StepTokenType.Whitespace:
                    while (cur < end && IsWhiteSpace(*cur)) cur++;
                    break;

                case StepTokenType.Identifier:
                    while (cur < end && IsIdentLookup[*cur]) cur++;
                    break;

                case StepTokenType.DoubleQuotedString:
                    while (cur < end && *cur != (byte)'"') cur++;
                    cur++; // Skip the closing quote
                    break;

                case StepTokenType.SingleQuotedString:
                    while (cur < end && *cur != (byte)'\'') cur++;
                    cur++; // Skip the closing quote
                    break;

                case StepTokenType.Number:
                    while (cur < end && IsNumberLookup[*cur]) cur++;
                    break;

                case StepTokenType.Symbol:
                    while (cur < end && IsIdentLookup[*cur]) cur++;
                    cur++; // Skip the closing '.'
                    break;

                case StepTokenType.Id:
                    while (cur < end && IsDigit(*cur)) cur++;
                    break;

                case StepTokenType.Comment:
                    var prev = *cur++;
                    while (cur < end && (prev != '*' || *cur != '/'))
                        prev = *cur++;
                    cur++;
                    break;
            }

            return type;
        }
        
        public static bool InternalParseTokenEatWhiteSpace(ref byte* cur, byte* end)
        {
            while (cur < end)
            {
                var type = InternalParseToken(ref cur, end);
                if (type != StepTokenType.Whitespace && type != StepTokenType.Comment)
                    return true;
            }
            return false;
        }

        public static bool IsWhiteSpace(StepTokenType type)
        {
            return type == StepTokenType.Whitespace || type == StepTokenType.Comment;
        }

        public static bool AdvancePast(ref byte* cur, byte* end, StepTokenType type)
        {
            if (cur >= end) return false;
            var r = InternalParseToken(ref cur, end);
            while (IsWhiteSpace(r))
            {
                if (cur >= end) return false;
                r = InternalParseToken(ref cur, end);
            }
            return r == type;
        }

        public static bool AdvanceTo(ref byte* cur, byte* end, out StepToken token, StepTokenType type)
        {
            while (cur < end)
            {
                var begin = cur;
                var r = InternalParseToken(ref cur, end);
                if (r == type)
                {
                    token = new StepToken(begin, cur);
                    return true;
                }
            }
            token = default;
            return false;
        }

        public static bool AdvanceToDefinition(ref byte* cur, byte* end, out StepToken id)
        {
            id = default;
            while (cur < end)
            {
                if (!AdvanceTo(ref cur, end, out id, StepTokenType.Id))
                    return false;
                if (AdvancePast(ref cur, end, StepTokenType.Definition))
                    return true;
            }
            return false;
        }
    
        public static bool AdvanceToAndTokenizeDefinition(ref byte* cur, byte* end, out StepToken id, UnmanagedList<StepToken> tokens)
        {
            Debug.Assert(tokens.Count == 0);

            if (!AdvanceToDefinition(ref cur, end, out id))
                return false;
            
            while (cur < end)
            {
                var begin = cur;
                var type = InternalParseToken(ref cur, end);
                if (type == StepTokenType.EndOfLine)
                    return true;
                if (ShouldStoreType(type))
                    tokens.Add(new StepToken(begin, cur));
            }

            return false;
        }

        public static void Tokenize(byte* begin, byte* end, UnmanagedList<StepToken> tokens)
        {
            while (begin < end)
            {
                var token = ParseToken(begin, end);
                if (ShouldStoreType(token.Type))
                {
                    tokens.Add(token);
                }
                Debug.Assert(token.End > begin);
                Debug.Assert(token.Begin == begin);
                begin = token.End;
            }
        }
    }
}