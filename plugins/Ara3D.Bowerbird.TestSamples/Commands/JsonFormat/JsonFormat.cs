using System;
using System.Text.Json;
using Ara3D.Bowerbird;

namespace Ara3D.Bowerbird.TestSamples.JsonFormat;

public class JsonFormatCommand : NamedCommand
{
    public override void Execute()
    {
        var payload = new SamplePayload("Bowerbird", 42, new[] { "compile", "run" });
        Console.WriteLine(JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
    }

    record SamplePayload(string Name, int Count, string[] Tags);
}
