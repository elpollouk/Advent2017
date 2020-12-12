using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;
using Xunit;

namespace Advent2020
{
    public class Day11
    {
        enum Cell
        {
            FLOOR,
            EMPTY_SEAT,
            TAKEN_SEAT
        }

        bool UpdateSeat1(Cell[,] inputGrid, Cell[,] outputGrid, int x, int y)
        {
            var cell = inputGrid[x, y];
            if (cell == Cell.EMPTY_SEAT)
            {
                if (!inputGrid.GetNeighbours(x, y).Where(c => c == Cell.TAKEN_SEAT).Any())
                {
                    outputGrid[x, y] = Cell.TAKEN_SEAT;
                    return true;
                }
            }
            else if (cell == Cell.TAKEN_SEAT)
            {
                if (inputGrid.GetNeighbours(x, y).Where(c => c == Cell.TAKEN_SEAT).Count() >= 4)
                {
                    outputGrid[x, y] = Cell.EMPTY_SEAT;
                    return true;
                }
            }

            outputGrid[x, y] = cell;
            return false;
        }

        bool UpdateSeat2(Cell[,] inputGrid, Cell[,] outputGrid, int x, int y)
        {
            var cell = inputGrid[x, y];
            if (cell == Cell.EMPTY_SEAT)
            {
                if (!CastNeighbours(inputGrid, x, y).Where(c => c == Cell.TAKEN_SEAT).Any())
                {
                    outputGrid[x, y] = Cell.TAKEN_SEAT;
                    return true;
                }
            }
            else if (cell == Cell.TAKEN_SEAT)
            {
                if (CastNeighbours(inputGrid, x, y).Where(c => c == Cell.TAKEN_SEAT).Count() >= 5)
                {
                    outputGrid[x, y] = Cell.EMPTY_SEAT;
                    return true;
                }
            }

            outputGrid[x, y] = cell;
            return false;
        }

        static Cell Cast(Cell[,] grid, int x, int y, int dX, int dY)
        {
            var lx = grid.GetLength(0);
            var ly = grid.GetLength(1);
            x += dX;
            y += dY;
            while ((0 <= x) && (0 <= y) && (x < lx) && (y < ly))
            {
                var c = grid[x, y];
                if (c != Cell.FLOOR) return c;
                x += dX;
                y += dY;
            }

            return Cell.FLOOR;
        }

        static IEnumerable<Cell> CastNeighbours(Cell[,] grid, int x, int y)
        {
            yield return Cast(grid, x, y, -1, -1);
            yield return Cast(grid, x, y,  0, -1);
            yield return Cast(grid, x, y,  1, -1);
            yield return Cast(grid, x, y, -1,  0);
            yield return Cast(grid, x, y,  1,  0);
            yield return Cast(grid, x, y, -1,  1);
            yield return Cast(grid, x, y,  0,  1);
            yield return Cast(grid, x, y,  1,  1);
        }

        static Cell[,] LoadGrid(string input) => FileIterator.LoadGrid(input, (c, _, _) => c switch {
            '.' => Cell.FLOOR,
            'L' => Cell.EMPTY_SEAT,
            '#' => Cell.TAKEN_SEAT,
            _ => throw new Expletive("Bummer")
        });

        static int Solve(string input, Func<Cell[,], Cell[,], int, int, bool> update)
        {
            var inputGrid = LoadGrid(input);
            var outputGrid = new Cell[inputGrid.GetLength(0), inputGrid.GetLength(1)];

            var hasChanged = true;
            while (hasChanged)
            {
                hasChanged = false;
                foreach (var (x, y) in inputGrid.Rectangle())
                    hasChanged |= update(inputGrid, outputGrid, x, y);

                (inputGrid, outputGrid) = (outputGrid, inputGrid);
            }

            return inputGrid.Items()
                .Where(c => c == Cell.TAKEN_SEAT)
                .Count();
        }

        [Theory]
        [InlineData("Data/Day11_test.txt", 37)]
        [InlineData("Data/Day11.txt", 2438)]
        public void Problem1(string input, int expected) => Solve(input, UpdateSeat1).Should().Be(expected);

        [Theory]
        [InlineData("Data/Day11_test.txt", 26)]
        [InlineData("Data/Day11.txt", 2174)]
        public void Problem2(string input, int expected) => Solve(input, UpdateSeat2).Should().Be(expected);
    }
}
