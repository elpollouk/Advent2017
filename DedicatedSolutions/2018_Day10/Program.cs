using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace _2018_Day10
{
    class Point
    {
        public int x;
        public int y;
        public readonly int dx;
        public readonly int dy;

        public Point(int x, int y, int dx, int dy)
        {
            this.x = x;
            this.y = y;
            this.dx = dx;
            this.dy = dy;
        }

        public void Update()
        {
            x += dx;
            y += dy;
        }

        public void Reverse()
        {
            x -= dx;
            y -= dy;
        }

        public override string ToString() => $"{x}, {y}";
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Day 10, 2018!");
            var points = new List<Point>();
            foreach (var line in FileIterator.Lines(args[0]))
            {
                var match = line.Match(@"<(.+),(.+)>.+<(.+),(.+)>");
                var x = int.Parse(match.Groups[1].Value);
                var y = int.Parse(match.Groups[2].Value);
                var dx = int.Parse(match.Groups[3].Value);
                var dy = int.Parse(match.Groups[4].Value);
                points.Add(new Point(x, y, dx, dy));
            }

            Console.WriteLine($"{points.Count()} points");

            var previousWidth = int.MaxValue;
            var previousHeight = int.MaxValue;
            var time = 0;

            // Run the simulation
            while (true)
            {

                var minX = int.MaxValue;
                var maxX = int.MinValue;
                var minY = int.MaxValue;
                var maxY = int.MinValue;

                foreach (var point in points)
                {
                    point.Update();
                    minX = Math.Min(minX, point.x);
                    maxX = Math.Max(maxX, point.x);
                    minY = Math.Min(minY, point.y);
                    maxY = Math.Max(maxY, point.y);
                }

                var currentWidth = maxX - minX;
                var currentHeight = maxY - minY;
                if (previousWidth < currentWidth || previousHeight < currentHeight)
                    break;
                previousWidth = currentWidth;
                previousHeight = currentHeight;
                time++;
            }

            {
                // Rewind 1 tick because I will overshoot the minimum using the above rollout
                // This saves me having to maintain duplicates of all the points data
                var minX = int.MaxValue;
                var minY = int.MaxValue;
                foreach (var point in points)
                {
                    point.Reverse();
                    minX = Math.Min(minX, point.x);
                    minY = Math.Min(minY, point.y);
                }

                // Map it into a 2D space
                var message = new char[previousWidth + 1, previousHeight + 1];
                foreach (var point in points)
                    message[point.x - minX, point.y - minY] = '#';

                // Print it out
                foreach (var coord in message.Rectangle())
                {
                    if (coord.x == 0) Console.WriteLine();
                    var c = message[coord.x, coord.y];
                    if (c == 0) c = ' ';
                    Console.Write(c);
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Time taken: {time}s");
        }
    }
}
