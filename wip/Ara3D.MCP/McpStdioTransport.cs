using System.Diagnostics;

namespace Ara3D.MCP;

/// <summary>Line-delimited JSON-RPC over a reader/writer pair — the transport a client uses when
/// it launches the server as a child process. One message per input line, one response line per
/// message that produces a body; notifications (HTTP 202) and unparseable input write nothing.
/// The pump ends when the reader hits EOF, which is how a client signals shutdown.</summary>
internal sealed class McpStdioTransport : IDisposable
{
    private readonly Func<string, McpHttpResult> _handle;
    private readonly TextReader _input;
    private readonly TextWriter _output;
    private readonly Thread _thread;
    private readonly ManualResetEventSlim _finished = new(false);
    private readonly CancellationTokenSource _cts = new();

    public McpStdioTransport(Func<string, McpHttpResult> handle, TextReader input, TextWriter output)
    {
        _handle = handle ?? throw new ArgumentNullException(nameof(handle));
        _input = input ?? throw new ArgumentNullException(nameof(input));
        _output = output ?? throw new ArgumentNullException(nameof(output));
        _thread = new Thread(Pump)
        {
            IsBackground = true,
            Name = "Ara3D MCP stdio"
        };
    }

    public bool Active => _thread.IsAlive;

    public void Start()
    {
        if (!_thread.IsAlive && !_finished.IsSet)
            _thread.Start();
    }

    /// <summary>Blocks until the pump ends, so a console host can start the server and sleep
    /// until its client closes stdin.</summary>
    public void WaitForShutdown()
        => _finished.Wait();

    public void Stop()
        => _cts.Cancel();

    private void Pump()
    {
        try
        {
            while (!_cts.IsCancellationRequested)
            {
                var line = _input.ReadLine();
                if (line == null)
                    return;
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                HandleLine(line);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
        finally
        {
            _finished.Set();
        }
    }

    private void HandleLine(string line)
    {
        var result = _handle(line);
        if (result.JsonBody == null)
            return;

        lock (_output)
        {
            _output.Write(result.JsonBody);
            _output.Write('\n');
            _output.Flush();
        }
    }

    public void Dispose()
    {
        Stop();
        _finished.Dispose();
        _cts.Dispose();
    }
}
