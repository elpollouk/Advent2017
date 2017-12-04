using System;
using System.IO;

namespace Adevent2017.Utils
{
    static class FileIterator
    {
        public static void EachLine(string filename, Action<string> onLine)
        {
            var reader = new StreamReader(filename);
            string line;
            while ((line = reader.ReadLine()) != null)
                onLine(line);
        }
    }
}
