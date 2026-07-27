using Ara3D.Logging;
using Ara3D.Utils;

namespace Ara3D.Bowerbird;

/// <summary>
/// Scans a commands root for valid per-folder manifests (no compilation).
/// </summary>
public class CommandCatalog
{
    public DirectoryPath CommandsRoot { get; }
    public IReadOnlyList<CommandDescriptor> Commands { get; }
    public ILogger Logger { get; }

    public CommandCatalog(DirectoryPath commandsRoot, ILogger logger = null)
        => (CommandsRoot, Logger, Commands) = (commandsRoot, logger, Scan(commandsRoot, logger).ToList());

    static IEnumerable<CommandDescriptor> Scan(DirectoryPath commandsRoot, ILogger logger)
    {
        if (!commandsRoot.Exists())
        {
            logger?.Log($"Commands root does not exist: {commandsRoot}");
            yield break;
        }

        foreach (var folder in commandsRoot.GetSubFolders().OrderBy(f => f.GetFolderName()))
        {
            if (!CommandManifestReader.TryRead(folder, out var manifest, out var error))
            {
                // A folder without a manifest is simply not a command folder (see tracker
                // issue studio-037); only an invalid manifest is worth reporting.
                if (CommandManifestReader.GetManifestPath(folder).Exists())
                    logger?.Log($"Skipping {folder}: {error}");
                continue;
            }

            var sourceFiles = folder.GetFiles("*.cs").OrderBy(f => f.GetFileName()).ToList();
            var manifestPath = CommandManifestReader.GetManifestPath(folder);
            yield return new CommandDescriptor(folder, manifest, sourceFiles, manifestPath);
        }
    }

    public CommandDescriptor FindByDisplayName(string displayName)
        => Commands.FirstOrDefault(c =>
            string.Equals(c.DisplayName, displayName, StringComparison.OrdinalIgnoreCase));

    public CommandDescriptor FindByFolderName(string folderName)
        => Commands.FirstOrDefault(c =>
            string.Equals(c.FolderName, folderName, StringComparison.OrdinalIgnoreCase));

    public CommandDescriptor ResolveByName(string name)
        => FindByDisplayName(name) ?? FindByFolderName(name);
}
