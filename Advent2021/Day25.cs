using FluentAssertions;
using Utils;
using Xunit;

namespace Advent2021
{
    public class Day25
    {
        static char GetCell(char[,] grid, (int x, int y) pos)
        {
            return grid[pos.x, pos.y] switch
            {
                'v' or '>' => grid[pos.x, pos.y],
                _ => '.'
            };
        }

        static (char[,], bool) Step(char[,] grid)
        {
            var width = grid.GetLength(0);
            var height = grid.GetLength(1);
            var nextGrid = new char[width, height];
            var changed = false;

            foreach (var pos in grid.Rectangle())
            {
                switch (grid[pos.x, pos.y])
                {
                    case '>':
                        var next = pos.x == width - 1 ? (x: 0, pos.y) : (x: pos.x + 1, pos.y);
                        if (GetCell(grid, next) != '.')
                        {
                            nextGrid[pos.x, pos.y] = '>';
                            continue;
                        }
                        nextGrid[next.x, next.y] = '>';
                        changed = true;
                        break;

                    default:
                        if (nextGrid[pos.x, pos.y] != '>') nextGrid[pos.x, pos.y] = grid[pos.x, pos.y];
                        break;
                }
            }

            grid = new char[width, height];
            foreach (var pos in nextGrid.Rectangle())
            {
                switch (nextGrid[pos.x, pos.y])
                {
                    case 'v':
                        var below = pos.y == height - 1 ? (pos.x, y: 0) : (pos.x, y: pos.y + 1);
                        if (GetCell(nextGrid, below) != '.')
                        {
                            grid[pos.x, pos.y] = 'v';
                            continue;
                        }
                        grid[below.x, below.y] = 'v';
                        changed = true;
                        break;

                    default:
                        if (grid[pos.x, pos.y] != 'v') grid[pos.x, pos.y] = nextGrid[pos.x, pos.y];
                        break;
                }
            }

            return (grid, changed);
        }

        [Theory]
        [InlineData("Data/Day25_Test.txt", 58)]
        [InlineData("Data/Day25.txt", 378)]
        public void Part1(string filename, long expectedAnswer)
        {
            var grid = FileIterator.LoadGrid(filename);
            bool changed = true;
            long count = 0;
            while (changed)
            {
                (grid, changed) = Step(grid);
                count++;
            }

            count.Should().Be(expectedAnswer);
        }
    }
}
