using Plato;

namespace Ara3D.Models;

public interface ITransformable3D<out T> where T : ITransformable3D<T>
{
    T Transform(Matrix4x4 matrix);
}