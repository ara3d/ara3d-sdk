    using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace Ara3D.Utils
{
    /// <summary>
    /// A simple web-server. 
    /// </summary>
    public class WebServer
    {
        public delegate void CallBackDelegate(
            string verb, 
            string path, 
            IDictionary<string, string> parameters, 
            Stream inputStream,
            Stream outputStream);

        private CallBackDelegate _callback;
        private HttpListener _listener;
        private Thread _listenerThread;

        public CancellationTokenSource CancellationTokenSource { get; }
        public CancellationToken Token { get; }
        
        public string Uri { get; }

        public WebServer(CallBackDelegate callback, int port = 8074)
        {
            CancellationTokenSource = new CancellationTokenSource();
            Token = CancellationTokenSource.Token;

            Uri = $"http://localhost:{port}/";
            _callback = callback;
            _listener = new HttpListener();
            _listener.Prefixes.Add(Uri);
            _listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            _listener.Start();
            _listenerThread = new Thread(StartListener);
        }

        public void Start()
        {
            _listenerThread.Start();
            Debug.WriteLine("Server Started");
        }

        public bool Active
            => _listenerThread.IsAlive;

        public void Stop()
        {
            CancellationTokenSource.Cancel();
        }
        
        private void StartListener()
        {
            while (true)
            {
                var result = _listener.BeginGetContext(ListenerCallback, _listener);

                // Every X msec, check for a cancelation request
                while (!result.AsyncWaitHandle.WaitOne(100))
                {
                    if (Token.IsCancellationRequested)
                    {
                        _listener.Close();
                        return;
                    }
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public void ProcessQuery(HttpListenerRequest request, HttpListenerResponse response)
        {
            var parameters = new Dictionary<string, string>();
            foreach (string key in request.QueryString.Keys)
                parameters.Add(key, request.QueryString[key]);
            var path = request.Url.LocalPath.Substring(1);
            path = path.TrimStart('/');
            if (path.EndsWith(".js"))
                response.ContentType = "text/javascript";
            _callback?.Invoke(request.HttpMethod, path
                , parameters, request.InputStream, response.OutputStream);
        }

        private void ListenerCallback(IAsyncResult result)
        {
            try
            {
                var context = _listener.EndGetContext(result);
                ProcessQuery(context.Request, context.Response);
                context.Response.Close();
            }
            catch (Exception e)
            {
                // An exception will be normal when the server is stopped .
                Debug.WriteLine(e);
            }
        }
    }
}
