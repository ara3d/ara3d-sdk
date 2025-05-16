using System;
using System.IO;

namespace Ara3D.Utils
{
    // https://learn.microsoft.com/en-us/previous-versions/visualstudio/visual-studio-2010/s2esdf4x(v=vs.100)?redirectedfrom=MSDN
    public static class SpecialFolders
    {
        public static DirectoryPath AdminTools => Environment.SpecialFolder.AdminTools.GetDir();
        public static DirectoryPath ApplicationData => Environment.SpecialFolder.ApplicationData.GetDir();
        public static DirectoryPath CDBurning => Environment.SpecialFolder.CDBurning.GetDir();
        public static DirectoryPath Windows => Environment.SpecialFolder.Windows.GetDir();
        public static DirectoryPath UserProfile => Environment.SpecialFolder.UserProfile.GetDir();
        public static DirectoryPath Templates => Environment.SpecialFolder.Templates.GetDir();
        public static DirectoryPath SystemX86 => Environment.SpecialFolder.SystemX86.GetDir();
        public static DirectoryPath System => Environment.SpecialFolder.System.GetDir();
        public static DirectoryPath StartMenu => Environment.SpecialFolder.StartMenu.GetDir();
        public static DirectoryPath SendTo => Environment.SpecialFolder.SendTo.GetDir();
        public static DirectoryPath Resources => Environment.SpecialFolder.Resources.GetDir();
        public static DirectoryPath Recent => Environment.SpecialFolder.Recent.GetDir();
        public static DirectoryPath Programs => Environment.SpecialFolder.Programs.GetDir();
        public static DirectoryPath ProgramFilesX86 => Environment.SpecialFolder.ProgramFilesX86.GetDir();
        public static DirectoryPath ProgramFiles => Environment.SpecialFolder.ProgramFiles.GetDir();
        public static DirectoryPath PrinterShortcuts => Environment.SpecialFolder.PrinterShortcuts.GetDir();
        public static DirectoryPath Personal => Environment.SpecialFolder.Personal.GetDir();
        public static DirectoryPath NetworkShortcuts => Environment.SpecialFolder.NetworkShortcuts.GetDir();
        public static DirectoryPath MyVideos => Environment.SpecialFolder.MyVideos.GetDir();
        public static DirectoryPath MyPictures => Environment.SpecialFolder.MyPictures.GetDir();
        public static DirectoryPath MyDocuments => Environment.SpecialFolder.MyDocuments.GetDir();
        public static DirectoryPath LocalizedResources => Environment.SpecialFolder.LocalizedResources.GetDir();
        public static DirectoryPath LocalApplicationData => Environment.SpecialFolder.LocalApplicationData.GetDir();
        public static DirectoryPath InternetCache => Environment.SpecialFolder.InternetCache.GetDir();
        public static DirectoryPath History => Environment.SpecialFolder.History.GetDir();
        public static DirectoryPath Fonts => Environment.SpecialFolder.Fonts.GetDir();
        public static DirectoryPath Favorites => Environment.SpecialFolder.Favorites.GetDir();
        public static DirectoryPath DesktopDirectory => Environment.SpecialFolder.DesktopDirectory.GetDir();
        public static DirectoryPath Desktop => Environment.SpecialFolder.Desktop.GetDir();
        public static DirectoryPath Cookies => Environment.SpecialFolder.Cookies.GetDir();
        public static DirectoryPath CommonVideos => Environment.SpecialFolder.CommonVideos.GetDir();
        public static DirectoryPath CommonTemplates => Environment.SpecialFolder.CommonTemplates.GetDir();
        public static DirectoryPath CommonStartup => Environment.SpecialFolder.CommonStartup.GetDir();
        public static DirectoryPath CommonStartMenu => Environment.SpecialFolder.CommonStartMenu.GetDir();
        public static DirectoryPath CommonPrograms => Environment.SpecialFolder.CommonPrograms.GetDir();
        public static DirectoryPath CommonProgramFilesX86 => Environment.SpecialFolder.CommonProgramFilesX86.GetDir();
        public static DirectoryPath CommonProgramFiles => Environment.SpecialFolder.CommonProgramFiles.GetDir();
        public static DirectoryPath CommonPictures => Environment.SpecialFolder.CommonPictures.GetDir();
        public static DirectoryPath CommonOemLinks => Environment.SpecialFolder.CommonOemLinks.GetDir();
        public static DirectoryPath CommonMusic => Environment.SpecialFolder.CommonMusic.GetDir();
        public static DirectoryPath CommonDocuments => Environment.SpecialFolder.CommonDocuments.GetDir();
        public static DirectoryPath CommonDesktopDirectory => Environment.SpecialFolder.CommonDesktopDirectory.GetDir();
        public static DirectoryPath CommonApplicationData => Environment.SpecialFolder.CommonApplicationData.GetDir();
        public static DirectoryPath CommonAdminTools => Environment.SpecialFolder.CommonAdminTools.GetDir();
            
        public static DirectoryPath Temp => Path.GetTempPath();

        public static DirectoryPath GetDir(this Environment.SpecialFolder folder)
            => Environment.GetFolderPath(folder);
    }
}