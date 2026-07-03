using System;
using Ara3D.Bowerbird;
using Ara3D.Utils;

namespace Ara3D.Bowerbird.TestSamples.TempFile;

public class TempFileCommand : NamedCommand
{
    const string Content = "Bowerbird temp file sample";

    public override void Execute()
    {
        var path = SpecialFolders.Temp.RelativeFile($"bowerbird_temp_{Guid.NewGuid():N}.txt");
        path.WriteAllText(Content);
        var readBack = path.ReadAllText();
        Console.WriteLine($"Path: {path}");
        Console.WriteLine($"Content: {readBack}");
        path.Delete();
    }
}
