using Ara3D.IO.StepParser;

namespace Ara3D.Ifc.Tests;

/// <summary>
/// The exact byte range of one entity instance inside a parsed STEP file, from its '#' through
/// its terminating ';'. Computed from the tokens <see cref="StepDocument"/> already produced, so
/// no re-tokenization happens; the range lets edits splice raw bytes and keep everything else
/// byte-identical.
/// </summary>
public readonly record struct IfcEntitySpan(int Id, string TypeName, int Begin, int End)
{
    public int Length
        => End - Begin;
}

public static unsafe class IfcEntitySpans
{
    public static IReadOnlyList<IfcEntitySpan> Compute(StepDocument doc)
    {
        var spans = new List<IfcEntitySpan>(doc.Definitions.Count);
        foreach (var def in doc.Definitions)
        {
            var begin = ScanBackToHash(doc, def);
            var end = ScanForwardToTerminator(doc, def);
            spans.Add(new IfcEntitySpan(def.Id, def.NameToken.ToString(), begin, end));
        }
        return spans;
    }

    /// <summary>From the type-name token, walk left over '=' and the id digits to the '#'.</summary>
    private static int ScanBackToHash(StepDocument doc, StepDefinition def)
    {
        var p = def.NameToken.Begin - 1;
        while (p > doc.DataStart && IsWhiteSpace(*p))
            p--;
        if (*p != (byte)'=')
            throw new FormatException($"Expected '=' before type name of #{def.Id}");
        p--;
        while (p > doc.DataStart && IsWhiteSpace(*p))
            p--;
        while (p > doc.DataStart && *p >= (byte)'0' && *p <= (byte)'9')
            p--;
        if (*p != (byte)'#')
            throw new FormatException($"Expected '#' at start of #{def.Id}");
        return (int)(p - doc.DataStart);
    }

    /// <summary>
    /// From the last attribute token, walk right over closing parens and whitespace to the ';'.
    /// List tokens store a token count rather than a byte length, so only their opening '(' is a
    /// usable position; any nested content token supersedes it.
    /// </summary>
    private static int ScanForwardToTerminator(StepDocument doc, StepDefinition def)
    {
        var attr = def.AttributesToken;
        var last = attr.Begin + 1;
        for (var i = attr.Index + 1; i <= attr.GetListEndIndex(); i++)
        {
            var t = doc.Tokens[i];
            var end = t.IsList ? t.Begin + 1 : t.End;
            if (end > last)
                last = end;
        }

        var p = last;
        while (p < doc.DataEnd && *p != (byte)';')
        {
            if (!IsWhiteSpace(*p) && *p != (byte)')')
                throw new FormatException($"Unexpected byte '{(char)*p}' before terminator of #{def.Id}");
            p++;
        }
        if (p >= doc.DataEnd)
            throw new FormatException($"Missing ';' after #{def.Id}");
        return (int)(p - doc.DataStart) + 1;
    }

    private static bool IsWhiteSpace(byte b)
        => b == ' ' || b == '\t' || b == '\r' || b == '\n';
}
