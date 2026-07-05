using Ara3D.Logging;
using Ara3D.Memory;
using Ara3D.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        var cur = DataStart;

        logger?.Log($"Parsing header");
        Header = StepHeader.Parse(ref cur, DataEnd);

        if (!StepTokenizer.AdvanceToData(ref cur, DataEnd))
            throw new Exception("Failed to find data");

        logger?.Log($"Parsing definitions");
        while (true)
        {
            var curIndex = Tokens.Count;
            
            if (!StepTokenizer.AdvanceToAndTokenizeDefinition(ref cur, DataEnd, out var id, out var nameToken, out var attrToken, Tokens))
                break;

            Debug.Assert(Tokens.Count > 0);
            Debug.Assert(Tokens[curIndex].IsList);
            Debug.Assert(attrToken.Equals(Tokens[curIndex]));
            Debug.Assert(attrToken.Index == curIndex);
            Debug.Assert(attrToken.IsList);
            Debug.Assert(attrToken.Length >= 0);

            // Force a debug check
            var _ = attrToken.AsSpan(this); 

            var def = new StepDefinition(id, nameToken, attrToken);
            Definitions.Add(def);
        }

        logger?.Log($"# tokens = {Tokens.Count}, estimated was {estimateNumTokens}");
        logger?.Log($"# definitions = {Definitions.Count}, estimated was {estimateNumDefs}");
    }

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
    }

    public static StepDocument Create(FilePath fp) 
        => new(fp);

}