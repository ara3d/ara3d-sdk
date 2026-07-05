using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

using BYTES = System.ArraySegment<byte>;
using MODEL = SharpGLTF.Schema2.ModelRoot;
namespace SharpGLTF.Schema2
{
    /// <summary>
    /// Callback used for loading associated files of current model.
    /// </summary>
    /// <param name="assetName">the asset relative path.</param>
    /// <returns>The file contents as a <see cref="byte"/> array.</returns>
    public delegate BYTES FileReaderCallback(String assetName);

    public delegate String UriResolver(String relativeUri);

    /// <summary>
    /// Context for reading a <see cref="MODEL"/>.
    /// </summary>
    public class ReadContext : ReadSettings
    {
        #region lifecycle

        public static ReadContext Create(FileReaderCallback callback)
        {
            Guard.NotNull(callback, nameof(callback));

            return new ReadContext(callback);
        }        

        public static ReadContext CreateFromDirectory(DirectoryInfo dinfo)
        {
            Guard.NotNull(dinfo, nameof(dinfo));
            Guard.MustExist(dinfo, nameof(dinfo));

            string _uriSolver(string rawUri)
            {
                var path = Uri.UnescapeDataString(rawUri);
                return Path.Combine(dinfo.FullName, path);
            }

            BYTES _loadFile(string rawUri)
            {
                var path = _uriSolver(rawUri);
                var content = File.ReadAllBytes(path);
                return new BYTES(content);
            }

            return new ReadContext(_loadFile, _uriSolver);
        }

        public static ReadContext CreateFromDictionary(IReadOnlyDictionary<string, BYTES> dictionary, bool checkExtensions = false)
        {
            return new ReadContext(rawUri => dictionary[rawUri], null, checkExtensions);
        }

        private ReadContext(FileReaderCallback reader, UriResolver uriResolver = null, bool checkExtensions = true)
        {
            _FileReader = reader;
            _UriResolver = uriResolver;
            _CheckSupportedExtensions = checkExtensions;
        }

        public ReadContext WithSettingsFrom(ReadSettings settings)
        {
            settings?.CopyTo(this);
            return this;
        }

        internal ReadContext(ReadContext other)
            : base(other)
        {
            _FileReader = other._FileReader;
            ImageDecoder = other.ImageDecoder;
        }

        #endregion

        #region data

        /// <summary>
        /// Unescapes glTF asset URIs so they can be consumed by <see cref="_FileReader"/>
        /// </summary>
        private UriResolver _UriResolver;

        /// <summary>
        /// Retrieves file blobs from the current context.
        /// </summary>
        private FileReaderCallback _FileReader;

        /// <summary>
        /// When loading a GLB, this represents the internal binary data chunk.
        /// </summary>
        private Byte[] _BinaryChunk;

        /// <summary>
        /// Gets a value indicating whether to check used/required extensions.
        /// </summary>
        internal Boolean _CheckSupportedExtensions { get; private set; } = true;

        #endregion

        #region API - File System

        public bool TryGetFullPath(string relativeUri, out string fullPath)
        {
            if (_UriResolver == null) { fullPath = null; return false; }
            fullPath = _UriResolver(relativeUri);
            return true;
        }

        /// <summary>
        /// Opens a file relative to this <see cref="ReadContext"/>.
        /// </summary>
        /// <param name="resourceName">A relative file Name path.</param>
        /// <returns>A <see cref="Stream"/>.</returns>
        public Stream OpenFile(string resourceName)
        {
            var content = ReadAllBytesToEnd(resourceName);

            return new MemoryStream(content.Array, content.Offset, content.Count);
        }

        public BYTES ReadAllBytesToEnd(string resourceName)
        {
            if (_BinaryChunk != null)
            {
                if (string.IsNullOrEmpty(resourceName)) return new BYTES(_BinaryChunk);
            }

            return _FileReader(resourceName);
        }

        #endregion

        #region API

        /// <summary>
        /// Reads a <see cref="MODEL"/> instance from the current context containing a GLB or a GLTF file.
        /// </summary>
        /// <param name="resourceName">The name of the resource within the context.</param>
        /// <returns>A <see cref="MODEL"/> instance.</returns>
        public MODEL ReadSchema2(string resourceName)
        {
            Guard.FilePathMustBeValid(resourceName, nameof(resourceName));
            if (Path.IsPathRooted(resourceName)) throw new ArgumentException("path must be relative", nameof(resourceName));

            var root = ReadAllBytesToEnd(resourceName);

            using (var stream = new MemoryStream(root.Array, root.Offset, root.Count, false))
            {
                return ReadSchema2(stream);
            }
        }

