using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Ara3D.ScriptService;
using Ara3D.Studio.API;
using Ara3D.Utils;

/// <summary>
/// Shows a message box with the text: "Hello world!"
/// </summary>
public class HelloWorld : IScriptedCommand
{
    public string Name => "Hello World!";

    public void Execute()
        => MessageBox.Show(Name);
}

/// <summary>
/// Shows a message box with a counter value which is incremented each time. 
/// </summary>
public class Counter : IScriptedCommand
{
    public static int Count;

    public string Name => "Count";

    public void Execute()
        => MessageBox.Show($"You have executed this command {++Count} time(s)");
}

/// <summary>
/// Opens a URL in the default web browser. 
/// </summary>
public class OpenUrl : IScriptedCommand
{
    public string Name => "Open URL";

    public void Execute()
        => ProcessUtil.OpenUrl("https://ara3d.com");
}

/// <summary>
/// Triggers the debugger to break. 
/// </summary>
public class LaunchDebugger : IScriptedCommand
{
    public string Name => "Launch Debugger";

    public void Execute()
        => Debugger.Break();
}

/// <summary>
/// Launches a simple web-server, returns a text response, and shuts down the web-server. 
/// </summary>
public class HttpServer : IScriptedCommand
{
    public string Name => "HTTP Server";
    public WebServer Server
    {
        get;
        private set;
    }

    public void Execute()
    {
        Server = new WebServer(Callback);
        Server.Start();
        ProcessUtil.OpenUrl(Server.Uri);
    }

    private void Callback(string verb, string path, IDictionary<string, string> parameters, Stream inputstream, Stream outputstream)
    {
        using (var writer = new StreamWriter(outputstream))
        {
            writer.WriteLine($"Hello, thanks for using the HTTP server. I will shut myself down now.");
        }
        Server.Stop();
    }
}
