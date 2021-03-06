﻿using System;
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

        public static IEnumerable<(int x, int y)> Rectangle(this Array array) => Rectangle(array.GetLength(0), array.GetLength(1));
    }
}
