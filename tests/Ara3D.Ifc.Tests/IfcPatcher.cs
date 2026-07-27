using System.Text;

namespace Ara3D.Ifc.Tests;

/// <summary>
/// Raw byte edits on an IFC file: append entity lines at the end of the DATA section, or remove
/// entities by id. Everything not explicitly touched is copied verbatim, so adding then removing
/// the same entities reproduces the original file byte-for-byte.
/// </summary>
public static class IfcPatcher
{
    /// <summary>Inserts each line (already of the form "#id=TYPE(...);") just before ENDSEC.</summary>
    public static byte[] Append(IfcSourceFile file, IReadOnlyList<string> lines)
    {
        var insertAt = FindEndSec(file);
        var newline = DetectNewline(file.Bytes);
        var sb = new StringBuilder();
        for (var i = 0; i < lines.Count; i++)
            sb.Append(lines[i]).Append(newline);
        var inserted = Encoding.Latin1.GetBytes(sb.ToString());

        var r = new byte[file.Bytes.Length + inserted.Length];
        Array.Copy(file.Bytes, 0, r, 0, insertAt);
        Array.Copy(inserted, 0, r, insertAt, inserted.Length);
        Array.Copy(file.Bytes, insertAt, r, insertAt + inserted.Length, file.Bytes.Length - insertAt);
        return r;
    }

    /// <summary>Removes each entity's bytes plus its trailing whitespace through one line terminator.</summary>
    public static byte[] Remove(IfcSourceFile file, IReadOnlyList<int> ids)
    {
        var cuts = new List<(int Begin, int End)>(ids.Count);
        foreach (var id in ids)
        {
            var span = file.GetSpan(id) ?? throw new ArgumentException($"No entity #{id} to remove");
            cuts.Add((span.Begin, SkipTrailingTrivia(file.Bytes, span.End)));
        }
        cuts.Sort((a, b) => a.Begin.CompareTo(b.Begin));

        using var ms = new MemoryStream(file.Bytes.Length);
        var pos = 0;
        foreach (var (begin, end) in cuts)
        {
            if (begin < pos)
                throw new ArgumentException("Overlapping entity ranges");
            ms.Write(file.Bytes, pos, begin - pos);
            pos = end;
        }
        ms.Write(file.Bytes, pos, file.Bytes.Length - pos);
        return ms.ToArray();
    }

    /// <summary>Byte offset of the "ENDSEC" keyword that closes the DATA section (line start, scanning from the last entity).</summary>
    private static int FindEndSec(IfcSourceFile file)
    {
        var start = file.Spans.Count > 0 ? file.Spans[^1].End : 0;
        var bytes = file.Bytes;
        for (var i = start; i < bytes.Length; i++)
        {
            if (bytes[i] != (byte)'E')
                continue;
            if (i + 6 <= bytes.Length && bytes[i + 1] == 'N' && bytes[i + 2] == 'D' &&
                bytes[i + 3] == 'S' && bytes[i + 4] == 'E' && bytes[i + 5] == 'C')
                return i;
        }
        throw new FormatException("No ENDSEC after last entity");
    }

    private static int SkipTrailingTrivia(byte[] bytes, int pos)
    {
        while (pos < bytes.Length && (bytes[pos] == ' ' || bytes[pos] == '\t' || bytes[pos] == '\r'))
            pos++;
        if (pos < bytes.Length && bytes[pos] == '\n')
            pos++;
        return pos;
    }

    private static string DetectNewline(byte[] bytes)
        => Array.IndexOf(bytes, (byte)'\r') >= 0 ? "\r\n" : "\n";
}
