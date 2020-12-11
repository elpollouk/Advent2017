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
                if (inputGrid.GetNeighbours(x, y).Where(c => c == Cell.TAKEN_SEAT).Count() == 0)
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
                if (CastNeighbours(inputGrid, x, y).Where(c => c == Cell.TAKEN_SEAT).Count() == 0)
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

        Cell Cast(Cell[,] grid, int x, int y, int dX, int dY)
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

        IEnumerable<Cell> CastNeighbours(Cell[,] grid, int x, int y)
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

        Cell[,] LoadGrid(string input) => FileIterator.LoadGrid(input, (c, x, y) =>
        {
            switch (c)
            {
                case '.': return Cell.FLOOR;
                case 'L': return Cell.EMPTY_SEAT;
                case '#': return Cell.TAKEN_SEAT;
                default: throw new Expletive("Bummer");
            }
        });

        int Solve(string input, Func<Cell[,], Cell[,], int, int, bool> solver)
        {
            var inputGrid = LoadGrid(input);
            var outputGrid = new Cell[inputGrid.GetLength(0), inputGrid.GetLength(1)];

            var hasChanged = true;
            //var numRounds = 0;
            while (hasChanged)
            {
                //numRounds++;
                hasChanged = false;
                foreach (var (x, y) in inputGrid.Rectangle())
                    hasChanged |= solver(inputGrid, outputGrid, x, y);

                var t = inputGrid;
                inputGrid = outputGrid;
                outputGrid = t;
            }

            var count = 0;
            foreach (var cell in inputGrid)
                if (cell == Cell.TAKEN_SEAT)
                    count++;

            return count;
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
