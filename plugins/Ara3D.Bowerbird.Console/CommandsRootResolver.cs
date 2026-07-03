using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.Bowerbird.Console;

/// <summary>
/// Finds ext/Ara3D.Bowerbird.TestSamples/Commands relative to the repo or assembly.
/// </summary>
public static class CommandsRootResolver
{
    public const string DefaultRelativePath = @"ext\Ara3D.Bowerbird.TestSamples\Commands";

    public static DirectoryPath Resolve(string overridePath = null)
    {
        if (!overridePath.IsNullOrWhiteSpace())
            return new DirectoryPath(overridePath);

        foreach (var start in CandidateStarts())
        {
            var found = start.GetSelfAndAncestors()
                .Select(p => p.RelativeFolder(DefaultRelativePath))
                .FirstOrDefault(p => p.Exists());
            if (found.Exists())
                return found;
        }

        throw new DirectoryNotFoundException(
            $"Could not find commands root. Pass --commands <path> or run from the repo. Expected: {DefaultRelativePath}");
    }

    static IEnumerable<DirectoryPath> CandidateStarts()
    {
        yield return new DirectoryPath(Environment.CurrentDirectory);
        var assemblyDir = new FilePath(typeof(CommandsRootResolver).Assembly.Location).GetDirectory();
        yield return assemblyDir;
        yield return assemblyDir.Up();
        yield return assemblyDir.Up().Up();
        yield return assemblyDir.Up().Up().Up();
    }
}
