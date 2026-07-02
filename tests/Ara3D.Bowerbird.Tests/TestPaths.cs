using Ara3D.Bowerbird;
using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.Bowerbird.Tests;

public static class TestPaths
{
    public static DirectoryPath TestSamplesCommands
    {
        get
        {
            var start = new FilePath(typeof(TestPaths).Assembly.Location).GetDirectory();
            foreach (var dir in start.GetSelfAndAncestors())
            {
                var candidate = dir.RelativeFolder(@"ext\Ara3D.Bowerbird.TestSamples\Commands");
                if (candidate.Exists())
                    return candidate;
            }

            throw new DirectoryNotFoundException("Could not find TestSamples Commands folder from test assembly location.");
        }
    }
}

public static class TestCommandsRoot
{
    public static DirectoryPath Create(Action<DirectoryPath> setup)
    {
        var root = SpecialFolders.Temp.RelativeFolder($"BowerbirdTests_{Guid.NewGuid():N}");
        root.Create();
        setup(root);
        return root;
    }

    public static void WriteCommand(DirectoryPath root, string folderName, string displayName, string typeName, string sourceBody)
    {
        var folder = root.RelativeFolder(folderName);
        folder.Create();
        folder.RelativeFile($"{folderName}.manifest.json").WriteAllText($$"""
            {
              "displayName": "{{displayName}}",
              "typeName": "{{typeName}}"
            }
            """);
        folder.RelativeFile($"{folderName}.cs").WriteAllText(sourceBody);
    }
}
