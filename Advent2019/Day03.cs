using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2019
{
    using Grid = Dictionary<(int, int), (int count, int time)>;

    public class Day03
    {
        void PaintLine(Grid grid, int x1, int y1, int x2, int y2, ref int time)
        {
            if (x1 == x2)
                for (var y = y1; y <= y2; y++)
                {
                    grid[(x1, y)] = (1, time++);
                }
            else
                for (var x = x1; x <= x2; x++)
                {
                    grid[(x, y1)] = (1, time++);
                }
        }

        void PaintLineReverse(Grid grid, int x1, int y1, int x2, int y2, ref int time)
        {
            if (x1 == x2)
                for (var y = y1; y >= y2; y--)
                {
                    grid[(x1, y)] = (1, time++);
                }
            else
                for (var x = x1; x >= x2; x--)
                {
                    grid[(x, y1)] = (1, time++);
                }
        }

        void PaintPath(Grid grid, string path)
        {
            var pathGrid = new Grid();
            var movements = path.Split(',');
            int x1 = 0, y1 = 0;
            int time = 0;

            foreach (var movement in movements)
            {
                int x2 = x1, y2 = y1;
                int distance = int.Parse(movement.Substring(1));
                switch (movement[0])
                {
                    case 'U':
                        y2 += distance;
                        PaintLine(pathGrid, x1, y1, x2, y2, ref time);
                        break;
                    case 'D':
                        y2 -= distance;
                        PaintLineReverse(pathGrid, x1, y1, x2, y2, ref time);
                        break;
                    case 'L':
                        x2 -= distance;
                        PaintLineReverse(pathGrid, x1, y1, x2, y2, ref time);
                        break;
                    case 'R':
                        x2 += distance;
                        PaintLine(pathGrid, x1, y1, x2, y2, ref time);
                        break;
                    default:
                        Oh.Bugger();
                        break;
                }
                x1 = x2;
                y1 = y2;
                time--;
            }

            foreach (var key in pathGrid.Keys)
            {
                var (count, t) = grid.GetOrDefault(key, (0, 0));
                grid[key] = (count + 1, pathGrid[key].time + t);
            }

            grid[(0, 0)] = (0, 0);
        }

        [Theory]
        [InlineData("Data/Day03-example.txt", 159, 610)]
        [InlineData("Data/Day03-example2.txt", 135, 410)]
        [InlineData("Data/Day03-example3.txt", 6, 30)]
        [InlineData("Data/Day03.txt", 896, 16524)]
        public void Problem(string input, int answer1, int answer2)
        {
            var grid = new Grid();
            FileIterator.ForEachLine<string>(input, path => PaintPath(grid, path));

            var crosses = grid.Keys.Where(k => grid[k].count > 1).ToList();
            var nearest = crosses.Select(k => Math.Abs(k.Item1) + Math.Abs(k.Item2)).Min();
            nearest.Should().Be(answer1);

            var shortest = crosses.Select(c => grid[c].time).Min();
            shortest.Should().Be(answer2);
        }
    }
}
