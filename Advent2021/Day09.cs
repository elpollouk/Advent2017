using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day09
    {
        static int GetRiskLevel(int[,] area, (int x, int y) pos)
        {
            var locationHeight = area[pos.x, pos.y];
            if (locationHeight >= 9) return 0;

            if (pos.x != 0 && area[pos.x - 1, pos.y] <= locationHeight) return 0;
            if (pos.x != area.GetLength(0) - 1 && area[pos.x + 1, pos.y] <= locationHeight) return 0;
            if (pos.y != 0 && area[pos.x, pos.y - 1] <= locationHeight) return 0;
            if (pos.y != area.GetLength(1) - 1 && area[pos.x, pos.y + 1] <= locationHeight) return 0;

            return locationHeight + 1;
        }

        static int Fill(HashSet<(int, int)> visited, int[,] area, (int x, int y) pos)
        {
            if (visited.Contains(pos)) return 0;
            if (area[pos.x, pos.y] == 9) return 0;

            int size = 0;
            Queue<(int x, int y)> q = new();
            q.Enqueue(pos);

            while (q.Count != 0)
            {
                pos = q.Dequeue();
                if (visited.Contains(pos)) continue;
                if (area[pos.x, pos.y] == 9) continue;
                size++;
                visited.Add(pos);

                if (pos.x != 0) q.Enqueue((pos.x -1, pos.y));
                if (pos.x != area.GetLength(0) - 1) q.Enqueue((pos.x + 1, pos.y));
                if (pos.y != 0) q.Enqueue((pos.x, pos.y - 1));
                if (pos.y != area.GetLength(1) - 1) q.Enqueue((pos.x, pos.y + 1));
            }

            return size;
        }

        [Theory]
        [InlineData("Data/Day09_Test.txt", 15)]
        [InlineData("Data/Day09.txt", 489)]
        public void Part1(string filename, int expectedAnswer)
        {
            var area = FileIterator.LoadGrid(filename, (c, _, _) => c - '0');
            area.Rectangle()
                .Select(pos => GetRiskLevel(area, pos))
                .Sum()
                .Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day09_Test.txt", 1134)]
        [InlineData("Data/Day09.txt", 1056330)]
        public void Part2(string filename, int expectedAnswer)
        {
            var area = FileIterator.LoadGrid(filename, (c, _, _) => c - '0');
            HashSet<(int, int)> visited = new();

            area.Rectangle()
                .Select(pos => Fill(visited, area, pos))
                .Where(size => size != 0)
                .OrderByDescending(size => size)
                .Take(3)
                .Aggregate((x, y) => x * y)
                .Should().Be(expectedAnswer);
        }
    }
}
