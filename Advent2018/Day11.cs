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

        (int x, int y) GetHighestPowerValue(int serialNumber)
        {
            var grid = new Cell[300, 300];
            foreach (var (x, y) in grid.Rectangle())
            {
                var cell = new Cell(GetPowerLevel(serialNumber, x + 1, y + 1));
                grid[x, y] = cell;
                if (x != 0 && y != 0)
                {
                    cell.AreaPowerLevel += grid[x - 1, y - 1].PowerLevel;
                    grid[x - 1, y - 1].AreaPowerLevel += cell.PowerLevel;
                }
                if (x != 0)
                {
                    cell.AreaPowerLevel += grid[x - 1, y].PowerLevel;
                    grid[x - 1, y].AreaPowerLevel += cell.PowerLevel;
                }
                if (y != 0)
                {
                    cell.AreaPowerLevel += grid[x, y - 1].PowerLevel;
                    grid[x, y - 1].AreaPowerLevel += cell.PowerLevel;
                }
                if (y != 0 &&x != grid.GetLength(0)-1)
                {
                    cell.AreaPowerLevel += grid[x + 1, y - 1].PowerLevel;
                    grid[x + 1, y - 1].AreaPowerLevel += cell.PowerLevel;
                }
            }

            var max = int.MinValue;
            var maxX = 0;
            var maxY = 0;
            foreach (var (x, y) in grid.Rectangle())
            {
                if (max < grid[x, y].AreaPowerLevel)
                {
                    max = grid[x, y].AreaPowerLevel;
                    maxX = x;
                    maxY = y;

                }
            }

            return (maxX, maxY);
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
        [InlineData(18, 33, 45)]
        [InlineData(42, 21, 61)]
        [InlineData(9306, 235, 38)]
        void GetHighestPowerValue_Test(int serialNumber, int expectedX, int expectedY) => GetHighestPowerValue(serialNumber).Should().Be((expectedX, expectedY));
    }
}
