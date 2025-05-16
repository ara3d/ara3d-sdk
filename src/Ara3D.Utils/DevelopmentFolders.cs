using System.Runtime.CompilerServices;

namespace Ara3D.Utils
{
    public static class SourceCodeLocation
    {
        public static DirectoryPath GetFolder([CallerFilePath] string filePath = "")
            => new FilePath(filePath).GetDirectory();

        public static FilePath GetFile([CallerFilePath] string filePath = "")
            => filePath;

        public static FilePath GetCsProjFile([CallerFilePath] string filePath = "")
            => new FilePath(filePath).GetDirectory().FindFirstInAncestor("*.csproj");
                
        public static FilePath GetSolutionFile([CallerFilePath] string filePath = "")
            => new FilePath(filePath).GetDirectory().FindFirstInAncestor("*.sln");

    }
}