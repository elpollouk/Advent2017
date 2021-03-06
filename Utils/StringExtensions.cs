﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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

        public static Match Match(this string s, string regex)
        {
            var _regex = new Regex(regex);
            return _regex.Match(s);
        }

        public static bool IsMatch(this string s, string regex)
        {
            var _regex = new Regex(regex);
            return _regex.IsMatch(s);
        }

        public static string[] Groups(this string s, string regex)
        {
            var _regex = new Regex(regex);
            var groups =_regex.Match(s).Groups;
            var result = new string[groups.Count - 1];
            for (var i = 0; i < result.Length; i++)
                result[i] = groups[i + 1].Value;
            return result;
        }

        public static string ToHexString(this byte[] buffer)
        {
            StringBuilder hex = new StringBuilder(buffer.Length * 2);
            foreach (byte b in buffer)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
