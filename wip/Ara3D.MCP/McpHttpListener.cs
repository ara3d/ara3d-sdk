using System.Diagnostics;
using System.Net;
using System.Text;

namespace Ara3D.MCP;

internal sealed class McpHttpListener : IDisposable
{
    private readonly McpJsonRpcHandler _handler;
    private readonly HttpListener _listener;
    private readonly Thread _listenerThread;
    private readonly CancellationTokenSource _cts = new();
    private readonly string _mcpPath;

    public McpHttpListener(McpJsonRpcHandler handler, int port, string host, string mcpPath)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _mcpPath = mcpPath ?? throw new ArgumentNullException(nameof(mcpPath));

        var prefix = $"http://{host}:{port}/";
        _listener = new HttpListener();
        _listener.Prefixes.Add(prefix);
        _listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
        _listener.Start();

        _listenerThread = new Thread(StartListener)
        {
            IsBackground = true,
            Name = "Ara3D MCP HTTP"
        };
    }

    public bool Active => _listener.IsListening && _listenerThread.IsAlive;

    public void Start()
    {
        if (!_listenerThread.IsAlive)
            _listenerThread.Start();
    }

    public void Stop()
    {
        if (_cts.IsCancellationRequested)
            return;

        _cts.Cancel();

        try
        {
            if (_listener.IsListening)
                _listener.Close();
        }
        catch
        {
            // Ignore shutdown errors.
        }
    }

    private void StartListener()
    {
        while (!_cts.IsCancellationRequested)
        {
            try
            {
                HandleContext(_listener.GetContext());
            }
            catch (HttpListenerException)
            {
                return;
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }

    private void HandleContext(HttpListenerContext context)
    {
        try
        {
            var request = context.Request;
            var response = context.Response;

            if (!IsAllowedOrigin(request))
            {
                response.StatusCode = 403;
                return;
            }

            var path = request.Url?.AbsolutePath ?? "";
            if (!path.Equals(_mcpPath, StringComparison.OrdinalIgnoreCase))
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

            var result = _handler.HandlePost(body);
            response.StatusCode = result.StatusCode;

            if (result.JsonBody == null)
                return;

            WriteJson(response, result.JsonBody);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);

            try
            {
                context.Response.StatusCode = 500;
            }
            catch
            {
                // Ignore response errors.
            }
        }
        finally
        {
            try
            {
                context.Response.Close();
            }
            catch
            {
                // Ignore close errors.
            }
        }
    }

    private static bool IsAllowedOrigin(HttpListenerRequest request)
    {
        var origin = request.Headers["Origin"];
        if (string.IsNullOrEmpty(origin))
            return true;

        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
            return false;

        return uri.Host is "localhost" or "127.0.0.1";
    }

    private static void WriteJson(HttpListenerResponse response, string json)
    {
        var bytes = Encoding.UTF8.GetBytes(json);
        response.ContentType = "application/json; charset=utf-8";
        response.ContentLength64 = bytes.Length;
        response.OutputStream.Write(bytes, 0, bytes.Length);
    }

    public void Dispose()
    {
        Stop();
        _cts.Dispose();
    }
}
