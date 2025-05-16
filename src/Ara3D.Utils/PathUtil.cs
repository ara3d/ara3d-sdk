using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Ara3D.Utils
{
    /// <summary>
    /// Todo: we need to replace strings with FilePath and DirectoryPath
    /// </summary>
    public static class PathUtil
    {
        public static FileVersionInfo GetVersionInfo(this FilePath filePath)
            => FileVersionInfo.GetVersionInfo(filePath);

        /// <summary>
        /// Generates a Regular Expression character set from an array of characters
        /// </summary>
        public static Regex CharSetToRegex(params char[] chars)
            => new Regex($"[{Regex.Escape(new string(chars))}]");

        /// <summary>
        /// Creates a regular expression for finding illegal file name characters.
        /// </summary>
        public static Regex InvalidFileNameRegex =>
            CharSetToRegex(Path.GetInvalidFileNameChars());

        /// <summary>
        /// A DateTime format suitable for inclusion in a filename.
        /// </summary>
        public const string TimeStampFormat = "yyyy-MM-dd-HH-mm-ss";

        /// <summary>
        /// Returns a time-stamp representation of the given DateTime.
        /// </summary>
        public static string ToTimeStamp(this DateTime dateTime)
            => dateTime.ToString(TimeStampFormat);

        /// <summary>
        /// Returns true or false depending on whether the file path has an extension 
        /// </summary>
        public static bool HasExtension(this FilePath filePath)
            => !filePath.GetExtension().IsNullOrWhiteSpace();

        /// <summary>
        /// Takes a file extension (like .txt) and prepends a "*" to turn it into a mask.
        /// If the file extension h
        /// </summary>
        public static string GetExtensionAsMask(this FilePath file)
            => file.HasExtension() ? $"*.{file.GetExtension()}" : "*.*";

        /// <summary>
        /// Given a file name (e.g. c:\myfile.text) will append a count
        /// tag to it. {c:\myfile{32}.text)
        /// </summary>
        public static string MakeCounted(this FilePath file, int n)
        {
            var dir = file.GetDirectory();
            var ext = file.GetExtension();
            var name = file.GetFileNameWithoutExtension();
            return dir.RelativeFile($"{name}{{n}}{ext}");
        }

        /// <summary>
        /// Number of files with the same base name and a "{X}" afterwards
        /// </summary>
        public static int NumCountedFiles(this FilePath file)
        {
            var name = file.GetFileNameWithoutExtension();
            var ext = file.GetExtension();
            return file.GetDirectory().GetFiles($"{name}{{*}}{ext}").Count();
        }

        /// <summary>
        /// Returns file path if it doesn't exist. 
        /// If the file exists (e.g., test.txt, creates a new file
        /// named "test{0}.txt". If one or more files exists
        /// with a similar name, then increments x, starting at n
        /// (where n is the number of similarly named files).
        /// Until a valid unused name is found (e.g., test{12}.txt).
        /// </summary>
        public static string MakeUnique(this FilePath file)
        {
            if (!file.Exists())
                return file;
            var i = file.NumCountedFiles();
            var newName = MakeCounted(file, i);
            while (File.Exists(newName))
                newName = MakeCounted(file, ++i);
            return newName;
        }

        /// <summary>
        /// Returns the current date-time in a format appropriate for appending to files.
        /// </summary>
        public static string GetTimeStamp()
            => DateTime.Now.ToTimeStamp();

        public static string GetFileName(this FilePath filePath)
            => Path.GetFileName(filePath);

        public static string GetFileNameWithoutExtension(this FilePath filePath)
            => Path.GetFileNameWithoutExtension(filePath);

        /// <summary>
        /// Returns the file name's extension with a period (.) before it,
        /// or either of NULL or empty is none is 
        /// </summary>
        public static string GetExtension(this FilePath filePath)
            => Path.GetExtension(filePath);

        public static bool HasExtension(this FilePath filePath, string extension)
            => filePath.Value.ToLowerInvariant().EndsWith(extension.ToLowerInvariant());

        public static string GetExtensionLower(this FilePath filePath)
            => Path.GetExtension(filePath).ToLowerInvariant();

        public static FilePath TransformName(this FilePath filePath, Func<string, string> f)
            => filePath.GetDirectory().RelativeFile(
                f(filePath.GetFileNameWithoutExtension()) + filePath.GetExtension());

        /// <summary>
        /// Appends a time stamp to the time stamped filename and extension.
        /// </summary>
        public static FilePath ToTimeStampedFileName(this FilePath filePath)
            => TransformName(filePath, name => $"{name}-{GetTimeStamp()}");

        /// <summary>
        /// Appends a time stamp to the time stamped filename and extension.
        /// If not unique, adds a "{x}" to it, similar to how
        /// Windows does it.
        /// </summary>
        public static FilePath ToUniqueTimeStampedFileName(this FilePath filePath)
            => filePath.ToTimeStampedFileName().MakeUnique();

        public static FilePath Move(this FilePath filePath, FilePath destination)
        {
            File.Move(filePath, destination);
            return destination;
        }

        public static FilePath Copy(this FilePath filePath, FilePath destination, bool overWrite = true)
        {
            File.Copy(filePath, destination, overWrite);
            return destination;
        }

        public static FilePath CopyToFolder(this FilePath path, DirectoryPath dir, bool dontCreate = false)
        {
            if (!dontCreate)
                Directory.CreateDirectory(dir);
            var newPath = path.ChangeDirectory(dir);
            path.Copy(newPath);
            return newPath;
        }

        public static FilePath MoveToFolder(this FilePath path, DirectoryPath dir, bool dontCreate = false)
        {
            if (!dontCreate)
                Directory.CreateDirectory(dir);
            var newPath = path.ChangeDirectory(dir);
            path.Move(newPath);
            return newPath;
        }

        public static void CopyDirectory(this DirectoryPath source, DirectoryPath target, bool recursive = false)
        {
            target.Create();

            // Copy each file into the new directory.
            foreach (var f in source.GetFiles())
                f.CopyToFolder(target);

            if (recursive)
            {
                // Copy subfolders recursively
                foreach (var d in source.GetSubFolders())
                {
                    var newDir = target.RelativeFolder(d.GetFolderName());
                    CopyDirectory(d, newDir, true);
                }
            }
        }

        /// <summary>
        /// Returns the size of the directory in bytes.
        /// </summary>
        public static long GetDirectorySizeInBytes(this DirectoryPath directoryPath)
            => directoryPath.GetAllFilesRecursively().Sum(GetFileSize);

        // Improved answer over:
        // https://stackoverflow.com/questions/211008/c-sharp-file-management
        // https://stackoverflow.com/questions/1358510/how-to-compare-2-files-fast-using-net?noredirect=1&lq=1
        // Should be faster than the SHA version, with no chance of a mismatch.
        public static bool CompareFiles(this FilePath filePath1, FilePath filePath2)
        {
            if (!filePath1.Exists() || !filePath2.Exists())
                return false;

            if (filePath1.GetFileSize() != filePath2.GetFileSize())      
                return false;

            // Default buffer size of File stream * 16.
            // Profiling revealed this to be faster than the default
            const int bufferSize = 4096 * 16;
            var buf1 = new byte[bufferSize];
            var buf2 = new byte[bufferSize];

            // open both for reading
            using (var stream1 = OpenFileStreamReading(filePath1, bufferSize))
            using (var stream2 = OpenFileStreamReading(filePath1, bufferSize))
            {
                // Read buffers (need to be careful because of contract of stream.Read)
                var tmp1 = stream1.SafeRead(buf1, 0, bufferSize);
                var tmp2 = stream2.SafeRead(buf2, 0, bufferSize);

                // Check that we read the same size
                if (tmp1 != tmp2) return false;

                // Compare the bytes
                for (var i = 0; i < tmp1; ++i)
                {
                    if (buf1[i] != buf2[i])
                        return false;
                }
            }

            return true;
        }

        public static byte[] SHA256Hash(this FilePath filePath)
            => SHA256.Create().ComputeHash(File.OpenRead(filePath));

        public static byte[] MD5Hash(this FilePath filePath)
            => MD5.Create().ComputeHash(File.OpenRead(filePath));

        public static FilePath Encrypt(this FilePath filePath)
        {
            File.Encrypt(filePath);
            return filePath;
        }

        public static FilePath Decrypt(this FilePath filePath)
        {
            File.Decrypt(filePath);
            return filePath;
        }

        /// <summary>
        /// Deletes the file or directory (recursively) at the given path.
        /// </summary>
        public static void Delete(this DirectoryPath path, bool recursive = true)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, recursive);
        }

        /// <summary>
        /// Deletes all contents in a folder
        /// https://stackoverflow.com/questions/1288718/how-to-delete-all-files-and-folders-in-a-directory
        /// </summary>
        public static void DeleteFolderContents(this DirectoryPath directoryPath)
        {
            var di = new DirectoryInfo(directoryPath);
            foreach (var dir in di.EnumerateDirectories().AsParallel())
                DeleteFolderAndAllContents(dir.FullName);
            foreach (var file in di.EnumerateFiles().AsParallel())
                file.Delete();
        }

        public static IEnumerable<DirectoryPath> GetSubFolders(this DirectoryPath path)
            => path.GetInfo().GetDirectories().Select(d => (DirectoryPath)d.FullName);

        public static bool Exists(this DirectoryPath folderPath)
            => Directory.Exists(folderPath);

        /// <summary>
        /// Deletes everything in a folder and then the folder.
        /// </summary>
        public static void DeleteFolderAndAllContents(this DirectoryPath folderPath)
        {
            if (!folderPath.Exists())
                return;
            DeleteFolderContents(folderPath);
            folderPath.Delete();
        }

        /// <summary>
        /// Creates a directory if it doesn't exist
        /// </summary>
        public static DirectoryPath Create(this DirectoryPath dirPath)
        {
            if (!dirPath.Exists())
                Directory.CreateDirectory(dirPath);
            return dirPath;
        }

        /// <summary>
        /// Create the directory for the given filepath if it doesn't exist.
        /// </summary>
        public static DirectoryPath CreateDirectory(this FilePath filepath)
            => filepath.GetDirectory().Create();

        /// <summary>
        /// Creates a directory if needed, or clears all of its contents otherwise
        /// </summary>
        public static DirectoryPath CreateAndClearDirectory(this DirectoryPath dirPath)
        {
            if (!dirPath.Exists())
                dirPath.Create();
            else
                dirPath.DeleteFolderContents();
            return dirPath;
        }

        /// <summary>
        /// Tries to create and clear a directory, but swallows exceptions if any occur
        /// </summary>
        public static DirectoryPath TryToCreateAndClearDirectory(this DirectoryPath dirPath)
            => FunctionUtils.TryGetValue(() => dirPath.CreateAndClearDirectory(), dirPath);

        /// <summary>
        /// Returns true if the given directory contains no files or if the directory does not exist.
        /// </summary>
        public static bool IsEmpty(this DirectoryPath dirPath)
            => !dirPath.Exists() || dirPath.GetAllFilesRecursively().Any();

        /// <summary>
        /// Deletes the target filepath if it exists and creates the containing directory.
        /// </summary>
        public static FilePath DeleteAndCreateDirectory(this FilePath filePath)
        {
            // Delete the filepath (or directory) if it already exists.
            if (filePath.Exists())
                filePath.Delete();

            if (!filePath.GetDirectory().Exists())
                filePath.GetDirectory().Create();

            // Create the target directory containing the output path.
            return filePath;
        }

        /// <summary>
        /// Get the base name of the specified directory 
        /// </summary>
        public static string GetFolderName(this DirectoryPath dirPath)
            => Path.GetDirectoryName(dirPath);
        
        /// <summary>
        /// Useful quick test to assure that we can create a file in the folder and write to it.
        /// </summary>
        public static void TestWrite(this DirectoryPath folder)
        {
            var fileName = Path.Combine(folder, "_deleteme_.tmp");
            File.WriteAllText(fileName, "test");
            File.Delete(fileName);
        }

        /// Convert a string to a valid name
        /// https://stackoverflow.com/questions/146134/how-to-remove-illegal-characters-from-path-and-filenames
        /// https://stackoverflow.com/questions/2230826/remove-invalid-disallowed-bad-characters-from-filename-or-directory-folder?noredirect=1&lq=1
        /// https://stackoverflow.com/questions/10898338/c-sharp-string-replace-to-remove-illegal-characters?noredirect=1&lq=1
        /// </summary>
        public static string ToValidFileName(this string s)
            => InvalidFileNameRegex.Replace(s, m => "_");

        /// <summary>
        /// Returns true if the string has any invalid file name chars
        /// </summary>
        public static bool HasInvalidFileNameChas(this FilePath filePath)
            => InvalidFileNameRegex.Match(filePath).Success;

        /// <summary>
        /// Returns the name of the outer most folder given a file path or a directory path
        /// https://stackoverflow.com/questions/3736462/getting-the-folder-name-from-a-path
        /// </summary>
        public static DirectoryPath GetDirectory(this FilePath filePath)
            => Path.GetDirectoryName(filePath.GetFullPath());

        /// <summary>
        /// Returns the full name of the path, resolving any relative paths as necessary. 
        /// </summary>
        public static DirectoryPath GetFullPath(this DirectoryPath dirPath)
            => Path.GetFullPath(dirPath);

        /// <summary>
        /// Changes the directory and the extension of a file
        /// </summary>
        public static FilePath ChangeDirectoryAndExt(this FilePath filePath, DirectoryPath newFolder, string newExt)
            => filePath.ChangeDirectory(newFolder).ChangeExtension(newExt);

        /// <summary>
        /// Changes the directory of a file
        /// </summary>
        public static FilePath ChangeDirectory(this FilePath filePath, DirectoryPath newFolder)
            => newFolder.RelativeFile(filePath.GetFileName());

        public static FilePath ChangeExtension(this FilePath filePath, string ext)
            => Path.ChangeExtension(filePath, ext);

        public static FilePath StripExtension(this FilePath filePath)
            => Path.ChangeExtension(filePath, null);

        public static DirectoryInfo GetInfo(this DirectoryPath path)
            => new DirectoryInfo(path);

        public static DirectoryPath GetParent(this DirectoryPath path)
            => path.GetInfo().Parent?.FullName;

        public static DirectoryPath GetParent(this FilePath path)
            => path.GetDirectory().GetParent();

        public static DirectoryPath Up(this DirectoryPath path)
            => path.GetParent();

        public static DirectoryPath Up(this FilePath path)
            => path.GetParent();

        public static FilePath FindFirstInAncestor(this DirectoryPath path, string searchPattern = "*.*")
        {
            foreach (var d in path.GetSelfAndAncestors())
            {
                var fs = d.GetFiles(searchPattern).ToList();
                if (fs.Count > 0)
                    return fs[0];
            }

            return null;
        }

        public static IEnumerable<DirectoryPath> GetSelfAndAncestors(this DirectoryPath current)
        {
            for (; current != null; current = current.GetParent())
                yield return current;
        }

        public static bool HasWildCard(this FilePath filePath)
            => filePath.Value.Contains("*") || filePath.Value.Contains("?");

        // File size reporting

        static readonly string[] ByteSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB

        /// Improved version of https://stackoverflow.com/questions/281640/how-do-i-get-a-human-readable-file-size-in-bytes-abbreviation-using-net
        public static string BytesToString(long byteCount, int numPlacesToRound = 1)
        {
            if (byteCount == 0) return "0B";
            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var num = Math.Round(bytes / Math.Pow(1024, place), numPlacesToRound);
            return $"{(Math.Sign(byteCount) * num).ToString($"F{numPlacesToRound}")}{ByteSuffixes[place]}";
        }

        /// <summary>
        /// Returns the file size in bytes, or 0 if there is no file.
        /// </summary>
        public static long GetFileSize(this FilePath fileName)
            => fileName.Exists() ? new FileInfo(fileName).Length : 0;

        /// <summary>
        /// Returns the file size in bytes, or 0 if there is no file.
        /// </summary>
        public static string GetFileSizeAsString(this FilePath filePath, int numPlacesToShow = 1)
            => BytesToString(GetFileSize(filePath), numPlacesToShow);

        /// <summary>
        /// Returns the total file size of all files given
        /// </summary>
        public static long TotalFileSize(this IEnumerable<FilePath> files)
            => files.Sum(GetFileSize);

        /// <summary>
        /// Prepends text to the file name keeping it in the same folder and with the same extension
        /// </summary>
        public static FilePath PrependFileName(this FilePath filePath, string text)
            => filePath.TransformName(name => text + name);

        /// <summary>
        /// Prepends text to the file name keeping it in the same folder and with the same extension
        /// </summary>
        public static FilePath AppendFileName(this FilePath filePath, string text)
            => filePath.TransformName(name => name + text);

        /// <summary>
        /// Returns all the lines of all the files
        /// </summary>
        public static IEnumerable<string> ReadManyLines(this IEnumerable<FilePath> fileNames)
            => fileNames.SelectMany(fp => fp.ReadLines());

        public static IEnumerable<string> ReadLines(this FilePath filePath)
            => File.ReadLines(filePath);

        /// <summary>
        /// Returns the path in which the given directory path has been removed.
        /// </summary>
        public static string TrimDirectoryFromPath(string dir, string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            if (string.IsNullOrEmpty(dir)) return path;

            var dirFullPath = Path.GetFullPath(dir).ToLowerInvariant();
            var fullPath = Path.GetFullPath(path).ToLowerInvariant();

            return fullPath.StartsWith(dirFullPath)
                ? fullPath.Substring(dir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                : fullPath;
        }

        public static FileStream OpenFileStreamWriting(this FilePath filePath, int bufferSize)
            => new FileStream(filePath, FileMode.Open, FileAccess.Write, FileShare.None, bufferSize);

        public static FileStream OpenFileStreamReading(this FilePath filePath, int bufferSize)
            => new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize);

        public static FileStream OpenRead(this FilePath filePath)
            => File.OpenRead(filePath);

        public static FileStream OpenWrite(this FilePath filePath)
            => File.OpenWrite(filePath);

        /// <summary>
        /// Returns a binary writer for the given file path
        /// </summary>
        public static BinaryWriter CreateBinaryWriter(this FilePath filePath)
            => new BinaryWriter(filePath.OpenWrite());

        /// <summary>
        /// Returns a binary reader for the given file path
        /// </summary>
        public static BinaryReader CreateBinaryReader(this FilePath filePath)
            => new BinaryReader(filePath.OpenRead());

        /// <summary>
        /// Creates an empty file 
        /// </summary>
        public static FilePath CreateEmpty(this FilePath filePath)
        {
            File.CreateText(filePath).Close();
            return filePath;
        }

        public static IEnumerable<FilePath> GetAllFilesRecursively(this DirectoryPath directoryPath)
            => directoryPath.GetFiles("*.*", true);

        public static IEnumerable<FilePath> GetFiles(this DirectoryPath directoryPath, string searchPattern = "*.*",
            bool recurse = false)
            => Directory.EnumerateFiles(directoryPath, searchPattern,
                recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Select(f => (FilePath)f);

        public static bool Exists(this FilePath filePath)
            => filePath.GetInfo().Exists;

        public static FileInfo GetInfo(this FilePath filePath)
            => new FileInfo(filePath);

        public static string ReadAllText(this FilePath self)
            => File.ReadAllText(self);

        public static byte[] ReadAllBytes(this FilePath self)
            => File.ReadAllBytes(self);

        public static string[] ReadAllLines(this FilePath self)
            => File.ReadAllLines(self);

        public static FilePath WriteAllText(this FilePath self, string contents)
        {
            File.WriteAllText(self, contents);
            return self;
        }

        public static FilePath WriteAllBytes(this FilePath self, byte[] bytes)
        {
            File.WriteAllBytes(self, bytes);
            return self;
        }

        public static FilePath WriteAllLines(this FilePath self, IEnumerable<string> lines)
        {
            File.WriteAllLines(self, lines);
            return self;
        }

        public static FilePath Delete(this FilePath self)
        {
            File.Delete(self);
            return self;
        }

        public static FilePath Create(this FilePath self)
        {
            File.Create(self);
            return self;
        }

        public static FilePath CopyToStreamAndClose(this FilePath filePath, Stream outputStream)
        {
            if (!filePath.Exists()) return filePath;
            using (var inputStream = File.OpenRead(filePath))
            {
                inputStream.CopyTo(outputStream);
            }
            return filePath;
        }

        public static FilePath GetFullPath(this FilePath self)
            => new FileInfo(self).FullName;

        public static FilePath CreateTempFile()
            => Path.GetTempFileName();

        public static FilePath CreateTempFileWithContents(string text)
            => CreateTempFile().WriteAllText(text);

        // There exist better solutions. See:
        // https://stackoverflow.com/questions/581570/how-can-i-create-a-temp-file-with-a-specific-extension-with-net
        public static FilePath CreateTempFile(string extension)
            => CreateTempFile().ChangeExtension(extension);

        public static FilePath MoveToRelativeFolder(this FilePath filePath, string subFolder)
            => filePath.MoveToFolder(filePath.RelativeFolder(subFolder));

        public static FilePath CreateTempFile(string subFolder, string extension)
            => CreateTempFile(extension).MoveToRelativeFolder(subFolder);

        public static FilePath Touch(this FilePath filePath)
        {
            File.SetLastWriteTimeUtc(filePath, DateTime.Now);
            return filePath;
        }

        public static DateTime GetCreatedTime(this FilePath filePath)
            => File.GetCreationTimeUtc(filePath);

        public static DateTime GetLastWriteTime(this FilePath filePath)
            => File.GetLastWriteTimeUtc(filePath);

        public static bool HasAttribute(this FilePath filePath, FileAttributes attribute)
            => (File.GetAttributes(filePath) | attribute) != 0;

        public static bool IsReadOnly(this FilePath filePath) => filePath.HasAttribute(FileAttributes.ReadOnly);
        public static bool IsHidden(this FilePath filePath) => filePath.HasAttribute(FileAttributes.Hidden);
        public static bool IsSystem(this FilePath filePath) => filePath.HasAttribute(FileAttributes.System);
        public static bool IsDirectory(this FilePath filePath) => filePath.HasAttribute(FileAttributes.Directory);
        public static bool IsArchive(this FilePath filePath) => filePath.HasAttribute(FileAttributes.Archive);
        public static bool IsDevice(this FilePath filePath) => filePath.HasAttribute(FileAttributes.Device);
        public static bool IsNormal(this FilePath filePath) => filePath.HasAttribute(FileAttributes.Normal);
        public static bool IsTemporary(this FilePath filePath) => filePath.HasAttribute(FileAttributes.Temporary);
        public static bool IsSparseFile(this FilePath filePath) => filePath.HasAttribute(FileAttributes.SparseFile);
        public static bool IsReparsePoint(this FilePath filePath) => filePath.HasAttribute(FileAttributes.ReparsePoint);
        public static bool IsCompressed(this FilePath filePath) => filePath.HasAttribute(FileAttributes.Compressed);
        public static bool IsOffline(this FilePath filePath) => filePath.HasAttribute(FileAttributes.Offline);
        public static bool IsNotContentIndexed(this FilePath filePath) => filePath.HasAttribute(FileAttributes.NotContentIndexed);
        public static bool IsEncrypted(this FilePath filePath) => filePath.HasAttribute(FileAttributes.Encrypted);
        public static bool IsIntegrityStream(this FilePath filePath) => filePath.HasAttribute(FileAttributes.IntegrityStream);
        public static bool IsNoScrubData(this FilePath filePath) => filePath.HasAttribute(FileAttributes.NoScrubData);

        public static DirectoryPath GetCallerSourceFolder([CallerFilePath] string filePath = "")
            => new FilePath(filePath).GetDirectory();

        public static DirectoryPath RelativeFolder(this DirectoryPath path, params string[] parts) 
            => Path.Combine(parts.Prepend(path.Value).ToArray());

        public static DirectoryPath RelativeFolder(this FilePath path, params string[] parts)
            => Path.Combine(parts.Prepend(path.Value).ToArray());

        public static FilePath RelativeFile(this DirectoryPath path, params string[] parts)
            => Path.Combine(parts.Prepend(path.Value).ToArray());

        public static FilePath RelativeFile(this FilePath path, params string[] parts)
            => path.GetDirectory().RelativeFile(parts);
            
        public static bool IsSameFile(this FilePath path, FilePath other)
            => path.GetFullPath().Value.ToLowerInvariant() == other.GetFullPath().Value.ToLowerInvariant();
    }
}