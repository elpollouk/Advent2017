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

        public static Func<T> CreateCycler<T>(this IEnumerable<T> collection)
        {
            var enumerator = collection.GetEnumerator();
            return () =>
            {
                if (!enumerator.MoveNext())
                {
                    enumerator.Reset();
                    enumerator.MoveNext();
                }
                return enumerator.Current;
            };
        }

        public static IEnumerable<T> Cycle<T>(this IEnumerable<T> range)
        {
            while (true)
                foreach (var v in range)
                    yield return v;
        }

        public static Func<T> Reader<T>(this IList<T> collection)
        {
            var current = 0;
            return () => collection[current++];
        }

        public static Func<char> Reader(this string text)
        {
            var current = 0;
            return () => text[current++];
        }

        public static Func<T> Reader<T>(params T[] collection)
        {
            var current = 0;
            return () => collection[current++];
        }

        public static IEnumerable<(int x, int y)> Rectangle(int width, int height)
        {
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    yield return (x, y);
        }

        public static IEnumerable<(int x, int y)> Rectangle((int x, int y) from, (int x, int y) to)
        {
            for (var y = from.y; y < to.y; y++)
                for (var x = from.x; x < to.x; x++)
                    yield return (x, y);
        }

        public static IEnumerable<(int x, int y)> Rectangle(this Array array) => Rectangle(array.GetLength(0), array.GetLength(1));

        public static IEnumerable<((int x, int y) pos, T item)> Iterate<T>(this T[,] array)
        {
            foreach (var pos in array.Rectangle())
                yield return (pos, array[pos.x, pos.y]);
        }
    }
}
