using System;
using System.Collections.Generic;

namespace Utils
{
    public static class DictionaryExtentions
    {
        public static V GetOrDefault<K, V>(this IDictionary<K, V> dic, K key, V def = default(V))
        {
            if (dic.TryGetValue(key, out V v))
                return v;
            return def;
        }

        public static V GetOrCreate<K, V>(this IDictionary<K, V> dic, K key, Func<V> creator)
        {
            V value;
            if (!dic.TryGetValue(key, out value))
            {
                value = creator();
                dic[key] = value;
            }
            return value;
        }

        public static void Index<K, V>(this IDictionary<K, V> dic, IEnumerable<V> collection, Func<V, K> getKey)
        {
            foreach (var item in collection)
                dic[getKey(item)] = item;
        }
    }
}
