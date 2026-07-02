using Ara3D.Bowerbird;
using Ara3D.Utils;

namespace Ara3D.Bowerbird.TestSamples.HttpServer;

public class HttpServerCommand : NamedCommand
{
    public WebServer Server { get; private set; }

    public override void Execute()
    {
        Server = new WebServer(Callback);
        Server.Start();
        ProcessUtil.OpenUrl(Server.Uri);
    }

    void Callback(string verb, string path, IDictionary<string, string> parameters, Stream inputStream, Stream outputStream)
    {
        using var writer = new StreamWriter(outputStream);
        writer.WriteLine("Hello, thanks for using the HTTP server. I will shut myself down now.");
        Server.Stop();
    }
}
