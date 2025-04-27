using System;
using Ara3D.Mathematics;

namespace Ara3D.Geometry
{


    public class Procedural<T1, T2> : IProcedural<T1, T2>
    {
        private readonly Func<T1, T2> _func;
        public Procedural(Func<T1, T2> func) => _func = func;
        public T2 Eval(T1 x) => _func(x);
    }

    public class Procedural3D<T> : Procedural<T, Vector3>
    {
        public Procedural3D(Func<T, Vector3> func) : base(func)
        { }
    }

    public class Procedural2D<T> : Procedural<T, Vector2>
    {
        public Procedural2D(Func<T, Vector2> func) : base(func)
        { }
    }

    public static class Procedurals
    {

        public static IProcedural<T1, T2> ToProcedural<T1,T2>(this Func<T1, T2> f)
            => new Procedural<T1, T2>(f);

        public static IProcedural<T1, T4> Zip<T1, T2, T3, T4>(
            this IProcedural<T1, T2> p1, 
            IProcedural<T1, T3> p2,
            Func<T2, T3, T4> f)
            => ToProcedural<T1, T4>(x => f(p1.Eval(x), p2.Eval(x)));

        public static IProcedural<T1, T3> Select<T1, T2, T3>(
            this IProcedural<T1, T2> self, 
            Func<T2, T3> f)
            => ToProcedural<T1, T3>(x => f(self.Eval(x)));
        
        public static IProcedural<T3, T2> Remap<T1, T2, T3>(
            this IProcedural<T1, T2> self, 
            Func<T3, T1> f)
            => ToProcedural<T3, T2>(x => self.Eval(f(x)));
    }
}
