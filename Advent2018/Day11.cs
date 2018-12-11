using FluentAssertions;
using System;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day11
    {
        int GetRackId(int x) => x + 10;

        int GetHundreds(int value) => (value / 100) % 10;

        int GetPowerLevel(int serialNumber, int x, int y)
        {
            var rackId = GetRackId(x);
            var powerLevel = ((rackId * y) + serialNumber) * rackId;
            return GetHundreds(powerLevel) - 5;
        }

        int[,] BuildGrid(int serialNumber, Func<int, int, int, int> getValue)
        {
            var grid = new int[300, 300];

            foreach (var (x, y) in grid.Rectangle())
            {
                grid[x, y] = getValue(serialNumber, x + 1, y + 1);
                if (x != 0 && y != 0)
                    grid[x, y] -= grid[x - 1, y - 1];
                if (x != 0)
                    grid[x, y] += grid[x - 1, y];
                if (y != 0)
                    grid[x, y] += grid[x, y - 1];
            }

            return grid;
        }

        int GetSum(int[,] grid, int x, int y, int size)
        {
            size--;
            long sum = 0;
            sum += grid[x + size, y + size];
            if (x != 0 && y != 0)
                sum += grid[x - 1, y - 1];
            if (x != 0)
                sum -= grid[x - 1, y + size];
            if (y != 0)
                sum -= grid[x + size, y - 1];
            return (int)sum;
        }

        (int x, int y, int value) GetHighestPowerValue(int[,] grid, int squareSize)
        {
            var maxPower = int.MinValue;
            var maxX = 0;
            var maxY = 0;

            foreach (var (x, y) in Generators.Rectangle(grid.GetLength(0) - squareSize - 1, grid.GetLength(1) - squareSize - 1))
            {
                var sum = GetSum(grid, x, y, squareSize);
                if (maxPower < sum)
                {
                    maxPower = sum;
                    maxX = x;
                    maxY = y;
                }
            }

            return (maxX + 1, maxY + 1, maxPower);
        }

        (int x, int y, int size) GetHighestPowerValueAndSize(int serialNumber)
        {
            var grid = BuildGrid(serialNumber, GetPowerLevel);

            var maxPower = int.MinValue;
            var maxX = 0;
            var maxY = 0;
            var maxSize = 0;

            foreach (var (x, y) in Generators.Rectangle(grid.GetLength(0) - 1, grid.GetLength(0) - 1))
            {
                var targetSize = Math.Min(grid.GetLength(0) - x, grid.GetLength(0) - y);
                for (var size = 1; size < targetSize; size++)
                {
                    var sum = GetSum(grid, x, y, size);
                    if (maxPower < sum)
                    {
                        maxPower = sum;
                        maxX = x;
                        maxY = y;
                        maxSize = size;
                    }
                }
            }

            return (maxX + 1, maxY + 1, maxSize);
        }

        [Theory]
        [InlineData(0, 0, 1, 2)]
        [InlineData(1, 1, 1, 2)]
        [InlineData(0, 1, 1, 2)]
        [InlineData(1, 0, 1, 2)]
        [InlineData(1, 1, 2, 8)]
        void GetSum_Test_Basic(int x, int y, int size, int expectedSum)
        {
            var grid = BuildGrid(0, (a, b, c) => 2);
            GetSum(grid, x, y, size).Should().Be(expectedSum);
        }

        [Theory]
        [InlineData(18, 32, 44, 29)]
        [InlineData(42, 20, 60, 30)]
        [InlineData(9306, 234, 37, 30)]
        void GetSum_Test_Complex(int serialNumber, int x, int y, int expectedSum)
        {
            var grid = BuildGrid(serialNumber, GetPowerLevel);
            GetSum(grid, x, y, 3).Should().Be(expectedSum);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(12, 0)]
        [InlineData(123, 1)]
        [InlineData(12345, 3)]
        [InlineData(32045, 0)]
        void GetHundreds_Test(int value, int expectedAnswer) => GetHundreds(value).Should().Be(expectedAnswer);

        [Theory]
        [InlineData(8, 3, 5, 4)]
        [InlineData(57, 122, 79, -5)]
        [InlineData(39, 217, 196, 0)]
        [InlineData(71, 101, 153, 4)]
        void GetPowerlevel_Test(int serialNumber, int x, int y, int expectedPowerLevel) => GetPowerLevel(serialNumber, x, y).Should().Be(expectedPowerLevel);

        [Theory]
        [InlineData(18, 33, 45, 29)]
        [InlineData(42, 21, 61, 30)]
        [InlineData(9306, 235, 38, 30)] // Part 1 Solution
        void GetHighestPowerValue3x3_Test(int serialNumber, int expectedX, int expectedY, int expectedPower)
        {
            var grid = BuildGrid(serialNumber, GetPowerLevel);
            GetHighestPowerValue(grid, 3).Should().Be((expectedX, expectedY, expectedPower));
        }

        [Theory]
        [InlineData(18, 90, 269, 16)]
        [InlineData(42, 232, 251, 12)]
        [InlineData(9306, 233, 146, 13)] // Part 2 Solution
        [InlineData(8772, 241, 65, 10)] // Dave's Solution
        void GetHighersPowerValueAny_Test(int serialNumber, int expectedX, int expectedY, int expectedSize) => GetHighestPowerValueAndSize(serialNumber).Should().Be((expectedX, expectedY, expectedSize));
    }
}
