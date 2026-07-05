using System.Text;
using Ara3D.Utils;

namespace Ara3D.Bowerbird;

/// <summary>
/// Writes folder/bin/compile.log with compile inputs and diagnostics.
/// </summary>
public static class CompilationLogWriter
{
    public const string LogFileName = "compile.log";

    public static void Write(CommandDescriptor descriptor, CommandCompileResult result, Exception exception = null)
    {
        descriptor.OutputFolder.Create();
        var logPath = descriptor.CompileLogPath;
        var sb = new StringBuilder();
        sb.AppendLine($"Compiled at {DateTime.Now:O}");
        sb.AppendLine($"Cache hit: {result.FromCache}");
        sb.AppendLine($"Fingerprint: {result.CacheKey.Fingerprint}");
        sb.AppendLine($"Output: {result.OutputDll}");
        sb.AppendLine("Source files:");
        foreach (var file in descriptor.SourceFiles)
            sb.AppendLine($"  {file}");

        if (result.Compilation != null)
        {
            sb.AppendLine($"Success: {result.Compilation.Success}");
            foreach (var diagnostic in result.Compilation.AllDiagnostics)
                sb.AppendLine(diagnostic);
        }
        else if (result.FromCache)
        {
            sb.AppendLine("Success: True");
        }

        if (exception != null)
            sb.AppendLine($"Exception: {exception}");

        logPath.WriteAllText(sb.ToString());
    }
}
