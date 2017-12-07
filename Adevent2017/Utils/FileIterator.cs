using System;
using System.Collections.Generic;
using System.IO;

namespace Adevent2017.Utils
{
    static class FileIterator
    {
        public static void ForEachLine<T>(string filename, Action<T> onLine)
        {
            using (var reader = new StreamReader(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    onLine((T)Convert.ChangeType(line, typeof(T)));
            }
        }

        public static T[] LoadLines<T>(string filename)
        {
            var lines = new List<T>();
            Action<T> handler = line => lines.Add(line);
            ForEachLine(filename, handler);
            return lines.ToArray();
        }

        public static T[] LoadTSV<T>(string filename)
        {
            using (var reader = new StreamReader(filename))
            {
                var line = reader.ReadLine();
                var sValues = line.Split('\t');
                var values = new T[sValues.Length];
                for (var i = 0; i < sValues.Length; i++)
                    values[i] = (T)Convert.ChangeType(sValues[i], typeof(T));

                return values;
            }
        }
    }
}
