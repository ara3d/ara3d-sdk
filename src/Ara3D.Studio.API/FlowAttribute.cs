using Ara3D.DataTable;
using Ara3D.Memory;

namespace Ara3D.Studio.API;

/// <summary>
/// A FlowAttribute is a set of data that is associated with some
/// component of the geometry. It can be accessed as a table, or as an array.
/// In some cases it provides access to raw memory data.  
/// The FlowObject holds a dynamic list of FlowAttributes. 
/// </summary>
public class FlowAttribute
{
    public enum AttributeDomain
    {
        Model,
        Mesh,
        Instance,
        Material,
        Entity,
        Vertex,
        Corner,
        DirectedEdge,
        UndirectedEdge,
        Face,
        None,
    }

    public static class CommonNames
    {
        public const string Selection = nameof(Selection);
        public const string Color = nameof(Color);
        public const string Id = nameof(Id);
    }

    /// <summary>
    /// Every attribute has a name 
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Some modifiers change the indexing of the associated component, causing the FlowAttribute to be discarded.    
    /// </summary>
    public AttributeDomain Domain { get; }


   /// <summary>
    /// Data can be accessed as a table.
    /// An array is a special table with one column. 
    /// </summary>
    public IDataTable TableData { get; }
    
    /// <summary>
    /// Data can be access as an array.
    /// A table is an array of IDataRows. 
    /// </summary>
    public Array ArrayData { get; }
    public Type ElementType { get; }

    /// <summary>
    /// Some attributes provide access to the raw unmanaged buffers for data,
    /// which can allow for faster access and computation.
    /// </summary>
    public IBuffer? RawData { get; }
    public bool HasRawData => RawData != null;

    /// <summary>
    /// Some attributes are made up of collections, of primitive scalars (e.g., Vector3 is 3 float)
    /// </summary>
    public Type ScalarType { get; }
    public int ScalarCount { get; }
    public bool HasScalars => ScalarCount > 1;
}