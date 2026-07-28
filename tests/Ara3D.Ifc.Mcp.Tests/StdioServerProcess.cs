using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Ara3D.Ifc.Mcp.Tests;

/// <summary>Launches the MCP server as a real child process and speaks line-delimited JSON-RPC to
/// it over the pipes an MCP client would use. Every read is bounded, so a server that never
/// answers fails the test instead of hanging it.</summary>
public sealed class StdioServerProcess : IDisposable
{
    private readonly Process _process;
    private readonly BlockingCollection<string> _stdout = new();
    private readonly StringBuilder _stderr = new();

    /// <summary>The server host executable, copied next to the tests by the project reference, so
    /// no build runs inside the timed window.</summary>
    public static string HostPath
        => Path.Combine(TestContext.CurrentContext.TestDirectory, "ara3d-ifc-mcp.exe");

    public StdioServerProcess(IReadOnlyList<string> extraArgs)
    {
        if (!File.Exists(HostPath))
            Assert.Ignore($"Server host {HostPath} not found; build the test project first.");

        var info = new ProcessStartInfo(HostPath)
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = new UTF8Encoding(false),
            StandardInputEncoding = new UTF8Encoding(false),
            WorkingDirectory = Path.GetDirectoryName(HostPath)!
        };
        for (var i = 0; i < extraArgs.Count; i++)
            info.ArgumentList.Add(extraArgs[i]);

        _process = Process.Start(info) ?? throw new InvalidOperationException("Failed to start the server host.");
        _process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
                _stdout.Add(e.Data);
        };
        _process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
                lock (_stderr) _stderr.AppendLine(e.Data);
        };
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
    }

    public string Stderr
    {
        get { lock (_stderr) return _stderr.ToString(); }
    }

    public void Send(object request)
        => _process.StandardInput.Write(JsonSerializer.Serialize(request) + "\n");

    /// <summary>Reads until a JSON object carrying <paramref name="id"/> arrives, ignoring any
    /// non-protocol chatter. Returns null when the budget expires or the pipe closes.</summary>
    public JsonElement? ReadResponse(int id, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (true)
        {
            var remaining = deadline - DateTime.UtcNow;
            if (remaining <= TimeSpan.Zero || !_stdout.TryTake(out var line, remaining))
                return null;
            if (!TryParse(line, out var element))
                continue;
            if (element.TryGetProperty("id", out var actual) && actual.ValueKind == JsonValueKind.Number
                && actual.GetInt32() == id)
                return element;
        }
    }

    private static bool TryParse(string line, out JsonElement element)
    {
        element = default;
        if (!line.TrimStart().StartsWith('{'))
            return false;
        try
        {
            element = JsonDocument.Parse(line).RootElement.Clone();
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    public void Dispose()
    {
        try
        {
            if (!_process.HasExited)
            {
                _process.StandardInput.Close();
                if (!_process.WaitForExit(5000))
                    _process.Kill(entireProcessTree: true);
            }
        }
        catch (InvalidOperationException)
        {
        }

        // The stdout collection is deliberately not disposed: the redirection handlers can still
        // fire after the kill, and adding to a disposed collection would throw on their thread.
        _process.Dispose();
    }
}
