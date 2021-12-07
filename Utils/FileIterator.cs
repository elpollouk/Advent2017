using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Utils
{
    public static class FileIterator
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

        public static IEnumerable<T> Lines<T>(string filename)
        {
            foreach (var line in Lines(filename))
                yield return (T)Convert.ChangeType(line, typeof(T));
        }

        public static IEnumerable<string> Lines(string filename)
        {
            using (var reader = new StreamReader(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    yield return line;
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

        public static T[] LoadSSV<T>(string filename)
        {
            var values = new List<T>();
            ForEachLine<string>(filename, line =>
            {
                var tsValues = line.SplitAndConvert<T>(' ');
                foreach (var v in tsValues)
                    values.Add(v);
            });
            return values.ToArray();
        }

        public static T[,] LoadGrid<T>(string filename, Func<char, int, int, T> cellMapper)
        {
            var lines = LoadLines<string>(filename);
            var grid = new T[lines[0].Length, lines.Length];

            foreach (var (x, y) in grid.Rectangle())
                grid[x, y] = cellMapper(lines[y][x], x, y);

            return grid;
        }

        public static Func<string> CreateLineReader(string filename)
        {
            var lines = Lines(filename).ToArray();
            var index = 0;
            return () =>
            {
                if (index == lines.Length) return null;
                return lines[index++];
            };
        }
    }
}
