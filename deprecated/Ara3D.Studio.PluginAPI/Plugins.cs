 using System;
using System.Collections.Generic;
using Plato;
using Plato.SinglePrecision;

namespace Ara3D.Studio.PluginAPI
{
    public interface IPlugin
    {
        string Name { get; }
        void OnInitialize();
        void OnFinalize();
    }

    public interface IPlugin<T> : IPlugin
    {
        T Evaluate();
    }

    public interface IPluginGeometry : IPlugin<TriangleMesh3D> { }

    public abstract class BasePlugin<T> : IPlugin<T>
    {
        public virtual string Name => GetType().Name;
        public virtual void OnInitialize() { }
        public virtual void OnFinalize() { }
        public abstract T Evaluate();
    }

    public interface IFileMetaData
    { }

    public interface ISceneLayer {  }

    public interface IFileType2
    {
        bool CanQuickValidate { get; }
        bool IsValid(string path);
        IReadOnlyList<string> Extensions { get; }
        string Description { get; }
        string Name { get; }
        IFileMetaData GetMetaData(string path);
        bool SupportsGeometry { get; }
        bool SupportsInstancing { get; }
        bool SupportsMultipleMeshes { get; }
        bool SupportsMultiData { get; }
        IReadOnlyList<string> ReferenceUrls { get; }
    }

    public interface IFileType
    {
        IReadOnlyList<string> Extensions { get; }
        string Description { get; }
        string Name { get; }
    }

    public interface IImporter
    {
        IFileType FileType { get; }
        ISceneLayer Import(string filePath);
    }

    public interface IExporter
    {
        IFileType FileType { get; }
        bool Export(string filePath, ISceneLayer layer);
    }


}
