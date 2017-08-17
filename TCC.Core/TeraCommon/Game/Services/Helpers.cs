using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Tera.Game
{
    public static class Helpers
    {
        public static Func<T, TResult> Memoize<T, TResult>(Func<T, TResult> func)
        {
            var lookup = new ConcurrentDictionary<T, TResult>();
            return x => lookup.GetOrAdd(x, func);
        }

        internal static void On<T>(this object obj, Action<T> callback)
        {
            if (obj is T)
            {
                var castObject = (T) obj;
                callback(castObject);
            }
        }

        internal class ProjectingEqualityComparer<T, TKey> : Comparer<T>
        {
            private readonly Comparer<TKey> _keyComparer = Comparer<TKey>.Default;
            private readonly Func<T, TKey> _projection;

            public ProjectingEqualityComparer(Func<T, TKey> projection)
            {
                _projection = projection;
            }

            public override int Compare(T x, T y)
            {
                return _keyComparer.Compare(_projection(x), _projection(y));
            }
        }

        // Thanks to Natan Podbielski
        //http://internetexception.com/post/2016/08/05/Faster-then-Reflection-Delegates.aspx
        //http://internetexception.com/post/2016/08/16/Faster-than-Reflection-Delegates-Part-2.aspx
        //http://internetexception.com/post/2016/09/02/Faster-than-Reflection-Delegates-Part-3.aspx
        public static TDelegate Contructor<TDelegate>() where TDelegate : class
        {
            var source = typeof(TDelegate).GetGenericArguments().Where(t => !t.IsGenericParameter).ToArray().Last();
            var ctrArgs = typeof(TDelegate).GetGenericArguments().Where(t => !t.IsGenericParameter).ToArray().Reverse().Skip(1).Reverse().ToArray();
            var constructorInfo = (source.GetConstructor(BindingFlags.Public, null, ctrArgs, null) ??
                                   source.GetConstructor(BindingFlags.NonPublic, null, ctrArgs, null)) ??
                                   source.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, ctrArgs, null);
            if (constructorInfo == null)
            {
                return null;
            }
            var parameters = ctrArgs.Select(Expression.Parameter).ToList();
            return Expression.Lambda(Expression.New(constructorInfo, parameters), parameters).Compile() as TDelegate;
        }
    }
}