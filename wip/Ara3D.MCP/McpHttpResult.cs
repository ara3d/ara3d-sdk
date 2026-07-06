using System.Text.Json.Nodes;

namespace Ara3D.MCP;

public readonly record struct McpHttpResult(int StatusCode, string? JsonBody)
{
    public static McpHttpResult Accepted() => new(202, null);
    public static McpHttpResult BadRequest() => new(400, null);
    public static McpHttpResult Json(JsonNode payload) => new(200, payload.ToJsonString());
}