        /// <summary>
        /// Reads a <see cref="MODEL"/> instance from a <see cref="Stream"/> containing a GLB or a GLTF file.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> to read from.</param>
        /// <returns>A <see cref="MODEL"/> instance.</returns>
        public MODEL ReadSchema2(Stream stream)
        {
            Guard.NotNull(stream, nameof(stream));
            Guard.IsTrue(stream.CanRead, nameof(stream));
            bool binaryFile = _BinarySerialization._Identify(stream);
            return binaryFile
                ? ReadBinarySchema2(stream)
                : ReadTextSchema2(stream);
        }

        /// <summary>
        /// Reads a <see cref="MODEL"/> instance from a <see cref="Stream"/> containing a GLTF file.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> to read from.</param>
        /// <returns>A <see cref="MODEL"/> instance.</returns>
        public MODEL ReadTextSchema2(Stream stream)
        {
            Guard.NotNull(stream, nameof(stream));
            Guard.IsTrue(stream.CanRead, nameof(stream));
            var json = stream.ReadBytesToEnd();
            return _Read(json);
        }

        /// <summary>
        /// Reads a <see cref="MODEL"/> instance from a <see cref="Stream"/> containing a GLB file.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> to read from.</param>
        /// <returns>A <see cref="MODEL"/> instance.</returns>
        public MODEL ReadBinarySchema2(Stream stream)
        {
            Guard.NotNull(stream, nameof(stream));
            Guard.IsTrue(stream.CanRead, nameof(stream));

            return _ReadGLB(stream);
        }
        #endregion

        #region core

        private MODEL _ReadGLB(Stream stream)
        {
            var chunks = _BinarySerialization.ReadBinaryFile(stream);
            var context = this;

            if (chunks.ContainsKey(_BinarySerialization.CHUNKBIN))
            {
                // clone self
                var binChunk = chunks[_BinarySerialization.CHUNKBIN];
                context = new ReadContext(context);
                context._BinaryChunk = binChunk;
            }

            var jsonChunk = chunks[_BinarySerialization.CHUNKJSON];

            return context._Read(jsonChunk);
        }

        private MODEL _Read(ReadOnlyMemory<Byte> jsonUtf8Bytes)
        {
            var root = new MODEL();

            if (jsonUtf8Bytes.IsEmpty) 
                throw new JsonException("Json is empty.");

            jsonUtf8Bytes = _Preprocess(jsonUtf8Bytes);
            var reader = new Utf8JsonReader(jsonUtf8Bytes.Span);

            if (!reader.Read())
                throw new Exception("Failed to read JSON");

            root.Deserialize(ref reader);
            root.OnDeserializationCompleted();

            root._ResolveSatelliteDependencies(this);
            return root;
        }

        private ReadOnlyMemory<Byte> _Preprocess(ReadOnlyMemory<Byte> jsonUtf8Bytes)
        {
            if (JsonPreprocessor == null) return jsonUtf8Bytes;
            var text = Encoding.UTF8.GetString(jsonUtf8Bytes.Span);
            text = JsonPreprocessor.Invoke(text);
            return new ReadOnlyMemory<Byte>(Encoding.UTF8.GetBytes(text));
        }

        #endregion

        #region extras

        /// <summary>
        /// Tries to identify a stream as a text (glTF) or binary (GLB) format.
        /// </summary>
        /// <param name="stream">An open <see cref="Stream"/> where <see cref="Stream.CanSeek"/> must be true.</param>
        /// <returns>True if it's a binary GLB format.</returns>
        /// <remarks>
        /// After identification, the <see cref="Stream.Position"/> will be reset to the position it had before calling this method.
        /// </remarks>
        public static bool IdentifyBinaryContainer(Stream stream)
        {
            Guard.NotNull(stream, nameof(stream));
            return _BinarySerialization._Identify(stream);
        }

        public static String ReadJson(Stream stream)
        {
            Guard.NotNull(stream, nameof(stream));

            bool binaryFile = IdentifyBinaryContainer(stream);

            if (binaryFile)
            {
                var chunks = _BinarySerialization.ReadBinaryFile(stream);

                return Encoding.UTF8.GetString(chunks[_BinarySerialization.CHUNKJSON]);
            }

            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }

        public static ReadOnlyMemory<Byte> ReadJsonBytes(Stream stream)
        {
            Guard.NotNull(stream, nameof(stream));

            bool binaryFile = IdentifyBinaryContainer(stream);

            if (binaryFile)
            {
                var chunks = _BinarySerialization.ReadBinaryFile(stream);

                return chunks[_BinarySerialization.CHUNKJSON];
            }

            return stream.ReadBytesToEnd();
        }

        #endregion
    }
}
