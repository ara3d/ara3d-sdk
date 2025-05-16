# G3D

[<img src="https://img.shields.io/nuget/v/Vim.G3d.svg">](https://www.nuget.org/packages/Vim.G3d) 

G3D is a simple, efficient, generic binary format for storing and transmitting geometry. The G3D format
is designed to be used either as a serialization format or as an in-memory data structure.

G3D can represent triangular meshes, quadrilateral meshes, polygonal meshes, point clouds, and line segments.  
It can be easily and efficiently deserialized and rendered in different languages and on different platforms.

The G3D format can contain a superset of geometry attributes found in most common geometry formats, 
including formats such as FBX, glTF, OBJ, PLY, and in memory data structures used in popular 3D APIs, like 
Unity, Three.JS, Assimp, and 3dsMax.

BFAST is maintained by [VIMaec LLC](https://vimaec.com) and is licensed under the terms of the MIT License.

# Format 

## BFAST Container

The underlying binary layout of a G3D file conforms to the [BFAST serialization format](https://github.com/vimaec/bfast), which is a simple and efficient binary format for serializing
collections of byte arrays. BFAST provides an interface that allows named arrays of binary data to be serialized and deserialized quickly and easily.

The first named buffer in the BFAST container is reserved for meta-information about the file encoded in JSON format. It has the name "meta".
Each subsequent buffer uses the attribute descriptor string as a name. 

## Meta-Information

The first buffer of a G3D file is named "meta" and is represented by the G3D 

## Attributes 

G3D is organized as a collection of attribute buffers. Each attributes describe what part of the incoming geometry they are associated with:

* point     // vertex data
* corner    // face-vertex data
* face      // per polygon data
* edge      // per half-edge data 
* group     // face group. Face groups are identified by a face data buffer 
* SubGeometry   // a contiguous section of the vertex-buffer, and index-buffer
* material  // data associated with a material, materials are usually associated with groups, a
* instance  // instance data, usually an instance has an index to a SubGeometry 
* all		// whole object data - for example face-size of 4 with whole object indicates a quad mesh
* none      // no association

Attributes also have a "semantic" which is used to identify what role the attribute has when parsing. These map roughly to FBX layer elements, or Three.JS buffer attributes.
There are a number of predefined semantic values with reserved names, but applications are free to define custom semantic values. The only required semantic in a G3D file is
"position". Here is a list of some of the predefined semantics: 

* unknown,       // no known attribute type
* position,      // vertex buffer 
* index,         // index buffer
* indexoffset,   // an offset into the index buffer (used with Subgeometries)
* vertexoffset,  // the offset into the vertex buffer (used only with Subgeometries)
* normal,        // computed normal information (per face, group, corner, or vertex)
* binormal,      // computed binormal information 
* tangent,       // computed tangent information 
* material,      // material index
* visibility,    // visibility data (e.g. 
* size,          // number of indices per face or group
* uv,            // UV (sometimes more than 1, e.g. Unity supports up to 8)
* color,         // usually vertex color, but could be edge color as well
* smoothing,     // identifies smoothing groups (e.g. ala 3ds Max and OBJ files)
* weight,        // in 3ds Max this is called selection 
* mapchannel,    // 3ds Max map channel (assoc of none => map verts, assoc of corner => map faces)
* id,            // used to identify what object each face part came from 
* joint,         // used to identify what a joint a skin is associated with 
* boxes,         // used to identify bounding boxes
* spheres,       // used to identify bounding spheres
* user,          // identifies user specific data (in 3ds Max this could be "per-vertex-data")

Attributes are stored in 512-byte aligned data-buffers arranged as arrays of scalars or fixed width vectors. The individual data values can be integers, or floating point values of various widths from 1 to 8 bytes. The data-types are:

* int8
* int16
* int32
* int64
* float32
* float64

The number of primitives per data element is called the "arity" and can be any integer value greater than zero. For example UV might have an arity of 2, while position data
frequently has an arity of 3. 

## Encoding Strings

While there is no explicit string type, one could encode string data by using a data-type uint8 with an arity of a fixed value (say 255) to store short strings. 

## Attribute Descriptor String

Every attribute descriptor has a one to one mapping to a string representation similar to a URN: 
    
    `g3d:<association>:<semantic>:<data_type>:<data_arity>`

This attribute descriptor string is the name of the buffer. 

# Recommended reading:

* [VIM AEC blog post about using G3D with Unity](https://www.vimaec.com/the-g3d-geometry-exchange-format/)
* [Hackernoon article about BFast](https://hackernoon.com/bfast-a-data-format-for-serializing-named-binary-buffers-243p130uw)
* http://assimp.sourceforge.net/lib_html/structai_mesh.html
* http://help.autodesk.com/view/FBX/2017/ENU/?guid=__files_GUID_5EDC0280_E000_4B0B_88DF_5D215A589D5E_htm
* https://help.autodesk.com/cloudhelp/2017/ENU/Max-SDK/cpp_ref/class_mesh.html
* https://help.autodesk.com/view/3DSMAX/2016/ENU/?guid=__files_GUID_CBBA20AD_F7D5_46BC_9F5E_5EDA109F9CF4_htm
* http://paulbourke.net/dataformats/
* http://paulbourke.net/dataformats/obj/
* http://paulbourke.net/dataformats/ply/
* http://paulbourke.net/dataformats/3ds/
* https://github.com/KhronosGroup/gltf
* http://help.autodesk.com/view/FBX/2017/ENU/?guid=__cpp_ref_class_fbx_layer_element_html
