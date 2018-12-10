using System;
using System.Collections.Generic;

namespace Utils
{
    public static class Generators
    {
        public static Func<int> Cycler(int range)
        {
            var current = 0;
            return () => current++ % range;
        }

        public static Func<T> Reader<T>(this IList<T> collection)
        {
            var current = 0;
            return () => collection[current++];
        }
    }
}
