using System.Text;
using Ara3D.Ifc.Mcp;
using Ara3D.MCP;

// Stdio is the default because that is how MCP clients launch a server. Under it stdout is the
// protocol stream, so every diagnostic goes to stderr. Pass --http [port] to listen instead.
var useHttp = args.Contains("--http", StringComparer.OrdinalIgnoreCase);
var port = ParsePort(args);

Console.OutputEncoding = new UTF8Encoding(false);

using var cache = new IfcSessionCache();
using var mcp = IfcMcpServer.Create(cache, useHttp ? McpTransport.Http : McpTransport.Stdio, port);

mcp.Start();

if (useHttp)
{
    Console.Error.WriteLine($"{IfcMcpServer.ServerName} listening at {mcp.Url}");
    Console.Error.WriteLine("Press Ctrl+C to stop.");
    using var stop = new ManualResetEventSlim(false);
    Console.CancelKeyPress += (_, e) =>
    {
        e.Cancel = true;
        // ReSharper disable once AccessToDisposedClosure
        stop.Set();
    };
    stop.Wait();
}
else
{
    Console.Error.WriteLine($"{IfcMcpServer.ServerName} {IfcMcpServer.ServerVersion} on stdio.");
    mcp.WaitForShutdown();
}

return 0;

static int ParsePort(string[] arguments)
{
    var index = Array.FindIndex(arguments, a => a.Equals("--http", StringComparison.OrdinalIgnoreCase));
    return index >= 0 && index + 1 < arguments.Length && int.TryParse(arguments[index + 1], out var value)
        ? value
        : McpServer.DefaultPort;
}
