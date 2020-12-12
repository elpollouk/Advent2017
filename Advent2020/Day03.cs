using FluentAssertions;
using Utils;
using Xunit;

namespace Advent2020
{
    public class Day03
    {
        static int HitCount(int[,] grid, int dX, int dY)
        {
            var x = 0;
            var count = 0;
            for (var y = 0; y < grid.GetLength(1); y += dY)
            {
                count += grid[x, y];
                x = (x + dX) % grid.GetLength(0);
            }

            return count;
        }

        [Theory]
        [InlineData("Data/Day03_test.txt", 7)]
        [InlineData("Data/Day03.txt", 292)]
        public void Part1(string input, int expected)
        {
            var grid = FileIterator.LoadGrid(input, (c, _, _) => c == '#' ? 1 : 0);
            HitCount(grid, 3, 1).Should().Be(expected);
        }

        [Theory]
        [InlineData("Data/Day03_test.txt", 336)]
        [InlineData("Data/Day03.txt", 9354744432)]
        public void Part2(string input, long expected)
        {
            var grid = FileIterator.LoadGrid(input, (c, x, y) => c == '#' ? 1 : 0);
            var deltas = new (int, int)[] {
                (1, 1),
                (3, 1),
                (5, 1),
                (7, 1),
                (1, 2)
            };

            var product = 1L;
            foreach (var (dX, dY) in deltas)
                product *= HitCount(grid, dX, dY);

            product.Should().Be(expected);
        }
    }
}
