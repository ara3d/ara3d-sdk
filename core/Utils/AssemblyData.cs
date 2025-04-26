using System;
using System.Diagnostics;
using System.Reflection;

namespace Ara3D.Utils
{
    /// <summary>
    /// Contains useful information describing an assembly. 
    /// </summary>
    public class AssemblyData
    {
        public AssemblyData(Assembly asm)
        {
            EntryPoint = asm.EntryPoint?.Name ?? "";
            FullName = asm.FullName ?? "";
            ImageRuntimeVersion = asm.ImageRuntimeVersion;
            IsFullyTrusted = asm.IsFullyTrusted;
            Location = asm.Location;
            Architecture = asm.GetName().ProcessorArchitecture.ToString();
            Version = asm.GetName().Version ?? new Version();
            ShortName = asm.GetName().Name ?? "";
            CodeBase = asm.GetName().CodeBase ?? "";
        }

        public readonly string EntryPoint;
        public readonly string FullName;
        public readonly string ImageRuntimeVersion;
        public readonly bool IsFullyTrusted;
        public readonly FilePath Location;
        public readonly string Architecture;
        public readonly Version Version;
        public readonly string ShortName;
        public readonly string CodeBase;
        public FileVersionInfo FileVersionInfo => Location.GetVersionInfo();
        public DirectoryPath LocationDir => Location.GetDirectory();

        public static AssemblyData Current => 
            new AssemblyData(Assembly.GetExecutingAssembly());
    }
}