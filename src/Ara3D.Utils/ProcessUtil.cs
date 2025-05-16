using System;
using System.Diagnostics;
using System.IO;

namespace Ara3D.Utils
{

    // https://stackoverflow.com/questions/5762526/how-can-i-make-something-that-catches-all-unhandled-exceptions-in-a-winforms-a

    public static class ProcessUtil
    {
        public static Process Current
            => Process.GetCurrentProcess();

        public static void SetExitCallback(Action<object, EventArgs> handler)
            => Current.SetExitCallback(handler);

        public static void SetExitCallback(this Process p, Action<object, EventArgs> handler)
        {
            p.EnableRaisingEvents = true;
            p.Exited += (sender, args) => handler(sender, args);
            //AppDomain.CurrentDomain.ProcessExit += (sender, args) => handler(sender, args);
        }

        public static ProcessData ToProcessData(this Process process)
            => new ProcessData(process);

        public static Process OpenFolderInExplorer(this DirectoryPath folderPath)
            => Process.Start("explorer.exe", folderPath);

        public static Process SelectFileInExplorer(this FilePath filePath)
            => Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"/select,\"{filePath}\"",
                UseShellExecute = false
            });

        public static Process ShellExecute(this FilePath filePath)
            => Process.Start(new ProcessStartInfo { FileName = filePath, UseShellExecute = true });

        public static Process OpenDefaultProcess(this FilePath filePath)
            => Process.Start(filePath);

        public static Process OpenFile(this FilePath filePath)
        {
            if (!filePath.Exists())
                throw new FileNotFoundException("", filePath);

            // Expand the file name
            filePath = filePath.GetFullPath();

            // Open the file with the default file extension handler.
            try
            {
                return filePath.OpenDefaultProcess();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }

            // If there is no default file extension handler, use shell execute
            try
            {
                return filePath.ShellExecute();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
            }

            // If that didn't work, show the file in explorer.
            return filePath.SelectFileInExplorer();
        }

        /// <summary>
        /// Closes a process if it isn't null and hasn't already exited.
        /// </summary>
        public static void SafeClose(this Process process)
        {
            if (process != null && !process.HasExited)
                process.CloseMainWindow();
        }

        public static string ReadOneLine(this ProcessStartInfo psi)
        {
            psi.RedirectStandardOutput = true;
            using (var p = new Process { StartInfo = psi })
            {
                p.Start();
                return p.StandardOutput.ReadLine() ?? "";
            }
        }

        public static void SetProcessExitCallback(Action<object, EventArgs> handler)
            => Process.GetCurrentProcess().Exited += (sender, args) => handler(sender, args);

        public static void SetProcessExitCallbackCurrentDomain(Action<object, EventArgs> handler)
            => AppDomain.CurrentDomain.ProcessExit += (sender, args) => handler(sender, args);

        // https://stackoverflow.com/questions/7693429/process-start-to-open-an-url-getting-an-exception
        public static void OpenUrl(string url)
            => ShellExecute(url);
    }
}
