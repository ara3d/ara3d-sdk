using System.Text;
using Ara3D.Utils;
using Ara3D.Utils.Roslyn;

namespace Ara3D.Bowerbird;

/// <summary>
/// Writes folder/bin/compile.log with compile inputs and diagnostics.
/// </summary>
public static class CompilationLogWriter
{
    public const string LogFileName = "compile.log";

    public static void Write(CommandDescriptor descriptor, CompilerOutput output, Exception exception = null)
    {
        descriptor.OutputFolder.Create();
        var logPath = descriptor.CompileLogPath;
        var sb = new StringBuilder();
        sb.AppendLine($"Compiled at {DateTime.Now:O}");
        sb.AppendLine($"Output: {output?.OutputFilePath ?? descriptor.OutputDll}");
        sb.AppendLine("Source files:");
        foreach (var file in descriptor.SourceFiles)
            sb.AppendLine($"  {file}");

        if (output != null)
        {
            sb.AppendLine($"Success: {output.Success}");
            foreach (var diagnostic in output.AllDiagnostics)
                sb.AppendLine(diagnostic);
        }

        if (exception != null)
            sb.AppendLine($"Exception: {exception}");

        logPath.WriteAllText(sb.ToString());
    }
}
