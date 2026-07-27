using System.Globalization;
using System.Text;

namespace Ara3D.Ifc.Tests;

/// <summary>Formats .NET values as ISO-10303-21 literals.</summary>
public static class IfcStepText
{
    /// <summary>
    /// Encodes a string literal: quotes and backslashes are doubled/escaped, and anything outside
    /// printable ASCII goes into a \X2\...\X0\ run of UTF-16 code units.
    /// </summary>
    public static string String(string value)
    {
        var sb = new StringBuilder(value.Length + 2);
        sb.Append('\'');
        var inExtended = false;
        foreach (var c in value)
        {
            if (c >= ' ' && c <= '~')
            {
                if (inExtended)
                {
                    sb.Append("\\X0\\");
                    inExtended = false;
                }
                if (c == '\'')
                    sb.Append("''");
                else if (c == '\\')
                    sb.Append("\\\\");
                else
                    sb.Append(c);
            }
            else
            {
                if (!inExtended)
                {
                    sb.Append("\\X2\\");
                    inExtended = true;
                }
                sb.Append(((int)c).ToString("X4", CultureInfo.InvariantCulture));
            }
        }
        if (inExtended)
            sb.Append("\\X0\\");
        return sb.Append('\'').ToString();
    }

    /// <summary>STEP reals must be distinguishable from integers, so a decimal point is always present.</summary>
    public static string Real(double value)
    {
        var s = value.ToString("R", CultureInfo.InvariantCulture);
        return s.Contains('.') || s.Contains('E') || s.Contains('e') ? s : s + ".";
    }

    public static string Integer(long value)
        => value.ToString(CultureInfo.InvariantCulture);

    public static string Boolean(bool value)
        => value ? ".T." : ".F.";

    public static string IdList(IReadOnlyList<int> ids)
    {
        var sb = new StringBuilder("(");
        for (var i = 0; i < ids.Count; i++)
        {
            if (i > 0)
                sb.Append(',');
            sb.Append('#').Append(ids[i]);
        }
        return sb.Append(')').ToString();
    }
}
