using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Ara3D.Bowerbird;
using Ara3D.Utils;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;

namespace Ara3D.Bowerbird.RevitSamples;

/// <summary>
/// Starts a localhost MCP server (Streamable HTTP) for Cursor.
/// </summary>
public class CommandMcpServer : NamedCommand
{
    const int Port = 8765;
    const string McpPath = "/mcp";
    const int RevitBridgeTimeoutMs = 30_000;

    static WebServer _server;
    static ExternalEvent _revitEvent;
    static RevitBridgeHandler _revitBridge;

    public override string Name => "Start MCP Server";

    public override void Execute(object arg)
    {
        if (_server?.Active == true)
        {
            MessageBox.Show($"MCP server is already running at {McpUrl}");
            return;
        }

        _revitBridge = new RevitBridgeHandler();
        _revitEvent = ExternalEvent.Create(_revitBridge);
        _server = new WebServer(HandleRequest, Port);
        _server.Start();

        var config = $@"Add to %USERPROFILE%\.cursor\mcp.json:

{{
  ""mcpServers"": {{
    ""bowerbird-revit"": {{
      ""url"": ""{McpUrl}""
    }}
  }}
}}

Reload MCP in Cursor (Settings → Tools & MCP), then try the echo or revit_document_info tools.";

        MessageBox.Show($"MCP server started at {McpUrl}\n\n{config}", "Bowerbird MCP Server");
    }

    static string McpUrl => $"http://127.0.0.1:{Port}{McpPath}";

    static void HandleRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;

        if (!IsAllowedOrigin(request))
        {
            response.StatusCode = 403;
            return;
        }

        var path = request.Url?.AbsolutePath ?? "";
        if (!path.Equals(McpPath, StringComparison.OrdinalIgnoreCase))
        {
            response.StatusCode = 404;
            return;
        }

        if (request.HttpMethod == "GET")
        {
            response.StatusCode = 405;
            return;
        }

        if (request.HttpMethod != "POST")
        {
            response.StatusCode = 405;
            return;
        }

        string body;
        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            body = reader.ReadToEnd();

        if (string.IsNullOrWhiteSpace(body))
        {
            response.StatusCode = 400;
            return;
        }

        var message = JToken.Parse(body);
        if (message is JArray batch)
        {
            HandleBatch(batch, response);
            return;
        }

