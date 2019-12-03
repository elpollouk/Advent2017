using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2019
{
    using Grid = Dictionary<(int, int), (int pathId, int time)>;

    public class Day03
    {
        const int INTERSECTION = -1;

        void PaintPath(Grid grid, string path, int pathId)
        {
            var movements = path.Split(',');
            int x = 0, y = 0;
            int time = 0;

            foreach (var movement in movements)
            {
                var (dX, dY) = movement[0] switch
                {
                    'U' => (0, 1),
                    'D' => (0, -1),
                    'L' => (-1, 0),
                    'R' => (1, 0),
                    _ => throw new InvalidOperationException()
                };

                int distance = int.Parse(movement.Substring(1));
                for (var i = 0; i < distance; i++)
                {
                    x += dX;
                    y += dY;
                    time++;
                    var key = (x, y);
                    var (p, t) = grid.GetOrDefault(key, (pathId, 0));
                    grid[key] = (p == pathId ? pathId : INTERSECTION, t + time);
                }
            }
        }

        [Theory]
        [InlineData("Data/Day03-example.txt", 159, 610)]
        [InlineData("Data/Day03-example2.txt", 135, 410)]
        [InlineData("Data/Day03-example3.txt", 6, 30)]
        [InlineData("Data/Day03.txt", 896, 16524)]
        public void Problem(string input, int answer1, int answer2)
        {
            var pathId = 1;
            var grid = new Grid();
            FileIterator.ForEachLine<string>(input, path => PaintPath(grid, path, pathId++));

            var crosses = grid.Keys.Where(k => grid[k].pathId == INTERSECTION).ToList();
            var nearest = crosses.Select(k => Math.Abs(k.Item1) + Math.Abs(k.Item2)).Min();
            nearest.Should().Be(answer1);

            var shortest = crosses.Select(c => grid[c].time).Min();
            shortest.Should().Be(answer2);
        }
    }
}
