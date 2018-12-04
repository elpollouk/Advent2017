using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public static class ArrayExtensions
    {
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

        public static T GetAtMod<T>(this T[] a, int index) => a[index % a.Length];

        public static void SetAtMod<T>(this T[] a, int index, T value) => a[index % a.Length] = value;

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

        public static int ArgMax<T>(this IList<T> list) where T : IComparable
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
    }
}
