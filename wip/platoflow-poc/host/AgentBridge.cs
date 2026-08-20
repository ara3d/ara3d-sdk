using System.Text.Json.Nodes;

namespace PlatoFlow.Host;

/// <summary>The whole agent-to-browser channel: a monotonic intent queue the browser polls, and the
/// last graph + eval results the browser pushed back. In memory only — restarting the host resets
/// the conversation, which for a PoC is the correct amount of durability.</summary>
public sealed class AgentBridge
{
    private readonly List<JsonNode> _intents = [];
    private readonly object _lock = new();
    private int _nextNodeId;

    public JsonNode? Doc { get; private set; }

    public JsonObject? Results { get; private set; }

    /// <summary>Sequence numbers are 1-based, so a client can poll with <c>since=0</c> and get
    /// everything issued so far.</summary>
    public int Enqueue(JsonNode intent)
    {
        lock (_lock)
        {
            _intents.Add(intent);
            return _intents.Count;
        }
    }

    public JsonObject Since(int since)
    {
        lock (_lock)
        {
            var list = new JsonArray();
            for (var i = Math.Max(since, 0); i < _intents.Count; i++)
                list.Add(new JsonObject { ["seq"] = i + 1, ["intent"] = _intents[i].DeepClone() });

            return new JsonObject { ["intents"] = list, ["now"] = _intents.Count };
        }
    }

    public void SetState(JsonNode? doc, JsonNode? results)
    {
        lock (_lock)
        {
            Doc = doc?.DeepClone();
            Results = results?.DeepClone() as JsonObject;
        }
    }

    /// <summary>Ids for agent-created nodes: a1, a2, ... The prefix keeps them from colliding with
    /// the editor's own ids, and the host owning the counter is what lets <c>add_node</c> answer
    /// with the id immediately instead of waiting for the browser to apply the intent.</summary>
    public string NextNodeId()
    {
        lock (_lock)
            return "a" + ++_nextNodeId;
    }

    /// <summary>The cached result for one node, trimmed to what an agent can usefully read: status,
    /// summary, and at most <paramref name="rowLimit"/> table rows.</summary>
    public JsonObject ReadNode(string nodeId, int rowLimit = 50)
    {
        lock (_lock)
        {
            if (Results?[nodeId] is not JsonObject entry)
                return new JsonObject
                {
                    ["node"] = nodeId,
                    ["known"] = false,
                    ["message"] = Results == null
                        ? "No evaluation results have been pushed yet (the browser POSTs /api/state after each eval)."
                        : $"No result for node '{nodeId}'.",
                };

            var result = new JsonObject
            {
                ["node"] = nodeId,
                ["known"] = true,
                ["state"] = entry["state"]?.DeepClone(),
                ["summary"] = entry["summary"]?.DeepClone(),
                ["message"] = entry["message"]?.DeepClone(),
            };

            if (entry["table"] is JsonObject table)
            {
                var rows = table["rows"] as JsonArray ?? [];
                var page = new JsonArray();
                foreach (var row in rows.Take(rowLimit))
                    page.Add(row!.DeepClone());

                result["table"] = new JsonObject
                {
                    ["columns"] = table["columns"]?.DeepClone() ?? new JsonArray(),
                    ["rows"] = page,
                    ["totalRows"] = rows.Count,
                };
            }

            return result;
        }
    }
}
