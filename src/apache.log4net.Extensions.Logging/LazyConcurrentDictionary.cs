using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace apache.log4net.Extensions.Logging
{
    class LazyConcurrentDictionary<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, Lazy<TValue>> _dictionary;

        public LazyConcurrentDictionary(IEqualityComparer<TKey> comparer)
        {
            _dictionary = new ConcurrentDictionary<TKey, Lazy<TValue>>(comparer);
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            var lazy = _dictionary.GetOrAdd(key,
                x => new Lazy<TValue>(() => valueFactory(x), LazyThreadSafetyMode.ExecutionAndPublication));

            return lazy.Value;
        }

        public void Clear()
        {
            _dictionary.Clear();
        }
    }
}
