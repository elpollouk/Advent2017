using System;
using System.Collections.Generic;

namespace Adevent2017.Utils
{
    static class DictionaryExtentions
    {
        public static V GetOrDefault<K, V>(this Dictionary<K, V> dic, K key, V def = default(V))
        {
            V v;
            if (dic.TryGetValue(key, out v))
                return v;
            return def;
        }

        public static V GetOrCreate<K, V>(this Dictionary<K, V> dic, K key, Func<V> creator)
        {
            V value;
            if (!dic.TryGetValue(key, out value))
            {
                value = creator();
                dic[key] = value;
            }
            return value;
        }
    }
}