        HandleMessage(message as JObject, response);
    }

    static void HandleBatch(JArray batch, HttpListenerResponse response)
    {
        var requests = batch.OfType<JObject>().Where(IsRequest).ToList();
        if (requests.Count == 0)
        {
            response.StatusCode = 202;
            return;
        }

        var responses = requests.Select(HandleRequestObject).Where(r => r != null).ToList();
        WriteJson(response, responses.Count == 1 ? responses[0]! : new JArray(responses));
    }

    static void HandleMessage(JObject message, HttpListenerResponse response)
    {
        if (message == null)
        {
            response.StatusCode = 400;
            return;
        }

        if (!IsRequest(message))
        {
            response.StatusCode = 202;
            return;
        }

        WriteJson(response, HandleRequestObject(message)!);
    }

    static JObject HandleRequestObject(JObject request)
    {
        var id = request["id"];
        var method = request["method"]?.Value<string>() ?? "";

        try
        {
            var result = method switch
            {
                "initialize" => HandleInitialize(),
                "tools/list" => HandleToolsList(),
                "tools/call" => HandleToolsCall(request["params"] as JObject),
                "ping" => new JObject(),
                _ => throw new McpProtocolException(-32601, $"Method not found: {method}"),
            };

            if (id == null || id.Type == JTokenType.Null)
                return null;

            return new JObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = id,
                ["result"] = result,
            };
        }
        catch (McpProtocolException ex)
        {
            if (id == null || id.Type == JTokenType.Null)
                return null;

            return new JObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = id,
                ["error"] = new JObject
                {
                    ["code"] = ex.Code,
                    ["message"] = ex.Message,
                },
            };
        }
        catch (Exception ex)
        {
            if (id == null || id.Type == JTokenType.Null)
                return null;

            return new JObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = id,
                ["error"] = new JObject
                {
                    ["code"] = -32603,
                    ["message"] = ex.Message,
                },
            };
        }
    }

    static bool IsRequest(JObject message)
        => message["method"] != null && message["id"] != null;

    static JObject HandleInitialize()
        => new()
        {
            ["protocolVersion"] = "2025-03-26",
            ["capabilities"] = new JObject
            {
                ["tools"] = new JObject(),
            },
            ["serverInfo"] = new JObject
            {
                ["name"] = "bowerbird-revit-mcp",
                ["version"] = "1.0.0",
            },
        };

    static JObject HandleToolsList()
        => new()
        {
            ["tools"] = new JArray
            {
                new JObject
                {
                    ["name"] = "echo",
                    ["description"] = "Echoes the message back to the client.",
                    ["inputSchema"] = new JObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JObject
                        {
                            ["message"] = new JObject
                            {
                                ["type"] = "string",
                                ["description"] = "Message to echo",
                            },
                        },
                        ["required"] = new JArray("message"),
                    },
                },
                new JObject
                {
                    ["name"] = "revit_document_info",
                    ["description"] = "Returns the active Revit document title and path.",
                    ["inputSchema"] = new JObject
                    {
                        ["type"] = "object",
                        ["properties"] = new JObject(),
                    },
                },
            },
        };

    static JObject HandleToolsCall(JObject parameters)
    {
        var name = parameters?["name"]?.Value<string>();
        var arguments = parameters?["arguments"] as JObject ?? new JObject();

        var text = name switch
        {
            "echo" => HandleEcho(arguments),
            "revit_document_info" => HandleRevitDocumentInfo(),
            _ => throw new McpProtocolException(-32602, $"Unknown tool: {name}"),
        };

        return ToolResult(text);
    }

    static string HandleEcho(JObject arguments)
    {
        var message = arguments["message"]?.Value<string>();
        if (string.IsNullOrEmpty(message))
            throw new McpProtocolException(-32602, "Missing required argument: message");

        return $"hello {message}";
    }

    static string HandleRevitDocumentInfo()
    {
        if (_revitEvent == null || _revitBridge == null)
            return "Revit bridge is not initialized.";

        var result = _revitBridge.RunOnRevitThread(_revitEvent, RevitBridgeTimeoutMs, app =>
        {
            var doc = app.ActiveUIDocument?.Document;
            if (doc == null)
                return "No document open";

            var title = doc.Title;
            var path = string.IsNullOrEmpty(doc.PathName) ? "(unsaved)" : doc.PathName;
            return $"Title: {title}\nPath: {path}";
        });

        return result ?? "Timed out waiting for Revit.";
    }

    static JObject ToolResult(string text, bool isError = false)
        => new()
        {
            ["content"] = new JArray
            {
                new JObject
                {
                    ["type"] = "text",
                    ["text"] = text,
                },
            },
            ["isError"] = isError,
        };

    static bool IsAllowedOrigin(HttpListenerRequest request)
    {
        var origin = request.Headers["Origin"];
        if (string.IsNullOrEmpty(origin))
            return true;

        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
            return false;

        return uri.Host is "localhost" or "127.0.0.1";
    }

    static void WriteJson(HttpListenerResponse response, JToken payload)
    {
        var json = payload.ToString(Newtonsoft.Json.Formatting.None);
        var bytes = Encoding.UTF8.GetBytes(json);
        response.StatusCode = 200;
        response.ContentType = "application/json; charset=utf-8";
        response.ContentLength64 = bytes.Length;
        response.OutputStream.Write(bytes, 0, bytes.Length);
    }

    sealed class RevitBridgeHandler : IExternalEventHandler
    {
        readonly ManualResetEventSlim _done = new(false);
        readonly object _lock = new();
        Func<UIApplication, string> _work;
        string _result;
        Exception _error;

        public string RunOnRevitThread(ExternalEvent ev, int timeoutMs, Func<UIApplication, string> work)
        {
            lock (_lock)
            {
                _work = work;
                _result = null;
                _error = null;
                _done.Reset();
                ev.Raise();
            }

            if (!_done.Wait(timeoutMs))
                return null;

            lock (_lock)
            {
                if (_error != null)
                    throw _error;
                return _result;
            }
        }

        public void Execute(UIApplication app)
        {
            try
            {
                Func<UIApplication, string> work;
                lock (_lock)
                    work = _work;

                _result = work?.Invoke(app);
            }
            catch (Exception ex)
            {
                _error = ex;
            }
            finally
            {
                _done.Set();
            }
        }

        public string GetName()
            => "MCP Revit Bridge";
    }

    sealed class McpProtocolException(int code, string message) : Exception(message)
    {
        public int Code { get; } = code;
    }
}
