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

    /// <summary>
    /// Domains whose attributes depend on component indexing, so they become invalid
    /// when a modifier changes that indexing. Used by FlowObject.WithNewContent to
    /// decide which attributes survive a content change.
    /// </summary>
    [Flags]
    public enum AttributeDomainMask
    {
        None = 0,
        Instance = 1,
        Vertex = 2,
        Corner = 4,
        DirectedEdge = 8,
        UndirectedEdge = 16,
        Face = 32,
        All = ~0,
    }

    /// <summary>
    /// The indexing-dependence of this attribute's domain: None for domains that
    /// survive any content change (Model, Mesh, Material, Entity).
    /// </summary>
    public AttributeDomainMask DomainDependence
        => Domain switch
        {
            AttributeDomain.Instance => AttributeDomainMask.Instance,
            AttributeDomain.Vertex => AttributeDomainMask.Vertex,
            AttributeDomain.Corner => AttributeDomainMask.Corner,
            AttributeDomain.DirectedEdge => AttributeDomainMask.DirectedEdge,
            AttributeDomain.UndirectedEdge => AttributeDomainMask.UndirectedEdge,
            AttributeDomain.Face => AttributeDomainMask.Face,
            _ => AttributeDomainMask.None,
        };

    public static class CommonNames
    {
        public const string Selection = nameof(Selection);
        public const string Color = nameof(Color);
        public const string SoftSelection = nameof(SoftSelection);
        public const string Id = nameof(Id);
    }

    public FlowAttribute(string name, AttributeDomain domain, Array arrayData)
    {
        Name = name;
        Domain = domain;
        ArrayData = arrayData;
        ElementType = arrayData.GetType().GetElementType()
            ?? throw new ArgumentException("Expected a single-dimensional array", nameof(arrayData));
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
    public IDataTable? TableData { get; }

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
    public Type? ScalarType { get; }
    public int ScalarCount { get; }
    public bool HasScalars => ScalarCount > 1;
}