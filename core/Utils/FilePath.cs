namespace Ara3D.Utils
{
    public class FilePath
    {
        public string Value { get; }
        public FilePath(string path) => Value = path;
        public override string ToString() => Value;
        public static implicit operator string(FilePath path) => path.Value;
        public static implicit operator FilePath(string path) => new FilePath(path);
    }
}