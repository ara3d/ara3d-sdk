using System;
using System.Diagnostics;
using System.Reflection;

namespace Ara3D.Utils
{
    public class SystemInfo
    {
        public string CommandLine { get; set; } = Environment.CommandLine;
        public string CurrentWorkingDirectory { get; set; } = Environment.CurrentDirectory;
        public bool Is64BitOperatingSystem { get; set; } = Environment.Is64BitOperatingSystem;
        public bool Is64BitProcess { get; set; } = Environment.Is64BitProcess;
        public string MachineName { get; set; } = Environment.MachineName;
        public OperatingSystem OSVersion { get; set; } = Environment.OSVersion;

        //TODO
        //public string ProcessPath { get; set; } = Environment.ProcessPath ?? "";
        //public int ProcessId { get; set; } = Environment.ProcessId;
        
        public int ProcessorCount { get; set; } = Environment.ProcessorCount;
        public string SystemDirectory { get; set; } = Environment.SystemDirectory;
        
        public long MSecSinceSystemStarted { get; set; } = Environment.TickCount;
        
        public bool UserInteractiveMode { get; set; } = Environment.UserInteractive;
        public Version CommonLanguageRuntime { get; set; } = Environment.Version;
        public long MappedPhysicalRAM { get; set; } = Environment.WorkingSet;

        public string AppDomainBaseDirectory { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
        public string AppDomainDynamicDirectory { get; set; } = AppDomain.CurrentDomain.DynamicDirectory ?? "";
        public long AppDominSurvivedMemorySize { get; set; } = AppDomain.CurrentDomain.MonitoringSurvivedMemorySize;
        public TimeSpan AppDoainTotalProcessTime { get; set; } = AppDomain.CurrentDomain.MonitoringTotalProcessorTime;
        public string AppDomainFriendlyName { get; set; } = AppDomain.CurrentDomain.FriendlyName;

        public bool IsDebuggerAttached { get; set; } = Debugger.IsAttached;

        public ProcessData CurrentProcess => Process.GetCurrentProcess().ToProcessData();

        public AssemblyData EntryAssembly { get; set; } = Assembly.GetEntryAssembly().ToAssemblyData();
        public AssemblyData ExecutingAssembly { get; set; } = Assembly.GetExecutingAssembly().ToAssemblyData();
    }
}