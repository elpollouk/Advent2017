using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;
using Xunit;
using Xunit.Abstractions;

namespace Advent2021
{
    public class Day20
    {
        class Image
        {
            int minX = int.MaxValue;
            int minY = int.MaxValue;
            int maxX = int.MinValue;
            int maxY = int.MinValue;
            int unexplored = 0;

            public HashSet<(int x, int y)> points = new();
            readonly bool[] rules = new bool[512];

            public Image(string filename)
            {
                var reader = FileIterator.CreateLineReader(filename);
                var rulesLine = reader();
                for (var i = 0; i < rulesLine.Length; i++)
                    rules[i] = rulesLine[i] == '#';

                reader();

                var y = 0;
                foreach (var line in FileIterator.Lines(reader))
                {
                    for (var x = 0; x < line.Length; x++)
                    {
                        if (line[x] == '#')
                        {
                            points.Add((x, y));
                            if (x < minX) minX = x;
                            else if (x > maxX) maxX = x;
                            if (y < minY) minY = y;
                            else if (y > maxY) maxY = y;
                        }
                    }
                    y++;
                }
            }

            int PointValue(int x, int y)
            {
                if (minX <= x && x <= maxX && minY <= y && y <= maxY)
                    return points.Contains((x, y)) ? 1 : 0;

                // We can't assume unexplored cells are 0, they may all be set to 1 after a step
                // if rules[0] == true
                return unexplored;
            }

            bool Scan(int x, int y)
            {
                int value = PointValue(x - 1, y - 1) << 8;
                value |= PointValue(x, y - 1) << 7;
                value |= PointValue(x + 1, y - 1) << 6;
                value |= PointValue(x - 1, y) << 5;
                value |= PointValue(x, y) << 4;
                value |= PointValue(x + 1, y) << 3;
                value |= PointValue(x - 1, y + 1) << 2;
                value |= PointValue(x, y + 1) << 1;
                value |= PointValue(x + 1, y + 1);

                return rules[value];
            }

            public void Step()
            {
                HashSet<(int x, int y)> newPoints = new();
                var newMinX = int.MaxValue;
                var newMinY = int.MaxValue;
                var newMaxX = int.MinValue;
                var newMaxY = int.MinValue;

                for (var y = minY - 1; y <= maxY + 1; y++)
                {
                    for (var x = minX - 1; x <= maxX + 1; x++)
                    {
                        if (Scan(x, y))
                        {
                            newPoints.Add((x, y));
                            if (x < newMinX) newMinX = x;
                            else if (x > newMaxX) newMaxX = x;
                            if (y < newMinY) newMinY = y;
                            else if (y > newMaxY) newMaxY = y;
                        }
                    }
                }

                points = newPoints;
                minX = newMinX;
                minY = newMinY;
                maxX = newMaxX;
                maxY = newMaxY;

                // After this step, all unexpored areas will be set based on either nothing being set (rules[0])
                // or everything being set (rules[511]) if rules[0] == true
                // i.e. They flip-flop between states each step based on the problem input, but not the example.
                unexplored ^= rules[0] ? 1 : 0;
            }

            public long Run(int iterations)
            {
                while (iterations-- > 0)
                    Step();

                return points.Count;
            }

            public void Render(Action<string> writeLine)
            {
                StringBuilder sb = new();

                for (var y = minY; y <= maxY; y++)
                {
                    sb.Clear();
                    for (var x = minX; x <= maxX; x++)
                    {
                        sb.Append(points.Contains((x, y)) ? '#' : '·');
                    }
                    writeLine(sb.ToString());
                }
            }
        }

        private readonly ITestOutputHelper output;

        public Day20(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("Data/Day20_Test.txt", 2, 35)]
        [InlineData("Data/Day20_Test.txt", 50, 3351)]
        [InlineData("Data/Day20.txt", 2, 5306)]
        [InlineData("Data/Day20.txt", 50, 17497)]
        public void Solution(string filename, int iterations, long expectedAnswer)
        {
            Image image = new(filename);
            var count = image.Run(iterations);
            image.Render(output.WriteLine);
            count.Should().Be(expectedAnswer);
        }
    }
}
