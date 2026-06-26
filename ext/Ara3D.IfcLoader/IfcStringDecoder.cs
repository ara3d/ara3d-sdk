using System.Text;

public static class IfcStringDecoder
{
    /// <summary>
    /// Decodes IFC / STEP escaped strings into normal .NET Unicode strings.
    ///
    /// Handles:
    ///   \\X\\hh\\              8-bit hex escape
    ///   \\X2\\hhhh...\\X0\\    UTF-16 hex escape, 4 hex digits per code unit
    ///   \\X4\\hhhhhhhh...\\X0\\ UTF-32 hex escape, 8 hex digits per code point
    ///   \\S\\c                 ISO-8859-1 high-half shorthand
    ///   \\\\                   literal backslash
    ///   \\''                   literal apostrophe
    ///
    /// Note: Input should be the IFC string content without the surrounding single quotes.
    /// </summary>
    public static string DecodeIfc(this string? value)
    {
        if (string.IsNullOrEmpty(value))
            return value ?? string.Empty;

        // Fast path: no STEP escape sequences.
        if (value.IndexOf('\\') < 0)
            return value;

        var sb = new StringBuilder(value.Length);

        for (var i = 0; i < value.Length;)
        {
            var c = value[i];

            if (c != '\\')
            {
                sb.Append(c);
                i++;
                continue;
            }

            // Try to decode an escape. If malformed, preserve the backslash.
            if (TryDecodeEscape(value, ref i, sb))
                continue;

            sb.Append('\\');
            i++;
        }

        return sb.ToString();
    }

    private static bool TryDecodeEscape(string s, ref int i, StringBuilder sb)
    {
        var n = s.Length;

        if (i + 1 >= n)
            return false;

        var next = s[i + 1];

        switch (next)
        {
            case '\\':
                sb.Append('\\');
                i += 2;
                return true;

            case '\'':
                sb.Append('\'');
                i += 2;
                return true;

            case 'S':
            case 's':
                return TryDecodeSlashS(s, ref i, sb);

            case 'X':
            case 'x':
                return TryDecodeSlashX(s, ref i, sb);

            default:
                return false;
        }
    }

    /// <summary>
    /// Decodes \S\c.
    /// In STEP, this represents a character in the upper half of ISO-8859-1.
    /// The decoded character is char(c + 128).
    /// </summary>
    private static bool TryDecodeSlashS(string s, ref int i, StringBuilder sb)
    {
        // Expected: \S\c
        if (i + 3 >= s.Length)
            return false;

        if (s[i + 2] != '\\')
            return false;

        var c = s[i + 3];
        sb.Append((char)(c + 128));

        i += 4;
        return true;
    }

    private static bool TryDecodeSlashX(string s, ref int i, StringBuilder sb)
    {
        // Possible:
        //   \X\hh\
        //   \X2\hhhh...\X0\
        //   \X4\hhhhhhhh...\X0\

        if (i + 2 >= s.Length)
            return false;

        var mode = s[i + 2];

        if (mode == '\\')
            return TryDecodeX8Bit(s, ref i, sb);

        if ((mode == '2' || mode == '4') && i + 3 < s.Length && s[i + 3] == '\\')
            return TryDecodeExtendedX(s, ref i, sb, mode == '2' ? 4 : 8);

        return false;
    }

    /// <summary>
    /// Decodes \X\hh\.
    /// </summary>
    private static bool TryDecodeX8Bit(string s, ref int i, StringBuilder sb)
    {
        // Expected: \X\hh\
        if (i + 5 >= s.Length)
            return false;

        var h1 = HexValue(s[i + 3]);
        var h2 = HexValue(s[i + 4]);

        if (h1 < 0 || h2 < 0)
            return false;

        if (s[i + 5] != '\\')
            return false;

        var code = (h1 << 4) | h2;
        sb.Append((char)code);

        i += 6;
        return true;
    }

    /// <summary>
    /// Decodes:
    ///   \X2\hhhh...\X0\
    ///   \X4\hhhhhhhh...\X0\
    /// </summary>
    private static bool TryDecodeExtendedX(string s, ref int i, StringBuilder sb, int hexDigitsPerChar)
    {
        var contentStart = i + 4;
        var end = FindX0Terminator(s, contentStart);

        if (end < 0)
            return false;

        var hexLength = end - contentStart;

        if (hexLength == 0 || hexLength % hexDigitsPerChar != 0)
            return false;

        for (var p = contentStart; p < end; p += hexDigitsPerChar)
        {
            var code = ParseHex(s, p, hexDigitsPerChar);

            if (code < 0)
                return false;

            if (hexDigitsPerChar == 4)
            {
                // X2 is UTF-16 code units.
                sb.Append((char)code);
            }
            else
            {
                // X4 is UTF-32 code points.
                if (code > 0x10FFFF)
                    return false;

                sb.Append(char.ConvertFromUtf32(code));
            }
        }

        i = end + 4; // Skip \X0\
        return true;
    }

    private static int FindX0Terminator(string s, int start)
    {
        for (var i = start; i + 3 < s.Length; i++)
        {
            if (s[i] == '\\' &&
                (s[i + 1] == 'X' || s[i + 1] == 'x') &&
                s[i + 2] == '0' &&
                s[i + 3] == '\\')
            {
                return i;
            }
        }

        return -1;
    }

    private static int ParseHex(string s, int start, int count)
    {
        var value = 0;

        for (var i = 0; i < count; i++)
        {
            var h = HexValue(s[start + i]);
            if (h < 0)
                return -1;

            value = (value << 4) | h;
        }

        return value;
    }

    private static int HexValue(char c)
    {
        if (c >= '0' && c <= '9')
            return c - '0';

        if (c >= 'A' && c <= 'F')
            return c - 'A' + 10;

        if (c >= 'a' && c <= 'f')
            return c - 'a' + 10;

        return -1;
    }
}