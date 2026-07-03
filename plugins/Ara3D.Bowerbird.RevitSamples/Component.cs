using System;

namespace Ara3D.Bowerbird.RevitSamples;

public class Component
{
    /// <summary>
    /// The ID of the entity this component is associated with 
    /// </summary>
    public Guid EntityId { get; set; }

    /// <summary>
    /// The ID of the type of component  
    /// </summary>
    public Guid ComponentTypeId { get; set; }

    /// <summary>
    /// The ID of the component that was created 
    /// </summary>
    public Guid ComponentId { get; set; }
    
    /// <summary>
    /// Identifies this particular component instance.
    /// Components are never modified, new ones are created with new version Ids.
    /// They should share the same component ID 
    /// </summary>
    public Guid VersionedComponentId { get; set; }

    /// <summary>
    /// The ID of the previous component, if this was created as a modification
    /// or a previous one. This allows the history to be created 
    /// </summary>
    public Guid ParentVersionedComponentId { get; set; }

    /// <summary>
    /// When this component was changed 
    /// </summary>
    public DateTimeOffset DateCreated { get; set; }

    /// <summary>
    /// Used to identify who created the component
    /// </summary>
    public Guid AuthorId { get; set; }

    /// <summary>
    /// Used to identify what specific tool was used to create the component 
    /// </summary>
    public Guid VersionedToolId { get; set; }
}