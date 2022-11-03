using System.Collections.Generic;
using System;
using System.Linq;

namespace BasicBilling.API.Extensions
{
    internal static class EnumerableExtensions
    {
        public static IEqualityComparer<TSource> By<TSource, TIdentity>(Func<TSource, TIdentity> identitySelector)
        {
            return new DelegateComparer<TSource, TIdentity>(identitySelector);
        }

        public static IEnumerable<T> DistinctBy<T, TIdentity>(this IEnumerable<T> source,
            Func<T, TIdentity> identitySelector)
        {
            return source.Distinct(By(identitySelector));
        }

        private class DelegateComparer<T, TIdentity> : IEqualityComparer<T>
        {
            private readonly Func<T, TIdentity> _identitySelector;

            public DelegateComparer(Func<T, TIdentity> identitySelector)
            {
                this._identitySelector = identitySelector;
            }

            public bool Equals(T x, T y)
            {
                return Equals(_identitySelector(x), _identitySelector(y));
            }

            public int GetHashCode(T obj)
            {
                return _identitySelector(obj).GetHashCode();
            }
        }
    }
}
