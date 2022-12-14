using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2022
{
    public class Day14
    {
        enum Cell
        {
            NONE,
            ROCK,
            SAND
        }

        IEnumerable<XY> Coords(string line)
        {
            var parts = line.Split(" -> ");
            foreach (var part in parts)
            {
                var xy = part.Split(',');
                yield return (int.Parse(xy[0]), int.Parse(xy[1]));
            }
        }

        (XY, XY) GetExtents(string filename)
        {
            XY min = (int.MaxValue, 0);
            XY max = (int.MinValue, int.MinValue);

            foreach (var line in FileIterator.Lines(filename))
            {
                foreach (var coord in Coords(line))
                {
                    if (coord.x < min.x) min.x = coord.x;
                    if (coord.x > max.x) max.x = coord.x;
                    if (coord.y > max.y) max.y = coord.y;
                }
            }

            return (min, max);
        }

        Cell[,] LoadCavern(string filename, XY min, XY max)
        {
            Cell[,] grid = new Cell[max.x - min.x + 1, max.y - min.y + 1];

            foreach (var line in FileIterator.Lines(filename))
            {
                var path = Coords(line)
                    .Select(coord => coord.Sub(min))
                    .ToArray();
                grid.DrawPath(path, Cell.ROCK);
            }

            return grid;
        }

        bool DropGrain(Cell[,] grid, XY pos)
        {
            XY next;

            while (true)
            {
                if (pos.y == grid.GetLength(1) - 1)
                    return false;

                next = pos.Clone().Add(0, 1);
                if (grid[next.x, next.y] != Cell.NONE) break;
                pos = next;
            }

            next = pos.Clone().Add(-1, 1);
            if (next.x < 0) return false;
            if (grid[next.x, next.y] == Cell.NONE) return DropGrain(grid, next);
            next.Add(2, 0);
            if (next.x == grid.GetLength(0)) return false;
            if (grid[next.x, next.y] == Cell.NONE) return DropGrain(grid, next);

            grid.Plot(pos, Cell.SAND);

            return true;
        }

        void Render(Cell[,] grid, string filename, string tag)
        {
            try
            {
                filename = Path.GetFileNameWithoutExtension(filename);
                Renderer.RenderGrid($"C:/temp/aoc/2022/{tag}_{filename}.png", grid, c => c switch
                {
                    Cell.NONE => Renderer.Black,
                    Cell.ROCK => Renderer.White,
                    Cell.SAND => Renderer.Yellow,
                    _ => throw new Exception()
                });
            }
            catch
            {
                Debug.WriteLine("Unable to save image");
            }
        }

        [Theory]
        [InlineData("Data/Day14_Test.txt", 24)]
        [InlineData("Data/Day14.txt", 1003)]
        public void Part1(string filename, int expectedAnswer)
        {
            XY start = (500, 0);
            var (min, max) = GetExtents(filename);
            var grid = LoadCavern(filename, min, max);
            start.Sub(min);

            var count = 0;
            while (DropGrain(grid, start))
            {
                count++;
            }

            Render(grid, filename, "Part1");

            count.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day14_Test.txt", 93)]
        [InlineData("Data/Day14.txt", 25771)]
        public void Part2(string filename, int expectedAnswer)
        {
            XY start = (500, 0);
            var (min, max) = GetExtents(filename);

            max.Add(0, 2);
            min.x = 500 - max.y;
            max.x = 500 + max.y;
            start.Sub(min);

            var grid = LoadCavern(filename, min, max);
            grid.DrawLine((0, grid.GetLength(1) - 1), (grid.GetLength(0) - 1, grid.GetLength(1) - 1), Cell.ROCK);

            var count = 0;
            while (grid[start.x, start.y] == Cell.NONE)
            {
                DropGrain(grid, start);
                count++;
            }

            Render(grid, filename, "Part2");

            count.Should().Be(expectedAnswer);
        }
    }
}
