namespace Ara3D.MCP;

public sealed class McpProtocolException(int code, string message) : Exception(message)
{
    public int Code { get; } = code;
}
