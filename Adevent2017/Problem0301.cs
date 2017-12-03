using FluentAssertions;
using System;
using Xunit;

namespace Adevent2017
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
            sum += grid[x - 1, y - 1];
            sum += grid[x    , y - 1];
            sum += grid[x + 1, y - 1];
            sum += grid[x + 1, y    ];
            sum += grid[x + 1, y + 1];
            sum += grid[x    , y + 1];
            sum += grid[x - 1, y + 1];
            sum += grid[x - 1, y    ];
            return sum;
        }

        [Fact]
        public void Solution02()
        {
            int target = 277678;

            // Just brute force it so I can go back to drinking
            int width = 25;
            // Zero the grid
            int[,] grid = new int[width, width];
            for (var i = 0; i < width; i++)
                for (var j = 0; j < width; j++)
                    grid[i, j] = 0;

            // Starting pos
            int x = width / 2;
            int y = width / 2;

            // Explicitly set the initial position to seed everything
            grid[x, y] = 1;

            var quadrantLength = 0;

            for (var ring = 1; ring < width / 2; ring++)
            {
                var dX = 0;
                var dY = -1;
                // This is a fudge to move the current write position to a place that makes the first increment work
                x++;
                y++;
                quadrantLength += 2;

                for (var quadrant = 0; quadrant < 4; quadrant++)
                {
                    for (var i = 0; i < quadrantLength; i++)
                    {
                        x += dX;
                        y += dY;

                        grid[x, y] = SumNeighbours(grid, x, y);
                        if (grid[x, y] > target)
                        {
                            // And the problem was looking for the value not the cell number being written to (faceplam)
                            grid[x, y].Should().Be(279138);
                            return;
                        }
                    }

                    // Update the deltas for the next quadrant
                    if (dY == -1)
                    {
                        dX = -1;
                        dY = 0;
                    }
                    else if (dX == -1)
                    {
                        dX = 0;
                        dY = 1;
                    }
                    else if (dY == 1)
                    {
                        dX = 1;
                        dY = 0;
                    }
                    
                    // No need to do anything after the last quadrant as we explicitly set dX/dY at the start of the next ring
                }
            }

            throw new Exception();
        }
    }
}
