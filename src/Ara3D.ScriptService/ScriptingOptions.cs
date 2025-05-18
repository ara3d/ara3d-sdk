using Ara3D.Utils;

namespace Ara3D.ScriptService
{
    public class ScriptingOptions
    {
        public string AppName { get; set; }
        public string OrgName { get; set; }
        public FilePath ConfigFile { get; set; }
        public DirectoryPath ScriptsFolder { get; set; }
        public DirectoryPath LibrariesFolder { get; set; }
        
        public static ScriptingOptions CreateFromName(string appName)
            => CreateFromName("Ara 3D", appName);

        public static ScriptingOptions CreateFromName(string orgName, string appName)
        {
            var appData = SpecialFolders.LocalApplicationData;
            return new ScriptingOptions()
            {
                OrgName = orgName,
                AppName = appName,
                ConfigFile = appData.RelativeFile(orgName, appName, "config.json"),
                ScriptsFolder = appData.RelativeFolder(orgName, appName, "Scripts"),
                LibrariesFolder = appData.RelativeFolder(orgName, appName, "Libraries"),
            };
        }
    }
}