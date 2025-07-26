using System;
using System.Collections.Generic;
using Ara3D.Logging;
using Ara3D.Memory;
using Ara3D.Utils;

namespace Ara3D.IO.StepParser
{
    public sealed unsafe class StepDocument : IDisposable
    {
        public readonly FilePath FilePath;
        public readonly byte* DataStart;
        public readonly byte* DataEnd;
        public readonly AlignedMemory Data;
        public readonly Dictionary<long, StepDefinition> Definitions = new();

        public StepDocument(FilePath filePath, ILogger logger = null)
        {
            FilePath = filePath;
            logger ??= Logger.Null;

            logger.Log($"Loading {filePath.GetFileSizeAsString()} of data from {filePath.GetFileName()}");
            Data = Serializer.ReadAllBytesAligned(filePath);
            DataStart = Data.GetPointer();
            DataEnd = DataStart + Data.NumBytes();

            logger.Log($"Starting tokenization");

            // Initialize the token list with a capacity of 16,000 tokens (the longest line we hope to encounter, but could be more)
            using var tokens = new UnmanagedList<StepToken>(16000);

            var cur = DataStart;

            while (true)
            {
                tokens.Clear();
                if (!StepTokenizer.AdvanceToAndTokenizeDefinition(ref cur, DataEnd, out var idToken, tokens))
                    break;

                var id = StepValueId.Create(idToken);
                if (!Assert(!Definitions.ContainsKey(id.Id), $"Duplicate definition found for ID {id.Id} in {filePath.GetFileName()}"))
                    continue;

                if (!Assert(tokens.Count > 2, "Expected at least 3 tokens for a definition identifier begin_group end_group"))
                    continue;

                if (!Assert(tokens[0].Type == StepTokenType.Identifier, "Expected Identifier token at start")) continue;
                if (!Assert(tokens[1].Type == StepTokenType.BeginGroup, "Expected BeginGroup token at start + 1")) continue;
                if (!Assert(tokens[^1].Type == StepTokenType.EndGroup, "Expected EndOfLine token at end")) continue;

                var index = 0;
                var value = CreateValue(tokens, ref index);
                if (!Assert(index == tokens.Count, "Did not consume all tokens in definition")) continue;
                var definition = new StepDefinition(id.Id, (StepValueEntity)value);
                Definitions.Add(id.Id, definition);
                
                tokens.Clear();
            }

            logger.Log($"Number of instance definitions = {Definitions.Count}");
            logger.Log($"Completed creation of STEP document from {filePath.GetFileName()}");
        }

        public static StepValue CreateValue(IReadOnlyList<StepToken> tokens, ref int current)
        {
            var token = tokens[current++];
            switch (token.Type)
            {
                case StepTokenType.SingleQuotedString:
                case StepTokenType.DoubleQuotedString:
                    return StepValueString.Create(token);

                case StepTokenType.Symbol:
                    return StepValueSymbol.Create(token);

                case StepTokenType.BeginGroup:
                    --current;
                    return CreateList(tokens, ref current);

                case StepTokenType.Identifier:
                {
                    var name = new StepEntityName(token.Slice);
                    var list = CreateList(tokens, ref current);
                    return new StepValueEntity(name, list);
                }
                case StepTokenType.Id:
                    return StepValueId.Create(token);

                case StepTokenType.Redeclared:
                    return StepValueRedeclared.Create(token);

                case StepTokenType.Unassigned:
                    return StepValueUnassigned.Create(token);

                case StepTokenType.Number:
                    return StepValueNumber.Create(token);

                default:
                    throw new Exception($"Cannot convert token type {token.Type} to a StepValue");
            }
        }

        public static bool Assert(bool condition, string text)
        {
            if (!condition)
                throw new Exception($"Assertion failed: {text}");
            return true;
        }
        
        public static StepValueList CreateList(IReadOnlyList<StepToken> tokens, ref int current)
        {
            var list = new StepValueList();
            Assert(tokens[current].Type == StepTokenType.BeginGroup, "Expected BeginGroup token to start a list");
            ++current; // Skip the BeginGroup token
            while (current < tokens.Count)
            {
                var token = tokens[current];
                if (token.Type == StepTokenType.EndGroup)
                {
                    ++current; // Skip the EndGroup token
                    return list;
                }
                list.Values.Add(CreateValue(tokens, ref current));
            }

            throw new Exception("End of the group was not found");
        }

        public void Dispose()
        {
            Data.Dispose();
            Data.Dispose();
        }

        public static StepDocument Create(FilePath fp) 
            => new(fp);

        public IEnumerable<StepDefinition> GetDefinitions()
            => Definitions.Values;
    }
}