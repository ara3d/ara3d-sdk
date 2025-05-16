namespace Ara3D.Utils
{
    /// <summary>
    /// Wraps a string used to represent a path to a directory.
    /// Implicitly casts to and from strings as needed. See PathUtil
    /// for a number of useful functions.
    /// </summary>
    public class DirectoryPath
    {
        public string Value { get; }
        public DirectoryPath(string path) => Value = path;
        public override string ToString() => Value;
        public static implicit operator string(DirectoryPath path) => path.Value;
        public static implicit operator DirectoryPath(string path) => new DirectoryPath(path);
    }
}