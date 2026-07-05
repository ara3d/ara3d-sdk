using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace Ara3D.Utils
{
    /// <summary>
    /// A simple local HTTP server based on HttpListener.
    /// Intended for local/demo/plugin use, not production hosting.
    /// </summary>
    public sealed class SimpleHttpServer : IDisposable
    {
        public delegate void RequestHandler(HttpListenerContext context);

        private readonly RequestHandler _handler;
        private readonly HttpListener _listener;
        private readonly Thread _listenerThread;
        private readonly CancellationTokenSource _cts = new();

        public string Uri { get; }

        public bool Active => _listener.IsListening && _listenerThread.IsAlive;

        public delegate void CallBackDelegate(
            string verb,
            string path,
            IDictionary<string, string> parameters,
            Stream inputStream,
            Stream outputStream);

        public SimpleHttpServer(CallBackDelegate callback, int port = 3000, string host = "127.0.0.1")
            : this(AdaptLegacyCallback(callback), port, host)
        { }

        public SimpleHttpServer(RequestHandler handler, int port = 3000, string host = "127.0.0.1")
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));

            Uri = $"http://{host}:{port}/";

            _listener = new HttpListener();
            _listener.Prefixes.Add(Uri);
            _listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            _listener.Start();

            _listenerThread = new Thread(StartListener)
            {
                IsBackground = true,
                Name = "Ara3D Local WebServer"
            };
        }

        public void Start()
        {
            if (!_listenerThread.IsAlive)
                _listenerThread.Start();

            Debug.WriteLine($"Server started: {Uri}");
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
                    var context = _listener.GetContext();

                    // For a simple demo server, handle one request at a time.
                    // If needed, this can be changed to ThreadPool.QueueUserWorkItem.
                    HandleContext(context);
                }
                catch (HttpListenerException)
                {
                    // Normal during shutdown.
                    return;
                }
                catch (ObjectDisposedException)
                {
                    // Normal during shutdown.
                    return;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }

        private void HandleContext(HttpListenerContext context)
        {
            try
            {
                _handler(context);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);

                try
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    using var writer = new StreamWriter(context.Response.OutputStream);
                    writer.Write("Internal server error");
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

        public void Dispose()
        {
            Stop();
            _cts.Dispose();
        }

        public static RequestHandler AdaptLegacyCallback(CallBackDelegate callback)
        {
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));

            return context =>
            {
                var request = context.Request;
                var response = context.Response;

                var parameters = new Dictionary<string, string>();

                foreach (string? key in request.QueryString.Keys)
                {
                    if (key == null)
                        continue;

                    parameters[key] = request.QueryString[key] ?? "";
                }

                var path = request.Url?.LocalPath ?? "";
                path = path.TrimStart('/');

                if (path.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
                    response.ContentType = "text/javascript; charset=utf-8";

                callback(
                    request.HttpMethod,
                    path,
                    parameters,
                    request.InputStream,
                    response.OutputStream);
            };
        }
    }
}