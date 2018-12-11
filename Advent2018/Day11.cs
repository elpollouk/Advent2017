using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Xunit;

namespace Advent2018
{
    public class Day11
    {
        class Cell
        {
            public readonly int PowerLevel;
            public int AreaPowerLevel;

            public Cell(int powerLevel)
            {
                PowerLevel = powerLevel;
                AreaPowerLevel = powerLevel;
            }

            public override string ToString() => $"Power={PowerLevel}, Area={AreaPowerLevel}";
        };

        int GetRackId(int x) => x + 10;

        int GetHundreds(int value) => (value / 100) % 10;
        int GetPowerLevel(int serialNumber, int x, int y)
        {
            var rackId = GetRackId(x);
            var powerLevel = ((rackId * y) + serialNumber) * rackId;
            return GetHundreds(powerLevel) - 5;
        }

        int AreaSum(int[,] grid, int offsetX, int offsetY, int size)
        {
            var sum = 0;
            for (var y = 0; y < size; y++)
                for (var x = 0; x < size; x++)
                    sum += grid[offsetX + x, offsetY + y];

            return sum;
        }

        (int x, int y, int value) GetHighestPowerValue(int serialNumber, int squareSize)
        {
            const int GridSize = 300;
            var grid = new int[GridSize, GridSize];
            foreach (var (x, y) in grid.Rectangle())
                grid[x, y] = GetPowerLevel(serialNumber, x + 1, y + 1);

            var maxPower = int.MinValue;
            var maxX = 0;
            var maxY = 0;

            for (var y = 0; y < GridSize - squareSize; y++)
            {
                for (var x = 0; x < GridSize - squareSize; x++)
                {
                    var power = AreaSum(grid, x, y, squareSize);
                    if (maxPower < power)
                    {
                        maxPower = power;
                        maxX = x;
                        maxY = y;
                    }
                }
            }

            return (maxX + 1, maxY + 1, maxPower);
        }

        (int x, int y, int size) GetHighestPowerValueAndSize(int serialNumber)
        {
            int maxPower = int.MinValue;
            int maxX = 0;
            int maxY = 0;
            int maxSize = 0;

            for (var size = 300; size >= 3; size--)
            {
                var (x, y, power) = GetHighestPowerValue(serialNumber, size);
                if (maxPower < power)
                {
                    maxPower = power;
                    maxX = x;
                    maxY = y;
                    maxSize = size;
                }
            }
            return (maxX, maxY, maxSize);
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
        void GetHighestPowerValue3x3_Test(int serialNumber, int expectedX, int expectedY, int expectedPower) => GetHighestPowerValue(serialNumber, 3).Should().Be((expectedX, expectedY, expectedPower));

        //[Theory]
        //[InlineData(18, 90, 269, 16)]
        //void GetHighersPowerValueAny_Test(int serialNumber, int expectedX, int expectedY, int expectedSize) => GetHighestPowerValueAndSize(serialNumber).Should().Be((expectedX, expectedY, expectedSize));
    }
}
