namespace Ara3D.MCP;

/// <summary>Runs MCP tool handlers and returns serialized <see cref="McpToolResult"/> JSON.</summary>
public static class ToolRunner
{
    public static Task<string> RunAsync(Func<Task<object>> action)
        => RunAsync(action, null);

    public static async Task<string> RunAsync(
        Func<Task<object>> action,
        IReadOnlyList<string>? nextRecommendedTools)
    {
        try
        {
            var payload = await action().ConfigureAwait(false);
            return SerializeSuccess(payload, nextRecommendedTools);
        }
        catch (Exception ex)
        {
            return McpJson.Serialize(McpToolResult.Failure(ex));
        }
    }

    public static Task<string> RunAsync(Func<object> action)
        => RunAsync(() => Task.FromResult(action()));

    public static Task<string> RunAsync(
        Func<object> action,
        IReadOnlyList<string>? nextRecommendedTools)
        => RunAsync(() => Task.FromResult(action()), nextRecommendedTools);

    private static string SerializeSuccess(object payload, IReadOnlyList<string>? nextRecommendedTools)
    {
        if (nextRecommendedTools is null || nextRecommendedTools.Count == 0)
            return McpJson.Serialize(McpToolResult.Success(payload));

        return McpJson.Serialize(McpToolResult.Success(payload, nextRecommendedTools));
    }
}
