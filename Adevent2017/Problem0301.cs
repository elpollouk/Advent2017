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
    }
}
