using Ara3D.Logging;
using System;
using System.Threading;

namespace Ara3D.Utils.Roslyn
{
    /// <summary>
    /// Used for a single compilation event.
    /// </summary>
    public class Compilation
    {
        public ILogger Logger { get; }
        public CompilerInput Input { get; }
        public CompilerOutput Output { get; }

        public Compilation(
            CompilerInput input,
            ILogger logger, 
            CancellationToken token)
        {
            Input = input;
            Logger = logger;

            try
            {
                Log("Parsing");
                if (token.IsCancellationRequested) return;
                var parsedInput = new ParsedCompilerInput(input, token);

                Log("Compiling");
                if (token.IsCancellationRequested) return;
                Output = parsedInput.CompileCSharpStandard(null, token);

                Log($"Diagnostics");
                foreach (var x in Output.Errors)
                    Log($"  {x}");

                Log(Output.Success ? "Compilation Succeeded" : "Compilation Failed");

            }
            catch (Exception e)
            {
                Logger?.LogError(e);
            }
        }

        public void Log(string s)
            => Logger?.Log(s);
    }
}