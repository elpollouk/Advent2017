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
        /*class Line
        {
            public readonly int x1, x2, y1, y2;
            public Line(int x1, int y1, int x2, int y2)
            {
                this.x1 = x1;
                this.y1 = y1;
                this.x2 = x2;
                this.y2 = y2;
            }

            public bool Horizontal => y1 == y2;
            public bool Vertical => x2 == x2;

            public bool Crosses(Line other, ref (int x, int y) pos)
            {
                if (Horizontal)
                {
                    if (other.Horizontal) return false;
                    if (other.y2 < y1) return false;
                    if (other.y1 > y1) return false;
                    if (other.x1 < x1) return false;
                    if (other.x1 > x2) return false;
                }
                if (Vertical && other.Vertical) return false;
            }

            public override string ToString()
            {
                return $"({x1},{y1})-({x2}.{y2})";
            }
        }

        List<Line> LoadLines(string input)
        {
            var lines = new List<Line>();
            var movements = input.Split(',');
            int x1 = 0, y1 = 0;
            foreach (var movement in movements)
            {
                int x2 = x1, y2 = y1;
                int distance = int.Parse(movement.Substring(1));
                switch (movement[0])
                {
                    case 'U':
                        y2 += distance;
                        lines.Add(new Line(x1, y2, x2, y1));
                        break;
                    case 'D':
                        y2 -= distance;
                        lines.Add(new Line(x1, y1, x2, y2));
                        break;
                    case 'L':
                        x2 -= distance;
                        lines.Add(new Line(x1, y1, x2, y2));
                        break;
                    case 'R':
                        x2 += distance;
                        lines.Add(new Line(x2, y1, x1, y2));
                        break;
                    default:
                        Oh.Bugger();
                        break;
                }
                x1 = x2;
                y1 = y2;
            }
            return lines;
        }

        (int, int) FindClosesCross(List<Line> path1, List<Line> path2)
        {
            (int x, int y) closest = (int.MaxValue, int.MaxValue);

            foreach (var line1 in path1)
            {
                foreach (var line2 in path2)
                {
                    //if (Line.x1)
                }
            }

            return closest;
        }*/

        void PaintLine(Grid grid, int x1, int y1, int x2, int y2)
        {
            if (x1 == x2)
                for (var y = y1; y <= y2; y++)
                {
                    var (count, time) = grid.GetOrDefault((x1, y), (0, 0));
                    grid[(x1, y)] = (count + 1, time);
                }
            else
                for (var x = x1; x <= x2; x++)
                {
                    var (count, time) = grid.GetOrDefault((x, y1), (0, 0));
                    grid[(x, y1)] = (count + 1, time);
                }
        }

        void PaintPath(Grid grid, string path)
        {
            var pathGrid = new Grid();
            var movements = path.Split(',');
            int x1 = 0, y1 = 0;
            foreach (var movement in movements)
            {
                int x2 = x1, y2 = y1;
                int distance = int.Parse(movement.Substring(1));
                switch (movement[0])
                {
                    case 'U':
                        y2 += distance;
                        PaintLine(pathGrid, x1, y1, x2, y2);
                        break;
                    case 'D':
                        y2 -= distance;
                        PaintLine(pathGrid, x1, y2, x2, y1);
                        break;
                    case 'L':
                        x2 -= distance;
                        PaintLine(pathGrid, x2, y1, x1, y2);
                        break;
                    case 'R':
                        x2 += distance;
                        PaintLine(pathGrid, x1, y1, x2, y2);
                        break;
                    default:
                        Oh.Bugger();
                        break;
                }
                x1 = x2;
                y1 = y2;
            }

            foreach (var key in pathGrid.Keys)
            {
                var (count, time) = grid.GetOrDefault(key, (0, 0));
                grid[key] = (count + 1, pathGrid[key].time + time);
            }

            grid[(0, 0)] = (0, 0);
        }

        [Theory]
        [InlineData("Data/Day03-example3.txt", 6)]
        [InlineData("Data/Day03-example.txt", 159)]
        [InlineData("Data/Day03-example2.txt", 135)]
        [InlineData("Data/Day03.txt", 896)]
        public void Problem1(string input, int answer)
        {
            var grid = new Grid();
            FileIterator.ForEachLine<string>(input, path => PaintPath(grid, path));

            var crosses = grid.Keys.Where(k => grid[k].count > 1).ToList();
            var nearest = crosses.Select(k => Math.Abs(k.Item1) + Math.Abs(k.Item2)).Min();
            nearest.Should().Be(answer);
        }
    }
}
