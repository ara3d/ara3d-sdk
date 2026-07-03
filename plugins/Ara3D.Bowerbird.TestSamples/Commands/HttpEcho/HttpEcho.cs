using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Ara3D.Bowerbird;
using Ara3D.Utils;

namespace Ara3D.Bowerbird.TestSamples.HttpEcho;

public class HttpEchoCommand : NamedCommand
{
    public const int Port = 8765;
    public const string ResponseBody = "Hello from HttpEcho";

    public WebServer Server { get; private set; }

    public override void Execute()
    {
        Server = new WebServer(Callback, Port);
        Server.Start();
        SpinWait.SpinUntil(() => Server.Active, TimeSpan.FromSeconds(2));
        Console.WriteLine($"HttpEcho listening at {Server.Uri}");
    }

    void Callback(string verb, string path, IDictionary<string, string> parameters, Stream inputStream, Stream outputStream)
    {
        var bytes = Encoding.UTF8.GetBytes(ResponseBody);
        outputStream.Write(bytes, 0, bytes.Length);
        outputStream.Flush();
    }
}
