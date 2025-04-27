using Ara3D.Collections;
using Ara3D.Mathematics;
using System;

namespace Ara3D.Geometry
{
    public interface IBounded
    {
        AABox Bounds { get; }
    }

    public interface IBounded2D
    {
        AABox2D Bounds { get; }
    }

    /// <summary>
    /// A deformable shape, can accept an arbitrary function from R3 -> R3 and
    /// produce a new shape.
    /// </summary>
    public interface IDeformable<out T> : ITransformable<T>
        where T : IDeformable<T>
    {
        T Deform(Func<Vector3, Vector3> f);
    }

    /// <summary>
    /// A deformable shape, can accept an arbitrary function from R2 -> R2 and
    /// produce a new shape.
    /// </summary>
    public interface IDeformable2D<out T>
        where  T: IDeformable2D<T>
    {
        T Deform2D(Func<Vector2, Vector2> f);
    }

    public interface IGeometry : 
        IDeformable<IGeometry>
    { }

    /// <summary>
    /// A surface is a 3-dimensional shape with no volume.
    /// Some examples of surfaces are: plane, sphere, cylinder, cone, torus.  
    /// </summary>
    public interface ISurface : IGeometry
    {
        bool ClosedX { get; }
        bool ClosedY { get; }
    }

    /// <summary>
    /// A bounded surface is a 2D sampling of a surface with
    /// an approximate bounded-box.
    /// The bounding box is intended for use with proportional deformations. 
    /// </summary>
    public interface IBoundedSurface : ISurface
    {
        AABox Bounds { get; }
        ISurface Surface { get; }
    }

    public interface IProcedural<TIn, TOut>
    {
        TOut Eval(TIn x);
    }

    /// <summary>
    /// A parametric surface maps UV coordinates to 3-dimensional points in space.
    /// Any explicit surface can be combined with a parametric surface by interpreting
    /// the normals.  
    /// </summary>
    public interface IParametricSurface : IProcedural<Vector2, Vector3>, ISurface
    { }

    public interface IColorGradient : IProcedural<float, Vector4> 
    { }
    
    public interface IHeightMap : IProcedural<Vector2, float> 
    { }
    
    public interface IDistanceField2D : IProcedural<Vector2, float> 
    { }
    
    public interface IVectorField2D : IProcedural<Vector2, Vector2> 
    { }

    public interface IVolume : IProcedural<Vector3, bool> 
    { }
    
    public interface IDistanceField3D : IProcedural<Vector3, float> 
    { }
    
    public interface IProceduralVectorField3D : IProcedural<Vector3, Vector3> 
    { }

    public interface IMask : IProcedural<Vector2, bool> 
    { }
    
    public interface IField : IProcedural<Vector2, float> 
    { }
    
    public interface IBump : IProcedural<Vector2, Vector3> 
    { }
    
    public interface IProceduralTexture : IProcedural<Vector2, Vector4> 
    { }
    
    public interface ICurve3D : ICurve<Vector3> 
    { }

    public interface ICurve<T> : IProcedural<float, T>
    {
        bool Closed { get; }
    }

    public interface ICurve2D : ICurve<Vector2> 
    { }

    public interface IPoints
        : IGeometry
    {
        IArray<Vector3> Points { get; }
    }

    public interface IPolyLine<T> 
    {
        bool Closed { get; }
        IArray<T> Points { get; }
        ILineSegment<T> Segment(int i);
    }

    public interface IPolyLine2D : IPolyLine<Vector2>, IDeformable2D<IPolyLine2D>
    { }

    public interface IPolyLine3D : IPolyLine<Vector3>, IDeformable<IPolyLine3D>
    { }

    public interface IPolygon : IPolyLine2D, IDeformable2D<IPolygon>
    { }

    public interface IMesh<T>
        : IPoints
    {
        IArray<T> FaceIndices { get; }
        IArray<int> Indices { get; }
    }

    public interface ITriMesh
        : IMesh<Int3>, IDeformable<ITriMesh>
    { }

    public interface IQuadMesh
        : IMesh<Int4>, IDeformable<IQuadMesh>
    { }
}