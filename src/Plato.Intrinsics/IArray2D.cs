namespace Plato
{
    public interface IArray2D<T> : IArray<T>
    {
        Integer NumRows { get; }
        Integer NumColumns { get; }
        T At(Integer col, Integer row);
        T this[Integer col, Integer row] { get; }
    }
}