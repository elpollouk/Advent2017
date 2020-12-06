using System;
using System.Collections.Generic;

namespace Utils
{
    public static class DictionaryExtentions
    {
        public static V GetOrDefault<K, V>(this IDictionary<K, V> dic, K key, V def = default)
        {
            if (dic.TryGetValue(key, out V v))
                return v;
            return def;
        }

        public static V GetOrCreate<K, V>(this IDictionary<K, V> dic, K key, Func<V> creator)
        {
            if (!dic.TryGetValue(key, out V value))
            {
                value = creator();
                dic[key] = value;
            }
            return value;
        }

        public static long Increment<K>(this IDictionary<K, long> dic, K key)
        {
            var count = dic.GetOrDefault(key, 0) + 1;
            dic[key] = count;
            return count;
        }

        public static void Index<K, V>(this IDictionary<K, V> dic, IEnumerable<V> collection, Func<V, K> getKey)
        {
            foreach (var item in collection)
                dic[getKey(item)] = item;
        }
    }
}
