using System;
using System.Diagnostics;

namespace Ara3D.Utils
{
    public class ProcessData
    {
        public ProcessData(Process p)
        {
            ExitCode = p.ExitCode;
            HasExited = p.HasExited;
            Responding = p.Responding;
            ExitTime = p.ExitTime;
            MachineName = p.MachineName;
            WindowTitle = p.MainWindowTitle;
            FileName = p.MainModule?.FileName ?? "";
            Id = p.Id;
            PagedMemorySize = p.PagedMemorySize64;
            NonPagedMemorySize = p.NonpagedSystemMemorySize64;
            VirtualMemorySize = p.VirtualMemorySize64;
            PeakVirtualMemorySize = p.PeakVirtualMemorySize64;
            PeakPagedMemorySize = p.PeakPagedMemorySize64;
            WorkingSet = p.WorkingSet64;
            PeakWorkingSet = p.PeakWorkingSet64;
            PrivateMemorySize = p.PrivateMemorySize64;
            FileVersionInfo = p.MainModule?.FileVersionInfo;
            ModuleName = p.MainModule?.ModuleName ?? "";
        }

        public readonly int ExitCode;
        public readonly bool HasExited;
        public readonly bool Responding;
        public readonly DateTimeOffset ExitTime; 
        public readonly string MachineName;
        public readonly string FileName;
        public readonly string WindowTitle;
        public readonly string ModuleName;
        public readonly int Id;
        public readonly long WorkingSet;
        public readonly long PeakWorkingSet;
        public readonly long PagedMemorySize;
        public readonly long NonPagedMemorySize;
        public readonly long PeakPagedMemorySize;
        public readonly long VirtualMemorySize;
        public readonly long PeakVirtualMemorySize;
        public readonly long PrivateMemorySize;
        public readonly FileVersionInfo FileVersionInfo;
    }
}