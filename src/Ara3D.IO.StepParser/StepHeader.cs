using System;
using System.Collections.Generic;
using System.Linq;
using Ara3D.Memory;

namespace Ara3D.IO.StepParser;

/// <summary>The header section's FILE_DESCRIPTION, FILE_NAME and FILE_SCHEMA fields as written: each string is the
/// text between its quotes with every escape (<c>''</c>, <c>\X2\...\X0\</c>) left in place, an unset field (<c>$</c>)
/// or an absent entity is empty. <see cref="FileDescription"/>, <see cref="FileName"/> and <see cref="FileSchema"/>
/// are the older summaries, with <c>''</c> unescaped and the schema shortened to IFC2X3, IFC4 or IFC4X3.</summary>
public unsafe class StepHeader
{
    public IReadOnlyList<string> Description { get; init; } = [];
    public string ImplementationLevel { get; init; } = "";
    public string Name { get; init; } = "";
    public string TimeStamp { get; init; } = "";
    public IReadOnlyList<string> Author { get; init; } = [];
    public IReadOnlyList<string> Organization { get; init; } = [];
    public string PreprocessorVersion { get; init; } = "";
    public string OriginatingSystem { get; init; } = "";
    public string Authorization { get; init; } = "";
    public IReadOnlyList<string> SchemaIdentifiers { get; init; } = [];

    /// <summary>Every string of FILE_DESCRIPTION joined with " | ".</summary>
    public string FileDescription { get; set; } = "";

    public string FileName { get; set; } = "";

    public string FileSchema { get; set; } = "";

    /// <summary>Throws on the first error <see cref="Parse(ref byte*, byte*, byte*, List{StepDiagnostic})"/> reports.</summary>
    public static StepHeader Parse(ref byte* ptr, byte* dataEnd)
    {
        var diagnostics = new List<StepDiagnostic>();
        var header = Parse(ref ptr, dataEnd, ptr, diagnostics);
        var error = diagnostics.FirstOrDefault(d => d.Severity == StepSeverity.Error);
        if (error.Expected != null)
            throw new Exception(error.ToString());
        return header;
    }

    /// <summary>Reads from the start of the file through the header's ENDSEC, leaving <paramref name="cur"/> after
    /// it. A fault adds an error to <paramref name="diagnostics"/>, with offsets from <paramref name="origin"/>, and
    /// returns the fields read before it; a missing entity adds a warning.</summary>
    public static StepHeader Parse(ref byte* cur, byte* end, byte* origin, List<StepDiagnostic> diagnostics)
    {
        using var tokens = new UnmanagedList<StepToken>();
        var entities = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        var error = ReadSection(ref cur, end, tokens, entities, out var fault);
        if (error != null)
            diagnostics.Add(new StepDiagnostic(StepSeverity.Error, error.Value, StepTokenizer.Offset(fault.At, origin, end), fault.Expected));
        else
            foreach (var name in RequiredEntities.Where(name => !entities.ContainsKey(name)))
                diagnostics.Add(new StepDiagnostic(StepSeverity.Warning, StepDiagnosticCode.HeaderIncomplete, cur - origin, $"{name} in the header"));

        var description = Attributes(tokens, entities, "FILE_DESCRIPTION");
        var fileName = Attributes(tokens, entities, "FILE_NAME");
        var schema = Attributes(tokens, entities, "FILE_SCHEMA");
        var schemaIdentifiers = Strings(tokens, schema, 0);
        return new StepHeader
        {
            Description = Strings(tokens, description, 0),
            ImplementationLevel = String(description, 1),
            Name = String(fileName, 0),
            TimeStamp = String(fileName, 1),
            Author = Strings(tokens, fileName, 2),
            Organization = Strings(tokens, fileName, 3),
            PreprocessorVersion = String(fileName, 4),
            OriginatingSystem = String(fileName, 5),
            Authorization = String(fileName, 6),
            SchemaIdentifiers = schemaIdentifiers,
            FileDescription = string.Join(" | ", AllStrings(tokens, description).Select(Unescape)),
            FileName = Unescape(String(fileName, 0)),
            FileSchema = NormalizeIfcSchema(schemaIdentifiers.Count == 0 ? "" : Unescape(schemaIdentifiers[0])),
        };
    }

    public static readonly IReadOnlyList<string> RequiredEntities = ["FILE_DESCRIPTION", "FILE_NAME", "FILE_SCHEMA"];

