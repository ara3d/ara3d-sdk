namespace Plato
{
    public interface IArray3D<T> : IArray<T>
    {
        Integer NumRows { get; }
        Integer NumColumns { get; }
        Integer NumLayers { get; }
        T At(Integer col, Integer row, Integer layer);
        T this[Integer col, Integer row, Integer layer] { get; }
    }
}