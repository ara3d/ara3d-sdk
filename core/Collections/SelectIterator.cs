using System;

namespace Ara3D.Collections
{
    public class SelectIterator<T1, T2> : IIterator<T2>
    {
        private readonly IIterator<T1> _iter;
        private readonly Func<T1, T2> _func;

        public SelectIterator(IIterator<T1> iter, Func<T1, T2> f)
            => (_iter, _func) = (iter, f);

        public T2 Value => _func(_iter.Value);

        public bool HasValue => _iter.HasValue;

        public IIterator<T2> Next => new SelectIterator<T1, T2>(_iter.Next, _func);
    }
}