using System;

namespace Ara3D.Utils
{
    /// <summary>
    /// Three strings, used to identify a particular application version.
    /// Useful for determining common folders on the file system.
    /// See "ApplicationFolders".
    /// </summary>
    public class ApplicationData
    {
        public readonly string OrgName;
        public readonly string AppName;
        public readonly Version AppVersion;

        public ApplicationData(string orgName, string appName, Version appVersion)
            => (OrgName, AppName, AppVersion) = (orgName, appName, appVersion);
    }
}