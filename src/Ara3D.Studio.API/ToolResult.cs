using Ara3D.Utils;

namespace Ara3D.Studio.API;

/// <summary>
/// Outcome of a tool or workflow run. Interactive hosts may ignore it; CLI runners and tests
/// turn it into exit codes and reports.
/// </summary>
public sealed record ToolResult(bool Success, string Message = "", IReadOnlyList<FilePath>? Artifacts = null)
{
    public static ToolResult Ok(string message = "")
        => new(true, message);

    public static ToolResult Fail(string message)
        => new(false, message);
}
