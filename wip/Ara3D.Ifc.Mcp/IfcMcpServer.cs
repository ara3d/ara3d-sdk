using Ara3D.MCP;

namespace Ara3D.Ifc.Mcp;

public static class IfcMcpServer
{
    public const string ServerName = "ara3d-ifc";
    public const string ServerVersion = "0.1.0";

    /// <summary>Builds a server with the whole IFC tool surface registered against one session
    /// cache. The caller owns both and must dispose them.</summary>
    public static McpServer Create(IfcSessionCache cache, McpTransport transport, int port = McpServer.DefaultPort)
        => RegisterTools(new McpServer(port, ServerName, ServerVersion, transport: transport), cache);

    public static McpServer RegisterTools(McpServer mcp, IfcSessionCache cache)
    {
        IfcModelTools.Register(mcp, cache);
        IfcEntityTools.Register(mcp, cache);
        IfcPropertyTools.Register(mcp, cache);
        IfcRelationTools.Register(mcp, cache);
        IfcAnalyticsTools.Register(mcp, cache);
        return mcp;
    }
}
