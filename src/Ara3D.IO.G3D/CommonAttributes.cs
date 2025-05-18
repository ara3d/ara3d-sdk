namespace Ara3D.IO.G3D
{
    public static class CommonAttributes
    {

        public const string ObjectFaceSize = "g3d:all:facesize:0:int32:1";
        public const string Index = "g3d:corner:index:0:int32:1";
        public const string Position = "g3d:vertex:position:0:float32:3";
        public const string VertexUv = "g3d:vertex:uv:0:float32:2";
        public const string VertexUvw = "g3d:vertex:uv:0:float32:3";
        public const string VertexNormal = "g3d:vertex:normal:0:float32:3";
        public const string VertexColor = "g3d:vertex:color:0:float32:4";
        public const string VertexColor8Bit = "g3d:vertex:color:0:int8:4";
        public const string VertexBitangent = "g3d:vertex:bitangent:0:float32:3";
        public const string VertexTangent = "g3d:vertex:tangent:0:float32:4";
        public const string VertexSelectionWeight = "g3d:vertex:weight:0:float32:1";
        public const string FaceColor = "g3d:face:color:0:float32:4";
        public const string FaceMaterial = "g3d:face:material:0:int32:1";
        public const string FaceNormal = "g3d:face:normal:0:float32:3";
        public const string MeshSubmeshOffset = "g3d:mesh:submeshoffset:0:int32:1";
        public const string InstanceTransform = "g3d:instance:transform:0:float32:16";
        public const string InstanceParent = "g3d:instance:parent:0:int32:1";
        public const string InstanceMesh = "g3d:instance:mesh:0:int32:1";
        public const string InstanceFlags = "g3d:instance:flags:0:uint16:1";
        public const string LineTangentIn = "g3d:vertex:tangent:0:float32:3";
        public const string LineTangentOut = "g3d:vertex:tangent:1:float32:3";
        public const string ShapeVertex = "g3d:shapevertex:position:0:float32:3";
        public const string ShapeVertexOffset = "g3d:shape:vertexoffset:0:int32:1";
        public const string ShapeColor = "g3d:shape:color:0:float32:4";
        public const string ShapeWidth = "g3d:shape:width:0:float32:1";
        public const string MaterialColor = "g3d:material:color:0:float32:4";
        public const string MaterialGlossiness = "g3d:material:glossiness:0:float32:1";
        public const string MaterialSmoothness = "g3d:material:smoothness:0:float32:1";
        public const string SubmeshIndexOffset = "g3d:submesh:indexoffset:0:int32:1";
        public const string SubmeshMaterial = "g3d:submesh:material:0:int32:1";
    }
}
