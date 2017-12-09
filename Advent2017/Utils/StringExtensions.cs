using System;

namespace Adevent2017.Utils
{
    static class StringExtensions
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
    }
}
