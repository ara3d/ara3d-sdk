namespace Ara3D.IO.StepParser;

public enum StepDiagnosticCode : byte
{
    /// <summary>The data does not begin with <c>ISO-10303-21;</c>; nothing was read.</summary>
    NotStepFile,

    /// <summary>The header lacks FILE_DESCRIPTION, FILE_NAME or FILE_SCHEMA; that entity's fields are empty.</summary>
    HeaderIncomplete,

    /// <summary>The text stops matching Part 21, or ends, where <see cref="StepDiagnostic.Expected"/> was due.</summary>
    Syntax,
}

public enum StepSeverity : byte
{
    Warning,

    /// <summary>Parsing stopped at this point: the document holds what came before it.</summary>
    Error,
}

/// <summary>A finding about the file's text. <paramref name="Offset"/> is the 0-based byte offset from the start of
/// the data where the parser found <paramref name="Expected"/> missing. When the fault lies inside an instance,
/// <paramref name="DefinitionId"/> and <paramref name="DefinitionOffset"/> name it and where it begins, and the
/// instance is not in <see cref="StepDocument.Definitions"/>; otherwise they are 0 and -1.</summary>
public readonly record struct StepDiagnostic(
    StepSeverity Severity,
    StepDiagnosticCode Code,
    long Offset,
    string Expected,
    int DefinitionId = 0,
    long DefinitionOffset = -1)
{
    public override string ToString()
        => DefinitionId == 0
            ? $"{Severity} {Code} at byte {Offset}: expected {Expected}"
            : $"{Severity} {Code} at byte {Offset} in #{DefinitionId} (starting at byte {DefinitionOffset}): expected {Expected}";
}
