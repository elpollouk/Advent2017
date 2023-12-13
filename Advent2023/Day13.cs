using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Utils;
using Xunit;
using Xunit.Sdk;

namespace Advent2023
{
    public class Day13
    {
        char[,] LoadGrid(Func<string> reader)
        {
            List<string> gridLines = [];

            var line = reader();

            while (!string.IsNullOrEmpty(line))
            {
                gridLines.Add(line);
                line = reader();
            }

            if (gridLines.Count == 0)
                return null;

            var grid = new char[gridLines[0].Length, gridLines.Count];
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                var l = gridLines[y];
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    grid[x, y] = l[x];
                }
            }

            return grid;
        }

        IEnumerable<char[,]> LoadGrids(string filename)
        {
            var reader = FileIterator.CreateLineReader(filename);
            while (true)
            {
                var grid = LoadGrid(reader);
                if (grid == null)
                    break;

                yield return grid;
            }
        }

        bool IsHorizontalReflectiveMatch(char[,] grid, int x, int y)
        {
            int maxX = grid.GetLength(0);
            int upperX = x + 1;


            while (0 <= x && upperX < maxX)
            {
                if (grid[x, y] != grid[upperX, y]) return false;
                x--;
                upperX++;
            }

            return true;
        }

        bool IsVerticalReflectiveMatch(char[,] grid, int x, int y)
        {
            int maxY = grid.GetLength(1);
            int upperY = y + 1;


            while (0 <= y && upperY < maxY)
            {
                if (grid[x, y] != grid[x, upperY]) return false;
                y--;
                upperY++;
            }

            return true;
        }

        long GetHorizontalReflection(char[,] grid)
        {
            long reflection = -2;
            for (int x = 0; x < grid.GetLength(0) - 1; x++)
            {
                reflection = x;
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (!IsHorizontalReflectiveMatch(grid, x, y))
                    {
                        reflection = -2;
                        break;
                    }
                }
                if (reflection != -2) break;
            }

            return reflection + 1;
        }

        long GetVerticalReflection(char[,] grid)
        {
            long reflection = -2;
            for (int y = 0; y < grid.GetLength(1) - 1; y++)
            {
                reflection = y;
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    if (!IsVerticalReflectiveMatch(grid, x, y))
                    {
                        reflection = -2;
                        break;
                    }
                }
                if (reflection != -2) break;
            }

            return reflection + 1;
        }

        long FindReflection(char[,] grid)
        {
            var reflection = GetHorizontalReflection(grid);
            if (reflection != - 1)
                return reflection;

            return GetVerticalReflection(grid) * 100;
        }

        [Theory]
        [InlineData("Data/Day13_Test.txt", 405)]
        [InlineData("Data/Day13.txt", 33780)]
        public void Part1(string filename, long expectedAnswer)
        {
            long total = 0;

            foreach (var grid in LoadGrids(filename))
            {
                total += FindReflection(grid);
            }

            total.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day13_Test.txt", 0)]
        [InlineData("Data/Day13.txt", 0)]
        public void Part2(string filename, long expectedAnswer)
        {

        }
    }
}
