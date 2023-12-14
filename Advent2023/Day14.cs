using FluentAssertions;
using System.Collections.Generic;
using Utils;
using Xunit;

namespace Advent2023
{
    public class Day14
    {
        void Roll(char[,] grid, int x, int y, int dX, int dY)
        {
            grid[x, y] = '.';
            while (true)
            {
                var nx = x + dX;
                var ny = y + dY;

                if (!grid.IsInBounds(nx, ny)) break;
                if (grid[nx, ny] != '.') break;

                x = nx;
                y = ny;
            }
            grid[x, y] = 'O';
        }

        void RollNorth(char[,] grid)
        {
            foreach (var ((x, y), item) in grid.Iterate())
            {
                if (item == 'O')
                {
                   Roll(grid, x, y, 0, -1);
                }
            }
        }

        void RollWest(char[,] grid)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    var item = grid[x, y];
                    if (item == 'O')
                    {
                        Roll(grid, x, y, -1, 0);
                    }
                }
            }
        }

        void RollSouth(char[,] grid)
        {
            for (int y = grid.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    var item = grid[x, y];
                    if (item == 'O')
                    {
                        Roll(grid, x, y, 0, 1);
                    }
                }
            }
        }

        void RollEast(char[,] grid)
        {
            for (int x = grid.GetLength(0) - 1; x>= 0; x--)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    var item = grid[x, y];
                    if (item == 'O')
                    {
                        Roll(grid, x, y, 1, 0);
                    }
                }
            }
        }

        void Cycle(char[,] grid)
        {
            RollNorth(grid);
            RollWest(grid);
            RollSouth(grid);
            RollEast(grid);
        }

        long CalcLoad(char[,] grid)
        {
            var maxY = grid.GetLength(1);
            long load = 0;

            foreach (var (pos, item) in grid.Iterate())
            {
                if (item == 'O')
                {
                    load += maxY - pos.y;
                }
            }

            return load;
        }

        [Theory]
        [InlineData("Data/Day14_Test.txt", 136)]
        [InlineData("Data/Day14.txt", 110090)]
        public void Part1(string filename, long expectedAnswer)
        {
            var grid = FileIterator.LoadGrid(filename);
            RollNorth(grid);
            var load = CalcLoad(grid);
            load.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("Data/Day14_Test.txt", 64)]
        [InlineData("Data/Day14.txt", 95254)]
        public void Part2(string filename, long expectedAnswer)
        {
            const long NUM_CYCLES = 1000000000;
            var grid = FileIterator.LoadGrid(filename);
            Dictionary<string, long> seen = [];
            long offset = -1;
            long cycleLength = -1;

            // Detect the cycle
            for (long i = 0; i < NUM_CYCLES; i++)
            {
                var key = grid.FlattenToString(c => c == 0 ? '.' : c);
                if (seen.TryGetValue(key, out var lastSeen))
                {
                    offset = lastSeen;
                    cycleLength = i - lastSeen;
                    break;
                }
                seen[key] = i;
                Cycle(grid);
            }

            // Calculate the remaining iterations required to align with the target number
            var remainingCycles = NUM_CYCLES - offset;
            remainingCycles %= cycleLength;

            // Finished the required number of cycles for alignment
            for (long i = 0; i < remainingCycles; i++)
            {
                Cycle(grid);
            }

            var load = CalcLoad(grid);
            load.Should().Be(expectedAnswer);
        }
    }
}
