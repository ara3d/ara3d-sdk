using Ara3D.Logging;
using Ara3D.Memory;
using Ara3D.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Ara3D.IO.StepParser;

public sealed unsafe class StepDocument : IDisposable
{
    public readonly FilePath FilePath;
    public readonly byte* DataStart;
    public readonly byte* DataEnd;
    public readonly IBuffer Data;
    public readonly List<StepDefinition> Definitions = new();
    public readonly StepHeader Header;
    public UnmanagedList<StepToken> Tokens = new();

    /// <summary>Findings in file order. An error means parsing stopped there: <see cref="Definitions"/> holds every
    /// instance completed before it, and <see cref="Header"/> the fields read before it.</summary>
    public readonly IReadOnlyList<StepDiagnostic> Diagnostics;

    public StepDocument(FilePath filePath, ILogger logger = null)
        : this(Serializer.ReadAllBytesAligned(filePath), filePath, logger)
    { }

    public StepDocument(IBuffer data, string filePath = "", ILogger logger = null)
    {
        FilePath = filePath;
        Data = data;
        DataStart = Data.GetPointer();
        DataEnd = DataStart + Data.NumBytes();

        logger?.Log($"Parsing definition");

        // Estimate average token length of 8 bytes
        var estimateNumTokens = Data.NumBytes() / 8;
        Tokens.Accomodate((int)estimateNumTokens);

        // Estimate about 8 tokens per definition on average per definition
        var estimateNumDefs = estimateNumTokens / 8; 
        Definitions = new List<StepDefinition>((int)estimateNumDefs);

        var diagnostics = new List<StepDiagnostic>();
        var cur = DataStart;

        logger?.Log($"Parsing header");
        Header = StepHeader.Parse(ref cur, DataEnd, DataStart, diagnostics);

        logger?.Log($"Parsing definitions");
        if (!diagnostics.Any(d => d.Severity == StepSeverity.Error))
        {
            if (StepTokenizer.ReadDataStart(ref cur, DataEnd, Tokens, out var dataFault))
                ReadDefinitions(ref cur, diagnostics);
            else
                diagnostics.Add(new StepDiagnostic(StepSeverity.Error, StepDiagnosticCode.Syntax, Offset(dataFault), "DATA; opening the DATA section"));
        }
        Diagnostics = diagnostics;

        logger?.Log($"# tokens = {Tokens.Count}, estimated was {estimateNumTokens}");
        logger?.Log($"# definitions = {Definitions.Count}, estimated was {estimateNumDefs}");
    }

    /// <summary>Reads instances until the DATA section's ENDSEC; a fault drops the unfinished instance's tokens,
    /// records an error, and ends the section.</summary>
    private void ReadDefinitions(ref byte* cur, List<StepDiagnostic> diagnostics)
    {
        while (true)
        {
            var tokenCount = Tokens.Count;
            var result = StepTokenizer.ReadDefinition(ref cur, DataEnd, Tokens, out var definition, out var fault);
            if (result == StepReadResult.EndOfSection)
                return;
            if (result == StepReadResult.Fault)
            {
                Tokens.SetCount(tokenCount);
                diagnostics.Add(new StepDiagnostic(StepSeverity.Error, StepDiagnosticCode.Syntax, Offset(fault.At), fault.Expected,
                    fault.DefinitionId, fault.DefinitionId == 0 ? -1 : Offset(fault.DefinitionBegin)));
                return;
            }

            Debug.Assert(definition.AttributesToken.Index == tokenCount);
            Debug.Assert(definition.AttributesToken.IsList);
            Debug.Assert(definition.AttributesToken.Length >= 0);
            Definitions.Add(definition);
        }
    }

    private long Offset(byte* at)
        => StepTokenizer.Offset(at, DataStart, DataEnd);

    public static bool Assert(bool condition, string text)
    {
        if (!condition)
            throw new Exception($"Assertion failed: {text}");
        return true;
    }
        
    public void Dispose()
    {
        Trace.WriteLine($"Disposing data");
        if (Data is IDisposable d)
            d.Dispose();
        Tokens.Dispose();
    }

    public static StepDocument Create(FilePath fp) 
        => new(fp);

}