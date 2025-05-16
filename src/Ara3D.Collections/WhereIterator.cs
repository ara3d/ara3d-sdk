using System;

namespace Ara3D.Collections
{
    public class WhereIterator<T1> : IIterator<T1>
    {
        private readonly IIterator<T1> _iter;
        private readonly Func<T1, bool> _predicate;

        public WhereIterator(IIterator<T1> iter, Func<T1, bool> f)
            => (_iter, _predicate) = (iter.SkipWhileNot(f), f);

        public T1 Value => _iter.Value;

        public bool HasValue => _iter.HasValue;

        public IIterator<T1> Next => new WhereIterator<T1>(_iter.Next, _predicate);
    }
}