using System.Security.Cryptography;
using System.Text;

namespace Ara3D.Ifc.Tests;

/// <summary>
/// Conversion between a 128-bit GUID and the 22-character base-64 form IFC uses for GlobalId
/// (ISO-10303 alphabet: digits, letters, '_', '$'). The first field is 8 bits written as two
/// characters, so a valid IfcGuid always starts with one of '0'..'3'.
/// </summary>
public static class IfcGuid
{
    public const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_$";
    public const int Length = 22;

    public static string ToIfcGuid(this Guid guid)
    {
        var b = guid.ToByteArray();
        var d1 = (uint)(b[0] | (b[1] << 8) | (b[2] << 16) | (b[3] << 24));
        var d2 = (uint)(b[4] | (b[5] << 8));
        var d3 = (uint)(b[6] | (b[7] << 8));

        var num = new uint[6];
        num[0] = d1 / 16777216;
        num[1] = d1 % 16777216;
        num[2] = d2 * 256 + d3 / 256;
        num[3] = (d3 % 256) * 65536 + (uint)(b[8] * 256) + b[9];
        num[4] = (uint)(b[10] * 65536 + b[11] * 256 + b[12]);
        num[5] = (uint)(b[13] * 65536 + b[14] * 256 + b[15]);

        var chars = new char[Length];
        var pos = 0;
        for (var i = 0; i < 6; i++)
            pos = WriteBase64(num[i], i == 0 ? 2 : 4, chars, pos);
        return new string(chars);
    }

    public static Guid FromIfcGuid(string text)
    {
        if (text == null)
            throw new ArgumentNullException(nameof(text));
        if (text.Length != Length)
            throw new ArgumentException($"IfcGuid must be {Length} characters, got {text.Length}", nameof(text));

        var num = new uint[6];
        var pos = 0;
        for (var i = 0; i < 6; i++)
            num[i] = ReadBase64(text, i == 0 ? 2 : 4, ref pos);

        var d1 = num[0] * 16777216 + num[1];
        var d2 = (ushort)(num[2] / 256);
        var d3 = (ushort)((num[2] % 256) * 256 + num[3] / 65536);
        return new Guid((int)d1, (short)d2, (short)d3,
            (byte)((num[3] / 256) % 256), (byte)(num[3] % 256),
            (byte)(num[4] / 65536), (byte)((num[4] / 256) % 256), (byte)(num[4] % 256),
            (byte)(num[5] / 65536), (byte)((num[5] / 256) % 256), (byte)(num[5] % 256));
    }

    public static bool IsValid(string text)
    {
        if (text == null || text.Length != Length || text[0] > '3' || text[0] < '0')
            return false;
        for (var i = 0; i < text.Length; i++)
            if (Alphabet.IndexOf(text[i]) < 0)
                return false;
        return true;
    }

    /// <summary>
    /// A GUID derived from <paramref name="key"/> by hashing, so that regenerating the same
    /// content twice yields the same GlobalId and output files stay comparable between runs.
    /// </summary>
    public static Guid Deterministic(string key)
        => new(MD5.HashData(Encoding.UTF8.GetBytes(key)));

    private static int WriteBase64(uint value, int digits, char[] chars, int pos)
    {
        for (var i = digits - 1; i >= 0; i--)
        {
            chars[pos + i] = Alphabet[(int)(value % 64)];
            value /= 64;
        }
        return pos + digits;
    }

    private static uint ReadBase64(string text, int digits, ref int pos)
    {
        var value = 0u;
        for (var i = 0; i < digits; i++)
        {
            var d = Alphabet.IndexOf(text[pos + i]);
            if (d < 0)
                throw new ArgumentException($"Invalid IfcGuid character '{text[pos + i]}'", nameof(text));
            value = value * 64 + (uint)d;
        }
        pos += digits;
        return value;
    }
}
