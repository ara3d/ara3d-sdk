using System.IO;

namespace Ara3D.Utils
{
    /// <summary>
    /// Given an application 
    /// </summary>
    public class ApplicationFolders
    {
        private readonly ApplicationData _appData;

        public string RelativeFolder
            => Path.Combine(_appData.OrgName, _appData.AppName);

        public ApplicationFolders(ApplicationData appData)
            => _appData = appData;

        public DirectoryPath ApplicationData => SpecialFolders.ApplicationData.RelativeFolder(RelativeFolder);
        public DirectoryPath CommonApplicationData => SpecialFolders.CommonApplicationData.RelativeFolder(RelativeFolder);
        public DirectoryPath ProgramFiles => SpecialFolders.ProgramFiles.RelativeFolder(RelativeFolder);
        public DirectoryPath Documents => SpecialFolders.MyDocuments.RelativeFolder(RelativeFolder);
        public DirectoryPath Temp => SpecialFolders.Temp.RelativeFolder(RelativeFolder);
    }
}