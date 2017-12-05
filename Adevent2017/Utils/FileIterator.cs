using System;
using System.IO;

namespace Adevent2017.Utils
{
    static class FileIterator
    {
        public static void ForEachLine(string filename, Action<string> onLine)
        {
            var reader = new StreamReader(filename);
            string line;
            while ((line = reader.ReadLine()) != null)
                onLine(line);
        }

        public static void ForEachInt(string filename, Action<int> onInt)
        {
            ForEachLine(filename, line =>
            {
                onInt(int.Parse(line));
            });
        }
    }
}
