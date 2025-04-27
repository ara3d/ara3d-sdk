namespace Plato
{
    public interface IArray<T> : IReadOnlyList<T>
    {
        Integer Count { get; }
        T At(Integer n); 
        T this[Integer n] { get; }
    }
}