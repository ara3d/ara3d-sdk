using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using Ara3D.Ifc.Mcp;

namespace PlatoFlow.Host;

/// <summary>Turns a natural-language question about one model into a single read-only DuckDB
/// SELECT via the Anthropic Messages API (raw HttpClient; no SDK dependency). Returns
/// <c>{sql}</c> or <c>{error}</c> and never executes anything itself — the browser feeds the
/// returned SQL through the existing read-only <c>/api/sql</c> path, so the worst a bad
/// completion can do is fail there.</summary>
public sealed class AskService(ModelCatalog catalog)
{
    private const string DefaultModel = "claude-sonnet-5";
    private const string Endpoint = "https://api.anthropic.com/v1/messages";

    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(90) };
    private readonly Dictionary<string, string> _prompts = new(StringComparer.OrdinalIgnoreCase);

    public JsonObject Ask(string? modelId, string? question)
    {
        if (string.IsNullOrWhiteSpace(question))
            return Error("question is required");

        var model = catalog.Require(modelId);

        var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
            return Error("ANTHROPIC_API_KEY not set on host");

        var llm = Environment.GetEnvironmentVariable("ANTHROPIC_MODEL");
        if (string.IsNullOrWhiteSpace(llm))
            llm = DefaultModel;

        var request = new JsonObject
        {
            ["model"] = llm,
            ["max_tokens"] = 1000,
            ["system"] = SystemPrompt(model),
            ["messages"] = new JsonArray
            {
                new JsonObject { ["role"] = "user", ["content"] = question.Trim() },
            },
        };

        JsonObject? completion;
        using (var http = new HttpRequestMessage(HttpMethod.Post, Endpoint))
        {
            http.Headers.Add("x-api-key", apiKey);
            http.Headers.Add("anthropic-version", "2023-06-01");
            http.Content = new StringContent(request.ToJsonString(), Encoding.UTF8, "application/json");

            string body;
            try
            {
                using var response = Http.Send(http);
                using var reader = new StreamReader(response.Content.ReadAsStream());
                body = reader.ReadToEnd();
                if (!response.IsSuccessStatusCode)
                    return Error($"Anthropic API HTTP {(int)response.StatusCode}: {ApiError(body)}");
            }
            catch (Exception ex)
            {
                return Error($"Anthropic API request failed: {ex.GetBaseException().Message}");
            }

            completion = JsonNode.Parse(body) as JsonObject;
        }

        // Check stop_reason before reading content: safety classifiers answer HTTP 200 with
        // stop_reason "refusal" and empty content.
        var stop = completion?["stop_reason"]?.GetValue<string>();
        if (stop == "refusal")
            return Error("The model declined to answer this question.");

        var text = ExtractText(completion);
        if (string.IsNullOrWhiteSpace(text))
            return Error($"The model returned no text (stop_reason: {stop ?? "unknown"}).");

        try
        {
            // Same guard as /api/sql: one statement, SELECT/WITH only. Throws otherwise.
            return new JsonObject { ["sql"] = IfcDuck.ReadOnlyQuery(StripFences(text)) };
        }
        catch (ArgumentException ex)
        {
            return Error($"The model did not return a single SELECT statement ({ex.Message.Split('\n')[0]})");
        }
    }

    /// <summary>The system prompt embeds the model's real view schemas (introspected once per model
    /// from its DuckDB via information_schema) so the LLM never has to guess column names.</summary>
    private string SystemPrompt(ModelEntry model)
    {
        if (_prompts.TryGetValue(model.Id, out var cached))
            return cached;

        var sb = new StringBuilder();
        sb.AppendLine("You translate questions about a BIM (IFC) building model into DuckDB SQL.");
        sb.AppendLine("Respond with EXACTLY ONE DuckDB SELECT (or WITH) statement and nothing else:");
        sb.AppendLine("no prose, no explanation, no markdown fences, no semicolons, one statement only.");
        sb.AppendLine("Query only these views:");

        foreach (var view in new[] { "EntityText", "ParameterText", "RelationText" })
        {
            sb.AppendLine().AppendLine($"{view}:");
            foreach (var column in Columns(model, view))
                sb.AppendLine($"  {column}");
        }

        sb.AppendLine();
        sb.AppendLine("Category values present in THIS model (with entity counts) — match these verbatim:");
        foreach (var category in Categories(model))
            sb.AppendLine($"  {category}");

        sb.AppendLine();
        sb.AppendLine("Gotchas:");
        sb.AppendLine("- EntityText.Category is the IFC class of an element; EntityText.Type is the family/type string, NOT the IFC class.");
        sb.AppendLine("- Category values are stored UPPERCASE (e.g. IFCDOOR); use the exact values listed above.");
        sb.AppendLine("- Parameter values are text; use TRY_CAST for arithmetic.");
        sb.AppendLine("- Prefer ILIKE for user-supplied name matching.");

        return _prompts[model.Id] = sb.ToString();
    }

    private IEnumerable<string> Categories(ModelEntry model)
    {
        try
        {
            var table = catalog.Query(model.Id,
                "SELECT Category, COUNT(*) AS n FROM EntityText GROUP BY Category ORDER BY n DESC LIMIT 60");
            return table["rows"]!.AsArray()
                .Select(row => $"{row![0]} ({row[1]})")
                .ToList();
        }
        catch (Exception ex)
        {
            return [$"(categories unavailable: {ex.Message.Split('\n')[0]})"];
        }
    }

    private IEnumerable<string> Columns(ModelEntry model, string view)
    {
        JsonObject table;
        try
        {
            table = catalog.Query(model.Id,
                "SELECT column_name, data_type FROM information_schema.columns "
                + $"WHERE table_name = '{view}' ORDER BY ordinal_position");
        }
        catch (Exception ex)
        {
            return [$"(schema unavailable: {ex.Message.Split('\n')[0]})"];
        }

        return table["rows"]!.AsArray()
            .Select(row => $"{row![0]} {row[1]}")
            .ToList();
    }

    private static string? ExtractText(JsonObject? completion)
    {
        if (completion?["content"] is not JsonArray blocks)
            return null;

        var sb = new StringBuilder();
        foreach (var block in blocks.OfType<JsonObject>())
            if (block["type"]?.GetValue<string>() == "text")
                sb.Append(block["text"]?.GetValue<string>());
        return sb.ToString();
    }

    /// <summary>Defensive cleanup: models sometimes fence the SQL despite instructions.</summary>
    private static string StripFences(string text)
    {
        var trimmed = text.Trim();
        if (!trimmed.StartsWith("```"))
            return trimmed;

        var lines = trimmed.Split('\n').ToList();
        lines.RemoveAt(0);                                    // ```sql or ```
        var closing = lines.FindLastIndex(l => l.TrimStart().StartsWith("```"));
        if (closing >= 0)
            lines.RemoveRange(closing, lines.Count - closing);
        return string.Join('\n', lines).Trim();
    }

    private static string ApiError(string body)
    {
        try
        {
            var parsed = JsonNode.Parse(body);
            return parsed?["error"]?["message"]?.GetValue<string>() ?? Excerpt(body);
        }
        catch
        {
            return Excerpt(body);
        }
    }

    private static string Excerpt(string text)
        => text.Length <= 300 ? text : text[..300] + "...";

    private static JsonObject Error(string message)
        => new() { ["error"] = message };
}
