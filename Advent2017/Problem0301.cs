using FluentAssertions;
using System;
using Xunit;

namespace Advent2017
{
    public class Problem0301
    {
        struct RingInfo
        {
            public int Width;
            public int QuadrantLength;
            public int[] QuadrantAddresses;
        }

        RingInfo GetRingInfo(int cell)
        {
            int width = 1;
            while (cell > width * width)
                width += 2;

            var info = new RingInfo {
                Width = width,
                QuadrantLength = width - 1,
                QuadrantAddresses = new int[5]
            };

            info.QuadrantAddresses[4] = (width * width) + 1;
            for (var i = 3; i >= 0; i--)
                info.QuadrantAddresses[i] = info.QuadrantAddresses[i + 1] - info.QuadrantLength;

            return info;
        }

        public int Solve01(int cell)
        {
            if (cell == 1) return 0;

            var info = GetRingInfo(cell);

            // Find the quadent the cell is in
            for (var i = 0; i < 4; i++)
            {
                if (info.QuadrantAddresses[i] <= cell && cell < info.QuadrantAddresses[i + 1])
                {
                    var centerCell = info.QuadrantAddresses[i] + ((info.QuadrantLength / 2) - 1);
                    var delta = Math.Abs(cell - centerCell);
                    return delta + info.Width / 2;
                }
            }

            throw new Exception();
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(9, 2)]
        [InlineData(12, 3)]
        [InlineData(23, 2)]
        [InlineData(1024, 31)]
        public void Example01(int cell, int answer) => Solve01(cell).Should().Be(answer);

        [Fact]
        public void Solution01() => Solve01(277678).Should().Be(475);

        int SumNeighbours(int[,] grid, int x, int y)
        {
            var sum = 0;
            // Loops are for girls
            sum += grid[x - 1, y - 1];
            sum += grid[x    , y - 1];
            sum += grid[x + 1, y - 1];
            sum += grid[x + 1, y    ];
            sum += grid[x + 1, y + 1];
            sum += grid[x    , y + 1];
            sum += grid[x - 1, y + 1];
            sum += grid[x - 1, y    ];

            if (sum == 0) sum = 1;

            return sum;
        }

        [Fact]
        public void Solution02()
        {
            int target = 277678;

            int gridWidth = 25;
            int edgeLength = 1;
            int dX = 1;
            int dY = 0;
            int[,] grid = new int[gridWidth, gridWidth];

            // Starting pos
            int x = gridWidth / 2;
            int y = gridWidth / 2;

            // If something goes wrong, we'll over shoot the edge of the array and get an bounds exception
            while (true)
            {
                for (var i = 0; i < 2; i++)
                {
                    for (var j = 0; j < edgeLength; j++)
                    {
                        var sum = SumNeighbours(grid, x, y);
                        if (sum > target)
                        {
                            // And the problem was looking for the value not the cell number being written to (faceplam)
                            sum.Should().Be(279138);
                            return;
                        }
                        grid[x, y] = sum;
                        x += dX;
                        y += dY;
                    }

                    var t = dX;
                    dX = dY;
                    dY = -t;
                }

                edgeLength++;
            }
        }
    }
}
