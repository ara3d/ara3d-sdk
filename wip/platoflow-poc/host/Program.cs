using Ara3D.Utils;

namespace PlatoFlow.Host;

public static class Program
{
    public const int DefaultPort = 5214;

    public static int Main(string[] args)
    {
        var port = args.Length > 0 && int.TryParse(args[0], out var parsed) ? parsed : DefaultPort;

        try
        {
            var root = PocPaths.FindRoot();
            Console.WriteLine($"platoflow-poc root: {root}");

            using var catalog = DataSetup.Prepare(root);
            var api = new HostApi(catalog, new AgentBridge());

            using var server = new SimpleHttpServer(api.Handle, port);
            server.Start();

            Console.WriteLine($"listening on http://127.0.0.1:{port}/");
            Console.WriteLine($"models: {string.Join(", ", catalog.Models.Select(m => m.Id))}");
            Console.WriteLine("Ctrl+C to stop.");

            var stop = new ManualResetEventSlim();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                stop.Set();
            };
            stop.Wait();
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }
}
