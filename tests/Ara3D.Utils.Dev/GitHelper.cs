using System.Diagnostics;

namespace Ara3D.Utils.Dev
{

    public static class GitHelper
    {
        // https://stackoverflow.com/questions/48421697/get-name-of-branch-into-code

        public static string CommitHash()
            => GitRunner("rev-parse --short HEAD");

        public static string CommitHashLong()
            => GitRunner("rev-parse HEAD");

        public static string BranchName()
            => GitRunner("rev-parse --abbrev-ref HEAD");

        public static string Remote()
            => GitRunner("config --get remote.origin.url");

        public static string GitRunner(string args)
            => ReadOneLineAndQuit("git", args);

        public static string ReadOneLineAndQuit(string processName, string args)
            => new ProcessStartInfo(processName)
                {
                    UseShellExecute = false,
                    Arguments = args
                }
                .ReadOneLineAndQuit();

        public static string GitLocation()
            => ReadOneLineAndQuit("where", "git.exe");

        public static string ReadOneLineAndQuit(this ProcessStartInfo psi)
        {
            psi.RedirectStandardOutput = true;
            using (var p = new Process { StartInfo = psi })
            {
                p.Start();
                return p.StandardOutput.ReadLine() ?? "";
            }
        }
    }
}