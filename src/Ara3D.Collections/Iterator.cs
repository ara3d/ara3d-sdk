using System;
using System.Diagnostics;

namespace Ara3D.Collections
{
    /// <summary>
    /// This class is a great example, of something that is simple,
    /// correct, robust, but inefficient, purely because of how
    /// the C# compiler works. 
    /// </summary>
    public class Iterator<T, TState> : IIterator<T>
    {
        private readonly TState _state;
        private readonly Func<TState, (TState, bool, T)> _func;
        public Iterator(TState state, Func<TState, (TState, bool, T)> func)
            => (_state, _func) = (state, func);
        public T Value => _func(_state).Item3;
        public bool HasValue => _func(_state).Item2;
        public IIterator<T> Next => new Iterator<T, TState>(_func(_state).Item1, _func);
    }

    public readonly struct WhereIndexIterator<T>
        : IIterator<T>
    {
        public WhereIndexIterator(IIterator<T> source, Func<T, int, bool> predicate, int index = 0)
        {
            while (source.HasValue && !predicate(source.Value, index))
            {
                source = source.Next;
                index++;
            }
            Predicate = predicate;
            Source = source;
            Index = index;
        }

        public int Index { get; }
        public IIterator<T> Source { get; }
        public Func<T, int, bool> Predicate { get; }
        public IIterator<T> Next => new WhereIndexIterator<T>(Source.Next, Predicate, Index);
        public bool HasValue => Source.HasValue;
        public T Value => Source.Value;
    }

    public readonly struct SelectIndexIterator<T0, T1>
        : IIterator<T1>
    {
        public SelectIndexIterator(IIterator<T0> source, Func<T0, int, T1> map, int index = 0)
            => (Source, Map, Index) = (source, map, index);
        public IIterator<T0> Source { get; }
        public Func<T0, int, T1> Map { get; }
        public int Index { get; }
        public IIterator<T1> Next => new SelectIndexIterator<T0, T1>(Source, Map, Index + 1);
        public bool HasValue => Source.HasValue;
        public T1 Value => Map(Source.Value, Index);
    }

    [DebuggerDisplay("[{Value}]")]
    public readonly struct SingleSequence<T> : IIterator<T>, ISet<T>, IArray<T>
    {
        public SingleSequence(T value) => Value = value;
        public T Value { get; }
        public IIterator<T> Next => EmptySequence<T>.Default;
        public bool HasValue => true;
        public IIterator<T> Iterator => this;
        public int Count => 1;
        public T this[int n] => Value;
        public bool Contains(T item) => item?.Equals(Value) ?? false;
    }

    [DebuggerDisplay("[{Value} * {Count}]")]
    public readonly struct RepeatedSequence<T> : IIterator<T>, ISet<T>, IArray<T>
    {
        public RepeatedSequence(T value, int count) => (Value, Count) = (value, count);
        public T Value { get; }
        public int Count { get; }
        public IIterator<T> Next => Count > 0 
            ? new RepeatedSequence<T>(Value, Count - 1) 
            : throw new ArgumentOutOfRangeException();
        public bool HasValue => Count > 0;
        public IIterator<T> Iterator => this;
        public T this[int n] => Value;
        public bool Contains(T item) => item?.Equals(Value) ?? false;
    }

    [DebuggerDisplay("[{From} .. {To})")]
    public readonly struct RangeSequence : IRange, IIterator<int>
    {
        public RangeSequence(int from, int count) => (From, Count) = (from, count);
        public int From { get; }
        public int Count { get; }
        public int this[int input] => From + input;
        public IIterator<int> Iterator => this;
        public bool HasValue => Count > 0;
        public int Value => From;
        public int To => From + Count;
        public IIterator<int> Next => new RangeSequence(From + 1, Count - 1);
        public bool Contains(int item) => item >= From && item <= Count + From;
        public IComparer<int> Ordering => Orderings.IntegerOrder;
        public int FindKey(int n) => n < From || n >= Count ? -1 : n - From;
    }

    public readonly struct TakeIterator<T>
        : IIterator<T>
    {
        public TakeIterator(IIterator<T> source, Func<T, int, bool> predicate, int index = 0) 
            => (Source, Predicate, Index) = (source, predicate, index);
        public IIterator<T> Source { get; }
        public Func<T, int, bool> Predicate { get; }
        public int Index { get; }
        public IIterator<T> Next => new TakeIterator<T>(Source.Next, Predicate, Index + 1);
        public bool HasValue => Source.HasValue && Predicate(Source.Value, Index);
        public T Value => Source.Value;
    }

    public readonly struct ConcatIterator<T>
        : IIterator<T>
    {
        public ConcatIterator(IIterator<T> source1, IIterator<T> source2) 
            => (Source1, Source2) = (source1, source2);
        public IIterator<T> Source1 { get; }
        public IIterator<T> Source2 { get; }
        public IIterator<T> Next => Source1.Next.HasValue ? Source1.Next : Source2;
        public bool HasValue => Source1.HasValue;
        public T Value => Source1.Value;
    }

    public readonly struct SelectManyIterator<T0, T1>
        : IIterator<T1>
    {
        public SelectManyIterator(IIterator<T0> source, Func<T0, ISequence<T1>> selector)
            : this(source, selector, new EmptySequence<T1>())
        { }

        public SelectManyIterator(IIterator<T0> source, Func<T0, ISequence<T1>> selector, IIterator<T1> current)
        {
            while (!current.HasValue && source.HasValue)
            {
                current = selector(source.Value).Iterator;
            }

            Selector = selector;
            Source = source;
            Current = current;
        }

        public IIterator<T0> Source { get; }
        public IIterator<T1> Current { get; }
        public Func<T0, ISequence<T1>> Selector { get; }
        public IIterator<T1> Next => new SelectManyIterator<T0, T1>(Source, Selector, Current.Next);
        public T1 Value => Current.Value;
        public bool HasValue => Current.HasValue;
        public IIterator<T1> Iterator => this;
    }

    public readonly struct Map<T0, T1> : IMap<T0, T1>
    {
        public Map(Func<T0, T1> func) => Function = func;
        private Func<T0, T1> Function { get; }
        public T1 this[T0 item] => Function(item);
    }

    public readonly struct Comparer<T> : IComparer<T>
    {
        public Comparer(Func<T, T, int> func) => Func = func;
        public Func<T, T, int> Func { get; }

        public int Compare(T x, T y)
            => x == null || y == null ? 0 : Func(x, y);
    }

    public static class Orderings
    {
        public static IComparer<int> IntegerOrder { get; }
            = new Comparer<int>((a, b) => b - a);
    }
}