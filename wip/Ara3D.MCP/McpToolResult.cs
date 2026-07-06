using System.Text.Json.Serialization;

namespace Ara3D.MCP;

/// <summary>Standard envelope returned by MCP tool handlers.</summary>
public sealed record McpToolResult(
    bool Ok,
    object? Data = null,
    string? Error = null,
    [property: JsonPropertyName("type")] string? ErrorType = null,
    IReadOnlyList<string>? NextRecommendedTools = null)
{
    public static McpToolResult Success(object data)
        => new(Ok: true, Data: data);

    public static McpToolResult Success(object data, IReadOnlyList<string>? nextRecommendedTools)
        => new(Ok: true, Data: data, NextRecommendedTools: nextRecommendedTools);

    public static McpToolResult Failure(Exception exception)
        => new(
            Ok: false,
            Error: exception.Message,
            ErrorType: exception.GetType().Name);
}
