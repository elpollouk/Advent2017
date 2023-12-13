using FluentAssertions;
using System;
using System.Collections.Generic;
using Utils;
using Xunit;

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

        int HorizontalErrorCount(char[,] grid, int x, int y)
        {
            int maxX = grid.GetLength(0);
            int upperX = x + 1;
            int errors = 0;

            while (0 <= x && upperX < maxX)
            {
                if (grid[x, y] != grid[upperX, y])
                {
                    errors++;
                    if (errors == 2) return 2;
                }
                x--;
                upperX++;
            }

            return errors;
        }

        int VerticalErrorCount(char[,] grid, int x, int y)
        {
            int maxY = grid.GetLength(1);
            int upperY = y + 1;
            int errors = 0;

            while (0 <= y && upperY < maxY)
            {
                if (grid[x, y] != grid[x, upperY])
                {
                    errors++;
                    if (errors == 2) return 2;
                }
                y--;
                upperY++;
            }

            return errors;
        }

        long GetHorizontalReflection(char[,] grid, int requiredErrors)
        {
            for (int x = 0; x < grid.GetLength(0) - 1; x++)
            {
                int errors = 0;
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    errors += HorizontalErrorCount(grid, x, y);
                    if (requiredErrors < errors)
                    {
                        break;
                    }
                }

                if (errors == requiredErrors)
                {
                    return x + 1;
                }
            }

            return -1;
        }

        long GetVerticalReflection(char[,] grid, int requiredErrors)
        {
            for (int y = 0; y < grid.GetLength(1) - 1; y++)
            {
                int errors = 0;
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    errors += VerticalErrorCount(grid, x, y);
                    if (requiredErrors < errors)
                    {
                        break;
                    }
                }

                if (errors == requiredErrors)
                {
                    return y + 1;
                }
            }

            throw new Exception("Reflection not found");
        }

        long FindReflection(char[,] grid, int requiredErrors)
        {
            var reflection = GetHorizontalReflection(grid, requiredErrors);
            if (reflection != - 1)
                return reflection;

            return GetVerticalReflection(grid, requiredErrors) * 100;
        }

        [Theory]
        [InlineData("Data/Day13_Test.txt", 0, 405)]
        [InlineData("Data/Day13_Test.txt", 1, 400)]
        [InlineData("Data/Day13.txt", 0, 33780)]
        [InlineData("Data/Day13.txt", 1, 23479)]
        public void Solve(string filename, int requiredErrors, long expectedAnswer)
        {
            long total = 0;

            foreach (var grid in LoadGrids(filename))
            {
                total += FindReflection(grid, requiredErrors);
            }

            total.Should().Be(expectedAnswer);
        }
    }
}