    private static StepDiagnosticCode? ReadSection(ref byte* cur, byte* end, UnmanagedList<StepToken> tokens,
        Dictionary<string, int> entities, out StepFault fault)
    {
        fault = default;
        if (end - cur >= 3 && cur[0] == 0xEF && cur[1] == 0xBB && cur[2] == 0xBF)
            cur += 3;

        var begin = cur;
        StepTokenizer.AdvancePastWhiteSpace(ref cur, end, ref begin);
        if (end - begin < 12 || !StepTokenizer.Span(begin, begin + 12).SequenceEqual("ISO-10303-21"u8))
            return Fail(out fault, begin, "ISO-10303-21; opening the file", StepDiagnosticCode.NotStepFile);
        cur = begin + 12;
        if (!StepTokenizer.Expect(ref cur, end, StepTokenType.Semicolon, out var at))
            return Fail(out fault, at, "';' after ISO-10303-21", StepDiagnosticCode.NotStepFile);
        if (!ExpectKeyword(ref cur, end, "HEADER"u8, out at) || !StepTokenizer.Expect(ref cur, end, StepTokenType.Semicolon, out at))
            return Fail(out fault, at, "HEADER;");

        while (true)
        {
            begin = cur;
            var type = StepTokenizer.AdvancePastWhiteSpace(ref cur, end, ref begin);
            if (type != StepTokenType.Identifier)
                return Fail(out fault, type == StepTokenType.None ? cur : begin, "a header entity or ENDSEC closing the HEADER section");

            var name = StepTokenizer.Span(begin, cur);
            if (name.SequenceEqual("ENDSEC"u8))
                return StepTokenizer.Expect(ref cur, end, StepTokenType.Semicolon, out at) ? null : Fail(out fault, at, "';' after ENDSEC");

            var listIndex = tokens.Count;
            if (!StepTokenizer.Expect(ref cur, end, StepTokenType.BeginGroup, out at))
                return Fail(out fault, at, "'(' after the header entity name");
            if (!StepTokenizer.TryParseList(ref cur, end, tokens, out at))
                return Fail(out fault, at, "')' closing the list");
            if (!StepTokenizer.Expect(ref cur, end, StepTokenType.Semicolon, out at))
                return Fail(out fault, at, "';' ending the header entity");
            entities.TryAdd(System.Text.Encoding.ASCII.GetString(name), listIndex);
        }
    }

    private static bool ExpectKeyword(ref byte* cur, byte* end, ReadOnlySpan<byte> keyword, out byte* at)
        => StepTokenizer.Expect(ref cur, end, StepTokenType.Identifier, out at) && StepTokenizer.Span(at, cur).SequenceEqual(keyword);

    private static StepDiagnosticCode? Fail(out StepFault fault, byte* at, string expected, StepDiagnosticCode code = StepDiagnosticCode.Syntax)
    {
        fault = new StepFault(at, expected);
        return code;
    }

    private static IReadOnlyList<StepToken> Attributes(UnmanagedList<StepToken> tokens, Dictionary<string, int> entities, string name)
        => entities.TryGetValue(name, out var listIndex) ? Children(tokens, listIndex) : [];

    /// <summary>The elements of the list at <paramref name="listIndex"/>; a typed parameter <c>NAME(...)</c> counts as
    /// its name token.</summary>
    private static IReadOnlyList<StepToken> Children(UnmanagedList<StepToken> tokens, int listIndex)
    {
        var r = new List<StepToken>();
        var last = listIndex + tokens[listIndex].Length;
        for (var i = listIndex + 1; i <= last; i++)
        {
            var token = tokens[i];
            r.Add(token);
            if (token.IsEntity && i < last && tokens[i + 1].IsList)
                i++;
            if (tokens[i].IsList)
                i += tokens[i].Length;
        }
        return r;
    }

    private static string String(IReadOnlyList<StepToken> attributes, int index)
        => index < attributes.Count && attributes[index].IsString ? attributes[index].AsString() : "";

    private static IReadOnlyList<string> Strings(UnmanagedList<StepToken> tokens, IReadOnlyList<StepToken> attributes, int index)
        => index < attributes.Count && attributes[index].IsList
            ? Children(tokens, attributes[index].Index).Where(t => t.IsString).Select(t => t.AsString()).ToList()
            : [];

    private static IEnumerable<string> AllStrings(UnmanagedList<StepToken> tokens, IReadOnlyList<StepToken> attributes)
        => attributes.SelectMany(t => t.IsList ? Children(tokens, t.Index).Where(c => c.IsString) : [t]).Where(t => t.IsString).Select(t => t.AsString());

    private static string Unescape(string text)
        => text.Replace("''", "'");

    /// <summary>IFC4X3_ADD2 becomes IFC4X3, IFC2X3 and IFC4 stay; any other name is kept, trimmed.</summary>
    private static string NormalizeIfcSchema(string schema)
    {
        var t = schema.Trim();
        if (t.StartsWith("IFC4X3", StringComparison.OrdinalIgnoreCase))
            return "IFC4X3";
        if (t.StartsWith("IFC2X3", StringComparison.OrdinalIgnoreCase))
            return "IFC2X3";
        if (t.StartsWith("IFC4", StringComparison.OrdinalIgnoreCase))
            return "IFC4";
        return t;
    }
}
