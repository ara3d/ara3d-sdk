using Ara3D.Utils;

namespace Ara3D.Bowerbird;

/// <summary>
/// Host configuration: commands root, shared libraries folder, and optional dir.txt redirect.
/// </summary>
public class BowerbirdOptions
{
    public string AppName { get; }
    public DirectoryPath CommandsRoot { get; }
    public DirectoryPath LibrariesFolder { get; }
    public bool EnableCompileCache { get; init; } = true;

    public BowerbirdOptions(string appName, DirectoryPath commandsRoot)
        : this(appName, commandsRoot, "") { }

    public BowerbirdOptions(string appName, DirectoryPath commandsRoot, DirectoryPath librariesFolder)
    {
        AppName = appName;
        CommandsRoot = ResolveRedirect(commandsRoot);
        LibrariesFolder = librariesFolder;
    }

    static DirectoryPath ResolveRedirect(DirectoryPath commandsRoot)
    {
        if (!commandsRoot.Exists())
            throw new Exception($"Commands root does not exist: {commandsRoot}");

        var dirFile = commandsRoot.RelativeFile("dir.txt");
        if (!dirFile.Exists())
            return commandsRoot;

        try
        {
            var redirect = new DirectoryPath(dirFile.ReadAllText().Trim());
            if (redirect.Exists())
                return redirect;
        }
        catch
        {
            // Fall through to the configured path.
        }

        return commandsRoot;
    }
}
