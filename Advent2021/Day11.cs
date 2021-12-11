using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day11
    {
        static int CharToInt(char c, int x, int y) => c - '0';

        static string Flatten(int[,] grid)
        {
            StringBuilder builder = new();

            foreach (var value in grid.Items())
            {
                builder.Append((char)('0' + value));
            }

            return builder.ToString();
        }

        static void Pulse(HashSet<(int, int)> flashed, int[,] grid, (int x, int y) pos, ref long flashCount)
        {
            if (flashed.Contains(pos)) return;

            var newState = ++grid[pos.x, pos.y];
            if (newState > 9)
            {
                flashed.Add(pos);
                grid[pos.x, pos.y] = 0;
                flashCount++;
                foreach (var npos in grid.GetNeighbourPos(pos.x, pos.y))
                    Pulse(flashed, grid, npos, ref flashCount);
            }
        }

        static long Step(int[,] grid)
        {
            HashSet<(int, int)> flashed = new();
            long flashCount = 0;

            foreach (var pos in grid.Rectangle())
            {
                Pulse(flashed, grid, pos, ref flashCount);
            }

            return flashCount;
        }

        [Theory]
        [InlineData("Data/Day11_Test2.txt", 1, "3454340004500054000434543")]
        [InlineData("Data/Day11_Test2.txt", 2, "4565451115611165111545654")]
        public void Examples(string filename, int iterations, string expectedState)
        {
            var grid = FileIterator.LoadGrid<int>(filename, CharToInt);
            while (iterations --> 0)
            {
                Step(grid);
            }
            Flatten(grid).Should().Be(expectedState);
        }

        [Theory]
        [InlineData("Data/Day11.txt", 100, 1697)]
        [InlineData("Data/Day11_Test1.txt", 100, 1656)]
        public void Part1(string filename, int iterations, long expectedAnswer)
        {
            var grid = FileIterator.LoadGrid<int>(filename, CharToInt);
            long flashCount = 0;
            while (iterations --> 0)
            {
                flashCount += Step(grid);
            }
            flashCount.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day11.txt", 344)]
        [InlineData("Data/Day11_Test1.txt", 195)]
        public void Part2(string filename, long expectedAnswer)
        {
            var grid = FileIterator.LoadGrid<int>(filename, CharToInt);
            long allFlashCount = grid.GetLength(0) * grid.GetLength(1);
            long iterationCount = 0;
            long flashCount;
            do
            {
                iterationCount++;
                flashCount = Step(grid);
            }
            while (allFlashCount != flashCount);

            iterationCount.Should().Be(expectedAnswer);
        }
    }
}
