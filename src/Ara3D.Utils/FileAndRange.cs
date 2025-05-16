using System.Text.RegularExpressions;

namespace Ara3D.Utils
{

    /// <summary>
    /// Used to indicate a specific file and range, for reporting diagnostics.
    /// Similar to how visual studio parses the output.
    /// </summary>
    public class FileAndRange
    {
        public FilePath FilePath { get; }
        public int StartIndex { get; }
        public int EndIndex { get; }

        public FileAndRange(FilePath filePath, int startIndex, int endIndex)
            => (FilePath, StartIndex, EndIndex) = (filePath, startIndex, endIndex);

        public static readonly Regex RangeRegex = new Regex(@"\((\d+)..(\d+)\)");

        /// <summary>
        /// Looks for a pattern [filepath]([start line], [start column], [end line], [end column]) 
        /// The file path is assumed to start with a single letter indicating a drive and then a ":" character.
        /// </summary>
        public static FileAndRange Parse(string input)
        {
            var match = RangeRegex.Match(input);
            if (!match.Success)
                return null;

            var index = match.Index;
            var subStr = input.Substring(0, index);
            var driveIndicator = subStr.LastIndexOf(':') - 1;
            FilePath fp = null;
            if (driveIndicator >= 0)
            {
                fp = subStr.Substring(driveIndicator);
            }

            Verifier.Assert(match.Groups.Count >= 2);
            int.TryParse(match.Groups[1].Value, out var startIndex);
            int.TryParse(match.Groups[2].Value, out var endIndex);

            return new FileAndRange(fp, startIndex, endIndex);
        }

        public override string ToString()
        {
            return $"{FilePath.GetFullPath()}({StartIndex}..{EndIndex})";
        }
    }
}