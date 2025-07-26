using System.Runtime.CompilerServices;
using Ara3D.Memory;

namespace Ara3D.IO.StepParser
{
    public unsafe struct StepToken
    {
        public byte* Begin;
        public byte* End;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StepToken(byte* begin, byte* end)
        {
            Begin = begin;
            End = end;
        }

        public StepTokenType Type
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GetTokenType(*Begin);
        }

        public ByteSlice Slice 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new (Begin, End);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
            => Slice.ToAsciiString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StepTokenType GetTokenType(byte b)
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
                case (byte)'+':
                case (byte)'-':
                    return StepTokenType.Number;

                case (byte)' ':
                case (byte)'\t':
                case (byte)'\n':
                case (byte)'\r':
                    return StepTokenType.Whitespace;

                case (byte)'\'':
                    return StepTokenType.SingleQuotedString;
                
                case (byte)'"':
                    return StepTokenType.DoubleQuotedString;

                case (byte)'.':
                    return StepTokenType.Symbol;

                case (byte)'#':
                    return StepTokenType.Id;

                case (byte)';':
                    return StepTokenType.EndOfLine;

                case (byte)'(':
                    return StepTokenType.BeginGroup;

                case (byte)'=':
                    return StepTokenType.Definition;

                case (byte)')':
                    return StepTokenType.EndGroup;

                case (byte)',':
                    return StepTokenType.Separator;

                case (byte)'$':
                    return StepTokenType.Unassigned;

                case (byte)'*':
                    return StepTokenType.Redeclared;

                case (byte)'/':
                    return StepTokenType.Comment;

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
                    return StepTokenType.Identifier;

                default:
                    return StepTokenType.Unknown;
            }
        }
    }
}