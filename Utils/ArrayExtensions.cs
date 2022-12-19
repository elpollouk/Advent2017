using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Utils
{
    public static class ArrayExtensions
    {
        public static int CharToInt(char c, int x, int y) => c - '0';
        public static char IntToChar(int v) => (char)(v + '0');

        public static string Join<T>(this T[] a, string joiner = ",")
        {
            if (a.Length == 0) return string.Empty;

            var builder = new StringBuilder();
            builder.Append(a[0]);
            for (var i = 1; i < a.Length; i++)
            {
                builder.Append(joiner);
                builder.Append(a[i]);
            }
            return builder.ToString();
        }

        public static T GetAtMod<T>(this IList<T> a, int index) => a[index % a.Count];
        public static char GetAtMod(this string s, int index) => s[index % s.Length];

        public static void SetAtMod<T>(this IList<T> a, int index, T value) => a[index % a.Count] = value;

        public static void SetAll<T>(this T[] a, T value)
        {
            for (int i = 0; i < a.Length; i++)
                a[i] = value;
        }

        // I know IList isn't an array. Fuck you.
        public static T Shift<T>(this IList<T> list)
        {
            var v = list[0];
            list.RemoveAt(0);
            return v;
        }

        public static IList<T> Clone<T>(this IList<T> list)
        {
            var clone = new List<T>();
            foreach (var v in list)
                clone.Add(v);

            return clone;
        }

        public static int ArgMax<T>(this IList<T> list) where T : IComparable<T>
        {
            var maxItem = list[0];
            var maxIndex = 0;

            for (var i = 1; i < list.Count; i++)
            {
                if (maxItem.CompareTo(list[i]) < 0)
                {
                    maxItem = list[i];
                    maxIndex = i;
                }
            }

            return maxIndex;
        }

        public static T MaxItem<T>(this IEnumerable<T> list, Func<T, IComparable> valueGetter)
        {
            var maxItem = list.First();
            var maxValue = valueGetter(maxItem);

            foreach (var item in list)
            {
                var value = valueGetter(item);
                if (maxValue.CompareTo(value) < 0)
                {
                    maxItem = item;
                    maxValue = value;
                }
            }

            return maxItem;
        }

        public static (T, T) MinAndMax<T>(this IEnumerable<T> list) where T: IComparable
        {
            var min = list.First();
            var max = list.First();
            foreach (var value in list)
            {
                if (value.CompareTo(min) < 0) min = value;
                if (value.CompareTo(max) > 0) max = value;
            }

            return (min, max);
        }

        public static long Product(this IEnumerable<long> list)
        {
            if (list.Count() == 0) return 0;

            long total = 1;

            foreach (var v in list)
                total *= v;

            return total;
        }

        public static void Swap<T>(this T[] a, int i1, int i2)
        {
            (a[i2], a[i1]) = (a[i1], a[i2]);
        }

        public static IEnumerable<T> GetNeighbours<T>(this T[,] grid, int x, int y)
        {
            foreach (var pos in grid.GetNeighbourPos(x, y))
                yield return grid[pos.x, pos.y];
        }

        public static IEnumerable<(int x, int y)> GetNeighbourPos<T>(this T[,] grid, int x, int y)
        {
            if (1 <= y && y <= grid.GetLength(1))
            {
                if (1 <= x && x <= grid.GetLength(0))
                    yield return (x - 1, y - 1);

                if (0 <= x && x <= grid.GetLength(0) - 1)
                    yield return (x, y - 1);

                if (-1 <= x && x <= grid.GetLength(0) - 2)
                    yield return (x + 1, y - 1);
            }

            if (0 <= y && y <= grid.GetLength(1) - 1)
            {
                if (1 <= x && x <= grid.GetLength(0))
                    yield return (x - 1, y);

                if (-1 <= x && x <= grid.GetLength(0) - 2)
                    yield return (x + 1, y);
            }

            if (-1 <= y && y <= grid.GetLength(1) - 2)
            {
                if (1 <= x && x <= grid.GetLength(0))
                    yield return (x - 1, y + 1);

                if (0 <= x && x <= grid.GetLength(0) - 1)
                    yield return (x, y + 1);

                if (-1 <= x && x <= grid.GetLength(0) - 2)
                    yield return (x + 1, y + 1);
            }
        }

        public static IEnumerable<T> GetAdjacent<T>(this T[,] grid, int x, int y)
        {
            foreach (var pos in grid.GetAdjacentPos(x, y))
                yield return grid[pos.x, pos.y];
        }

        public static IEnumerable<(int x, int y)> GetAdjacentPos<T>(this T[,] grid, int x, int y)
        {
            if (y > 0)
                yield return (x, y - 1);

            if (x > 0)
                yield return (x - 1, y);

            if (x < grid.GetLength(0) - 1)
                yield return (x + 1, y);

            if (y < grid.GetLength(1) - 1)
                yield return (x, y + 1);
        }

        public static (int, int) Shape<T>(this T[,] array)
        {
            return (array.GetLength(0), array.GetLength(1));
        }

        public static IEnumerable<T> Items<T>(this T[,] array)
        {
            foreach (var (x, y) in array.Rectangle())
                yield return array[x, y];
        }

        public static string FlattenToString<T>(this T[,] grid, Func<T, char> converter)
        {
            var builder = new StringBuilder();

            foreach (var value in grid.Items())
                builder.Append(converter(value));

            return builder.ToString();
        }

        public static void DebugDump<T>(this T[,] grid, Func<T, char> charMapper)
        {
            DebugDump(grid, s => Debug.WriteLine(s), charMapper);
        }

        public static void DebugDump<T>(this T[,] grid, Action<string> output, Func<T, char> charMapper)
        {
            var builder = new StringBuilder();

            for (var y = 0; y < grid.GetLength(1); y++)
            {
                builder.Clear();
                for (var x = 0; x < grid.GetLength(0); x++)
                {
                    builder.Append(charMapper(grid[x, y]));
                }
                output(builder.ToString());
            }
        }

        public static IEnumerable<T[]> Combinations<T>(this T[] a, int size, T[] buffer = null)
        {
            buffer ??= new T[size];
            return CombinationsInternal(a, 0, buffer, 0);
        }

        private static IEnumerable<T[]> CombinationsInternal<T>(T[] src, int srcIndex, T[] buffer, int bufferIndex)
        {
            if (bufferIndex == buffer.Length)
            {
                yield return buffer;
                yield break;
            }

            int maxSourceIndex = src.Length - (buffer.Length - bufferIndex);
            for (; srcIndex <= maxSourceIndex; srcIndex++)
            {
                buffer[bufferIndex] = src[srcIndex];
                foreach (var t in CombinationsInternal(src, srcIndex+1, buffer, bufferIndex+1))
                {
                    yield return t;
                }
            }
        }
    }
}
