using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day05
    {
        record Line((int x, int y) From, (int x, int y) To);

        static Line[] ParseLines(string inputFile)
        {
            List<Line> lines = new();

            FileIterator.ForEachLine<string>(inputFile, line => {
                var fromTo = line.Split(" -> ");
                var parts = fromTo[0].Split(",");
                int fromX = int.Parse(parts[0]);
                int fromY = int.Parse(parts[1]);
                parts = fromTo[1].Split(",");
                int toX = int.Parse(parts[0]);
                int toY = int.Parse(parts[1]);

                lines.Add(new((fromX, fromY), (toX, toY)));
            });

            return lines.ToArray();
        }

        static void RenderPart1(Dictionary<(int, int), long> area, Line line)
        {
            if (line.From.x == line.To.x)
            {
                int x = line.From.x;
                int from = line.From.y;
                int to = line.To.y;
                if (to < from)
                {
                    (from, to) = (to, from);
                }

                for (int y = from; y <= to; y++)
                {
                    area.Increment((x, y));
                }
            }
            else if (line.From.y == line.To.y)
            {
                int y = line.From.y;
                int from = line.From.x;
                int to = line.To.x;
                if (to < from)
                {
                    (from, to) = (to, from);
                }

                for (int x = from; x <= to; x++)
                {
                    area.Increment((x, y));
                }
            }
        }

        static void RenderPart2(Dictionary<(int, int), long> area, Line line)
        {
            int dX = (line.To.x < line.From.x) ? -1 : ((line.To.x == line.From.x) ? 0 : 1);
            int dY = (line.To.y < line.From.y) ? -1 : ((line.To.y == line.From.y) ? 0 : 1);

            int x = line.From.x;
            int y = line.From.y;
            while (x != line.To.x || y != line.To.y)
            {
                area.Increment((x, y));
                x += dX;
                y += dY;
            }

            area.Increment((line.To));
        }

        [Theory]
        [InlineData("Data/Day05_Test.txt", 5)]
        [InlineData("Data/Day05.txt", 7085)]
        public void Part1(string inputFile, int expectedAnswer)
        {
            Dictionary<(int, int), long> area = new();
            var lines = ParseLines(inputFile);
            foreach (var line in lines)
            {
                RenderPart1(area, line);
            }

            int count = area.Values.Where(v => v > 1).Count();
            count.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day05_Test.txt", 12)]
        [InlineData("Data/Day05.txt", 20271)]
        public void Part2(string inputFile, int expectedAnswer)
        {
            Dictionary<(int, int), long> area = new();
            var lines = ParseLines(inputFile);
            foreach (var line in lines)
            {
                RenderPart2(area, line);
            }

            int count = area.Values.Where(v => v > 1).Count();
            count.Should().Be(expectedAnswer);
        }
    }
}
