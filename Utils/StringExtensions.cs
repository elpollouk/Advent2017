using System;
using System.Collections.Generic;

namespace Utils
{
    public static class StringExtensions
    {
        public static bool ForEachWord(this string text, Action<string> onWord)
        {
            var words = text.Split(' ');
            try
            {
                foreach (var word in words)
                    onWord(word);

                return true;
            }
            catch (StopIteration)
            {
                return false;
            }
        }

        public static bool ForEachLowerWord(this string text, Action<string> onWord) => text.ToLowerInvariant().ForEachWord(onWord);

        public static T[] SplitAndConvert<T>(this string s, params char[] splitChars)
        {
            var values = new List<T>();
            var tsValues = s.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            foreach (var v in tsValues)
                values.Add((T)Convert.ChangeType(v, typeof(T)));

            return values.ToArray();
        }

        public static T[] SplitAndConvert<T>(this string s) => s.SplitAndConvert<T>(',');

    }
}
