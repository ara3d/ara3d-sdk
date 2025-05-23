using System;
using System.Collections.Generic;
using Ara3D.Utils;

namespace Ara3D.ScriptService
{
    public class ScriptingDataModel
    {
        public IReadOnlyList<string> TypeNames = Array.Empty<string>();
        public IReadOnlyList<FilePath> Files = Array.Empty<FilePath>();
        public IReadOnlyList<string> Assemblies = Array.Empty<string>();
        public IReadOnlyList<string> Diagnostics = Array.Empty<string>();
        public FilePath Dll = "";
        public DirectoryPath Directory = "";
        public ScriptingOptions Options = new ScriptingOptions();
        public bool ParseSuccess;
        public bool EmitSuccess;
        public bool LoadSuccess;
    }
}