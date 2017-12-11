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
            var values = new List<T>();
            ForEachLine<string>(filename, line =>
            {
                var tsValues = line.SplitAndConvert<T>('\t');
                foreach (var v in tsValues)
                    values.Add(v);
            });
            return values.ToArray();
        }

        public static T[] LoadCSV<T>(string filename)
        {
            var values = new List<T>();
            ForEachLine<string>(filename, line =>
            {
                var tsValues = line.SplitAndConvert<T>();
                foreach (var v in tsValues)
                    values.Add(v);
            });
            return values.ToArray();
        }
    }
}
